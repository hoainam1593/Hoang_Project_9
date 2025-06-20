
using System;
using UnityEngine;

[CreateAssetMenu(fileName = "GameFrameworkConfig", menuName = "Configs/GameFrameworkConfig", order = 1)]
public partial class GameFrameworkConfig : ScriptableObject
{
    public static GameFrameworkConfig instance
    {
        get
        {
            var go = Resources.Load<GameFrameworkConfig>("GameFrameworkConfig");
            if (go == null)
            {
                throw new Exception(
                    "create config Configs/GameFrameworkConfig at folder Resources/GameFrameworkConfig");
            }

            return go;
        }
    }
}