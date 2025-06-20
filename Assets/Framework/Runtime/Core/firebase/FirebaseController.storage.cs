#if USE_SERVER_AWS

using System;
using System.IO;
using Cysharp.Threading.Tasks;
using Firebase.Storage;
using UnityEngine.Events;

public partial class FirebaseController
{
    #region upload/download

    public async UniTask StorageUploadFile(string filename, Stream fileContent)
    {
        var storage = FirebaseStorage.DefaultInstance;
        var fileRef = storage.RootReference.Child(filename);
        fileContent.Position = 0;
        await fileRef.PutStreamAsync(fileContent);
    }

    public async UniTask StorageDownloadFile(string filename, UnityAction<Stream> callback)
    {
        try
        {
            var storage = FirebaseStorage.DefaultInstance;
            var fileRef = storage.RootReference.Child(filename);
            var stream = await fileRef.GetStreamAsync();
            stream.Position = 0;
            callback?.Invoke(stream);
            stream.Close();
        }
        catch (Exception e)
        {
            if (IsObjectNotFoundException(e))
            {
                callback?.Invoke(null);
                return;
            }

            StaticUtils.RethrowException(e);
        }
    }

    #endregion

    #region check exist

    public async UniTask<bool> StorageFileExists(string path)
    {
        var storage = FirebaseStorage.DefaultInstance;
        var fileRef = storage.RootReference.Child(path);
        try
        {
            await fileRef.GetMetadataAsync();
            return true;
        }
        catch (Exception e)
        {
            if (IsObjectNotFoundException(e))
            {
                return false;
            }

            StaticUtils.RethrowException(e);
            return false;
        }
    }

    public async UniTask<bool> StorageFolderExists(string path)
    {
        await UniTask.CompletedTask;
        
        //firebase has an API call "List", we can use this to check a folder exists,
        //this API is available on all platforms, but UNITY 
        throw new Exception("not supported");
    }

    #endregion

    #region utils

    private bool IsObjectNotFoundException(Exception e)
    {
        if (e is StorageException storageException)
        {
            if (storageException.ErrorCode == StorageException.ErrorObjectNotFound)
            {
                return true;
            }
        }

        return false;
    }

    #endregion
}

#endif