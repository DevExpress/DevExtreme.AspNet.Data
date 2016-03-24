using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.Helpers {

    public static class Compat {
        public static bool EF3361;

#warning remove when https://github.com/aspnet/EntityFramework/issues/3361 is fixed (RC2)
        internal static Expression Workaround_EF3361(DateTime date) {
            Expression<Func<DateTime>> closure = () => date;
            return closure.Body;
        }
    }

}
