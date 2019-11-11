using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DymeExpansion.Core.Models
{
  [DebuggerDisplay("{Hash()}")]
  public class DymeCase
  {
    public IEnumerable<DymeCaseProperty> Properties { get;set; } = new List<DymeCaseProperty>();

    public string Property(string propertyName)
    {
      ValidatePropertyExists(propertyName);
      return Properties.Single(p => p.Name == propertyName).Value;
    }

    public string this[string propertyName]
    {
      get {
        ValidatePropertyExists(propertyName);
        return Properties.Single(p => p.Name == propertyName).Value; 
      }
      set {
        ValidatePropertyExists(propertyName);
        Properties.Single(p => p.Name == propertyName).Value = value; 
      }
    }

    public string PropertyOrDefault(string propertyName)
    {
      ValidatePropertyExists(propertyName);
      return Properties.SingleOrDefault(p => p.Name == propertyName)?.Value;
    }

    public string Hash() {
      return Properties
        .OrderBy(i => i.Name)
        .Select(s => s.Hash())
        .Aggregate((a,b) => $"{a}|{b}");
    }

    private void ValidatePropertyExists(string name)
    {
      if (!Properties.Any(p => p.Name == name))
        throw new Exception($"There is no property by the name \"{name}\" in the case");
    }

  }
}
