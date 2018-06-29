using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using System.Windows.Threading;
using EMMA.Helper_Classes;

namespace EMMA
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static DB db;

        public MainWindow()
        {
            InitializeComponent();

            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            KeyDown += MainWindow_KeyDown;
            SearchBox.KeyDown += SearchBox_KeyDown;
            MasterDataGrid.MouseDoubleClick += MasterDataGrid_MouseDoubleClick;
            MasterDataGrid.KeyDown += MasterDataGrid_KeyDown;
            inventoryButton.Click += inventoryButton_Click;
        }

        #region EVENT HANDLERS

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
                    DisplayItemDetails(MasterDataGrid.SelectedItem as StockItem);
                }
                if (e.Key == Key.I)
                {
                    e.Handled = true;
                    ShowIssueWindow(MasterDataGrid.SelectedItem as StockItem);
                }
            }
        }

        private void MasterDataGrid_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (MasterDataGrid.CurrentColumn.IsReadOnly)
            {
                DisplayItemDetails(MasterDataGrid.SelectedItem as StockItem);
            }
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
            SaveDatabse();
        }

        private void SearchBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down || e.Key == Key.Up)
            {
                MasterDataGrid.Focus();
            }
            else
            {
                Dispatcher.BeginInvoke(DispatcherPriority.Input,
                    (Action) (() => MasterDataGrid.Items.Filter = DescriptionFilter));
            }
        }

        private void MainWindow_Loaded(object sender, RoutedEventArgs e)
        {
            LoadDatabase();

            if (db == null) db = new DB();
            MasterDataGrid.ItemsSource = db.Database;
            SearchBox.Text = "";
            SearchBox.Focus();
        }

        #endregion

        #region MISC METHODS

        private void DisplayItemDetails(StockItem stockItem)
        {
            var idw = new ItemDisplayWindow();
            idw.DataContext = stockItem;
            idw.Show();
        }

        private void ShowIssueWindow(StockItem stockItem)
        {
            var iw = new IssueWindow();
            iw.DataContext = stockItem;
            iw.Show();
        }


        private void ExportTransactionReport()
        {
            //var Output = new ExcelPackage(new FileInfo("report.xlsx"));
            //var workSheet = Output.Workbook.Worksheets[1];
            //var rowCount = 1;
            //foreach (var transaction in db.Transactions)
            //{
            //    workSheet.Cells[rowCount, 1].Value = transaction.Date;
            //    workSheet.Cells[rowCount, 2].Value = transaction.ItemStockCode;
            //    workSheet.Cells[rowCount, 3].Value = transaction.Qty;
            //    workSheet.Cells[rowCount, 4].Value = transaction.Project;
            //    rowCount++;
            //}

            //Output.Save();
            //MessageBox.Show("Report Generated successfully");
        }

        private bool DescriptionFilter(object obj)
        {
            var filterItem = obj as StockItem;
            char[] searchSplitters = {'+', '-'};
            var filterText = SearchBox.Text.Split(searchSplitters, StringSplitOptions.RemoveEmptyEntries);

            if (filterText.All(s => filterItem.FullDescription.Contains(s.ToUpper())))
            {
                return true;
            }
            if (filterItem.StockCode.Contains(filterText[0]))
            {
                return true;
            }
            return false;
        }

        private void SaveDatabse()
        {
            StorageSystem.Store(db);
        }

        private void LoadDatabase()
        {
            db = StorageSystem.Read();
        }

        private void GenerateFakeItems()
        {
            //var source = new ExcelPackage(new FileInfo(@"D:\AGHILS\STOCK\MASTER STOCK.xlsx"));
            //var SWS = source.Workbook.Worksheets[1];
            //var aAddressRow = 2;

            //while (aAddressRow < 2500)
            //{
            //    if (!string.IsNullOrEmpty(SWS.Cells[aAddressRow, 1].Text) &&
            //        !string.IsNullOrEmpty(SWS.Cells[aAddressRow, 6].Text))
            //    {
            //        var tempItem = new StockItem();
            //        tempItem.StockCode = SWS.Cells[aAddressRow, 1].Text;
            //        tempItem.FullDescription = SWS.Cells[aAddressRow, 2].Text;
            //        tempItem.Location = SWS.Cells[aAddressRow, 6].Text;
            //        tempItem.PhysicalQty = double.Parse(SWS.Cells[aAddressRow, 3].Text);
            //        tempItem.StockQty = Double.Parse(SWS.Cells[aAddressRow, 4].Text);
            //        tempItem.Type = SWS.Cells[aAddressRow, 9].Text;
            //        tempItem.Price = SWS.Cells[aAddressRow, 15].Text;
            //        db.Database.Add(tempItem);
            //    }
            //    aAddressRow++;
            //}
        }

        #endregion
    }
}