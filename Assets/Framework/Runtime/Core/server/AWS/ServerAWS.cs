
#if USE_SERVER_AWS

using System;
using Amazon;
using Amazon.CloudFront;
using Amazon.Runtime;
using Amazon.Runtime.CredentialManagement;
using Amazon.S3;

public partial class ServerAWS : IServer
{
    private AmazonS3Client s3Client;
    private AmazonCloudFrontClient cloudFrontClient;
    
    public ServerAWS(bool isAdmin)
    {
        if (isAdmin)
        {
            var region = RegionEndpoint.GetBySystemName(GameFrameworkConfig.instance.awsRegion);
            s3Client = new AmazonS3Client(GetAWSCredentials(), region);
            cloudFrontClient = new AmazonCloudFrontClient(GetAWSCredentials(), region);
        }
    }

    private AWSCredentials GetAWSCredentials()
    {
        var chain = new CredentialProfileStoreChain();
        if (chain.TryGetAWSCredentials("default", out var credentials))
        {
            return credentials;
        }
        else
        {
            throw new Exception("need login AWS first");
        }
    }
}

#endif