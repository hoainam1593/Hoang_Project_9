public class MapInfo : IFileStreamObject
{
    public int ModelVersion => 1;

    public int Id;
    public int Star;

    public MapInfo(int id, int star)
    {
        this.Id = id;
        this.Star = star;
    }

    public MapInfo()
    {
        this.Id = 0;
        this.Star = -1;
    }

    public void ReadOrWrite(IFileStream stream, int version)
    {
        stream.ReadOrWriteInt(ref Id, "Id");
        stream.ReadOrWriteInt(ref Star, "Star");
    }
}