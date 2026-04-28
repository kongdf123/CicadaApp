using Cicada.Biz.Plugins;
using Cicada.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cicada.Biz.Services
{
    public class TelemetryConsumer
    {
        private readonly TelemetryChannel _channel;
        private readonly ITelemetryRepository _repo;
        private readonly TelemetryUiDispatcher _dispatcher;
        private readonly AlarmService _alarmService;
        private readonly PipelineEngine _engine;
        //private readonly PluginEngine _engine;

        public TelemetryConsumer(
            TelemetryChannel channel,
            ITelemetryRepository repo,
            TelemetryUiDispatcher dispatcher,
            AlarmService alarmService
             )
        {
            _channel = channel;
            _repo = repo;
            _dispatcher = dispatcher;
            _alarmService = alarmService;  
        }

        public async Task StartAsync(CancellationToken token)
        {
            //await foreach (var data in _channel.Reader.ReadAllAsync(token))
            //{
            //    await _engine.ExecuteAsync(data);
            //}
            await foreach (var item in _channel.Reader.ReadAllAsync(token))
            {
                await _repo.SaveAsync(item);

                // 👉 统一在 consumer 广播
                _dispatcher.Publish(item);

                // 👉 新增：告警判断
                _alarmService.Evaluate(item);
                // 👉 后面可以扩展：
                // - 报警
                // - UI 推送
                // - 缓存
            }
        }
    }
}
