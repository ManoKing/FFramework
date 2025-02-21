对于所有游戏设计来说，新手引导已经是提前引领玩家快速了解游戏玩法的必要模块，但是因为引导牵涉到太多的功能模块，使其代码和其他模块耦合比较高，如何设计一套高效，且独立的新手引导，是客户端程序必须要掌握的一个技巧。  

本文以U3D为开发平台，来设计一个引导框架，旨在帮助更多的前端，打开一个思路。  

### 一、新手引导保存的格式   
一般的配置信息，通常用Excel进行保存，但是不能存放一些嵌套的格式，或者存放起来读取并不方便，所以我们使用json进行存储。  

### 二、新手引导存储结构  
##### 1.新手引导主结构  
代码如下（示例）：  
```C#  
public class MainGuideData
{
    // 引导的名称
    public string GuideName; 
    // 引导的id;
    public int GuideId;
    // 是否强制引导
    public int ForceGuide;
	// 前置引导id;
	public int PreId;
	// 后置引导id;
	public int PostId;
	// 引导优先级
	public int Priority;
	// 中断后重新触发的子进度索引;
	public int RestartSubIndex;
	// 保存进度的子进度索引;
	public int SaveSubIndex;
	// 存出的子引导任务;
	public List<SubGuideData> SubGuideList;
}
```  
每个引导通常由一些列子引导组成，而每个引导之间可能有一定的前置后置关系，比如只有当引导了玩家收集装备后。才会再次有引导玩家进行装备强化或者升级。  

ForceGuide 代表是否是强制引导，强制引导，整个屏幕将被引导蒙版进行遮挡，仅仅只有需要点击或者拖动的，进行高亮，玩家只能在高亮的位置进行操作.  
PreId 和PostId 目的就是为了将所有的引导串成多颗树，每一个树代表一系列独立的引导，而每个树中的叶子节点，则代表引导的先后顺序。  
Priority优先级，是当有同时多个引导进行触发时，但是对于玩家来说，只能按照优先级进行一个一个的触发，这时就需要这个字段.  
RestartSubIndex, 中断后重新触发的引导步骤， 对于一些非强制型引导，可能中间玩家选择其他按钮或入口，离开当前因引导的界面，此时再次返回时，整个引导需要重置，且直接从RestartSubIndex这一步进行执行.  
SaveSubIndex 当子引导执行到这一步时，尽管后面可能还有引导任务，也需要进行保存，标识该引导已经完成，如果此时中断该引导，或者玩家杀进程，再次重新登陆，该引导标识已经完成，且不会再次执行该引导了.  
SubGuideList 子引导列表，该列表代表这个引导的所有具体的子引导，其子引导包括具体的表现，如对话，高亮按钮，等。  
##### 2.子引导结构    
子引导是具体的引导，引导一般包含有触发时机，触发条件，以及触发后表现。触发时机比如，当玩家升级结束后，玩家打开指定界面，玩家获得物品等等，这些类型繁多，对应的参数也各不同。所有定义一个通用的结构很难，我们可以采用Dictionary来进行存储配置信息，用不同key和value来作为对应器触发条件的参数.  
对于条件，也有多种，如，玩家是否达到多少及，玩家是否拥有指定装备，当前是否打开了指定界面等等，其具体条件也众多，我们也可以采用和触发时机一样用Dictionary来进行存储配置。  
触发后表现，同样也用Dictionary进行存储配置。因此，我们定义一个公共的GuideItemData结构  
```C#  
public class GuideItemData
{
	// 具体的Action, Conditon,或者Trigger的类型;
    public string Type;
    // 具体的参数信息;
    public Dictionary<string, string> Params;
}
```
有了GuideItemData结构后，我们可以开始定义SubGuideData的结构了  

