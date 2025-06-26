public class ChapterObj : IFileStreamObject
{
    public int ModelVersion => 1;

    public int Id;
    public int Star;
    public bool IsActive;

    public ChapterObj(int id, int star, bool isActive)
    {
        this.Id = id;
        this.Star = star;
        this.IsActive = isActive;
    }

    public ChapterObj()
    {
        this.Id = 0;
        this.Star = 0;
        this.IsActive = false;
    }

    public void ReadOrWrite(IFileStream stream, int version)
    {
        stream.ReadOrWriteInt(ref Id, "Id");
        stream.ReadOrWriteInt(ref Star, "Star");
        stream.ReadOrWriteBool(ref IsActive,  "IsActive");
    }
}