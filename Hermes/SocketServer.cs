using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Hermes;

public class SocketServer
{
    private Socket _serverSocket;
    private List<Socket> _connectedClients = new List<Socket>();
    private Action<string> _onMessageReceived;

    public void CreateConnection(string ip, int port)
    {
        var ipAddress = IPAddress.Parse(ip);
        _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _serverSocket.Bind(new IPEndPoint(ipAddress, port));
        _serverSocket.Listen(10);

        Task.Run(ListenForClients);
    }

    public void OnMessage(Action<string> handler)
    {
        _onMessageReceived = handler;
    }

    private async Task ListenForClients()
    {
        while (true)
        {
            var clientSocket = await _serverSocket.AcceptAsync();
            _connectedClients.Add(clientSocket);
            _ = Task.Run(() => HandleClient(clientSocket));
        }
    }

    private async Task HandleClient(Socket clientSocket)
    {
        try
        {
            while (true)
            {
                var buffer = new byte[1024];
                var bytesRead = await clientSocket.ReceiveAsync(buffer, SocketFlags.None);
                var data = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                _onMessageReceived?.Invoke(data);

                if (data == "exit")
                {
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine("Error while handling client: " + ex.Message);
        }
        finally
        {
            _connectedClients.Remove(clientSocket);
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }
    }

    public async Task SendToAll(string message)
    {
        byte[] buffer = Encoding.ASCII.GetBytes(message);
        foreach (var client in _connectedClients)
        {
            await client.SendAsync(buffer, SocketFlags.None);
        }
    }
}