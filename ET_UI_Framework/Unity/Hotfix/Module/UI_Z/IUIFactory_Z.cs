using UnityEngine;

namespace ETHotfix
{
	public interface IUIFactory_Z
	{
		UI_Z Create(Scene scene, string type, GameObject parent);
		void Remove(string type);
	}
}