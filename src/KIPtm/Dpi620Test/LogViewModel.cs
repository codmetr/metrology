using NLog;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Threading;
using NLog.Common;
using NLog.Config;
using NLog.Targets;

namespace Dpi620Test
{

    public class LogViewModel: INotifyPropertyChanged
    {
        /// <summary>
        /// Лог сообщений
        /// </summary>
        public ObservableCollection<LogMessage> Log { get; set; } = new ObservableCollection<LogMessage>();
        private readonly Dispatcher _disp;

        private readonly int _maxLen;

        static LogViewModel()
        {
            //target
            //ConfigurationItemFactory.Default.Targets
            //    .RegisterDefinition("LogView", typeof(LogViewModel));
        }

        public LogViewModel(Dispatcher disp, int maxLen = 50)
        {
            _disp = disp;
            this._maxLen = maxLen;
            foreach (NlogViewerTarget target in NLog.LogManager.Configuration.AllTargets.Where(t => t is NlogViewerTarget).Cast<NlogViewerTarget>())
            {
                target.LogReceived += LogReceived;
            }
        }

        private void LogReceived(AsyncLogEventInfo log)
        {
            LogMessage vm = new LogMessage(log.LogEvent.Level, log.LogEvent.TimeStamp, log.LogEvent.FormattedMessage);

            if (Log.Count >= _maxLen)
                Invoke(() => Log.RemoveAt(0));

            Invoke(() => Log.Add(vm));
        }

        private void Invoke(Action act)
        {
            _disp.BeginInvoke(new Action<LogViewModel>((sender) => act()), this);
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    [Target("NlogViewer")]
    public class NlogViewerTarget : Target
    {
        public event Action<AsyncLogEventInfo> LogReceived;

        protected override void Write(NLog.Common.AsyncLogEventInfo logEvent)
        {
            base.Write(logEvent);

            if (LogReceived != null)
                LogReceived(logEvent);
        }
    }

    public class LogMessage : TargetWithLayout
    {
        public LogMessage(LogLevel level, DateTime timeStamp, string message)
        {
            Level = level;
            TimeStamp = timeStamp;
            Message = message;
        }

        public LogLevel Level { get; private set; }

        public DateTime TimeStamp { get; private set; }

        public string Message { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
