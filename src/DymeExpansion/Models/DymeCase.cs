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
      return Properties.Single(p => p.Name == propertyName).Value;
    }

    public string this[string propertyName]
    {
      get { return Properties.Single(p => p.Name == propertyName).Value; }
      set { Properties.Single(p => p.Name == propertyName).Value = value; }
    }

    public string PropertyOrDefault(string propertyName)
    {
      return Properties.SingleOrDefault(p => p.Name == propertyName)?.Value;
    }

    public string Hash() {
      return Properties
        .OrderBy(i => i.Name)
        .Select(s => s.Hash())
        .Aggregate((a,b) => $"{a}|{b}");
    }


  }
}
