using System.Windows;
using System.Windows.Input;

namespace EMMA
{
    /// <summary>
    ///     Interaction logic for ItemDisplayWindow.xaml
    /// </summary>
    public partial class ItemDisplayWindow : Window
    {
        public ItemDisplayWindow()
        {
            InitializeComponent();
            KeyDown += ItemDisplayWindow_KeyDown;
            close_button.Click += close_button_Click;
            issue_button.Click += issue_button_Click;
        }

        private void issue_button_Click(object sender, RoutedEventArgs e)
        {
            ShowIssueWindow(DataContext as StockItem);
        }

        private void ShowIssueWindow(StockItem stockItem)
        {
            IssueWindow iw = new IssueWindow();
            iw.DataContext = stockItem;
            iw.Show();
        }

        private void close_button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ItemDisplayWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
            if (e.Key == Key.I)
            {
                e.Handled = true;
                ShowIssueWindow(DataContext as StockItem);
            }
        }
    }
}