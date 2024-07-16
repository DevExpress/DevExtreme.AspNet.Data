﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DevExtreme.AspNet.Data.Helpers {
    using BinaryExpressionCompilerFunc = Func<IBinaryExpressionInfo, Expression>;
    using BinaryExpressionCompilerWithContextFunc = Func<IBinaryExpressionInfo, FilterExpressionCompiler, Expression>;

    public static class CustomFilterCompilers {

        internal static class Binary {
            internal readonly static ICollection<BinaryExpressionCompilerWithContextFunc> CompilerFuncsWithContext = new List<BinaryExpressionCompilerWithContextFunc>();

            internal static Expression TryCompile(IBinaryExpressionInfo info, FilterExpressionCompiler filterExpressionCompiler) {
                foreach(var func in CompilerFuncsWithContext) {
                    var result = func(info, filterExpressionCompiler);
                    if(result != null)
                        return result;
                }
                return null;
            }
        }

        public static void RegisterBinaryExpressionCompiler(BinaryExpressionCompilerFunc compilerFunc) {
            Binary.CompilerFuncsWithContext.Add((item, _cx) => compilerFunc(item));
        }

        public static void RegisterBinaryExpressionCompilerWithContext(BinaryExpressionCompilerWithContextFunc compilerFunc) {
            Binary.CompilerFuncsWithContext.Add(compilerFunc);
        }
    }

}
