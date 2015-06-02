using FMStudio.Lib;
using System;
using System.Collections.Generic;
using System.Windows;

namespace FMStudio.App.Utility
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
            if (Application.Current != null)
            {
                Application.Current.Dispatcher.Invoke(() =>
                {
                    foreach (var listener in _listeners)
                    {
                        listener(output);
                    }
                });
            }
            else
            {
                foreach (var listener in _listeners)
                {
                    listener(output);
                }
            }
        }
    }
}