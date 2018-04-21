using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(INCISE2018_MVC.Startup))]
namespace INCISE2018_MVC
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
