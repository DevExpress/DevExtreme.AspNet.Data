using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNet.Mvc;
using Sample.Models;
using DevExtreme.AspNet.Data;
using Newtonsoft.Json;

namespace Sample.Controllers {

    [Route("nwind")]
    public class NorthwindController {
        NorthwindContext _nwind;

        public NorthwindController(NorthwindContext nwind) {
            _nwind = nwind;
        }

        [HttpGet("orders")]
        public IActionResult Orders(DataSourceLoadOptions loadOptions) {
            return DataSourceLoadResult.Create(_nwind.Orders, loadOptions);
        }

        [HttpGet("order-details")]
        public IActionResult OrderDetails(int orderID, DataSourceLoadOptions options) {
            return DataSourceLoadResult.Create(
                from i in _nwind.Order_Details
                where i.OrderID == orderID
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
        public IActionResult CustomersLookup(DataSourceLoadOptions options) {
            return DataSourceLoadResult.Create(
                from c in _nwind.Customers orderby c.CompanyName select new {
                    Value = c.CustomerID,
                    Text = $"{c.CompanyName} ({c.Country})"
                },
                options
            );
        }

        [HttpGet("shippers-lookup")]
        public IActionResult ShippersLookup(DataSourceLoadOptions options) {
            return DataSourceLoadResult.Create(
                from s in _nwind.Shippers orderby s.CompanyName select new {
                    Value = s.ShipperID,
                    Text = s.CompanyName
                },
                options
            );
        }

        [HttpPut("update-order")]
        public void UpdateOrder(int key, string values) {
            var order = _nwind.Orders.First(o => o.OrderID == key);
            JsonConvert.PopulateObject(values, order);
            _nwind.SaveChanges();
        }

        [HttpPost("insert-order")]
        public void InsertOrder(string values) {
            var order = new Order();
            JsonConvert.PopulateObject(values, order);
            _nwind.Orders.Add(order);
            _nwind.SaveChanges();
        }

        [HttpDelete("delete-order")]
        public void DeleteOrder(int key) {
            var order = _nwind.Orders.First(o => o.OrderID == key);
            _nwind.Orders.Remove(order);
            _nwind.SaveChanges();
        }

    }
}
