using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.Helpers {

    public static class Compat {

        internal static bool IsEntityFrramework(IQueryProvider provider) {
            var type = provider.GetType().FullName;

            return type == "System.Data.Entity.Internal.Linq.DbQueryProvider"
                || type == "Microsoft.Data.Entity.Query.Internal.EntityQueryProvider";
        }

    }

}
