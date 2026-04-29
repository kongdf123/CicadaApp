using Cicada.Domain.Entities;
using Cicada.UI.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Cicada.UI
{
    public partial class AlarmForm : Form
    {
        private readonly AlarmUiDispatcher _dispatcher;

        private readonly Dictionary<string, AlarmEvent> _alarms = new();

        public AlarmForm(AlarmUiDispatcher dispatcher)
        {
            InitializeComponent();
            _dispatcher = dispatcher;

            InitGrid();

            _dispatcher.OnAlarm += HandleAlarm;

        }

        private void InitGrid()
        {
            dataGridView1.Columns.Add("Device", "Device");
            dataGridView1.Columns.Add("Tag", "Tag");
            dataGridView1.Columns.Add("Value", "Value");
            dataGridView1.Columns.Add("Level", "Level");
            dataGridView1.Columns.Add("Status", "Status");
            dataGridView1.Columns.Add("Time", "Time");

            dataGridView1.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        private void HandleAlarm(AlarmEvent alarm)
        {
            if (InvokeRequired)
            {
                Invoke(new Action(() => HandleAlarm(alarm)));
                return;
            }

            _alarms[alarm.Key] = alarm;

            RefreshGrid();
        }

        private void RefreshGrid()
        {
            dataGridView1.Rows.Clear();

            foreach (var alarm in _alarms.Values.OrderByDescending(a => a.Timestamp))
            {
                var rowIndex = dataGridView1.Rows.Add(
                    alarm.DeviceId,
                    alarm.Tag,
                    alarm.Value.ToString("F2"),
                    alarm.Level,
                    alarm.Status,
                    alarm.Timestamp.ToString("HH:mm:ss")
                );

                var row = dataGridView1.Rows[rowIndex];

                // 🎨 颜色
                if (alarm.Level == AlarmLevel.Critical)
                    row.DefaultCellStyle.BackColor = Color.Red;
                else if (alarm.Level == AlarmLevel.Warning)
                    row.DefaultCellStyle.BackColor = Color.Yellow;

                if (alarm.Status == AlarmStatus.Recovered)
                    row.DefaultCellStyle.BackColor = Color.LightGray;
            }
        }

        private void btnAck_Click(object sender, EventArgs e)
        {
            if (dataGridView1.SelectedRows.Count == 0)
                return;

            var device = dataGridView1.SelectedRows[0].Cells[0].Value.ToString();
            var tag = dataGridView1.SelectedRows[0].Cells[1].Value.ToString();

            var key = $"{device}:{tag}";

            if (_alarms.TryGetValue(key, out var alarm))
            {
                alarm.Status = AlarmStatus.Acknowledged;
                RefreshGrid();
            }
        }

    }
}
