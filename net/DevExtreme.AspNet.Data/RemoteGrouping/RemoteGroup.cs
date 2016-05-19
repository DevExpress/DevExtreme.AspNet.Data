using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DevExtreme.AspNet.Data.RemoteGrouping {

    class RemoteGroup<TK0, TK1, TK2, TK3, TK4, TK5, TK6, TK7, TT0, TT1, TT2, TT3, TT4, TT5, TT6, TT7, TG0, TG1, TG2, TG3, TG4, TG5, TG6, TG7> : IRemoteGroup {

        #pragma warning disable CS0649

        public TK0 K0;
        public TK1 K1;
        public TK2 K2;
        public TK3 K3;
        public TK4 K4;
        public TK5 K5;
        public TK6 K6;
        public TK7 K7;

        public TT0 T0;
        public TT1 T1;
        public TT2 T2;
        public TT3 T3;
        public TT4 T4;
        public TT5 T5;
        public TT6 T6;
        public TT7 T7;

        public TG0 G0;
        public TG1 G1;
        public TG2 G2;
        public TG3 G3;
        public TG4 G4;
        public TG5 G5;
        public TG6 G6;
        public TG7 G7;

        #pragma warning restore CS0649

        public int Count { get; set; }

        object IRemoteGroup.GetKey(int index) {
            switch(index) {
                case 0: return K0;
                case 1: return K1;
                case 2: return K2;
                case 3: return K3;
                case 4: return K4;
                case 5: return K5;
                case 6: return K6;
                case 7: return K7;
            }

            throw new ArgumentOutOfRangeException();
        }

        object IRemoteGroup.GetTotalAggregate(int index) {
            switch(index) {
                case 0: return T0;
                case 1: return T1;
                case 2: return T2;
                case 3: return T3;
                case 4: return T4;
                case 5: return T5;
                case 6: return T6;
                case 7: return T7;
            }

            throw new ArgumentOutOfRangeException();
        }

        object IRemoteGroup.GetGroupAggregate(int index) {
            switch(index) {
                case 0: return G0;
                case 1: return G1;
                case 2: return G2;
                case 3: return G3;
                case 4: return G4;
                case 5: return G5;
                case 6: return G6;
                case 7: return G7;
            }

            throw new ArgumentOutOfRangeException();
        }
    }

}
