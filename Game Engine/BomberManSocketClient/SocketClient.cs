using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using TestHarness.TestHarnesses.SocketHarness;

namespace BomberManSocketClient
{
    public class SocketClient : IDisposable
    {
        private readonly String _hostname;
        private readonly String _username;
        private Socket _socket;
        private bool killed = false;
        private char playerKey;

        public SocketClient(string hostname, string username)
        {
            _hostname = hostname;
            _username = username;

            var ipHost = Dns.GetHostEntry(hostname);
            var socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            var state = new SocketState
            {
                Socket = socket
            };

            var endpoint = ipHost.AddressList.Last(x => x.AddressFamily == AddressFamily.InterNetwork);
            socket.BeginConnect(endpoint, 19010, RegisterCallback, state);
        }

        public static void Main(string[] args)
        {
            Start();
        }

        private static void Start()
        {
            Console.WriteLine("Please Enter Host Name");
            var hostname = Console.ReadLine();
            Console.WriteLine("Please Enter Player Name");
            var username = Console.ReadLine();


            using (var socketClient = new SocketClient(hostname, username))
            {
                var line = Console.ReadLine();
                while (line != "quit")
                {
                    if (!string.IsNullOrEmpty(line))
                        socketClient.SendCommand(line);

                    line = Console.ReadLine();
                }
            }
        }

        private void RegisterCallback(IAsyncResult ar)
        {
            var state = (SocketState)ar.AsyncState;
            var socket = state.Socket;

            state.ByteBuffer = new byte[10240];

            socket.BeginReceive(state.ByteBuffer, 0, state.ByteBuffer.Length, SocketFlags.None, RegistrationMessage, state);
            SocketHarnessMessage.SendMessage(socket, SocketHarnessMessage.MessageType.RegisterPlayer, _username);
        }

        private void RegistrationMessage(IAsyncResult ar)
        {
            var state = (SocketState)ar.AsyncState;
            var socket = state.Socket;

            var message = SocketHarnessMessage.ProcessMessage(ar);
            int port = 0;

            switch (message.MessageType)
            {
                case SocketHarnessMessage.MessageType.RegistrationPort:
                    port = Int32.Parse(message.Message);
                    break;
                default:
                    throw new ArgumentException("Expected registration port message " + message.MessageType);
            }

            socket.EndReceive(ar);

            if (socket.Connected)
            {
                socket.Disconnect(false);
                socket.Close();
            }
            socket.Close();

            var ipHost = Dns.GetHostEntry(_hostname);
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            state.Socket = _socket;
            state.ByteBuffer = new byte[20480];

            _socket.BeginConnect(ipHost.AddressList.Last(x => x.AddressFamily == AddressFamily.InterNetwork), port, GameSocketConnected, state);

            Console.WriteLine("Congratulations, player registered on server.");
            Console.WriteLine("Please wait while the others connect and the game is started.");
            Console.WriteLine("To move, type a,w,s,d and press enter");
            Console.WriteLine("To plant a bomb, type z and enter");
            Console.WriteLine("You can reduce the bomb timer to 1 using x");
        }

        private void GameSocketConnected(IAsyncResult ar)
        {
            try
            {
                var state = (SocketState)ar.AsyncState;
                var socket = state.Socket;

                socket.BeginReceive(state.ByteBuffer, 0, state.ByteBuffer.Length, SocketFlags.None, GameSocketMessage,
                    state);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void GameSocketMessage(IAsyncResult ar)
        {
            try
            {
                var state = (SocketState)ar.AsyncState;
                var socket = state.Socket;

                var message = SocketHarnessMessage.ProcessMessage(ar);
                switch (message.MessageType)
                {
                    case SocketHarnessMessage.MessageType.GameMap:
                        UpdateGameMap(message.Message);
                        break;
                    case SocketHarnessMessage.MessageType.Killed:
                        killed = true;
                        break;
                    case SocketHarnessMessage.MessageType.CommandFailed:
                        CommandFailedMessage(message.Message);
                        break;
                    case SocketHarnessMessage.MessageType.PlayerRegistered:
                        playerKey = message.Message.First();
                        break;
                    case SocketHarnessMessage.MessageType.GameComplete:
                        Console.Clear();
                        Console.WriteLine("Game Has ended!");

                        socket.EndReceive(ar);
                        if (socket.Connected)
                        {
                            socket.Close();
                            return;
                        }

                        break;
                    default:
                        throw new ArgumentException("Cannot process this message type " + message.MessageType);
                }

                state.ByteBuffer = new byte[20480];

                socket.EndReceive(ar);
                socket.BeginReceive(state.ByteBuffer, 0, state.ByteBuffer.Length, SocketFlags.None, GameSocketMessage,
                    state);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        private void UpdateGameMap(String map)
        {
            Console.Clear();

            bool insideMap = false;

            foreach (var character in map)
            {
                if (character == '#')
                {
                    Console.ForegroundColor = ConsoleColor.Blue;
                    insideMap = true;
                }
                if (character == '\t')
                {
                    insideMap = false;
                }
                if (character == '+')
                {
                    Console.ForegroundColor = ConsoleColor.DarkGray;
                }
                if (character == '!')
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                }
                if (character == '&')
                {
                    Console.ForegroundColor = ConsoleColor.Cyan;
                }
                if (character == '$')
                {
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }
                if (character == '*')
                {
                    Console.ForegroundColor = ConsoleColor.DarkRed;
                }
                if (insideMap && (character == playerKey || Char.ToUpperInvariant(character) == playerKey))
                {
                    Console.ForegroundColor = ConsoleColor.Magenta;
                }

                Console.Write(character);

                Console.ResetColor();
            }

            if (killed)
            {
                Console.WriteLine("");
                Console.WriteLine("You have been killed and can no longer send commands");
            }

        }

        private void CommandFailedMessage(String message)
        {
            Console.WriteLine("Command failed: " + message);
        }

        public void SendCommand(String command)
        {
            if (_socket != null && _socket.Connected && !killed)
            {
                SocketHarnessMessage.SendMessage(_socket, SocketHarnessMessage.MessageType.Command, command);
                Console.WriteLine("Your command has been sent to the server.  Wait for the other players and the map will update");
            }

            if (killed)
            {
                Console.WriteLine("You have been killed and can no longer send commands");
            }
        }

        public void Dispose()
        {
            if (_socket != null)
            {
                if (_socket.Connected)
                {
                    _socket.Disconnect(false);
                    _socket.Close();
                }

                _socket.Close();
            }
        }
    }
}
