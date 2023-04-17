using System;
using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel;
using MySql.Data.MySqlClient;
using Mysqlx.Prepare;
using MySqlX.XDevAPI;
using Google.Protobuf.WellKnownTypes;
using System.Data.Common;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System.Data;
using dotenv.net;

namespace stock_trader_backend.Models { 

public class MySqlCommands
{
    private string connectionString;
    private MySqlConnection conn;
        
        MySqlCommand cmd;
        public MySqlCommands()
        {
            //Load Env Vars
            DotEnv.Load();
            string user = Environment.GetEnvironmentVariable("DB_USERNAME");
            string pwd = Environment.GetEnvironmentVariable("DB_PASSWORD");
            connectionString = "server=localhost;port=3306;database=stock_trader;user=" + user + ";password=" + pwd;
            conn = new MySqlConnection(connectionString);
            cmd = conn.CreateCommand();
        }

        //public List<object[]> SelectAll(String table)
        //{
        //    List<object[]> list;
        //    try
        //    {
        //        list = new List<object[]>();
        //        cmd.CommandText = "SELECT * FROM @table";
        //        cmd.Parameters.AddWithValue("@table", table);
        //        var reader = cmd.ExecuteReader();
        //        list = addToList(reader, list);
        //        reader.Close();
        //        return list;
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //    finally
        //    {
        //        conn.Dispose();
        //    }

        //    return new List<object[]>();
        //}
        public List<object[]> Select(string field)
        {
            List<object[]> list;
            try
            {
                conn.Open();
                if (field.Equals("*"))
                {
                    cmd.CommandText = "SELECT * FROM stocks";
                }
                else if (field.Equals("stock_ticker"))
                {
                    cmd.CommandText = "SELECT stock_ticker FROM stocks";
                }
                else
                {
                    cmd.CommandText = "SELECT * FROM market WHERE day_of_week = @field";
                }
                list = new List<object[]>();
                cmd.Parameters.AddWithValue("@field", field);
                var reader = cmd.ExecuteReader();
                list = addToList(reader, list);
                reader.Close();
                return list;
            }
            catch (Exception ex)
            {
                return new List<object[]>() { new object[] { ex.Message }};
            }
            finally
            {
                conn.Dispose();
            }

            //return new List<object[]>();
        }

        public List<object[]> Select(string stock_ticker, double price)
        {
            List<object[]> list;
            try
            {
                conn.Open();
                list = new List<object[]>();
                cmd.CommandText = "SELECT *,FORMAT(price,4) FROM limit_orders WHERE stock_ticker = @stock_ticker and price = @price";
                cmd.Parameters.AddWithValue("@stock_ticker", stock_ticker);
                cmd.Parameters.AddWithValue("@price", price);
                var reader = cmd.ExecuteReader();
                list = addToList(reader, list);
                reader.Close();
                return list;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Dispose();
            }

            return new List<object[]>();
        }

        public async Task<List<object[]>> SelectAsync(string table, string field)
        {
            List<object[]> list;
            try
            {
                await conn.OpenAsync();
                if (table.Equals("user_balance"))
                {
                    cmd.CommandText = "SELECT FORMAT(balance,2) FROM user_balance WHERE username = @username1;";
                    cmd.Parameters.AddWithValue("@username1", field);
                }
                else if (table.Equals("limit_orders"))
                {
                    cmd.CommandText = "SELECT * FROM limit_orders WHERE username = @username2;";
                    cmd.Parameters.AddWithValue("@username2", field);
                }
                else
                {
                    cmd.CommandText = "SELECT * FROM user_stocks WHERE username = @username3";
                    cmd.Parameters.AddWithValue("@username3", field);
                }
                list = new List<object[]>();
                var reader = await cmd.ExecuteReaderAsync();
                list = await addToListAsync(reader, list);
                await reader.CloseAsync();
                return list;
            } 
            catch (Exception ex)
            {
                return new List<object[]>() { new object[] { ex.Message } };
            }
            finally
            {
               conn.Dispose();
            }

            
        }

