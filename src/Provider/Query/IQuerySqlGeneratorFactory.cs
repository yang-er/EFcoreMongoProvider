namespace Microsoft.EntityFrameworkCore.Mongo.Query
{
    public interface IQuerySqlGeneratorFactory
    {
        QuerySqlGenerator Create();
    }
}
