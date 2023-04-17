using System;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using stock_trader_backend.Models;
using System.Text;
using System.Text.Json;
using Google.Protobuf;

    [ApiController]
    [Route("[controller]")]
    public class AdminController : ControllerBase
	{
		private MySqlCommands command;
		public AdminController()
		{
			command = new MySqlCommands();
		}

        [HttpPost("create-stock")]
        public async Task<IActionResult> CreateStock([FromBody] StockObject data)
        {
            var message = new { message = "Stock could not be created" };
            try
            {
                    int result = await command.InsertNewStock(data.companyName, data.stock_ticker, data.volume ?? 0, Convert.ToDouble(data.opening_price));
                    message = new { message = result.ToString()};
                    if (result > 0)
                    {
                        message = new { message = "Stock was created" };
                        return Ok(message);
                    }      
            } catch (Exception ex)
            {
                message = new { message = ex.Message };
            }
            return BadRequest(message);
        }

        [HttpPut("change-schedule")]
        public IActionResult changeMarketSchedule(ScheduleObject data)
		{
            var message = new { message = "Hours could not be updated" };
            try
			{
                    int result = command.UpdateTimeSchedule(data);
                    if (result > 0)
                    {
                        message = new { message = "Hours were updated" };
                        return Ok(message);
				}
			} catch(Exception ex)
			{
				Console.WriteLine(ex.Message);
			}
            return BadRequest(message);
        }

	}
