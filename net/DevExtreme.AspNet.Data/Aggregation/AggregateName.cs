using System;
using System.Linq;

namespace DevExtreme.AspNet.Data.Aggregation {

    static class AggregateName {
        public const string
            MIN = "min",
            MAX = "max",
            SUM = "sum",
            AVG = "avg",
            COUNT = "count";

        public const string
            COUNT_NOT_NULL = "cnn",
            REMOTE_COUNT = "remoteCount",
            REMOTE_AVG = "remoteAvg";
    }

}
