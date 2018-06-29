using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;

namespace EMMA
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static EquipmentDataModel Database = new EquipmentDataModel();

        public MainWindow()
        {
            InitializeComponent();
            Equipments = new ObservableCollection<Equipment>();
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            KeyDown += MainWindow_KeyDown;
            SearchBox.KeyDown += SearchBox_KeyDown;
            MasterDataGrid.MouseDoubleClick += MasterDataGrid_MouseDoubleClick;
            MasterDataGrid.CellEditEnding += MasterDataGrid_CellEditEnding;
            CloseButton.Click += CloseButton_Click;
            SaveButton.Click += SaveButton_Click;
            SearchBox.PreviewKeyDown += SearchBox_PreviewKeyDown;
            ;

            // MasterDataGrid.KeyDown += MasterDataGrid_KeyDown;
            inventoryButton.Click += inventoryButton_Click;
        }

        public static ObservableCollection<Equipment> Equipments { get; set; }

        private void SearchBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                MasterDataGrid.Focus();
                //then create a new cell info, with the item we wish to edit and the column number of the cell we want in edit mode
                DataGridCellInfo cellInfo = new DataGridCellInfo(MasterDataGrid.Items[0], MasterDataGrid.Columns[0]);
                //set the cell to be the active one
                MasterDataGrid.CurrentCell = cellInfo;
                //scroll the item into view
                MasterDataGrid.ScrollIntoView(MasterDataGrid.Items[0]);
                //begin the edit
                MasterDataGrid.BeginEdit();
            }

            if (e.Key == Key.Up)
            {
                MasterDataGrid.Focus();
                //then create a new cell info, with the item we wish to edit and the column number of the cell we want in edit mode
                DataGridCellInfo cellInfo = new DataGridCellInfo(MasterDataGrid.Items[MasterDataGrid.Items.Count - 1],
                    MasterDataGrid.Columns[0]);
                //set the cell to be the active one
                MasterDataGrid.CurrentCell = cellInfo;
                //scroll the item into view
                MasterDataGrid.ScrollIntoView(MasterDataGrid.Items[MasterDataGrid.Items.Count - 1]);
                //begin the edit
                MasterDataGrid.BeginEdit();
            }
        }

        private void MasterDataGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            if (MasterDataGrid.SelectedItems.Count > 1)
            {
                var column = e.Column as DataGridBoundColumn;
                if (column != null)
                {
                    var bindingPath = (column.Binding as Binding).Path.Path;
                    if (bindingPath == "Description")
                    {
                        var el = e.EditingElement as TextBox;
                        foreach (Equipment selectedItem in MasterDataGrid.SelectedItems)
                        {
                            if (el.Text != null) selectedItem.Description = el.Text;
                        }
                    }
                }
            }
        }

        private void EditItem(object sender, RoutedEventArgs e)
        {
        }

        #region EVENT HANDLERS

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Database.SaveChangesAsync();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void inventoryButton_Click(object sender, RoutedEventArgs e)
        {
            GenerateFakeItems();
        }

        private void MasterDataGrid_KeyDown(object sender, KeyEventArgs e)
        {
            if (MasterDataGrid.CurrentColumn.IsReadOnly)
            {
                if (e.Key == Key.S)
                {
                    e.Handled = true;
                    DisplayItemDetails(MasterDataGrid.SelectedItem as Equipment);
                }
                if (e.Key == Key.I)
                {
                    e.Handled = true;
                    ShowIssueWindow(MasterDataGrid.SelectedItem as Equipment);
                }
            }
        }

        private void MasterDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            DisplayItemDetails(MasterDataGrid.SelectedItem as Equipment);
        }

        private void MainWindow_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Oem3)
            {
                Application.Current.Shutdown();
            }
            if (e.Key == Key.Escape)
            {
                SearchBox.Focus();
            }
            if (e.Key == Key.Down || e.Key == Key.Up)
            {
                if (!MasterDataGrid.IsFocused)
                {
                    MasterDataGrid.Focus();
                }
            }
        }

        private void MainWindow_Closing(object sender, CancelEventArgs e)
        {
            Database.SaveChanges();
        }

        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {


            {
                Dispatcher.BeginInvoke(DispatcherPriority.Input,
                    (Action) (() => MasterDataGrid.Items.Filter = DescriptionFilter));
            }
        }


        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            if (Database.Equipments != null) MasterDataGrid.ItemsSource = Equipments;
            SearchBox.Text = "";
            SearchBox.Focus();
            Thread t = new Thread(UpdateMasterList);
            t.Start();
        }

        #endregion

        #region MISC METHODS

        private void UpdateMasterList()
        {
            StatusUpdate("Loading");
            foreach (Equipment equ in Database.Equipments)
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Loaded, (Action) (() => Equipments.Add(equ)));
            }

            StatusUpdate("");
        }

        private void StatusUpdate(string s)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Loaded,
                (Action) (() => _statusTextBlock.Text = s));
        }

        private void DisplayItemDetails(Equipment equipment)
        {
            var idw = new ItemDisplayWindow();
            idw.DataContext = equipment;
            idw.Show();
        }

        private void ShowIssueWindow(Equipment equipment)
        {
            var iw = new IssueWindow();
            iw.DataContext = equipment;
            iw.Show();
        }


        private bool DescriptionFilter(object obj)
        {
            var filterItem = obj as Equipment;
            char[] searchSplitters = {'+', '-'};
            var filterText = SearchBox.Text.Split(searchSplitters, StringSplitOptions.RemoveEmptyEntries);

            if (filterText.All(s => filterItem.Description.ToLower().Contains(s.ToLower())))
            {
                return true;
            }
            if (filterItem.EquipmentNumber.Contains(filterText[0]))
            {
                return true;
            }
            return false;
        }


        private void GenerateFakeItems()
        {
            int lastid = 1;
            for (int i = 0; i < 10; i++)
            {
                Equipment e = new Equipment();
                e.id = lastid + 1;
                e.Description = "Item number" + i;
                e.EquipmentNumber = i.ToString("0000000000");
                Database.Equipments.Add(e);
                lastid++;
            }
            Database.SaveChanges();
        }


        private void headerChanged(object sender, TextChangedEventArgs e)
        {
            TextBox t = (TextBox) sender;
            string filter = t.Text;
            ICollectionView cv = CollectionViewSource.GetDefaultView(MasterDataGrid.ItemsSource);
            if (filter == "")
                cv.Filter = null;
            else
            {
                cv.Filter = o =>
                {
                    var filterItem = o as Equipment;
                    char[] searchSplitters = {'+', '-'};
                    var filterText = filter.Split(searchSplitters, StringSplitOptions.RemoveEmptyEntries);

                    if (filterText.All(s => filterItem.Description.ToLower().Contains(s.ToLower())))
                    {
                        return true;
                    }
                    if (filterItem.EquipmentNumber.Contains(filterText[0]))
                    {
                        return true;
                    }
                    return false;
                };
            }
        }

        #endregion
    }
}