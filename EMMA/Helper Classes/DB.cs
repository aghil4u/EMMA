using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace EMMA.Helper_Classes
{

    public class DB
    {


        private ObservableCollection<Equipment> _database = new ObservableCollection<Equipment>();
        private ObservableCollection<Transaction> _transactions = new ObservableCollection<Transaction>();


        public ObservableCollection<Equipment> Database
        {
            get { return _database; }
            set { _database = value; }
        }

        public ObservableCollection<Transaction> Transactions
        {
            get { return _transactions; }
            set { _transactions = value; }
        }

        public void NewTransaction(Equipment item, double iqty, string iproject, Transaction.TransactionTypes itype)
        {
            Transaction iTransaction = new Transaction();
            iTransaction.ItemStockCode = item.EquipmentNumber;
            iTransaction.Qty = iqty;
            iTransaction.Project = iproject;
            iTransaction.Type = itype;
            iTransaction.Date = DateTime.Now;
            iTransaction.ItemDescription = item.Description;
            Transactions.Add(iTransaction);

            Database.First((s) => s.EquipmentNumber == item.EquipmentNumber).PhysicalQty -= iqty;
            Database.First((s) => s.EquipmentNumber == item.EquipmentNumber).LastUpdatedPhysicalQty = DateTime.Now;


        }
    }
}
