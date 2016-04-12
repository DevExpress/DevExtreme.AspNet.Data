using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {

    public class ExpressionCompilerTests {

        class SampleCompiler : ExpressionCompiler {
            bool _guardNulls;

            public SampleCompiler(bool guardNulls) {
                _guardNulls = guardNulls;
            }

            protected override bool GuardNulls {
                get { return _guardNulls;  }
            }
            
        }

        class TargetClassBase {
            public int Value = 0;
            public int? Nullable = 0;
            public string String = "";
        }

        class TargetClass1 : TargetClassBase {
            public TargetClass2 Ref = null;
        }

        class TargetClass2 : TargetClassBase {
            public TargetClass1 Ref = null;
        }
        
        string CompileAccessor(bool guardNulls, string selector, bool forceToString = false) {
            return new SampleCompiler(guardNulls)
                .CompileAccessorExpression_NEW(Expression.Parameter(typeof(TargetClass1), "t"), selector, forceToString)
                .ToString();
        }

        [Fact]
        public void Accessor_NoGuard_RefChain() {
            Assert.Equal("t.Ref.Ref", CompileAccessor(false, "Ref.Ref"));
        }

        [Fact]
        public void Accessor_Guard_SingleComponent() {
            Assert.Equal("IIF((t == null), 0, t.Value)", CompileAccessor(true, "Value"));
            Assert.Equal("IIF((t == null), null, t.Ref)", CompileAccessor(true, "Ref"));
        }

        [Fact]
        public void Accessor_Guard_String() {
            Assert.Equal(
                @"IIF((t == null), 0, (t.String ?? """").Length)",
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
        public void Accessor_Guard_ExplicitNullable() {
            Assert.Equal(
                "IIF(((t == null) OrElse (t.Nullable == null)), 0, t.Nullable.Value)", 
                CompileAccessor(true, "Nullable.Value")
            );
        }

        [Fact]
        public void Accessor_ForceToString() {
            Assert.Equal("t.String", CompileAccessor(false, "String", true));
            Assert.Equal("t.Value.ToString()", CompileAccessor(false, "Value", true));
            Assert.Equal("t.Ref.ToString()", CompileAccessor(false, "Ref", true));

            Assert.Equal(
                @"IIF((t == null), """", (t.String ?? """"))", 
                CompileAccessor(true, "String", true)
            );

            Assert.Equal(
                @"IIF((t == null), """", t.Value.ToString())",
                CompileAccessor(true, "Value", true)
            );

            Assert.Equal(
                @"IIF((((t == null) OrElse (t.Ref == null)) OrElse (t.Ref.Ref == null)), """", t.Ref.Ref.ToString())",
                CompileAccessor(true, "Ref.Ref", true)
            );
        }

    }

}
