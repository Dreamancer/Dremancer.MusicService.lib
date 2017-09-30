using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MusicWebApi.ApiWrappers;
using MusicWebApi.MusicRepo;
using MusicWebApi.DB;

namespace MusicWebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
            using (var client = new WebDbContext())
            {
                client.Database.EnsureCreated();
            }
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc();
            services.AddSingleton(Configuration);
            services.AddTransient<IMusicApiWrapper, LastFmWrapper>();
            services.AddTransient<IMusicApiWrapper, SpotifyWrapper>();
            services.AddTransient<IMusicApiWrapper, MusicBrainzWrapper>();
            services.AddTransient<IWebRepo, WebRepo>();
            //services.AddTransient<ILocalRepo, LocalRepo>();
            services.AddEntityFrameworkSqlite().AddDbContext<WebDbContext>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseMvc();
        }
    }
}
