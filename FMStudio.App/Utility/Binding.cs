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