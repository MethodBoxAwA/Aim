using System;
using System.Reflection;

namespace StonePlanner
{
    internal class Interfaces
    {
        internal interface ISignal
        {
            int Value { get; set; }

            bool AddSignal(int Sign);
        }

        internal interface IVersion
        {
            string Version { get; set; }
            int Number { get; set; }
            Uri DownloadUri { get; set; }

            string GetVersion();
            Uri GetUri();
            bool IsNeedUpdate(int equNum);
        }

        internal interface IManager<T>
        {
            void Change(T delta);
            T GetValue();
        }
        
        internal interface IMappingTable
        {
            string GetPropertyName(string columnName);
            string GetColumnName(string propertyName);
        }

        internal interface IDBEntity
        {
            int ID { get; set; }
        }
    }
}
