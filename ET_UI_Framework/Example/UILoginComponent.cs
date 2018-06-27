using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ETModel;
using UnityEngine.UI;

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