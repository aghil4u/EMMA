using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace EMMA.Helper_Classes
{

    public class DB
    {


        private ObservableCollection<StockItem> _database = new ObservableCollection<StockItem>();
        private ObservableCollection<Transaction> _transactions = new ObservableCollection<Transaction>();


        public ObservableCollection<StockItem> Database
        {
            get { return _database; }
            set { _database = value; }
        }

        public ObservableCollection<Transaction> Transactions
        {
            get { return _transactions; }
            set { _transactions = value; }
        }

        public void NewTransaction(StockItem item, double iqty, string iproject, Transaction.TransactionTypes itype)
        {
            Transaction iTransaction = new Transaction();
            iTransaction.ItemStockCode = item.StockCode;
            iTransaction.Qty = iqty;
            iTransaction.Project = iproject;
            iTransaction.Type = itype;
            iTransaction.Date = DateTime.Now;
            iTransaction.ItemDescription = item.FullDescription;
            Transactions.Add(iTransaction);

            Database.First((s) => s.StockCode == item.StockCode).PhysicalQty -= iqty;
            Database.First((s) => s.StockCode == item.StockCode).LastUpdatedPhysicalQty = DateTime.Now;


        }
    }
}
