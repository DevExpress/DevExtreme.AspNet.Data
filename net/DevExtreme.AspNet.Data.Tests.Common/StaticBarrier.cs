using DevExtreme.AspNet.Data.Aggregation;
using DevExtreme.AspNet.Data.Helpers;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.Tests {

    public class StaticBarrier {
        static readonly SemaphoreSlim SYNC = new SemaphoreSlim(1, 1);

        public static async Task RunAsync(Func<Task> action) {
            await SYNC.WaitAsync();
            try {
                await action();
            } finally {
                CustomAggregators.Clear();
                CustomAccessorCompilers.Clear();
                DataSourceLoadOptionsBase.StringToLowerDefault = null;
                SYNC.Release();
            }
        }

        public static async Task RunAsync(Action action) {
            await RunAsync(delegate {
                action();
                return Task.CompletedTask;
            });
        }
    }

}
