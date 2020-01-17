﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace DevExtreme.AspNet.Data.Helpers {
    using BinaryExpressionCompilerFunc = Func<IBinaryExpressionInfo, Expression>;

    public static class CustomFilterCompilers {

        public static class Binary {
            public readonly static ICollection<BinaryExpressionCompilerFunc> CompilerFuncs = new List<BinaryExpressionCompilerFunc>();

            public static Expression TryCompile(IBinaryExpressionInfo info) {
                foreach(var func in CompilerFuncs) {
                    var result = func(info);
                    if(result != null)
                        return result;
                }
                return null;
            }
        }

        public static void RegisterBinaryExpressionCompiler(BinaryExpressionCompilerFunc compilerFunc) {
            Binary.CompilerFuncs.Add(compilerFunc);
        }

    }

}
