using System.Windows;
using System.Windows.Input;
using EMMA.Helper_Classes;

namespace EMMA
{
    /// <summary>
    ///     Interaction logic for IssueWindow.xaml
    /// </summary>
    public partial class IssueWindow : Window
    {
        public IssueWindow()
        {
            InitializeComponent();
            Loaded += IssueWindow_Loaded;
            KeyDown += IssueWindow_KeyDown;
        }

        private void IssueWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        private void IssueWindow_Loaded(object sender, RoutedEventArgs e)
        {
            qty_textbox.Focus();
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void SaveButtonClick(object sender, RoutedEventArgs e)
        {
            MainWindow.db.NewTransaction(DataContext as StockItem, double.Parse(qty_textbox.Text), project_textbox.Text,
                Transaction.TransactionTypes.Release);

            Close();
        }
    }
}