        public async Task<List<object[]>> SelectAsync(string role, string username, string pwd)
        {
            List<object[]> list;
            try
            {
                await conn.OpenAsync();
                if (role.Equals("admin"))
                {
                    cmd.CommandText = "SELECT * FROM admin WHERE admin_user_name = @username and admin_pwd = SHA2(@pwd,256)";
                }
                else
                {
                    cmd.CommandText = "SELECT * FROM users WHERE username = @username and user_pwd = SHA2(@pwd,256)";
                }
                list = new List<object[]>();
                
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@pwd", pwd);
                var reader = await cmd.ExecuteReaderAsync();
                list = await addToListAsync(reader, list);
                await reader.CloseAsync();
                return list;
            }
            catch (Exception ex)
            {
                return new List<object[]>() { new object[] { ex.Message } };
            }
            finally
            {
                await conn.DisposeAsync();
            }

           // return cmd.CommandText;
        }

        public List<object[]> SelectPrices(string stock_ticker, DateTime field)
        {
            List<object[]> list;
            try
            {
                conn.Open();
                list = new List<object[]>();
                cmd.CommandText = "SELECT FORMAT(price,4) FROM prices WHERE stock_ticker = @stock_ticker_prices and last_updated >= @date;";
                cmd.Parameters.AddWithValue("@stock_ticker_prices", stock_ticker);
                cmd.Parameters.AddWithValue("@date", field);
                var reader = cmd.ExecuteReader();
                list = addToList(reader, list);
                reader.Close();
                cmd.Parameters.Clear();
                return list;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Dispose();
            }

            return new List<object[]>();
        }

        public async Task<List<object[]>> SelectPricesAsync(string stock_ticker, DateTime date)
        {
            List<object[]> list;
            try
            {
                await conn.OpenAsync();
                list = new List<object[]>();
                cmd.CommandText = "SELECT FORMAT(price,4) FROM prices WHERE stock_ticker = @stock_ticker and last_updated >= @date;";
                cmd.Parameters.AddWithValue("@stock_ticker", stock_ticker);
                cmd.Parameters.AddWithValue("@date", date);
                var reader = await cmd.ExecuteReaderAsync();
                list = await addToListAsync(reader, list);
                await reader.DisposeAsync();
                return list;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                await conn.DisposeAsync();
            }

            return new List<object[]>();
        }

        public async Task<int> InsertAccount(string fullName, string username, string email, string pwd)
        {
            try
            {
                await conn.OpenAsync();
                cmd.CommandText = "INSERT INTO users (user_full_name, username, user_email, user_pwd) VALUES (@FullName, @UserName, @Email, SHA2(@Password, 256)); INSERT INTO user_balance (username, balance) VALUES (@UserName, 0.00);";
                cmd.Parameters.AddWithValue("@FullName", fullName);
                cmd.Parameters.AddWithValue("@UserName", username);
                cmd.Parameters.AddWithValue("@Email", email);
                cmd.Parameters.AddWithValue("@Password", pwd);
                return await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                await conn.DisposeAsync();
            }
            return 0;
        }

        public int InsertPrices(string stock_ticker, double price, DateTime date)
        {
            try
            {
                conn.Open();
                cmd.CommandText = "INSERT INTO prices (stock_ticker, price, last_updated) VALUES (@stock_ticker, @price, @date);";
                cmd.Parameters.AddWithValue("@stock_ticker", stock_ticker);
                
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                return 0;
            }
            finally
            {
                conn.Dispose();
            }

        }

        public async Task<int> InsertPricesAsync(string stock_ticker, double price, DateTime date)
        {
            try
            {
                conn.Open();
                cmd.CommandText = "INSERT INTO prices (stock_ticker, price, last_updated) VALUES (@stock_ticker, @price, @date);";
                cmd.Parameters.AddWithValue("@stock_ticker", stock_ticker);
                cmd.Parameters.AddWithValue("@price", price);
                cmd.Parameters.AddWithValue("@date", date.ToString());
                return await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                return 0;
            }
            finally
            {
                conn.Dispose();
            }


        }

