using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using System;
using System.Linq.Expressions;

namespace Microsoft.EntityFrameworkCore.Mongo.Query
{
    public class MongoQueryableMethodTranslatingExpressionVisitor :
        QueryableMethodTranslatingExpressionVisitor
    {
        private readonly IModel _model;
        private readonly ISqlExpressionFactory _sqlExpressionFactory;
        private readonly MongoSqlTranslatingExpressionVisitor _sqlTranslator;

        public MongoQueryableMethodTranslatingExpressionVisitor(
            QueryableMethodTranslatingExpressionVisitorDependencies dependencies,
            ISqlExpressionFactory sqlExpressionFactory,
            MongoSqlTranslatingExpressionVisitor sqlTranslator,
            IModel model)
            : base(dependencies, subquery: false)
        {
            _model = model;
            _sqlExpressionFactory = sqlExpressionFactory;
            _sqlTranslator = sqlTranslator;
        }

        protected override QueryableMethodTranslatingExpressionVisitor CreateSubqueryVisitor()
            => throw new InvalidOperationException();

        public override ShapedQueryExpression TranslateSubquery(Expression expression)
            => throw new InvalidOperationException(CoreStrings.TranslationFailed(expression.Print()));

        protected override ShapedQueryExpression CreateShapedQueryExpression(Type elementType)
        {
            var entityType = _model.FindEntityType(elementType);
            var selectExpression = _sqlExpressionFactory.Select(entityType);

            return new ShapedQueryExpression(
                selectExpression,
                new EntityShaperExpression(
                    entityType,
                    new ProjectionBindingExpression(
                        selectExpression,
                        new ProjectionMember(),
                        typeof(ValueBuffer)),
                    false));
        }

        protected override ShapedQueryExpression? TranslateAll(
            ShapedQueryExpression source,
            LambdaExpression predicate)
            => null;

        protected override ShapedQueryExpression? TranslateAny(
            ShapedQueryExpression source,
            LambdaExpression predicate)
            => null;

        protected override ShapedQueryExpression? TranslateAverage(
            ShapedQueryExpression source,
            LambdaExpression selector,
            Type resultType)
            => null;

        protected override ShapedQueryExpression? TranslateCast(
            ShapedQueryExpression source,
            Type resultType)
            => null;

        protected override ShapedQueryExpression? TranslateConcat(
            ShapedQueryExpression source1,
            ShapedQueryExpression source2)
            => null;

        protected override ShapedQueryExpression? TranslateContains(
            ShapedQueryExpression source,
            Expression item)
            => null;

        protected override ShapedQueryExpression? TranslateCount(
            ShapedQueryExpression source,
            LambdaExpression predicate)
            => null;

        protected override ShapedQueryExpression? TranslateDefaultIfEmpty(
            ShapedQueryExpression source,
            Expression defaultValue)
            => null;

        protected override ShapedQueryExpression? TranslateDistinct(
            ShapedQueryExpression source)
            => null;

        protected override ShapedQueryExpression? TranslateElementAtOrDefault(
            ShapedQueryExpression source,
            Expression index,
            bool returnDefault)
            => null;

        protected override ShapedQueryExpression? TranslateExcept(
            ShapedQueryExpression source1,
            ShapedQueryExpression source2)
            => null;

        protected override ShapedQueryExpression? TranslateFirstOrDefault(
            ShapedQueryExpression source,
            LambdaExpression predicate,
            Type returnType,
            bool returnDefault)
            => null;

        protected override ShapedQueryExpression? TranslateGroupBy(
            ShapedQueryExpression source,
            LambdaExpression keySelector,
            LambdaExpression elementSelector,
            LambdaExpression resultSelector)
            => null;

        protected override ShapedQueryExpression? TranslateGroupJoin(
            ShapedQueryExpression outer,
            ShapedQueryExpression inner,
            LambdaExpression outerKeySelector,
            LambdaExpression innerKeySelector,
            LambdaExpression resultSelector)
            => null;

        protected override ShapedQueryExpression? TranslateIntersect(
            ShapedQueryExpression source1,
            ShapedQueryExpression source2)
            => null;

        protected override ShapedQueryExpression? TranslateJoin(
            ShapedQueryExpression outer,
            ShapedQueryExpression inner,
            LambdaExpression outerKeySelector,
            LambdaExpression innerKeySelector,
            LambdaExpression resultSelector)
            => null;

        protected override ShapedQueryExpression? TranslateLeftJoin(
            ShapedQueryExpression outer,
            ShapedQueryExpression inner,
            LambdaExpression outerKeySelector,
            LambdaExpression innerKeySelector,
            LambdaExpression resultSelector)
            => null;

        protected override ShapedQueryExpression? TranslateLastOrDefault(
            ShapedQueryExpression source,
            LambdaExpression predicate,
            Type returnType,
            bool returnDefault)
            => null;

        protected override ShapedQueryExpression? TranslateLongCount(
            ShapedQueryExpression source,
            LambdaExpression predicate)
            => null;

        protected override ShapedQueryExpression? TranslateMax(
            ShapedQueryExpression source,
            LambdaExpression selector,
            Type resultType)
            => null;

        protected override ShapedQueryExpression? TranslateMin(
            ShapedQueryExpression source,
            LambdaExpression selector,
            Type resultType)
            => null;

        protected override ShapedQueryExpression? TranslateOfType(
            ShapedQueryExpression source,
            Type resultType)
            => null;

        protected override ShapedQueryExpression? TranslateOrderBy(
            ShapedQueryExpression source,
            LambdaExpression keySelector,
            bool ascending)
            => null;

        protected override ShapedQueryExpression? TranslateReverse(
            ShapedQueryExpression source)
            => null;

        protected override ShapedQueryExpression? TranslateSelect(
            ShapedQueryExpression source,
            LambdaExpression selector)
            => null;

        protected override ShapedQueryExpression? TranslateSelectMany(
            ShapedQueryExpression source,
            LambdaExpression collectionSelector,
            LambdaExpression resultSelector)
            => null;

        protected override ShapedQueryExpression? TranslateSelectMany(
            ShapedQueryExpression source,
            LambdaExpression selector)
            => null;

        protected override ShapedQueryExpression? TranslateSingleOrDefault(
            ShapedQueryExpression source,
            LambdaExpression predicate,
            Type returnType,
            bool returnDefault)
        {
            if (predicate != null)
            {
                source = TranslateWhere(source, predicate);
                if (source == null)
                {
                    return null;
                }
            }

            var selectExpression = (SelectExpression)source.QueryExpression;
            selectExpression.ApplyLimit(TranslateExpression(Expression.Constant(2)));

            if (source.ShaperExpression.Type != returnType)
            {
                source.ShaperExpression = Expression.Convert(source.ShaperExpression, returnType);
            }

            return source;
        }

        private SqlExpression TranslateExpression(Expression expression)
            => _sqlTranslator.Translate(expression);

        protected override ShapedQueryExpression? TranslateSkip(
            ShapedQueryExpression source,
            Expression count)
            => null;

        protected override ShapedQueryExpression? TranslateSkipWhile(
            ShapedQueryExpression source,
            LambdaExpression predicate)
            => null;

        protected override ShapedQueryExpression? TranslateSum(
            ShapedQueryExpression source,
            LambdaExpression selector,
            Type resultType)
            => null;

        protected override ShapedQueryExpression? TranslateTake(
            ShapedQueryExpression source,
            Expression count)
            => null;

        protected override ShapedQueryExpression? TranslateTakeWhile(
            ShapedQueryExpression source,
            LambdaExpression predicate)
            => null;

        protected override ShapedQueryExpression? TranslateThenBy(
            ShapedQueryExpression source,
            LambdaExpression keySelector,
            bool ascending)
            => null;

        protected override ShapedQueryExpression? TranslateUnion(
            ShapedQueryExpression source1,
            ShapedQueryExpression source2)
            => null;

        protected override ShapedQueryExpression? TranslateWhere(
            ShapedQueryExpression source,
            LambdaExpression predicate)
            => null;
    }
}
