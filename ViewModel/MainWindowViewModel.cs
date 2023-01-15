using System;
using System.Windows;
using System.Windows.Threading;
using Ironclad.Helper;
using ServiceStack;

namespace Ironclad.ViewModel
{
    public class MainWindowViewModel : BindableBase
    {
        public int LogEventCount { get; private set; }
        public int LogEventCountSlow { get; private set; }
        private string _logText;
        private string _scriptSize;
        private string _monitorCount;
        private string _eotwait;
        private string _featuresCount;
        public string LogText
        {
            get => _logText;
            private set => SetProperty(ref _logText, value);
        }
        public string ScriptSize
        {
            get => _scriptSize;
            private set => SetProperty(ref _scriptSize, value);
        }
        public string MonitorCount
        {
            get => _monitorCount;
            private set => SetProperty(ref _monitorCount, value);
        }
        public string FeaturesCount
        {
            get => _featuresCount;
            private set => SetProperty(ref _featuresCount, value);
        }
        public string EOTWait
        {
            get => _eotwait;
            private set => SetProperty(ref _eotwait, value);
        }

        public MainWindowViewModel()
        {

            LogText = "";
            ScriptSize = "";
            MonitorCount = "";
            FeaturesCount = "";
            EOTWait = "";
            var dispatcherTimer = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(3)
            };
            var dispatcherTimerSlow = new DispatcherTimer
            {
                Interval = TimeSpan.FromSeconds(10)
            };

            dispatcherTimer.Tick += (object sender, EventArgs e) =>
            {
                LogEventCount++;
                LogText = System.IO.File.ReadAllText(Hardcoded.IRONCLADLOG);
            };
            dispatcherTimer.Start();
            dispatcherTimerSlow.Tick += (object sender, EventArgs e) =>
            {
                LogEventCountSlow++;
                ScriptSize = $"Size: {FO.GetSizeMB(Hardcoded.CAMPAIGN)} MB";
                var cntMonitors = FO.CountWordInFile(Hardcoded.CAMPAIGN, "end_monitor");
                MonitorCount = $"Monitors: {cntMonitors}";
                FeaturesCount = $"Features: {MainWindow.FeatureCount}";
                EOTWait = $"EOT Wait: {Math.Round(cntMonitors * Settings.WaitTimeEndTurnSecondsPerMonitor)} s";
            };
            dispatcherTimerSlow.Start();

        }

    }
}
