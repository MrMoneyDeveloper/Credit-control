using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(eBaseApp.Startup))]
namespace eBaseApp
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
