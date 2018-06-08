using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.Types {

    partial class AnonType {
        public const string ITEM_PREFIX = "I";

        public virtual int Size {
            get { return 0; }
        }

        public virtual object this[int index] {
            get { throw new ArgumentOutOfRangeException(); }
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

        public static NewExpression CreateNewExpression(IEnumerable<Expression> expressions) {
            var typeArguments = expressions.Select(i => i.Type).ToArray();
            var type = Get(typeArguments);

            return Expression.New(
                type.GetConstructor(typeArguments),
                expressions,
                Enumerable.Range(0, typeArguments.Length).Select(i => type.GetField(ITEM_PREFIX + i))
            );
        }

    }

}
