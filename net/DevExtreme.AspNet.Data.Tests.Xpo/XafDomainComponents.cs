using DevExpress.Xpo;
using DevExtreme.AspNet.Data.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.Xpo {

    public class XafDomainComponents {

        interface IMyComponent {
            int Value { get; set; }
        }

        [Persistent(nameof(XafDomainComponents) + "_" + nameof(MyComponentImpl))]
        public class MyComponentImpl : XPObject, IMyComponent {

            public MyComponentImpl(Session session)
                : base(session) {
            }

            public int Value { get; set; }
        }

        const string
            OID = "Oid",
            LOCK_FILED = "OptimisticLockFieldInDataLayer";

        [Fact]
        public void Scenario() {
            StaticBarrier.Run(delegate {
                CustomAccessorCompilers.Register((target, accessorText) => {
                    if(accessorText == OID) {
                        return Expression.Property(
                            Expression.Convert(target, typeof(XPObject)),
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

                UnitOfWorkHelper.Exec(uow => {
                    uow.Save(new MyComponentImpl(uow) { Value = 123 });
                    uow.CommitChanges();

                    var loadResult = DataSourceLoader.Load<IMyComponent>(uow.Query<MyComponentImpl>(), new SampleLoadOptions {
                        PrimaryKey = new[] { OID },
                        RemoteSelect = false,
                        PreSelect = new[] { OID, "Value", LOCK_FILED }
                    });

                    var item = loadResult.data.Cast<IDictionary<string, object>>().First();

                    Assert.Equal(1, item[OID]);
                    Assert.Equal(123, item["Value"]);
                    Assert.Equal(0, item[LOCK_FILED]);
                });
            });
        }

    }

}
