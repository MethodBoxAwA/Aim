using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.Linq;
using System.Text;
using static StonePlanner.Interfaces;

namespace StonePlanner
{
    /// <summary>
    /// Provide methods for serialization or deserialization objects of Access database
    /// </summary>
    internal class AccessEntity
    {
        private static AccessEntity _accessEntityInstance;
        static readonly object _threadLock = new();
        private List<string> _ignoredPropertyList;

        private OleDbConnection _dbConnection;

        private AccessEntity(string fileName, string password, List<string> ignoredPropertyList = null)
        {
            // Connect and open database
            string stringConnection = $@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source={fileName};Jet OLEDB:Database Password={password}";
            // Build ignored property list
            if (ignoredPropertyList is not null)
            {
                _ignoredPropertyList = ignoredPropertyList;
            }
            _dbConnection = new OleDbConnection(stringConnection);
            _dbConnection.Open();
        }

        /// <summary>
        /// Get singleton instance of access entity object
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        internal static AccessEntity GetAccessEntityInstance(string fileName = null, string password = null, List<string> ignoredPropertyList = null)
        {
            if (_accessEntityInstance is null)
            {
                lock (_threadLock)
                {
                    _accessEntityInstance ??= new AccessEntity(fileName, password, ignoredPropertyList);
                }
            }
            return _accessEntityInstance;
        }

