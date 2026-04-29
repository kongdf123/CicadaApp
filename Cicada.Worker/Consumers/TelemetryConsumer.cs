using Cicada.Biz.Plugins;
using Cicada.Domain.Interfaces;
using Serilog;
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
        private readonly PipelineEngine _pipeline;

        private long _processedCount = 0;

        public TelemetryConsumer(
            TelemetryChannel channel,
            PipelineEngine pipeline)
        {
            _channel = channel;
            _pipeline = pipeline;
        }

        public async Task StartAsync(CancellationToken token)
        {
            Log.Information("📥 TelemetryConsumer started");

            try
            {
                await foreach (var data in _channel.Reader.ReadAllAsync(token))
                {
                    try
                    {
                        // =========================
                        // 1. Execute Pipeline
                        // =========================
                        await _pipeline.ExecuteAsync(data);

                        // =========================
                        // 2. Metrics
                        // =========================
                        _processedCount++;

                        if (_processedCount % 100 == 0)
                        {
                            Log.Information($"📊 Processed: {_processedCount} messages");
                        }
                    }
                    catch (Exception ex)
                    {
                        // =========================
                        // ❗ 不要让单条数据拖垮整个系统
                        // =========================
                        Log.Error(ex, "❌ Error processing telemetry: {@Telemetry}", data);
                    }
                }
            }
            catch (OperationCanceledException)
            {
                Log.Warning("⚠ TelemetryConsumer canceled");
            }
            catch (Exception ex)
            {
                Log.Fatal(ex, "💥 TelemetryConsumer crashed");
            }

            Log.Information("🛑 TelemetryConsumer stopped");
        }
        //private readonly TelemetryChannel _channel;
        //private readonly ITelemetryRepository _repo;
        //private readonly TelemetryUiDispatcher _dispatcher;
        //private readonly AlarmService _alarmService;
        ////private readonly PipelineEngine _engine;
        ////private readonly PluginEngine _engine;

        //public TelemetryConsumer(
        //    TelemetryChannel channel,
        //    ITelemetryRepository repo,
        //    TelemetryUiDispatcher dispatcher,
        //    AlarmService alarmService
        //     )
        //{
        //    _channel = channel;
        //    _repo = repo;
        //    _dispatcher = dispatcher;
        //    _alarmService = alarmService;  
        //}

        //public async Task StartAsync(CancellationToken token)
        //{
        //    //await foreach (var data in _channel.Reader.ReadAllAsync(token))
        //    //{
        //    //    await _engine.ExecuteAsync(data);
        //    //}
        //    await foreach (var item in _channel.Reader.ReadAllAsync(token))
        //    {
        //        await _repo.SaveAsync(item);

        //        // 👉 统一在 consumer 广播
        //        _dispatcher.Publish(item);

        //        // 👉 新增：告警判断
        //        _alarmService.Evaluate(item);
        //        // 👉 后面可以扩展：
        //        // - 报警
        //        // - UI 推送
        //        // - 缓存
        //    }
        //}
    }
}
