using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Text;
using static StonePlanner.Interfaces;

namespace StonePlanner
{
    internal class AccessEntity
    {
        private static AccessEntity _accessEntityInstance;
        static readonly object _threadLock = new();

        private OleDbConnection _dbConnection;

        private AccessEntity(string fileName, string password)
        {
            // Connect and open database
            string stringConnection = $@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={fileName};Jet OLEDB:Database Password={password}";
            _dbConnection = new OleDbConnection(stringConnection);
            _dbConnection.Open();
        }

        internal static AccessEntity GetAccessEntityInstance(string fileName, string password)
        {
            if (_accessEntityInstance is null)
            {
                lock (_threadLock)
                {
                    _accessEntityInstance ??= new AccessEntity(fileName, password);
                }
            }
            return _accessEntityInstance;
        }

        internal int AddElement<T>(T instance, string tableName)
        {
            var instanceType = typeof(T);
            var properties = instanceType.GetProperties();
            var insertStringBuilderName = new StringBuilder();
            var insertStringBuilderValue = new StringBuilder();
            
            insertStringBuilderName.Append($"INSERT INTO {tableName}(");

            for (int i = 0; i < properties.Length; i++)
            {
                if (i == properties.Length - 1)
                {
                    insertStringBuilderName.Append($"{properties[i].Name})");

                    if (properties[i].PropertyType == typeof(string))
                    {
                        insertStringBuilderValue.Append($"'{properties[i].GetValue(instance)}')");
                    }
                    else
                    {
                        insertStringBuilderValue.Append($"{properties[i].GetValue(instance)})");
                    }
                }

                else
                {
                    insertStringBuilderName.Append($"{properties[i].Name},");

                    if (properties[i].PropertyType == typeof(string))
                    {
                        insertStringBuilderValue.Append($"'{properties[i].GetValue(instance)}',");
                    }
                    else
                    {
                        insertStringBuilderValue.Append($"{properties[i].GetValue(instance)},");
                    }
                }
            }

            insertStringBuilderName.Append(" VALUES (");
            insertStringBuilderName.Append(insertStringBuilderValue);

            // Insert object to database
            var insertCommand = _dbConnection.CreateCommand();
            insertCommand.CommandText = insertStringBuilderName.ToString();
            return insertCommand.ExecuteNonQuery();
        }

        internal IEnumerable<T> GetElement<T,R>(R mappingTable, string tableName) 
            where R:IMappingTable
        {
            // Get all name of column
            var queryCommand = _dbConnection.CreateCommand();
            queryCommand.CommandText = $"SELECT * FROM {tableName}";
            var reader = queryCommand.ExecuteReader();

            // Read and construct object
            var objectType = typeof(T);
            List<T> result = new();

            while (reader.Read())
            {
                T temp = (T)Activator.CreateInstance(objectType);

                for (int i = 0; i < reader.FieldCount; i++)
                {
                    var field = objectType.GetField(mappingTable.GetPropertyName(reader.GetName(i)));
                    field.SetValue(temp, reader[i]);
                }

                result.Add(temp);
            }

            return result;
        }
    }
}
