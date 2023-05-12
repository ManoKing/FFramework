---

## 工程介绍  

此工程热更模块基于 HybridCLR + Addressable 

HybridCLR: https://github.com/focus-creative-games/HybridCLR 
Addressable: https://docs.unity3d.com/Packages/com.unity.addressables@1.21/manual/index.html 

主要展示资源和逻辑热更的基础工程。  

资源管理基于Asset Graph: https://docs.unity3d.com/Packages/com.unity.assetgraph@1.7/manual/index.html  

AssetGraph is a tool that aims to reduce the workload needed to build workflows around asset importing,   
building Asset Bundles and building Player Apps. With this tool, you can build workflows to create, modify,   
and change asset settings graphically, and even automate them, thus freeing designers and artists from   
repetitive tasks during game development.


---

## 项目包含一个完整的小游戏实例  
#### 游戏介绍
游戏通过监听麦克风音量，来控制角色上下移动，角色掉到最下面则游戏结束。  
游戏资源全部来源于 OpenGameArt.org   

![Image](https://github.com/ManoKing/FFramework/blob/main/Assets/Res/Art/Sprite/FlappyBeans/Sample/fbs.screen-52.png)
![Image](https://github.com/ManoKing/FFramework/blob/main/Assets/Res/Art/Sprite/FlappyBeans/Sample/fbs.screen-53.png)
![Image](https://github.com/ManoKing/FFramework/blob/main/Assets/Res/Art/Sprite/FlappyBeans/Sample/fbs.screen-54.png)

#### UI框架基于QFramework  
使用框架的UI工具集，并加以改进

#### 渲染基于URP
基于URP定制相关功能  

## 如何让小游戏运行并实现热更  

#### 游戏入口
(1)在Main场景下，对资源进行预下载，关闭了Addressable的自动加载（自动加载，进入游戏的时候会卡顿一会，很影响用户体验）;  
(2)加载完资源会调用初始化LoadDll,加载热更DLL;  
(3)加载完DLL切换热更场景，进入热更模块;  

#### HybridCLR
参考 https://github.com/focus-creative-games/hybridclr_trial 实例项目的README  

#### Addressable  
需要利用Addressables Hosting搭建一个本地服务器，确保手机和电脑处于同一网络，便可实现热更

#### 其他
(1)如需做新的界面，需要对QFramework进行简单了解，可快速实现;  
(2)Shader编写，需要基于URP;  
(3)Unity 版本使用是2020.3.44f1

## FAQ

#### 代码剪切
(1)TypeLoadException:Could not load type 'UnityEngine.Microphone' frome assembly 'UnityEngine.AudioModule', 出现类似问题，需要修改Assets文件夹下的link文件，添加相应Assembly;  
(2)Android 权限获取，参考Unity官方文档 平台开发/Android/Device features and permissions;  
(3)CDN服务器缓存问题，不能及时获取到hash, json;  
(4)Addressables版本问题，进入游戏卡住一段时间;  

