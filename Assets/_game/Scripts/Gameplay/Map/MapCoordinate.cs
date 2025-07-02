public class MapCoordinate
{
    public static MapCoordinate oneNegative = new MapCoordinate(-1, -1);
        
    public int x;
    public int y;

    public MapCoordinate(MapCoordinate other)
    {
        this.x = other.x;
        this.y = other.y;
    }

    public MapCoordinate(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public override string ToString()
    {
        return $"({x},{y})";
    }
    
    public static MapCoordinate operator +(MapCoordinate a, MapCoordinate b)
    {
        return new  MapCoordinate(a.x + b.x, a.y + b.y);
    }
    
    public static bool operator ==(MapCoordinate a, MapCoordinate b)
    {
        if (ReferenceEquals(a, b)) return true;
        if (a is null || b is null) return false;

        return a.x == b.x && a.y == b.y;
    }

    public static bool operator !=(MapCoordinate a, MapCoordinate b)
    {
        return !(a == b);
    }

    public override bool Equals(object obj)
    {
        if (obj is MapCoordinate other)
            return this == other;

        return false;
    }

    public override int GetHashCode()
    {
        return (x, y).GetHashCode();
    }

    public static MapCoordinate operator-(MapCoordinate a, MapCoordinate b)
    {
        return new MapCoordinate(a.x - b.x, a.y - b.y);
    }
}