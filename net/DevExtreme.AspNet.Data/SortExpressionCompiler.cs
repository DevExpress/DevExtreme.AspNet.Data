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

                var accessorExpr = CompileAccessorExpression(dataItemExpr, selector);

                target = Expression.Call(typeof(Queryable), Utils.GetSortMethod(first, item.Desc), new[] { ItemType, accessorExpr.Type }, target, Expression.Quote(Expression.Lambda(accessorExpr, dataItemExpr)));
                first = false;
            }

            return target;
        }


    }

}
