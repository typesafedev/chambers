using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Publish.Api.Services;
using Publish.Core.Interfaces;
using System.IO.Abstractions;
using System.Reflection;

namespace Publish.Api
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
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Publish.Api", Version = "v1" });
            });

            services.AddHttpContextAccessor();
            services.AddDbContext<PublishContext>(
                options => options.UseSqlite(Configuration.GetConnectionString("PublishDb")));

            // Scans Startup assembly and added handlers, preprocessors, postprocessors implementations to container
            services.AddMediatR(typeof(Startup).GetTypeInfo().Assembly);

            services.AddScoped<IFileValidationService, FileValidationService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IFileSystem, FileSystem>();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Publish.Api v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
