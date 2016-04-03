using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace TestHarness.TestHarnesses.SocketHarness
{
    public class SocketHarnessMessage
    {
        private const char MessageTypeSeperator = '|';
        private const char _terminator = '<';

        public static char Terminator
        {
            get { return _terminator; }
        }

        public static MessageResponse ProcessMessage(IAsyncResult ar)
        {
            var socketState = (SocketState) ar.AsyncState;
            var buffer = socketState.ByteBuffer;

            var messageTypeInt = Encoding.Unicode.GetString(buffer.TakeWhile(x => x != MessageTypeSeperator).ToArray());
            var trimmed = buffer.Skip(4).TakeWhile(x => x != Terminator);
            var line = Encoding.Unicode.GetString(trimmed.ToArray());

            var messageType = (MessageType)Int32.Parse(messageTypeInt);

            return new MessageResponse
            {
                MessageType = messageType,
                Message = line
            };
        }

        public static void SendMessage(Socket socket, MessageType messageType, String message)
        {
            SendMessage(socket, messageType, message, SendCallBack);
        }

        public static void SendMessage(Socket socket, MessageType messageType, String message, AsyncCallback callback)
        {
            var messageBytes = Encoding.Unicode.GetBytes(((int)messageType).ToString() + MessageTypeSeperator + message + Terminator);
            var socketState = new SocketState
            {
                Socket = socket,
                ByteBuffer = new byte[message.Length]
            };

            socket.BeginSend(messageBytes, 0, messageBytes.Length, SocketFlags.None, callback, socketState);
        }

        private static void SendCallBack(IAsyncResult ar)
        {
            var socketState = (SocketState)ar.AsyncState;
            var socket = socketState.Socket;

            socket.EndSend(ar);
        }

        public class MessageResponse
        {
            public MessageType MessageType { get; set; }
            public String Message { get; set; }
        }

        public enum MessageType
        {
            RegistrationPort = 1,
            GameMap = 2,
            Command = 3,
            Killed = 4,
            RegisterPlayer = 5,
            CommandFailed = 6,
            GameComplete = 7,
            PlayerRegistered = 8
        }
    }
}
