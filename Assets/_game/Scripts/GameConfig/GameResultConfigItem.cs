using Amazon.S3.Model;
using UnityEngine;

public class GameResultConfigItem : BaseConfigItem
{
    public int playerHpRequiredFor1Star;
    public int playerHpRequiredFor2Star;
    public int playerHpRequiredFor3Star;
    public int bonusGold1;
    public int bonusGold2;
    public int bonusGold3;

    public override void ReadOrWrite(IFileStream stream)
    {
        stream.ReadOrWriteInt(ref playerHpRequiredFor1Star, "player_hp_for_1_star");
        stream.ReadOrWriteInt(ref playerHpRequiredFor2Star, "player_hp_for_2_star");
        stream.ReadOrWriteInt(ref playerHpRequiredFor3Star, "player_hp_for_3_star");
        stream.ReadOrWriteInt(ref bonusGold1, "bonus_gold_1_star");
        stream.ReadOrWriteInt(ref bonusGold2, "bonus_gold_2_star");
        stream.ReadOrWriteInt(ref bonusGold3, "bonus_gold_3_star");
    }
}
