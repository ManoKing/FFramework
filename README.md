---

## 工程介绍   

此工程热更模块基于 HybridCLR + YooAsset，并将主流的GameFramework修改支持YooAsset。  
并实现，Jenkins自动多渠道打包，白名单灰度测试，一键发布热更资源及版本回退。  

（1）代码热更基于[HybridCLR跳转](https://github.com/focus-creative-games/HybridCLR)  
（2）资源热更基于[YooAsset跳转](https://www.yooasset.com/)     

---

## 如何让项目运行并实现热更及资源管理  

### HybridCLR编辑器操作
(1)点击执行HybridCLR/Installer打开一个窗体，点击Install等待安装完成  
(2)点击执行HybridCLR/Generate/All, 等待执行完毕    
(3)点击执行HybridCLR/Build/BuildAssetsAndCopyToRes,将Dll生成并拷贝到资源文件夹中   

### 资源管理操作
点击执行YooAsset/AssetBundle Collector，用于管理资源，这里通过成品资源对生资源依赖，进行加载，实现包体内零冗余   
（1）实现了启动场景热更新，在第二次进入游戏时实现更新  
（2）实现了玩家边玩边下载  
（3）玩家自己选择下载关卡内容  
注意：生资源，成品资源路径区分  

### 游戏入口流程
(1)在Main场景下，对资源进行预下载（通过玩家的UID添加白名单测试）    
(2)加载完资源会调用初始化LoadDll,加载热更Dll   
(3)加载完Dll切换热更场景，进入热更模块   

### YooAsset 实现本地模拟
需要利用HFS搭建一个本地服务器，确保手机和电脑处于同一网络，便可实现热更  
[HFS跳转](https://github.com/rejetto/hfs)  

### 其他
(1)Unity 版本使用是2022.3.xx  
(2)基于URP渲染管线(开发中)  
(3)完善UI框架，及代码自动生成工具  

---

## 项目包含一个完整的小游戏实例  

#### 游戏介绍  
基于GameFramework框架实现的塔防游戏，将资源模块改为Addressable，并接入HybridCLR代码热更。  

![Image](ReadMe/TowerDefense.png)   
   
   
![Image](ReadMe/TowerGame.png)

### 框架基于GameFramework  
使用框架的架构思想，并加以改进，如需扩展开发，只需要对GameFramework进行简单了解，可快速实现    

### 渲染基于URP  
基于URP渲染管线（开发中）  
（1）通过判断设备，启动时加载对应的低中高管线  
（2）采用非真实渲染NRP  

### 自动打包流程  
  
![Image](ReadMe/AutoAPK.png)  
   
点击Build Android，即可在工程目录build文件夹下看到FFramework.apk(有可能需要点击两次，第一次点击有可能失败)  
只需在Jenkins配置，Choice Parameter，即可实现打包自动化（开发中）   
HotFixResources：热更资源；  ChannelPack：渠道包；  LocalPack：本地包；  FirstPack：首包；（渠道包和本地包基于首包的.bin输出）  

   
---

## FAQ

### 问题
(1)CDN服务器缓存问题，不能及时获取到hash, json（增加HTTP DNS，提高热更成功率）   
(2)Addressables版本问题，资源依赖问题，不能实现增量更新（升级版本后解决）    

