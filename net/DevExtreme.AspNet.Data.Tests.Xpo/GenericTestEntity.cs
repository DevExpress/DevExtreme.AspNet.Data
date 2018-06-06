using DevExpress.Xpo;
using System;

namespace DevExtreme.AspNet.Data.Tests.Xpo {

    class GenericTestEntity : XPLiteObject {
        Guid _oid;
        string _text;

        public GenericTestEntity(Session session)
            : base(session) {
        }

        [Key(true)]
        public Guid Oid {
            get { return _oid; }
            set { SetPropertyValue(nameof(Oid), ref _oid, value); }
        }

        public string Text {
            get { return _text; }
            set { SetPropertyValue(nameof(Text), ref _text, value); }
        }
    }

}
