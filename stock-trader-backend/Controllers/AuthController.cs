using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using stock_trader_backend.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication.Cookies;
using System.Security.Claims;
using System.Web;
using Google.Protobuf.WellKnownTypes;
using static Mysqlx.Expect.Open.Types.Condition.Types;
using System.Net.Http;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace stock_trader_backend.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AuthController : ControllerBase
    {
        private MySqlCommands command;
        public AuthController()
        {
            command = new MySqlCommands();
        }

        [HttpPost("user")]
        public async Task<IActionResult> authenticateUser([FromBody] AccountData data)
        {
            var message = new { message = "Could not authenticate" };
            try
            {
                var userAuth = await command.SelectAsync("user", data.username ?? "", data.pwd ?? "");
                if (userAuth.Count != 0)
                {
                    return Ok(new {message = "Access Granted"});
                }
                else
                {
                    message = new { message = "User does not exist"};
                }
            }
            catch (Exception ex)
            {
                message = new { message = ex.Message };
            }

            return BadRequest(message);
        }

        [HttpPost("admin")]
        public async Task<IActionResult> authenticateAdmin([FromBody] AccountData data)
        {
            var message = new { message = "Could not authenticate" };
            try
            {
                var userAuth = await command.SelectAsync("admin", data.username ?? "", data.pwd ?? "");
                if (userAuth.Count != 0)
                {
                    return Ok(new { message = "Authenticated"});
                }
                else
                {
                    message = new { message = "User does not exist" };
                }
            }
            catch (Exception ex)
            {
                message = new { message = ex.Message };
            }

            return BadRequest(message);
        }

    }
}

