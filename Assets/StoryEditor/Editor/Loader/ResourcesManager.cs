// **********************************************************************
// Author Name :zsm
// Create Time :2022-05-06
// Description :
// **********************************************************************
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.AddressableAssets.ResourceLocators;
using UnityEngine.Events;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;
using Object = UnityEngine.Object;

namespace StoryEditor
{
    public class SpawnPoolObject : SpawnPool<Object>
    {
        public SpawnPoolObject(Object source, string path, UnityAction<Object> actionOnGet = null, UnityAction<Object> actionOnRelease = null) : base(
            source, path, actionOnGet, actionOnRelease)
        {
        }
    }

    public class ResourcesManager : Singleton<ResourcesManager>
    {
        Dictionary<string, SpawnPoolObject> SpawnPoolDic = new Dictionary<string, SpawnPoolObject>();
        private AsyncOperationStatus isCatLogLoadComplete = AsyncOperationStatus.None;
        /// <summary>
        /// 所有资源列表
        /// </summary>
        private Dictionary<string, IResourceLocation> allResourcesList = new Dictionary<string, IResourceLocation>();

        protected void GetAllResourcesList()
        {
            var allLocations = new List<IResourceLocation>();
            foreach (var resourceLocator in Addressables.ResourceLocators)
            {
                Debug.Log(resourceLocator);
                if (resourceLocator is ResourceLocationMap map)
                {
                    foreach (var locations in map.Locations.Values)
                    {
                        allLocations.AddRange(locations);
                    }
                }
            }
            allResourcesList.Clear();
            foreach (var location in allLocations)
            {
                if (location.ResourceType == typeof(UnityEngine.ResourceManagement.ResourceProviders.IAssetBundleResource))
                    continue;
                if (allResourcesList.ContainsKey(location.InternalId))
                    continue;
                allResourcesList.Add(location.InternalId, location);
                //Debug.LogError($"hascode:{location.GetHashCode()},InternalId:{ location.InternalId}\nProviderId:{ location.ProviderId}\nData:{ location.Data}\nPrimaryKey:{ location.PrimaryKey}\nResourceType:{location.ResourceType}");
            }
        }

        ResourcesManager()
        {
            if (!UnityEditor.EditorApplication.isPlaying)
            {
                Debug.LogError("非运行状态下，不要启动ResourcesManager");
                return;
            }
            if (StoryEditorSetting.IsRunTime)
                LoadAddressableCataLog(StoryEditorSetting.AssetbundlePath);
            else
            {
                isCatLogLoadComplete = AsyncOperationStatus.Succeeded;
                GetAllResourcesList();
            }           
        }

        /// <summary>
        /// 加载Addressable资源
        /// </summary>
        public void LoadAddressableCataLog(string path)
        {
            Debug.Log("LoadAddressableCataLog Start");
            AsyncOperationHandle<IResourceLocator> loadContentCatalogAsync = Addressables.LoadContentCatalogAsync(path);
            loadContentCatalogAsync.Completed += (obj) =>
            {
                if (AsyncOperationStatus.Succeeded == obj.Status)
                {
                    isCatLogLoadComplete = AsyncOperationStatus.Succeeded;
                    Debug.Log("LoadAddressableCataLog Succeeded");
                    GetAllResourcesList();
                }
                else
                {
                    isCatLogLoadComplete = AsyncOperationStatus.Failed;
                    if (obj.OperationException != null)
                        Debug.LogError($"LoadAddressableCataLog failed, OperationException:{obj.OperationException.ToString()}");
                        //complete($"LoadAddressableCataLog failed, OperationException:{obj.OperationException.ToString()}");
                    else
                        Debug.LogError($"LoadAddressableCataLog failed");
                }
            };
        }

        public void GetSourceAsync<T>(out T obj, string path, UnityAction<bool> Completed) where T : SourceBase
        {
            obj = (T)Activator.CreateInstance(typeof(T), path);
            obj.Completed = Completed;
            obj.StartLoad();
        }

        protected async Task<bool> WaitLoadCatlog()
        {
            return await Task.Run<bool>(() =>
            {
                while (true)
                {
                    if (isCatLogLoadComplete != AsyncOperationStatus.None)
                        return true;
                }
            });
        }

        /// <summary>
        /// 放在这里，方便后面如果Addressables不好用，直接改成别的
        /// </summary>
        /// <param name="path"></param>
        /// <param name="callback"></param>
        public async void InstantiateAsync(string path, UnityAction<Object, string> callback)
        {
            GameObject obj = null;
            string msg = null;
            if (isCatLogLoadComplete == AsyncOperationStatus.None)
            {
                await WaitLoadCatlog();                
            }
            try
            {
                obj = await Addressables.InstantiateAsync(path).Task;
            }
            catch (Exception ex)
            {
                Debug.LogError($"AddressablesLoad error:{ex.Message}");
            }
            finally
            {
                callback(obj, msg);
            }
        }

        /// <summary>
        /// 放在这里，方便后面如果Addressables不好用，直接改成别的
        /// </summary>
        /// <param name="path"></param>
        /// <param name="callback"></param>
        public async void LoadAssetAsync<T>(string path, UnityAction<T, string> callback) where T : Object
        {
            T obj = default(T);
            string msg = null;
            if (isCatLogLoadComplete == AsyncOperationStatus.None)
            {
                await WaitLoadCatlog();
            }
            try
            {
                AsyncOperationHandle<T> asoh = Addressables.LoadAssetAsync<T>(path);
                obj = await asoh.Task;
            }
            catch (Exception ex)
            {
                Debug.LogError($"AddressablesLoad error:{ex.Message}");
            }
            finally
            {
                callback(obj, msg);
            }
        }

        /// <summary>
        /// 释放某个使用完的资源
        /// </summary>
        /// <param name="obj"></param>
        public void ReleaseObjectBase(SourceBase obj)
        {
            obj.Dispose();
        }

        public void ReleaseInstance(GameObject obj)
        {
            Addressables.ReleaseInstance(obj);
        }

        public void ReleaseAsset(Object obj)
        {
            Addressables.Release(obj);
        }

        protected void DisposePool(SpawnPoolObject pool, bool removeDic = false)
        {
            ReleaseAsset(pool.source);
            pool.Dispose();
            if (removeDic)
            {
                foreach (KeyValuePair<string, SpawnPoolObject> kv in SpawnPoolDic)
                {
                    if (kv.Value == pool)
                    {
                        SpawnPoolDic.Remove(kv.Key);
                        break;
                    }
                }
            }
        }

        protected void DisposePool(string path)
        {
            SpawnPoolObject pool;
            if (!SpawnPoolDic.Remove(path, out pool))
            {
                return;
            }
            DisposePool(pool);
        }
    }
}
