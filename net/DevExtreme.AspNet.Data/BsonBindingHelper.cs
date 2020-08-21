using System;
using System.Linq.Expressions;

namespace DevExtreme.AspNet.Data {

    static class BsonBindingHelper {
        static readonly Type BSON_VALUE_TYPE;

        static BsonBindingHelper() {
            BSON_VALUE_TYPE = Type.GetType("MongoDB.Bson.BsonValue, MongoDB.Bson");
        }

        public static bool IsBsonType(Type type) {
            return BSON_VALUE_TYPE != null && BSON_VALUE_TYPE.IsAssignableFrom(type);
        }

        public static Expression CompileGetMember(Expression expr, string clientExpr) {
            return Expression.Call(expr, "get_Item", Type.EmptyTypes, Expression.Constant(clientExpr));
        }

        public static Expression CreateBsonValueExpr(object value) {
            if(value == null)
                return Expression.Constant(null, BSON_VALUE_TYPE);
            return Expression.Convert(Expression.Constant(value), BSON_VALUE_TYPE);
        }

    }

}
