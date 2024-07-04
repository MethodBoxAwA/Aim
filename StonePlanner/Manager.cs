using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using static StonePlanner.Interfaces;

namespace StonePlanner
{
    /// <summary>
    /// Provide managers
    /// </summary>
    internal class Manager
    {

        /// <summary>
        /// Provide functions about manage task number
        /// </summary>
        internal class SerialManager
        {
            private static object _lock = new object();
            private static SerialManager _manager;
            private List<Plan> _planList;
            private List<int> _deleteMemory = new List<int>();

            /// <summary>
            /// Count of current tasks
            /// </summary>
            public int TaskCount { get => _planList.Count - _deleteMemory.Count; }

            private SerialManager()
            {
                _planList = new();
            }

            /// <summary>
            /// Get singal instance
            /// </summary>
            /// <returns></returns>
            public static SerialManager GetManagerInstance()
            {
                if (_manager is null)
                {
                    lock (_lock)
                    {
                        if (_manager is null)
                        {
                            _manager = new SerialManager();
                        }
                    }
                }
                return _manager;
            }

            /// <summary>
            /// Add specific task to list
            /// </summary>
            /// <param name="task">The postion of current task</param>
            /// <returns></returns>
            public int AddTask(Plan task)
            {
                // Exist empty postion
                try
                {
                    int postion = FindNextPosition();
                    _planList[postion] = task;
                    return task.Height * postion;
                }

                // NOT exist empty postion
                catch (Exception e) when (e is IndexOutOfRangeException)
                {
                    _planList.Add(task);
                    return task.Height * (_planList.Count - 1);
                }
            }

            /// <summary>
            /// Remove specific task and remember the number
            /// </summary>
            /// <param name="serial"></param>
            public void RemoveTask(int serial)
            {
                _deleteMemory.Add(serial);
                _planList[serial] = null;
            }

            private int FindNextPosition()
            {
                if (_deleteMemory.Count == 0)
                    throw new IndexOutOfRangeException("Array is empty!");

                var nextPosition = (from position in _deleteMemory
                                    orderby position ascending
                                    select position).ToList()[0];

                _deleteMemory.Remove(nextPosition);
                return nextPosition;
            }

            public List<Plan> GetList() => _planList;
        }

        /// <summary>
        /// Provide functions to manage money
        /// </summary>
        internal class MoneyManager:IManager<int>
        {
            private int _Money;
            private static MoneyManager _Manager { get; set; }

            private MoneyManager(int money)
            {
                _Money = money;
            }

            public static MoneyManager GetManagerInstance(int money = 0)
            {
                if (_Manager is null)
                {
                    _Manager = new(money);
                }

                return _Manager;
            }

            public void Change(int delta) 
            {
                _Money += delta;
                SQLConnect.SQLCommandExecution($"UPDATE Users SET Cmoney = {_Money} WHERE Username = {Login.UserName}", ref Main.odcConnection);
            }

            public int GetValue() => _Money;
        }

        /// <summary>
        /// Provide functions to manage property
        /// </summary>
        public class PropertyManager : IManager<(int, int, int)>
        {
            private (int,int,int) _Property;

            public int Lasting 
            {
                get => _Property.Item1;
            }

            public int Explosive
            {
                get => _Property.Item2;
            }

            public int Wisdom
            {
                get => _Property.Item3;
            }

            public void Update(string name,int delta)
            {
                int insert = 0;
                switch (name)
                {
                    case "lasting":
                        insert = _Property.Item1 + delta;
                        _Property.Item1 = insert;
                        break;
                    case "exposive":
                        insert = _Property.Item2 + delta;
                        _Property.Item2 = insert;
                        break;
                    case "wisdom":
                        insert = _Property.Item3 + delta;
                        _Property.Item3 = insert;
                        break;
                    default:
                        break;
                }
                SQLConnect.SQLCommandExecution($"UPDATE Users SET ABT_{name} = " +
                    $"{insert} WHERE Username = '{Login.UserName}';",
                    ref Main.odcConnection);
            }

            private static PropertyManager _Manager { get; set; }

            private PropertyManager((int,int,int) property)
            {
                _Property = property;
            }

            public static PropertyManager GetManagerInstance(int lasting = 0,int explosive = 0,int wisdom = 0) 
            {
                if (_Manager is null)
                {
                    _Manager = new((lasting, explosive, wisdom));
                }
                return _Manager;
            }

            public (int, int, int) GetValue() => _Property;

            public void Change((int, int, int) delta) 
            {
                Update("lasting", delta.Item1);
                Update("explosive", delta.Item2);
                Update("wisdom", delta.Item3);
            }
        }
    }
}
