using UnityEngine;

public class PlayerConfigItem : BaseConfigItem
{
    public int hp;
    public int coin;

    public override void ReadOrWrite(IFileStream stream)
    {
        stream.ReadOrWriteInt(ref hp, "hp");
        stream.ReadOrWriteInt(ref coin, "coin");
    }
}
