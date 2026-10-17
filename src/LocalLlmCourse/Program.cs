using OllamaSharp;
using Microsoft.Extensions.AI;

var ollamaUri = new Uri("http://localhost:11434");

var model = "qwen3:4b-instruct";

IChatClient chat = new OllamaApiClient(ollamaUri, model);

var system = new ChatMessage(ChatRole.System,
    "You are a friendly assistant. Answer in one short sentence.");

List<ChatMessage> messages = [system];

string[] questions = [
        "Hi! My name is Burak, and I teach .Net",
        "What is my name?",
        "What do I teach"
    ];

for(var turn = 1; turn <= questions.Length; turn++)
{
    var question = questions[turn - 1];
    Console.WriteLine($"You: {question}");

    messages.Add(new ChatMessage(ChatRole.User, question));

    Console.Write("Model: ");
    ChatResponse response = await AskAsync(messages);

    messages.AddMessages(response);

    Console.WriteLine($"    turn {turn}");
    Console.WriteLine();
}

async Task<ChatResponse> AskAsync(IEnumerable<ChatMessage> conversation)
{
    List<ChatResponseUpdate> updates = [];

    await foreach(ChatResponseUpdate update in chat.GetStreamingResponseAsync(conversation))
    {
        Console.Write(update.Text);
        updates.Add(update);
    }

    Console.WriteLine(); // empty line
    return updates.ToChatResponse();
}


