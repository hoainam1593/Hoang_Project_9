
#if IN_CSHARP_PROJ

using Amazon;
using Amazon.CloudFront;
using Amazon.S3;

namespace CSharpProjUploadAWS;

internal class Program
{
    #region core

    private static async Task Main(string[] args)
    {
        var region = RegionEndpoint.GetBySystemName(Const.awsRegion);
        var s3Client = new AmazonS3Client(Const.awsAccessKeyId, Const.awsSecretAccessKey, region);
        var cloudFrontClient = new AmazonCloudFrontClient(Const.awsAccessKeyId, Const.awsSecretAccessKey, region);
        
        if (args == null || args.Length == 0)
        {
            args = GetDummyArgs();
        }

        var type = args[0];
        var serverEnvironment = args[1];

        if (type.Equals("config"))
        {
            await UploadCfg(s3Client, serverEnvironment, args);
        }
        else
        {
            await UploadAddressable(s3Client, serverEnvironment, args);
        }
        
        await UpdateServerConfig(s3Client, serverEnvironment, type);
        await ApplySet(cloudFrontClient);
        
        Console.WriteLine("DONE...........");
    }

    private static string[] GetDummyArgs()
    {
        //config
        /**/
        return
        [
            "config",
            "Dev",
            @"D:\unity-projects\project_11\GameConfig",
            @"D:\unity-projects\project_11\Assets\StreamingAssets\GameConfig\all_config.bin",
        ];
        /**/
            
        //addressable
        /*
        return 
        [
            "addressable",
            "Dev",
            @"D:\unity-projects\project_11_event_assets\ServerData",
        ];
        /**/
    }
    
    private static async Task UpdateServerConfig(AmazonS3Client s3Client, string serverEnvironment, string type)
    {
        var serverCfg = new ServerConfigJson(serverEnvironment, s3Client);
        await serverCfg.Download();

        if (type.Equals("config"))
        {
            serverCfg.gameConfigVersion++;
        }
        else
        {
            serverCfg.addressableVersion++;
        }

        await serverCfg.Upload();
    }

    private static async Task ApplySet(AmazonCloudFrontClient cloudFrontClient)
    {
        await AwsApi.InvalidateCloudFront(cloudFrontClient);
    }
    
    private static async Task UploadFile(AmazonS3Client s3Client, string filepath, string serverPath, FileType fileType)
    {
        var fileStream = new FileStream(filepath, FileMode.Open, FileAccess.Read);
        await AwsApi.UploadFileToS3(s3Client, fileStream, fileType, serverPath);
    }

    #endregion

    #region config

    private static async Task UploadCfg(AmazonS3Client s3Client, string serverEnvironment, string[] args)
    {
        var cfgTextFolder = args[2];
        var cfgBinaryFile = args[3];
        
        await UploadCfg_text(s3Client, cfgTextFolder, serverEnvironment);
        await UploadCfg_binary(s3Client, cfgBinaryFile, serverEnvironment);
    }
    
    private static async Task UploadCfg_text(AmazonS3Client s3Client, string cfgTextFolder, string serverEnvironment)
    {
        var lTasks = new List<Task>();
        var lFiles = Directory.GetFiles(cfgTextFolder);

        foreach (var file in lFiles)
        {
            var fileName = Path.GetFileName(file);
            var serverPath = $"{serverEnvironment}/game_configs_text/{fileName}";
            lTasks.Add(UploadFile(s3Client, file, serverPath, FileType.CSV));
        }
        
        await Task.WhenAll(lTasks);
    }

    private static async Task UploadCfg_binary(AmazonS3Client s3Client, string cfgBinaryFile, string serverEnvironment)
    {
        var serverPath = $"{serverEnvironment}/game_configs_binary/all_config.bin";
        await UploadFile(s3Client, cfgBinaryFile, serverPath, FileType.Binary);
    }

    #endregion

    #region addressable

    private static async Task UploadAddressable(AmazonS3Client s3Client, string serverEnvironment, string[] args)
    {
        var parentFolder = args[2];
        var lTasks = new List<Task>();
        var lPlatforms = new List<string>()
        {
            "StandaloneWindows64", 
            "Android",
            "iOS"
        };

        foreach (var platform in lPlatforms)
        {
            var localPath = $"{parentFolder}/{platform}/remotegroup_assets_all.bundle";
            var serverPath = $"{serverEnvironment}/addressable/{platform}/remotegroup_assets_all.bundle";
            lTasks.Add(UploadFile(s3Client, localPath, serverPath, FileType.Binary));
        }
        
        await Task.WhenAll(lTasks);
    }

    #endregion
}

#endif