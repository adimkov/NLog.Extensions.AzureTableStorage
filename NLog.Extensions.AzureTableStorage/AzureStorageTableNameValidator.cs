using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace NLog.Extensions.AzureTableStorage
{
    //validation rules described in: http://msdn.microsoft.com/en-us/library/windowsazure/dd179338.aspx
    public class AzureStorageTableNameValidator
    {
        private const string TableNameValidationRegularExpression = @"^[A-Za-z][A-Za-z0-9]{2,62}$";
        private const string TableNameFixRegularExpression = @"(?<invalid>[^a-zA-Z0-9])";

        private readonly List<string> _reservedWords;

        public AzureStorageTableNameValidator()
        {
            _reservedWords = new List<string> { "tables" };
        }

        public bool IsValid(string tableName)
        {
            return !_reservedWords.Contains(tableName) 
                && Regex.IsMatch(tableName, TableNameValidationRegularExpression);
        }

        internal bool IsValid()
        {
            throw new NotImplementedException();
        }

        public string PrepareTableName(string tableName)
        {
            if (_reservedWords.Contains(tableName))
            {
                return new string(tableName.Reverse().ToArray());
            }

            var nameCandidat = Regex.Replace(tableName, TableNameFixRegularExpression, "");
            if (nameCandidat.Length <= 62)
            {
                nameCandidat = nameCandidat.Substring(0, 62);
            }

            return nameCandidat;
        }
    }
}
