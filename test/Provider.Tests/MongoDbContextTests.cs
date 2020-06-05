using Microsoft.EntityFrameworkCore.ChangeTracking;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Microsoft.EntityFrameworkCore.Mongo.Tests
{
    [Collection("DbContext")]
    public class MongoDbContextTests : MongoContextTestBase, IClassFixture<MongoEntityFixture>
    {
        private readonly Family _onlyFamily;

        public MongoDbContextTests(MongoEntityFixture fixture)
        {
            _onlyFamily = fixture.OnlyFamily;
        }

        [Fact]
        public async Task CanQueryFromMongoDb()
        {
            await ExecuteUnitOfWorkAsync(async dbContext =>
            {
                Assert.Empty(await dbContext.Families.ToListAsync());
                Assert.Empty(await dbContext.Companies.ToListAsync());
            });
        }

        [Fact]
        public async Task CanWriteSimpleRecord()
        {
            var company = new Company { Name = "Microsoft", Valid = true };

            await ExecuteUnitOfWorkAsync(async dbContext =>
            {
                dbContext.Add(company);
                Assert.Equal(1, await dbContext.SaveChangesAsync(acceptAllChangesOnSuccess: true));
            });

            await ExecuteUnitOfWorkAsync(async dbContext =>
            {
                var item = await dbContext.Companies.SingleAsync();
                Assert.Equal(company.Valid, item.Valid);
                Assert.Equal(company.Name, item.Name);
                Assert.Equal(company.Id, item.Id);
            });
        }

        [Fact]
        public async Task CanWriteComplexRecord()
        {
            await ExecuteUnitOfWorkAsync(async dbContext =>
            {
                dbContext.Families.Add(_onlyFamily);
                Assert.Equal(1, await dbContext.SaveChangesAsync(acceptAllChangesOnSuccess: true));
            });
        }

        /*
        [Fact(Skip = "Not checked yet.")]
        public Task CanWritePolymorphicRecords()
        {
            await ExecuteUnitOfWorkAsync(async dbContext =>
            {
                dbContext.Animals.AddRange(_zooEntities.Animals);
                Assert.Equal(
                    _zooEntities.Entities.Count,
                    await dbContext.SaveChangesAsync(acceptAllChangesOnSuccess: true));
            });

            await ExecuteUnitOfWorkAsync(async dbContext =>
            {
                IList<Animal> queriedEntities = await dbContext.Animals
                    .OrderBy(animal => animal.Name)
                    .ThenBy(animal => animal.Height)
                    .ToListAsync();
                Assert.Equal(_zooEntities.Animals,
                    queriedEntities,
                    new AnimalEqualityComparer());
            });
        }

        [Fact]
        public async Task CanUpdateExistingEntity()
        {
            await ExecuteUnitOfWorkAsync(async dbContext =>
            {
                EntityEntry entityEntry = dbContext.Add(_zooEntities.Tigger);
                Assert.Equal(EntityState.Added, entityEntry.State);
                Assert.Equal(7, await dbContext.SaveChangesAsync(acceptAllChangesOnSuccess: true));
                Assert.Equal(EntityState.Unchanged, entityEntry.State);
                Assert.NotNull(_zooEntities.Tigger.ConcurrencyField);

                _zooEntities.Tigger.Name = "Tigra";
                dbContext.ChangeTracker.DetectChanges();
                Assert.Equal(EntityState.Modified, entityEntry.State);
                Assert.Equal(1, await dbContext.SaveChangesAsync(acceptAllChangesOnSuccess: true));
            });

            await ExecuteUnitOfWorkAsync(async dbContext =>
            {
                Tiger tigger = await dbContext.Animals.OfType<Tiger>().FirstOrDefaultAsync();

                Assert.Equal(
                    _zooEntities.Tigger,
                    tigger,
                    new AnimalEqualityComparer());
            });
        }

        [Fact]
        public async Task CanUpdateSubDocument()
        {
            await ExecuteUnitOfWorkAsync(async dbContext =>
            {
                EntityEntry entityEntry = dbContext.Add(_zooEntities.TaigaMasuta);
                Assert.Equal(EntityState.Added, entityEntry.State);
                Assert.Equal(5, await dbContext.SaveChangesAsync(acceptAllChangesOnSuccess: true));
                Assert.Equal(EntityState.Unchanged, entityEntry.State);
            });

            await ExecuteUnitOfWorkAsync(async dbContext =>
            {
                Employee taigaMasuta = await dbContext.Employees
                    .FirstAsync(employee => employee.LastName == _zooEntities.TaigaMasuta.LastName
                                            && employee.FirstName == _zooEntities.TaigaMasuta.FirstName);

                Specialty firstSpecialty = taigaMasuta.Specialties[0];
                EntityEntry<Specialty> specialtyEntry = dbContext.Entry(firstSpecialty);
                Assert.Equal(EntityState.Unchanged, specialtyEntry.State);

                firstSpecialty.AnimalType = nameof(PolarBear);

                dbContext.ChangeTracker.DetectChanges();

                Assert.Equal(EntityState.Modified, specialtyEntry.State);

                Assert.Equal(1, await dbContext.SaveChangesAsync(acceptAllChangesOnSuccess: true));
            });

            await ExecuteUnitOfWorkAsync(async dbContext =>
            {
                Employee taigaMasuta = await dbContext.Employees
                    .FirstAsync(employee => employee.LastName == _zooEntities.TaigaMasuta.LastName
                                            && employee.FirstName == _zooEntities.TaigaMasuta.FirstName
                                            && employee.Specialties
                                                .Any(specialty => specialty.AnimalType == nameof(PolarBear)));

                Assert.NotNull(taigaMasuta);
            });
        }

        [Fact]
        public async Task ConcurrencyFieldPreventsUpdates()
        {
            await ExecuteUnitOfWorkAsync(async dbContext =>
            {
                dbContext.Add(_zooEntities.Tigger);
                Assert.Equal(
                    3 + _zooEntities.TigerEnclosure.WeeklySchedule.Approver.DirectReports.Count,
                    await dbContext.SaveChangesAsync(acceptAllChangesOnSuccess: true));
                Assert.False(string.IsNullOrWhiteSpace(_zooEntities.Tigger.ConcurrencyField));
            });

            await ExecuteUnitOfWorkAsync(async dbContext =>
            {
                Tiger tigger = _zooEntities.Tigger;

                dbContext.Update(tigger);

                string concurrencyToken = tigger.ConcurrencyField;

                await ExecuteUnitOfWorkAsync(async innerdbContext =>
                {
                    Tiger innerTigger = await innerdbContext.Animals
                        .OfType<Tiger>()
                        .SingleOrDefaultAsync(tiger => tiger.Name == _zooEntities.Tigger.Name);

                    innerdbContext.Update(innerTigger);

                    Assert.Equal(1, await innerdbContext.SaveChangesAsync(acceptAllChangesOnSuccess: true));

                    Assert.NotEqual(concurrencyToken, innerTigger.ConcurrencyField);
                });

                Assert.Equal(concurrencyToken, tigger.ConcurrencyField);

                Assert.Equal(0, await dbContext.SaveChangesAsync(acceptAllChangesOnSuccess: true));
            });
        }

        [Fact]
        public async Task CanQueryComplexRecord()
        {
            await ExecuteUnitOfWorkAsync(async dbContext =>
            {
                dbContext.Add(_zooEntities.TaigaMasuta);
                Assert.Equal(
                    _zooEntities.Employees.Count,
                    await dbContext.SaveChangesAsync(acceptAllChangesOnSuccess: true));
            });

            await ExecuteUnitOfWorkAsync(async dbContext =>
            {
                Assert.Equal(
                    _zooEntities.TaigaMasuta,
                    await dbContext.Employees
                        .SingleAsync(searchedEmployee => searchedEmployee.Specialties
                            .Any(specialty => specialty.AnimalType == nameof(Tiger)
                                              && specialty.Task == ZooTask.Feeding)),
                    new EmployeeEqualityComparer());
            });
        }

        [Fact]
        public async Task CanQueryPolymorphicSubTypes()
        {
            await ExecuteUnitOfWorkAsync(async dbContext =>
            {
                dbContext.Animals.AddRange(_zooEntities.Animals);
                Assert.Equal(
                    _zooEntities.Entities.Count,
                    await dbContext.SaveChangesAsync(acceptAllChangesOnSuccess: true));
            });

            await ExecuteUnitOfWorkAsync(async dbContext =>
            {
                Assert.Equal(
                    _zooEntities.Animals.OfType<Tiger>().Single(),
                    await dbContext.Animals.OfType<Tiger>().SingleAsync(),
                    new AnimalEqualityComparer());
            });

            await ExecuteUnitOfWorkAsync(async dbContext =>
            {
                Assert.Equal(
                    _zooEntities.Animals.OfType<PolarBear>().Single(),
                    await dbContext.Animals.OfType<PolarBear>().SingleAsync(),
                    new AnimalEqualityComparer());
            });

            await ExecuteUnitOfWorkAsync(async dbContext =>
            {
                Assert.Equal(
                    _zooEntities.Animals.OfType<SeaOtter>().Single(),
                    await dbContext.Animals.OfType<SeaOtter>().SingleAsync(),
                    new AnimalEqualityComparer());
            });

            await ExecuteUnitOfWorkAsync(async dbContext =>
            {
                Assert.Equal(
                    _zooEntities.Animals.OfType<EurasianOtter>().Single(),
                    await dbContext.Animals.OfType<EurasianOtter>().SingleAsync(),
                    new AnimalEqualityComparer());
            });

            await ExecuteUnitOfWorkAsync(async dbContext =>
            {
                IList<Otter> originalOtters = _zooEntities.Animals
                    .OfType<Otter>()
                    .OrderBy(otter => otter.Name)
                    .ToList();
                IList<Otter> queriedOtters = await dbContext.Animals
                    .OfType<Otter>()
                    .OrderBy(otter => otter.Name)
                    .ToListAsync();
                Assert.Equal(originalOtters, queriedOtters, new AnimalEqualityComparer());
            });
        }

        [Fact]
        public void CanListSync()
        {
            ExecuteUnitOfWork(dbContext =>
            {
                dbContext.Animals.AddRange(_zooEntities.Animals);
                Assert.Equal(
                    _zooEntities.Entities.Count,
                    dbContext.SaveChanges(acceptAllChangesOnSuccess: true));
            });

            ExecuteUnitOfWork(dbContext =>
            {
                IQueryable<Animal> animalQuery = dbContext.Animals
                    .OrderBy(animal => animal.Name)
                    .ThenBy(animal => animal.Height);

                Assert.Equal(_zooEntities.Animals,
                    dbContext.Animals
                        .OrderBy(animal => animal.Name)
                        .ThenBy(animal => animal.Height)
                        .ToList(),
                    new AnimalEqualityComparer());
            });
        }

        [Fact]
        public async Task CanListAsync()
        {
            await ExecuteUnitOfWorkAsync(async dbContext =>
            {
                dbContext.Animals.AddRange(_zooEntities.Animals);
                Assert.Equal(
                    _zooEntities.Entities.Count,
                    await dbContext.SaveChangesAsync(acceptAllChangesOnSuccess: true));
            });

            await ExecuteUnitOfWorkAsync(async dbContext =>
            {
                IQueryable<Animal> animalQuery = dbContext.Animals
                    .OrderBy(animal => animal.Name)
                    .ThenBy(animal => animal.Height);

                Assert.Equal(_zooEntities.Animals,
                    await dbContext.Animals
                        .OrderBy(animal => animal.Name)
                        .ThenBy(animal => animal.Height)
                        .ToListAsync(),
                    new AnimalEqualityComparer());
            });
        }

        [Fact]
        public async Task CanQueryFirstOrDefaultAsync()
        {
            await ExecuteUnitOfWorkAsync(async dbContext =>
            {
                dbContext.Animals.AddRange(_zooEntities.Animals);
                Assert.Equal(
                    _zooEntities.Entities.Count,
                    await dbContext.SaveChangesAsync(acceptAllChangesOnSuccess: true));
            });

            await ExecuteUnitOfWorkAsync(async dbContext =>
            {
                Assert.Equal(
                    _zooEntities.Animals.OfType<Tiger>().Single(),
                    await dbContext.Animals.OfType<Tiger>().FirstOrDefaultAsync(),
                    new AnimalEqualityComparer());
            });
        }

        [Fact]
        public async Task CanIncludeDirectCollection()
        {
            await ExecuteUnitOfWorkAsync(async dbContext =>
            {
                dbContext.Enclosures.AddRange(_zooEntities.Enclosures);
                Assert.Equal(
                    _zooEntities.Entities.Count,
                    await dbContext.SaveChangesAsync(acceptAllChangesOnSuccess: true));
            });

            await ExecuteUnitOfWorkAsync(async dbContext =>
            {
                IEnumerable<Enclosure> queriedEnclosures = await dbContext.Enclosures
                    .Include(enclosure => enclosure.Animals)
                    .OrderBy(enclosure => enclosure.AnimalEnclosureType)
                    .ThenBy(enclosure => enclosure.Name)
                    .ToListAsync();
                Assert.Equal(_zooEntities.Enclosures,
                    queriedEnclosures,
                    new EnclosureEqualityComparer()
                        .WithAnimalEqualityComparer(animalEqualityComparer => animalEqualityComparer
                            .WithEnclosureEqualityComparer()));
            });
        }

        [Fact]
        public async Task CanIncludeDirectReference()
        {
            await ExecuteUnitOfWorkAsync(async dbContext =>
            {
                dbContext.Animals.AddRange(_zooEntities.Animals);
                Assert.Equal(
                    _zooEntities.Entities.Count,
                    await dbContext.SaveChangesAsync(acceptAllChangesOnSuccess: true));
            });

            await ExecuteUnitOfWorkAsync(async dbContext =>
            {
                IEnumerable<Animal> queriedAnimals = await dbContext.Animals
                    .Include(animal => animal.Enclosure)
                    .OrderBy(animal => animal.Name)
                    .ThenBy(animal => animal.Age)
                    .ToListAsync();
                Assert.Equal(_zooEntities.Animals,
                    queriedAnimals,
                    new AnimalEqualityComparer()
                        .WithEnclosureEqualityComparer(enclosureEqualityComparer =>
                            enclosureEqualityComparer.WithAnimalEqualityComparer()));
            });
        }

        [Fact(Skip = "Test currently fails.")]
        public async Task CanIncludeSelfReference()
        {
            await ExecuteUnitOfWorkAsync(async dbContext =>
            {
                dbContext.AddRange(_zooEntities.Employees);
                Assert.Equal(
                    _zooEntities.Employees.Count,
                    await dbContext.SaveChangesAsync(acceptAllChangesOnSuccess: true));
            });

            await ExecuteUnitOfWorkAsync(async dbContext =>
            {
                IEnumerable<Employee> queriedEmployees = await dbContext.Employees
                    .Include(employee => employee.Manager)
                    .OrderBy(employee => employee.FullName)
                    .ToListAsync();
                Assert.Equal(_zooEntities.Employees,
                    queriedEmployees,
                    new EmployeeEqualityComparer()
                        .WithManagerComparer(managerEqualityComparer =>
                            managerEqualityComparer.WithDirectReportsComparer()));
            });
        }

        [Fact(Skip = "IncludeCompiler does not currently support DI or being independently overriden.")]
        public async Task CanIncludeOwnedCollection()
        {
            // IncludeCompiler uses the entity metadata to generate the underlying join clauses,
            // however it currently does not properly support being injected through DI, being created
            // by a factory, or being independently overriden without also having to override several
            // other query-generation-related classes. This makes it virtually impossible to generate
            // the correct MongoDb-side query syntax for supporting Join and GroupJoin statements
            // against owned collections where the ownership requires a level of indirection to get to
            // the foreign key.

            await ExecuteUnitOfWorkAsync(async dbContext =>
            {
                dbContext.Enclosures.AddRange(_zooEntities.Enclosures);
                Assert.Equal(
                    _zooEntities.Entities.Count,
                    await dbContext.SaveChangesAsync(acceptAllChangesOnSuccess: true));
            });

            await ExecuteUnitOfWorkAsync(async dbContext =>
            {
                IEnumerable<Enclosure> queriedEnclosures = await dbContext.Enclosures
                    .Include(enclosure => enclosure.Animals)
                    .Include(enclosure => enclosure.WeeklySchedule.Assignments)
                    .ThenInclude(zooAssignment => zooAssignment.Assignee)
                    .OrderBy(enclosure => enclosure.AnimalEnclosureType)
                    .ThenBy(enclosure => enclosure.Name)
                    .ToListAsync();
                Assert.Equal(_zooEntities.Enclosures,
                    queriedEnclosures,
                    new EnclosureEqualityComparer()
                        .WithAnimalEqualityComparer(animalEqualityComparer => animalEqualityComparer
                            .WithEnclosureEqualityComparer())
                        .ConfigureWeeklyScheduleEqualityComparer(
                            scheduleEqualityComparer => scheduleEqualityComparer.ConfigureZooAssignmentEqualityComparer(
                                zooAssignmentEqualityComparer => zooAssignmentEqualityComparer.WithEmployeeEqualityComparer())));
            });
        }

        [Fact]
        public async Task CanIncludeOwnedReference()
        {
            await ExecuteUnitOfWorkAsync(async dbContext =>
            {
                dbContext.Enclosures.AddRange(_zooEntities.Enclosures);
                Assert.Equal(
                    _zooEntities.Entities.Count,
                    await dbContext.SaveChangesAsync(acceptAllChangesOnSuccess: true));
            });

            await ExecuteUnitOfWorkAsync(async dbContext =>
            {
                IEnumerable<Enclosure> queriedEnclosures = await dbContext.Enclosures
                    .Include(enclosure => enclosure.Animals)
                    .Include(enclosure => enclosure.WeeklySchedule.Approver)
                    .OrderBy(enclosure => enclosure.AnimalEnclosureType)
                    .ThenBy(enclosure => enclosure.Name)
                    .ToListAsync();

                Assert.Equal(_zooEntities.Enclosures,
                    queriedEnclosures,
                    new EnclosureEqualityComparer()
                        .WithAnimalEqualityComparer(animalEqualityComparer => animalEqualityComparer
                            .WithEnclosureEqualityComparer())
                        .ConfigureWeeklyScheduleEqualityComparer(
                            scheduleEqualityComparer => scheduleEqualityComparer
                                .WithApproverEqualityComparer()));
            });
        }

        [Fact]
        public async Task CanExecuteGroupJoinWithoutIncludes()
        {
            await ExecuteUnitOfWorkAsync(async dbContext =>
            {
                dbContext.Enclosures.AddRange(_zooEntities.Enclosures);
                Assert.Equal(
                    _zooEntities.Entities.Count,
                    await dbContext.SaveChangesAsync(acceptAllChangesOnSuccess: true));
            });

            await ExecuteUnitOfWorkAsync(async dbContext =>
            {
                IEnumerable<Enclosure> queriedEnclosures = await dbContext.Enclosures
                    .GroupJoin(
                        dbContext.Employees
                            .Join(
                                dbContext.Enclosures.SelectMany(
                                    enclosure => enclosure.WeeklySchedule.Assignments,
                                    (enclosure, assignment) => new
                                    {
                                        enclosure.EnclosureId,
                                        Assignment = assignment
                                    }),
                                employee => employee.EmployeeId,
                                enclosureAssignment => enclosureAssignment.Assignment.Assignee.EmployeeId,
                                (employee, enclosureAssignment) => new
                                {
                                    enclosureAssignment.EnclosureId,
                                    Assignment = AssignAssignee(enclosureAssignment.Assignment, employee)
                                }),
                        enclosure => enclosure.EnclosureId,
                        enclosureAssignment => enclosureAssignment.EnclosureId,
                        (enclosure, enclosureAssignments) => AssignAssignments(
                            enclosure,
                            enclosureAssignments.Select(enclosureAssignment => enclosureAssignment.Assignment)))
                    .ToListAsync();
                Assert.Equal(_zooEntities.Enclosures,
                    queriedEnclosures,
                    new EnclosureEqualityComparer()
                        .ConfigureWeeklyScheduleEqualityComparer(
                            scheduleEqualityComparer => scheduleEqualityComparer
                                .ConfigureZooAssignmentEqualityComparer(
                                    zooAssignmentEqualityComparer => zooAssignmentEqualityComparer
                                        .WithEmployeeEqualityComparer())));
            });
        }

        private static Enclosure AssignAssignments(Enclosure enclosure, IEnumerable<ZooAssignment> zooAssignments)
        {
            foreach (var pair in enclosure.WeeklySchedule.Assignments
                .Join(
                    zooAssignments,
                    includedAssignment => includedAssignment.Assignee.EmployeeId,
                    denormalizedAssignment => denormalizedAssignment.Assignee.EmployeeId,
                    (denormalizedAssignment, includedAssignment) => new
                    {
                        Assignment = denormalizedAssignment,
                        includedAssignment.Assignee
                    }))
            {
                pair.Assignment.Assignee = pair.Assignee;
            };
            return enclosure;
        }

        private static ZooAssignment AssignAssignee(ZooAssignment assignment, Employee assignee)
        {
            assignment.Assignee = assignee;
            return assignment;
        }

        [Fact]
        public async Task ConcurrentQuery()
        {
            await ExecuteUnitOfWorkAsync(async dbContext =>
            {
                dbContext.Employees.AddRange(_zooEntities.Employees);
                Assert.Equal(
                    _zooEntities.Employees.Count,
                    await dbContext.SaveChangesAsync(acceptAllChangesOnSuccess: true));
            });

            Employee[] employees = await Task.WhenAll(
                ExecuteUnitOfWorkAsync(dbContext => dbContext.Employees.SingleAsync(employee => employee.FullName == _zooEntities.ManAgier.FullName)),
                ExecuteUnitOfWorkAsync(dbContext => dbContext.Employees.SingleAsync(employee => employee.FullName == _zooEntities.BearOCreary.FullName)),
                ExecuteUnitOfWorkAsync(dbContext => dbContext.Employees.SingleAsync(employee => employee.FullName == _zooEntities.OttoVonEssenmacher.FullName)),
                ExecuteUnitOfWorkAsync(dbContext => dbContext.Employees.SingleAsync(employee => employee.FullName == _zooEntities.TaigaMasuta.FullName)),
                ExecuteUnitOfWorkAsync(dbContext => dbContext.Employees.SingleAsync(employee => employee.FullName == _zooEntities.TurGuidry.FullName))
            );

            Employee[] expectedEmployees =
            {
                _zooEntities.ManAgier,
                _zooEntities.BearOCreary,
                _zooEntities.OttoVonEssenmacher,
                _zooEntities.TaigaMasuta,
                _zooEntities.TurGuidry
            };

            Assert.All(employees, Assert.NotNull);
            Assert.Equal(expectedEmployees, employees, new EmployeeEqualityComparer());
        }

        [Fact]
        public async Task CanListAsyncTwice()
        {
            await ExecuteUnitOfWorkAsync(async dbContext =>
            {
                dbContext.Animals.AddRange(_zooEntities.Animals);
                Assert.Equal(
                    _zooEntities.Entities.Count,
                    await dbContext.SaveChangesAsync(acceptAllChangesOnSuccess: true));
            });

            await ExecuteUnitOfWorkAsync(async dbContext =>
            {
                Assert.Equal(_zooEntities.Animals,
                    await dbContext.Animals
                        .OrderBy(animal => animal.Name)
                        .ThenBy(animal => animal.Height)
                        .ToListAsync(),
                    new AnimalEqualityComparer());
            });

            await ExecuteUnitOfWorkAsync(async dbContext =>
            {
                Assert.Equal(_zooEntities.Animals,
                    await dbContext.Animals
                        .OrderBy(animal => animal.Name)
                        .ThenBy(animal => animal.Height)
                        .ToListAsync(),
                    new AnimalEqualityComparer());
            });
        }

        [Fact(Skip = "This test is a performance test and take a long time to execute.")]
        public async Task CanQueryMultipleConcurrentItemsFromLargeDataSet()
        {
            IList<Employee> tigerFodderEmployees = Enumerable
                .Range(1, 100000)
                .Select(index => new Employee
                {
                    Age = 34.7M,
                    FirstName = "Fodder",
                    LastName = index.ToString()
                })
                .ToList();

            await ExecuteUnitOfWorkAsync(async dbContext =>
            {
                dbContext.Employees.AddRange(tigerFodderEmployees);
                Assert.Equal(
                    tigerFodderEmployees.Count,
                    await dbContext.SaveChangesAsync(acceptAllChangesOnSuccess: true));
            });

            Employee[] employees = await ExecuteUnitOfWorkAsync(async dbContext =>
            {
                IList<Task<Employee>> tasks = Enumerable
                    .Range(1, 100)
                    .Select(index =>
                        dbContext.Employees
                            .FirstOrDefaultAsync(e => e.LastName == (index * 1000).ToString())
                    )
                    .ToList();
                return await Task.WhenAll(tasks);
            });

            Assert.All(employees, Assert.NotNull);
        }*/
    }
}
