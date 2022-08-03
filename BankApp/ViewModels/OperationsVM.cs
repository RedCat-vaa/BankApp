using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using BankApp.Models;
using BankApp.Views;
using System.Windows;

namespace BankApp.ViewModels
{
    class OperationsVM : INotifyPropertyChanged
    {
        private Operations opWin;
        private decimal ostatok;
        Account currentAccount;
        public decimal Ostatok
        {
            get { return ostatok; }
            set
            {
                ostatok = value;
                OnPropertyChanged("Ostatok");
            }
        }

        private decimal sum;
        public decimal Sum
        {
            get { return sum; }
            set
            {
                sum = value;
                OnPropertyChanged("Sum");
            }
        }

        private string infoOperation;
        public string InfoOperation
        {
            get { return infoOperation; }
            set
            {
                infoOperation = value;
                OnPropertyChanged("InfoOperation");
            }
        }

        public OperationsVM(Operations opWin, Account currentAccount)
        {
            this.opWin = opWin;
            this.currentAccount = currentAccount;
        }

        private MyCommand _GetOperation;
        public MyCommand GetOperation
        {
            get
            {
                return _GetOperation ??
                  (_GetOperation = new MyCommand(obj =>
                  {                   

                      opWin.CashWin.Content = new Cash();
                  }));
            }
        }

        //Получение остатка
        private MyCommand _GetSum;
        public MyCommand GetSum
        {
            get
            {
                return _GetSum ??
                  (_GetSum = new MyCommand(obj =>
                  {
                      Ostatok = new DB().GetSum(currentAccount);
                      opWin.SumWin.Content = new Sum();
                  }));
            }
        }

        //Снятие денег
        private MyCommand _StartOperation;
        public MyCommand StartOperation
        {
            get
            {
                return _StartOperation ??
                  (_StartOperation = new MyCommand(obj =>
                  {
                      if (Sum==0)
                      {
                          MessageBox.Show("Введите сумму для снятия денег");
                          return;
                      }
                      if (Sum< currentAccount.Sum)
                      {
                          currentAccount.Sum -= Sum;
                          Ostatok = currentAccount.Sum;
                          DB db = new DB();
                          db.StartOperation(currentAccount);
                          InfoOperation = "Операция выполнена: " + $"({DateTime.Now.ToShortDateString()}/{DateTime.Now.ToShortTimeString()}/777/" +
                      $"{currentAccount.Number.Substring(currentAccount.Number.Length - 4)} / {Sum} / {currentAccount.Sum})";
                      }
                      else
                      {
                          InfoOperation = "Недостаточно средств";
                      }                    
                  }));
            }
        }

        private MyCommand _Cancel;
        public MyCommand Cancel
        {
            get
            {
                return _Cancel ??
                  (_Cancel = new MyCommand(obj =>
                  {
                      opWin.Close();
                  }));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
    
}
