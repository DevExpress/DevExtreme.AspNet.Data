using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {

    public class Bug240Tests {

        static DataSourceLoadOptionsBase CreateFullStuffedLoadOptions() {
            var selector = "Item1.Year";

            return new SampleLoadOptions {
                Filter = new[] { selector, "123" },
                Sort = new[] {
                    new SortingInfo { Selector = selector }
                },
                Select = new[] { selector },
                Group = new[] {
                    new GroupingInfo { Selector = selector, IsExpanded = false }
                },
                TotalSummary = new[] {
                    new SummaryInfo { Selector = selector, SummaryType = "max" }
                },
                GroupSummary = new[] {
                    new SummaryInfo { Selector = selector, SummaryType = "max" }
                }
            };
        }

        static string FormatExpectedSelectorExpr(bool convertToNullable, bool guardNulls) {
            var result = "obj.Item1.Value.Year";

            if(convertToNullable)
                result = Compat.ExpectedConvert(result, "Nullable`1");

            if(guardNulls)
                result = $"IIF(((obj == null) OrElse (obj.Item1 == null)), null, {result})";

            return result;
        }

        [Fact]
        public void BuildLoadExpr_NoGuardNulls() {
            var expr = new DataSourceExpressionBuilder<Tuple<DateTime?>>(CreateFullStuffedLoadOptions(), false).BuildLoadExpr();

            Assert.Equal(
                "data"
                    // Where and OrderBy use simple selectors
                    + $".Where(obj => ({FormatExpectedSelectorExpr(false, false)} == 123))"
                    + $".OrderBy(obj => {FormatExpectedSelectorExpr(false, false)})"

                    // Select uses conversion to Nullable
                    + ".Select(obj => new AnonType`1("
                    + $"I0 = {FormatExpectedSelectorExpr(true, false)}"
                    + "))",
                expr.ToString()
            );
        }

        [Fact]
        public void BuildLoadExpr_WithGuardNulls() {
            var expr = new DataSourceExpressionBuilder<Tuple<DateTime?>>(CreateFullStuffedLoadOptions(), true).BuildLoadExpr();

            Assert.Equal(
                "data"
                    // All selectors are guarded and use conversion to Nullable
                    + $".Where(obj => ({FormatExpectedSelectorExpr(true, true)} == 123))"
                    + $".OrderBy(obj => {FormatExpectedSelectorExpr(true, true)})"
                    + ".Select(obj => new AnonType`1("
                    + $"I0 = {FormatExpectedSelectorExpr(true, true)}"
                    + "))",
                expr.ToString()
            );
        }

        [Fact]
        public void BuildLoadGroupsExpr_NoGuardNulls() {
            var expr = new DataSourceExpressionBuilder<Tuple<DateTime?>>(CreateFullStuffedLoadOptions(), false).BuildLoadGroupsExpr();

            Assert.Equal(
                // Only selectors that land in .Select() use conversion to Nullable
                "data"
                    + $".Where(obj => ({FormatExpectedSelectorExpr(false, false)} == 123))"
                    + $".GroupBy(obj => new AnonType`1(I0 = {FormatExpectedSelectorExpr(true, false)}))"
                    + ".OrderBy(g => g.Key.I0)"
                    + ".Select(g => new AnonType`4("
                    + "I0 = g.Count(), "
                    + "I1 = g.Key.I0, "
                    + $"I2 = g.Max(obj => {FormatExpectedSelectorExpr(true, false)}), "
                    + $"I3 = g.Max(obj => {FormatExpectedSelectorExpr(true, false)})"
                    + "))",
                expr.ToString()
            );
        }

        [Fact]
        public void BuildLoadGroupsExpr_WithGuardNulls() {
            var expr = new DataSourceExpressionBuilder<Tuple<DateTime?>>(CreateFullStuffedLoadOptions(), true).BuildLoadGroupsExpr();

            Assert.Equal(
                // All selectors are guarded and use conversion to Nullable
                "data"
                    + $".Where(obj => ({FormatExpectedSelectorExpr(true, true)} == 123))"
                    + $".GroupBy(obj => new AnonType`1(I0 = {FormatExpectedSelectorExpr(true, true)}))"
                    + ".OrderBy(g => g.Key.I0)"
                    + ".Select(g => new AnonType`4("
                    + "I0 = g.Count(), "
                    + "I1 = g.Key.I0, "
                    + $"I2 = g.Max(obj => {FormatExpectedSelectorExpr(true, true)}), "
                    + $"I3 = g.Max(obj => {FormatExpectedSelectorExpr(true, true)})"
                    + "))",
                expr.ToString()
            );
        }

        [Fact]
        public void L2O_Select_Null() {
            var data = new string[] { null };

            var loadOptions = new SampleLoadOptions {
                Select = new[] { "Length" }
            };

            var loadResult = DataSourceLoader.Load(data, loadOptions);
            var items = loadResult.data.Cast<IDictionary<string, object>>();
            Assert.Null(items.First()["Length"]);
        }

        [Fact]
        public void L2O_NullVsDefault_Filter() {
            var data = new[] {
                null,
                ""
            };

            var loadOptions = new SampleLoadOptions {
                Filter = new[] { "Length", "0" }
            };

            var loadResult = DataSourceLoader.Load(data, loadOptions);
            var items = loadResult.data.Cast<string>().ToArray();

            Assert.Single(items);
        }

        [Fact]
        public void L2O_NullVsDefault_Sort() {
            var data = new[] {
                "",
                null,
                "",
                null
            };

            var loadOptions = new SampleLoadOptions {
                Sort = new[] {
                    new SortingInfo { Selector = "Length" }
                }
            };

            var loadResult = DataSourceLoader.Load(data, loadOptions);
            var items = loadResult.data.Cast<string>().ToArray();

            Assert.Null(items[0]);
            Assert.Null(items[1]);
        }

    }

}
