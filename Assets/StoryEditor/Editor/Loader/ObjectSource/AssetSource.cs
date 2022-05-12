// **********************************************************************
// Author Name :zsm
// Create Time :2022-05-09
// Description :
// **********************************************************************
using System;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.Events;

namespace StoryEditor
{
    public abstract class AssetSource<T> : SourceBase where T : Object
    {
        /// <summary>
        /// 正常情况下不需要使用这个，预留的接口
        /// </summary>
        public override object BaseObjectTarget
        {
            get { return Source; }
        }
        public override string Path { get; protected set; } = null;
        public T Source { get; protected set; } = null;
        protected UnityAction<Object, string> m_callback;
        public override bool isDispose { get; protected set; } = false;

        public override bool isLoadComleted
        {
            get { return Source != null; }
        }

        public AssetSource(string path)
        {
            Path = path;
        }

        /// <summary>
        /// 重载，选择调用StartLoadGameOjbecct或者StartLoadAssest
        /// </summary>
        public override void StartLoad()
        {
            StartLoadAssest();
        }

        public override void ReleaseSource()
        {
            if (Source == null)
                return;
            if (Source is GameObject)
            {
                ReleaseGameOjbecct(Source);
            }
            else
            {
                ReleaseAssest(Source);
            }
        }

        protected override void StartLoadGameOjbecct()
        {
            UnityAction<Object, string> cb = null;
            cb = delegate (Object obj, string msg)
            {
                if (obj == null)
                {//obj是nil，报错了
                    if (msg == null)
                    {
                        Debug.LogError("ObjectBase StartLoadGameOjbecct msg is nil and obj is nil too");
                    }
                    if (Completed != null)
                    {
                        Completed(false);
                    }
                    return;
                }
                if (m_callback != cb)
                {//被销毁了，dispose掉了
                    ReleaseGameOjbecct(obj);
                    return;
                }
                Source = obj as T;
                if (Completed != null)
                {
                    Completed(true);
                }
            };
            m_callback = cb;
            ResourcesManager.Instance.InstantiateAsync(Path, cb);
        }

        protected override void ReleaseGameOjbecct(Object obj)
        {
            ResourcesManager.Instance.ReleaseInstance(obj as GameObject);
        }

        protected override void StartLoadAssest()
        {
            UnityAction<Object, string> cb = null;
            cb = delegate (Object obj, string msg)
            {
                if (obj == null)
                {//obj是nil，报错了
                    if (msg == null)
                    {
                        Debug.LogError("ObjectBase StartLoadGameOjbecct msg is nil and obj is nil too");
                    }
                    if (Completed != null)
                    {
                        Completed(false);
                    }
                    return;
                }
                if (m_callback != cb)
                {//被销毁了，dispose掉了
                    ReleaseAssest(obj);
                    return;
                }
                Source = obj as T;
                if (Completed != null)
                {
                    Completed(true);
                }
            };
            m_callback = cb;
            ResourcesManager.Instance.LoadAssetAsync<Object>(Path, cb);
        }

        protected override void ReleaseAssest(Object obj)
        {
            ResourcesManager.Instance.ReleaseAsset(obj);
        }

        ~AssetSource()
        {
            Dispose();
        }

        public override void Dispose()
        {
            ReleaseSource();
            isDispose = true;
            Source = null;
            m_callback = null;
            Completed = null;
        }
    }
}
