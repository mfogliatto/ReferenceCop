namespace ReferenceCop
{
    using System;

    public class ConfigurationFileNotFoundException : Exception
    {
        public ConfigurationFileNotFoundException()
            : base("ReferenceCop configuration file was not found or defined in the AdditionalFiles property.")
        {
        }
    }
}
