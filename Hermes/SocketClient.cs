using System.Net;
using System.Net.Sockets;
using System.Text;

namespace Hermes;

public class SocketClient
{
    private Socket _clientSocket;
    private Action<string> _onMessageReceived;

    public void Connect(string ip, int port)
    {
        var ipAddress = IPAddress.Parse(ip);
        _clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        _clientSocket.Connect(new IPEndPoint(ipAddress, port));
        _ = ReceiveMessages();
    }

    public void OnMessage(Action<string> handler) { _onMessageReceived = handler; }

    private async Task ReceiveMessages()
    {
        while (true)
        {
            var buffer = new byte[1024];
            var bytesRead = await _clientSocket.ReceiveAsync(buffer, SocketFlags.None);
            var data = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            _onMessageReceived?.Invoke(data);
            if (data == "exit")
            {
                break;
            }
        }
        _clientSocket.Shutdown(SocketShutdown.Both);
        _clientSocket.Close();
    }

    public async Task Send(string message)
    {
        byte[] buffer = Encoding.ASCII.GetBytes(message);
        await _clientSocket.SendAsync(buffer, SocketFlags.None);
    }
}