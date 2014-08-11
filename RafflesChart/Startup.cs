using Microsoft.Owin;
using Owin;

[assembly: OwinStartupAttribute(typeof(RafflesChart.Startup))]
namespace RafflesChart
{
    public partial class Startup
    {
        public void Configuration(IAppBuilder app)
        {
            ConfigureAuth(app);
        }
    }
}
