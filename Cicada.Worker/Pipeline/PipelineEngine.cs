using Cicada.Domain.Entities;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cicada.Biz.Plugins
{
    public class PipelineEngine
    {
        private readonly IReadOnlyList<IPipelineModule> _modules;

        public PipelineEngine(IEnumerable<IPipelineModule> modules)
        {
            // 固化执行顺序（DI 注册顺序）
            _modules = modules.ToList();

            Log.Information("🧩 Pipeline initialized with modules: {Modules}",
                string.Join(", ", _modules.Select(m => m.GetType().Name)));
        }

        public async Task ExecuteAsync(Telemetry data, CancellationToken token = default)
        {
            // 👉 顺序执行（工业系统常用：可控、可预测）
            foreach (var module in _modules)
            {
                try
                {
                    await module.ProcessAsync(data, token);
                }
                catch (Exception ex)
                {
                    // ❗ 单个模块失败，不影响整个 Pipeline
                    Log.Error(ex,
                        "❌ Pipeline module failed: {Module} | Data: {@Telemetry}",
                        module.GetType().Name,
                        data);
                }
            }
        }

        //private readonly IEnumerable<IPipelineModule> _modules;

        //public PipelineEngine(IEnumerable<IPipelineModule> modules)
        //{
        //    _modules = modules;
        //}

        //public async Task ExecuteAsync(Telemetry data)
        //{
        //    foreach (var module in _modules)
        //    {
        //        await module.ProcessAsync(data);
        //    }
        //}
    }
}
