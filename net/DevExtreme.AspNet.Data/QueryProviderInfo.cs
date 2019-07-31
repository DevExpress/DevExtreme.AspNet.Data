using System;
using System.Linq;

namespace DevExtreme.AspNet.Data {

    class QueryProviderInfo {
        public readonly bool IsLinqToObjects;
        public readonly bool IsEFClassic;
        public readonly bool IsEFCore;
        public readonly bool IsXPO;
        public readonly bool IsNH;
        public readonly bool IsL2S;
        public readonly Version Version;

        public QueryProviderInfo(IQueryProvider provider) {
            if(provider is EnumerableQuery) {
                IsLinqToObjects = true;
            } else {
                var type = provider.GetType();
                var typeName = type.FullName;

                if(typeName == "Microsoft.Data.Entity.Query.Internal.EntityQueryProvider" || typeName == "System.Data.Entity.Internal.Linq.DbQueryProvider")
                    IsEFClassic = true;
                else if(typeName == "Microsoft.EntityFrameworkCore.Query.Internal.EntityQueryProvider")
                    IsEFCore = true;
                else if(typeName.StartsWith("DevExpress.Xpo.XPQuery"))
                    IsXPO = true;
                else if(typeName.StartsWith("NHibernate.Linq."))
                    IsNH = true;
                else if(typeName.StartsWith("System.Data.Linq."))
                    IsL2S = true;

                Version = type.Assembly.GetName().Version;
            }
        }

    }

}
