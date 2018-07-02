using System.Collections.Generic;
using System.Windows;
using System.Windows.Input;

namespace EMMA
{
    /// <summary>
    ///     Interaction logic for ReplaceWindow.xaml
    /// </summary>
    public partial class ReplaceWindow : Window
    {
        public List<Equipment> ItemsList;
        public ReplaceWindow(List<Equipment> items)
        {
            InitializeComponent();
            ItemsList = items;
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
            searchtextbox.Focus();
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ReplaceButtonClick(object sender, RoutedEventArgs e)
        {
           // MainWindow.Database.NewTransaction(DataContext as Equipment, double.Parse(qty_textbox.Text), project_textbox.Text,
             //   Transaction.TransactionTypes.Release);

            Close();
        }
    }
}