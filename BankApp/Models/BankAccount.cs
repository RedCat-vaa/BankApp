using System;
using System.Collections.Generic;
using System.Text;

namespace BankApp.Models
{
    public class Account
    {
        public int Id { get; set; }
        //Номер карты
        public string Number { get; set; }
        //Пин-код
        public string PinCode { get; set; }
        //Открыт ли счет или закрыт
        public bool IsOpen { get; set; }
        //Счетчик попыток ввода пин-кода
        public int Attempts { get; set; }
        //Остаток на счете
        public decimal Sum { get; set; } = 0;
    }
}
