using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using OfficeOpenXml;

namespace EMMA
{
    /// <summary>
    ///     Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public static string FAR = @"D:\Aghils\ASSETS\FAR.xlsx";
        public static string EQM = @"D:\Aghils\ASSETS\EQM.xlsx";
        public static string REPORT = @"D:\Aghils\ASSETS\ASSET MASTER UPDATE.xlsx";
        public static List<Equipment> EquipmentMaster = new List<Equipment>();

        public static List<Equipment> AssetMaster = new List<Equipment>();
        //private static GuiSession session;

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
            wipeButton.Click += WipeButton_Click;

            // MasterDataGrid.KeyDown += MasterDataGrid_KeyDown;
            inventoryButton.Click += inventoryButton_Click;
        }

        public static ObservableCollection<Equipment> Equipments { get; set; }

        private void WipeButton_Click(object sender, RoutedEventArgs e)
        {
            Database.Equipments.RemoveRange(Database.Equipments);
            Database.SaveChanges();
        }


        #region EVENT HANDLERS

        private void SearchBox_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Down)
            {
                MasterDataGrid.Focus();
                //then create a new cell info, with the item we wish to edit and the column number of the cell we want in edit mode
                var cellInfo = new DataGridCellInfo(MasterDataGrid.Items[0], MasterDataGrid.Columns[0]);
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
                var cellInfo = new DataGridCellInfo(MasterDataGrid.Items[MasterDataGrid.Items.Count - 1],
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
                            if (el.Text != null)
                                selectedItem.New.AssetDescription = el.Text;
                    }
                }
            }
        }

        private void EditItem(object sender, RoutedEventArgs e)
        {
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            Database.SaveChanges();
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
            if (e.Key == Key.Oem3) Application.Current.Shutdown();
            if (e.Key == Key.Escape) SearchBox.Focus();
            if (e.Key == Key.Down || e.Key == Key.Up)
                if (!MasterDataGrid.IsFocused)
                    MasterDataGrid.Focus();
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
            var t = new Thread(UpdateMasterList);
            t.Start();
        }

        #endregion

        #region MISC METHODS

        private void UpdateMasterList()
        {
            StatusUpdate("Loading");
            foreach (var equ in Database.Equipments)
                Dispatcher.BeginInvoke(DispatcherPriority.Loaded, (Action) (() => Equipments.Add(equ)));

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

            if (filterText.All(s => filterItem.New.AssetDescription.ToLower().Contains(s.ToLower()))) return true;
            if (filterItem.EquipmentNumber.Contains(filterText[0])) return true;
            return false;
        }


        private void GenerateFakeItems()
        {
            var t = new Thread(() =>
            {
                ReadFAR();
                ReadEQM();
                ProcessFAR();
                Database.SaveChanges();
            });

            t.Start();
        }

        private void ProcessFAR()
        {
            Console.WriteLine();
            var q = 1;
            for (var i = 0; i < AssetMaster.Count; i++)
            {
                var e = EquipmentMaster.FirstOrDefault(m => m.AssetNumber == AssetMaster[i].AssetNumber);
                if (e != null)
                {
                    AssetMaster[i].EquipmentNumber = e.EquipmentNumber;
                    AssetMaster[i].Old.EquipmentDescription = e.Old.EquipmentDescription;
                    AssetMaster[i].Old.OperationId = e.Old.OperationId;
                    AssetMaster[i].Old.SubType = e.Old.SubType;
                    AssetMaster[i].Old.Weight = e.Old.Weight;
                    AssetMaster[i].Old.WeightUnit = e.Old.WeightUnit;
                    AssetMaster[i].Old.Dimensions = e.Old.Dimensions;
                    AssetMaster[i].Old.ModelNumber = e.Old.ModelNumber;
                    AssetMaster[i].Old.SerialNumber = e.Old.SerialNumber;
                    AssetMaster[i].Old.AssetLocation = e.Old.EquipmentLocation;

                    AssetMaster[i].EquipmentNumber = e.EquipmentNumber;
                    AssetMaster[i].New.EquipmentDescription = e.New.EquipmentDescription;
                    AssetMaster[i].New.OperationId = e.New.OperationId;
                    AssetMaster[i].New.SubType = e.New.SubType;
                    AssetMaster[i].New.Weight = e.New.Weight;
                    AssetMaster[i].New.WeightUnit = e.New.WeightUnit;
                    AssetMaster[i].New.Dimensions = e.New.Dimensions;
                    AssetMaster[i].New.ModelNumber = e.New.ModelNumber;
                    AssetMaster[i].New.SerialNumber = e.New.SerialNumber;
                    AssetMaster[i].New.AssetLocation = e.New.EquipmentLocation;


                    q++;
                    Database.Equipments.Add(AssetMaster[i]);

                    Dispatcher.BeginInvoke(DispatcherPriority.Input,
                        (Action)(() =>
                            _statusTextBlock.Text = "Processing EQM & FAR " + i));
                }
            }

            Database.SaveChanges();
        }

        private void ReadEQM()
        {
            var fi = new FileInfo(EQM);
            using (var excelPackage = new ExcelPackage(fi))
            {
                var myWorkbook = excelPackage.Workbook;
                var myWorksheet = myWorkbook.Worksheets[1];

                for (var i = 1; i < myWorksheet.Dimension.End.Row; i++)
                {
                    var e = new Equipment();
                    e.EquipmentNumber = myWorksheet.Cells[i, 1].Text.Trim();
                    e.AssetNumber = myWorksheet.Cells[i, 2].Text.Trim();
                    e.Old.EquipmentDescription = myWorksheet.Cells[i, 3].Text.Trim();
                    e.Old.OperationId = myWorksheet.Cells[i, 4].Text.Trim();
                    e.Old.SubType = myWorksheet.Cells[i, 5].Text.Trim();
                    e.Old.Weight = myWorksheet.Cells[i, 6].Text.Trim();
                    e.Old.WeightUnit = myWorksheet.Cells[i, 7].Text.Trim();
                    e.Old.Dimensions = myWorksheet.Cells[i, 8].Text.Trim();
                    e.Old.ModelNumber = myWorksheet.Cells[i, 9].Text.Trim();
                    e.Old.SerialNumber = myWorksheet.Cells[i, 10].Text.Trim();
                    e.Old.EquipmentLocation = myWorksheet.Cells[i, 12].Text.Trim();


                    e.New.EquipmentDescription = myWorksheet.Cells[i, 3].Text.Trim();
                    e.New.OperationId = myWorksheet.Cells[i, 4].Text.Trim();
                    e.New.SubType = myWorksheet.Cells[i, 5].Text.Trim();
                    e.New.Weight = myWorksheet.Cells[i, 6].Text.Trim();
                    e.New.WeightUnit = myWorksheet.Cells[i, 7].Text.Trim();
                    e.New.Dimensions = myWorksheet.Cells[i, 8].Text.Trim();
                    e.New.ModelNumber = myWorksheet.Cells[i, 9].Text.Trim();
                    e.New.SerialNumber = myWorksheet.Cells[i, 10].Text.Trim();
                    e.New.EquipmentLocation = myWorksheet.Cells[i, 12].Text.Trim();
                    EquipmentMaster.Add(e);

                    Dispatcher.BeginInvoke(DispatcherPriority.Input,
                        (Action)(() =>
                            _statusTextBlock.Text = "Reading EQM " + i));
                }
            }
        }

        private void ReadFAR()
        {
            var fi = new FileInfo(FAR);
            using (var excelPackage = new ExcelPackage(fi))
            {
                var myWorkbook = excelPackage.Workbook;
                var myWorksheet = myWorkbook.Worksheets[1];

                for (var i = 2; i < myWorksheet.Dimension.End.Row; i++)
                {
                    var e = new Equipment();
                    e.AcquisitionValue = myWorksheet.Cells[i, 6].Text.Trim().Replace(",", string.Empty);
                    e.BookValue = myWorksheet.Cells[i, 8].Text.Trim().Replace(",", string.Empty);
                    e.AcquisitionDate = DateTime.Parse(myWorksheet.Cells[i, 4].Value.ToString()).ToOADate().ToString();
                    e.AssetNumber = myWorksheet.Cells[i, 1].Text.Trim();
                    e.EquipmentNumber = myWorksheet.Cells[i, 3].Text.Trim();
                    e.Old.AssetDescription = myWorksheet.Cells[i, 5].Text.Trim();
                    e.Old.AssetLocation = myWorksheet.Cells[i, 10].Text.Trim();
                    e.New.AssetDescription = myWorksheet.Cells[i, 5].Text.Trim();
                    e.New.AssetLocation = myWorksheet.Cells[i, 10].Text.Trim();
                    if (e.AssetNumber != "" && e.AssetNumber != "Asset") Database.Equipments.Add(e);

                    Dispatcher.BeginInvoke(DispatcherPriority.Input,
                        (Action) (() =>
                            _statusTextBlock.Text = "Reading FAR " + i));
                }
            }
        }

        private void headerChanged(object sender, TextChangedEventArgs e)
        {
            var t = (TextBox) sender;
            var filter = t.Text;
            var cv = CollectionViewSource.GetDefaultView(MasterDataGrid.ItemsSource);
            if (filter == "")
                cv.Filter = null;
            else
                cv.Filter = o =>
                {
                    var filterItem = o as Equipment;
                    char[] searchSplitters = {'+', '-'};
                    var filterText = filter.Split(searchSplitters, StringSplitOptions.RemoveEmptyEntries);

                    if (filterText.All(s => filterItem.New.AssetDescription.ToLower().Contains(s.ToLower())))
                        return true;
                    if (filterItem.EquipmentNumber.Contains(filterText[0])) return true;
                    return false;
                };
        }

        #endregion
    }
}