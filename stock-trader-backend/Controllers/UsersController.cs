using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json.Linq;
using stock_trader_backend.Models;
using System.Text;
using System.Text.Json;
using Google.Protobuf;
using System.Threading.Tasks;

namespace stock_trader_backend.Controllers
{ 
    [Route("[controller]")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private MySqlCommands commands;
        public UsersController()
        {
            commands = new MySqlCommands();
        }


        [HttpPost("create")]
        public async Task<IActionResult> CreateAccount([FromBody] AccountData data) 
        {
            var message = new { message = "Account could not be created" };
            try
            {
                var AccountData = new AccountData
                {
                    fullName = data.fullName ?? throw new ArgumentException("Invalid Full Name"),
                    username = data.username ?? throw new ArgumentException("Invalid Username"),
                    email = data.email ?? throw new ArgumentException("Invalid Email"),
                    pwd = data.pwd ?? throw new ArgumentException("Invalid Password")
                };

                    int result = await commands.InsertAccount(AccountData.fullName, AccountData.username, AccountData.email, AccountData.pwd);
                    message = new { message = result.ToString() };
                    if (result > 0)
                    {
                        message = new { message = "Account was created" };
                        return Ok(message);
                    }
                
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                message = new { message = ex.Message };
            }

            return BadRequest(message);
        }

        [HttpPost("buy-stock")]
        public async Task<IActionResult> BuyStock([FromBody] StockObject data)
        {
            var message = new { message = "Stock could not be bought" };
            try
            {
                MarketController market = new MarketController();
                if (!(market.checkSchedule()))
                {
                    return BadRequest(new { message = "Market is closed right now "});
                }
                string username = HttpContext.Request.Cookies["Name"] ?? throw new ArgumentException("Invalid Username");
                string stock_ticker = data.stock_ticker;
                double price = Convert.ToDouble((await commands.SelectPricesAsync(data.stock_ticker, DateTime.Today.AddDays(-2)))[0].Last());
                message = new { message = "price " + price.ToString() };
                double balance = Convert.ToDouble((await commands.SelectAsync("user_balance", username))[0][0]);
                if (balance <= 0)
                {
                    return BadRequest(new { message = "Insufficient funds in account" });
                }
                //message = new { message = "balance " + balance.ToString()};
                    
                if (price * data.volume > balance)
                {
                    return BadRequest(new { message = "Insufficient funds in account" });
                }
                else
                {
                    int result1 = await commands.InsertUserStockTransactionAsync("bought", username, data.stock_ticker, data.volume ?? 0, price);
                    int result2 = await commands.UpdateBalanceAsync(username, balance - price * data.volume ?? 0);
                    int result3 = await commands.UpdateVolumeAsync(data.stock_ticker, data.volume ?? 0);
                    if (result1 > 0 && result2 > 0 && result3 > 0)
                    {
                        message = new { message = "Stock was bought" };
                        return Ok(message);
                    }

                }
            }
            catch (Exception ex)
            {
                message = new { message = "There was an error with your purchase"};
            }
            return BadRequest(message);
        }

        [HttpPost("sell-stock")]
        public async Task<IActionResult> SellStock([FromBody] StockObject data)
        {
            var message = new { message = "Stock could not be sold" };
            try
            {
                    MarketController market = new MarketController();
                    if (!(market.checkSchedule()))
                    {
                        return BadRequest(new { message = "Market is closed right now "});
                    }
                    string username = HttpContext.Request.Cookies["Name"] ?? throw new ArgumentException("Invalid Username");
                    int quantity = data.volume ?? 0;
                    double price = Convert.ToDouble((await commands.SelectPricesAsync(data.stock_ticker, DateTime.Today.AddDays(-2)))[0].Last());
                    double balance = Convert.ToDouble((await commands.SelectAsync("user_balance", username))[0][0]);
                    if (quantity > (await stockPortfolio(username))[data.stock_ticker])
                    {
                        message = new { message = "Insufficient funds in account" };
                    }
                    else
                    {
                        int result1 = await commands.InsertUserStockTransactionAsync("sold", username, data.stock_ticker, quantity, price);
                        int result2 = await commands.UpdateBalanceAsync(username, balance + (price * quantity));
                        int result3 = await commands.UpdateVolumeAsync(data.stock_ticker, quantity);
                        if (result1 > 0 && result2 > 0 || result3 > 0)
                        {
                            message = new { message = "Stock was sold" };
                        }
                    }

            }
            catch (Exception ex)
            {
                message = new { message = ex.Message };
            }
            return BadRequest(message);
        }

        [HttpPost(("limit-order-buy"))]
        public async Task<IActionResult> LimitOrderBuy([FromBody] StockObject data)
        {
            var message = new { message = "Selling Order not set" };
            try
            {
                    string order_id = generateId(8);
                    string username = HttpContext.Request.Cookies["Name"] ?? throw new ArgumentException("Invalid Username");
                    int result = await commands.InsertLimitOrder(order_id, username, data.stock_ticker, "buy", Convert.ToDouble(data.current_price), data.volume ?? 0, data.expire_date ?? DateTime.Today);
                    if (result > 0)
                    {
                        message = new { message = "Selling Order set" };
                    }
                    return Ok(message);
            }
            catch (Exception ex)
            {
                message = new { message = ex.Message };
            }
            return BadRequest(message);
        }

        [HttpPost("limit-order-sell")]
        public async Task<IActionResult> LimitOrderSell([FromBody] StockObject data)
        {
            var message = new { message = "Selling Order not set" };
            try
            {
                string order_id = generateId(8);
                string username = HttpContext.Request.Cookies["Name"] ?? throw new ArgumentException("Invalid Username");
                int result = await commands.InsertLimitOrder(order_id, username, data.stock_ticker, "sell", Convert.ToDouble(data.current_price), data.volume ?? 0, data.expire_date ?? DateTime.Today);
                if (result > 0)
                {
                    message = new { message = "Selling Order set" };
                }
                return Ok(message);
            }
            catch (Exception ex)
            {
                message = new { message = ex.Message };
            }
            return BadRequest(message);
        }

        [HttpDelete("delete-order/{order_id}")]
        public async Task<IActionResult> CancelLimitOrder([FromRoute] string order_id)
        {

            var message = new { message = "Limit Order not canceled"};
            try
            {
                    
                    int result = await commands.DeleteRecordAsync(order_id);
                    if (result > 0)
                    {
                        message = new { message = "Limit Order canceled" };
                        return Ok(message);
                    }
                    
            }
            catch (Exception ex)
            {
                message = new { message = ex.Message };
            }
            return BadRequest(message);
        }

        [HttpGet("get-limit-orders")]
        public async Task<IActionResult> GetLimitOrders()
        {

            var message = new { message = "Limit Order not retrieved" };
            var limitOrders = new List<LimitOrderObject>();
            try
            {
                    string username = HttpContext.Request.Cookies["Name"] ?? throw new ArgumentException("Invalid Username");
                    var list = await commands.SelectAsync("limit_orders", username);
                    foreach (object[] row in list)
                    {
                        //message = new { message = row[0].ToString() };
                        limitOrders.Add(new LimitOrderObject { order_id = row[1].ToString(), stock_ticker = row[3].ToString(), order_type = row[4].ToString(), price = Convert.ToDouble(row[5]).ToString("C2"), quantity = Convert.ToInt16(row[6]), expireDate = Convert.ToDateTime(row[7])});
                    }
                    return Ok(limitOrders);

            }
            catch (Exception ex)
            {
                message = new { message = ex.Message };
            }
            return BadRequest(message);
        }

        [HttpPut("deposit-funds")]
        public async Task<IActionResult> depositFunds([FromBody] JsonElement data)
        {
            var message = new { message = "Funds could not be deposited" };
            try
            {
                    double funds = Convert.ToDouble(data.GetProperty("amount").ToString());
                    string username = HttpContext.Request.Cookies["Name"] ?? throw new ArgumentException("Invalid Username");
                    double balance = Convert.ToDouble((await commands.SelectAsync("user_balance", username))[0][0]);
                    int result = await commands.UpdateBalanceAsync(username, balance + funds);
                    if (result > 0)
                    {
                        message = new { message = "Funds deposited" };
                        Ok(message);
                    }
            }
            catch (Exception ex)
            {
                message = new { message = ex.Message };
            }
            return BadRequest(message);
        }

        [HttpPut("withdraw-funds")]
        public async Task<IActionResult> withdrawFunds([FromBody] JsonElement data)
        {
            var message = new { message = "Funds could not be withdrawn" };
            try
            {
                double funds = Convert.ToDouble(data.GetProperty("amount").ToString());
                string username = HttpContext.Request.Cookies["Name"] ?? throw new ArgumentException("Invalid Username");
                double balance = Convert.ToDouble((await commands.SelectAsync("user_balance", username))[0][0]);
                int result = await commands.UpdateBalanceAsync(username, balance - funds);
                if (result > 0)
                {
                    message = new { message = "Funds withdrawn" };
                    Ok(message);
                }
            }
            catch (Exception ex)
            {
                message = new { message = ex.Message };
            }
            return BadRequest(message);
        }

        [HttpGet("view-history")]
        public async Task<IActionResult> viewHistory()
        {
            try
            {
                string username = HttpContext.Request.Cookies["Name"] ?? throw new ArgumentException("Invalid Username");
                var list = await commands.SelectAsync("user_stocks", username);
                var stock_list = new List<StockHistory>();
                foreach (object[] row in list)
                {
                    var stock_event = new StockHistory
                    {
                        transaction_type = row[1].ToString().ToUpper(),
                        stock_ticker = row[3].ToString(),
                        quantity = Convert.ToInt16(row[4]),
                        datePurchased = Convert.ToDateTime(row[5]),
                        purchasedPrice = Convert.ToDouble(row[6])
                    };

                    stock_list.Add(stock_event);
                }
            
                return Ok(stock_list);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return BadRequest("Could not access history");
        }

        [HttpGet("view-assets")]
        public async Task<IActionResult> viewAssets()
        {
            try
            {
                string username = HttpContext.Request.Cookies["Name"] ?? throw new ArgumentException("Invalid Username");
                var stocks = await stockPortfolio(username);
                var balance = await commands.SelectAsync("user_balance", username);
                object funds = "0.00";
                foreach (object[] row in balance)
                {
                    funds = row[0];
                }
                //return Ok(new { message = funds });
                return Ok(new { stocks = stocks, cash = funds });
            }
            catch (Exception ex)
            {
               return BadRequest(new { message = ex.Message });
            }

            //return BadRequest(new { message = "Could not access assets" });
        }


        public async Task<Dictionary<string, int>> stockPortfolio(string username)
        {
            Dictionary<string, int> stockPortfolio = new Dictionary<string, int>();
            try
            {
                var list = await commands.SelectAsync("user_stocks", username);
                foreach (object[] stock in list)
                {
                    string stock_ticker = stock?[3].ToString() ?? "";
                    int quantity = Convert.ToInt16((stock?[4]));
                    string transaction = stock?[1].ToString() ?? "";
                    if (!(stockPortfolio.ContainsKey(stock_ticker)))
                    {
                        if (transaction.Equals("bought"))
                        {

                            stockPortfolio.Add(stock_ticker, quantity);
                        }
                        else if (transaction.Equals("sold"))
                        {
                            stockPortfolio.Add(stock_ticker, -(quantity));
                        }
                    }
                    else
                    {
                        int previousQuantity = stockPortfolio[stock_ticker];
                        if (transaction.Equals("bought"))
                        {

                            stockPortfolio[stock_ticker] = previousQuantity + quantity;
                        }
                        else if (transaction.Equals("sold"))
                        {
                            stockPortfolio[stock_ticker] = previousQuantity - quantity;
                        }
                    }

                    if (stockPortfolio[stock_ticker] <= 0)
                    {
                        stockPortfolio.Remove(stock_ticker);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return stockPortfolio;
        }

        private string generateId(int length)
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var idBuilder = new StringBuilder(length);

            for (int i = 0; i < length; i++)
            {
                idBuilder.Append(chars[random.Next(chars.Length)]);
            }

            return idBuilder.ToString();
        }

    }

}