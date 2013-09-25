using System;
using System.Collections;
using System.Reflection;
using System.Threading;
using System.Diagnostics;

namespace WebSearch.Common.Net
{
    ///<summary>   
    /// 使用HashCodeLock.Lock超时   
    ///</summary>   
    [Serializable]
    public sealed class HashCodeLockTimeoutException : Exception
    {
        internal HashCodeLockTimeoutException(string msg) : base(msg) { }
    }

    /// <summary>
    /// 针对类型和值的锁
    /// </summary>
    public sealed class HashCodeLock : MarshalByRefObject
    {
        #region Constructors

        /// <summary>
        /// 用30秒作为默认过期时间,并且超时引发异常   
        /// </summary>
        public HashCodeLock() : this(30000, true)
        {
        }

        /// <summary>
        /// 指定默认过期时间,和是否引发异常
        /// </summary>
        /// <param name="msTimeout"></param>
        /// <param name="timeoutAsException"></param>
        public HashCodeLock(int msTimeout, bool timeoutAsException)
        {
            _timeout = msTimeout;
            _excep = timeoutAsException;
        }

        #endregion

        #region Properties

        private int _timeout;
        private bool _excep;

        /// <summary>
        /// 默认过期时间
        /// </summary>
        public int DefaultTimeout
        {
            get { return _timeout; }
            set { _timeout = value; }
        }

        #endregion

        #region class Allocator

        private class Allocator
        {
            private class Node
            {
                private Type _type;
                private object _value;

                public Node(object obj)
                {
                    _value = obj;
                    _type = _value.GetType();
                }

                public override bool Equals(object obj)
                {
                    Node node = (Node)obj;
                    if (_type != node._type) 
                        return false;
                    if (_value != node._value)
                        return false;
                    return true;
                }

                public override int GetHashCode()
                {
                    return _value.GetHashCode();
                }
            }

            private ArrayList _list = new ArrayList();

            /// <summary>
            /// 
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public object Allocate(object obj)
            {
                object node = new Node(obj);
                lock (_list.SyncRoot)
                {
                    int index = _list.IndexOf(node);

                    // index is not in list
                    if (index != -1)
                        node = _list[index];
                    _list.Add(node);
                    return node;
                }
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="obj"></param>
            /// <returns></returns>
            public object Release(object obj)
            {
                object node = new Node(obj);
                lock (_list.SyncRoot)
                {
                    int index = _list.LastIndexOf(node);

                    // index is not in list
                    if (index == -1)
                        throw (new Exception("can't release obj:node not found"));
                    node = _list[index];
                    _list.RemoveAt(index);
                    return node;
                }
            }
        }

        #endregion

        #region class InternalLock

        private class InternalLock : IDisposable
        {
            private HashCodeLock _lock;
            private object _value;

            public InternalLock(HashCodeLock hcl, object obj, int timeout)
            {
                this._lock = hcl;
                this._value = obj;

                object syncobj = hcl.allocator.Allocate(obj);
                try
                {
                    if (!Monitor.TryEnter(syncobj, timeout))
                    {
                        throw (new HashCodeLockTimeoutException(
                            "object: " + _value.ToString() + " lock timeout"));
                    }
                }
                catch
                {
                    hcl.allocator.Release(obj);
                    throw;
                }

                SetStackTrace();
            }

            private string s = "unknown method (not query in release)";

            //[Conditional("DEBUG+X")]
            void SetStackTrace()
            {
                try
                {
                    StackTrace st = new StackTrace(4);//skip at lease 3 frame

                    for (int i = 0; i < st.FrameCount; i++)
                    {
                        MethodBase mb = st.GetFrame(i).GetMethod();
                        if (mb.ReflectedType != typeof(HashCodeLock))
                        {
                            s = mb.ReflectedType.FullName + ":" + mb.ToString();
                            return;
                        }
                    }
                }
                catch
                {
                }
            }

            public void Dispose()
            {
                if (_lock == null)
                    throw (new Exception("lock   at   " + s + "   already   released!"));
                Monitor.Exit(_lock.allocator.Release(_value));
                _lock = null;
                _value = null;
            }

            ~InternalLock()
            {
                if (_lock != null)
                {
                    try
                    {
                        //回收对象。但无法使用Monitor.Exit   
                        _lock.allocator.Release(_value);
                        //不同的线程了，Monitor.Exit(h.ac.Release(o));不起作用   
                    }
                    finally
                    {
                        throw (new Exception("lock at " + s + " have not released!"));
                    }
                }
            }
        }

        #endregion

        #region LockHolder

        /// <summary>
        /// 用于防止lock(hcl.Lock(xx))的误用   
        /// </summary>
        public struct LockHolder : IDisposable
        {
            IDisposable dis;

            internal LockHolder(IDisposable hee)
            {
                dis = hee;
            }

            void IDisposable.Dispose()
            {
                if (dis != null)
                {
                    dis.Dispose();
                    dis = null;
                }
            }
        }

        #endregion

        #region Instance

        Allocator allocator = new Allocator();

        static NothingToDispose Nothing = new NothingToDispose();

        private class NothingToDispose : IDisposable
        {
            public void Dispose()
            {
            }
        }

        LockHolder CreateLock(object obj, int msTimeout)
        {
            if (_excep)
                return new LockHolder(new InternalLock(this, obj, msTimeout));
            try
            {
                return new LockHolder(new InternalLock(this, obj, msTimeout));
            }
            catch (HashCodeLockTimeoutException)
            {
                return new LockHolder(null);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public LockHolder Lock(object obj)
        {
            if (obj == null) throw (new ArgumentNullException("obj"));
            return CreateLock(obj, _timeout);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="msTimeout"></param>
        /// <returns></returns>
        public LockHolder Lock(object obj, int msTimeout)
        {
            if (obj == null) throw (new ArgumentNullException("obj"));
            return CreateLock(obj, msTimeout);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="tsTimeout"></param>
        /// <returns></returns>
        public LockHolder Lock(object obj, TimeSpan tsTimeout)
        {
            if (obj == null) throw (new ArgumentNullException("obj"));
            return CreateLock(obj, (int)tsTimeout.TotalMilliseconds);
        }

        #endregion

        #region Static Members

        private static HashCodeLock hcl = new HashCodeLock();

        /// <summary>
        /// 用于全局的HashCodeLock
        /// </summary>
        public static HashCodeLock GlobalLock
        {
            get { return hcl; }
        }

        #endregion
    }
}
