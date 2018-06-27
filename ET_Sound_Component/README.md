音效组件，简单易用

在EditorCreateAudio.cs类中有两个字段，根据自己项目需求，进行设置

//音效资源路径
private static string audiosDir = "assets/ETAB/audios";
//导出预制体路径
private static string prefabDir = "assets/ETAB/SoundPrefabs";

音效导入到audiosDir路径下，然后点击菜单项Tools->创建音效预设，即可看到prefabDir路径下生成的音效预制体
可以根据需求修改预制体上SoundData的属性，也可以根据项目需求在SoundData类中拓展属性和字段

在ETHotfix.SoundName.cs中添加音效条目

测试代码：

ETModel.Game.Scene.AddComponent<SoundComponent>();

ETModel.Game.Scene.GetComponent<SoundComponent>().PlayClip(SoundName.NormalBtn);

ETModel.Game.Scene.GetComponent<SoundComponent>().PlayMusic(SoundName.MainBgm);

对本组件更详细的介绍请点击：http://www.tinkingli.com/?p=136

音效组件作者：渐渐（QQ:598262064）