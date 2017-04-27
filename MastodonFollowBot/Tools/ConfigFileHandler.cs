using System;
using System.Dynamic;
using System.IO;
using System.Runtime.InteropServices;
using Newtonsoft.Json;

namespace MastodonFollowBot.Tools
{
    public class ConfigFileHandler
    {
        private readonly string _appName;
        private readonly string _configFileName;
        private DirectoryInfo _configFolder;
        private FileInfo _fileInfo;

        #region Ctor
        public ConfigFileHandler(string appName, string configFileName)
        {
            _appName = appName;
            _configFileName = configFileName;

            CheckIfAppFolderExistsAndCreate();
        }
        #endregion

        private void CheckIfAppFolderExistsAndCreate()
        {
            var userFolder = Environment.GetEnvironmentVariable(RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "LocalAppData" : "Home");
            var appFolderFullName = userFolder + $@"\{_appName}";

            if (!Directory.Exists(appFolderFullName)) Directory.CreateDirectory(appFolderFullName);

            _configFolder = new DirectoryInfo(appFolderFullName);
            _fileInfo = new FileInfo(_configFolder.FullName + $@"\{_configFileName}");
        }

        public T ReadConfigFile<T>()
        {
            var configData = File.ReadAllText(_fileInfo.FullName);
            return JsonConvert.DeserializeObject<T>(configData);
        }

        public void WriteConfigFile<T>(T configObject)
        {
            var textData = JsonConvert.SerializeObject(configObject);
            File.WriteAllText(_fileInfo.FullName, textData);
        }
    }
}