引导要有触发时机， 我们需要一个 Triggers字段，它是一个列表，因为触发的条件可能一个，也可能多个。  
当触发条件中任意一个时机触发后，我们需要检测当前触发条件是否满足，如果满足，则进行表现的播放，所有需要一个Conditions.  
再次，当前触发后，满足触发条件，需要执行相应的表现，我们需要一个Actions来进行表现的展现.  
通常有这三个字段就能够满足大部分引导的需求了。  
最后，当有表现时，我们什么时候结束当前表现，然后再进行下一个子引导，这时需要一个结束触发器，来完成当前子引导的触发。 我们将这个触发器列表取名为Dones.  
通常一般强制引导需求，用这结构能满足需要，但是对于非强制引导任务来说，比如当前我在战斗主场景正在对话，玩家这时不点击对话，而是点击战斗场景，此时，我们应该中断当前引导，并且让其重新从RestartSubIndex处来时引导，但是如何判断当前应该中断该引导呢，所有还需要一个表示当前Action需要维持的条件。这个我们取名为StayConditions.  
通常玩家执行一次donetrigger,就结束当前action进行到下一个子引导，但是有时，可能需要执行多次donetrigger,知道满足条件后，才结束当前的action进行下一个引导，比如，引导玩家将装备连续升级到5级，才执行后续引导，所以，还需要一个DoneConditions.  
有时还有，一个Action可能将真个游戏更改成某个状态(比如游戏暂停)，当执行Done后，再次进行恢复(游戏恢复），此时，我们还需要一个DoneActions.  
最终我们的子引导结构如下  
```C#  
public class SubGuideData
{
	// 引导名称
	public string Name;
	// 引导触发器列表
	public List<GuideItemData> Triggers;
	// 引导触发条件
	public List<GuideItemData> Conditions;
	// 引导结束触发器;
	public List<GuideItemData> Dones;
	// 引导结束条件
	public List<GuideItemData> DoneConditions;
	// 引导Action保持需要的条件;
	public List<GuideItemData> StayConditions;	
	// 引导结束后的动作表现;
	public List<GuideItemData> DoneActions;
}
```
有了这些结构，我们可以用该结构配置一个json数据，代表一个引导，  
可以看下下面的引导例子  
```Json  
{
    "GuideList":
    [
        {
            "GuideId":101,
            "GuideName":"出来咋到",
            "PreId":-1,
            "PostId":102,
            "Priority":0,
            "ForceGuide":1,
            "SubGuide":
            [
                {
                    "Name":"初来咋到-说明文字1",
                    "Triggers":
                    [
                        {
                            "Type":"EnterScene",
                            "Params":
                            {
                                "SceneName":"MainScene"
                            }
                        }
                    ],
                    "Conditions":[
                        {
                            "Type":"PanelVisible",
                            "Params":
                            {
                                "UIName":"HallPanel"
                            }
                        },
                        {
                            "Type":"PlayerLevel",
                            "Params":
                            {
                                "Value":1
                            }
                        }
                    ],
                    "Actions":
                    [
                        {
                            "Type":"Dialog",
                            "Params":
                            {
                                "DialogType":"Left",
                                "Content":"欢迎{PlayerName}勇士来到{BornName}",
                                "ShowIntervalTime":0.05
                            }
                        }
                    ],
                    "Dones":
                    [
                        {
                            "Type":"ButtonClick",
                            "Params":
                            {
                                "UIName":"GuidePanel",
                                "ButtonName":"dialog"
                            }
                        }
                    ]
                },
                {
                    "Name":"初来咋到-点击装备",
                    "Triggers":
                    [
                        {
                            "Type":"Sequence"
                        }
                    ],
                    "Actions":
                    [
                        {
                            "Type":"ShowButtonAnim",
                            "Params":
                            {
                                "UIName":"HallPanel",
                                "ButtonName":"BtnEquipment"
                            }
                        }
                    ],
                    "Dones":
                    [
                        {
                            "Type":"ButtonClick",
                            "Params":
                            {
                                "UIName":"HallPanel",
                                "ButtonName":"BtnEquipment"
                            }
                        }
                    ]
                }
            ]
        }
    ] 
}
```
### 总结  
在进行引导设计时，首先需要根据需求，进行设计对应的数据结构，最后再进行对应的编码。  
