using System.Collections.Generic;
using System.Linq;
using DymeExpansion.Core.Enums;

namespace DymeExpansion.Core.Models
{

  public class DymeConfigProperty
  {
    public string Name { get;set;}
    public IEnumerable<string> Values { get; set; }
    internal ExpansionTypeEnum ExpansionType { get; set; } = ExpansionTypeEnum.expansive;
    public string CorrelationKey { get; set; }
    public DymeConfigProperty() { }

    public DymeConfigProperty(string name,  string value){
      Name = name;
      Values = new[] { value };
      //CorrelationKey = correlationKey;
    }

    public DymeConfigProperty(string name,  string[] values, string correlationKey = null){
      Name = name;
      Values = values;
      CorrelationKey = correlationKey;
    }

    public DymeConfigProperty(string name, string[] values, ExpansionTypeEnum propertyExpansionType)
    {
      Name = name;
      Values = values;
      ExpansionType = propertyExpansionType;
    }

    public DymeConfigProperty WithValue(string value)
    {
      if (Values == null) Values = new List<string>();
      Values.Append(value);
      return this;
    }

    public DymeConfigProperty And(string value)
    {
      return WithValue(value);
    }

    public override string ToString()
    {
      return $"\"{Name}\": [{Values.Select(v => "\""+v+ "\"").Aggregate((a,b) => a + "," + b)}]";
    }
  }
}
