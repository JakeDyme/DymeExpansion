using System.Collections.Generic;
using System.Diagnostics;
using DymeExpansion.Core.Enums;

namespace DymeExpansion.Core.Models
{
  [DebuggerDisplay("{Name}:{Value}")]
  public class DymeCaseProperty
  {
    public string Name { get;set;}
    public string Value { get; set; }
    public string CorrelationKey { get; set; }
    public ExpansionTypeEnum ExpansionType { get; set; } = ExpansionTypeEnum.expansive;

    // Tracking
    internal int PropertyIndex { get; set; }
    internal int ValueIndexFromOriginalList { get; set; }
    public string CorrelationPath { get; set; }
    public string OriginPath { get; set; }
    public string[] OriginConfigPath { get; set; }
    
    public DymeCaseProperty() { }
    
    public DymeCaseProperty(string name,  string value, string correlationKey = null){
      Name = name;
      Value = value;
      CorrelationKey = correlationKey;
    }

    public string Hash()
    {
      return $"{Name}:{Value}";
    }
  }
}
