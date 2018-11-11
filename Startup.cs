using System;
using System.IO;
using System.Net;
using System.Net.Http;

using AutoMapper;

using Hangfire;
using Hangfire.Mongo;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using Q.Squid.Config;
using Q.Squid.DAL;
using Q.Squid.Jobs;
using Q.Squid.Models;
using Q.Squid.Store;

using Swashbuckle.AspNetCore.Examples;
using Swashbuckle.AspNetCore.Swagger;

namespace Q.Squid
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IHostingEnvironment env)
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(env.ContentRootPath)
                .AddJsonFile("appsettings.json", optional: false)
                .AddJsonFile($"appsettings.{env.EnvironmentName}.json", optional: false)
                .AddEnvironmentVariables();

            Configuration = builder.Build();
            SquidConfig.BranchName = Configuration["BRANCH_NAME"] ?? "depupdate-squid";
            SquidConfig.CryptoKey = Configuration["CRYPTO_KEY"] ?? throw new ArgumentNullException("The 'CRYPTO_KEY' setting or environment variable is required.");

            SquidConfig.DependencyJobInterval = Configuration["DEPENDENCY_JOB_INTERVAL"] != null ? int.Parse(Configuration["DEPENDENCY_JOB_INTERVAL"]) : 5;
            SquidConfig.PullRequestJobInterval = Configuration["PULLREQUEST_JOB_INTERVAL"] != null ? int.Parse(Configuration["PULLREQUEST_JOB_INTERVAL"]) : 3;
            SquidConfig.NuGetJobInterval = Configuration["NUGET_JOB_INTERVAL"] != null ? int.Parse(Configuration["NUGET_JOB_INTERVAL"]) : 5;
            SquidConfig.PrivateFeedJobInterval = Configuration["PRIVATEFEED_JOB_INTERVAL"] != null ? int.Parse(Configuration["PRIVATEFEED_JOB_INTERVAL"]) : 5;
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddMvc(options =>
                {
                    options.Filters.Add(new ProducesAttribute("application/json"));
                    options.Filters.Add(new ConsumesAttribute("application/json"));
                }).AddJsonOptions(options =>
                {
                    // Setup json serializer
                    options.SerializerSettings.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                }).SetCompatibilityVersion(CompatibilityVersion.Version_2_1);

            string connectionString = Configuration["MONGODB_CONNECTION_STRING"] ?? "mongodb://127.0.0.1:27017";
            string databaseName = Configuration["MONGODB_DATABASE_NAME"] ?? "squid";
            var repo = new RepositoryConfigRepository(connectionString, databaseName);
            services.AddSingleton(repo);

            var feedRepo = new NugetFeedConfigRepository(connectionString, databaseName);
            services.AddSingleton(feedRepo);

            services.AddSingleton<HttpClient>();

            services.AddHangfire(x => x.UseMongoStorage(connectionString, databaseName));

            // Add the mapping.
            services.AddSingleton(MappingHelper.GetMapper());

            // In production, the Angular files will be served from this directory
            services.AddSpaStaticFiles(configuration =>
            {
                configuration.RootPath = "ClientApp/dist";
            });

            var filePath = Path.Combine(System.AppContext.BaseDirectory, "squid.xml");
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info { Title = "squid API", Version = "v1" });
                c.IncludeXmlComments(filePath);
                c.OperationFilter<ExamplesOperationFilter>();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app,
            IHostingEnvironment env,
            RepositoryConfigRepository repo,
            NugetFeedConfigRepository feedsRepo,
            ILoggerFactory loggerFactory,
            HttpClient client,
            IMapper mapper)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            var path = env.ContentRootPath;

            SquidStore.Dependencies = new System.Collections.Concurrent.ConcurrentDictionary<string, Dependencies>();
            SquidStore.Packages = new System.Collections.Concurrent.ConcurrentDictionary<string, PackageDetails>();

            app.UseHangfireServer();
            app.UseHangfireDashboard();

            app.UseStaticFiles();
            app.UseSpaStaticFiles();

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "squid API");
            });

            app.UseMvc(routes =>
            {
                routes.MapRoute(
                    name: "default",
                    template: "{controller}/{action=Index}/{id?}");
            });

            app.UseSpa(spa =>
            {
                spa.Options.SourcePath = "ClientApp";

                if (env.IsDevelopment())
                {
                    spa.UseAngularCliServer(npmScript: "start");
                }
            });

            // Run the jobs once now.
            BackgroundJob.Enqueue(
                () => new DependencyJob(
                    loggerFactory.CreateLogger<DependencyJob>(),
                    repo,
                    client,
                    mapper).Execute());

            BackgroundJob.Enqueue(
                () => new PrivateNuGetFeedJob(
                    loggerFactory.CreateLogger<PrivateNuGetFeedJob>(),
                    client,
                    feedsRepo).Execute());

            // Start the recurring jobs.
            RecurringJob.AddOrUpdate(
                DependencyJob.GetName(),
                () => new DependencyJob(
                    loggerFactory.CreateLogger<DependencyJob>(),
                    repo,
                    client,
                    mapper).Execute(),
                $"*/{SquidConfig.DependencyJobInterval} * * * *");

            RecurringJob.AddOrUpdate(
                NuGetFeedJob.GetName(),
                () => new NuGetFeedJob(
                    loggerFactory.CreateLogger<NuGetFeedJob>(),
                    client).Execute(),
                $"*/{SquidConfig.NuGetJobInterval} * * * *");

            RecurringJob.AddOrUpdate(
                PrivateNuGetFeedJob.GetName(),
                () => new PrivateNuGetFeedJob(
                    loggerFactory.CreateLogger<PrivateNuGetFeedJob>(),
                    client,
                    feedsRepo).Execute(),
                $"*/{SquidConfig.PrivateFeedJobInterval} * * * *");

            RecurringJob.AddOrUpdate(
                CheckPullRequestStatusJob.GetName(),
                () => new CheckPullRequestStatusJob(
                    loggerFactory.CreateLogger<CheckPullRequestStatusJob>(),
                    repo,
                    client).Execute(),
                $"*/{SquidConfig.PullRequestJobInterval} * * * *");
        }
    }
}
