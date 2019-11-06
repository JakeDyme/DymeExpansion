using System.Diagnostics;
using DymeExpansion.Core.Enums;

namespace DymeExpansion.Core.Models
{
  [DebuggerDisplay("{Name}:{Value}")]
  public class CaseProperty
  {
    public string Name { get;set;}
    public string Value { get; set; }
    public ExpansionTypeEnum ExpansionType { get; set; } = ExpansionTypeEnum.expansive;
    public string CorrelationId { get; set; }
    public CaseProperty() { }
    
    public CaseProperty(string name,  string value, string correlationId = null){
      Name = name;
      Value = value;
      CorrelationId = correlationId;
    }

    public string Hash()
    {
      return $"{Name}:{Value}";
    }
  }
}
