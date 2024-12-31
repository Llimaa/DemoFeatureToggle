using System.Text.Json;
using Amazon.AppConfigData;
using Amazon.AppConfigData.Model;
using Amazon.Runtime;

namespace DemoFeatureToggle;

public static class FeatureToggleService 
{
    public static async Task<FeatureFlag> GetFeatureToggleAsync(IConfiguration configuration) 
    {
        var accessKey = configuration.GetValue<string>("AwsConfig:AccessKey");
        var secretKey = configuration.GetValue<string>("AwsConfig:SecretKey");
        var credentials = new BasicAWSCredentials(accessKey, secretKey);

        var configDataClient = new AmazonAppConfigDataClient(
        credentials, new AmazonAppConfigDataConfig
            {
                RegionEndpoint = Amazon.RegionEndpoint.USEast1
            }
        );

        var startSessionRequest = new StartConfigurationSessionRequest
        {
            ApplicationIdentifier = configuration.GetValue<string>("AppConfig:ApplicationId"),
            EnvironmentIdentifier = configuration.GetValue<string>("AppConfig:EnvironmentId"),
            ConfigurationProfileIdentifier = configuration.GetValue<string>("AppConfig:ConfigurationProfileId")
        };

        var sessionResponse = await configDataClient.StartConfigurationSessionAsync(startSessionRequest);

        var getConfigRequest = new GetLatestConfigurationRequest
        {
            ConfigurationToken = sessionResponse.InitialConfigurationToken
        };

        var configResponse = await configDataClient.GetLatestConfigurationAsync(getConfigRequest);
        var configJson = System.Text.Encoding.UTF8.GetString(configResponse.Configuration.ToArray());
        
        var featureConfig = JsonSerializer.Deserialize<FeatureFlag>(configJson);
        return featureConfig ?? new(false);
    }
}

public record FeatureFlag(bool NewFeature);