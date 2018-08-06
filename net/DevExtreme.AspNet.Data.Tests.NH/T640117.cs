using DevExtreme.AspNet.Data.ResponseModel;
using FluentNHibernate.Mapping;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace DevExtreme.AspNet.Data.Tests.NH {

    public class T640117 {

        public class Project {
            public virtual int Id { get; set; }
            public virtual string Name { get; set; }
            public virtual Client Client { get; set; }
        }

        public class Client {
            public virtual int Id { get; set; }
            public virtual IList<Project> Projects { get; set; }

            public Client() {
                Projects = new List<Project>();
            }
        }

        public class ProjectMap : ClassMap<Project> {
            public ProjectMap() {
                Table(nameof(T640117) + "_" + nameof(Project));
                Id(p => p.Id);
                Map(p => p.Name);
                References(p => p.Client);
            }
        }

        public class ClientMap : ClassMap<Client> {
            public ClientMap() {
                Table(nameof(T640117) + "_" + nameof(Client));
                Id(c => c.Id);
                HasMany(c => c.Projects);
            }
        }

        [Fact]
        public void Scenario() {
            SessionFactoryHelper.Exec(session => {
                var proj = new Project();
                proj.Name = "P1";
                session.Save(proj);

                // 'Blanks' filter issue
                {
                    var loadResult = DataSourceLoader.Load(session.Query<Project>(), new SampleLoadOptions {
                        Filter = new[] { "Client.Id", null }
                    });

                    Assert.Single(loadResult.data);
                }

                // Simulate header filter popup query
                {
                    var loadResult = DataSourceLoader.Load(session.Query<Project>(), new SampleLoadOptions {
                        Group = new[] {
                            new GroupingInfo { Selector = "Client.Id", IsExpanded = false },
                            new GroupingInfo { Selector = "Name", IsExpanded = false },
                        }
                    });

                    Assert.Null(Record.Exception(delegate {
                        loadResult.data.Cast<Group>().First();
                    }));
                }
            });
        }

    }
}
