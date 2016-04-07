using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data {

    class DataSourceExpressionBuilder<T> {

        public int Skip { get; set; }
        public int Take { get; set; }
        public IList Filter { get; set; }
        public SortingInfo[] Sort { get; set; }
        public GroupingInfo[] Group { get; set; }

        public bool HasGroups {
            get { return Group != null && Group.Length > 0; }
        }

        public string[] GetGroupSelectors() {
            return Group.Select(i => i.Selector).ToArray();
        }

        public LambdaExpression Build(bool isCountQuery) {
            var queryableType = typeof(Queryable);
            var genericTypeArguments = new[] { typeof(T) };
            var paramExpr = Expression.Parameter(typeof(IQueryable<T>), "data");

            Expression body = paramExpr;

            if(Filter != null)
                body = Expression.Call(queryableType, "Where", genericTypeArguments, body, new FilterExpressionCompiler<T>().Compile(Filter));

            if(!isCountQuery) {
                if(Sort != null)
                    body = new SortExpressionCompiler<T>().Compile(body, GetFullSort());

                if(Skip > 0)
                    body = Expression.Call(queryableType, "Skip", genericTypeArguments, body, Expression.Constant(Skip));

                if(Take > 0)
                    body = Expression.Call(queryableType, "Take", genericTypeArguments, body, Expression.Constant(Take));
            }

            if(isCountQuery)
                body = Expression.Call(queryableType, "Count", genericTypeArguments, body);

            return Expression.Lambda(body, paramExpr);
        }

        IEnumerable<SortingInfo> GetFullSort() {
            if(!HasGroups)
                return Sort;

            var memo = new HashSet<string>();
            var result = new List<SortingInfo>();

            foreach(var g in Group) {
                memo.Add(g.Selector);
                result.Add(g);
            }

            foreach(var s in Sort) {
                if(!memo.Contains(s.Selector))
                    result.Add(s);
            }

            return result;
        }
    }

}
