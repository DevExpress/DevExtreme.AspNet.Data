using DevExpress.Xpo;
using DevExpress.Xpo.DB;
using DevExpress.Xpo.Metadata;
using System;

namespace DevExtreme.AspNet.Data.Tests.Xpo {

    class UnitOfWorkHelper {
        static readonly object LOCK = new object();
        static IDataLayer DATA_LAYER;

        public static void Exec(Action<UnitOfWork> action) {
            XpoDefault.Session = null;

            lock(LOCK) {
                if(DATA_LAYER == null) {
                    var sqlHelper = new SqlServerTestDbHelper("DevExtreme_AspNet_Data_Tests_Xpo_DB");
                    sqlHelper.ResetDatabase();

                    var dict = new ReflectionDictionary();
                    dict.GetDataStoreSchema(
                        typeof(GenericTestEntity)
                    );

                    var provider = XpoDefault.GetConnectionProvider(
                        sqlHelper.ConnectionString,
                        AutoCreateOption.SchemaOnly
                    );

                    DATA_LAYER = new SimpleDataLayer(dict, provider);
                }

                try {
                    using(var uow = new UnitOfWork(DATA_LAYER)) {
                        action(uow);
                    }
                } finally {
                    using(var purger = new Session(DATA_LAYER)) {
                        purger.ExecuteNonQuery("delete from " + nameof(GenericTestEntity));
                    }
                }
            }
        }
    }

}
