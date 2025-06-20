
using System;
using System.Collections.Generic;
using System.IO;
using Cysharp.Threading.Tasks;

public class PlayerModelManager : SingletonMonoBehaviour<PlayerModelManager>
{
#if (UNITY_EDITOR && !EDITOR_USE_BINARY_MODEL) || (!UNITY_EDITOR && UNITY_STANDALONE)
    private IPlayerModelFile modelFile = new PlayerModelFile_text();
#else
	private IPlayerModelFile modelFile = new PlayerModelFile_binary();
#endif

    private List<BasePlayerModel> lModels;
    public string DefaultModelName { get; set; }

    #region list models

    public List<string> GetListModelNames()
    {
        var l = new List<string>();
        foreach (var i in lModels)
        {
            l.Add(i.GetType().Name);
        }
        return l;
    }

    public BasePlayerModel GetPlayerModel(string modelName)
    {
        BasePlayerModel result = null;
        foreach (var i in lModels)
        {
            if (i.GetType().Name == modelName)
            {
                if (result == null)
                {
                    result = i;
                }
                else
                {
                    throw new Exception($"there're more than 1 {modelName} in lModels");
                }
            }
        }
        if (result != null)
        {
            return result;
        }

        throw new Exception($"there's no {modelName} in lModels");
    }

    public BasePlayerModel GetPlayerModel(Type type)
    {
        return GetPlayerModel(type.Name);
    }

    public T GetPlayerModel<T>() where T : BasePlayerModel
    {
        return (T)GetPlayerModel(typeof(T));
    }

    #endregion

    #region save/load locally

    //defaultModelName: to check player data existed on server or not,
    //we will check the path [player id]/[defaultModelName] is valid or not
    public void LoadAllModels(List<BasePlayerModel> lModels, string defaultModelName)
    {
        this.lModels = lModels;
        DefaultModelName = defaultModelName;
        
        foreach (var i in lModels)
        {
            var path = GetModelFilePath(i);
            if (StaticUtils.CheckFileExist(path))
            {
                modelFile.ReadModel(path, i);
            }
            else
            {
                i.OnModelInitializing();
                SaveModel(i);
            }
            i.OnModelLoaded();
        }
    }

    public void SaveModel(BasePlayerModel model)
    {
        var path = GetModelFilePath(model);
        modelFile.WriteModel(path, model);
    }

    public void SaveAllModel()
    {
        foreach (var i in lModels)
        {
            SaveModel(i);
        }
    }

    #endregion

    #region save/load remotely

    public async UniTask UploadPlayerModel()
    {
        if (!ServerController.instance.IsLoggedIn)
        {
            return;
        }

        var lTasks = new List<UniTask>();
        var userId = ServerController.instance.userUID.Value;
        var binModel = new PlayerModelFile_binary();
        foreach (var model in lModels)
        {
            lTasks.Add(UploadPlayerModel(userId, model, binModel));
        }

        await UniTask.WhenAll(lTasks);
    }

    private async UniTask UploadPlayerModel(string userId, BasePlayerModel model, PlayerModelFile_binary binModel)
    {
        var stream = new MemoryStream();

        binModel.WriteModel(stream, model);
        await ServerController.instance.PlayerData_set(userId, model.GetType().Name, stream);
        
        stream.Close();
    }

    public async UniTask<T> DownloadPlayerModel<T>() where T : BasePlayerModel, new()
    {
        var userId = ServerController.instance.userUID.Value;
        var binModel = new PlayerModelFile_binary();
        var model = new T();

        await ServerController.instance.PlayerData_get(userId, typeof(T).Name,
            stream => { binModel.ReadModel(stream, model); });

        return model;
    }

    public async UniTask DownloadPlayerModel()
    {
        var lTasks = new List<UniTask>();
        var userId = ServerController.instance.userUID.Value;
        var binModel = new PlayerModelFile_binary();
        foreach (var model in lModels)
        {
            lTasks.Add(DownloadPlayerModel(userId, model, binModel));
        }

        await UniTask.WhenAll(lTasks);

        SaveAllModel();
        GameReloader.instance.Reload();
    }

    private async UniTask DownloadPlayerModel(string userId, BasePlayerModel model, PlayerModelFile_binary binModel)
    {
        await ServerController.instance.PlayerData_get(userId, model.GetType().Name, stream =>
        {
            if (stream != null)
            {
                binModel.ReadModel(stream, model);
            }
        });
    }

    #endregion

    #region utils

    public void ClearAllModels()
    {
        foreach (var i in lModels)
        {
            var path = GetModelFilePath(i);
            StaticUtils.DeleteFile(path);
        }
    }

    public static string GetModelFolderPath()
    {
#if UNITY_EDITOR
        return "../PlayerModels";
#elif UNITY_STANDALONE
	    return "../../PlayerModels";
#else
	    return "PlayerModels";
#endif
    }

    private string GetModelFilePath(BasePlayerModel model)
    {
        var folder = GetModelFolderPath();
        var filename = $"{model.GetType().Name}.{modelFile.Extension}";
        return $"{folder}/{filename}";
    }

    #endregion
}