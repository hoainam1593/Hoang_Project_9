
#if IN_CSHARP_PROJ

using System.Text;
using Amazon.S3;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace CSharpProjUploadAWS;

public class ServerConfigJson
{
    public int gameConfigVersion;
    public string gameClientVersion;
    public int addressableVersion;

    private string serverEnvironment;
    private AmazonS3Client s3Client;

    public ServerConfigJson(string serverEnvironment, AmazonS3Client s3Client)
    {
        this.serverEnvironment = serverEnvironment;
        this.s3Client = s3Client;
    }

    public async Task Download()
    {
        var json = await AwsApi.DownloadFileFromS3($"{serverEnvironment}/server_config.json");
        if (!string.IsNullOrEmpty(json))
        {
            JsonConvert.PopulateObject(json, this);
        }
    }

    public async Task Upload()
    {
        var json = JsonSerializeToFriendlyText(this);
        var stream = new MemoryStream(Encoding.UTF8.GetBytes(json));
        await AwsApi.UploadFileToS3(s3Client, stream, FileType.Json, $"{serverEnvironment}/server_config.json");
    }

    private static string JsonSerializeToFriendlyText(object obj)
    {
        var settings = new JsonSerializerSettings();
        settings.Formatting = Formatting.Indented;
        settings.Converters.Add(new StringEnumConverter());
        return JsonConvert.SerializeObject(obj, settings);
    }
}

#endif