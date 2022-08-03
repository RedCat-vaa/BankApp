using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Data;
using Microsoft.Data.Sqlite;

namespace BankApp.Models
{
    public class DB
    {
        string connectionString = String.Empty;

        public DB()
        {
            connectionString = "Data Source=Bankomat.db";
            //FillDB();
        }

        //Запись в базу данных количества оставшихся попыток для ввода пин-кода
        public void ChangeAttempts(Account currentAccount)
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand();
                command.Connection = connection;

                if (currentAccount.Attempts < 3)
                {
                    command.CommandText =
                "UPDATE PinCodeInfo SET Attempts = @attempts WHERE AccountId = @id";
                }
                else
                {
                    command.CommandText =
                "UPDATE PinCodeInfo SET Attempts = @attempts WHERE AccountId = @id; " +
                "UPDATE Accounts SET IsOpen = @isopen WHERE Id = @id";
                    currentAccount.Attempts = 0;
                    currentAccount.IsOpen = false;
                    SqliteParameter isopenParam = new SqliteParameter("@isopen", Convert.ToInt32(currentAccount.IsOpen));
                    command.Parameters.Add(isopenParam);
                }

                SqliteParameter idParam = new SqliteParameter("@id", currentAccount.Id);
                command.Parameters.Add(idParam);

                SqliteParameter attempsParam = new SqliteParameter("@attempts", currentAccount.Attempts);
                command.Parameters.Add(attempsParam);
                command.ExecuteNonQuery();
            }
        }

        //Получение данных по карте
        public Account GetCard(string number)
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand();
                command.Connection = connection;
                command.CommandText = "SELECT Accounts.Id, Accounts.Number, PinCodeInfo.PinCode, " +
                    "Accounts.IsOpen, PinCodeInfo.Attempts, CashData.Sum FROM Accounts " +
                    "LEFT JOIN CashData ON Accounts.Id = CashData.AccountId " +
                    "LEFT JOIN PinCodeInfo ON Accounts.Id = PinCodeInfo.AccountId " +
                    "WHERE Accounts.Number = @number";
                SqliteParameter numberParam = new SqliteParameter("@number", number);
                command.Parameters.Add(numberParam);
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            try
                            {
                                int id = Convert.ToInt32(reader.GetValue(0));
                                string numberDb = Convert.ToString(reader.GetValue(1));
                                string pincode = Convert.ToString(reader.GetValue(2));
                                bool isopen = Convert.ToBoolean(Convert.ToInt32(reader.GetValue(3)));
                                int attemps = Convert.ToInt32(reader.GetValue(4));
                                decimal sum = Convert.ToInt32(reader.GetValue(5));
                                Account account = new Account()
                                {
                                    Id = id,
                                    Number = numberDb,
                                    IsOpen = isopen,
                                    Attempts = attemps,
                                    PinCode = pincode,
                                    Sum = sum
                                };
                                return account;
                            }
                            catch
                            {
                                return null;
                            }
                        }
                    }

                }

            }
            return null;
        }

        //Получение остатка
        public int GetSum(Account currentAccount)
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand();
                command.Connection = connection;
                command.CommandText = "SELECT Sum FROM CashData " +
                    "WHERE AccountId = @id";
                SqliteParameter idParam = new SqliteParameter("@id", currentAccount.Id);
                command.Parameters.Add(idParam);
                using (SqliteDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            try
                            {
                                int sum = Convert.ToInt32(reader.GetValue(0));
                                return sum;
                            }
                            catch
                            {
                                return 0;
                            }
                        }
                    }

                }

            }
            return 0;
        }

        //Снятие денег
        public void StartOperation(Account currentAccount)
        {
            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand();
                command.Connection = connection;

                command.CommandText = "UPDATE CashData SET Sum = @sum WHERE AccountId = @id";
                SqliteParameter idParam = new SqliteParameter("@id", currentAccount.Id);
                command.Parameters.Add(idParam);
                SqliteParameter sumParam = new SqliteParameter("@sum", currentAccount.Sum);
                command.Parameters.Add(sumParam);
                command.ExecuteNonQuery();
            }
        }

        //Метод для заполнения начальных данных
        private void FillDB()
        {

            using (SqliteConnection connection = new SqliteConnection(connectionString))
            {
                connection.Open();

                SqliteCommand command = new SqliteCommand();
                command.Connection = connection;
                command.CommandText = "INSERT INTO Accounts (Id, Number, IsOpen) " +
                    "VALUES (1, '1111-1111-1111-1111', 1); " +
                    "INSERT INTO PinCodeInfo (PinCode, Attempts, AccountId) " +
                    "VALUES ('1234', 0, 1); " +
                    "INSERT INTO CashData (Sum, AccountId) " +
                    "VALUES (100000, 1); ";
                command.ExecuteNonQuery();

                command.CommandText = "INSERT INTO Accounts (Id, Number, IsOpen) " +
                   "VALUES (2, '2222-2222-2222-2222', 1); " +
                   "INSERT INTO PinCodeInfo (PinCode, Attempts, AccountId) " +
                   "VALUES ('2222', 0, 2); " +
                   "INSERT INTO CashData (Sum, AccountId) " +
                   "VALUES (60000, 2); ";
                command.ExecuteNonQuery();

                command.CommandText = "INSERT INTO Accounts (Id, Number, IsOpen) " +
                   "VALUES (3, '1234-5555-6666-7777', 1); " +
                   "INSERT INTO PinCodeInfo (PinCode, Attempts, AccountId) " +
                   "VALUES ('4321', 0, 3); " +
                   "INSERT INTO CashData (Sum, AccountId) " +
                   "VALUES (80000, 3); ";
                command.ExecuteNonQuery();
            }
        }

    }
}
