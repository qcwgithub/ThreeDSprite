using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data;

namespace Script
{
    public class BattleScript
    {
        public static void initBattleScripts(IBattleConfigs configs/* in */, IBattleScripts scripts/* out */)
        {
            
        // btCreateScript createScript { get; set; }
        // btMoveScript moveScript { get; set; }
        // btMainScript mainScript { get; set; }
        // btContactListenerScript contactListenerScript { get; set; }
        // btDestroyScript destroyScript { get; set; }
        // btUpdateScript updateScript { get; set; }

            scripts.createScript = new btCreateScript();
            scripts.createScript.Init(configs, scripts);

            scripts.moveScript = new btMoveScript();
            scripts.moveScript.Init(configs, scripts);

            scripts.mainScript = new btMainScript();
            scripts.mainScript.Init(configs, scripts);

            scripts.contactListenerScript = new btContactListenerScript();
            scripts.contactListenerScript.Init(configs, scripts);

            scripts.destroyScript = new btDestroyScript();
            scripts.destroyScript.Init(configs, scripts);
            
            scripts.updateScript = new btUpdateScript();
            scripts.updateScript.Init(configs, scripts);
        }

        public static void loadMap(JsonUtils jsonUtils, IBattleConfigs configs, int mapId, Func<int, string> loadTmx, Func<string, string> loadTileset)
        {
            btTilemapData mapData;
            if (!configs.tilemapDatas.TryGetValue(mapId, out mapData))
            {
                string tmxText = loadTmx(mapId);
                mapData = jsonUtils.parse<btTilemapData>(tmxText);
                configs.tilemapDatas[mapId] = mapData;
            }

            for (int i = 0; i < mapData.layerDatas.Count; i++)
            {
                btTileLayerData layerData = mapData.layerDatas[i];

                for (int j = 0; j < layerData.tileDatas.Count; j++)
                {
                    btTileData thingData = layerData.tileDatas[j];
                    // string key = Path.GetFileNameWithoutExtension(aThing.tileset);

                    btTilesetConfig tilesetConfig;
                    if (!configs.tilesetConfigs.TryGetValue(thingData.tileset, out tilesetConfig))
                    {
                        string tilesetText = loadTileset(thingData.tileset);
                        tilesetConfig = jsonUtils.parse<btTilesetConfig>(tilesetText);
                        configs.tilesetConfigs.Add(thingData.tileset, tilesetConfig);
                    }
                }
            }
        }
    }
}