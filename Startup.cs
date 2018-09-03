using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using ProjetoSaude.Data;
using ProjetoSaude.Manager;
using ProjetoSaude.Models;

namespace ProjetoSaude
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {


            /*Use para Banco de Dados InMemory*/
            services.AddEntityFrameworkInMemoryDatabase().AddDbContext<IDatabaseContext>(options =>
            {
                options.UseInMemoryDatabase("SaudeDb");
            });


            /*
             * Use para Banco de Dados MySql.
             * Connection String definida no appsettings.json
             */
            services.AddDbContext<IDatabaseContext>(options =>
            {
                options.UseMySql(Configuration["ConnectionString"]);
            });


            /*
             * Adiciona o serviço de autenticação por Cookie
             *
            services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme).AddCookie(CookieAuthenticationDefaults.AuthenticationScheme,
            options =>
            {
                    options.LoginPath = "/Account/Login";
                    options.LogoutPath = "/Account/Logout";

            }); */
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            });

            services.AddMvc();
                
            /*
             * Adiciona um "service"custom que é instanciado no construtor de cada Controller
             */
            services.AddTransient(m => new AppManager());
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseBrowserLink();
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseStaticFiles();
            app.UseAuthentication();

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");
            });

        }
    }
}
