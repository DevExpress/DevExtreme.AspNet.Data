using System;
using System.Linq;

namespace DevExtreme.AspNet.Data.Tests {

    static class Compat {

        public static bool CanUseRemoteAvg(IQueryProvider provider) {
            var providerInfo = new QueryProviderInfo(provider);

            if(providerInfo.IsEFCore) {
                var version = providerInfo.Version.Major;

                // https://github.com/aspnet/EntityFrameworkCore/issues/11711
                if(version == 2 || version == 3)
                    return false;
            }

            return true;
        }

    }

}
