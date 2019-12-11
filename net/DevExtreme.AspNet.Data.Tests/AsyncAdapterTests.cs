using DevExtreme.AspNet.Data.Async;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests {

    public class AsyncAdapterTests {
        readonly IQueryable<int> SAMPLE_DATA = new EnumerableQuery<int>(new int[0]);

        [Fact]
        public async Task NotSupported() {
            var error = await Record.ExceptionAsync(async delegate {
                await DataSourceLoader.LoadAsync(SAMPLE_DATA, new SampleLoadOptions());
            });

            Assert.True(error is NotSupportedException);
        }

        [Fact]
        public async Task AllowAsyncOverSync() {
            var error = await Record.ExceptionAsync(async delegate {
                await DataSourceLoader.LoadAsync(SAMPLE_DATA, new SampleLoadOptions {
                    AllowAsyncOverSync = true
                });
            });

            Assert.Null(error);
        }

        [Fact]
        public async Task Canceled() {
            var token = new CancellationToken(true);

            async Task Case(DataSourceLoadOptionsBase loadOptions) {
                var error = await Record.ExceptionAsync(async delegate {
                    await DataSourceLoader.LoadAsync(SAMPLE_DATA, loadOptions, token);
                });

                Assert.True(error is OperationCanceledException);
            }

            await Case(new SampleLoadOptions());
            await Case(new SampleLoadOptions { IsCountQuery = true });
        }

        [Fact]
        public async Task RegisterAdapter() {
            try {
                var adapter = new MyAdapter();
                CustomAsyncAdapters.RegisterAdapter(typeof(EnumerableQuery), adapter);

                await DataSourceLoader.LoadAsync(SAMPLE_DATA, new SampleLoadOptions {
                    RequireTotalCount = true
                });

                Assert.True(adapter.CountCalled);
                Assert.True(adapter.ToEnumerableCalled);
            } finally {
                CustomAsyncAdapters.Clear();
            }
        }

        class MyAdapter : IAsyncAdapter {
            public bool
                CountCalled,
                ToEnumerableCalled;

            public Task<int> CountAsync(IQueryProvider provider, Expression expr, CancellationToken cancellationToken) {
                CountCalled = true;
                return Task.FromResult(provider.Execute<int>(expr));
            }

            public Task<IEnumerable<T>> ToEnumerableAsync<T>(IQueryProvider provider, Expression expr, CancellationToken cancellationToken) {
                ToEnumerableCalled = true;
                return Task.FromResult(provider.CreateQuery<T>(expr).AsEnumerable());
            }
        }
    }

}
