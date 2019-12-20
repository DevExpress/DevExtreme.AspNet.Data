using System;
using System.Linq.Expressions;

namespace DevExtreme.AspNet.Data.Tests {

    static class Extensions {
        public static Expression BuildLoadExpr<T>(this DataSourceExpressionBuilder<T> builder) => builder.BuildLoadExpr(true);
        public static Expression BuildLoadGroupsExpr<T>(this DataSourceExpressionBuilder<T> builder) => builder.BuildLoadGroupsExpr(false);
    }

}
