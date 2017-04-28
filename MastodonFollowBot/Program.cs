using System;
using MastodonFollowBot.Model;
using MastodonFollowBot.Settings;
using MastodonFollowBot.Tools;

namespace MastodonFollowBot
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("MASTODON FOLLOW BOT");
            Console.WriteLine("Please use this for testing purpose");
            Console.WriteLine();
            
            //Read Config File
            var configFileHandler = new ConfigFileHandler("MastodonFollowBot", "appconfig.json");
            var config = GetAppSettings(configFileHandler);

            //Launch follow bot
            var followBot = new FollowBot(config);
            followBot.Run();

            Console.ReadLine();
        }

        private static AppSettings GetAppSettings(ConfigFileHandler configFileHandler)
        {
            try
            {
                var config = configFileHandler.ReadConfigFile<AppSettings>();
                return config;
            }
            catch (Exception e)
            {
                var settings = new AppSettings()
                {
                    Source = new AppAccount(),
                    Target = new AppAccount(),
                };
                configFileHandler.WriteConfigFile(settings);

                Console.WriteLine("Please set your config file in AppData\\Local\\MastodonFollowBot");
                Console.ReadLine();
                throw;
            }
        }
    }
}