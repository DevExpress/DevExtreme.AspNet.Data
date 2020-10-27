using System;
using System.Linq.Expressions;

namespace DevExtreme.AspNet.Data.Helpers {

    public interface ISortExpressionInfo {
        Expression DataItemExpression { get; }
        string AccessorText { get; }
        bool Desc { get; set; }
        bool First { get; set; }
    }
}
