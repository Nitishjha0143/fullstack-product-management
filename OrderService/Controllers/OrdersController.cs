using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrderService.Data;
using OrderService.Models;
using System.Net.Http.Json;

namespace OrderService.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class OrdersController : ControllerBase
    {
        private readonly OrderDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _config;

        public OrdersController(OrderDbContext context, IHttpClientFactory httpClientFactory, IConfiguration config)
        {
            _context = context;
            _httpClient = httpClientFactory.CreateClient();
            _config = config;
        }

        [HttpPost("create")]
        public async Task<IActionResult> CreateOrder([FromBody] Order order)
        {
            // Get base URLs from config
            var productServiceUrl = _config["ServiceUrls:ProductService"];
            var authServiceUrl = _config["ServiceUrls:AuthService"];

            // Validate product
            var product = await _httpClient.GetFromJsonAsync<ProductDto>(
                $"{productServiceUrl}/api/product/{order.ProductId}");

            if (product == null)
                return BadRequest("Invalid Product ID");

            // Validate user
            var user = await _httpClient.GetFromJsonAsync<UserDto>(
                $"{authServiceUrl}/api/auth/users/{order.UserId}");

            if (user == null)
                return BadRequest("Invalid User ID");

            // Calculate total
            order.TotalPrice = product.Price * order.Quantity;

            _context.Orders.Add(order);
            await _context.SaveChangesAsync();

            return Ok(order);
        }

        [HttpGet]
        public async Task<IActionResult> GetOrders()
        {
            var orders = await _context.Orders
                .Include(o => o.Items)
                .ToListAsync();

            return Ok(orders);
        }
    }
}
