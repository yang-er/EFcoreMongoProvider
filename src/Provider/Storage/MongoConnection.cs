using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Internal;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Mongo.DependencyInjection;
using Microsoft.EntityFrameworkCore.Mongo.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Utilities;
using Microsoft.Extensions.Logging;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Core.Events;
using System;
using System.Diagnostics;

namespace Microsoft.EntityFrameworkCore.Mongo.Storage
{
    public class MongoConnection : IMongoConnection
    {
        public MongoProvider Provider { get; }

        public MongoClientSettings Settings { get; }

        public IMongoClient Client { get; }

        public IMongoDatabase Database { get; }

        public IModel Model { get; }

        public IDiagnosticsLogger<DbLoggerCategory.Database.Command> Logger { get; }

        public string Name { get; }

        public MongoConnection(
            IDbContextOptions options,
            IModel model,
            IDiagnosticsLogger<DbLoggerCategory.Database.Command> logger)
        {
            Provider = Check.NotNull(options.FindExtension<MongoProvider>(), nameof(Provider));
            Settings = Provider.Settings.Clone();
            Logger = logger;
            Client = new MongoClient(Settings);
            Name = Provider.DatabaseName;
            Database = Client.GetDatabase(Name);
            Model = model;

            Settings.ClusterConfigurator = builder => builder
                .Subscribe<CommandStartedEvent>(LogCommandExecution);
        }

        public virtual IMongoCollection<TEntity> GetCollection<TEntity>()
        {
            var collectionEntityType = Model
                .FindEntityType(typeof(TEntity))
                .GetCollectionEntityType();

            var annotations = collectionEntityType.MongoDb();

            return Database.GetCollection<TEntity>(annotations.CollectionName, annotations.CollectionSettings);
        }

        public static readonly EventId DocumentQueryCommand = new EventId(
            CoreEventId.CoreBaseId * 100 + 1000,
            $"{DbLoggerCategory.Database.Command.Name}.{CoreEventId.CoreBaseId * 100 + 1000}");

        /// <summary>
        /// Executing document query command: {documentQuery}
        /// </summary>
        public static readonly EventDefinition<string> LogDocumentQueryCommand
            = new EventDefinition<string>(
                new LoggingOptions(),
                DocumentQueryCommand,
                LogLevel.Debug,
                "DocumentEventId.DocumentQueryCommand",
                _ => LoggerMessage.Define<string>(
                    LogLevel.Debug, DocumentQueryCommand,
                    "Executing document query command: {documentQuery}"));

        private void LogCommandExecution(CommandStartedEvent commandStartedEvent)
        {
            if (commandStartedEvent.CommandName == "aggregate")
            {
                var documentQuery = commandStartedEvent.Command.ToJson();
                var definition = LogDocumentQueryCommand;

                var warningBehavior = definition.GetLogBehavior(Logger);
                if (warningBehavior != WarningBehavior.Ignore)
                {
                    definition.Log(
                        Logger,
                        warningBehavior,
                        documentQuery);
                }

                if (Logger.DiagnosticSource.IsEnabled(definition.EventId.Name))
                {
                    Logger.DiagnosticSource.Write(
                        definition.EventId.Name,
                        new DocumentQueryEvent(
                            definition,
                            DocumentQueryCommand,
                            documentQuery));
                }

                static string DocumentQueryCommand(EventDefinitionBase definition, EventData payload)
                {
                    var eventDefinition = (EventDefinition<string>)definition;
                    var documentQueryEvent = (DocumentQueryEvent)payload;
                    return eventDefinition.GenerateMessage(documentQueryEvent.DocumentQuery);
                }

                LogDocumentQueryCommand.Log(Logger, WarningBehavior.Ignore, documentQuery);
            }
        }

        /// <inheritdoc />
        /// <summary>
        ///   A <see cref="DiagnosticSource" /> event payload class for query command execution.
        /// </summary>
        private sealed class DocumentQueryEvent : EventData
        {
            /// <inheritdoc />
            /// <summary>
            /// Initializes a new instance of the <see cref="DocumentQueryEvent"/> class.
            /// </summary>
            /// <param name="eventDefinition"> The event definition. </param>
            /// <param name="messageGenerator"> A delegate that generates a log message for this event. </param>
            /// <param name="documentQuery">A textual representation of the document query.</param>
            public DocumentQueryEvent(
                EventDefinitionBase eventDefinition,
                Func<EventDefinitionBase, EventData, string> messageGenerator,
                string documentQuery)
                : base(eventDefinition, messageGenerator)
            {
                DocumentQuery = Check.NotEmpty(documentQuery, nameof(documentQuery));
            }

            /// <summary>
            ///    A textual representation of the document query.
            /// </summary>
            public string DocumentQuery { get; }
        }
    }
}
