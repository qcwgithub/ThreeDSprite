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
                c.id = helper.ReadInt("id");
                c.name = helper.ReadString("name");
                c.prefab = helper.ReadString("prefab");

                _.characterConfigDict.Add(c.id, c);

                if (_.minCharacterConfigId == -1 || c.id < _.minCharacterConfigId)
                {
                    _.minCharacterConfigId = c.id;
                }
                if (_.maxCharacterConfigId == -1 || c.id > _.maxCharacterConfigId)
                {
                    _.maxCharacterConfigId = c.id;
                }
            }
        }
    }
}