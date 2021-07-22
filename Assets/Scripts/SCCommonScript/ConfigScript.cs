using System;
using System.Collections.Generic;
using Data;

namespace Script
{
    public class ConfigScript
    {
        public static void Load(IGameConfigs _this, Func<string, string> loadFileText)
        {
            ParseCharacters(_this, loadFileText("characters.csv"));
        }

        static void ParseCharacters(IGameConfigs _, string text)
        {
            _.minCharacterConfigId = -1;
            _.maxCharacterConfigId = -1;
            _.characterConfigDict = new Dictionary<int, CharacterConfig>();
            
            CsvHelper helper = CsvUtils.Parse(text);
            while (helper.ReadRow())
            {
                CharacterConfig c = new CharacterConfig();
                c.characterId = helper.ReadInt("characterId");
                c.name = helper.ReadString("name");
                c.prefab_ui = helper.ReadString("prefab_ui");
                c.prefab_battle = helper.ReadString("prefab_battle");

                _.characterConfigDict.Add(c.characterId, c);

                if (_.minCharacterConfigId == -1 || c.characterId < _.minCharacterConfigId)
                {
                    _.minCharacterConfigId = c.characterId;
                }
                if (_.maxCharacterConfigId == -1 || c.characterId > _.maxCharacterConfigId)
                {
                    _.maxCharacterConfigId = c.characterId;
                }
            }
        }
    }
}