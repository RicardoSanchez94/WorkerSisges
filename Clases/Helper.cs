using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Text;

namespace WorkerServiceSisges.Clases
{
    public class Helper
    {
        public static ApiSiges Endpoints()
        {
            var model = new ApiSiges();
            var builder = new ConfigurationBuilder()
               //.SetBasePath(Directory.GetCurrentDirectory())
               .SetBasePath(AppContext.BaseDirectory)
              .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true);
            IConfiguration configuration = builder.Build();
            model = configuration.GetSection("ApiSisges").Get<ApiSiges>();
            return model;
        }
    }
}
