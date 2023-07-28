using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Hermes;

public class SocketServer
{
    private Socket _serverSocket;
    private List<(int Id, Socket Socket)> _connectedClients = new List<(int, Socket)>();
    private Action<string, int, SocketServer> _onMessageReceived;
    private Action<int, SocketServer> _onConnection;
    private Action<int, SocketServer> _OnDisconnection;
    private int _nextUserId = 1;

    public void CreateConnection(string ip, int port)
    {
        var ipAddress = IPAddress.Parse(ip);
        _serverSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _serverSocket.Bind(new IPEndPoint(ipAddress, port));
        _serverSocket.Listen(10);
        Task.Run(ListenForClients);
    }

    public void OnMessage(Action<string, int, SocketServer> handler)
    {
        _onMessageReceived = handler;
    }

    public void OnConnection(Action<int, SocketServer> handler)
    {
        _onConnection = handler;
    }

    public void OnDisconnection(Action<int, SocketServer> handler)
    {
        _OnDisconnection = handler;
    }

    private async Task ListenForClients()
    {
        while (true)
        {
            var clientSocket = await _serverSocket.AcceptAsync();
            var userId = _nextUserId++;
            _connectedClients.Add((userId, clientSocket));

            _onConnection?.Invoke(userId, this);

            _ = Task.Run(() => HandleClient(userId, clientSocket));
        }
    }

    private async Task HandleClient(int userId, Socket clientSocket)
    {
        try
        {
            while (true)
            {
                var buffer = new byte[1024];
                var bytesRead = await clientSocket.ReceiveAsync(buffer, SocketFlags.None);
                var data = Encoding.ASCII.GetString(buffer, 0, bytesRead);

                _onMessageReceived?.Invoke(data, userId, this);

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
            _connectedClients.RemoveAll(client => client.Id == userId);

            _OnDisconnection?.Invoke(userId, this);
            clientSocket.Shutdown(SocketShutdown.Both);
            clientSocket.Close();
        }
    }

    public async Task SendToAll(string message)
    {
        byte[] buffer = Encoding.ASCII.GetBytes(message);
        foreach (var client in _connectedClients)
        {
            await client.Socket.SendAsync(buffer, SocketFlags.None);
        }
    }
}