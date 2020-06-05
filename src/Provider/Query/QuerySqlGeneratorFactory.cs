namespace Microsoft.EntityFrameworkCore.Mongo.Query
{
    public class QuerySqlGeneratorFactory : IQuerySqlGeneratorFactory
    {
        public QuerySqlGenerator Create()
        {
            return new QuerySqlGenerator();
        }
    }
}
