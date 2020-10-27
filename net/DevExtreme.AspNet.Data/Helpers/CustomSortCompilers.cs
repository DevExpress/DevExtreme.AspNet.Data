using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DevExtreme.AspNet.Data.Helpers {
    using SortExpressionCompilerFunc = Func<Expression, ISortExpressionInfo, Expression>;

    public static class CustomSortCompilers {

        internal static class Sort {
            internal readonly static ICollection<SortExpressionCompilerFunc> CompilerFuncs = new List<SortExpressionCompilerFunc>();

            internal static Expression TryCompile(Expression target, ISortExpressionInfo info) {
                foreach(var func in CompilerFuncs) {
                    var result = func(target, info);
                    if(result != target)
                        return result;
                }
                return null;
            }
        }

        public static void RegisterBinaryExpressionCompiler(SortExpressionCompilerFunc compilerFunc) {
            Sort.CompilerFuncs.Add(compilerFunc);
        }
    }
}
