using System;
using Microsoft.AspNet.SignalR.Client;

namespace ConsoleClient
{
    internal class Program
    {
        private const string ServerUrl = "http://localhost:54121/";

        private static readonly HubConnection Connection = new HubConnection(ServerUrl);
        private static IHubProxy _hubProxy;
        private static string _userName = string.Empty;

        private static void Main()
        {
            Console.Title = "ChatClient";           
            Console.Write("Enter your nickname: ");
            _userName = Console.ReadLine();
            Console.Clear();

            IntitalizeChatConnection();
            Console.WriteLine("Connected, now you can write message to chat (press enter to send) write '/exit' to exit from chat" + Environment.NewLine);

            while (ExecuteChatCommand(Console.ReadLine())) { }
        }
        /// <summary>
        /// Intialize connection to chat server and events
        /// </summary>
        private static void IntitalizeChatConnection()
        {
            _hubProxy = Connection.CreateHubProxy("ChatHub");
            _hubProxy.On<string, string>("SendMessage", OnMessageRecived);
            _hubProxy.On<string>("Join", OnUserConnected);
            _hubProxy.On<string, string>("Leave", OnUserDisconnected);
            Connection.Start().Wait();

            Join();
        }

        /// <summary>
        /// Process command written in console
        /// </summary>
        /// <param name="data">command</param>
        /// <returns>Execute next command</returns>
        private static bool ExecuteChatCommand(string data)
        {
            //Close chat connection and program if user write "/exit"
            if (data.Equals("/exit"))
            {
                Connection.Stop();
                return false;
            }

            //send message to server
            SendMessage(data);
            return true;
        }

        /// <summary>
        /// Send "Join" command to chat server
        /// </summary>
        private static void Join()
        {
            _hubProxy.Invoke("Join", _userName);
        }

        /// <summary>
        /// Send message to chat server
        /// </summary>
        /// <param name="message"></param>
        private static void SendMessage(string message)
        {
            _hubProxy.Invoke("Send", _userName, message);

            //Replace written message with Username: message
            Console.SetCursorPosition(0, Console.CursorTop - 1);
            Console.Write($"{_userName}: {message}");
            Console.SetCursorPosition(0, Console.CursorTop + 1);
        }

        /// <summary>
        /// Recived message callback
        /// </summary>
        /// <param name="name">Sender name</param>
        /// <param name="message">Message</param>
        private static void OnMessageRecived(string name, string message)
        {
            Console.WriteLine($"{name}: {message}");
        }

        /// <summary>
        /// User connected callback
        /// </summary>
        /// <param name="name">Connected user name</param>
        private static void OnUserConnected(string name)
        {
            Console.WriteLine($"{name} joind in chat");
        }

        /// <summary>
        /// User disconnected callback
        /// </summary>
        /// <param name="name">Disconnected user name</param>
        /// <param name="connectionId">Disconnected user connection id</param>
        private static void OnUserDisconnected(string name, string connectionId)
        {
            Console.WriteLine($"{name} left from chat");
        }
    }
}
