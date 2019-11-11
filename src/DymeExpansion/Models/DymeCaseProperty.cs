using System.Diagnostics;
using DymeExpansion.Core.Enums;

namespace DymeExpansion.Core.Models
{
  [DebuggerDisplay("{Name}:{Value}")]
  public class DymeCaseProperty
  {
    public string Name { get;set;}
    public string Value { get; set; }
    internal int PropertyIndex { get; set; }
    internal int ValueIndex { get; set; }
    internal ExpansionTypeEnum ExpansionType { get; set; } = ExpansionTypeEnum.expansive;
    public string CorrelationKey { get; set; }
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
