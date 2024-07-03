using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading.Tasks;

namespace StonePlanner
{
    internal class DataType
    {
        internal enum ExceptionsLevel
        {
            Infomation,
            Caution,
            Warning,
            Error
        }

        internal class SignChangedEventArgs:EventArgs
        {
            public int Sign { get; set; }
        }

        internal class VersionInfo : Interfaces.IVersion
        {
            public string Version { get; set; }
            public int Number { get; set; }
            public Uri DownloadUri { get; set; }

            public string GetVersion() 
            {
                return Version;
            }

            public bool IsNeedUpdate(int now) 
            {
                if (Number > now)
                    return true;
                return false;
            }

            public Uri GetUri() 
            { 
                return DownloadUri; 
            }
        }

        /// <summary>
        /// Provide functions about manage task number
        /// </summary>
        public class SerialManager
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
    }
}
