using System;
using System.Collections.Generic;
using System.Linq;
using ETModel;
using UnityEngine;
using UnityEngine.UI;

namespace ETHotfix
{
	[ObjectSystem]
	public class UiComponentAwakeSystem : AwakeSystem<UIComponent_Z>
	{
		public override void Awake(UIComponent_Z self)
		{
			self.Awake();
		}
	}

	[ObjectSystem]
	public class UiComponentLoadSystem : LoadSystem<UIComponent_Z>
	{
		public override void Load(UIComponent_Z self)
		{
			self.Load();
		}
	}

	/// <summary>
	/// 管理所有UI
	/// </summary>
	public class UIComponent_Z: Component
	{
	    private GameObject Root;
	    private Dictionary<string, IUIFactory_Z> UiTypes;
	    private readonly Dictionary<string, UI_Z> uis = new Dictionary<string, UI_Z>();

	    private Dictionary<string, GameObject> m_allLayers = new Dictionary<string, GameObject>();

        public override void Dispose()
        {
            if (this.IsDisposed)
            {
                return;
            }

            foreach (string type in uis.Keys.ToArray())
            {
                UI_Z ui;
                if (!uis.TryGetValue(type, out ui))
                {
                    continue;
                }
                uis.Remove(type);
                ui.Dispose();
            }

            base.Dispose();
        }

        public void Awake()
        {
            this.Root = GameObject.Find("Global/UI/");
            this.InstantiateUi(Root.transform);
            this.Load();
        }


        public void Load()
        {
            UiTypes = new Dictionary<string, IUIFactory_Z>();

            foreach (Type type in ETModel.Game.Hotfix.GetHotfixTypes())
            {
                object[] attrs = type.GetCustomAttributes(typeof(UIFactoryAttribute), false);
                if (attrs.Length == 0)
                {
                    continue;
                }

                UIFactoryAttribute attribute = attrs[0] as UIFactoryAttribute;
                if (UiTypes.ContainsKey(attribute.Type))
                {
                    Log.Debug($"已经存在同类UI Factory: {attribute.Type}");
                    throw new Exception($"已经存在同类UI Factory: {attribute.Type}");
                }
                object o = Activator.CreateInstance(type);
                IUIFactory_Z factory = o as IUIFactory_Z;
                if (factory == null)
                {
                    Log.Error($"{o.GetType().FullName} 没有继承 IUIFactory");
                    continue;
                }
                this.UiTypes.Add(attribute.Type, factory);
            }
        }

        /// <summary>
        /// 初始化UI设置，建立层级结构
        /// </summary>
        /// <param name="parent"></param>
        private void InstantiateUi(Transform parent)
        {
            //此处务必按照显示层级由低到高排序
            string[] _names = new String[] {
                WindowLayer.UIHiden,
                WindowLayer.Bottom,
                WindowLayer.Medium,
                WindowLayer.Top,
                WindowLayer.TopMost
            };

            Camera _cam = new GameObject().AddComponent<Camera>();
            _cam.clearFlags = CameraClearFlags.Depth;
            _cam.cullingMask = 1 << LayerMask.NameToLayer("UI");
            _cam.orthographic = true;
            _cam.depth = 10;
            _cam.name = "UiCamera";
            _cam.transform.SetParent(parent);
            _cam.transform.localPosition = Vector3.zero;

            for (int i = 0; i < _names.Length; i++)
            {
                GameObject _go = new GameObject();
                this.m_allLayers.Add(_names[i], _go);
                Canvas _canvas = _go.AddComponent<Canvas>();
                _canvas.renderMode = RenderMode.ScreenSpaceCamera;
                _canvas.worldCamera = _cam;
                _canvas.sortingOrder = i;
                CanvasScaler _scale = _go.AddComponent<CanvasScaler>();
                _scale.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
                _scale.referenceResolution = new Vector2(1920, 1080);
                _scale.matchWidthOrHeight = 0;
                GraphicRaycaster _graphic = _go.AddComponent<GraphicRaycaster>();
                _go.name = _names[i];
                _go.transform.SetParent(parent);
                _go.transform.localPosition = Vector3.zero;
                if (_names[i] == WindowLayer.UIHiden)
                {
                    _go.layer = LayerMask.NameToLayer("UIHiden");
                    _graphic.enabled = false;
                }
                else
                {
                    _go.layer = LayerMask.NameToLayer("UI");
                }
            }
        }

        /// <summary>
        /// 创建ui
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public UI_Z Create(string type)
        {
            try
            {
                UI_Z ui;

                if (uis.ContainsKey(type))
                {
                    ui = uis[type];
                }
                else
                {
                    ui = UiTypes[type].Create(this.GetParent<Scene>(), type, Root);
                    uis.Add(type, ui);
                }

                // 设置canvas
                SetViewParent(ui, ui.GameObject.GetComponent<UIPanelConfig>().WindowLayer);
                ui.UiComponent.Show();

                return ui;
            }
            catch (Exception e)
            {
                throw new Exception($"{type} UI 错误: {e.ToStr()}");
            }
        }

        /// <summary>
        /// 关闭ui
        /// </summary>
        /// <param name="type"></param>
        public void Close(string type)
        {
            UI_Z ui;
            if (!uis.TryGetValue(type, out ui))
            {
                return;
            }
            uis[type].UiComponent.Close();
            SetViewParent(uis[type], WindowLayer.UIHiden);
        }

        /// <summary>
        /// 设置ui显示层级
        /// </summary>
        /// <param name="ui"></param>
        /// <param name="layer"></param>
        private void SetViewParent(UI_Z ui, string layer)
        {
            RectTransform _rt = ui.GameObject.GetComponent<RectTransform>();
            _rt.SetParent(m_allLayers[layer].transform);
            _rt.anchorMin = Vector2.zero;
            _rt.anchorMax = Vector2.one;
            _rt.offsetMax = Vector2.zero;
            _rt.offsetMin = Vector2.zero;
            _rt.pivot = new Vector2(0.5f, 0.5f);

            _rt.localScale = Vector3.one;
            _rt.localPosition = Vector3.zero;
            _rt.localRotation = Quaternion.identity;

            ui.UiComponent.Layer = layer;
        }

        /// <summary>
        /// 移除窗体
        /// </summary>
        /// <param name="type"></param>
        public void Remove(string type)
        {
            UI_Z ui;
            if (!uis.TryGetValue(type, out ui))
            {
                return;
            }
            //如果没有关闭，先关闭再移除
            if (ui.UiComponent.InShow)
            {
                Close(type);
            }

            UiTypes[type].Remove(type);
            uis.Remove(type);
            ui.Dispose();
        }

        public void RemoveAll()
        {
            foreach (string type in this.uis.Keys.ToArray())
            {
                Remove(type);
            }
        }

        public UI_Z Get(string type)
        {
            UI_Z ui;
            this.uis.TryGetValue(type, out ui);
            return ui;
        }

        public List<string> GetUITypeList()
        {
            return new List<string>(this.uis.Keys);
        }
    }
}