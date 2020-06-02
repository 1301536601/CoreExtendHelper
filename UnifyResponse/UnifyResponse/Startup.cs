using Exceptionless;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.PlatformAbstractions;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using UnifyResponse.Middlewar;
using UnifyResponse.Middlewar.Model;
using UnifyResponse.Unitl;

namespace UnifyResponse
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
            //返回内容进行压缩
            services.AddResponseCompression();
            services.AddControllers();
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("v1", new OpenApiInfo
                {
                    Version = "v1",
                    Title = "SwaggerDemo"
                });

                //Determine base path for the application.  
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                //Set the comments path for the swagger json and ui.  
                var xmlPath = Path.Combine(basePath, "UnifyResponse.xml");
                options.IncludeXmlComments(xmlPath);
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //中间件顺序
            // 异常 / 错误处理
            // 静态文件服务
            // 身份认证
            // MVC
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            #region 注入exceptionless

            ExceptionlessClient.Default.Configuration.ApiKey = Configuration.GetSection("ExceptionLess:AppKey").Value;
            Console.WriteLine(Configuration.GetSection("ExceptionLess:AppKey").Value);
            ExceptionlessClient.Default.Configuration.ServerUrl = Configuration.GetSection("ExceptionLess:ServerUrl").Value;
            Console.WriteLine(Configuration.GetSection("ExceptionLess:ServerUrl").Value);
            app.UseExceptionless();

            #endregion

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();


            //app.UseMiddleware(typeof(LoggerMiddleware));
            //app.UseMiddleware(typeof(AppExceptionHandlerMiddleware)); //注意顺序关系
            app.UseMiddleware<AutomaticParamMiddleware>(15.33d, 8.96d);
            app.UseResponseCompression();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "MsSystem API V1");
            });
        }
    }
}
