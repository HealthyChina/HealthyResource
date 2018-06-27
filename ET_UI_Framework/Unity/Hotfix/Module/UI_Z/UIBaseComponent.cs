using ETModel;
using System;

namespace ETHotfix
{
    /// <summary>
    /// UI窗体主控组件需要继承此类
    /// </summary>
    public abstract class UIBaseComponent : Component
    {
        public event Action OnCloseOneTime;
        public event Action OnShow;
        public event Action OnClose;

        public bool InShow { get { return Layer != WindowLayer.UIHiden; } }
        public string Layer { get; set; } = WindowLayer.UIHiden;

        public virtual void Show()
        {
            GetParent<UI_Z>().GameObject.SetActive(true);
            OnShow?.Invoke();
        }

        public virtual void Close()
        {
            GetParent<UI_Z>().GameObject.SetActive(false);

            if (OnCloseOneTime != null)
            {
                OnCloseOneTime.Invoke();
                OnCloseOneTime = null;
            }
            OnClose?.Invoke();
        }

        public override void Dispose()
        {
            base.Dispose();

            OnCloseOneTime = null;
            OnShow = null;
            OnClose = null;
            Layer = WindowLayer.UIHiden;
        }
    }
}

