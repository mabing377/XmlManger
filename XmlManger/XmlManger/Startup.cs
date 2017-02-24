using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(XmlManger.Startup))]
namespace XmlManger
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
