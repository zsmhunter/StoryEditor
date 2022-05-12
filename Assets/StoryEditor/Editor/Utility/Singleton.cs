// **********************************************************************
// Author Name :zsm
// Create Time :2022-05-06
// Description :µ¥Àý
// **********************************************************************
using System;
using System.Reflection;

namespace StoryEditor
{
    public class Singleton<T> where T : Singleton<T>
    {
        private static T mInstance;

        public static T Instance
        {
            get
            {
                if (mInstance == null)
                {
                    var ctors = typeof(T).GetConstructors(BindingFlags.Instance | BindingFlags.NonPublic);
                    var ctor = Array.Find(ctors, c => c.GetParameters().Length == 0);
                    if (ctor == null)
                    {
                        throw new Exception("Non_Public Constructors not Found in  " + typeof(T).Name);
                    }
                    mInstance = ctor.Invoke(null) as T;
                }
                return mInstance;
            }
        }
    }
}
