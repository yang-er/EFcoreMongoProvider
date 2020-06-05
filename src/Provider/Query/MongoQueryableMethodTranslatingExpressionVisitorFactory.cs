using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using System;
using System.Collections.Generic;
using System.Text;

namespace Microsoft.EntityFrameworkCore.Mongo.Query
{
    public class MongoQueryableMethodTranslatingExpressionVisitorFactory :
        IQueryableMethodTranslatingExpressionVisitorFactory
    {
        private readonly QueryableMethodTranslatingExpressionVisitorDependencies _dependencies;
        private readonly ISqlExpressionFactory _sqlExpressionFactory;
        private readonly MongoSqlTranslatingExpressionVisitor _sqlTranslator;


        public MongoQueryableMethodTranslatingExpressionVisitorFactory(
            QueryableMethodTranslatingExpressionVisitorDependencies dependencies,
            ISqlExpressionFactory sqlExpressionFactory,
            IMemberTranslatorProvider memberTranslatorProvider,
            IMethodCallTranslatorProvider methodCallTranslatorProvider,
            IModel model
            )//ISqlExpressionFactory sqlExpressionFactory,
            //MongoSqlTranslatingExpressionVisitor sqlTranslator)
        {
            _dependencies = dependencies;
            _sqlExpressionFactory = sqlExpressionFactory;
            _sqlTranslator = new MongoSqlTranslatingExpressionVisitor(model, sqlExpressionFactory, memberTranslatorProvider, methodCallTranslatorProvider);
            //_sqlTranslator = sqlTranslator;
        }

        public virtual QueryableMethodTranslatingExpressionVisitor Create(IModel model)
            => new MongoQueryableMethodTranslatingExpressionVisitor(
                _dependencies,
                _sqlExpressionFactory,
                _sqlTranslator,
                model);
    }
}
