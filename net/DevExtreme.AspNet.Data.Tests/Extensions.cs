using System;
using System.Linq.Expressions;

namespace DevExtreme.AspNet.Data.Tests {

    static class Extensions {
        public static Expression BuildLoadExpr(this DataSourceExpressionBuilder builder) => builder.BuildLoadExpr(true);
        public static Expression BuildLoadGroupsExpr(this DataSourceExpressionBuilder builder) => builder.BuildLoadGroupsExpr(false);
    }

}
