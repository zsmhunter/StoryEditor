// **********************************************************************
// Author Name :zsm
// Create Time :2022-05-10
// Description :
// **********************************************************************
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace StoryEditor
{

    public class SpawnPool<T> where T : Object
    {
        protected readonly Stack<T> m_Stack = new Stack<T>();
        protected UnityAction<T> m_ActionOnGet;
        protected UnityAction<T> m_ActionOnRelease;
        public string Path { get; protected set; }
        LS_ObjectPool<GameObject> modelPrefab = new LS_ObjectPool<GameObject>();
        public T source { get; protected set; }
        public int countAll { get; protected set; }
        public int countActive { get { return countAll - countInactive; } }
        public int countInactive { get { return m_Stack.Count; } }

        public List<T> AllObjects { get; protected set; }

        /// <summary>
        /// 缓存池
        /// </summary>
        /// <param name="source">原生资源</param>
        /// <param name="path">资源路径，缓存跟debug用</param>
        /// <param name="actionOnGet">获取的时候的回调函数</param>
        /// <param name="actionOnRelease">释放的时候的回调函数</param>
        public SpawnPool(T source, string path, UnityAction<T> actionOnGet = null, UnityAction<T> actionOnRelease = null)
        {
            this.source = source;
            Path = path;
            AllObjects = new List<T>();
            m_ActionOnGet = actionOnGet;
            m_ActionOnRelease = actionOnRelease;
        }

        public T Get()
        {
            T element;
            if (m_Stack.Count == 0)
            {
                element = UnityEngine.Object.Instantiate<T>(source);
                AllObjects.Add(element);
                countAll++;
            }
            else
            {
                element = m_Stack.Pop();
            }
            return element;
        }

        public void Release(T element)
        {
            if (m_Stack.Count > 0 && ReferenceEquals(m_Stack.Peek(), element))
                Debug.LogError("Internal error. Trying to destroy object that is already released to pool.");
            if (m_ActionOnRelease != null)
                m_ActionOnRelease(element);
            m_Stack.Push(element);
        }

        public void Dispose()
        {
            foreach (Object obj in AllObjects)
                Object.DestroyImmediate(obj);
            AllObjects.Clear();
            source = null;
        }
    }
}
