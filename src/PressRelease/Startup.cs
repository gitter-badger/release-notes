﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AspNet.Security.OAuth.GitHub;
using Microsoft.AspNet.Authentication.Cookies;
using Microsoft.AspNet.Builder;
using Microsoft.AspNet.Hosting;
using Microsoft.AspNet.Http;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Data.Entity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using PressRelease.Models;

namespace PressRelease
{
	public class Startup
	{
		public Startup( IHostingEnvironment env )
		{
			// Set up configuration sources.
			var builder = new ConfigurationBuilder()
				.AddJsonFile( "appsettings.json" )
				.AddJsonFile( $"appsettings.{env.EnvironmentName}.json", optional: true );

			if ( env.IsDevelopment() )
			{
				// For more details on using the user secret store see http://go.microsoft.com/fwlink/?LinkID=532709
				builder.AddUserSecrets();
			}

			builder.AddEnvironmentVariables();
			Configuration = builder.Build();
		}

		public IConfigurationRoot Configuration { get; set; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices( IServiceCollection services )
		{
			// Add framework services.
			services.AddEntityFramework()
				.AddSqlServer()
				.AddDbContext<ApplicationDbContext>( options =>
					 options.UseSqlServer( Configuration["Data:DefaultConnection:ConnectionString"] ) );

			services.AddIdentity<ApplicationUser, IdentityRole>()
				.AddEntityFrameworkStores<ApplicationDbContext>()
				.AddDefaultTokenProviders();

			services.AddMvc();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure( IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory )
		{
			loggerFactory.AddConsole( Configuration.GetSection( "Logging" ) );
			loggerFactory.AddDebug();

			if ( env.IsDevelopment() )
			{
				app.UseBrowserLink();
				app.UseDeveloperExceptionPage();
				app.UseDatabaseErrorPage();
			}
			else
			{
				app.UseExceptionHandler( "/Home/Error" );

				// For more details on creating database during deployment see http://go.microsoft.com/fwlink/?LinkID=615859
				try
				{
					using ( var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>()
						.CreateScope() )
					{
						serviceScope.ServiceProvider.GetService<ApplicationDbContext>()
							 .Database.Migrate();
					}
				}
				catch { }
			}

			app.UseIISPlatformHandler( options => options.AuthenticationDescriptions.Clear() );

			app.UseStaticFiles();

			app.UseIdentity();

			app.UseCookieAuthentication(
				new CookieAuthenticationOptions
				{
					AutomaticAuthenticate = true,
					AutomaticChallenge = true,
					AuthenticationScheme = CookieAuthenticationDefaults.AuthenticationScheme,
					LoginPath = new PathString( "/Account/Login" )
				} );

			app.UseGitHubAuthentication(
				new GitHubAuthenticationOptions
				{
					ClientId = "f862c3e7ec79cbe519e4",
					ClientSecret = "d42344973ba23706d18b4c17ef33759df8f2b8f4",
					Scope = { "user:email", "repo" },
					CallbackPath = "/Home/Index"
				} );

			app.UseMvc( routes =>
			 {
				 routes.MapRoute(
					 name: "default",
					 template: "{controller=Home}/{action=Index}/{id?}" );
			 } );
		}

		// Entry point for the application.
		public static void Main( string[] args ) => WebApplication.Run<Startup>( args );
	}
}
