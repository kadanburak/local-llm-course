using OllamaSharp;
using Microsoft.Extensions.AI;

// "In about 150 words, explain what .NET is."
var ollamaUri = new Uri("http://localhost:11434");

var model = "qwen3:4b-instruct";

IChatClient chat = new OllamaApiClient(ollamaUri, model);

var options = new ChatOptions { MaxOutputTokens = 20 };

ChatResponse response = await AskAsync([
    new ChatMessage(ChatRole.User, "In about 150 words, explain what .NET is.")
    ], options);

Console.WriteLine($"Output: {response.Usage?.OutputTokenCount}");
Console.WriteLine($"Finish Reason: {response.FinishReason}");

await RunFiveTimesAsync("Test 2 - default settings", null);

await RunFiveTimesAsync("Test 3 - Temp 0", new ChatOptions { Temperature=0, Seed = 42});

async Task<ChatResponse> AskAsync(IEnumerable<ChatMessage> conversation, ChatOptions? options = null)
{
    List<ChatResponseUpdate> updates = [];

    await foreach(ChatResponseUpdate update in chat.GetStreamingResponseAsync(conversation, options))
    {
        Console.Write(update.Text);
        updates.Add(update);
    }

    Console.WriteLine(); // empty line
    return updates.ToChatResponse();
}

async Task RunFiveTimesAsync(string title, ChatOptions? options)
{
    Console.WriteLine(title);
    var answers = new HashSet<string>();

    for(var run = 1; run <= 5; run++)
    {
        Console.Write($"    run {run}:");
        ChatResponse response = await AskAsync(
            [new ChatMessage(
                ChatRole.User, "Suggest one name for a cooking app built with .NET. Reply with the name only.")], options);
        answers.Add(response.Text.Trim());
    }

    Console.WriteLine($"{answers.Count} different answers from 5 runs.");
    Console.WriteLine();
}