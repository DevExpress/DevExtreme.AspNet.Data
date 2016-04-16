#if DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data {

    class QueryableOnce<T> : IQueryable<T> {
        IQueryable<T> _source;
        bool _enumerated;

        public QueryableOnce(IQueryable<T> source) {
            _source = source;
        }

        Type IQueryable.ElementType {
            get { return _source.ElementType; }
        }

        Expression IQueryable.Expression {
            get { return _source.Expression; }
        }

        IQueryProvider IQueryable.Provider {
            get { throw new NotSupportedException(); }
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() {
            return GetEnumerator();
        }

        IEnumerator<T> GetEnumerator() {
            if(_enumerated)
                throw new InvalidOperationException("Query already enumerated");

            _enumerated = true;

            foreach(var i in _source)
                yield return i;
        }
    }

}
#endif