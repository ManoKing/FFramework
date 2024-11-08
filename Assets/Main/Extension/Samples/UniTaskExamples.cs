using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
    为 Unity 提供有效的无GC async/await集成。
    基于Struct `UniTask<T>` 的自定义 AsyncMethodBuilder，实现零GC,使所有Unity的异步操作和协程可以await
    基于PlayerLoop的Task（ `UniTask.Yield`、 `UniTask.Delay`、 `UniTask.DelayFrame` 等）这使得能够替换所有协程操作
    MonoBehaviour 消息事件和 uGUI 事件为可使用Await/AsyncEnumerable
    完全在 Unity 的 PlayerLoop 上运行，因此不使用线程，可在 WebGL、wasm 等平台上运行。
    异步 LINQ，具有Channel和 AsyncReactiveProperty
    防止内存泄漏的 TaskTracker (Task追踪器)窗口
    与Task/ValueTask/IValueTaskSource 的行为高度兼容
 */
public class UniTaskExamples : MonoBehaviour
{
    private bool isActive;
    private Button _button;

    private void Start()
    {
        var token = this.GetCancellationTokenOnDestroy();

        //防连击按钮
        //按一下按钮就会有两秒钟没有反应
        _button.OnClickAsAsyncEnumerable()
            .ForEachAwaitWithCancellationAsync(async (_, ct) =>
            {
                Debug.Log("Clicked!");
                await UniTask.Delay(2000, cancellationToken: ct);
            }, token);
    }

    //实现0开销(0GC和快速执行)的async/await Unity集成
    async UniTask<string> DemoAsync()
    {
        //您可以await Unity async对象
        var asset = await Resources.LoadAsync<TextAsset>("foo");
        var txt = (await UnityWebRequest.Get("https://...").SendWebRequest()).downloadHandler.text;
        await SceneManager.LoadSceneAsync("scene2");

        //.WithCancellation 启用取消方法，GetCancellationTokenOnDestroy 与 GameObject 的生命周期同步
        var asset2 = await Resources.LoadAsync<TextAsset>("bar").WithCancellation(this.GetCancellationTokenOnDestroy());

        //.ToUniTask 接受进度回调（和完整的参数），Progress.Create 是 IProgress<T> 的轻量级替代品
        var asset3 = await Resources.LoadAsync<TextAsset>("baz").ToUniTask(Progress.Create<float>(x => Debug.Log(x)));

        //像协程一样，是await基于帧的操作
        await UniTask.DelayFrame(100);

        //替换 yield return new WaitForSeconds/WaitForSecondsRealtime
        await UniTask.Delay(TimeSpan.FromSeconds(10), ignoreTimeScale: false);

        //产生任何播放器循环时间（PreUpdate、Update、LateUpdate 等...）
        await UniTask.Yield(PlayerLoopTiming.PreLateUpdate);

        //替换 yield return null
        await UniTask.Yield();
        await UniTask.NextFrame();

        //替换 WaitForEndOfFrame(需要 MonoBehaviour(CoroutineRunner))
        await UniTask.WaitForEndOfFrame(this); //这是 MonoBehaviour
                                               //替换 yield return new WaitForFixedUpdate(同 UniTask.Yield(PlayerLoopTiming.FixedUpdate))
        await UniTask.WaitForFixedUpdate();

        //替换 yield return WaitUntil
        await UniTask.WaitUntil(() => isActive == false);

        //WaitUntil 的帮助方法
        await UniTask.WaitUntilValueChanged(this, x => x.isActive);

        //您可以await IEnumerator 协程
        await FooCoroutineEnumerator();

        //您可以await C#标准Task
        await Task.Run(() => 100);

        //多线程，此代码下运行在 ThreadPool 上
        await UniTask.SwitchToThreadPool();

        /*在线程池上工作*/
        //返回主线程（与 UniRx 中的 `ObserveOnMainThread` 相同）
        await UniTask.SwitchToMainThread();

        //获取async网络请求
        async UniTask<string> GetTextAsync(UnityWebRequest req)
        {
            var op = await req.SendWebRequest();
            return op.downloadHandler.text;
        }

        var task1 = GetTextAsync(UnityWebRequest.Get("http://google.com"));
        var task2 = GetTextAsync(UnityWebRequest.Get("http://bing.com"));
        var task3 = GetTextAsync(UnityWebRequest.Get("http://yahoo.com"));

        //并发async await并通过元组语法轻松获取结果
        var (google, bing, yahoo) = await UniTask.WhenAll(task1, task2, task3);

        //WhenAll的简写，tuple可以直接await
        var (google2, bing2, yahoo2) = await (task1, task2, task3);

        //返回async值。（或者您可以使用 `UniTask`（无结果）、`UniTaskVoid`（即发即弃））。
        return (asset as TextAsset)?.text ?? throw new InvalidOperationException("Asset not found");
    }

    private IEnumerator FooCoroutineEnumerator()
    {
        yield return null;
    }
}
