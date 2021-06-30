using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Data
{
    public class btTileLayerData
    {
        public int id;
        public string name;
        public FVector3 offset;
        public btObjectType objectType;

        public List<btTileData> tileDatas;

        // when objectType == stair
        public StairDir stairDir;
    }
}