using UnityEngine;


public class GEventData
{
    public struct MapSizeData
    {
        public float Width;
        public float Height;
        public Vector3 BottomLeftPoint;

        public MapSizeData(float width, float height, Vector3 bottomLeftPoint)
        {
            Width = width;
            Height = height;
            BottomLeftPoint = bottomLeftPoint;
        }
    }
}
