using UnityEngine;
using UnityEngine.Tilemaps;

public static class TilemapHelper
{
    public static void RotateTile(this Tilemap tilemap, Vector3Int cellPos, float angle)
    {
        tilemap.SetTileFlags(cellPos, TileFlags.None);
        
        Matrix4x4 rotateMatrix = Matrix4x4.TRS(Vector3.zero, Quaternion.Euler(0, 0, angle), tilemap.transform.lossyScale);
        
        tilemap.SetTransformMatrix(cellPos, rotateMatrix);
        
        tilemap.SetTileFlags(cellPos, TileFlags.LockTransform);
    }
}