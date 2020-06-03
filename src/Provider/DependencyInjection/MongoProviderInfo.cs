using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Utilities;
using System.Collections.Generic;

namespace Microsoft.EntityFrameworkCore.Mongo.DependencyInjection
{
    public class MongoProviderInfo : DbContextOptionsExtensionInfo
    {
        public MongoProvider Provider { get; }

        public override bool IsDatabaseProvider => true;

        public override string LogFragment => "MongoDb";

        public override long GetServiceProviderHashCode()
            => (Provider.ConnectString.GetHashCode() * 327)
            ^ Provider.DatabaseName.GetHashCode();

        public override void PopulateDebugInfo(IDictionary<string, string> debugInfo)
        {
            debugInfo.Add("MongoDb:ConnectString", Provider.ConnectString);
            debugInfo.Add("MongoDb:DatabaseName", Provider.DatabaseName);
        }

        public MongoProviderInfo(MongoProvider extension) :
            base(Check.NotNull(extension, nameof(extension)))
        {
            Provider = extension;
        }
    }
}
