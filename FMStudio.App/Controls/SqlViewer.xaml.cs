using FMStudio.App.Utility;
using FMStudio.App.ViewModels;
using ICSharpCode.AvalonEdit.Document;
using System;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
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

            InitializeSqlHighlighting();

            FindText = new Binding<string>();
            FindText.PropertyChanged += (s, e) => Find(FindText.Value);

            FindCommand = new RelayCommand(param => Find(param as string));
            FocusFindCommand = new RelayCommand(param => txtSearch.Focus());
        }

        private void InitializeSqlHighlighting()
        {
            var themeName = MainWindow.Instance.Root.Configuration.Preferences.Theme;
            var theme = ThemeViewModel.GetThemesList().FirstOrDefault(t => t.Name == themeName);

            if (theme == null)
                theme = ThemeViewModel.GetThemesList()[0];

            var xshd = "FMStudio.App.Themes.{0}.xshd".FormatInvariant(theme.SqlViewerResourceName);

            using (var stream = System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream(xshd))
            {
                var reader = new XmlTextReader(stream);

                txtSql.SyntaxHighlighting =
                    ICSharpCode.AvalonEdit.Highlighting.Xshd.HighlightingLoader.Load(reader,
                    ICSharpCode.AvalonEdit.Highlighting.HighlightingManager.Instance);
            }
        }

        public TextDocument Document { get; set; }

        public bool HasValue
        {
            get { return (bool)GetValue(HasValueProperty); }
            set { SetValue(HasValueProperty, value); }
        }

        public string Text
        {
            get { return (string)GetValue(TextProperty); }
            set { SetValue(TextProperty, value); }
        }

        public static DependencyProperty HasValueProperty = DependencyProperty.Register("HasValue", typeof(bool), typeof(SqlViewer));

        public static DependencyProperty TextProperty = DependencyProperty.Register("Text", typeof(string), typeof(SqlViewer));

        private void OnTextChanged(object sender, EventArgs e)
        {
            if (Text != null)
                Document.Text = Text;
        }

        public ICommand FindCommand { get; private set; }

        public ICommand FocusFindCommand { get; private set; }

        public Binding<string> FindText { get; private set; }

        private int lastSearchIndex = 0;
        private string lastSearchQuery;

        private void Find(string searchQuery)
        {
            if (string.IsNullOrEmpty(searchQuery))
            {
                lastSearchIndex = 0;
                return;
            }

            var editorText = txtSql.Text;

            if (string.IsNullOrEmpty(editorText))
            {
                lastSearchIndex = 0;
                return;
            }

            if (lastSearchIndex >= editorText.Count())
                lastSearchIndex = 0;

            if (searchQuery != lastSearchQuery)
                lastSearchIndex = 0;

            var nIndex = editorText.IndexOf(searchQuery, lastSearchIndex, StringComparison.OrdinalIgnoreCase);

            if (nIndex != -1)
            {
                var area = txtSql.TextArea;

                txtSql.ScrollToLine(txtSql.Document.GetLineByOffset(nIndex).LineNumber);
                txtSql.Select(nIndex, searchQuery.Length);

                lastSearchIndex = nIndex + searchQuery.Length;
                lastSearchQuery = searchQuery;
            }
            else
            {
                if (lastSearchIndex != 0)
                {
                    lastSearchIndex = 0;
                    Find(searchQuery);
                }
                else
                {
                    lastSearchIndex = 0;
                }
            }
        }
    }
}