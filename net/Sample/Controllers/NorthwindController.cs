using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Sample.Models;
using DevExtreme.AspNet.Data;
using Newtonsoft.Json;
using Microsoft.EntityFrameworkCore;
using MongoDB.Driver;
using MongoDB.Bson;

namespace Sample.Controllers {

    // https://stackoverflow.com/q/32770477
    class LookupItem {
        public BsonValue Value { get; set; }
        public BsonValue Text { get; set; }
    }

    [Route("nwind")]
    public class NorthwindController : Controller {
        IMongoDatabase _nwind;

        public NorthwindController() {
            // https://github.com/InfinniPlatform/Northwind
            _nwind = new MongoClient("mongodb://localhost").GetDatabase("Northwind");
        }

        [HttpGet("orders")]
        public IActionResult Orders(DataSourceLoadOptions loadOptions) {
            var source = _nwind.GetCollection<BsonDocument>("Orders").AsQueryable();

            loadOptions.PreSelect = new[] {
                "_id",
                "Customer.Id",
                "OrderDate",
                "Freight",
                "ShipCountry",
                "ShipRegion",
                "ShipVia.Id"
            };

            loadOptions.StringToLower = true;

            return Json2(DataSourceLoader.Load(source, loadOptions));
        }

        [HttpGet("customers-lookup")]
        public object CustomersLookup(DataSourceLoadOptions loadOptions) {
            var source = _nwind.GetCollection<BsonDocument>("Customers").AsQueryable()
                .OrderBy(c => c["CompanyName"])
                .Select(c => new LookupItem {
                    Value = c["_id"],
                    Text = c["CompanyName"]
                });

            return Json2(DataSourceLoader.Load(source, loadOptions));
        }

        [HttpGet("shippers-lookup")]
        public object ShippersLookup(DataSourceLoadOptions loadOptions) {
            var source = _nwind.GetCollection<BsonDocument>("Shippers").AsQueryable()
                .OrderBy(s => s["CompanyName"])
                .Select(s => new LookupItem {
                    Value = s["_id"],
                    Text = s["CompanyName"]
                });

            return Json2(DataSourceLoader.Load(source, loadOptions));
        }

        IActionResult Json2(object obj) {
            var json = JsonConvert.SerializeObject(obj, new BsonValueConverter());
            return Content(json, "application/json");
        }

    }
}
