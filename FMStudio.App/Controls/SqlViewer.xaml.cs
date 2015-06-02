using ICSharpCode.AvalonEdit.Document;
using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Xml;

namespace FMStudio.App.Controls
{
    public partial class SqlViewer : UserControl
    {
        public SqlViewer()
        {
            InitializeComponent();

            Document = new TextDocument();

            var dp = DependencyPropertyDescriptor.FromProperty(TextProperty, (typeof(SqlViewer)));
            dp.AddValueChanged(this, OnTextChanged);

            using (var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream("FMStudio.App.Resources.sql.xshd"))
            {
                using (var reader = new XmlTextReader(stream))
                {
                    txtSql.SyntaxHighlighting =
                        ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(reader,
                        ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance);
                }
            }
        }

        public TextDocument Document { get; set; }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(SqlViewer));

        private void OnTextChanged(object sender, EventArgs e)
        {
            if (Text != null)
                Document.Text = Text;
        }
    }
}