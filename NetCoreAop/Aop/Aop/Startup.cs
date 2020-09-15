using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Aop.IService;
using Aop.Service;

using Autofac;
using Autofac.Extensions.DependencyInjection;
using Autofac.Extras.DynamicProxy;

using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Aop
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
            services.AddControllers();
        }


        /// <summary>
        /// AutoFac
        /// </summary>
        /// <param name="builder"></param>
        public void ConfigureContainer(ContainerBuilder builder)
        {
            //builder.RegisterType<MyService>().As<IMyService>();

            #region ����ע��

            //�������ע������
            //builder.RegisterType<MyNameService>();
            //��������ע��
            //builder.RegisterType<MyServiceV2>().Named<IMyService>("MyService2").PropertiesAutowired();

            #endregion

            //ע��������
            builder.RegisterType<MyInterceptor>();
            builder.RegisterType<MyService>().As<IMyService>().PropertiesAutowired().InterceptedBy(typeof(MyInterceptor)).EnableInterfaceInterceptors();

        }

        /// <summary>
        /// ע��
        /// </summary>
        private ILifetimeScope autofacContain { get; set; }


        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            //����autoFac �������Ƶ�
            //autofacContain = app.ApplicationServices.GetAutofacRoot();
            //var serviceV2 = autofacContain.ResolveNamed<IMyService>("MyService2");
            //serviceV2.ShowCode();

            //����autoFac ���������Ƶ�
            autofacContain = app.ApplicationServices.GetAutofacRoot();
            var service = autofacContain.Resolve<IMyService>();
            service.ShowCode();



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
