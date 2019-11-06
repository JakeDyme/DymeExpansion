using System.Collections.Generic;
using System.Linq;

namespace DymeExpansion.Core.Models
{

  public class Config
  {
    public string Name { get;set; }
    public IList<ConfigProperty> Properties { get;set; } = new List<ConfigProperty>(); 
    public Config(string name, IEnumerable<ConfigProperty> properties)
    {
      Name = name;
      Properties = properties.ToList();
    }

    public Config() {}

    public static Config New(string name) { 
      return new Config(name, new List<ConfigProperty>());
    }

    public Config AddProperty(string name, string[] values, string correlationId = null)
    {
      Properties.Add(new ConfigProperty(name, values, correlationId));
      return this;
    }

    public Config AddProperty(string name, string value, string correlationId = null)
    {
      Properties.Add(new ConfigProperty(name, value, correlationId));
      return this;
    }

  }
}
