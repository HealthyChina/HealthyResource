using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode()]
public class SpriteData : MonoBehaviour
{
    public Sprite[] sprites;

    private Dictionary<string, int> Index = new Dictionary<string, int>();
    private int indexS = 0;

    public Sprite[] SetSP
    {
        set
        {
            sprites = value;
        }
    }

    public Sprite GetSprite(string spName)
    {
        if (Index.Count == 0)
        {
            for (int i = 0; i < sprites.Length; i++)
            {
                Index.Add(sprites[i].name, i);
            }
        }
        if (Index.TryGetValue(spName, out indexS))
        {
            return sprites[indexS];
        }
        return null;
    }
}