        public object InsertUserStockTransaction(string transaction_type, string username, string stock_ticker, int quantity, double purchase_price )
        {
            try
            {
                conn.Open();
                cmd.CommandText = "INSERT INTO user_stocks (transaction_type, username, stock_ticker, quantity, date_purchased, purchased_price) VALUES (@transcation_type, @username, @stock_ticker, @quantity, @date_purchased, @purchased_price);";
                cmd.Parameters.AddWithValue("@transaction_type", transaction_type);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@stock_ticker", stock_ticker);
                cmd.Parameters.AddWithValue("@quantity", quantity);
                cmd.Parameters.AddWithValue("@date_purchased", DateTime.Now);
                cmd.Parameters.AddWithValue("@purchased_price", purchase_price);
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                return (ex.Message);
            }
            finally
            {
                conn.Dispose();
            }
           
        }
        public async Task<int> InsertUserStockTransactionAsync(string transaction_type, string username, string stock_ticker, int quantity, double purchase_price )
        {
            try
            {
                conn.Open();
                cmd.CommandText = "INSERT INTO user_stocks (transaction_type, username, stock_ticker, quantity, date_purchased, purchased_price) VALUES (@transaction_type, @username, @stock_ticker1, @quantity, @date_purchased, @purchased_price);";
                cmd.Parameters.AddWithValue("@transaction_type", transaction_type);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@stock_ticker1", stock_ticker);
                cmd.Parameters.AddWithValue("@quantity", quantity);
                cmd.Parameters.AddWithValue("@date_purchased", DateTime.Now);
                cmd.Parameters.AddWithValue("@purchased_price", purchase_price);
                return await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                return 0;
            }
            finally
            {
                conn.Dispose();
            }
            //return 0;
           
        }
        public async Task<int> InsertLimitOrder(string order_id, string username, string stock_ticker, string order_type, double price, int quantity, DateTime expire_date)
        {
            try
            {
                await conn.OpenAsync();
                cmd.CommandText = "INSERT INTO limit_orders (order_id, username, stock_ticker, order_type, price, quantity, expire_date) VALUES (@order_id, @username, @stock_ticker, @order_type,  @price, @quantity, @expire_date);";
                cmd.Parameters.AddWithValue("@order_id", order_id);
                cmd.Parameters.AddWithValue("@username", username);
                cmd.Parameters.AddWithValue("@stock_ticker", stock_ticker);
                cmd.Parameters.AddWithValue("@order_type", order_type);
                cmd.Parameters.AddWithValue("@price", price);
                cmd.Parameters.AddWithValue("@quantity", quantity);
                cmd.Parameters.AddWithValue("@expire_date", expire_date.Date);
                return await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                await conn.DisposeAsync();
            }
            return 0;
            
        }

        public async Task<int> InsertNewStock(string company_name, string stock_ticker, int volume, double initialPrice)
        {

            try
            {
                await conn.OpenAsync();
                cmd.CommandText = "INSERT INTO stocks (company_name, stock_ticker, volume) VALUES (@company_name, @stock_ticker, @volume); INSERT INTO prices (stock_ticker, price, last_updated) VALUES (@stock_ticker, @price, NOW());";
                //cmd.CommandText = "INSERT INTO stocks (company_name, stock_ticker, volume) VALUES (@company_name, @stock_ticker , @volume);";
                cmd.Parameters.AddWithValue("@company_name", company_name);
                cmd.Parameters.AddWithValue("@stock_ticker", stock_ticker);
                cmd.Parameters.AddWithValue("@volume", volume);
                cmd.Parameters.AddWithValue("@price", initialPrice);
                //cmd.Parameters.AddWithValue("@date", DateTime.Now.ToString());
                //int result = await InsertPricesAsync(stock_ticker, initialPrice, DateTime.Now);
                return (await cmd.ExecuteNonQueryAsync());
            }
            catch (Exception ex)
            {
                return 0;
            }
            finally
            {
                await conn.DisposeAsync();
            }
          
        }
        public int DeleteRecord(string order_id)
        {
            try
            {
                conn.Open();
                cmd.CommandText = "DELETE FROM limit_orders WHERE order_id = @order_id;";
                cmd.Parameters.AddWithValue("@order_id", order_id);
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Dispose();
            }
            return 0;
        }
        public async Task<int> DeleteRecordAsync(string order_id)
        {
            try
            {
                await conn.OpenAsync();
                cmd.CommandText = "DELETE FROM limit_orders WHERE order_id = @order_id;";
                cmd.Parameters.AddWithValue("@order_id", order_id);
                return await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                await conn.DisposeAsync();
            }
            return 0;
        }

