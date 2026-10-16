using OllamaSharp;
using Microsoft.Extensions.AI;

IChatClient chat = new OllamaApiClient("http://localhost:11434", "qwen3:4b");

ChatResponse response = await chat.GetResponseAsync("reply with only one word: pong!");

Console.WriteLine($"Response: {response.Text}");
