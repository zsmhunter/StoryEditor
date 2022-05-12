using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using Cysharp.Threading.Tasks;

namespace StoryEditor
{
    public class AddressableExample : MonoBehaviour
    {
        private CancellationTokenSource cancellation;
        [SerializeField]
        private AssetReference _asset;

        private GameObject _instance;

        public GameObject Instance { get => _instance; set => _instance = value; }

        private async UniTaskVoid OnEnable()
        {
            // 加载会触发内部引用计数(ref count)增加1次，手动实例化不会变更引用计数，必须手动计数
            // Addressable没办法持有手动实例化后的物件的引用，Unload也不会摧毁对应实例

            #region Load by addressables and handle instantiate manually

            // AsyncOperationHandle 完成时的回调
            // _asset.LoadAssetAsync<GameObject>().Completed += OnLoadedCompleted;

            // 使用await/async 的加载方法
            // 开始加载，使用await 等待异步操作完成
            // var obj = await _asset.LoadAssetAsync<GameObject>().Task;
            // _instance = Instantiate(obj);

            #endregion

            // 使用Addressable直接实例化会增加引用计数，同时内部保存实例
            // 当使用ReleaseAsset或ReleaseInstance时，引用计数归零会自动Unload Bundle
            // 在不需要手动管理的情况下，推荐使用Addressable系统实例化

            #region Load and instantiate by addressables

            //_asset.InstantiateAsync().Completed += OnInstantiatedCompleted;
            // 使用await/async 的加载方法
            cancellation = new CancellationTokenSource();
            cancellation.RegisterRaiseCancelOnDestroy(this);
            {
                Debug.Log("Instantiated start");
                //_instance = await _asset.InstantiateAsync();
                _instance = await _asset.InstantiateAsync().WithCancellation<GameObject>(cancellationToken: cancellation.Token);
                //await _asset.InstantiateAsync().WithCancellation(cancellation.Token);
                Debug.Log($"Instantiated finished:{_instance}");
            }
            {
                Debug.Log($"Instantiated finally:{_instance}");
                cancellation.Dispose();
                cancellation = null;
            }

            #endregion
        }

        public void cancel()
        {
            cancellation.Cancel();
        }

        private void OnInstantiatedCompleted(AsyncOperationHandle<GameObject> obj)
        {
            Debug.Log($"Instantiate {obj.Result.name} completed.");
            _instance = obj.Result;
        }

        private void OnLoadedCompleted(AsyncOperationHandle<GameObject> obj)
        {
            Debug.Log($"Load {obj.Result.name} from async operation.");
            _instance = Instantiate(obj.Result);
        }

        private void OnDisable()
        {
            if (cancellation != null)
            {
                cancellation.Cancel();
                cancellation = null;
            }
            Debug.Log($"AddressableExample OnDisable:{_instance}");
            // 使用Addressables实例化的实例不能手动删除，否则会破坏ref count的正确性
            // Ref Count到达0后会Unload bundle，这时如果依然引用到bundle内的资源的话，他们之间就会失去live-link，出现资源引用错误

            // 释放资源异步句柄
            _asset.ReleaseAsset();

            // 释放实例，减少ref count，如果是Addressable来Instantiate的实例也会被删除
            // 切换场景时，场景上保有的物件如果是addressable生成的话，也会调用release instance
            // 如果此时ref count达到0也会触发bundle的unload（自动化管理）
            _asset.ReleaseInstance(_instance);
        }
    }
}