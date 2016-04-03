using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using GameEngine.Common;
using TestHarness.TestHarnesses.SocketHarness;

namespace SocketHost
{
    public class ClientRegistrationService : IDisposable
    {
        public const int Port = 19010;
        private readonly Socket _socket;
        private readonly List<Player> _players = new List<Player>();

        public ClientRegistrationService()
        {
            var ipHost = Dns.GetHostEntry("");
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            foreach (var ipAddress in ipHost.AddressList.Where(x => x.AddressFamily == AddressFamily.InterNetwork))
            {
                var ipEndPoint = new IPEndPoint(ipAddress, Port);
                _socket.Bind(ipEndPoint);
            }

            Console.WriteLine("Waiting for client registrations on port " + Port);

            _socket.Listen(12);
            _socket.BeginAccept(ConnectCallBack, _socket);
        }

        public List<Player> Players
        {
            get { return _players; }
        }

        private void ConnectCallBack(IAsyncResult ar)
        {
            try
            {
                var listener = (Socket) ar.AsyncState;

                var clientSocket = listener.EndAccept(ar);

                var state = new SocketState()
                {
                    Socket = clientSocket,
                    ByteBuffer = new byte[10240]
                };

                clientSocket.BeginReceive(state.ByteBuffer, 0, state.ByteBuffer.Length, SocketFlags.None,
                    ReceiveCallBack,
                    state);

                _socket.BeginAccept(ConnectCallBack, _socket);
            }
            catch (ObjectDisposedException)
            {
                //This is supposed to happen once the game is started and no clients can be registered anymore

            }
        }

        private void ReceiveCallBack(IAsyncResult ar)
        {
            var socketState = (SocketState)ar.AsyncState;
            var socket = socketState.Socket;

            var result = SocketHarnessMessage.ProcessMessage(ar);

            socket.EndReceive(ar);
            switch (result.MessageType)
            {
                    case SocketHarnessMessage.MessageType.RegisterPlayer:
                    HandleRegistration(socket, result.Message);
                    break;
                default:
                    throw new ArgumentException("Only registration messages allowed " + result.MessageType);
            }
        }

        private void HandleRegistration(Socket socket, String name)
        {
            var port = 20000 + _players.Count;
            var player = new SocketServer(name, port);

            _players.Add(player);

            SocketHarnessMessage.SendMessage(socket, SocketHarnessMessage.MessageType.RegistrationPort, port.ToString(), Callback);
        }

        private void Callback(IAsyncResult ar)
        {
            var socketState = (SocketState)ar.AsyncState;
            var socket = socketState.Socket;

            if (socket.Connected)
            {
                socket.Disconnect(false);
                socket.Close();
            }
            socket.Close();
        }

        public void Dispose()
        {
            _socket.Close();
            _socket.Close();
        }
    }
}
