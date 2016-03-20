using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Table;
using NLog.Layouts;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NLog.Extensions.AzureTableStorage
{
    public class TableStorageManager
    {
        private readonly AzureStorageTableNameValidator _validator;
        private readonly CloudStorageAccount _storageAccount;
        private readonly Dictionary<string, CloudTable> _cloudTables = new Dictionary<string, CloudTable>();

        public TableStorageManager(ConfigManager configManager)
        {
            _storageAccount = configManager.GetStorageAccount();
            _validator = new AzureStorageTableNameValidator();
        }

        public void ValidateNameForTableStorage(string tableName)
        {
            if (!_validator.IsValid())
            {
                throw new NotSupportedException(tableName + " is not a valid name for Azure storage table name.")
                {
                    HelpLink = "http://msdn.microsoft.com/en-us/library/windowsazure/dd179338.aspx"
                };
            }
        }

        public void Add(LogEntity entity, string tableName)
        {
            var table = GetCloudTable(tableName);
            var insertOperation = TableOperation.Insert(entity);
            table.Execute(insertOperation);
        }

        public void Add(IEnumerable<LogEntity> entities, string tableName)
        {
            var table = GetCloudTable(tableName);
            foreach(var entriesGroup in entities.GroupBy(x => x.PartitionKey))
            {
                var butch = new TableBatchOperation();

                foreach (var entry in entriesGroup)
                {
                    // todo: also could be problem with inserting more than 1000 items in single butch
                    butch.Add(TableOperation.Insert(entry));
                }

                if (butch.Any())
                {
                    table.ExecuteBatch(butch);
                }
            }
        }

        private CloudTable GetCloudTable(string tableName)
        {
            if (_cloudTables.ContainsKey(tableName))
            {
                return _cloudTables[tableName];
            }
            else
            {
                var tableClient = _storageAccount.CreateCloudTableClient();
                CloudTable table;
                var cloudName = tableName;
                if (!_validator.IsValid(cloudName))
                {
                    cloudName = _validator.PrepareTableName(cloudName);
                };

                table = tableClient.GetTableReference(cloudName);
                table.CreateIfNotExists();
                _cloudTables.Add(tableName, table);

                return table;
            }
        }
    }
}
