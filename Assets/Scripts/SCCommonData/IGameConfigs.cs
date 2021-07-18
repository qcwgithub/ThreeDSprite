using System.Collections.Generic;

namespace Data
{
    public interface IGameConfigs
    {
        int minCharacterConfigId { get; set; }
        int maxCharacterConfigId { get; set; }
        Dictionary<int, CharacterConfig> characterConfigDict { get; set; }
        CharacterConfig GetCharacterConfig(int characterId);
    }
}