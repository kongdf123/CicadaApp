namespace Cicada.UI
{
    partial class MainForm
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            LiveChartsCore.SkiaSharpView.SKCharts.SKDefaultLegend skDefaultLegend1 = new LiveChartsCore.SkiaSharpView.SKCharts.SKDefaultLegend();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            LiveChartsCore.Drawing.Padding padding1 = new LiveChartsCore.Drawing.Padding();
            LiveChartsCore.SkiaSharpView.SKCharts.SKDefaultTooltip skDefaultTooltip1 = new LiveChartsCore.SkiaSharpView.SKCharts.SKDefaultTooltip();
            LiveChartsCore.Drawing.Padding padding2 = new LiveChartsCore.Drawing.Padding();
            lblValue = new Label();
            lblTime = new Label();
            lstbTelemetry = new ListBox();
            cartesianChart1 = new LiveChartsCore.SkiaSharpView.WinForms.CartesianChart();
            lstAlarms = new ListBox();
            SuspendLayout();
            // 
            // lblValue
            // 
            lblValue.AutoSize = true;
            lblValue.Location = new Point(1099, 70);
            lblValue.Name = "lblValue";
            lblValue.Size = new Size(59, 25);
            lblValue.TabIndex = 0;
            lblValue.Text = "label1";
            // 
            // lblTime
            // 
            lblTime.AutoSize = true;
            lblTime.Location = new Point(1293, 70);
            lblTime.Name = "lblTime";
            lblTime.Size = new Size(59, 25);
            lblTime.TabIndex = 1;
            lblTime.Text = "label2";
            // 
            // lstbTelemetry
            // 
            lstbTelemetry.FormattingEnabled = true;
            lstbTelemetry.Location = new Point(1099, 123);
            lstbTelemetry.Name = "lstbTelemetry";
            lstbTelemetry.Size = new Size(610, 279);
            lstbTelemetry.TabIndex = 2;
            // 
            // cartesianChart1
            // 
            cartesianChart1.AutoUpdateEnabled = true;
            cartesianChart1.ChartTheme = null;
            skDefaultLegend1.AnimationsSpeed = TimeSpan.Parse("00:00:00.1500000");
            skDefaultLegend1.Content = null;
            skDefaultLegend1.IsValid = false;
            skDefaultLegend1.Opacity = 1F;
            padding1.Bottom = 0F;
            padding1.Left = 0F;
            padding1.Right = 0F;
            padding1.Top = 0F;
            skDefaultLegend1.Padding = padding1;
            skDefaultLegend1.RemoveOnCompleted = false;
            skDefaultLegend1.RotateTransform = 0F;
            skDefaultLegend1.X = 0F;
            skDefaultLegend1.Y = 0F;
            cartesianChart1.Legend = skDefaultLegend1;
            cartesianChart1.Location = new Point(1099, 492);
            cartesianChart1.MatchAxesScreenDataRatio = false;
            cartesianChart1.Name = "cartesianChart1";
            cartesianChart1.Size = new Size(500, 225);
            cartesianChart1.TabIndex = 3;
            skDefaultTooltip1.AnimationsSpeed = TimeSpan.Parse("00:00:00.1500000");
            skDefaultTooltip1.Content = null;
            skDefaultTooltip1.IsValid = false;
            skDefaultTooltip1.Opacity = 1F;
            padding2.Bottom = 0F;
            padding2.Left = 0F;
            padding2.Right = 0F;
            padding2.Top = 0F;
            skDefaultTooltip1.Padding = padding2;
            skDefaultTooltip1.RemoveOnCompleted = false;
            skDefaultTooltip1.RotateTransform = 0F;
            skDefaultTooltip1.Wedge = 10;
            skDefaultTooltip1.X = 0F;
            skDefaultTooltip1.Y = 0F;
            cartesianChart1.Tooltip = skDefaultTooltip1;
            cartesianChart1.TooltipFindingStrategy = LiveChartsCore.Measure.TooltipFindingStrategy.Automatic;
            cartesianChart1.UpdaterThrottler = TimeSpan.Parse("00:00:00.0500000");
            // 
            // lstAlarms
            // 
            lstAlarms.FormattingEnabled = true;
            lstAlarms.Location = new Point(92, 123);
            lstAlarms.Name = "lstAlarms";
            lstAlarms.Size = new Size(842, 279);
            lstAlarms.TabIndex = 4;
            // 
            // MainForm
            // 
            AutoScaleDimensions = new SizeF(10F, 25F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1816, 856);
            Controls.Add(lstAlarms);
            Controls.Add(cartesianChart1);
            Controls.Add(lstbTelemetry);
            Controls.Add(lblTime);
            Controls.Add(lblValue);
            Name = "MainForm";
            Text = "Main Form";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label lblValue;
        private Label lblTime;
        private ListBox lstbTelemetry;
        private LiveChartsCore.SkiaSharpView.WinForms.CartesianChart cartesianChart1;
        private ListBox lstAlarms;
    }
}
