using System;
using System.Collections.Generic;

namespace FMStudio.Lib
{
    public class NotifyingOutputWriter : IOutputWriter
    {
        private List<Action<string>> _listeners;

        public NotifyingOutputWriter()
        {
            _listeners = new List<Action<string>>();
        }

        public void OnOutput(Action<string> action)
        {
            _listeners.Add(action);
        }

        public void Write(string output)
        {
            foreach (var listener in _listeners)
            {
                listener(output);
            }
        }
    }
}