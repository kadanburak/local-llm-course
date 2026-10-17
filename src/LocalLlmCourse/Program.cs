using System.Diagnostics;
using OllamaSharp;
using Microsoft.Extensions.AI;

var ollamaUri = new Uri("http://localhost:11434");

var model = "qwen3:4b-instruct";

var prompt = "In about 150 words, explain what .NET is and what you can build with it.";

IChatClient chat = new OllamaApiClient(ollamaUri, model);

var sw = Stopwatch.StartNew();

List<ChatResponseUpdate> updates = [];

TimeSpan? firstToken = null;

await foreach(ChatResponseUpdate update in chat.GetStreamingResponseAsync(prompt))
{
    if(firstToken is null && !string.IsNullOrEmpty(update.Text))
    {
        firstToken = sw.Elapsed;
    }

    Console.Write(update.Text);

    updates.Add(update);
}

ChatResponse response = updates.ToChatResponse();

sw.Stop();

Console.WriteLine($"Response: {response.Text}");
Console.WriteLine($"Answered By: {response.ModelId}");
Console.WriteLine($"Finish Reason: {response.FinishReason}");
Console.WriteLine($"Input Tokens: {response.Usage?.InputTokenCount}");
Console.WriteLine($"Output Tokens: {response.Usage?.OutputTokenCount}");
Console.WriteLine($"Total Tokens: {response.Usage?.TotalTokenCount}");
Console.WriteLine($"Time: {sw.Elapsed.TotalSeconds:F2} s");
Console.WriteLine($"First Token: {firstToken?.Seconds:F2} s");