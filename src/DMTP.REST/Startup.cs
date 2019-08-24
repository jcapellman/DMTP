using DMTP.lib.Databases;
using DMTP.lib.Databases.Base;
using DMTP.REST.InterfaceImplementations;

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

            var db = new LiteDBDatabase();

            services.AddSingleton(new InMemoryCaching());
            services.AddSingleton(db.GetSettings());

            services.AddSingleton<IDatabase>(db);

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