using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(BeadKeychainDesignPlatform.Startup))]
namespace BeadKeychainDesignPlatform
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
