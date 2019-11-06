using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using DymeExpansion.Core.Enums;
using DymeExpansion.Core.Models;

namespace DymeExpansion.Core.Services
{
  public class TestCaseLoader
  {
    private string _regexMatchForSpecialPropertyConfigReference = "IMPORT\\..*|^IMPORT";
    private string _regexMatchForSpecialPropertyConfigName = "IMPORT\\..*|^IMPORT";

    public TestCaseLoader() { }

    public TestCaseLoader(string configReferencePropertyName = null, string configNamePrefix = null) {
      _regexMatchForSpecialPropertyConfigReference = configReferencePropertyName;
      _regexMatchForSpecialPropertyConfigName = configNamePrefix;
    }

    public TestCaseLoader(CaseLoaderOptions options)
    {
      _regexMatchForSpecialPropertyConfigReference = options.PrefixForConfigName;
      _regexMatchForSpecialPropertyConfigName = options.PrefixForConfigReference;
    }

    public IEnumerable<Case> CasesFromConfigs(Config config, List<Config> configLibrary) { 
      var nodeTree = NodeTreeFromConfigs(config, configLibrary);
      var cases = CasesFromNodeTree(nodeTree);
      return cases;
    }

    public Node NodeTreeFromConfigs(Config config, List<Config> configLibrary) { 
      var configNodeTree = new Node(config.Name, NodeTypeEnum.ValueNode);
      configNodeTree.Children = PropertyNodesFromConfig(config, configLibrary).Select(n => n as Node).ToList();
      return configNodeTree;
    }

    public IEnumerable<PropertyNode> PropertyNodesFromConfig(Config parentConfig, IEnumerable<Config> configLibrary) { 
      
      var propertyNodes = new List<PropertyNode>();
      foreach (var prop in parentConfig.Properties)
      {
        var newPropertyNode = new PropertyNode(prop.Name, NodeTypeEnum.PropertyNode, prop.CorrelationId);
        newPropertyNode.Children = LeafNodesFromConfigProperty(configLibrary, prop).Select(n => n as Node).ToList();
        propertyNodes.Add(newPropertyNode);
      }
      return propertyNodes;
    }

    public string CasesToString(IEnumerable<Case> cases, string caseSeparator = "\n", string propertySeparator = " ")
    {
      return cases.Select(c => CaseToString(c, propertySeparator)).Aggregate((a, b) => $"{a}{caseSeparator}{b}");
    }

    public string CaseToString(Case caseX, string propertySeparator = " ")
    {
      return caseX.Properties.OrderBy(i => i.Name).Select(s => $"p:{s.Name}({s.Value})").Aggregate((a, b) => $"{a}{propertySeparator}{b}");
    }

    public string CaseToGrid(Case caseX, string propertySeparator = "\t")
    {
      return caseX.Properties.OrderBy(i => i.Name).Select(s => s.Value).Aggregate((a, b) => $"{a}{propertySeparator}{b}");
    }

    public IEnumerable<Case> CasesFromNodeTree(Node forNode, Node parent = null)
    {
      if (forNode.Children.Count == 0)
      {
        var newCase = new Case();
        newCase.Properties = newCase.Properties.Append(new CaseProperty(parent.Value, forNode.Value));
        return new List<Case>() { newCase };
      }

      if (forNode.NodeType == NodeTypeEnum.PropertyNode)
      {
        /// Because its a property, the child cases will expand eachother. (be added, or additively extracted)
        var mergedCases = forNode.Children
          .SelectMany(c => CasesFromNodeTree(c, forNode));

        var distinctCases = mergedCases
          .GroupBy(c => c.Hash())
          .Select(g => g.First())
          .ToList();

        return distinctCases;
      }

      if (forNode.NodeType == NodeTypeEnum.ValueNode)
      {
        /// Because its a leaf, the child cases will overlay eachother. (be multiplied, or aggregated)
        var mergedCasesByCorrelationGroup = forNode.Children
          .GroupBy(n => n.CorrelationId)
          .Select(correlationGroup => correlationGroup
            .Select(node => CasesFromNodeTree(node, forNode))
            .Aggregate((caseSet1, caseSet2) => MergeCorrelatedCases(caseSet1, caseSet2)))
          .ToList();

        var mergedCases = mergedCasesByCorrelationGroup
          .Aggregate((caseSet1, caseSet2) => MergeCaseSets(caseSet1, caseSet2))
          .ToList();

        var distinctCases = mergedCases
          .GroupBy(c => c.Hash())
          .Select(g => g.First())
          .ToList();

        return distinctCases;
      }

      throw new System.Exception("Should never reach here");
    }

    private IEnumerable<ValueNode> LeafNodesFromConfigProperty(IEnumerable<Config> configLibrary, ConfigProperty configProperty)
    {
      Regex match = new Regex(_regexMatchForSpecialPropertyConfigReference);
      var leafNodes = new List<ValueNode>();
      foreach (var value in configProperty.Values)
      {
        var leafNode = new ValueNode(value, NodeTypeEnum.ValueNode);
        leafNode.LeafType = ValueTypeEnum.Text;
        if (match.IsMatch(configProperty.Name))
        {
          leafNode.LeafType = ValueTypeEnum.ImportedSetup;
          var matchingLibraryConfig = configLibrary.Single(c => c.Name == value);
          leafNode.Children = PropertyNodesFromConfig(matchingLibraryConfig, configLibrary).Select(n => n as Node).ToList();
        }
        leafNodes.Add(leafNode);
      }
      return leafNodes;
    }

    private IEnumerable<Case> MergeCorrelatedCases(IEnumerable<Case> caseSet1, IEnumerable<Case> caseSet2)
    {
      var finalLeafNodeSet = new List<Case>();
      var caseSet1L = caseSet1.ToList();
      var caseSet2L = caseSet2.ToList();
      for (var caseIndex = 0; caseIndex < caseSet1L.Count(); caseIndex++)
      {
        var newMergedCase = OverlayCases(caseSet1.ElementAt(caseIndex),caseSet2.ElementAt(caseIndex) );
        finalLeafNodeSet.Add(newMergedCase);
      }
      return finalLeafNodeSet;
    }

    private IEnumerable<Case> MergeCaseSets(IEnumerable<Case> caseSet1, IEnumerable<Case> caseSet2)
    {
      var finalLeafNodeSet = new List<Case>();
      /// Expansion happens here, this nested foreach creates a cartesian product of properties over test cases...
      foreach (var leafNode1 in caseSet1)
      {
        foreach (var leafNode2 in caseSet2)
        {
          /// This merges different properties into one case (or overrides properties if they share the same name)...
          var newMergedCase = OverlayCases(leafNode1, leafNode2);
          finalLeafNodeSet.Add(newMergedCase);
        }
      }
      return finalLeafNodeSet;
    }

    private Case OverlayCases(Case baseCase, Case overlayCase)
    {
      var newPropertySet = new List<CaseProperty>();
      newPropertySet.AddRange(overlayCase.Properties.ToList());
      newPropertySet.AddRange(baseCase.Properties.Where(bc => !overlayCase.Properties.Any(op => op.Name == bc.Name)));
      return new Case(){Properties = newPropertySet };
    }

  }
}

