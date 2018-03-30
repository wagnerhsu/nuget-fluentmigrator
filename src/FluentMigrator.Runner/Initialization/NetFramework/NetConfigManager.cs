#if NET40 || NET45
using System;
using System.Configuration;
using System.IO;

namespace FluentMigrator.Runner.Initialization.NetFramework
{
    internal class NetConfigManager
        : INetConfigManager
    {
        public Configuration LoadFromFile(string path)
        {
            if (String.IsNullOrEmpty(path) || !File.Exists(path))
                throw new ArgumentException("Specified configuration file path does not exist", "path");

            string configFile = path.Trim();

            if (!configFile.EndsWith(".config", StringComparison.InvariantCultureIgnoreCase))
                configFile += ".config";

            var fileMap = new ExeConfigurationFileMap { ExeConfigFilename = configFile };

            return ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);
        }

        public System.Configuration.Configuration LoadFromMachineConfiguration()
        {
            return ConfigurationManager.OpenMachineConfiguration();
        }
    }
}
#endif
