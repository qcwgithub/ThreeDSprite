using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    // 在 layer 上摆放的一个东西
    public class btTileData
    {
        public int id;

        // 东西是啥
        public string tileset;
        public int tileId; // tile id in tileset

        // 坐标是啥，左下角
        // transform.position
        public FVector3 position;
    }
}