using System;
using System.Collections.Concurrent;
using System.Windows.Threading;

namespace FMStudio.App.ViewModels
{
    public class OutputLogViewModel
    {
        public event EventHandler OnOutputChanged = delegate { };

        public int MaxLines { get; set; }

        public string Output { get; private set; }

        private ConcurrentQueue<string> _rows;

        private DispatcherTimer _timer;

        public OutputLogViewModel(int maxLines)
        {
            MaxLines = maxLines;

            _rows = new ConcurrentQueue<string>();

            _timer = new DispatcherTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(200);
            _timer.Tick += (s, e) => UpdateOutput();
        }

        public OutputLogViewModel()
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

        private void UpdateOutput()
        {
            _timer.Stop();

            string dequeue;

            while (_rows.Count > MaxLines) _rows.TryDequeue(out dequeue);

            Output = string.Join(Environment.NewLine, _rows);

            OnOutputChanged(this, EventArgs.Empty);
        }
    }
}