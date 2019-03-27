using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sample.Models;
using DevExtreme.AspNet.Data;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;

namespace Sample.Controllers {

    [Route("nwind")]
    public class NorthwindController : Controller {
        NorthwindContext _nwind;

        public NorthwindController(NorthwindContext nwind) {
            _nwind = nwind;
        }

        [HttpGet("orders")]
        public object Orders(DataSourceLoadOptions loadOptions) {
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

            return DataSourceLoader.Load(source, loadOptions);
        }

        [HttpGet("order-details")]
        public object OrderDetails(int orderId, DataSourceLoadOptions options) {
            return DataSourceLoader.Load(
                from i in _nwind.OrderDetails
                where i.OrderId == orderId
                select new {
                    Product = i.Product.ProductName,
                    Price = i.UnitPrice,
                    Quantity = i.Quantity,
                    Sum = i.UnitPrice * i.Quantity
                },
                options
            );
        }

        [HttpGet("customers-lookup")]
        public object CustomersLookup(DataSourceLoadOptions options) {
            return DataSourceLoader.Load(
                from c in _nwind.Customers orderby c.CompanyName select new {
                    Value = c.CustomerId,
                    Text = $"{c.CompanyName} ({c.Country})"
                },
                options
            );
        }

        [HttpGet("shippers-lookup")]
        public object ShippersLookup(DataSourceLoadOptions options) {
            return DataSourceLoader.Load(
                from s in _nwind.Shippers orderby s.CompanyName select new {
                    Value = s.ShipperId,
                    Text = s.CompanyName
                },
                options
            );
        }

        [HttpPut("update-order")]
        public IActionResult UpdateOrder(int key, string values) {
            var order = _nwind.Orders.FirstOrDefault(o => o.OrderId == key);
            if(order == null)
                return StatusCode(409, "Order not found");

            JsonConvert.PopulateObject(values, order);

            if(!TryValidateModel(order))
                return BadRequest(ModelState.ToFullErrorString());

            _nwind.SaveChanges();

            return Ok();
        }

        [HttpPost("insert-order")]
        public IActionResult InsertOrder(string values) {
            var order = new Order();
            JsonConvert.PopulateObject(values, order);

            if(!TryValidateModel(order))
                return BadRequest(ModelState.ToFullErrorString());

            _nwind.Orders.Add(order);
            _nwind.SaveChanges();

            return Json(order.OrderId);
        }

        [HttpDelete("delete-order")]
        public IActionResult DeleteOrder(int key) {
            var order = _nwind.Orders.FirstOrDefault(o => o.OrderId == key);
            if(order == null)
                return StatusCode(409, "Order not found");

            _nwind.Orders.Remove(order);
            _nwind.SaveChanges();

            return Ok();
        }

        [HttpGet("products")]
        public object Products(DataSourceLoadOptions loadOptions) {
            var projection = _nwind.Products.Select(p => new {
                p.ProductId,
                p.ProductName,
                p.Category.CategoryName,
                p.UnitPrice
            });

            return DataSourceLoader.Load(projection, loadOptions);
        }

    }
}
