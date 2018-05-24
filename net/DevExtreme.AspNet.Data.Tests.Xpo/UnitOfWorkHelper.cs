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
                    var dict = new ReflectionDictionary();
                    dict.GetDataStoreSchema(
                        typeof(GenericTestEntity)
                    );

                    var provider = XpoDefault.GetConnectionProvider(
                        new SqlServerTestDbHelper("DevExtreme_AspNet_Data_Tests_Xpo_DB").ConnectionString,
                        AutoCreateOption.DatabaseAndSchema
                    );

                    DATA_LAYER = new SimpleDataLayer(dict, provider);
                }

                using(var purger = new Session(DATA_LAYER)) {
                    purger.ExecuteNonQuery("delete from " + nameof(GenericTestEntity));
                }

                using(var uow = new UnitOfWork(DATA_LAYER)) {
                    action(uow);
                }
            }
        }
    }

}
