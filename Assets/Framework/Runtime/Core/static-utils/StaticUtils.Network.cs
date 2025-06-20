
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static partial class StaticUtils
{
    //if you got InvalidOperationException: Insecure connection not allowed,
    //Go to player settings -> other settings -> configuration -> allow downloads over HTTP -> always allowed
    
    #region get request

    public static async UniTask<HttpGetResult> GetImageHttpRequest(string url)
    {
        var req = UnityWebRequestTexture.GetTexture(url);
        await req.SendWebRequest();

        var result = new HttpGetResult() { url = url };
        var texture = DownloadHandlerTexture.GetContent(req);
        result.resultAsBinary = texture.EncodeToPNG();

        req.Dispose();
        return result;
    }

    public static async UniTask<HttpGetResult> GetHttpRequest(string url, bool returnText)
    {
        var req = UnityWebRequest.Get(url);
        var op = await req.SendWebRequest();

        var result = new HttpGetResult() { url = url };
        if (returnText)
        {
            result.resultAsText = op.downloadHandler.text;
        }
        else
        {
            result.resultAsBinary = op.downloadHandler.data;
        }

        req.Dispose();
        return result;
    }

    #endregion
    
    #region post request

    public static async UniTask<string> PostHttpRequest(string url, Dictionary<string, string> form)
    {
        var req = UnityWebRequest.Post(url, form);
        try
        {
            var op = await req.SendWebRequest();
            return op.downloadHandler.text;
        }
        catch (UnityWebRequestException e)
        {
            throw new HttpPostException(e);
        }
        finally
        {
            req.Dispose();
        }
    }

    public static async UniTask<string> PostHttpRequest(string url, string json)
    {
        var req = UnityWebRequest.Put(url, json);
        req.SetRequestHeader("Content-Type", "application/json");
        try
        {
            var op = await req.SendWebRequest();
            return op.downloadHandler.text;
        }
        catch (UnityWebRequestException e)
        {
            throw new HttpPostException(e);
        }
        finally
        {
            req.Dispose();
        }
    }

    public static async UniTask<string> PostUploadHttpRequest(string url, byte[] data,
        Dictionary<string, string> headers, bool overwrite)
    {
        var req = new UnityWebRequest(url, overwrite ? "PATCH" : "POST");
        req.uploadHandler = new UploadHandlerRaw(data);
        req.downloadHandler = new DownloadHandlerBuffer();
        foreach (var i in headers)
        {
            req.SetRequestHeader(i.Key, i.Value);
        }

        try
        {
            var op = await req.SendWebRequest();
            return op.downloadHandler.text;
        }
        catch (UnityWebRequestException e)
        {
            throw new HttpPostException(e);
        }
        finally
        {
            req.Dispose();
        }
    }

    #endregion
}
