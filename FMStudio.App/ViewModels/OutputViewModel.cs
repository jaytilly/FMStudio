using FMStudio.App.Utility;
using FMStudio.Utility.Logging;
using System;
using System.Collections.Concurrent;
using System.Windows.Input;
using System.Windows.Threading;

namespace FMStudio.App.ViewModels
{
    public class OutputViewModel : ILog
    {
        public int MaxLines { get; set; }

        public Binding<string> Output { get; private set; }

        public ICommand ClearOutputCommand { get; private set; }

        private ConcurrentQueue<string> _rows;

        private DispatcherTimer _timer;

        public OutputViewModel(int maxLines)
        {
            MaxLines = maxLines;
            Output = new Binding<string>();

            _rows = new ConcurrentQueue<string>();

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(200);
            _timer.Tick += (s, e) => UpdateOutput();

            ClearOutputCommand = new RelayCommand(param => ClearOutput());
        }

        public OutputViewModel()
            : this(100)
        {
        }

        public void Write(string message, params object[] args)
        {
            var text = string.Format(message, args);

            if (!string.IsNullOrWhiteSpace(text))
            {
                var line = string.Format("{0} {1}", DateTime.Now.ToString(), text);

                _rows.Enqueue(line);

                _timer.Start();
            }
        }

        private void ClearOutput()
        {
            string dequeue;

            while (_rows.Count > 0) _rows.TryDequeue(out dequeue);

            Output.Value = string.Empty;
        }

        private void UpdateOutput()
        {
            _timer.Stop();

            string dequeue;

            while (_rows.Count > MaxLines) _rows.TryDequeue(out dequeue);

            Output.Value = string.Join(Environment.NewLine, _rows);
        }

        public void Debug(string message, params object[] args)
        {
            Write(message, args);
        }

        public void Error(string message, params object[] args)
        {
            Write(message, args);
        }

        public void Info(string message, params object[] args)
        {
            Write(message, args);
        }

        public void Warning(string message, params object[] args)
        {
            Write(message, args);
        }
    }
}