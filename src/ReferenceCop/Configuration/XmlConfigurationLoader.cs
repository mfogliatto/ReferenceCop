namespace ReferenceCop
{
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Xml;
    using System.Xml.Serialization;
    using Microsoft.CodeAnalysis.Diagnostics;

    public class XmlConfigurationLoader : IConfigurationLoader
    {
        private const string ReferenceCopConfigPath = "ReferenceCop.config";

        private readonly ReferenceCopConfig loadedConfig;

        public XmlConfigurationLoader(CompilationAnalysisContext compilationAnalysisContext)
        {
            var configFile = compilationAnalysisContext.Options.AdditionalFiles.FirstOrDefault(file => Path.GetFileName(file.Path).Equals(ReferenceCopConfigPath));
            if (configFile == null)
            {
                throw new ConfigurationFileNotFoundException();
            }

            var configFilePath = configFile.Path;
            using (var stream = new FileStream(configFilePath, FileMode.Open))
            {
                this.loadedConfig = ParseConfigFrom(stream);
            }
        }

        public XmlConfigurationLoader(string configFilePath)
        {
            using (var stream = new FileStream(configFilePath, FileMode.Open))
            {
                this.loadedConfig = ParseConfigFrom(stream);
            }
        }

        internal XmlConfigurationLoader(XmlDocument xmlDocument)
        {
            using (var stream = new MemoryStream(Encoding.UTF8.GetBytes(xmlDocument.OuterXml)))
            {
                this.loadedConfig = ParseConfigFrom(stream);
            }
        }

        public ReferenceCopConfig Load()
        {
            return this.loadedConfig;
        }

        private static ReferenceCopConfig ParseConfigFrom(Stream stream)
        {
            var xmlSerializer = new XmlSerializer(typeof(ReferenceCopConfig));

            return (ReferenceCopConfig)xmlSerializer.Deserialize(stream);
        }
    }
}
