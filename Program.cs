using System;
using System.Text;
using System.Net;
using System.Net.Sockets;

namespace TcpServer
{
    class Program
    {
        private const string IpAddress = "127.0.0.1";
        private const int Port = 30001;

        private const int MaxListeningConnections = 1;
        private const string EndConnectionPhrase = "EOF";
        private const int MessageSize = 1024;

        static void Main()
        {
            var ipAddress = IPAddress.Parse(IpAddress);
            var endPoint = new IPEndPoint(ipAddress, Port);

            var listeningSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
            listeningSocket.Bind(endPoint);

            listeningSocket.Listen(MaxListeningConnections);
            Console.WriteLine("Waiting for connection...");
            var connectionSocket = listeningSocket.Accept();
            Console.WriteLine("The connection was established! \nWaiting for incoming messages btw...");

            while (true)
            {
                var receivedBytes = new byte[MessageSize];
                int numberOfReceivedBytes;

                try
                {
                    numberOfReceivedBytes = connectionSocket.Receive(receivedBytes);
                }
                catch
                {
                    Console.WriteLine("The connection was aborted! :(");
                    listeningSocket.Close();
                    connectionSocket.Close();
                    return;
                }

                var receivedMessage = Encoding.UTF8.GetString(receivedBytes, 0, numberOfReceivedBytes);

                if (receivedMessage.Equals(EndConnectionPhrase))
                {
                    break;
                }

                Console.WriteLine($"Received message: {receivedMessage}");
            }

            listeningSocket.Close();
            connectionSocket.Close();
        }
    }
}