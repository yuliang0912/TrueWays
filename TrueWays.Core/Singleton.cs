using System;
using System.Reflection;

namespace TrueWays.Core
{
    /// <summary>
    /// 单例基类，集成自它的对象，都可以是单例对象。
    /// 要求必须有私有无参构造函数
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class Singleton<T>
        where T : Singleton<T>
    {
        private class SingleHolder
        {
            public static T Instance;

            static SingleHolder()
            {
                var constructor = typeof(T).GetConstructor(BindingFlags.NonPublic | BindingFlags.Instance, null, new Type[0], null);
                if (constructor == null)
                {
                    throw new Exception($"类型“{typeof (T).FullName}”不存在无参构私有造函数。");
                }

                Instance = constructor.Invoke(null) as T;
            }
        }

        public static T Instance => SingleHolder.Instance;
    }
}
