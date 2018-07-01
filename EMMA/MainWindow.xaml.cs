using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Threading;
using OfficeOpenXml;
using SapROTWr;
using SAPFEWSELib;

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
        private static GuiSession session;

        public static EquipmentDataModel Database = new EquipmentDataModel();

        public MainWindow()
        {
            InitializeComponent();
            Equipments = new ObservableCollection<Equipment>();
            Loaded += MainWindow_Loaded;
            Closing += MainWindow_Closing;
            KeyDown += MainWindow_KeyDown;
            SearchBox.KeyDown += SearchBox_KeyDown;
            //MasterDataGrid.MouseDoubleClick += MasterDataGrid_MouseDoubleClick;
            MasterDataGrid.CellEditEnding += MasterDataGrid_CellEditEnding;
            CloseButton.Click += CloseButton_Click;
            SaveButton.Click += SaveButton_Click;
            SearchBox.PreviewKeyDown += SearchBox_PreviewKeyDown;
            wipeButton.Click += WipeButton_Click;
            syncButton.Click += SyncButton_Click;

            // MasterDataGrid.KeyDown += MasterDataGrid_KeyDown;
            inventoryButton.Click += inventoryButton_Click;
        }

        public static ObservableCollection<Equipment> Equipments { get; set; }


        #region EVENT HANDLERS

        private void SyncButton_Click(object sender, RoutedEventArgs e)
        {
            var t = new Thread(UpdateChanges);
            t.Start();
        }


        private void WipeButton_Click(object sender, RoutedEventArgs e)
        {
            var t = new Thread(() =>
            {
                Database.Equipments.RemoveRange(Database.Equipments);
                Database.SaveChanges();
                Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                    (Action) (() => _statusTextBlock.Text = "All Data Wiped"));
            });

            t.Start();
        }

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
            try
            {
                ReadFromRegisters();
            }
            catch (Exception exception)
            {
                MessageBox.Show(exception.Message);
            }
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
            try
            {
                Database.SaveChanges();
            }
            catch (Exception exception)
            {
                MessageBox.Show("Database Error", exception.Message);
            }
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

        private void UpdateChanges()
        {
            ActivateSap();
            for (var i = 0; i < Equipments.Count; i++)
            {
                try
                {
                    if (Equipments[i].Old.EquipmentDescription != Equipments[i].New.EquipmentDescription)
                        EUpdateEquipmentDescription(i);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                try
                {
                    if (Equipments[i].Old.OperationId != Equipments[i].New.OperationId) EUpdateOperationId(i);
                    ;
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                try
                {
                    if (Equipments[i].Old.ModelNumber != Equipments[i].New.ModelNumber) EUpdateModelNumber(i);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                try
                {
                    if (Equipments[i].Old.SerialNumber != Equipments[i].New.SerialNumber) EUpdateSerialNumber(i);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                try
                {
                    if (Equipments[i].Old.AssetDescription != Equipments[i].New.EquipmentDescription &&
                        Equipments[i].New.EquipmentDescription != "")
                        EUpdateAssetDescription(i);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }

                Dispatcher.BeginInvoke(DispatcherPriority.Input,
                    (Action) (() => _statusTextBlock.Text = "Updating Values in SAP " + i));
            }

            Dispatcher.BeginInvoke(DispatcherPriority.Input,
                (Action) (() => _statusTextBlock.Text = "All Values Updated in SAP"));
        }

        private static void ActivateSap()
        {
            //Get the Windows Running Object Table
            var sapROTWrapper = new CSapROTWrapper();
            //Get the ROT Entry for the SAP Gui to connect to the COM
            object SapGuilRot = sapROTWrapper.GetROTEntry("SAPGUI");
            //Get the reference to the Scripting Engine
            var engine = SapGuilRot.GetType()
                .InvokeMember("GetScriptingEngine", BindingFlags.InvokeMethod, null, SapGuilRot, null);
            //Get the reference to the running SAP Application Window
            var GuiApp = (GuiApplication) engine;
            //Get the reference to the first open connection
            var connection = (GuiConnection) GuiApp.Connections.ElementAt(0);
            //get the first available session
            session = (GuiSession) connection.Children.ElementAt(0);
            //Get the reference to the main "Frame" in which to send virtual key commands
            //GuiFrameWindow frame = (GuiFrameWindow)session.FindById("wnd[0]");
        }

        private static void EUpdateAssetDescription(int i)
        {
            Equipments[i].New.AssetDescription = Equipments[i].New.EquipmentDescription;

            ((GuiMainWindow) session.FindById("wnd[0]")).Maximize();
            ((GuiOkCodeField) session.FindById("wnd[0]/tbar[0]/okcd")).Text = "/NAS02";
            ((GuiMainWindow) session.FindById("wnd[0]")).SendVKey(0);
            ((GuiTextField) session.FindById("wnd[0]/usr/ctxtANLA-ANLN1")).Text = Equipments[i].AssetNumber;
            ((GuiMainWindow) session.FindById("wnd[0]")).SendVKey(0);
            ((GuiTextField) session.FindById(
                    "wnd[0]/usr/subTABSTRIP:SAPLATAB:0100/tabsTABSTRIP100/tabpTAB01/ssubSUBSC:SAPLATAB:0200/subAREA1:SAPLAIST:1140/txtANLA-TXT50")
                ).Text = Equipments[i].New.AssetDescription;
            ;
            ((GuiTextField) session.FindById(
                    "wnd[0]/usr/subTABSTRIP:SAPLATAB:0100/tabsTABSTRIP100/tabpTAB01/ssubSUBSC:SAPLATAB:0200/subAREA1:SAPLAIST:1140/txtANLA-TXA50")
                ).Text = Equipments[i].New.AssetDescription;
            ((GuiTextField) session.FindById(
                    "wnd[0]/usr/subTABSTRIP:SAPLATAB:0100/tabsTABSTRIP100/tabpTAB01/ssubSUBSC:SAPLATAB:0200/subAREA1:SAPLAIST:1140/txtANLH-ANLHTXT")
                ).Text = Equipments[i].New.AssetDescription;
            ((GuiMainWindow) session.FindById("wnd[0]")).SendVKey(0);
            ((GuiMainWindow) session.FindById("wnd[0]")).SendVKey(11);

            Equipments[i].Old.AssetDescription = Equipments[i].New.AssetDescription;
            var l = new Transaction();
            l.Equipment = Equipments[i];
            l.Remarks = ((GuiStatusbar) session.FindById("wnd[0]/sbar")).Text;
            l.Type = Transaction.TransactionTypes.AssetDescription;
            l.OldValue = Equipments[i].Old.AssetDescription;
            l.NewValue = Equipments[i].New.AssetDescription;

            Database.Transactions.Add(l);
            Database.SaveChanges();
        }

        private static void EUpdateSerialNumber(int i)
        {
            ((GuiMainWindow) session.FindById("wnd[0]")).Maximize();
            ((GuiOkCodeField) session.FindById("wnd[0]/tbar[0]/okcd")).Text = "/nie02";
            ((GuiMainWindow) session.FindById("wnd[0]")).SendVKey(0);
            ((GuiTextField) session.FindById("wnd[0]/usr/ctxtRM63E-EQUNR")).Text = Equipments[i].EquipmentNumber;
            ((GuiCTextField) session.FindById("wnd[0]/usr/ctxtRM63E-EQUNR")).CaretPosition = 8;
            ((GuiMainWindow) session.FindById("wnd[0]")).SendVKey(0);
            ((GuiTextField) session.FindById(
                    "wnd[0]/usr/tabsTABSTRIP/tabpT\\01/ssubSUB_DATA:SAPLITO0:0102/subSUB_0102D:SAPLITO0:1022/txtITOB-SERGE")
                )
                .Text = Equipments[i].New.SerialNumber;
            ((GuiMainWindow) session.FindById("wnd[0]")).SendVKey(11);
            Equipments[i].Old.SerialNumber = Equipments[i].New.SerialNumber;
            var l = new Transaction();
            l.Equipment = Equipments[i];
            l.Remarks = ((GuiStatusbar) session.FindById("wnd[0]/sbar")).Text;
            l.Type = Transaction.TransactionTypes.SerialNumber;
            l.OldValue = Equipments[i].Old.SerialNumber;
            l.NewValue = Equipments[i].New.SerialNumber;

            Database.Transactions.Add(l);
            Database.SaveChanges();
        }

        private static void EUpdateModelNumber(int i)
        {
            ((GuiMainWindow) session.FindById("wnd[0]")).Maximize();
            ((GuiOkCodeField) session.FindById("wnd[0]/tbar[0]/okcd")).Text = "/nie02";
            ((GuiMainWindow) session.FindById("wnd[0]")).SendVKey(0);
            ((GuiTextField) session.FindById("wnd[0]/usr/ctxtRM63E-EQUNR")).Text = Equipments[i].EquipmentNumber;
            ((GuiCTextField) session.FindById("wnd[0]/usr/ctxtRM63E-EQUNR")).CaretPosition = 8;
            ((GuiMainWindow) session.FindById("wnd[0]")).SendVKey(0);
            ((GuiTextField) session.FindById(
                    "wnd[0]/usr/tabsTABSTRIP/tabpT\\01/ssubSUB_DATA:SAPLITO0:0102/subSUB_0102D:SAPLITO0:1022/txtITOB-TYPBZ")
                )
                .Text = Equipments[i].New.ModelNumber;

            ((GuiMainWindow) session.FindById("wnd[0]")).SendVKey(11);

            Equipments[i].Old.ModelNumber = Equipments[i].New.ModelNumber;
            var l = new Transaction();
            l.Equipment = Equipments[i];
            l.Remarks = ((GuiStatusbar) session.FindById("wnd[0]/sbar")).Text;
            l.Type = Transaction.TransactionTypes.ModelNumber;
            l.OldValue = Equipments[i].Old.ModelNumber;
            l.NewValue = Equipments[i].New.ModelNumber;
            Database.Transactions.Add(l);
            Database.SaveChanges();
        }

        private static void EUpdateOperationId(int i)
        {
            ((GuiMainWindow) session.FindById("wnd[0]")).Maximize();
            ((GuiOkCodeField) session.FindById("wnd[0]/tbar[0]/okcd")).Text = "/nie02";
            ((GuiMainWindow) session.FindById("wnd[0]")).SendVKey(0);
            ((GuiTextField) session.FindById("wnd[0]/usr/ctxtRM63E-EQUNR")).Text = Equipments[i].EquipmentNumber;
            ((GuiCTextField) session.FindById("wnd[0]/usr/ctxtRM63E-EQUNR")).CaretPosition = 8;
            ((GuiMainWindow) session.FindById("wnd[0]")).SendVKey(0);
            ((GuiTextField) session.FindById(
                    "wnd[0]/usr/tabsTABSTRIP/tabpT\\01/ssubSUB_DATA:SAPLITO0:0102/subSUB_0102A:SAPLITO0:1020/txtITOB-INVNR")
                )
                .Text = Equipments[i].New.OperationId;
            ((GuiMainWindow) session.FindById("wnd[0]")).SendVKey(11);
            Equipments[i].Old.OperationId = Equipments[i].New.OperationId;
            var l = new Transaction();
            l.Equipment = Equipments[i];
            l.Remarks = ((GuiStatusbar) session.FindById("wnd[0]/sbar")).Text;
            l.Type = Transaction.TransactionTypes.OperationId;
            l.OldValue = Equipments[i].Old.OperationId;
            l.NewValue = Equipments[i].New.OperationId;
            Database.Transactions.Add(l);
            Database.SaveChanges();
        }

        private static void EUpdateEquipmentDescription(int i)
        {
            ((GuiMainWindow) session.FindById("wnd[0]")).Maximize();
            ((GuiOkCodeField) session.FindById("wnd[0]/tbar[0]/okcd")).Text = "/nie02";
            ((GuiMainWindow) session.FindById("wnd[0]")).SendVKey(0);
            ((GuiTextField) session.FindById("wnd[0]/usr/ctxtRM63E-EQUNR")).Text = Equipments[i].EquipmentNumber;
            ((GuiCTextField) session.FindById("wnd[0]/usr/ctxtRM63E-EQUNR")).CaretPosition = 8;
            ((GuiMainWindow) session.FindById("wnd[0]")).SendVKey(0);
            ((GuiTextField) session.FindById(
                    "wnd[0]/usr/subSUB_EQKO:SAPLITO0:0152/subSUB_0152B:SAPLITO0:1525/txtITOB-SHTXT")).Text =
                Equipments[i].New.EquipmentDescription;
            ((GuiTextField) session.FindById(
                "wnd[0]/usr/subSUB_EQKO:SAPLITO0:0152/subSUB_0152B:SAPLITO0:1525/txtITOB-SHTXT")).CaretPosition = 24;
            ((GuiMainWindow) session.FindById("wnd[0]")).SendVKey(11);

            Equipments[i].Old.EquipmentDescription = Equipments[i].New.EquipmentDescription;
            var l = new Transaction();
            l.Equipment = Equipments[i];
            l.Remarks = ((GuiStatusbar) session.FindById("wnd[0]/sbar")).Text;
            l.Type = Transaction.TransactionTypes.EquipmentDescription;
            l.OldValue = Equipments[i].Old.EquipmentDescription;
            l.NewValue = Equipments[i].New.EquipmentDescription;

            Database.Transactions.Add(l);
            Database.SaveChanges();
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

        private void ReadFromRegisters()
        {
            var t = new Thread(() =>
            {
                ReadFar();
                ReadEqm();
                ProcessFar();
                Database.SaveChanges();
            });

            t.Start();
        }

        private void ProcessFar()
        {
            Console.WriteLine();
            var q = 1;
            for (var i = 0; i < AssetMaster.Count; i++)
            {
                var e = EquipmentMaster.FirstOrDefault(m => m.AssetNumber == AssetMaster[i].AssetNumber);
                if (e != null)
                {
                    AssetMaster[i].EquipmentNumber = e.EquipmentNumber;
                    AssetMaster[i].Old.EquipmentDescription = e.Old.EquipmentDescription.Trim();
                    AssetMaster[i].Old.OperationId = e.Old.OperationId;
                    AssetMaster[i].Old.SubType = e.Old.SubType;
                    AssetMaster[i].Old.Weight = e.Old.Weight;
                    AssetMaster[i].Old.WeightUnit = e.Old.WeightUnit;
                    AssetMaster[i].Old.Dimensions = e.Old.Dimensions;
                    AssetMaster[i].Old.ModelNumber = e.Old.ModelNumber;
                    AssetMaster[i].Old.SerialNumber = e.Old.SerialNumber;
                    AssetMaster[i].Old.EquipmentLocation = e.Old.EquipmentLocation;

                    AssetMaster[i].EquipmentNumber = e.EquipmentNumber;
                    AssetMaster[i].New.EquipmentDescription = e.New.EquipmentDescription.Trim();
                    AssetMaster[i].New.OperationId = e.New.OperationId;
                    AssetMaster[i].New.SubType = e.New.SubType;
                    AssetMaster[i].New.Weight = e.New.Weight;
                    AssetMaster[i].New.WeightUnit = e.New.WeightUnit;
                    AssetMaster[i].New.Dimensions = e.New.Dimensions;
                    AssetMaster[i].New.ModelNumber = e.New.ModelNumber;
                    AssetMaster[i].New.SerialNumber = e.New.SerialNumber;
                    AssetMaster[i].New.EquipmentLocation = e.New.EquipmentLocation;

                    Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                        (Action) (() =>
                            _statusTextBlock.Text = "Processing EQM & FAR " + q));

                    q++;
                    Database.Equipments.Add(AssetMaster[i]);
                }
            }

            Database.SaveChanges();
            Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                (Action) (() =>
                    _statusTextBlock.Text = "Database Updated"));
        }

        private void ReadEqm()
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

                    Dispatcher.BeginInvoke(DispatcherPriority.Normal,
                        (Action) (() =>
                            _statusTextBlock.Text = "Reading EQM " + i));
                }
            }
        }

        private void ReadFar()
        {
            var fi = new FileInfo(FAR);
            using (var excelPackage = new ExcelPackage(fi))
            {
                var myWorkbook = excelPackage.Workbook;
                var myWorksheet = myWorkbook.Worksheets[1];

                for (var i = 2; i < myWorksheet.Dimension.End.Row; i++)
                {
                    var e = new Equipment();
                    e.AcquisitionValue = float.Parse(myWorksheet.Cells[i, 6].Text.Trim().Replace(",", string.Empty));
                    e.BookValue = myWorksheet.Cells[i, 8].Text.Trim().Replace(",", string.Empty);
                    e.AcquisitionDate = DateTime.Parse(myWorksheet.Cells[i, 4].Value.ToString()).ToOADate()
                        .ToString(CultureInfo.CurrentCulture);
                    e.AssetNumber = myWorksheet.Cells[i, 1].Text.Trim();
                    e.EquipmentNumber = myWorksheet.Cells[i, 3].Text.Trim();
                    e.Old.AssetDescription = myWorksheet.Cells[i, 5].Text.Trim();
                    e.Old.AssetLocation = myWorksheet.Cells[i, 10].Text.Trim();
                    e.New.AssetDescription = myWorksheet.Cells[i, 5].Text.Trim();
                    e.New.AssetLocation = myWorksheet.Cells[i, 10].Text.Trim();
                    if (e.AssetNumber != "" && e.AssetNumber != "Asset") AssetMaster.Add(e);

                    Dispatcher.BeginInvoke(DispatcherPriority.Normal,
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