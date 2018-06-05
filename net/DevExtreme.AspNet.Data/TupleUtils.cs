using DevExtreme.AspNet.Data.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace DevExtreme.AspNet.Data {

    static class TupleUtils {
        public static readonly IAccessor<object> ACCESSOR = new AccessorImpl();

        static readonly PropertyInfo ITEM_PROP;

        static TupleUtils() {
            // Indexer is available in .NET >= 4.7.1 & .NET Core >= 2
            // See https://docs.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.ituple.item
            ITEM_PROP = Type.GetType("System.Runtime.CompilerServices.ITuple")?.GetProperty("Item");
        }

        public static object ReadItem(object tuple, int itemIndex, bool useIndexer = true) {
            if(ITEM_PROP == null || !useIndexer) {
                while(itemIndex > 6) {
                    tuple = tuple.GetType().GetProperty("Rest").GetValue(tuple, null);
                    itemIndex -= 7;
                }
                return tuple.GetType().GetProperty("Item" + (1 + itemIndex)).GetValue(tuple, null);
            }

            return ITEM_PROP.GetValue(tuple, new object[] { itemIndex });
        }

        public static Type CreateType(IList<Type> typeArguments) {
            var result = typeof(object);
            var levelTypeArguments = new Type[typeArguments.Count < 8 ? typeArguments.Count : typeArguments.Count % 7];
            var levelIndex = levelTypeArguments.Length - 1;

            for(var i = typeArguments.Count - 1; i >= 0; i--) {
                levelTypeArguments[levelIndex] = typeArguments[i];

                if(levelIndex == 0) {
                    result = GetTypeTemplate(levelTypeArguments.Length).MakeGenericType(levelTypeArguments);
                    levelTypeArguments = new Type[8];
                    levelTypeArguments[7] = result;
                    levelIndex = 6;
                } else {
                    levelIndex--;
                }
            }

            return result;
        }

        static Type GetTypeTemplate(int count) {
            switch(count) {
                case 1: return typeof(Tuple<>);
                case 2: return typeof(Tuple<,>);
                case 3: return typeof(Tuple<,,>);
                case 4: return typeof(Tuple<,,,>);
                case 5: return typeof(Tuple<,,,,>);
                case 6: return typeof(Tuple<,,,,,>);
                case 7: return typeof(Tuple<,,,,,,>);
                case 8: return typeof(Tuple<,,,,,,,>);
            }
            throw new NotSupportedException();
        }

        public static NewExpression CreateNewExpr(Type tupleType, IEnumerable<Expression> itemExpressions) {
            var typeArguments = tupleType.GetTypeInfo().GenericTypeArguments;
            var size = typeArguments.Length;

            var newExprArguments = itemExpressions.Take(7);
            if(size == 8)
                newExprArguments = newExprArguments.Concat(new[] { CreateNewExpr(typeArguments[7], itemExpressions.Skip(7)) });

            return Expression.New(
                tupleType.GetConstructor(typeArguments),
                newExprArguments,
                Enumerable.Range(1, size).Select(i => tupleType.GetProperty(i < 8 ? "Item" + i : "Rest"))
            );
        }

        public static Expression CreateReadItemExpr(Expression tupleExpr, int itemIndex) {
            while(itemIndex > 6) {
                tupleExpr = Expression.Property(tupleExpr, "Rest");
                itemIndex -= 7;
            }
            return Expression.Property(tupleExpr, "Item" + (1 + itemIndex));
        }


        class AccessorImpl : IAccessor<object> {
            public object Read(object tuple, string itemIndex) {
                return ReadItem(tuple, Int32.Parse(itemIndex));
            }
        }
    }

}
