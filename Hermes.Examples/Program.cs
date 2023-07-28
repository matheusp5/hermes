
namespace Hermes.Examples;

using System;


class Program
{
    static async Task Main()
    {
        var server = new SocketServer();
        server.CreateConnection("127.0.0.1", 12345);
        server.OnMessage(HandleServerMessage);

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

    private static void HandleServerMessage(string message)
    {
        Console.WriteLine("Received from server: " + message);
    }

    private static void HandleClientMessage(string message)
    {
        Console.WriteLine("Received from client: " + message);
    }
}