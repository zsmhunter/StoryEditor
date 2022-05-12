// **********************************************************************
// Author Name :zsm
// Create Time :2022-05-10
// Description :������Դ����Ļ���
// **********************************************************************
using System;
using Object = UnityEngine.Object;

namespace StoryEditor
{
    public abstract class SourceBase: System.IDisposable
    {
        public virtual object BaseObjectTarget { get; }
        public virtual string Path { get; protected set; }
        public virtual bool isDispose { get; protected set; } = false;
        public virtual bool isLoadComleted { get; }

        public UnityEngine.Events.UnityAction<bool> Completed;

        /// <summary>
        /// ���أ�ѡ�����StartLoadGameOjbecct����StartLoadAssest
        /// </summary>
        public virtual void StartLoad() { }

        public virtual void ReleaseSource() { }

        protected virtual void StartLoadGameOjbecct() { }

        protected virtual void ReleaseGameOjbecct(Object obj) { }

        protected virtual void StartLoadAssest() { }

        protected virtual void ReleaseAssest(Object obj) { }

        public virtual void Dispose() { }
    }
}
