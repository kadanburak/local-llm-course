using System.Diagnostics;                         
using Microsoft.Extensions.AI;                   
using Microsoft.Extensions.DependencyInjection;  
using Microsoft.Extensions.Hosting;              
using OllamaSharp;                               
using Microsoft.Extensions.Logging;

// The host. It also reads appsettings.json from the app's own folder.
var builder = Host.CreateApplicationBuilder(new HostApplicationBuilderSettings
{
    Args = args,
    ContentRootPath = AppContext.BaseDirectory
});

// Show the log messages of Microsoft.Extensions.AI (they are written at Debug level).
builder.Logging.AddFilter("Microsoft.Extensions.AI", LogLevel.Debug);

// A cache that lives in memory, for as long as the app runs.
builder.Services.AddDistributedMemoryCache();

var chat = builder.Configuration.GetSection("Chat");
string provider = chat["Provider"] ?? "Ollama";
string model = chat["Model"]
    ?? throw new InvalidOperationException("Chat: Model is missing in appsettings file");
var endpoint = new Uri(chat["Endpoint"] ?? "http://localhost:11434");

IChatClient innerClient = provider switch
{
    "Ollama" => new OllamaApiClient(endpoint, model),
    _ => throw new InvalidOperationException($"Unknown provider: {provider}")
};

// Register IChatClient, and wrap it in a logging layer.
builder.Services
    .AddChatClient(innerClient)
    .UseLogging()
    .UseDistributedCache();

using IHost host = builder.Build();

// The rest of the app asks for IChatClient. It doesn't know what's behind it.
IChatClient client = host.Services.GetRequiredService<IChatClient>();

#region Rest of the application
Console.WriteLine($"Provider: {provider} Model: {model}");
var prompt = "In one sentence, what is .NET?";

// The same question, twice.
for (var call = 1; call <= 2; call++)
{
    var stopwatch = Stopwatch.StartNew();
    ChatResponse response = await client.GetResponseAsync(prompt);
    stopwatch.Stop();

    Console.WriteLine($"call {call}   {stopwatch.Elapsed.TotalMilliseconds,7:F0} ms {response.ModelId}  {response.Text}");
}
#endregion