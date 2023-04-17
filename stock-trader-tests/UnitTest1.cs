namespace stock_trader_tests;

using Microsoft.AspNetCore.Mvc;
using Mysqlx.Crud;
using stock_trader_backend.Models;
using static System.Runtime.InteropServices.JavaScript.JSType;

[TestClass]
public class UnitTest1
{
    [DataTestMethod]
    [DataRow("AAPL", 13.00, "2022-01-01 12:00:00", 1)]
    [DataRow("MSFT", 2.10, "2023-04-02 12:00:00", 1)]
    public void MySqlCommandsInsertPricesAction(string stock_ticker, double price, string dateTime, int actual)
    {
        var command = new MySqlCommands();

        var result = command.InsertPrices(stock_ticker, price, DateTime.Parse(dateTime));

        Assert.AreEqual(result, actual);


    }



    [TestMethod]
    [DataRow("user_balance", "aryemansingh", 0.0)]
    public async Task MySqlCommandsSelectAsyncAction(string table, string stock_ticker, double balance)
    {
        var actual = new List<object[]>() { new object[] { balance } };

        var command = new MySqlCommands();

        var result = await command.SelectAsync(table, stock_ticker);

        Assert.AreEqual(result[0][0], actual[0][0]);


    }



}
