using DymeExpansion.Core.Models;
using System;
using System.Collections.Generic;
using System.Text;

namespace DymeExpansion.Core.ExtensionMethods
{
  public static class DymeCasePropertyExtensionMethods
  {
    public static DymeCaseProperty Clone(this DymeCaseProperty property)
    {
      return new DymeCaseProperty() { 
        Name = property.Name, 
        CorrelationKey = property.CorrelationKey, 
        ExpansionType = property.ExpansionType, 
        OriginConfigPath = property.OriginConfigPath, 
        OriginPath = property.OriginPath, 
        PropertyIndex = property.PropertyIndex, 
        Value = property.Value, 
        ValueIndexFromOriginalList = property.ValueIndexFromOriginalList
      };
    }
  }
}
