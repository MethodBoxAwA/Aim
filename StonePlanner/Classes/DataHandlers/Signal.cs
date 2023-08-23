using System;
using StonePlanner.Classes.DataTypes;
using static StonePlanner.Classes.DataTypes.Interfaces;

namespace StonePlanner.Classes.DataHandlers
{
    internal class Signal : ISignal
    { 
        public int Value
        {
            get => Value;
            set 
            {
                DataType.SignChangedEventArgs e = new DataType.SignChangedEventArgs
                {
                    Sign = value
                };
                SignChanged?.Invoke(null,e);
            }
        }
        public bool AddSignal(int signal) 
        {
            Value = signal;
            return true;
        }
        public event Action<object,DataType.SignChangedEventArgs> SignChanged;
    }
}
