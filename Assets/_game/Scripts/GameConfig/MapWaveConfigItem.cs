using NUnit.Framework.Constraints;
using System.Collections.Generic;

/// <summary>
/// Configuration item for map waves containing wave data
/// </summary>
public class MapWaveConfigItem : BaseConfigItem
{
    public int mapId;
    public List<WaveConfigItem> waves = new List<WaveConfigItem>();

    public override void ReadOrWrite(IFileStream stream)
    {
        stream.ReadOrWriteInt(ref mapId, "map_id");
        stream.ReadOrWriteListObj(ref waves, "wave_id");
    }

    public override string ToString()
    {
        var str = $"{{map_id: {mapId}, waves: {waves.Count}";
        str += "[\n";
        foreach (var wave in waves)
        {
            str += $"\n  {wave}";
        }
        str += "\n]";
        return str;
    }
}