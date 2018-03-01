﻿using System;
using System.Collections;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Linq;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.EF6 {

    class SelectNotMapped_DataItem {
        public int ID { get; set; }

        public string Name { get; set; }

        public byte[] Blob { get; set; }

        [NotMapped]
        public string BlobUrl {
            get { return "blob:" + Convert.ToBase64String(Blob); }
        }
    }

    partial class TestDbContext {
        public DbSet<SelectNotMapped_DataItem> SelectNotMapped_Data { get; set; }
    }

    public class SelectNotMapped {

        [Fact]
        public void Scenario() {
            TestDbContext.Exec(context => {
                var dbSet = context.SelectNotMapped_Data;

                dbSet.Add(new SelectNotMapped_DataItem { Blob = new byte[] { 143, 93, 183 } });
                context.SaveChanges();

                var loadOptions = new SampleLoadOptions {
                    PreSelect = new[] { "ID", "Name", "BlobUrl" },
                };

                var error = Record.Exception(delegate {
                    DataSourceLoader.Load(dbSet, loadOptions).data.Cast<object>().First();
                });

                Assert.Contains("member 'BlobUrl' is not supported ", error.Message);

                loadOptions.RemoteSelect = false;
                var loadResult = DataSourceLoader.Load(dbSet, loadOptions);
                var item = loadResult.data.Cast<IDictionary>().First();

                Assert.Equal(3, item.Keys.Count);
                Assert.True(item.Contains("ID"));
                Assert.True(item.Contains("Name"));
                Assert.Equal("blob:j123", item["BlobUrl"]);
            });
        }

    }
}
