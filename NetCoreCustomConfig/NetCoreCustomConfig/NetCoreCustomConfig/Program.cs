using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Primitives;

using NetCoreCustomConfig.Common;

namespace NetCoreCustomConfig
{
    /// <summary>
    /// �Զ����ȡ������   ��Ф�ļ���ʱ��̳�
    /// </summary>
    public class Program
    {
        public static void Main(string[] args)
        {
            CreateHostBuilder(args).Build().Run();
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .ConfigureHostConfiguration(configure =>
                {
                    //ʹ��
                    configure.AddMyConfiguration();
                    var config = configure.Build();

                    ChangeToken.OnChange(() => config.GetReloadToken(), () =>
                     {
                         Console.WriteLine(config["lastTime"]);
                     });
                    //�Զ������ÿ������
                    Console.WriteLine("��ʼ��");
                })
            ;
    }
}
