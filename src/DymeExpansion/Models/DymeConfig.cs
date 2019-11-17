using DymeExpansion.Core.Enums;
using System.Collections.Generic;
using System.Linq;

namespace DymeExpansion.Core.Models
{

  public class DymeConfig
  {
    public string Name { get;set; }
    public IList<DymeConfigProperty> Properties { get;set; } = new List<DymeConfigProperty>(); 
    public DymeConfig(string name, IEnumerable<DymeConfigProperty> properties)
    {
      Name = name;
      Properties = properties.ToList();
    }

    public DymeConfig() {}

    public static DymeConfig New(string name) { 
      return new DymeConfig(name, new List<DymeConfigProperty>());
    }

    public DymeConfig AddProperty(string name, string[] values, string correlationKey = null)
    {
      Properties.Add(new DymeConfigProperty(name, values, correlationKey));
      return this;
    }

    public DymeConfig AddProperty(string name, string[] values, ExpansionTypeEnum expansionType)
    {
      Properties.Add(new DymeConfigProperty(name, values, expansionType));
      return this;
    }

    public DymeConfig AddProperty(string name, string value)
    {
      Properties.Add(new DymeConfigProperty(name, value));
      return this;
    }

    public DymeConfigProperty AddProperty(string name)
    {
      var newProperty = new DymeConfigProperty();
      newProperty.Name = name;
      Properties.Add(newProperty);
      return newProperty;
    }

    public override string ToString()
    {
      return $"{{\"Name\": \"{Name ?? ""}\" \"Properties\": {{{Properties.Select(p => p.ToString()).Aggregate((a,b) => a + ", " + b )}}}}}";
    }
  }
}
