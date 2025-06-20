
#if USE_FIREBASE_AUTHENTICATION

using System;
using Cysharp.Threading.Tasks;
using R3;
using UnityEngine;

public partial class ServerController
{
    public ReactiveProperty<string> userUID = new();

    public bool IsLoggedIn => !string.IsNullOrEmpty(userUID.Value);

    public async UniTask<bool> Login(bool syncData)
    {
        try
        {
            userUID.Value = await FirebaseController.instance.SignIn();
            Debug.Log($"[Firebase] login successful with id={userUID.Value}");

            if (syncData)
            {
                if (await IsPlayerDataExist())
                {
                    await PlayerModelManager.instance.DownloadPlayerModel();
                }
                else
                {
                    await PlayerModelManager.instance.UploadPlayerModel();
                }
            }
            
            return true;
        }
        catch (Exception e)
        {
            Debug.LogException(e);
            return false;
        }
    }

    private async UniTask<bool> IsPlayerDataExist()
    {
        var defaultModelName = PlayerModelManager.instance.DefaultModelName;
        return await PlayerData_exists(userUID.Value, defaultModelName);
    }

    public async UniTask Logout()
    {
        await PlayerModelManager.instance.UploadPlayerModel();
        await FirebaseController.instance.SignOut();
        PlayerModelManager.instance.ClearAllModels();
        GameReloader.instance.Reload();
    }
}

#endif