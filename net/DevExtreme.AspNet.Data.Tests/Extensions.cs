using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace DevExtreme.AspNet.Data.Tests {

    static class Extensions {

        public static Expression BuildLoadExpr<T>(this DataSourceExpressionBuilder<T> builder, bool paginate = true) {
            return builder.BuildLoadExpr(CreateSourceExpr<T>(), paginate);
        }

        public static Expression BuildCountExpr<T>(this DataSourceExpressionBuilder<T> builder) {
            return builder.BuildCountExpr(CreateSourceExpr<T>());
        }

        public static Expression BuildLoadGroupsExpr<T>(this DataSourceExpressionBuilder<T> builder) {
            return builder.BuildLoadGroupsExpr(CreateSourceExpr<T>());
        }

        static Expression CreateSourceExpr<T>() {
            return Expression.Parameter(typeof(IQueryable<T>), "data");
        }

    }

}
