using System.Collections;
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
        public IList ItemsList { get; set; }
        public ReplaceWindow(IList items)
        {
            InitializeComponent();
            ItemsList = items;
            Loaded += WindowLoaded;
            KeyDown += WindowKeyDown;
        }

        private void WindowKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape)
            {
                Close();
            }
        }

        private void WindowLoaded(object sender, RoutedEventArgs e)
        {
            ItemCount.Content = ItemsList.Count + " Items Selected";
            searchtextbox.Focus();
        }

        private void CloseButtonClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void ReplaceButtonClick(object sender, RoutedEventArgs e)
        {
            foreach (Equipment equipment in ItemsList)
            {
                equipment.New.EquipmentDescription.Replace(searchtextbox.Text, replacetextbox.Text);
            }

            Close();
        }
    }
}