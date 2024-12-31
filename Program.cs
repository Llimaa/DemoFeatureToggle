using System.Text.Json;
using Amazon.AppConfigData;
using Amazon.AppConfigData.Model;
using Amazon.Runtime;
using DemoFeatureToggle;


var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();

app.MapGet("/", async (IConfiguration configuration) => 
{
    var result = await FeatureToggleService.GetFeatureToggleAsync(configuration);

    if(result.NewFeature)
        return "Feature Toggle Enable";

    return "Feature Toggle Disable!";
});

app.Run();


