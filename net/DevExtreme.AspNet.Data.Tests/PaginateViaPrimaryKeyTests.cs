using Newtonsoft.Json;
using System;
using System.Linq;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {

    public class PaginateViaPrimaryKeyTests {

        [Fact]
        public void SingleKey() {
            var data = Enumerable.Range(0, 5)
                .Select(i => new { ID = i })
                .ToArray();

            var loadOptions = new SampleLoadOptions {
                GuardNulls = false,

                PrimaryKey = new[] { "ID" },
                PaginateViaPrimaryKey = true,

                Filter = new[] { "ID", ">", "0" },

                Skip = 2,
                Take = 2
            };

            var loadResult = DataSourceLoader.Load(data, loadOptions);

            Assert.Equal(
                "[{ID:3},{ID:4}]",
                DataToString(loadResult.data)
            );

            var log = loadOptions.ExpressionLog;

            Assert.EndsWith(
                ".Where(obj => (obj.ID > 0))" +
                ".OrderBy(obj => obj.ID)" +
                ".Select(obj => new AnonType`1(I0 = obj.ID))" +
                ".Skip(2).Take(2)",
                log[0]
            );

            Assert.EndsWith(
                ".Where(obj => ((obj.ID == 3) OrElse (obj.ID == 4)))" +
                ".OrderBy(obj => obj.ID)",
                log[1]
            );
        }

        [Fact]
        public void MultiKey() {
            var data = Enumerable.Range(0, 5)
                .Select(i => new { K1 = i, K2 = 5 - i })
                .ToArray();

            var loadOptions = new SampleLoadOptions {
                GuardNulls = false,

                PrimaryKey = new[] { "K1", "K2" },
                PaginateViaPrimaryKey = true,

                Skip = 3,
                Take = 2
            };

            DataSourceLoader.Load(data, loadOptions);

            Assert.Contains(
                ".Where(obj => (((obj.K1 == 3) AndAlso (obj.K2 == 2)) OrElse ((obj.K1 == 4) AndAlso (obj.K2 == 1))))",
                loadOptions.ExpressionLog[1]
            );
        }

        [Fact]
        public void ActiveForFirstPage() {
            var data = new[] {
                new { ID = 123 }
            };

            var loadOptions = new SampleLoadOptions {
                GuardNulls = false,
                PrimaryKey = new[] { "ID" },
                PaginateViaPrimaryKey = true,

                Skip = 0,
                Take = 123
            };

            DataSourceLoader.Load(data, loadOptions);

            Assert.Contains(".Select(obj => new AnonType`1", loadOptions.ExpressionLog[0]);
        }

        [Fact]
        public void DisabledWithoutTake() {
            var loadOptions = new SampleLoadOptions {
                GuardNulls = false,
                PaginateViaPrimaryKey = true
            };

            DataSourceLoader.Load(new object[0], loadOptions);

            Assert.NotEmpty(loadOptions.ExpressionLog);
            Assert.DoesNotContain(loadOptions.ExpressionLog, line => line.Contains(".Select"));
        }

        [Fact]
        public void RequiresPrimaryKey() {
            var error = Record.Exception(delegate {
                DataSourceLoader.Load(new string[0], new SampleLoadOptions {
                    PaginateViaPrimaryKey = true,
                    Take = 1
                });
            });
            Assert.True(error is InvalidOperationException);
        }

        [Fact]
        public void Bug349() {
            // https://github.com/DevExpress/DevExtreme.AspNet.Data/issues/349

            var loadResult = DataSourceLoader.Load(new[] { new { ID = 1 } }, new SampleLoadOptions {
                PaginateViaPrimaryKey = true,
                PrimaryKey = new[] { "ID" },
                RequireTotalCount = true,
                Skip = 1,
                Take = 1
            });

            Assert.Empty(loadResult.data);
        }

        static string DataToString(object data) {
            return JsonConvert.SerializeObject(data).Replace("\"", "");
        }

    }

}