        public int UpdateVolume(string stock_ticker, int field)
        {
            try
            {
                conn.Open();
                cmd.CommandText = "UPDATE stocks SET volume = volume + @field WHERE stock_ticker = @stock_ticker_volume;";
                cmd.Parameters.AddWithValue("@stock_ticker_volume", stock_ticker);
                cmd.Parameters.AddWithValue("@field", field);
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Dispose();
            }
            return 0;
        }
        public async Task<int> UpdateVolumeAsync(string stock_ticker, int volume)
        {
            try
            {
                await conn.OpenAsync();
                cmd.CommandText = "UPDATE stocks SET volume = volume + @volume WHERE stock_ticker = @stock_ticker_volume;";
                cmd.Parameters.AddWithValue("@stock_ticker_volume", stock_ticker);
                cmd.Parameters.AddWithValue("@volume", volume);
                return await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                return 0;
            }
            finally
            {
                await conn.DisposeAsync();
            }
        }
        public int UpdateBalance(string username, double field)
        {
            try
            {
                conn.Open();
                cmd.CommandText = "UPDATE user_balance SET balance = @field WHERE username = @username_balance;";
                cmd.Parameters.AddWithValue("@field", field);
                cmd.Parameters.AddWithValue("@username_balance", username);
                return cmd.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Dispose();
            }
            return 0;
        }

        public async Task<int> UpdateBalanceAsync(string username, double balance)
        {
            try
            {
                await conn.OpenAsync();
                cmd.CommandText = "UPDATE user_balance SET balance = @balance WHERE username = @username_balance;";
                cmd.Parameters.AddWithValue("@username_balance", username);
                cmd.Parameters.AddWithValue("@balance", balance);
                return await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                await conn.DisposeAsync();
            }
            return 0;
        }

        public int UpdateTimeSchedule(ScheduleObject day)
        {
            try
            {
                var result = 0;
                conn.Open();
                    string holiday;
                    cmd.Parameters.Clear();
                    if(day.dayOfWeek == "Saturday" || day.dayOfWeek == "Sunday")
                    {
                        holiday = "true";
                    }
                    else
                    {
                        holiday = day.isHoliday.ToString() ?? "false";
                    }
                    cmd.CommandText = "UPDATE market SET start_time = @startTime, end_time = @endTime, is_holiday = @isHoliday WHERE day_of_week = @day;";
                    cmd.Parameters.AddWithValue("@startTime", Convert.ToDateTime(day.startTime));
                    cmd.Parameters.AddWithValue("@endTime", Convert.ToDateTime(day.endTime));
                    cmd.Parameters.AddWithValue("@isHoliday", holiday);
                    cmd.Parameters.AddWithValue("@day", day.dayOfWeek);
                    result = result + cmd.ExecuteNonQuery();
                    cmd.Parameters.Clear();
                if(result > 0)
                {
                    return result;
                }             
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                conn.Dispose();
            }

            return 0;
        }
        //public async Task<int> UpdateHolidaySchedule(List<ScheduleObject> schedule)
        //{
        //    try
        //    {
        //        cmd.CommandText = "UPDATE market SET startTime = CASE @holidayColumQuery END WHERE dayOfWeek IN (@whereQuery);";
        //        var holidayColumnQuery = "";
        //        var whereQuery = new List<string>();
        //        foreach (ScheduleObject day in schedule)
        //        {
        //            holidayColumnQuery = holidayColumnQuery + String.Format("WHEN dayOfWeek = {0} THEN {1} ", day.dayOfWeek ?? "", day.isHoliday ?? "true");
        //            whereQuery.Add((day.dayOfWeek ?? ""));
        //        }
        //        cmd.Parameters.AddWithValue("@holidayColumnQuery", holidayColumnQuery);
        //        cmd.Parameters.AddWithValue("@whereQuery", string.Join(",", whereQuery));
        //        return await cmd.ExecuteNonQueryAsync();
        //}
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine(ex.Message);
        //    }
        //    finally
        //    {
        //        conn.Dispose();
        //    }

        //    return 0;
        //}

        private async Task<List<object[]>> addToListAsync(DbDataReader reader, List<object[]> list)
        {
            while (await reader.ReadAsync())
            {
                var row = new object[reader.FieldCount];
                reader.GetValues(row);
                list.Add(row);
            }
            reader.Dispose();
        return list;
        }

        private List<object[]> addToList(MySqlDataReader reader, List<object[]> list)
        {
            while (reader.Read())
            {
                var row = new object[reader.FieldCount];
                reader.GetValues(row);
                list.Add(row);
            }
        return list;
        }



        
}

}
