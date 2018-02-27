using DevExtreme.AspNet.Data.Aggregation;
using System;
using System.Linq;

namespace DevExtreme.AspNet.Data.Tests {

    static class CustomAggregatorsBarrier {
        static readonly object SYNC = new object();

        public static void Run(Action action) {
            lock(SYNC) {
                try {
                    action();
                } finally {
                    CustomAggregators.Clear();
                }
            }

        }
    }

}
