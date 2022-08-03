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
    public class PinVM : INotifyPropertyChanged
    {
        private string pincode;
        Account currentAccount;
        private InputPin pinWin;

        public string Pincode
        {
            get { return pincode; }
            set
            {
                pincode = value;
                OnPropertyChanged("Pincode");
            }
        }
        public PinVM(InputPin pinWin, Account currentAccount)
        {
            this.pinWin = pinWin;
            this.currentAccount = currentAccount;
        }

        private MyCommand _GetPin;
        public MyCommand GetPin
        {
            get
            {
                return _GetPin ??
                  (_GetPin = new MyCommand(obj =>
                  {
                      if (currentAccount.PinCode != Pincode)
                      {
                          MessageBox.Show("Пин-код введен неверно");
                          currentAccount.Attempts++;
                          DB db = new DB();
                          db.ChangeAttempts(currentAccount);
                          if (currentAccount.IsOpen == false)
                          {
                              MessageBox.Show("Карта заблокирована");
                              pinWin.Close();
                          }
                          return;
                      }
                      else
                      {
                          currentAccount.Attempts = 0;
                          DB db = new DB();
                          db.ChangeAttempts(currentAccount);
                      }
                      
                      Operations newWindow = new Operations();
                      OperationsVM opVM = new OperationsVM(newWindow, currentAccount);
                      newWindow.Closed += (object sender, EventArgs e) =>
                      {
                          pinWin.Visibility = System.Windows.Visibility.Visible;
                      };
                      newWindow.DataContext = opVM;
                      pinWin.Hide();
                      newWindow.ShowDialog();
                  }));
            }
        }

        private MyCommand _DeleteNumber;
        public MyCommand DeleteNumber
        {
            get
            {
                return _DeleteNumber ??
                  (_DeleteNumber = new MyCommand(obj =>
                  {
                      Pincode = "";
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
                      pinWin.Close();
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
