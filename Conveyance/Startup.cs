using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(Conveyance.Startup))]
namespace Conveyance
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            app.MapSignalR();
            ConfigureAuth(app);
        }
    }
}
