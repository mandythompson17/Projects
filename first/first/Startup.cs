using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(first.Startup))]
namespace first
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
