using System;
using System.Linq;

namespace DevExtreme.AspNet.Data {

    class QueryProviderInfo {
        public readonly bool IsLinqToObjects;
        public readonly bool IsEFClassic;
        public readonly bool IsEFCore;
        public readonly bool IsXPO;
        public readonly Version Version;

        public QueryProviderInfo(IQueryProvider provider) {
            if(provider is EnumerableQuery) {
                IsLinqToObjects = true;
            } else {
                var typeInfo = provider.GetType().AssemblyQualifiedName.Split(new[] { ", " }, 4, StringSplitOptions.None);
                var typeName = typeInfo[0];

                if(typeName == "Microsoft.Data.Entity.Query.Internal.EntityQueryProvider" || typeName == "System.Data.Entity.Internal.Linq.DbQueryProvider")
                    IsEFClassic = true;
                else if(typeName == "Microsoft.EntityFrameworkCore.Query.Internal.EntityQueryProvider")
                    IsEFCore = true;
                else if(typeName.StartsWith("DevExpress.Xpo.XPQuery"))
                    IsXPO = true;

                Version = new Version(typeInfo[2].Substring(8));
            }
        }

        public bool RequiresNullSafety {
            get {
                if(IsLinqToObjects)
                    return true;

                // https://docs.microsoft.com/en-us/ef/core/querying/client-eval
                // https://github.com/aspnet/EntityFrameworkCore/issues/12284
                if(IsEFCore)
                    return true;

                return false;
            }
        }

        public bool SupportsRemoteGrouping {
            get {
                if(IsLinqToObjects)
                    return false;

                // https://github.com/aspnet/EntityFrameworkCore/issues/2341
                // https://github.com/aspnet/EntityFrameworkCore/issues/11993
                if(IsEFCore && Version < new Version(2, 1, 1))
                    return false;

                return true;
            }
        }

    }

}
