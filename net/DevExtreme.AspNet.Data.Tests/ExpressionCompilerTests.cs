using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {

    public class ExpressionCompilerTests {

        class SampleCompiler : ExpressionCompiler {

            public SampleCompiler(bool guardNulls)
                : base(guardNulls) {
            }

        }

        class TargetClass {
            public TargetClass Ref = null;
            public int Value = 0;
            public DateTime? Nullable = DateTime.Now;
            public string String = "";
            public StructWithRef StructWithRef = new StructWithRef(null);
        }

        struct StructWithRef {
            public TargetClass Ref;

            public StructWithRef(object any) {
                Ref = null;
            }
        }

        string CompileAccessor(bool guardNulls, string selector, bool forceToString = false) {
            return new SampleCompiler(guardNulls)
                .CompileAccessorExpression(Expression.Parameter(typeof(TargetClass), "t"), selector, forceToString)
                .ToString();
        }

        [Fact]
        public void Accessor_NoGuard_RefChain() {
            Assert.Equal("t.Ref.Ref", CompileAccessor(false, "Ref.Ref"));
        }

        [Fact]
        public void Accessor_NoGuard_Nullable() {
            Assert.Equal("t.Nullable.Value.Year", CompileAccessor(false, "Nullable.Year"));
        }

        [Fact]
        public void Accessor_Guard_SingleComponent() {
            Assert.Equal("IIF((t == null), 0, t.Value)", CompileAccessor(true, "Value"));
            Assert.Equal("IIF((t == null), null, t.Ref)", CompileAccessor(true, "Ref"));
        }

        [Fact]
        public void Accessor_Guard_String() {
            Assert.Equal(
                "IIF(((t == null) OrElse (t.String == null)), 0, t.String.Length)",
                CompileAccessor(true, "String.Length")
            );
        }

        [Fact]
        public void Accessor_Guard_RefChain() {
            Assert.Equal(
                "IIF((((t == null) OrElse (t.Ref == null)) OrElse (t.Ref.Ref == null)), null, t.Ref.Ref.Ref)",
                CompileAccessor(true, "Ref.Ref.Ref")
            );
        }

        [Fact]
        public void Accessor_Guard_Nullable() {
            Assert.Equal("IIF((t == null), null, t.Nullable)", CompileAccessor(true, "Nullable"));
            Assert.Equal("IIF(((t == null) OrElse (t.Nullable == null)), 0, t.Nullable.Value.Year)", CompileAccessor(true, "Nullable.Year"));
        }

        [Fact]
        public void Accessor_Guard_NullInStruct() {
            Assert.Equal(
                "IIF(((t == null) OrElse (t.StructWithRef.Ref == null)), 0, t.StructWithRef.Ref.Value)",
                CompileAccessor(true, "StructWithRef.Ref.Value")
            );
        }

        [Fact]
        public void Accessor_ForceToString() {
            Assert.Equal("t.String", CompileAccessor(false, "String", true));
            Assert.Equal("t.Value.ToString()", CompileAccessor(false, "Value", true));
            Assert.Equal("t.Ref.ToString()", CompileAccessor(false, "Ref", true));

            Assert.Equal(
                "IIF((t == null), null, t.String)",
                CompileAccessor(true, "String", true)
            );

            Assert.Equal(
                "IIF((t == null), null, t.Value.ToString())",
                CompileAccessor(true, "Value", true)
            );

            Assert.Equal(
                "IIF((((t == null) OrElse (t.Ref == null)) OrElse (t.Ref.Ref == null)), null, t.Ref.Ref.ToString())",
                CompileAccessor(true, "Ref.Ref", true)
            );
        }

        [Fact]
        public void Issue142() {
            Assert.Equal(
                DateTime.MinValue.ToString() + ".Date.Date",
                new SampleCompiler(true).CompileAccessorExpression(Expression.Constant(DateTime.MinValue), "Date.Date").ToString()
            );
        }

    }

}
