using Amazon.SecretsManager.Model;
using Amazon.SecretsManager;
using Amazon;

namespace ApiAWSMatchUp.Helpers
{
    public class HelperSecretManager
    {
        public static async Task<string> GetSecretAsync()
        {
            string secretName = "secretsmatchup";
            string region = "us-east-1";

            IAmazonSecretsManager client = new AmazonSecretsManagerClient(RegionEndpoint.GetBySystemName(region));

            GetSecretValueRequest request = new GetSecretValueRequest
            {
                SecretId = secretName,
                VersionStage = "AWSCURRENT", // VersionStage defaults to AWSCURRENT if unspecified. 
            };

            GetSecretValueResponse response;
            response = await client.GetSecretValueAsync(request);
            string secret = response.SecretString;
            return secret;
        }
    }
}
