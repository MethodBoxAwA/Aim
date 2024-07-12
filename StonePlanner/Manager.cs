using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Threading;
using static StonePlanner.DataType.Structs;
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

            internal event Action<int> MoneyChanged;

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
                var _accountManager = AccountManager.GetManagerInstance();
                _Money += delta;

                var entity = AccessEntity.GetAccessEntityInstance();
                var user = new User() { UserMoney = _Money };

                MoneyChanged(_Money);
                entity.UpdateElement(user, new NonMappingTable(), "ID", "tb_Users");
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
                var user = new User();

                switch (name)
                {
                    case "lasting":
                        int insert = _Property.Item1 + delta;
                        _Property.Item1 = insert;
                        user.Lasting = insert;
                        break;
                    case "exposive":
                        insert = _Property.Item2 + delta;
                        _Property.Item2 = insert;
                        user.Explosive = insert;
                        break;
                    case "wisdom":
                        insert = _Property.Item3 + delta;
                        _Property.Item3 = insert;
                        user.Wisdom = insert;
                        break;
                    default:
                        break;
                }

                name = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(name);
                var accountManager = AccountManager.GetManagerInstance();
                var entity = AccessEntity.GetAccessEntityInstance();

                entity.UpdateElement(user, new NonMappingTable(), "ID", "tb_Users");
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

        public class AccountManager : IManager<(string, string)> 
        {
            private string _Account;
            private string _Type;
            private static AccountManager _Manager { get; set; }

            private AccountManager(string account, string type)
            {
                _Account = account;
                _Type = type;
            }

            public static AccountManager GetManagerInstance(string account = null, string type = null)
            {
                if (_Manager is null)
                {
                    _Manager = new(account ,type);
                }

                return _Manager;
            }

            public void Change((string, string) value)
            {
                _Account = value.Item1;
                _Type = value.Item2;
            }

            public (string,string) GetValue() => (_Account, _Type);
        }
    }
}
