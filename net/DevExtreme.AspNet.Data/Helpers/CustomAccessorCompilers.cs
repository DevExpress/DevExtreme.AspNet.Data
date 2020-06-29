using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;

namespace DevExtreme.AspNet.Data.Helpers {

    public static class CustomAccessorCompilers {
        public delegate Expression CompilerFunc(Expression expr, string accessorText);

        static readonly ICollection<CompilerFunc> _compilers = new List<CompilerFunc>();

        public static void Register(CompilerFunc compilerFunc) {
            _compilers.Add(compilerFunc);
        }

        internal static Expression TryCompile(Expression expr, string accessorText) {
            if(_compilers.Count < 1)
                return null;

            foreach(var compiler in _compilers) {
                var result = compiler(expr, accessorText);
                if(result != null)
                    return result;
            }

            return null;
        }

#if DEBUG
        internal static void Clear() {
            _compilers.Clear();
        }
#endif

    }

}
