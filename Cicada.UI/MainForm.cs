using Cicada.Biz.Services;
using Cicada.Domain.Entities;
using Cicada.Domain.Interfaces;
using Cicada.UI.Models;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Microsoft.AspNetCore.SignalR.Client;
using Serilog;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Cicada.UI
{
    public partial class MainForm : Form
    {
        private readonly List<ChartPoint> chartPoints = new List<ChartPoint>();
        private readonly ObservableCollection<double> _values = new ObservableCollection<double>();
        private readonly AlarmService _alarmService;

        private readonly TelemetryUiDispatcher _dispatcher;
        public MainForm(ITelemetryRepository repo, TelemetryUiDispatcher dispatcher, AlarmService alarmService)
        {
            InitializeComponent();

            Log.Information("Application started");

            _alarmService = alarmService;
            SetupAlarm(); 

            _dispatcher = dispatcher;
            _dispatcher.OnData += HandleTelemetry;
            // _ = Test(repo);

            SetupChart();

            //var connection = new HubConnectionBuilder()
            //    .WithUrl("http://localhost:5000/telemetryHub")
            //    .Build();
            //connection.On<Telemetry>("telemetry", data =>
            //{
            //    if (InvokeRequired)
            //    {
            //        Invoke(() => HandleTelemetry(data));
            //        return;
            //    }

            //    HandleTelemetry(data);
            //});
        }

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ISeries[] Series { get; set; }
        private void SetupChart()
        {
            Series = new ISeries[]
            {
                new LineSeries<double>
                {
                    Values = _values,
                    GeometrySize = 0
                }
            };

            cartesianChart1.Series = Series;
            cartesianChart1.XAxes = new[]
            {
                new Axis
                {
                    Name = "Time",
                    LabelsRotation = 0,

                    // 👉 optional: show last N labels only
                    Labeler = value => DateTime.Now
                        .AddSeconds(value - _values.Count)
                        .ToString("HH:mm:ss")
                }
            };

            cartesianChart1.YAxes = new[]
            {
                new Axis
                {
                    Name = "Temperature (°C)",
                    MinLimit = 0,
                    MaxLimit = 120
                }
            };
        }

        private void SetupAlarm()
        { 
            _alarmService.OnAlarm += alarm =>
            {
                if (InvokeRequired)
                {
                    Invoke(new Action(() => AddAlarm(alarm)));
                    return;
                }

                AddAlarm(alarm);
            };
        }

        private void AddAlarm(Alarm alarm)
        {
            lstAlarms.Items.Insert(0,
                $"{alarm.Timestamp:HH:mm:ss} | {alarm.DeviceId} | {alarm.Tag} | {alarm.Value:F2} | {alarm.Level}");
        }

        private DateTime _lastUpdate = DateTime.MinValue;
        private void HandleTelemetry(Telemetry data)
        {
            //if ((DateTime.Now - _lastUpdate).TotalMilliseconds < 50) // 50ms 更新一次 UI，避免过于频繁 This throttling is intentional, but it means that if multiple telemetry events arrive within 200ms, only the first will update the UI. The rest will be ignored.
            //    return;

            _lastUpdate = DateTime.Now;

            if (InvokeRequired)
            {
                Invoke(new Action(() => HandleTelemetry(data)));
                return;
            }
            // 👉 这里可以根据 Tag 映射到不同的 UI 控件
            //lblDeviceId.Text = data.DeviceId;
            //lblTag.Text = data.Tag;
            //lblValue.Text = data.Value.ToString();
            //lblTimestamp.Text = data.Timestamp.ToString("HH:mm:ss");
            lblValue.Text = $"{data.Tag}: {data.Value:F2}";
            lblTime.Text = data.Timestamp.ToString("HH:mm:ss.fff");

            lstbTelemetry.Items.Insert(0, $"{data.Timestamp:HH:mm:ss} | {data.Tag} = {data.Value:F2}");

            // Chart update
            UpdateChart(data.Value);
        }

        private const int MaxPoints = 50;
        private DateTime _lastChartUpdate = DateTime.MinValue;
        private void UpdateChart(double value)
        {
            if ((DateTime.Now - _lastChartUpdate).TotalMilliseconds < 200)
                return;

            _lastChartUpdate = DateTime.Now;

            _values.Add(value);

            // 👉 控制内存（滑动窗口）
            if (_values.Count > MaxPoints)
                _values.RemoveAt(0);
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
