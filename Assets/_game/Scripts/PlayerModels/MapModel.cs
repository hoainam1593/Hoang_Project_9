using UnityEngine;
using System.Collections.Generic;

public class MapModel : BasePlayerModel
{
    public override int ModelVersion => 1;

    public int MapCount;
    public List<ChapterObj> Chapters;


    public override void ReadOrWrite(IFileStream stream, int version)
    {
        stream.ReadOrWriteInt(ref MapCount, "MapCount");
        stream.ReadOrWriteListObj<ChapterObj>(ref Chapters, "Chapters");
    }

    public override void OnModelInitializing()
    {
        var mapConfig = ConfigManager.instance.GetConfig<MapConfig>();
        
        MapCount = mapConfig.listConfigItems.Count;
        Chapters = new List<ChapterObj>();
        
        foreach (var item in mapConfig.listConfigItems)
        {
            Chapters.Add(new ChapterObj(item.mapId, -1));
        }

        Chapters[0].Star = 0;
    }
}
