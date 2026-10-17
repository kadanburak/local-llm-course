using System.Diagnostics;
using OllamaSharp;
using Microsoft.Extensions.AI;

IChatClient chat = new OllamaApiClient("http://localhost:11434", "qwen3:4b");

var sw = Stopwatch.StartNew();

ChatResponse response = await chat.GetResponseAsync("reply with only one word: pong!");

sw.Stop();

Console.WriteLine($"Response: {response.Text}");
Console.WriteLine($"Answered By: {response.ModelId}");
Console.WriteLine($"Finish Reason: {response.FinishReason}");
Console.WriteLine($"Input Tokens: {response.Usage?.InputTokenCount}");
Console.WriteLine($"Output Tokens: {response.Usage?.OutputTokenCount}");
Console.WriteLine($"Total Tokens: {response.Usage?.TotalTokenCount}");
Console.WriteLine($"Time: {sw.Elapsed.TotalSeconds:F2} s");