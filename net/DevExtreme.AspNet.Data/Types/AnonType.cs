using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.Types {

    partial class AnonType {
        protected virtual int Size {
            get { return 0; }
        }

        internal virtual object this[int index] {
            get { throw new IndexOutOfRangeException(); }
        }

        public override bool Equals(object obj) {
            var other = obj as AnonType;
            if(other == null)
                return false;

            if(other.Size != Size)
                return false;

            for(var i = 0; i < Size; i++) {
                if(!Equals(this[i], other[i]))
                    return false;
            }

            return true;
        }

        public override int GetHashCode() {
            // http://stackoverflow.com/a/1646913
            unchecked {
                var result = 17;
                for(var i = 0; i < Size; i++)
                    result = result * 31 + EqualityComparer<object>.Default.GetHashCode(this[i]);
                return result;
            }
        }

        public static Type Get(IList<Type> typeArguments) {
            var size = typeArguments.Count;

            if(size == 0)
                return typeof(AnonType);

            if(size > MAX_SIZE)
                throw new ArgumentException("Too many type arguments");

            size = SnapSize(size);

            typeArguments = new List<Type>(typeArguments);
            while(typeArguments.Count < size)
                typeArguments.Add(typeof(bool));

            return GetTemplate(size).MakeGenericType(typeArguments.ToArray());
        }

        public static Expression CreateNewExpression(IReadOnlyCollection<Expression> expressions, AnonTypeNewTweaks tweaks) {
            if(tweaks != null) {
                if(!tweaks.AllowEmpty && expressions.Count < 1)
                    return Expression.Constant(1);

                if(!tweaks.AllowUnusedMembers)
                    expressions = Pad(expressions, SnapSize(expressions.Count));
            }

            var typeArguments = expressions.Select(i => i.Type).ToArray();
            return CreateNewExpression(Get(typeArguments), typeArguments, expressions, true);
        }

        public static Expression CreateNewExpression(Type type, Type[] typeArguments, IEnumerable<Expression> expressions, bool useFields) {
            return Expression.New(
                type.GetConstructor(typeArguments),
                expressions,
                Enumerable.Range(0, typeArguments.Length).Select(i => {
                    var name = IndexToField(i);
                    return useFields
                        ? (MemberInfo)type.GetField(name)
                        : (MemberInfo)type.GetProperty(name);
                })
            );
        }

        public static string IndexToField(int index) {
            return "I" + index;
        }

        public static int FieldToIndex(string field) {
            return int.Parse(field.Substring(1));
        }

        static IReadOnlyCollection<Expression> Pad(IReadOnlyCollection<Expression> collection, int totalCount) {
            var padLen = totalCount - collection.Count;

            if(padLen < 1)
                return collection;

            var padding = Enumerable.Repeat(Expression.Constant(false), padLen);
            return new List<Expression>(collection.Concat(padding));
        }
    }

}
