#if DEBUG
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DevExtreme.AspNet.Data {

    class EnumerableOnce<T> : IEnumerable<T> {
        IEnumerable<T> _source;
        bool _enumerated;

        public EnumerableOnce(IEnumerable<T> source) {
            _source = source;
        }

        IEnumerator<T> IEnumerable<T>.GetEnumerator() {
            return GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() {
            return GetEnumerator();
        }

        IEnumerator<T> GetEnumerator() {
            if(_enumerated)
                throw new InvalidOperationException("Sequence already enumerated");

            _enumerated = true;
            return _source.GetEnumerator();
        }
    }

}
#endif
