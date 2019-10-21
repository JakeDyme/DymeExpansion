using System.Collections.Generic;
using DymeExpansion.Enums;

namespace DymeExpansion.Models
{

  public class ConfigProperty
  {
    public string Name { get;set;}
    public IEnumerable<string> Values { get; set; }
    public ExpansionTypeEnum ExpansionType { get; set; } = ExpansionTypeEnum.expansive;
    public string CorrelationId { get; set; }
    public ConfigProperty() { }
    
    public ConfigProperty(string name,  string value, string correlationId = null){
      Name = name;
      Values = new[] { value };
      CorrelationId = correlationId;
    }

    public ConfigProperty(string name,  string[] values, string correlationId = null){
      Name = name;
      Values = values;
      CorrelationId = correlationId;
    }

  }
}
