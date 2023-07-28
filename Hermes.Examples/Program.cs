
namespace Hermes.Examples;

using System;


class Program
{
    static async Task Main()
    {
        var server = new SocketServer();
        server.CreateConnection("127.0.0.1", 12345);
        server.OnMessage(HandleServerMessage);
        server.OnConnection(HandleClientConnection);
        server.OnDisconnection(HandleClientDisconnect);

        var client = new SocketClient();
        client.Connect("127.0.0.1", 12345);
        client.OnMessage(HandleClientMessage);

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
            await server.SendToAll(message);
        }
    }

    private static void HandleServerMessage(string message, int userId, SocketServer server)
    {
        Console.WriteLine($"Message from client, user id: {userId}: {message}");
    }
    
    private static void HandleClientConnection(int userId, SocketServer server)
    {
        Console.WriteLine($"New client connected: {userId}");
    }
    
    private static void HandleClientDisconnect(int userId, SocketServer server)
    {
        Console.WriteLine($"Client disconnected: {userId}");
    }

    private static void HandleClientMessage(string message)
    {
        Console.WriteLine("Message from server: " + message);
    }
}