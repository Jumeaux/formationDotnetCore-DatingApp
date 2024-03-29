using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using API.Extensions;
using API.Middleware;
using API.SignalR;

namespace API
{
    public class Startup
    {
        private IConfiguration _config;

        public Startup(IConfiguration configuration)
        {
            _config = configuration;
        }

    

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {  
            
            services.AddApplicationServices(_config);
            services.AddControllers();
            services.AddCors();
            services.AddIdentityService(_config);
            services.AddSignalR();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
           /*  if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }*/

            app.UseMiddleware<ExceptionMiddleware>();
    
            app.UseHttpsRedirection();

            app.UseRouting();
            
            app.UseCors(x=>x.AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials()
            .WithOrigins("https://localhost:4200"));

            app.UseAuthentication();
            app.UseAuthorization();

            app.UseDefaultFiles();
            app.UseStaticFiles();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapHub<PresenceHub>("hubs/presence");
                endpoints.MapHub<MessageHub>("hubs/messages");
                endpoints.MapFallbackToController("Index","Falback");
            });
        }
    }
}
