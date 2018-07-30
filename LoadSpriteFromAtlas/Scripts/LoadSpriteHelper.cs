using System.Collections.Generic;
using UnityEngine;

public static class LoadSpriteHelper
{
    //所有图集名
    public const string Atl_UI = "UI";
    public const string Atl_Scene = "Scene";
    public const string Atl_Role = "Role";

    /// <summary>
    /// SpriteData缓存
    /// </summary>
    private static Dictionary<string, SpriteData> spriteDates = new Dictionary<string, SpriteData>();

    /// <summary>
    /// 获取图片（自动加载缺失的图集）
    /// </summary>
    /// <param name="atlName"></param>
    /// <param name="spriteName"></param>
    /// <returns></returns>
    public static Sprite LoadSprite(string atlName, string spriteName)
    {
        if (!spriteDates.ContainsKey(atlName))
        {
            GameObject atl = null;//todo 此处加载atl_开头的预制体
            spriteDates.Add(atlName, atl.GetComponent<SpriteData>());
        }
        return spriteDates[atlName].GetSprite(spriteName);
    }
}