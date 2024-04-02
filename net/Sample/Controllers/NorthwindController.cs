using DevExtreme.AspNet.Data;

using Sample.Models;

using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

using Newtonsoft.Json;

namespace Sample.Controllers {

    [Route("nwind")]
    public class NorthwindController : Controller {
        NorthwindContext _nwind;

        public NorthwindController(NorthwindContext nwind) {
            _nwind = nwind;
        }

        [HttpGet("orders")]
        public async Task<IActionResult> Orders(DataSourceLoadOptions loadOptions) {
            var source = _nwind.Orders.Select(o => new {
                o.OrderId,
                o.CustomerId,
                o.OrderDate,
                o.Freight,
                o.ShipCountry,
                o.ShipRegion,
                o.ShipVia
            });

            loadOptions.PrimaryKey = new[] { "OrderId" };
            loadOptions.PaginateViaPrimaryKey = true;

            return Json(await DataSourceLoader.LoadAsync(source, loadOptions));
        }

        [HttpGet("order-details")]
        public async Task<IActionResult> OrderDetails(int orderId, DataSourceLoadOptions loadOptions) {
            var source = _nwind.OrderDetails
                .Where(i => i.OrderId == orderId)
                .Select(i => new {
                    Product = i.Product.ProductName,
                    Price = i.UnitPrice,
                    i.Quantity,
                    Sum = i.UnitPrice * i.Quantity
                });

            return Json(await DataSourceLoader.LoadAsync(source, loadOptions));
        }

        [HttpGet("customers-lookup")]
        public async Task<object> CustomersLookup(DataSourceLoadOptions loadOptions) {
            var source = _nwind.Customers
                .OrderBy(c => c.CompanyName)
                .Select(c => new {
                    Value = c.CustomerId,
                    Text = $"{c.CompanyName} ({c.Country})"
                });

            return Json(await DataSourceLoader.LoadAsync(source, loadOptions));
        }

        [HttpGet("shippers-lookup")]
        public async Task<object> ShippersLookup(DataSourceLoadOptions loadOptions) {
            var source = _nwind.Shippers
                .OrderBy(s => s.CompanyName)
                .Select(s => new {
                    Value = s.ShipperId,
                    Text = s.CompanyName
                });

            return Json(await DataSourceLoader.LoadAsync(source, loadOptions));
        }

        [HttpPut("update-order")]
        public async Task<IActionResult> UpdateOrder(int key, string values) {
            var order = await _nwind.Orders.FirstOrDefaultAsync(o => o.OrderId == key);
            if(order == null)
                return StatusCode(409, "Order not found");

            JsonConvert.PopulateObject(values, order);

            if(!TryValidateModel(order))
                return BadRequest(ModelState.ToFullErrorString());

            await _nwind.SaveChangesAsync();

            return Ok();
        }

        [HttpPost("insert-order")]
        public async Task<IActionResult> InsertOrder(string values) {
            var order = new Order();

            JsonConvert.PopulateObject(values, order);

            if(!TryValidateModel(order))
                return BadRequest(ModelState.ToFullErrorString());

            _nwind.Orders.Add(order);
            await _nwind.SaveChangesAsync();

            return Json(order.OrderId);
        }

        [HttpDelete("delete-order")]
        public async Task<IActionResult> DeleteOrder(int key) {
            var order = await _nwind.Orders.FirstOrDefaultAsync(o => o.OrderId == key);
            if(order == null)
                return StatusCode(409, "Order not found");

            _nwind.Orders.Remove(order);
            await _nwind.SaveChangesAsync();

            return Ok();
        }

        [HttpGet("products")]
        public async Task<IActionResult> Products(DataSourceLoadOptions loadOptions) {
            var source = _nwind.Products.Select(p => new {
                p.ProductId,
                p.ProductName,
                p.Category.CategoryName,
                p.UnitPrice
            });

            return Json(await DataSourceLoader.LoadAsync(source, loadOptions));
        }

    }
}
