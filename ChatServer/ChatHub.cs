using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ChatServer.Models;
using Microsoft.AspNet.SignalR;

namespace ChatServer
{
    public class ChatHub : Hub
    {
        /// <summary>
        /// Users list in current pool session
        /// </summary>
        private static readonly List<ChatUser> Users = new List<ChatUser>();

        /// <summary>
        /// Send message to all users except sender
        /// </summary>
        /// <param name="name">Name of sender</param>
        /// <param name="message">Sended message</param>
        public void Send(string name, string message)
        {
            //invoke "callback" on client
            Clients.Others.SendMessage(name, message);
        }

        /// <summary>
        /// Send all users "Join" command when some new user connected to chat server
        /// </summary>
        /// <param name="name">Connected user name</param>
        public void Join(string name)
        {
            //Check to user already exist in users list to prevent duplicates
            if (Users.Find(u => u.ConnectionId.Equals(Context.ConnectionId)) == null)
            {
                //add new user in list
                Users.Add(new ChatUser { ConnectionId = Context.ConnectionId, Name = name });
                //invoke "callback" on client
                Clients.All.Join(name, Context.ConnectionId);
            }           
        }

        /// <summary>
        /// Send all users "Leave" command when user disconnected from server
        /// </summary>
        /// <param name="name">User name</param>
        /// <param name="id">User connection id</param>
        public void Leave(string name, string id)
        {
            //invoke "callback" on client
            Clients.All.Leave(name, id);
        }

        /// <summary>
        /// Get all connected users
        /// </summary>
        /// <returns>Users list</returns>
        public IEnumerable<ChatUser> GetUsersList()
        {
            return Users.Where(user => user.ConnectionId != Context.ConnectionId);
        }

        /// <summary>
        /// Event which invokes when user closes connection to server. After that server must send "leave" command to other connected users
        /// </summary>
        /// <param name="stopCalled"></param>
        /// <returns></returns>
        public override Task OnDisconnected(bool stopCalled)
        {
            //Find if disconnected user exist in current users list to prevent exceptions
            var user = Users.Find(u => u.ConnectionId.Equals(Context.ConnectionId));
            if (user != null)
            {
                //Remove user from users list
                Users.Remove(user);
                Leave(user.Name, user.ConnectionId);
            }

            return base.OnDisconnected(stopCalled);
        }
    }
}