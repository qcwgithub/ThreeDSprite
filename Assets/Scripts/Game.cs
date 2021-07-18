using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Script;
using Data;

public class Game : IProfileInput, IGameConfigs, IGameScripts
{
    //// IProfileInput
    public Profile profile { get; set; }

    //// IGameConfigs
    public int minCharacterConfigId { get; set; }
    public int maxCharacterConfigId { get; set; }
    public Dictionary<int, CharacterConfig> characterConfigDict { get; set; }
    public CharacterConfig GetCharacterConfig(int characterId)
    {
        CharacterConfig characterConfig;
        return this.characterConfigDict.TryGetValue(characterId, out characterConfig) ? characterConfig : null;
    }

    //// IGameScritps
    public SCUtils scUtils { get; private set; }
    public GameScript gameScript { get; private set; }

    public void Load(Func<string, string> loadFileText)
    {
        ConfigScript.Load(this, loadFileText);

        this.gameScript = new GameScriptClient();
        this.gameScript.Init(this, this);
    }
}
