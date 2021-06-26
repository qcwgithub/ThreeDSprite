using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TiledCoordConverter
{
    public int pixelsPerTileX;
    public int pixelsPerTileYZ;

    public Vector3Int originTile;

    public float ConvertLengthX(int tilesX)
    {
        return (tilesX * this.pixelsPerTileX) / btConstants.pixels_per_unit;
    }

    public float ConvertLengthY(int tilesY)
    {
        return (tilesY * this.pixelsPerTileYZ * btConstants.sqrt2) / btConstants.pixels_per_unit;
    }

    public float ConvertLengthZ(int tilesZ)
    {
        return (tilesZ * this.pixelsPerTileYZ * btConstants.sqrt2) / btConstants.pixels_per_unit;
    }

    public float ConvertCoordX(int tileX)
    {
        return this.ConvertLengthX(tileX - this.originTile.x);
    }

    public float ConvertCoordY(int tileY)
    {
        return this.ConvertLengthY(tileY - this.originTile.y);
    }

    public float ConvertCoordZ(int tileZ)
    {
        return -this.ConvertLengthZ(tileZ - this.originTile.z);
    }
}
