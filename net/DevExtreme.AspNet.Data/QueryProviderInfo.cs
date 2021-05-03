using System;
using System.Linq;
using System.Reflection;

namespace DevExtreme.AspNet.Data {

    class QueryProviderInfo {
        public readonly bool IsLinqToObjects;
        public readonly bool IsEFClassic;
        public readonly bool IsEFCore;
        public readonly bool IsXPO;
        public readonly bool IsNH;
        public readonly bool IsL2S;
        public readonly bool IsMongoDB;
        public readonly Version Version;

        public QueryProviderInfo(IQueryProvider provider) {
            if(provider is EnumerableQuery) {
                IsLinqToObjects = true;
            } else {
                var type = provider.GetType();
                var typeName = type.FullName;
                var providerAssembly = type.Assembly;

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
                else if(typeName.StartsWith("MongoDB.Driver.Linq."))
                    IsMongoDB = true;
                else if(typeName.StartsWith("LinqKit.ExpandableQueryProvider`1")) {
                    switch(providerAssembly.GetName().Name) {
                        case "LinqKit.Microsoft.EntityFrameworkCore":
                            IsEFCore = true;
                            providerAssembly = Assembly.Load("Microsoft.EntityFrameworkCore");
                            break;

                        case "LinqKit.EntityFramework":
                            IsEFClassic = true;
                            providerAssembly = Assembly.Load("EntityFramework");
                            break;
                    }
                }

                Version = providerAssembly.GetName().Version;
            }
        }
    }

}
