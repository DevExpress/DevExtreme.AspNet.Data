using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DevExtreme.AspNet.Data.Helpers {
    using CompilerFunc = Func<CustomFilterCompilers.BinaryExpressionInfo, Expression>;

    public static class CustomFilterCompilers {

        public class BinaryExpressionInfo {
            public Expression DataItemExpression { get; internal set; }
            public string AccessorText { get; internal set; }
            public string Operation { get; internal set; }
            public object Value { get; internal set; }
        }

        readonly static ICollection<CompilerFunc> _binaryCompilers = new List<CompilerFunc>();

        internal static bool HasBinaryCompilers => _binaryCompilers.Count > 0;

        public static void RegisterBinary(CompilerFunc compilerFunc) {
            _binaryCompilers.Add(compilerFunc);
        }

        internal static Expression TryCompileBinary(BinaryExpressionInfo info) {
            foreach(var compiler in _binaryCompilers) {
                var result = compiler(info);
                if(result != null)
                    return result;
            }
            return null;
        }

#if DEBUG
        internal static void Clear() {
            _binaryCompilers.Clear();
        }
#endif
    }

}
