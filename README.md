# Hermes 📨
Hermes is a powerful and user-friendly library crafted with .NET 7.0 and C# to streamline the development of Socket Apps. Born in 2023, its primary mission is to empower developers in building efficient and seamless Client-Side and Server-Side applications. Whether you're looking to establish real-time communication, enhance networking capabilities, or create interactive multiplayer experiences, Hermes has got you covered. By abstracting the complexities of low-level socket programming, Hermes enables developers to focus on crafting robust and feature-rich applications with ease. With its intuitive API and comprehensive documentation, Hermes proves to be an indispensable tool in your development toolkit. Embrace the power of Hermes and revolutionize your Socket App development journey today!

## How to install
We do not yet support the C# package manager, NuGet. However, it is possible to use Hermes with .dll! For you to use it, you need to go to the 'releases' section here on github and download the latest version. Another way is downloading the source code and compiling it manually.

After installing our DLL, you need to reference it in your project. It's a simple task and is usually done by the IDE. If you don't want to install the DLL, you can download our source code and reference it with the code below:

```
dotnet add reference PATH_TO_CSPROJ.csproj
```

## How to use

First, you need to create the Socket server. In the code below, we are opening a server on port 12345 (with SocketServer) and joining that channel with SocketClient

```csharp
var server = new SocketServer();
server.CreateConnection("127.0.0.1", 12345);
server.OnMessage(HandleServerMessage);

var client = new SocketClient();
client.Connect("127.0.0.1", 12345);
client.OnMessage(HandleClientMessage);
```

Sending messages to server...
```csharp
while (true)
{
    Thread.Sleep(1000);
    Console.Write("Enter a message to send from client (type 'exit' to quit): ");
    var message = Console.ReadLine();

    if (message.ToLower() == "exit")
    {
        break;
    }

    await client.Send(message);
}
```

Capturing new messages...
```csharp
private static void HandleServerMessage(string message, int userId, SocketServer server)
{
    Console.WriteLine("Received from server: " + message);
}

private static void HandleClientMessage(string message)
{
    Console.WriteLine("Received from client: " + message);
}
```

## License
This project is under the MIT License

## Contributing
You can contribute by opening pull requests if you have any changes to make to the code, or open an issue if you are having a problem.

<hr>

Built with ❤ by Matheus. Could you follow me?
