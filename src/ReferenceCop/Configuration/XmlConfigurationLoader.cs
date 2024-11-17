namespace ReferenceCop
{
    using Microsoft.CodeAnalysis;
    using Microsoft.CodeAnalysis.Diagnostics;
    using System;
    using System.Collections.Immutable;
    using System.IO;
    using System.Linq;
    using System.Text;
    using System.Threading;
    using System.Xml;
    using System.Xml.Serialization;

    public class XmlConfigurationLoader : IConfigurationLoader
    {
        private const string ReferenceCopConfigPath = "ReferenceCop.config";

        private readonly string configAsString;

        public XmlConfigurationLoader(CompilationAnalysisContext compilationAnalysisContext) 
        {
            this.configAsString = GetConfigAsStringFrom(
                ReferenceCopConfigPath, compilationAnalysisContext.Options.AdditionalFiles, compilationAnalysisContext.CancellationToken);
        }

        internal XmlConfigurationLoader(XmlDocument xmlDocument)
        {
            this.configAsString = GetConfigAsStringFrom(xmlDocument);
        }

        public ReferenceCopConfig Load()
        {
            var config = ParseConfigFrom(this.configAsString);

            return config;
        }

        private static string GetConfigAsStringFrom(XmlDocument xmlDocument)
        {
            return xmlDocument.OuterXml;
        }

        private static string GetConfigAsStringFrom(string configFilePath, ImmutableArray<AdditionalText> additionalFiles, CancellationToken cancellationToken)
        {
            var illegalReferencesFile = additionalFiles.FirstOrDefault(file => Path.GetFileName(file.Path).Equals(configFilePath));

            if (illegalReferencesFile == null)
            {
                throw new InvalidOperationException("Configuration file containing illegal references was not found.");
            }

            var sourceText = illegalReferencesFile.GetText(cancellationToken);
            return sourceText.ToString();
        }

        private static ReferenceCopConfig ParseConfigFrom(string configAsString)
        {
            var xmlSerializer = new XmlSerializer(typeof(ReferenceCopConfig));
            using (var reader = new MemoryStream(Encoding.UTF8.GetBytes(configAsString)))
            {
                return (ReferenceCopConfig)xmlSerializer.Deserialize(reader);
            }
        }
    }
}
