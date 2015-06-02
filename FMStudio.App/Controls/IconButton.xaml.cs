using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace FMStudio.App.Controls
{
    public partial class IconButton : UserControl
    {
        public IconButton()
        {
            InitializeComponent();
        }

        public ICommand Command
        {
            get { return (ICommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }

        public string Icon
        {
            get { return (string)GetValue(IconProperty); }
            set { SetValue(IconProperty, value); }
        }

        public Brush IconColor
        {
            get { return (Brush)GetValue(IconColorProperty); }
            set { SetValue(IconColorProperty, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(IconButton));

        public static DependencyProperty IconProperty = DependencyProperty.Register("Icon", typeof(string), typeof(IconButton));

        public static DependencyProperty IconColorProperty = DependencyProperty.Register("IconColor", typeof(Brush), typeof(IconButton));

        public static DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(IconButton));
    }
}