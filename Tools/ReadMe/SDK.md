在现代游戏开发中，接入第三方SDK（软件开发工具包）是提升游戏功能的重要方式。对于Unity开发者而言，接入iOS SDK能够实现广告展示、用户分析、推送通知等功能，从而增强游戏的市场竞争力。本文将详细介绍如何在Unity中集成iOS SDK，并提供代码示例。  

准备工作  
在开始之前，确保你已经完成以下准备工作：  

Unity环境：安装最新版本的Unity。  
Xcode安装：需要在Mac上安装Xcode，以便编译并运行iOS应用。  
SDK获取：下载你想要接入的iOS SDK，通常SDK会提供相应的文档。  
### 1. 创建Unity项目  
首先，我们创建一个新的Unity项目：  

打开Unity Hub，点击“新建项目”。  
选择一个2D或3D模板，给项目命名，然后点击“创建”按钮。  
### 2. 导入SDK  
将SDK导入Unity项目：  

解压下载的SDK文件，将其中的“iOS”文件夹复制到Unity项目的Assets目录下。  
在Unity中，右键点击Assets文件夹，选择“Import Package” > “Custom Package”。  
选择SDK提供的.unitypackage文件，导入所需的Assets。  
### 3. 编写iOS原生代码   
Unity提供了与原生代码交互的方法。我们需要编写Objective-C或Swift代码来实现SDK的功能。下面的代码示例展示了如何创建一个简单的Objective-C类来调用SDK的初始化方法。  

在Unity的Assets目录下，创建一个名为“Plugins/iOS”的文件夹，并添加一个名为MySDKManager.mm的文件：  

```C++  
#import <Foundation/Foundation.h>
#import <UnityAppController.h>
#import "<SDK_HEADER>.h" // 替换为SDK的头文件名

@interface MySDKManager: NSObject
+ (void)initializeSDK;
@end

@implementation MySDKManager
+ (void)initializeSDK {
    // 初始化SDK
    [SDKClass initialize]; // 替换为SDK的初始化方法
}
@end
```  
### 4. C#脚本交互   
现在，我们来创建一个C#脚本来调用刚才实现的iOS方法。新建一个名为SDKManager.cs的C#脚本，并将其放入Assets目录下：  
```C#  
using UnityEngine;
using System.Runtime.InteropServices;

public class SDKManager : MonoBehaviour
{
    [DllImport("__Internal")]
    private static extern void initializeSDK();

    void Start()
    {
#if UNITY_IOS && !UNITY_EDITOR
        initializeSDK();
#endif
    }
}
```  
### 5. 添加到场景  
接下来，我们需要将SDKManager添加到场景中以确保它会在游戏启动时调用：  

在Hierarchy窗口中右键点击，选择“Create Empty”创建一个空物体。  
将SDKManager脚本拖放到这个空物体上。  
### 6. 测试与构建  
在Unity中，选择“File” > “Build Settings”，选择“iOS”作为目标平台，然后点击“Build”按钮。选择一个输出目录，Unity将生成一个Xcode项目。  

打开Xcode项目，连接你的iOS设备，并运行项目。  
