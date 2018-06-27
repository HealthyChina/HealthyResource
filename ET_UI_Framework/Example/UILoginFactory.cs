using System;
using ETModel;
using UnityEngine;

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
