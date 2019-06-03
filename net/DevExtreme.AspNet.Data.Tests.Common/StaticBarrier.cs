using DevExtreme.AspNet.Data.Aggregation;
using DevExtreme.AspNet.Data.Helpers;
using System;
using System.Linq;

namespace DevExtreme.AspNet.Data.Tests {

    public class StaticBarrier {
        static readonly object SYNC = new object();

        public static void Run(Action action) {
            lock(SYNC) {
                try {
                    action();
                } finally {
                    CustomAggregators.Clear();
                    CustomAccessorCompilers.Clear();
                    CustomFilterCompilers.Clear();
                    DataSourceLoadOptionsBase.StringToLowerDefault = null;
                }
            }

        }
    }

}
