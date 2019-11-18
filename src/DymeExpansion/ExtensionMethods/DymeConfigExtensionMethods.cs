using DymeExpansion.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace DymeExpansion.Core.ExtensionMethods
{
  public static class DymeConfigExtensionMethods
  {

    public static DymeConfig ConfigWithNameFromConfigWithNameProperty(this DymeConfig config, string nameProperty = null)
    {
      if (nameProperty == null) return config;
      Regex match = new Regex(nameProperty);
      var configNameProp = config.Properties.SingleOrDefault(p => match.IsMatch(p.Name));
      if (configNameProp == null)
        throw new FormatException($"A config is missing the mandatory identifier property: \"{nameProperty}\". Config: {config.Name ?? config.ToString()}");
      var otherProps = config.Properties.Where(p => !match.IsMatch(p.Name));
      return new DymeConfig(configNameProp.Name, otherProps);
    }

  }
}
