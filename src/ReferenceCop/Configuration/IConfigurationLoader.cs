using System;
using System.Collections.Generic;
using System.Text;

namespace ReferenceCop.Configuration
{
    internal interface IConfigurationLoader
    {
        ReferenceCopConfig Load();
    }
}
