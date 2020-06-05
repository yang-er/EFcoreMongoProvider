using Microsoft.EntityFrameworkCore.Mongo.Adapter;
using MongoDB.Bson;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Microsoft.EntityFrameworkCore.Mongo.Tests
{
    public class MongoEntityFixture
    {
        static MongoEntityFixture()
        {
            EntityFrameworkCoreConventionPack.Register(type => true);
        }

        public Family OnlyFamily { get; } = new Family
        {
            Id = ObjectId.GenerateNewId(),
            LastName = "Andersen",

            Parents = new List<Parent>
            {
                new Parent { FirstName = "Thomas" },
                new Parent { FirstName = "Mary Kay" }
            },

            Children = new Collection<Child>
            {
                new Child
                {
                    FirstName = "Henriette Thaulow",
                    Gender = "female",
                    Grade = 5,
                    Pets = new HashSet<Pet>
                    {
                        new Pet { GivenName = "Fluffy" }
                    }
                }
            },
            
            Address = new Address { State = "WA", County = "King", City = "Seattle" },
            IsRegistered = false
        };
    }
}
