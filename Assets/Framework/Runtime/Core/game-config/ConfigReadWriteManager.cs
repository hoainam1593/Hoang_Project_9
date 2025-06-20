
using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class ConfigReadWriteManager
{
    #region data member

    public List<IBaseConfig> listConfigs;

    public ConfigReadWriteManager(List<IBaseConfig> listConfigs)
    {
        this.listConfigs = listConfigs;
    }

    #endregion

    #region read

    public void ReadConfig_editor()
    {
        foreach (var cfg in listConfigs)
        {
            var cfgPath = $"../GameConfig/{cfg.GetType().Name}.csv";
            StaticUtils.OpenFileForRead(cfgPath, stream =>
            {
                LoadCfgFromStream(stream, cfg);
            });
        }
    }

    public async UniTask ReadConfig_standalone()
    {
        foreach (var cfg in listConfigs)
        {
            var cfgName = cfg.GetType().Name;

#if USE_SERVER_GAME_CONFIG
            var cfgPath = $"../../GameConfig/{cfgName}.csv";
            var cfgTxt = StaticUtils.ReadTextFile(cfgPath);
#else
            var cfgPath = $"GameConfig/{cfgName}.csv";
            var cfgTxt = await StaticUtils.GetStreamingFileText(cfgPath);
#endif
            using (var stream = new StringStream(cfgTxt))
            {
                LoadCfgFromStream(stream.GetReader(), cfg);
            }
        }
    }

    public async UniTask ReadConfig_mobile()
    {
        const string cfgPath = "GameConfig/all_config.bin";

#if USE_SERVER_GAME_CONFIG
        var cfgBin = StaticUtils.ReadBinaryFile(cfgPath);
#else
        var cfgBin = await StaticUtils.GetStreamingFileBinary(cfgPath);
#endif

        using (var stream = new MemoryStream(cfgBin))
        {
            var reader = new BinaryReader(stream);
            var filestream = new FileStream_binaryReader(reader);
            foreach (var cfg in listConfigs)
            {
                var numItems = 0;
                try
                {
                    filestream.ReadOrWriteInt(ref numItems);
                    cfg.Read(filestream, numItems);
                }
                catch (Exception e)
                {
                    var cfgName = cfg.GetType().Name;
                    Debug.LogError($"[Config] read config {cfgName} failed");
                    StaticUtils.RethrowException(e);
                }
            }
        }
    }

    private void LoadCfgFromStream(StreamReader reader, IBaseConfig cfg)
    {
        try
        {
            var filestream = new FileStream_csvReader(reader);
            cfg.Read(filestream, filestream.NumItems);
        }
        catch (Exception e)
        {
            var cfgName = cfg.GetType().Name;
            Debug.LogError($"[Config] read config {cfgName} failed");
            StaticUtils.RethrowException(e);
        }
    }

    #endregion

    #region write

    public void WriteConfig_standalone()
    {
        var projPath = StaticUtils.GetProjectPath();
        var srcPath = $"{projPath}/GameConfig";
        var destPath = $"{projPath}/Assets/StreamingAssets/GameConfig";
        StaticUtils.CopyFolder(srcPath, destPath, isAbsolutePath: true);
    }

    public void WriteConfig_mobile()
    {
        var projPath = StaticUtils.GetProjectPath();
        var path = $"{projPath}/Assets/StreamingAssets/GameConfig/all_config.bin";
        StaticUtils.OpenFileForWrite(path, WriteConfigBinary, isAbsolutePath: true);
    }

    public void WriteConfigBinary(BinaryWriter writer)
    {
        var streamWriter = new FileStream_binaryWriter(writer);
        foreach (var cfg in listConfigs)
        {
            cfg.Write(streamWriter);
        }
    }

    #endregion
}