using Cicada.Domain.Entities;
using Cicada.Domain.Interfaces;
using Serilog;

namespace Cicada.UI
{
    public partial class MainForm : Form
    {
        public MainForm(ITelemetryRepository repo)
        {
            InitializeComponent();

            Log.Information("Application started");

            // _ = Test(repo);
        }

        private async Task Test(ITelemetryRepository repo)
        {
            await repo.SaveAsync(new Telemetry
            {
                DeviceId = "Device1",
                Tag = "Temperature",
                Value = 25.5,
                Timestamp = DateTime.UtcNow
            });
        }
    }
}
