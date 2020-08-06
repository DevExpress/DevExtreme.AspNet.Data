using System;

namespace DevExtreme.AspNet.Data.Types {

    class DynamicClassAdapter : AnonType {
        readonly object Obj;

        public DynamicClassAdapter(object obj) {
#if DEBUG
            DynamicClassBridge.ValidateInstance(obj);
#endif
            Obj = obj;
        }

        internal override object this[int index]
            => DynamicClassBridge.GetMember(Obj, index);

        protected override int Size
            => throw new NotSupportedException();

        public override bool Equals(object obj)
            => Obj.Equals(obj);

        public override int GetHashCode()
            => Obj.GetHashCode();
    }

}
