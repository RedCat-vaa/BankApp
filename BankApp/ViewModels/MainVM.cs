using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using BankApp.Models;
using System.Linq;
using System.Windows;

namespace BankApp.ViewModels
{
    public class MainVM : INotifyPropertyChanged
    {
        private string number;
        private MainWindow mainWin;

        public string Number
        {
            get { return number; }
            set
            {
                number = value;
                OnPropertyChanged("Number");
            }
        }
        public MainVM(MainWindow mainWin)
        {
            this.mainWin = mainWin;
        }

        private MyCommand _GetCard;
        public MyCommand GetCard
        {
            get
            {

                return _GetCard ??
                  (_GetCard = new MyCommand(obj =>
                  {
                      Account currentAccount = null;
                      DB db = new DB();
                      currentAccount = db.GetCard(Number);

                      if (currentAccount != null)
                      {
                          if (currentAccount.IsOpen == false)
                          {
                              MessageBox.Show("Карта заблокирована");
                              return;
                          }
                          InputPin newWindow = new InputPin();
                          PinVM pinVM = new PinVM(newWindow, currentAccount);
                          newWindow.Closed += (object sender, EventArgs e) =>
                          {
                              mainWin.Visibility = System.Windows.Visibility.Visible;
                          };
                          newWindow.DataContext = pinVM;
                          mainWin.Hide();
                          newWindow.ShowDialog();
                      }
                      else
                      {
                          MessageBox.Show("Карта не найдена");
                      }

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
                      Number = "";
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
