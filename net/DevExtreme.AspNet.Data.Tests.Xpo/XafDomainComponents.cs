using DevExpress.Xpo;
using DevExtreme.AspNet.Data.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.Xpo {

    public class XafDomainComponents {

        public abstract class DCBaseObject : XPCustomObject {
            public DCBaseObject(Session session)
                : base(session) {
            }

            [Key]
            public Guid Oid { get; set; }
        }

        interface IMyComponent {
            int Value { get; set; }
        }

        [Persistent(nameof(XafDomainComponents) + "_" + nameof(MyComponentImpl))]
        public class MyComponentImpl : DCBaseObject, IMyComponent {

            public MyComponentImpl(Session session)
                : base(session) {
            }

            public int Value { get; set; }
        }

        const string
            OID = "Oid",
            LOCK_FILED = "OptimisticLockFieldInDataLayer";

        [Fact]
        public async Task Scenario() {
            try {
                CustomAccessorCompilers.Register((target, accessorText) => {
                    if(accessorText == OID) {
                        return Expression.Property(
                            Expression.Convert(target, typeof(DCBaseObject)),
                            OID
                        );
                    }

                    if(accessorText == LOCK_FILED) {
                        return Expression.Call(
                            Expression.Convert(target, typeof(PersistentBase)),
                            "GetPropertyValue",
                            null,
                            Expression.Constant(LOCK_FILED)
                        );
                    }

                    return null;
                });

                var key = Guid.NewGuid();

                await UnitOfWorkHelper.ExecAsync(uow => {
                    uow.Save(new MyComponentImpl(uow) {
                        Oid = key,
                        Value = 123
                    });

                    uow.CommitChanges();

                    IQueryable<IMyComponent> interfaceQuery = uow.Query<MyComponentImpl>();

                    var loadResult = DataSourceLoader.Load(interfaceQuery, new SampleLoadOptions {
                        PrimaryKey = new[] { OID },
                        RemoteSelect = false,
                        PreSelect = new[] { OID, "Value", LOCK_FILED }
                    });

                    var item = loadResult.data.Cast<IDictionary<string, object>>().First();

                    Assert.Equal(key, item[OID]);
                    Assert.Equal(123, item["Value"]);
                    Assert.Equal(0, item[LOCK_FILED]);
                });
            } finally {
                CustomAccessorCompilers.Clear();
            }
        }

    }

}
