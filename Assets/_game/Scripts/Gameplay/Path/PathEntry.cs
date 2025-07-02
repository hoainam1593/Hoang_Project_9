using System;

[Serializable]
public class PathEntry
{
    public MapCoordinate mapCoordinate;
    public bool isCorner;

    public PathEntry(MapCoordinate coordinate, bool corner)
    {
        mapCoordinate = coordinate;
        isCorner = corner;
    }

    public override string ToString()
    {
        return $"PathEntry({mapCoordinate}, Corner: {isCorner})";
    }

    public override bool Equals(object obj)
    {
        if (obj is PathEntry other)
        {
            return mapCoordinate.Equals(other.mapCoordinate) && isCorner == other.isCorner;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(mapCoordinate.GetHashCode(), isCorner.GetHashCode());
    }
}