using System;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using Domain.Common;
using GameEngine.Commands;
using GameEngine.Commands.PlayerCommands;
using GameEngine.Common;
using GameEngine.Renderers;
using TestHarness.TestHarnesses.SocketHarness;

namespace SocketHost
{
    public class SocketServer : Player
    {
        private readonly Socket _socket;
        private  Socket _clientSocket;
        private GameMap _gameMap;

        public SocketServer(string name, int port) 
            : base(name)
        {
            var ipHost = Dns.GetHostEntry(""); 
            _socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

            foreach (var ipAddress in ipHost.AddressList.Where(x => x.AddressFamily == AddressFamily.InterNetwork))
            {
                var ipEndPoint = new IPEndPoint(ipAddress, port);
                _socket.Bind(ipEndPoint);
            }
            _socket.Listen(1);
            _socket.BeginAccept(SocketCallBack, _socket);

            Console.WriteLine("Player " + name + " bound to port " + port);
        }

        private void SocketCallBack(IAsyncResult ar)
        {
            var listener = (Socket)ar.AsyncState;
            _clientSocket = listener.EndAccept(ar);

            BeginRecieve(_clientSocket);
        }

        private void BeginRecieve(Socket socket)
        {
            var state = new SocketState
            {
                Socket = socket,
                ByteBuffer = new byte[10240]
            };
            if (socket.Connected)
            {
                try
                {
                    socket.BeginReceive(state.ByteBuffer, 0, state.ByteBuffer.Length,
                        SocketFlags.None, ReceiveCallback, state);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Player " + Name + " no longer connected");
                    Console.WriteLine(ex);
                }
            }
        }

        private void ReceiveCallback(IAsyncResult ar)
        {
            var state = (SocketState)ar.AsyncState;
            var handler = state.Socket;
            try
            {
                var message = SocketHarnessMessage.ProcessMessage(ar);
                handler.EndReceive(ar);

                switch (message.MessageType)
                {
                    case SocketHarnessMessage.MessageType.Command:
                        HandleCommandMessage(message.Message);
                        break;
                    default:
                        throw new ArgumentException("Cannot process client message of type " + message.MessageType);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Client sent bad data " + ex);
                Console.WriteLine("Sending no move command");
                HandleCommandMessage("");
            }

            if (handler.Connected)
            {
                BeginRecieve(handler);
            }

        }

        private void HandleCommandMessage(String message)
        {
            if (message.Equals("w", StringComparison.InvariantCultureIgnoreCase))
                PublishCommand(new MovementCommand(MovementCommand.Direction.Up));
            else if (message.Equals("s", StringComparison.InvariantCultureIgnoreCase))
                PublishCommand(new MovementCommand(MovementCommand.Direction.Down));
            else if (message.Equals("a", StringComparison.InvariantCultureIgnoreCase))
                PublishCommand(new MovementCommand(MovementCommand.Direction.Left));
            else if (message.Equals("d", StringComparison.InvariantCultureIgnoreCase))
                PublishCommand(new MovementCommand(MovementCommand.Direction.Right));
            else if (message.Equals("z", StringComparison.InvariantCultureIgnoreCase))
                PublishCommand(new PlaceBombCommand());
            else if (message.Equals("x", StringComparison.InvariantCultureIgnoreCase))
                PublishCommand(new TriggerBombCommand());
            else
                PublishCommand(new DoNothingCommand());
        }


        public override void StartGame(GameMap gameState)
        {
            _gameMap = gameState;

            try
            {
                SocketHarnessMessage.SendMessage(_clientSocket, SocketHarnessMessage.MessageType.PlayerRegistered,
                    PlayerEntity.Key.ToString());
            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not send message to client " + Name + " because it disconnected");
                Console.WriteLine(ex);
            }

            NewRoundStarted(_gameMap);
        }

        public override void NewRoundStarted(GameMap gameState)
        {
            var renderer = new ConsoleRender(gameState);

            if (_clientSocket.Connected)
            {
                try
                {
                    SocketHarnessMessage.SendMessage(_clientSocket, SocketHarnessMessage.MessageType.GameMap,
                        renderer.RenderTextGameState().ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Could not send message to client " + Name + " because it disconnected");
                    Console.WriteLine(ex);
                }
            }
            else
            {
                PublishCommand(new DoNothingCommand());
            }
        }

        public override void GameEnded(GameMap gameMap)
        {
            if (_clientSocket.Connected)
            {
                try
                {
                    SocketHarnessMessage.SendMessage(_clientSocket, SocketHarnessMessage.MessageType.GameComplete, "");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Could not send message to client " + Name + " because it disconnected");
                    Console.WriteLine(ex);
                }
            }
        }

        public override void PlayerKilled(GameMap gameMap)
        {
            if (_clientSocket.Connected)
            {
                try
                {
                    var renderer = new ConsoleRender(gameMap);
                    SocketHarnessMessage.SendMessage(_clientSocket, SocketHarnessMessage.MessageType.Killed, renderer.RenderTextGameState().ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Could not send message to client " + Name + " because it disconnected");
                    Console.WriteLine(ex);
                }
            }
        }

        public override void PlayerCommandFailed(ICommand command, string reason)
        {
            if (_clientSocket.Connected)
            {
                try
                {
                    SocketHarnessMessage.SendMessage(_clientSocket, SocketHarnessMessage.MessageType.CommandFailed, reason);
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Could not send message to client " + Name + " because it disconnected");
                    Console.WriteLine(ex);
                }
            }
        }

        public override void Dispose()
        {
            if (_clientSocket.Connected)
            {
                _clientSocket.Disconnect(false);
                _clientSocket.Close();
            }
            _clientSocket.Close();

            if (_socket.Connected)
            {
                _socket.Disconnect(false);
                _socket.Close();
            }
            _socket.Close();
        }
    }
}
