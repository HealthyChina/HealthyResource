#基于ET框架和UGUI的简易UI框架（et3.3版）

##案例：

namespace ETHotfix
{
    [ObjectSystem]
    public class UILoginComponentAwakeSystem : AwakeSystem<UILoginComponent>
    {
        public override void Awake(UILoginComponent self)
        {
            self.Awake();
        }
    }

    public class UILoginComponent : UIBaseComponent//此处继承自UIBaseComponent而不是Component
    {
        //Awake方法只调用一次，通常用于获取引用，绑定事件，初始化部分道具
        public void Awake()
        {
            //获取ReferenceCollector的引用
            ReferenceCollector rc = this.GetParent<UI_Z>().GameObject.GetComponent<ReferenceCollector>();

            //通过ReferenceCollector获取添加的物体引用
            //此处获取返回按钮的Button组件，并且绑定点击事件
            rc.GetUnityComponent<Button>("ReturnBtn").Add(OnClickReturnBtn);
        }

        //每次Show窗体都会调用，通常用于初始化界面
        public override void Show()
        {
            base.Show();

            //展示界面逻辑
        }

        //关闭时调用
        public override void Close()
        {
            base.Close();

            //关闭界面逻辑
        }

        //点击返回按钮事件
        private void OnClickReturnBtn()
        {
            //点击退出界面逻辑
        }
    }
}

namespace ETHotfix
{
    [UIFactory(UIType_Z.UILogin)]
    public class UILoginFactory : IUIFactory_Z
    {
        public UI_Z Create(Scene scene, string type, GameObject parent)
        {
            try
            {
                ResourcesComponent resourcesComponent = Game.Scene.GetComponent<ResourcesComponent>();
                resourcesComponent.LoadBundle($"{type}.unity3d");
                GameObject bundleGameObject = resourcesComponent.GetAsset<GameObject>($"{type}.unity3d", $"{type}");
                GameObject newUi = UnityEngine.Object.Instantiate(bundleGameObject);
                newUi.layer = LayerMask.NameToLayer(LayerNames.UI);
                UI_Z ui = ComponentFactory.Create<UI_Z, GameObject>(newUi);

                //此处务必使用AddUiComponent代替原本et中的AddComponent否则UI_Z中的UiComponent属性会为空
                ui.AddUiComponent<UILoginComponent>();
                return ui;
            }
            catch (Exception e)
            {
                Log.Error(e.ToStr());
                return null;
            }
        }

        public void Remove(string type)
        {
            Game.Scene.GetComponent<ResourcesComponent>().UnloadBundle($"{type}.unity3d");
        }
    }
}

对本框架更详细的介绍请点击：http://www.tinkingli.com/?p=270

框架作者：渐渐（QQ:598262064）