using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.Helpers {

    public static class Compat {
        const string EF_CORE_PROVIDER_TYPE = "Microsoft.EntityFrameworkCore.Query.Internal.EntityQueryProvider";

        internal static bool IsEntityFramework(IQueryProvider provider) {
            switch(provider.GetType().FullName) {
                case "System.Data.Entity.Internal.Linq.DbQueryProvider":
                case "Microsoft.Data.Entity.Query.Internal.EntityQueryProvider":
                case EF_CORE_PROVIDER_TYPE:
                    return true;
            }

            return false;
        }

        internal static bool IsEFCore(IQueryProvider provider) {
            return provider.GetType().FullName == EF_CORE_PROVIDER_TYPE;
        }

        internal static bool IsXPO(IQueryProvider provider) {
            return provider.GetType().FullName.StartsWith("DevExpress.Xpo.XPQuery");
        }
    }

}
