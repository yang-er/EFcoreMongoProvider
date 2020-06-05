using MongoDB.Bson;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Microsoft.EntityFrameworkCore.Mongo.Tests
{
    public class MongoContext : DbContext
    {
        public DbSet<Family> Families { get; set; }
        public DbSet<Company> Companies { get; set; }

        public MongoContext(DbContextOptions<MongoContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Family>(entity =>
            {
                entity.HasKey(e => e.Id);

                entity.OwnsMany(e => e.Parents);

                entity.OwnsMany(e => e.Children, owned =>
                {
                    owned.OwnsMany(e => e.Pets);
                });

                entity.OwnsOne(e => e.Address);
            });

            modelBuilder.Entity<Company>(entity =>
            {
                entity.HasKey(e => e.Id);
            });
        }
    }

    public class Company
    {
        [Key]
        public ObjectId Id { get; set; }
        public string Name { get; set; }
        public bool Valid { get; set; }
    }

    [Owned]
    public class Address
    {
        public string State { get; set; }
        public string County { get; set; }
        public string City { get; set; }
    }

    [Owned]
    public class Child
    {
        public string FamilyName { get; set; }
        public string FirstName { get; set; }
        public string Gender { get; set; }
        public int Grade { get; set; }
        public ICollection<Pet> Pets { get; set; }
    }

    public class Family
    {
        [Key]
        public ObjectId Id { get; set; }
        public string LastName { get; set; }
        public ICollection<Parent> Parents { get; set; }
        public ICollection<Child> Children { get; set; }
        public Address Address { get; set; }
        public bool IsRegistered { get; set; }
    }

    [Owned]
    public class Parent
    {
        public string FamilyName { get; set; }
        public string FirstName { get; set; }
    }

    [Owned]
    public class Pet
    {
        public string GivenName { get; set; }
    }
}
