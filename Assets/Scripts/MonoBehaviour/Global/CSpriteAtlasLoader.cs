using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

// 暂时用不上
// 这个是当 sprite atlas.includesInBuild = false 时使用，延迟绑定
// see https://docs.unity3d.com/2019.4/Documentation/Manual/LateBinding.html
public class CSpriteAtlasLoader : MonoBehaviour
{
    void Awake()
    {
        DontDestroyOnLoad(this.gameObject);

        SpriteAtlasManager.atlasRequested += this.onAtlasRequest;
        SpriteAtlasManager.atlasRegistered += this.onAtlasRegistered;
    }

    void onAtlasRequest(string atlasName, Action<SpriteAtlas> action)
    {
        // 1 load from Resources
        // 2 from asset bundle
    }

    void onAtlasRegistered(SpriteAtlas registeredAtlas)
    {

    }
}
