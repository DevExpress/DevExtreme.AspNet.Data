using DevExtreme.AspNet.Data.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {

    public class EFSortingTests {

        class TestClass_Base {
            protected int Bad1 = 0;
            protected int Bad2 { get; set; }

            public int Bad3 { get; }
            public int Bad4 { get; internal set; }

            public object Bad5 = null;
            public object Bad6 { get; set; }
        }

        class TestClass_LikelyKey : TestClass_Base {
            public DateTime Skip { get; set; }
            public Guid Key { get; set; }
        }

        class TestClass_Field : TestClass_Base {
            public int Key = 0;
        }

        class TestClass_OtherSortable : TestClass_Base {
            public SByte Sortable { get; set; }
        }

        class TestClass_Nullable : TestClass_Base {
            public int? Sortable { get; set; }
        }


        [Fact]
        public void FindSortableMember() {
            Assert.Null(EFSorting.FindSortableMember(typeof(TestClass_Base)));
            Assert.Equal("Key", EFSorting.FindSortableMember(typeof(TestClass_LikelyKey)));
            Assert.Equal("Key", EFSorting.FindSortableMember(typeof(TestClass_Field)));
            Assert.Equal("Sortable", EFSorting.FindSortableMember(typeof(TestClass_OtherSortable)));
            Assert.Equal("Sortable", EFSorting.FindSortableMember(typeof(TestClass_Nullable)));
        }


    }

}
