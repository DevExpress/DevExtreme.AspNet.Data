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
        public class Department {
            [Key]
            public Guid ID { get; set; }
        }

        [Persistent(nameof(Bug486) + "_" + nameof(Employee))]
        public class Employee {
            [Key]
            public Guid ID { get; set; }

            [Association("Department-Employees")]
            public Department Department { get; set; }
        }

        [Fact]
        public async Task Scenario() {
            await UnitOfWorkHelper.ExecAsync(uow => {
                {
                    var department = new Department();
                    var employee = new Employee {
                        Department = department
                    };

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
