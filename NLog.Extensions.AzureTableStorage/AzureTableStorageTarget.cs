using System;
using System.ComponentModel.DataAnnotations;
using NLog.Targets;
using NLog.Layouts;

namespace NLog.Extensions.AzureTableStorage
{
    [Target("AzureTableStorage")]
    public class AzureTableStorageTarget : TargetWithLayout
    {
        private ConfigManager _configManager;
        private TableStorageManager _tableStorageManager;

        [Required]
        public string ConnectionStringKey { get; set; }

        [Required]
        public SimpleLayout TableName { get; set; }

        public string PartitionKeyPrefix { get; set; }
        public string PartitionKeyPrefixKey { get; set; }
        public string PartitionKeyPrefixDateFormat { get; set; }

        public string LogTimestampFormatString { get; set; }

        protected override void InitializeTarget()
        {
            base.InitializeTarget();
            _configManager = new ConfigManager(ConnectionStringKey);
            _tableStorageManager = new TableStorageManager(_configManager);

            // use PartitionKeyPrefixKey if present
            if (!string.IsNullOrWhiteSpace(PartitionKeyPrefixKey))
            {
                PartitionKeyPrefix = _configManager.GetSettingByKey(PartitionKeyPrefixKey);
            }
            // else use PartitionKeyPrefixDateFormat if available
            else if (!string.IsNullOrWhiteSpace(PartitionKeyPrefixDateFormat))
            {
                PartitionKeyPrefix = DateTime.UtcNow.ToString(PartitionKeyPrefixDateFormat);
            }

            if (TableName.IsFixedText)
            {
                _tableStorageManager.ValidateNameForTableStorage(TableName.Text);
            }
        }

        protected override void Write(LogEventInfo logEvent)
        {
            if (_tableStorageManager != null)
            {
                var layoutMessage = Layout.Render(logEvent);
                var tableName = TableName.Render(logEvent);
                if (string.IsNullOrEmpty(LogTimestampFormatString))
                {
                    _tableStorageManager.Add(new LogEntity(PartitionKeyPrefix, logEvent, layoutMessage), tableName);
                }
                else
                {
                    _tableStorageManager.Add(new LogEntity(PartitionKeyPrefix, logEvent, layoutMessage, LogTimestampFormatString), tableName);
                }
            }
        }
    }
}
