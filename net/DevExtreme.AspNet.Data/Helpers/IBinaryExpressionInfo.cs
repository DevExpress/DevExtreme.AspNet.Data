using System;
using System.Linq.Expressions;

namespace DevExtreme.AspNet.Data.Helpers {

    public interface IBinaryExpressionInfo {
        Expression DataItemExpression { get; }
        string AccessorText { get; }
        string Operation { get; }
        object Value { get; }
    }

}
