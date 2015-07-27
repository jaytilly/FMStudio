using System;
using System.Threading.Tasks;
using System.Windows.Input;

namespace FMStudio.App.Utility
{
    public class Binding<T> : NotifyPropertyChanged
    {
        private bool _isInitialized;
        private Func<T> _factory;

        public bool HasValue
        {
            get { return _value != null; }
        }

        private bool _isChanged;

        public bool IsChanged
        {
            get { return _isChanged; }
            set
            {
                _isChanged = value;
                Notify(() => IsChanged);
            }
        }

        private T _value;

        public T Value
        {
            get
            {
                if (!_isInitialized && _factory != null)
                {
                    Task.Run(() =>
                    {
                        _value = _factory();
                        _isInitialized = true;
                        Notify(() => HasValue);
                        Notify(() => Value);
                    });
                }

                return _value;
            }
            set
            {
                _value = value;
                Notify(() => Value);
                Notify(() => HasValue);

                IsChanged = true;

                // Make sure any dependent CanExecute() handlers are refreshed
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public Binding(T value)
        {
            _value = value;
        }

        public Binding()
        {
            _value = default(T);
        }

        public Binding(Func<T> factory)
        {
            _factory = factory;
        }

        public override string ToString()
        {
            return _value != null ? _value.ToString() : "<Empty>";
        }
    }
}