        /// <summary>
        /// Insert object instance to database
        /// </summary>
        /// <typeparam name="T">Type of data source</typeparam>
        /// <param name="instance">Object instance</param>
        /// <param name="tableName">Name of data table</param>
        /// <returns>The number of rows affected</returns>
        internal int AddElement<T>(T instance, string tableName, List<string> ignoredPropertyList = null)
        {
            var instanceType = typeof(T);
            var properties = instanceType.GetProperties();
            var insertStringBuilderName = new StringBuilder();
            var insertStringBuilderValue = new StringBuilder();

            _ignoredPropertyList = ignoredPropertyList;
            insertStringBuilderName.Append($"INSERT INTO {tableName}(");

            for (int i = 0; i < properties.Length; i++)
            {
                // Ignored
                if (_ignoredPropertyList is not null)
                {
                    if (_ignoredPropertyList.Contains(properties[i].Name))
                    {
                        continue;
                    }
                }

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

            insertStringBuilderName.Remove(insertStringBuilderName.Length - 1, 1);
            insertStringBuilderName.Append(")");
            insertStringBuilderName.Append(" VALUES (");
            insertStringBuilderName.Append(insertStringBuilderValue);

            // Insert object to database
            var insertCommand = _dbConnection.CreateCommand();
            insertStringBuilderName.Remove(insertStringBuilderName.Length - 1, 1);
            insertStringBuilderName.Append(")");
            insertCommand.CommandText = insertStringBuilderName.ToString();

            return insertCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// Remove specific object from database when the value of propertyName equals propertyValue
        /// </summary>
        /// <typeparam name="R">Type of mapping table</typeparam>
        /// <param name="mappingTable">Provide mapping from database row headers to field names</param>
        /// <param name="propertyName">Name of specific property</param>
        /// <param name="propertyValue">Value of specific property</param>
        /// <returns>The number of rows affected</returns>
        internal int RemoveElement<R>(R mappingTable, string propertyName,
            string propertyValue, string tableName) where R : IMappingTable
        {
            string databaseColumnName;
            if (mappingTable.GetType() == typeof(NonMappingTable)) databaseColumnName = propertyName;
            else databaseColumnName = mappingTable.GetColumnName(propertyName);
            var removeSring = $"delete from {tableName} where {databaseColumnName} = {propertyValue}";
            var removeCommand = _dbConnection.CreateCommand();
            removeCommand.CommandText = removeSring;
            return removeCommand.ExecuteNonQuery();
        }

        /// <summary>
        /// Get specific object instance from database
        /// </summary>
        /// <typeparam name="T">Type of data source</typeparam>
        /// <typeparam name="R">Type of mapping table</typeparam>
        /// <param name="mappingTable">Instance of mapping table</param>
        /// <param name="tableName">Name of data table</param>
        /// <param name="propertyName">Name of specific property</param>
        /// <param name="propertyValue">Value of specific property</param>
        /// <param name="isString">Whether use string pattern</param>
        /// <param name="tableFirst">Whether search from table first</param>
        /// <param name="ignoredPropertyList">Ignored property</param>
        /// <returns>Specific object instance list</returns>
        internal List<T> GetElement<T, R>(R mappingTable, string tableName,
            string propertyName, string propertyValue, bool isString = false, 
            bool tableFirst = true, List<string> ignoredPropertyList = null)
        where R : IMappingTable
        {
            string databaseColumnName;
            this._ignoredPropertyList = ignoredPropertyList;
            List<T> result = new List<T>();

            if (mappingTable.GetType() == typeof(NonMappingTable)) databaseColumnName = propertyName;
            else databaseColumnName = mappingTable.GetColumnName(propertyName);

            if (tableFirst)
            {
                string searchSring;
                if (!isString)
                    searchSring = $"select * from {tableName} where {databaseColumnName} = {propertyValue}";
                else
                    searchSring = $"select * from {tableName} where {databaseColumnName} = '{propertyValue}'";
                var removeCommand = _dbConnection.CreateCommand();
                removeCommand.CommandText = searchSring;
                var reader = removeCommand.ExecuteReader();

                while (reader.Read())
                {
                    T instance = (T) Activator.CreateInstance(typeof(T));

                    for (int i = 0; i < reader.FieldCount; i++)
                    {
                        string fromColumnName = reader.GetName(i);
                        string fromPropertyName;

                        if (mappingTable.GetType() == typeof(NonMappingTable))
                            fromPropertyName = fromColumnName;
                        else
                            fromPropertyName = mappingTable.GetPropertyName(fromColumnName);

                        if (ignoredPropertyList is not null)
                            if (ignoredPropertyList.Contains(fromPropertyName))
                                continue;
                        
                        var propertyInfo = instance.GetType().GetProperty(fromPropertyName);
                        propertyInfo.SetValue(instance, reader[i]);
                    }
                    result.Add(instance);
                }
            }
            else
            {
                List<T> allItems = GetElements<T, R>(tableName, mappingTable);
                result = (List<T>) allItems.Where(obj =>
                {
                    var type = typeof(T);
                    var propertyInfo = type.GetProperty(propertyName);
                    return Convert.ToString(propertyInfo.GetValue(obj)) == propertyValue;
                });
            }

            return result;
        }

        /// <summary>
        /// Get all object instance from database
        /// </summary>
        /// <typeparam name="T">Type of data source</typeparam>
        /// <typeparam name="R">Type of mapping table</typeparam>
        /// <param name="tableName">Name of data table</param>
        /// <param name="mappingTable">Instance of mapping table</param>
        /// <param name="ignoredPropertyList">Ignored property</param>
        /// <returns>Object instance list</returns>
        internal List<T> GetElements<T, R>(string tableName, R mappingTable, List<string> ignoredPropertyList = null)
            where R : IMappingTable
        {
            // Get all name of column
            var queryCommand = _dbConnection.CreateCommand();
            queryCommand.CommandText = $"SELECT * FROM {tableName}";
            var reader = queryCommand.ExecuteReader();

            // Re write
            _ignoredPropertyList = ignoredPropertyList;

            // Read and construct object
            var objectType = typeof(T);
            List<T> result = new();

            while (reader.Read())
            {
                T temp = (T) Activator.CreateInstance(objectType);

                // Ignored
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    if (_ignoredPropertyList is not null)
                    {
                        if (_ignoredPropertyList.Contains(reader.GetName(i)))
                        {
                            continue;
                        }
                    }

                    System.Reflection.PropertyInfo property;

                    if (mappingTable.GetType() == typeof(NonMappingTable))
                        property = objectType.GetProperty(reader.GetName(i));
                    else
                        property = objectType.GetProperty(mappingTable.GetPropertyName(reader.GetName(i)));

                    property.SetValue(temp, reader[i]);
                }

                result.Add(temp);
            }
            return result;
        }


        /// <summary>
        /// Set new value to specific object of database
        /// </summary>
        /// <typeparam name="T">Type of data source</typeparam>
        /// <typeparam name="R">Type of mapping table</typeparam>
        /// <inheritdoc cref="RemoveElement{R}(R, string, string, string)"/>
        /// <param name="element">Object that want to change</param>
        /// <param name="tableName">Name of data table</param>
        /// <param name="ignoredPropertyList">Ignored property</param>
        internal int UpdateElement<T, R>(T element, R mappingTable,string propertyName, 
            string tableName, List<string> ignoredPropertyList = null) 
            where R : IMappingTable
        {
            string databaseColumnName;
            _ignoredPropertyList = ignoredPropertyList;
            if (mappingTable.GetType() == typeof(NonMappingTable)) databaseColumnName = propertyName;
            else databaseColumnName = mappingTable.GetColumnName(propertyName);
            var type = typeof(T);

            var setBuilder = new StringBuilder();
            var properties = type.GetProperties();
            setBuilder.Append($"update {tableName} ");
            setBuilder.Append("set ");

            foreach (var property in properties)
            {
                // Ignored
                if (_ignoredPropertyList is null)
                    _ignoredPropertyList = new List<string>();

                // Do not update ID
                _ignoredPropertyList.Add("ID");
                if (_ignoredPropertyList!.Contains(property.Name))
                {
                    continue;
                }

                string inlineName;
                if (mappingTable.GetType() == typeof(NonMappingTable)) inlineName = property.Name;
                else inlineName = mappingTable.GetColumnName(propertyName);
                if (property.PropertyType == typeof(string))
                    setBuilder.Append($"{inlineName} = '{property.GetValue(element)}', ");
                else
                    setBuilder.Append($"{inlineName} = {property.GetValue(element)}, ");
            }
            setBuilder.Remove(setBuilder.Length - 2, 2);

            var setType = type.GetProperty(propertyName).PropertyType;

            if (setType == typeof(string))
                setBuilder.Append($" where {databaseColumnName} = '{type.GetProperty(databaseColumnName).GetValue(element)}'");
            else
                setBuilder.Append($" where {databaseColumnName} = {type.GetProperty(databaseColumnName).GetValue(element)}");
            var commandLine = _dbConnection.CreateCommand();
            commandLine.CommandText = setBuilder.ToString();
            _ignoredPropertyList.Remove("ID");
            return commandLine.ExecuteNonQuery();
        }
    }

    /// <summary>
    /// Indicates that no mapping table is required
    /// </summary>
    internal class NonMappingTable : IMappingTable
    {
        public string GetPropertyName(string _) => null;
        public string GetColumnName(string _) => null;
    }
}