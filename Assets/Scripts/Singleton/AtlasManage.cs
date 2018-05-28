using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtlasManage : Singleton<AtlasManage> {

    private Dictionary<string, Object[]> atlasDic;//图集的集合  
    void Awake()
    {
        initData();
    }
    private void initData()
    {
        atlasDic = new Dictionary<string, Object[]>();
    }
    // Use this for initialization  
    void Start()
    {
    }
    //加载图集上的一个精灵  
    public Sprite LoadAtlasSprite(string spriteAtlasPath, string spriteName)
    {
        Sprite sprite = FindSpriteFormBuffer(spriteAtlasPath, spriteName);
        if (sprite == null)
        {
            Object[] atlas = Resources.LoadAll(spriteAtlasPath);
            atlasDic.Add(spriteAtlasPath, atlas);
            sprite = SpriteFormAtlas(atlas, spriteName);
        }
        return sprite;
    }
    //删除图集缓存  
    public void DeleteAtlas(string spriteAtlasPath)
    {
        if (atlasDic.ContainsKey(spriteAtlasPath))
        {
            atlasDic.Remove(spriteAtlasPath);
        }
    }
    //从缓存中查找图集，并找出sprite  
    private Sprite FindSpriteFormBuffer(string spriteAtlasPath, string spriteName)
    {
        if (atlasDic.ContainsKey(spriteAtlasPath))
        {
            Object[] atlas = atlasDic[spriteAtlasPath];
            Sprite sprite = SpriteFormAtlas(atlas, spriteName);
            return sprite;
        }
        return null;
    }
    //从图集中，并找出sprite  
    private Sprite SpriteFormAtlas(Object[] atlas, string spriteName)
    {
        for (int i = 0; i < atlas.Length; i++)
        {
            if (atlas[i].GetType() == typeof(UnityEngine.Sprite))
            {
                if (atlas[i].name == spriteName)
                {
                    return (Sprite)atlas[i];
                }
            }
        }
        Debug.LogWarning("图片名:" + spriteName + ";在图集中找不到");
        return null;
    }
}
