using DevExtreme.AspNet.Data.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data {

    class SortExpressionCompiler : ExpressionCompiler {

        public SortExpressionCompiler(Type itemType, bool guardNulls)
            : base(itemType, guardNulls) {
        }

        public Expression Compile(Expression target, IEnumerable<SortingInfo> clientExprList) {
            var dataItemExpr = CreateItemParam();
            var first = true;

            foreach(var item in clientExprList) {
                var selector = item.Selector;
                if(String.IsNullOrEmpty(selector))
                    continue;

                var customTarget = CustomSortCompilers.Sort.CompilerFuncs.Count == 0 ? null
                    : CustomSortCompilers.Sort.TryCompile(target, new SortExpressionInfo {
                        DataItemExpression = dataItemExpr,
                        AccessorText = selector,
                        Desc = item.Desc,
                        First = first
                    });

                if(customTarget == null) {
                    var accessorExpr = CompileAccessorExpression(dataItemExpr, selector);
                    target = Expression.Call(typeof(Queryable), Utils.GetSortMethod(first, item.Desc), new[] { ItemType, accessorExpr.Type }, target, Expression.Quote(Expression.Lambda(accessorExpr, dataItemExpr)));
                } else {
                    target = customTarget;
                }

                first = false;
            }

            return target;
        }

        class SortExpressionInfo : ISortExpressionInfo {
            public Expression DataItemExpression { get; set; }
            public string AccessorText { get; set; }
            public bool Desc { get; set; }
            public bool First { get; set; }
        }
    }

}
