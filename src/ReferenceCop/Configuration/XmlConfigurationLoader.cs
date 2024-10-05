namespace ReferenceCop
{
    using System;
    using System.Collections.Generic;
    using System.Text;

    internal class XmlConfigurationLoader : IConfigurationLoader
    {
        private const string ReferenceCopConfigPath = "ReferenceCop.config";

        private readonly string configAsString;

        public XmlConfigurationLoader(CompilationAnalysisContext compilationAnalysisContext) 
        {
            this.configAsString = GetConfigAsString(
                ReferenceCopConfigPath, compilationAnalysisContext.Options.AdditionalFiles, compilationAnalysisContext.CancellationToken);
        }

        public ReferenceCopConfig Load()
        {
            var config = ParseConfigFrom(this.configAsString);

            return config;
        }

        private static ReferenceCopConfig GetConfigAsStringFrom(XmlDocument xmlDocument)
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
