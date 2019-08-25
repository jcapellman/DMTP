using DMTP.lib.dal.Databases;
using DMTP.lib.dal.Manager;
using DMTP.lib.Managers;

using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpOverrides;

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using IHostingEnvironment = Microsoft.AspNetCore.Hosting.IHostingEnvironment;

namespace DMTP.REST
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddOptions();

            services.AddSingleton(Helpers.AssemblyReader.LoadAssemblies());

            var dbManager = new DatabaseManager(new LiteDBDatabase(), new InMemoryCache());
            
            services.AddSingleton(dbManager);
            services.AddSingleton(new SettingManager(dbManager).GetSettings());

            services.AddMvc();

            services.Configure<FormOptions>(options => options.MultipartBodyLengthLimit = long.MaxValue);

            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
                .AddCookie(a => a.LoginPath = "/Account");
        }

        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });

            app.UseDeveloperExceptionPage();
          
            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Account}/{action=Index}/{id?}");
            });
        }
    }
}