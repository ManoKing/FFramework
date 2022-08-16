---

## 工程介绍  

此工程热更模块基于 HybridCLR + Addressable 

1. https://github.com/focus-creative-games/HybridCLR 原Huatuo
2. https://docs.unity3d.com/Packages/com.unity.addressables@1.20/manual/index.html Addressables

同时支持资源和逻辑热更的基础工程

---

## 项目包含一个完整的小游戏实例  
#### 游戏介绍
游戏通过监听麦克风音量，来控制角色上下移动，角色掉到最下面则游戏结束。  

![Image](https://github.com/ManoKing/FFramework/blob/main/Assets/Res/Art/Image/flappy_beans/sample/fbs.screen-52.png)
![Image](https://github.com/ManoKing/FFramework/blob/main/Assets/Res/Art/Image/flappy_beans/sample/fbs.screen-53.png)
![Image](https://github.com/ManoKing/FFramework/blob/main/Assets/Res/Art/Image/flappy_beans/sample/fbs.screen-54.png)

#### UI框架基于QFramework  
使用框架的UI工具集，并加以改进

#### 渲染基于URP
因为URP对2D光照和阴影的支持，选择了URP  

## 如何让小游戏运行并实现热更  

#### HybridCLR
参考 https://github.com/focus-creative-games/hybridclr_trial 实例项目的README  

#### Addressable  
需要利用Addressables Hosting搭建一个本地服务器，并把电脑的防火墙关闭，确保手机和电脑处于同一网络，便可实现热更

#### 其他
(1)如需做新的界面，需要对QFramework进行简单了解，可快速实现;  
(2)Shader编写，需要基于URP,使用HLSL;  
(3)Unity 版本使用是2020.3.26f1c1，使用3.xx版本都可以

## FAQ

#### 代码剪切
(1)TypeLoadException:Could not load type 'UnityEngine.Microphone' frome assembly 'UnityEngine.AudioModule', 出现类似问题，需要修改Assets文件夹下的link文件，添加相应Assembly;  
(2)Android 权限获取，参考Unity官方文档 平台开发/Android/Device features and permissions;  

