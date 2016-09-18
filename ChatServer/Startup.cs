using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(ChatServer.Startup))]
namespace ChatServer
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
        }
    }
}
