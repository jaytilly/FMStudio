namespace FMStudio.App.Utility
{
    public class Binding<T> : NotifyPropertyChanged
    {
        private T _value;

        public T Value
        {
            get { return _value; }
            set
            {
                _value = value;
                Notify(() => Value);

                IsChanged = true;
            }
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