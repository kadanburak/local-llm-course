using OllamaSharp;
using Microsoft.Extensions.AI;
using System.Diagnostics;

// "In about 150 words, explain what .NET is."
var ollamaUri = new Uri("http://localhost:11434");

var model = "qwen3:4b";

IChatClient chat = new OllamaApiClient(ollamaUri, model);

var prompt = "ping. reply with one word only";

var options = new ChatOptions
{
    Reasoning = new ReasoningOptions { Effort = ReasoningEffort.None}
};

// warm-up call
await chat.GetResponseAsync(prompt, options);

// the measured call
var sw = Stopwatch.StartNew();
ChatResponse response = await chat.GetResponseAsync(prompt, options);
sw.Stop();

string thinking = string.Concat(
    response.Messages
    .SelectMany(message=>message.Contents)
    .OfType<TextReasoningContent>()
    .Select(reasoning=>reasoning.Text));

Console.WriteLine($"Model: { response.ModelId}");
Console.WriteLine($"Answer: {response.Text}");
Console.WriteLine($"OutputTokens : {response.Usage?.OutputTokenCount}");
Console.WriteLine($"Elapsed: {sw.Elapsed.TotalSeconds:F2} s");
Console.WriteLine($"Thinking Chars: {thinking.Length}");

Console.WriteLine();
Console.WriteLine("Hidden thinking...");
Console.WriteLine(thinking);

const string endOfThinking = "</think>";
bool leaked = response.Text.Contains(endOfThinking);

string realAnswer = leaked
    ? response.Text[(response.Text.LastIndexOf(endOfThinking) + endOfThinking.Length)..]
    : response.Text;

Console.WriteLine();
Console.WriteLine($"Leaked : {(leaked?"yes":"no")}");
Console.WriteLine($"Real Answer : {realAnswer}");