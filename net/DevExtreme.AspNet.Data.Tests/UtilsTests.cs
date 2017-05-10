using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {

    public class UtilsTests {

        // AddRequiredSort

        [Fact]
        public void AddRequiredSort_Deduplicate() {
            var sort = Utils.AddRequiredSort(
                new[] {
                    new SortingInfo { Selector = "A" },
                    new SortingInfo { Selector = "B" }
                },
                new[] { "A", "C" }
            );

            Assert.Equal(new[] { "A", "B", "C" }, sort.Select(i => i.Selector));
        }

        [Fact]
        public void AddRequiredSort_Desc() {
            var initalSort = new[] {
                new SortingInfo {
                    Selector = "A",
                    Desc = false
                }
            };
            var requiredSelectors = new[] { "R1", "R2" };

            var ensuredSort = Utils.AddRequiredSort(initalSort, requiredSelectors).ToArray();
            Assert.False(ensuredSort[1].Desc);
            Assert.False(ensuredSort[2].Desc);

            initalSort[0].Desc = true;
            ensuredSort = Utils.AddRequiredSort(initalSort, requiredSelectors).ToArray();
            Assert.True(ensuredSort[1].Desc);
            Assert.True(ensuredSort[2].Desc);
        }

        [Fact]
        public void AddRequiredSort_InitialSortIsNull() {
            var sort = Utils.AddRequiredSort(null, new[] { "R" });
            Assert.Equal("R", sort.First().Selector);
        }

        // GetPrimaryKey

        class ClassWithMultiplePK {
            [System.ComponentModel.DataAnnotations.Key]
            public int Z = 0;

            [System.ComponentModel.DataAnnotations.Key]
            public int A { get; set; }
        }

        [Fact]
        public void GetPrimaryKey_NoKey() {
            Assert.Empty(Utils.GetPrimaryKey(typeof(int)));
        }

        [Fact]
        public void GetPrimaryKey_MultiKey() {
            Assert.Equal(
                new[] { "A", "Z" },
                Utils.GetPrimaryKey(typeof(ClassWithMultiplePK))
            );
        }

    }

}
