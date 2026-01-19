using Dapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace GameShopWeb.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IConfiguration configuration;

        public OrderController(IConfiguration configuration)
        {
            this.configuration = configuration;
        }

        [HttpPost("")]
        [Authorize]
        public IActionResult CreateOrder(Order order)
        {
            using (var connection = new SqlConnection(configuration.GetConnectionString("connString")))
            {
                string sql = @"INSERT INTO Order(
                idUser,idEmployee,dateOrdered,dateSent
                ) OUTPUT inserted.id VALUES(
                @idUser,@idEmployee,@dateOrdered,@dateSent
                )";
                int orderId = connection.ExecuteScalar<int>(sql, order);
                order.Id = orderId;
                foreach(OrderDetail detail in order.Details)
                {
                    sql = @"INSERT INTO OrderDetail(
                    idOrder,idGame,quantity,unitprice
                    ) OUTPUT inserted.id VALUES(
                    @idOrder,@idGame,@quantity,@unitprice
                    )";
                    detail.IdOrder = orderId;
                    connection.Execute(sql, detail);
                }
                return Ok(order);
            }
        }
    }
}
