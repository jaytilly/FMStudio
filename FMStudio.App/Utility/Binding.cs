namespace FMStudio.App.Utility
{
    public class Binding<T> : NotifyPropertyChanged
    {
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
            get { return _value; }
            set
            {
                _value = value;
                Notify(() => Value);
                Notify(() => HasValue);

                IsChanged = true;
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
    }
}