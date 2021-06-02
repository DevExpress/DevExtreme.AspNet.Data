using DevExpress.Xpo;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.Xpo {
    public class Bug486 {

        [Persistent(nameof(Bug486) + "_" + nameof(Department))]
        public class Department : XPObject {
            string _title;

            public Department(Session session)
                : base(session) {
            }

            public string Title {
                get => _title;
                set => SetPropertyValue(nameof(Title), ref _title, value);
            }

            [Association("Department-Employees")]
            public XPCollection<Employee> Employees => GetCollection<Employee>(nameof(Employees));
        }

        [Persistent(nameof(Bug486) + "_" + nameof(Employee))]
        public class Employee : XPObject {
            Department _department;

            public Employee(Session session)
                : base(session) {
            }

            [Association("Department-Employees")]
            public Department Department {
                get => _department;
                set => SetPropertyValue("Department", ref _department, value);
            }
        }

        [Fact]
        public async Task Scenario() {
            await UnitOfWorkHelper.ExecAsync(uow => {
                { 
                    var department = new Department(uow);
                    var employee = new Employee(uow);

                    department.Employees.Add(employee);

                    uow.Save(department);
                    uow.Save(employee);

                    uow.CommitChanges();
                }
                {
                    var employees = uow.Query<Employee>();
                    var department = uow.Query<Department>().First();

                    var loadOptions = new SampleLoadOptions {
                        Filter = new object[] {
                            nameof(Employee.Department),
                            department
                        }
                    };

                    var exception = Record.Exception(() => DataSourceLoader.Load(employees, loadOptions));

                    Assert.Null(exception);
                }
            });
        }

    }
}
