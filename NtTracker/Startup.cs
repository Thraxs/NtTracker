using Microsoft.Owin;
using Owin;

[assembly: OwinStartup(typeof(NtTracker.Startup))]
namespace NtTracker
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}