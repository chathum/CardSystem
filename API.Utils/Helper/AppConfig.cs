using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.Configuration;

public static class AppConfig
{
    private static IConfiguration _config;

    static AppConfig()
    {
        _config = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();
    }

    public static string Get(string key)
    {
        return _config[key];
    }
}
