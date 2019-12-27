using DevExtreme.AspNet.Data.Helpers;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.EFCore {

    public class ShadowProps {
        public const string SHADOW_NAME = "ShadowCol";

        [Table(nameof(ShadowProps) + "_" + nameof(DataItem))]
        public class DataItem  {
            public int ID { get; set; }
        }

        [Fact]
        public async Task Scenario() {
            await TestDbContext.ExecAsync(context => {
                var set = context.Set<DataItem>();

                for(var i = 0; i < 3; i++) {
                    var item = new DataItem();
                    context.Entry(item).Property(SHADOW_NAME).CurrentValue = i;
                    set.Add(item);
                }
                context.SaveChanges();

                Expression<Func<IQueryable>> sourceExpr = () => set.Where(i => i.ID != 123);
                var source = (set as IQueryable).Provider.CreateQuery<DataItem>(sourceExpr.Body);

                RunIsolated(source);
            });
        }

        static void RunIsolated(IQueryable<DataItem> source) {
            var dbSet = (new DbSetFinder().Visit(source.Expression) as ConstantExpression).Value;
            var dbSetItemType = dbSet.GetType().GetGenericArguments().First();
            var dbContext = (dbSet as IInfrastructure<IServiceProvider>).GetService<ICurrentDbContext>().Context;
            var shadowType = dbContext.Model.FindEntityType(dbSetItemType).FindProperty(SHADOW_NAME).ClrType;

            CustomAccessorCompilers.Register((expr, accessorText) => {
                if(expr.Type == typeof(DataItem) && String.Compare(accessorText, SHADOW_NAME, true) == 0) {
                    return Expression.Call(typeof(EF), nameof(EF.Property),
                        new[] { shadowType },
                        expr, Expression.Constant(SHADOW_NAME)
                    );
                }

                return null;
            });

            try {

                var loadResult = DataSourceLoader.Load(source, new SampleLoadOptions {
                    Filter = new[] { SHADOW_NAME, "<>", "1" },
                    Sort = new[] {
                        new SortingInfo { Selector = SHADOW_NAME, Desc = true }
                    },
                    Select = new[] { SHADOW_NAME }
                });

                var items = loadResult.data.Cast<IDictionary<string, object>>().ToArray();

                Assert.Equal(2, items.Length);
                Assert.Equal(2, items[0][SHADOW_NAME]);
                Assert.Equal(0, items[1][SHADOW_NAME]);
            } finally {
                CustomAccessorCompilers.Clear();
            }
        }

        class DbSetFinder : ExpressionVisitor {

            protected override Expression VisitMethodCall(MethodCallExpression node) {
                if(node.Method.DeclaringType == typeof(Queryable))
                    return Visit(node.Arguments[0]);

                // context.Set<T>()
                if(node.Arguments.Count < 1) {
                    if(Visit(node.Object) is ConstantExpression target)
                        return Expression.Constant(node.Method.Invoke(target.Value, new object[0]));
                }

                return base.VisitMethodCall(node);
            }

            protected override Expression VisitMember(MemberExpression node) {
                // Closure captured vars
                if(node.Member is FieldInfo f && node.Expression is ConstantExpression c)
                    return Expression.Constant(f.GetValue(c.Value));

                return base.VisitMember(node);
            }

        }

    }

}
