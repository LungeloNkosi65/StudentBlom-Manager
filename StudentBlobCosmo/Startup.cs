using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(StudentBlobCosmo.Startup))]
namespace StudentBlobCosmo
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
