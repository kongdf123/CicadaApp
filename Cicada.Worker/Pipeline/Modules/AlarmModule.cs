using Cicada.Biz.Services;
using Cicada.Domain.Entities;
using Cicada.Domain.Models; 
using Serilog;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cicada.Biz.Plugins
{
    public class AlarmModule : IPipelineModule
    {
        private readonly AlarmEventBus _bus;
        private readonly Dictionary<string, AlarmRule> _rules;
        private readonly ConcurrentDictionary<string, AlarmState> _states;

        public AlarmModule(AlarmEventBus bus)
        {
            _bus = bus;
            _states = new ConcurrentDictionary<string, AlarmState>();

            // 👉 可以后续改成从配置文件加载
            _rules = new Dictionary<string, AlarmRule>
            {
                ["Temperature"] = new AlarmRule
                {
                    Tag = "Temperature",
                    WarningThreshold = 70,
                    CriticalThreshold = 85,
                    CooldownSeconds = 10
                },
                ["Pressure"] = new AlarmRule
                {
                    Tag = "Pressure",
                    WarningThreshold = 50,
                    CriticalThreshold = 70,
                    CooldownSeconds = 10
                }
            };
        }

        public Task ProcessAsync(Telemetry data, CancellationToken token = default)
        {
            if (!_rules.TryGetValue(data.Tag, out var rule))
                return Task.CompletedTask;

            var level = EvaluateLevel(data.Value, rule);

            var key = $"{data.DeviceId}:{data.Tag}";
            var state = _states.GetOrAdd(key, _ => new AlarmState());

            // =========================
            // 1. 状态没有变化 → 不处理
            // =========================
            if (level == state.CurrentLevel)
                return Task.CompletedTask;

            // =========================
            // 2. 冷却时间控制（防止频繁触发）
            // =========================
            var now = DateTime.UtcNow;
            if ((now - state.LastTriggerTime).TotalSeconds < rule.CooldownSeconds)
                return Task.CompletedTask;

            // =========================
            // 3. 更新状态
            // =========================
            var previousLevel = state.CurrentLevel;
            state.CurrentLevel = level;
            state.LastTriggerTime = now;

            // =========================
            // 4. 触发事件（进入或恢复）
            // =========================
            HandleAlarmTransition(data, previousLevel, level);

            return Task.CompletedTask;
        }

        private AlarmLevel EvaluateLevel(double value, AlarmRule rule)
        {
            if (value >= rule.CriticalThreshold)
                return AlarmLevel.Critical;

            if (value >= rule.WarningThreshold)
                return AlarmLevel.Warning;

            return AlarmLevel.None;
        }

        private void HandleAlarmTransition(Telemetry data, AlarmLevel prev, AlarmLevel current)
        {
            var message = $"{data.DeviceId} | {data.Tag} = {data.Value:F2}";

            var alarm = new AlarmEvent
            {
                DeviceId = data.DeviceId,
                Tag = data.Tag,
                Value = data.Value,
                Level = current,
                Timestamp = DateTime.UtcNow,
                Status = current == AlarmLevel.None ? AlarmStatus.Recovered : AlarmStatus.Active
            };
            _bus.Publish(alarm);

            // 恢复
            if (current == AlarmLevel.None && prev != AlarmLevel.None)
            {
                Log.Information($"✅ RECOVERED: {message}");
                return;
            }

            // 升级报警
            if (current == AlarmLevel.Warning)
            {
                Log.Warning($"⚠ WARNING: {message}");
            }
            else if (current == AlarmLevel.Critical)
            {
                Log.Error($"🔥 CRITICAL: {message}");
            }
        }
    }

    //public class AlarmModule : IPipelineModule
    //{
    //    private readonly AlarmService _alarm;

    //    public AlarmModule(AlarmService alarm)
    //    {
    //        _alarm = alarm;
    //    }

    //    public Task ProcessAsync(Telemetry data)
    //    {
    //        _alarm.Evaluate(data);
    //        return Task.CompletedTask;
    //    }
    //}
}
