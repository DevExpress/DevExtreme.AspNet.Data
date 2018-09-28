using DevExtreme.AspNet.Data.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {

    public class AnonTypeNewTweaksTests {
        static readonly ICollection<Expression> EMPTY_EXPR_LIST = Array.Empty<Expression>();
        static readonly ICollection<Expression> PARTIAL_EXPR_LIST = new[] {
            Expression.Constant(0),
            Expression.Constant(1),
            Expression.Constant(2),
        };


        [Fact]
        public void Null() {
            AssertDefaultBehavior(null);
        }

        [Fact]
        public void Defaults() {
            AssertDefaultBehavior(new AnonTypeNewTweaks());
        }


        static void AssertDefaultBehavior(AnonTypeNewTweaks tweaks) {
            Assert.Equal(
                "new AnonType()",
                AnonType.CreateNewExpression(EMPTY_EXPR_LIST, tweaks).ToString()
            );

            Assert.Equal(
                "new AnonType`4(I0 = 0, I1 = 1, I2 = 2)",
                AnonType.CreateNewExpression(PARTIAL_EXPR_LIST, tweaks).ToString()
            );
        }

        [Fact]
        public void AllowEmptyFalse() {
            var tweaks = new AnonTypeNewTweaks { AllowEmpty = false };

            Assert.Equal(
                "1",
                AnonType.CreateNewExpression(EMPTY_EXPR_LIST, tweaks).ToString()
            );
        }

        [Fact]
        public void AllowUnusedMembersFalse() {
            var tweaks = new AnonTypeNewTweaks { AllowUnusedMembers = false };

            Assert.Equal(
                "new AnonType`4(I0 = 0, I1 = 1, I2 = 2, I3 = False)",
                AnonType.CreateNewExpression(PARTIAL_EXPR_LIST, tweaks).ToString()
            );
        }

    }

}
