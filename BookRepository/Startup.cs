using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using BookRepository.Data;
using Microsoft.Extensions.FileProviders;
using Microsoft.AspNetCore.Mvc.Versioning;
using BookRepository.Controllers;

namespace BookRepository
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

            _ = services.AddControllers();
            _ = services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "BookRepository", Version = "v1" });
            });

            _ = services.AddDbContextPool<BookRepoDbContext>(dbContextOptns =>
            {
                _ = dbContextOptns.UseSqlServer(
                    Configuration.GetConnectionString("BookConn"));
            });

            _ = services.AddScoped<IBookData, SqlData>();

            _ = services.AddApiVersioning(o =>
            {
                o.AssumeDefaultVersionWhenUnspecified = true;
                o.DefaultApiVersion = new ApiVersion(1, 1);
                o.ReportApiVersions = true;
                //////o.ApiVersionReader = new QueryStringApiVersionReader("v");
                //////o.ApiVersionReader = new HeaderApiVersionReader("x-version");
                o.ApiVersionReader = ApiVersionReader.Combine(
                    new HeaderApiVersionReader("x-version")
                    , new QueryStringApiVersionReader("version", "ver", "v"));
                //o.ApiVersionReader = new UrlSegmentApiVersionReader();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                _ = app.UseDeveloperExceptionPage();
                _ = app.UseSwagger();
                _ = app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "BookRepository v1"));
            }

            _ = app.UseHttpsRedirection();

            _ = app.UseRouting();

            _ = app.UseAuthorization();

            _ = app.UseEndpoints(endpoints =>
            {
                _ = endpoints.MapControllers();
            });
        }
    }
}
