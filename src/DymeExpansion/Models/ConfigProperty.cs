using System.Collections.Generic;
using System.Linq;
using DymeExpansion.Core.Enums;

namespace DymeExpansion.Core.Models
{

  public class ConfigProperty
  {
    public string Name { get;set;}
    public IEnumerable<string> Values { get; set; }
    internal ExpansionTypeEnum ExpansionType { get; set; } = ExpansionTypeEnum.expansive;
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

    public ConfigProperty WithValue(string value)
    {
      if (Values == null) Values = new List<string>();
      Values.Append(value);
      return this;
    }

    public ConfigProperty And(string value)
    {
      return WithValue(value);
    }

  }
}
