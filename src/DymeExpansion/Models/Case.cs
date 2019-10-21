using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace DymeExpansion.Models
{
  [DebuggerDisplay("{Hash()}")]
  public class Case
  {
    public IEnumerable<CaseProperty> Properties { get;set; } = new List<CaseProperty>();

    public string Hash() {
      return Properties
        .OrderBy(i => i.Name)
        .Select(s => s.Hash())
        .Aggregate((a,b) => $"{a}|{b}");
    }
  }
}
