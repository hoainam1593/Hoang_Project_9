
#if USE_SERVER_AWS

using System.IO;
using System.Text;
using Cysharp.Threading.Tasks;
using UnityEngine.Events;

public partial class ServerAWS
{
    #region game content

    public async UniTask<string> GameContent_get(string key)
    {
        var rootUrl = GameFrameworkConfig.instance.awsS3GameContentUrl;
        var url = $"{rootUrl}/{key}";

        var res = await StaticUtils.GetHttpRequest(url, true);
        return res.resultAsText;
    }

    public async UniTask GameContent_download(string key, string path)
    {
        var rootUrl = GameFrameworkConfig.instance.awsS3GameContentUrl;
        var url = $"{rootUrl}/{key}";
        
        var res = await StaticUtils.GetHttpRequest(url, false);
        path = $"{path}/{Path.GetFileName(key)}";
        StaticUtils.WriteBinaryFile(path, res.resultAsBinary, true);
    }

    public async UniTask GameContent_set(string key, string value)
    {
        var cfg = GameFrameworkConfig.instance;
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(value));
        var path = $"{cfg.awsS3GameContentRoot}/{key}";
        await UploadFileToS3(s3Client, stream, FileType.Text, path);
    }

    public async UniTask GameContent_set(string key, byte[] value)
    {
        var cfg = GameFrameworkConfig.instance;
        var stream = new MemoryStream(value);
        var path = $"{cfg.awsS3GameContentRoot}/{key}";
        await UploadFileToS3(s3Client, stream, FileType.Binary, path);
    }

    public async UniTask GameContent_applySet()
    {
        await InvalidateCloudFront(cloudFrontClient);
    }

    #endregion

    #region player data

    public async UniTask PlayerData_get(string userId, string playerModelName, UnityAction<Stream> callback)
    {
        var path = $"{userId}/{playerModelName}.bin";
        await FirebaseController.instance.StorageDownloadFile(path, callback);
    }

    public async UniTask PlayerData_set(string userId, string playerModelName, Stream modelContent)
    {
        var path = $"{userId}/{playerModelName}.bin";
        await FirebaseController.instance.StorageUploadFile(path, modelContent);
    }

    public async UniTask<bool> PlayerData_exists(string userId, string playerModelName)
    {
        var path = $"{userId}/{playerModelName}.bin";
        return await FirebaseController.instance.StorageFileExists(path);
    }

    #endregion
}

#endif