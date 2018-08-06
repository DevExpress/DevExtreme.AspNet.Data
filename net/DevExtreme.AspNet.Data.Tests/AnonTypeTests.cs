using DevExtreme.AspNet.Data.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {

    public class AnonTypeTests {

        [Fact]
        public void Get() {
            Assert.Same(
                typeof(AnonType),
                AnonType.Get(new Type[0])
            );

            Assert.Same(
                typeof(AnonType<int, string>),
                AnonType.Get(new[] { typeof(int), typeof(string) })
            );

            Assert.Same(
                typeof(AnonType<int, int, int, int, int, bool, bool, bool>),
                AnonType.Get(new[] { typeof(int), typeof(int), typeof(int), typeof(int), typeof(int) })
            );

            var x = Record.Exception(delegate {
                AnonType.Get(Enumerable.Repeat(typeof(int), 100).ToArray());
            });
            Assert.Contains("Too many", x.Message);
        }

        [Fact]
        public void Equality() {
            var o1 = new AnonType<int, int>(1, 2);
            var o2 = new AnonType<int, int>(1, 2);
            var o3 = new AnonType<int, string>(1, "a");
            var o4 = new AnonType<int, int>(1, 3);
            var o5 = new AnonType<int, int, int, int>(1, 2, 3, 4);

            Assert.True(o1.Equals(o2));
            Assert.Equal(o1.GetHashCode(), o2.GetHashCode());

            Assert.False(o1.Equals(o3));
            Assert.False(o1.Equals(o4));
            Assert.False(o1.Equals(o5));
            Assert.False(o1.Equals(null));
        }

        [Fact]
        public void GetHashCode_Null() {
            var x = Record.Exception(delegate {
                new AnonType<object>(null).GetHashCode();
            });

            Assert.Null(x);
        }

        [Fact]
        public void Accessor() {
            var o = new AnonType<int, int>(42, 84);
            Assert.Equal(42, AnonTypeAccessor.Instance.Read(o, "I0"));
            Assert.Equal(84, AnonTypeAccessor.Instance.Read(o, "I1"));
        }

    }

}
