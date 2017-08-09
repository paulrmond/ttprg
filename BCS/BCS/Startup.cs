using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BCS.Startup))]
namespace BCS
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
