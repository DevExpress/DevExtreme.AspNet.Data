using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.RemoteGrouping {

    class RemoteGroupKey<T0, T1, T2, T3, T4, T5, T6, T7> : IEquatable<RemoteGroupKey<T0, T1, T2, T3, T4, T5, T6, T7>> {
        T0 _k0;
        T1 _k1;
        T2 _k2;
        T3 _k3;
        T4 _k4;
        T5 _k5;
        T6 _k6;
        T7 _k7;

        public RemoteGroupKey() {
        }

        public RemoteGroupKey(T0 k0) {
            _k0 = k0;
        }

        public RemoteGroupKey(T0 k0, T1 k1)
            : this(k0) {
            _k1 = k1;
        }

        public RemoteGroupKey(T0 k0, T1 k1, T2 k2)
            : this(k0, k1) {
            _k2 = k2;
        }

        public RemoteGroupKey(T0 k0, T1 k1, T2 k2, T3 k3)
            : this(k0, k1, k2) {
            _k3 = k3;
        }

        public RemoteGroupKey(T0 k0, T1 k1, T2 k2, T3 k3, T4 k4)
            : this(k0, k1, k2, k3) {
            _k4 = k4;
        }

        public RemoteGroupKey(T0 k0, T1 k1, T2 k2, T3 k3, T4 k4, T5 k5)
            : this(k0, k1, k2, k3, k4) {
            _k5 = k5;
        }

        public RemoteGroupKey(T0 k0, T1 k1, T2 k2, T3 k3, T4 k4, T5 k5, T6 k6)
            : this(k0, k1, k2, k3, k4, k5) {
            _k6 = k6;
        }

        public RemoteGroupKey(T0 k0, T1 k1, T2 k2, T3 k3, T4 k4, T5 k5, T6 k6, T7 k7)
            : this(k0, k1, k2, k3, k4, k5, k6) {
            _k7 = k7;
        }

        public T0 K0 { get { return _k0; } }
        public T1 K1 { get { return _k1; } }
        public T2 K2 { get { return _k2; } }
        public T3 K3 { get { return _k3; } }
        public T4 K4 { get { return _k4; } }
        public T5 K5 { get { return _k5; } }
        public T6 K6 { get { return _k6; } }
        public T7 K7 { get { return _k7; } }

        public bool Equals(RemoteGroupKey<T0, T1, T2, T3, T4, T5, T6, T7> other) {
            return Equals(_k0, other._k0)
                && Equals(_k1, other._k1)
                && Equals(_k2, other._k2)
                && Equals(_k3, other._k3)
                && Equals(_k4, other._k4)
                && Equals(_k5, other._k5)
                && Equals(_k6, other._k6)
                && Equals(_k7, other._k7);
        }
        public override bool Equals(object obj) {
            var key = obj as RemoteGroupKey<T0, T1, T2, T3, T4, T5, T6, T7>;
            return key != null && Equals(key);
        }

        public override int GetHashCode() {
            return GetHashCode(_k0, _k1, _k2, _k3, _k4, _k5, _k6, _k7);
        }

        static int GetHashCode(params object[] items) {
            // http://stackoverflow.com/a/1646913
            unchecked {
                var result = 17;
                foreach(var i in items)
                    result = result * 31 + (i == null ? 0 : i.GetHashCode());
                return result;
            }            
        }

    }

}
