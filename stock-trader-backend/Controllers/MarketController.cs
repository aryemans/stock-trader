using System;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using stock_trader_backend.Models;
using System.Text;
using System.Text.Json;
using Google.Protobuf;
namespace stock_trader_backend.Controllers
{

	[ApiController]
	[Route("[controller]")]
	public class MarketController : ControllerBase
	{
		private MySqlCommands command;
		public MarketController()
		{
			command = new MySqlCommands();
		}
		public async void setMarketPrices()
		{
			int interval = 5;
			try
			{
				while (true)
				{
					DateTime previousUpdate = DateTime.Now - TimeSpan.FromMinutes(interval);
					var stockList = command.Select("stock_ticker");
					foreach (object stock_ticker in stockList)
					{
						var priceList = command.SelectPrices(stock_ticker.ToString(), previousUpdate);
						command.InsertPrices(stock_ticker.ToString(), Convert.ToDouble(priceList[0]), DateTime.Now);
						checkLimitOrders(Convert.ToDouble(priceList[0]), stock_ticker.ToString());
					}
					await Task.Delay(TimeSpan.FromSeconds(interval));
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		[HttpGet("stock-market")]
		public IActionResult getMarketPrices()
		{
			var list = command.Select("*");
			//return BadRequest(list[0][0]);
			var response = new List<StockObject>();
			
			foreach (object[] row in list)
			{
				try
				{
					var pricelist = command.SelectPrices(row[2].ToString() ?? "", DateTime.Now.AddDays(-2));
					if (pricelist.Count == 0)
					{
						throw new Exception("The stock does not exist");
					}
					else
					{
						var prices = new List<double>();
						foreach (object[] priceArray in pricelist)
						{
							prices.Add(Convert.ToDouble(priceArray[0]));
						}
						double openingPrice = Convert.ToDouble((prices[0]));
						double currentPrice = Convert.ToDouble((prices.Last()));
						double marketCapitalization = openingPrice * Convert.ToInt16(row[3]);
						double high = Convert.ToDouble((prices.Max()));
						double low = Convert.ToDouble(prices.Min());
						response.Add(new StockObject { companyName = row[1].ToString() ?? "", stock_ticker = row[2].ToString() ?? "", volume = (Convert.ToInt16((row[3]))), opening_price = openingPrice.ToString("C2"), current_price = currentPrice.ToString("C2"), high = high.ToString("C2"), low = low.ToString("C2"), market_capitalization = marketCapitalization });
						
					}
				}
				catch (Exception ex)
				{
                    return BadRequest("Could not get prices");
                }
			}
            return Ok(response);
            
		}

        [HttpGet("current-price/{stock_ticker}")]
        public IActionResult getCurrentPrice([FromRoute] string stock_ticker)
        {
			try
			{
				var pricelist = command.SelectPrices(stock_ticker, DateTime.Today.AddDays(-2));
				if (pricelist.Count == 0)
				{
					throw new Exception("The stock does not exist");
				}
				else
				{
					var prices = new List<double>();
					foreach (object[] priceArray in pricelist)
					{
						prices.Add(Convert.ToDouble(priceArray[0]));
					}
					double currentPrice = Convert.ToDouble((prices.Last()));
					return Ok(new { current_price = currentPrice.ToString("C2") });

				}
			}
			catch (Exception ex)
			{
				return BadRequest("Could not get prices");
			}

        }
        public void checkLimitOrders(double price, string stockTicker)
		{
			var queryList = new List<string>();
			try
			{
				var limitOrderList = command.Select("limit_order", price);
				if (limitOrderList.Count == 0)
				{
					return;
				}
				foreach (object[] row in limitOrderList)
				{
					if ((DateTime)row[8] > DateTime.Now)
					{
						command.InsertUserStockTransaction(row[4].ToString(), row[2].ToString(), row[3].ToString(), Convert.ToInt16(row[5]), Convert.ToDouble(row[6]));
						command.UpdateBalance(row[2].ToString(), Convert.ToDouble(row[6]));
						command.UpdateVolume(row[3].ToString(), Convert.ToInt16(row[5]));
					}
                    command.DeleteRecord(row[0].ToString());
                }
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
		}

		public bool checkSchedule()
		{
			try
			{
				var now = DateTime.Now.DayOfWeek;
				var list = command.Select(now.ToString());
				foreach (object[] day in list)
				{
					if (Convert.ToBoolean(day[4]) == true)
					{
						return false;
					}
					else
					{
						if ((DateTime.Now.TimeOfDay > (Convert.ToDateTime(day[2])).TimeOfDay) && (DateTime.Now.TimeOfDay < (Convert.ToDateTime(day[3])).TimeOfDay))
						{
							return true;
						}

					}


				}
			}
			catch (Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
			return false;
		}

		private double randomPriceGenerator(double price)
		{
			Random random = new Random();
			double randValue = random.NextDouble();
			double randNum = randValue * 0.0000999 + 0.0000001;
			double randNegNum = randValue * -0.0000999 - 0.0000001;
			int choice = random.Next(1);

			if (choice == 0)
			{
				return price * (1 + randNum);
			}
			else
			{
				return price * (1 + randNegNum);
			}
		}
	}

}
