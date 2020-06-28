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

using UnifyResponse.Filter;
using UnifyResponse.Middlewar;
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="services"></param>
        public void ConfigureServices(IServiceCollection services)
        {
            //�����Դ������ 
            services.AddControllersWithViews(option =>
            {
                //option.Filters.Add<ResourceFilter>();
                option.Filters.Add<ExceptionFilter>();
            });

            //�������ݽ���ѹ��
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="app"></param>
        /// <param name="env"></param>
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //�м��˳��
            // �쳣 / ������
            // ��̬�ļ�����
            // �����֤
            // MVC
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            #region ע��exceptionless

            ExceptionlessClient.Default.Configuration.ApiKey = Configuration.GetSection("ExceptionLess:AppKey").Value;
            Console.WriteLine(Configuration.GetSection("ExceptionLess:AppKey").Value);
            ExceptionlessClient.Default.Configuration.ServerUrl = Configuration.GetSection("ExceptionLess:ServerUrl").Value;
            Console.WriteLine(Configuration.GetSection("ExceptionLess:ServerUrl").Value);
            app.UseExceptionless();

            #endregion

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();


            app.UseMiddleware(typeof(LoggerMiddleware));
            //app.UseMiddleware(typeof(AppExceptionHandlerMiddleware)); //ע��˳���ϵ
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
