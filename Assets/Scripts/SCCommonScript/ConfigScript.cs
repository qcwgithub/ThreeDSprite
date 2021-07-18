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

        static void ParseCharacters(IGameConfigs _this, string text)
        {
            _this.minCharacterConfigId = -1;
            _this.maxCharacterConfigId = -1;
            _this.characterConfigDict = new Dictionary<int, CharacterConfig>();
            
            CsvHelper helper = CsvUtils.Parse(text);
            while (helper.ReadRow())
            {
                CharacterConfig c = new CharacterConfig();
                c.id = helper.ReadInt("id");
                c.name = helper.ReadString("name");
                c.prefab = helper.ReadString("prefab");

                _this.characterConfigDict.Add(c.id, c);

                if (_this.minCharacterConfigId == -1 || c.id < _this.minCharacterConfigId)
                {
                    _this.minCharacterConfigId = c.id;
                }
                if (_this.maxCharacterConfigId == -1 || c.id > _this.maxCharacterConfigId)
                {
                    _this.maxCharacterConfigId = c.id;
                }
            }
        }
    }
}