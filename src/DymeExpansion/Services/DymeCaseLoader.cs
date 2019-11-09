using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DymeExpansion.Core.Enums;
using DymeExpansion.Core.Models;

namespace DymeExpansion.Core.Services
{
  public class DymeCaseLoader
  {
    private string _regexMatchForSpecialPropertyConfigName;
    private string _regexMatchForSpecialPropertyConfigReference = "IMPORT\\..*|^IMPORT";

    public static IEnumerable<DymeCase> CasesFromConfig(DymeConfig config, IEnumerable<DymeConfig> configLibrary)
    {
      var testCaseLoader = new DymeCaseLoader();
      return testCaseLoader.CasesFromConfigs(config, configLibrary);
    }

    public static IEnumerable<DymeCase> CasesFromConfig(DymeConfig config)
    {
      var testCaseLoader = new DymeCaseLoader();
      return testCaseLoader.CasesFromConfigs(config);
    }

    public DymeCaseLoader() {}

    public DymeCaseLoader(string configReferencePropertyName = null, string configNamePrefix = null) {
      _regexMatchForSpecialPropertyConfigReference = configReferencePropertyName;
      _regexMatchForSpecialPropertyConfigName = configNamePrefix;
    }

    public DymeCaseLoader(DymeCaseLoaderOptions options)
    {
      _regexMatchForSpecialPropertyConfigReference = options.PrefixForConfigName;
      _regexMatchForSpecialPropertyConfigName = options.PrefixForConfigReference;
    }

    public IEnumerable<DymeCase> CasesFromConfigs(DymeConfig config)
    {
      var configLibrary = new List<DymeConfig>() { config };
      var parsedConfigLibrary = configLibrary.Select(ParseConfig);
      var nodeTree = NodeTreeFromConfigs(ParseConfig(config), parsedConfigLibrary);
      var cases = CasesFromNodeTree(nodeTree);
      return cases;
    }

    public IEnumerable<DymeCase> CasesFromConfigs(DymeConfig config, IEnumerable<DymeConfig> configLibrary) { 
      var nodeTree = NodeTreeFromConfigs(config, configLibrary);
      var cases = CasesFromNodeTree(nodeTree);
      return cases;
    }

    public string CasesToString(IEnumerable<DymeCase> cases, string caseSeparator = "\n", string propertySeparator = " ")
    {
      return cases.Select(c => CaseToString(c, propertySeparator)).Aggregate((a, b) => $"{a}{caseSeparator}{b}");
    }

    public string CaseToString(DymeCase caseX, string propertySeparator = " ")
    {
      return caseX.Properties.OrderBy(i => i.Name).Select(s => $"p:{s.Name}({s.Value})").Aggregate((a, b) => $"{a}{propertySeparator}{b}");
    }

    public string CaseToGrid(DymeCase caseX, string propertySeparator = "\t")
    {
      return caseX.Properties.OrderBy(i => i.Name).Select(s => s.Value).Aggregate((a, b) => $"{a}{propertySeparator}{b}");
    }

    private DymeConfig ParseConfig(DymeConfig config)
    {
      if (_regexMatchForSpecialPropertyConfigName == null) return config;      
      Regex match = new Regex(_regexMatchForSpecialPropertyConfigName);
      var leafNodes = new List<ValueNode>();
      var configNameProp = config.Properties.SingleOrDefault(p => match.IsMatch(p.Name));
      if (configNameProp == null) throw new Exception($"Your config is missing a mandatory property \"{_regexMatchForSpecialPropertyConfigName}\"");
      var otherProps = config.Properties.Where(p => !match.IsMatch(p.Name));
      return new DymeConfig(configNameProp.Name, otherProps);
    }

    internal Node NodeTreeFromConfigs(DymeConfig config, IEnumerable<DymeConfig> configLibrary) { 
      var configNodeTree = new Node(config.Name, NodeTypeEnum.ValueNode);
      configNodeTree.Children = PropertyNodesFromConfig(config, configLibrary).Select(n => n as Node).ToList();
      return configNodeTree;
    }

    internal IEnumerable<PropertyNode> PropertyNodesFromConfig(DymeConfig parentConfig, IEnumerable<DymeConfig> configLibrary) { 
      
      var propertyNodes = new List<PropertyNode>();
      foreach (var prop in parentConfig.Properties)
      {
        var newPropertyNode = new PropertyNode(prop.Name, NodeTypeEnum.PropertyNode, prop.CorrelationKey);
        newPropertyNode.Children = LeafNodesFromConfigProperty(configLibrary, prop).Select(n => n as Node).ToList();
        propertyNodes.Add(newPropertyNode);
      }
      return propertyNodes;
    }

    internal IEnumerable<DymeCase> CasesFromNodeTree(Node forNode, Node parent = null)
    {
      if (forNode.Children.Count == 0)
      {
        var newCase = new DymeCase();
        newCase.Properties = newCase.Properties.Append(new DymeCaseProperty(parent.Value, forNode.Value));
        return new List<DymeCase>() { newCase };
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
          .GroupBy(n => n.CorrelationKey)
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

      throw new Exception("Unknown NodeType");
    }

    private IEnumerable<ValueNode> LeafNodesFromConfigProperty(IEnumerable<DymeConfig> configLibrary, DymeConfigProperty configProperty)
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
          var matchingLibraryConfig = configLibrary.SingleOrDefault(c => c.Name == value);
          if (matchingLibraryConfig == null) throw new Exception($"You are attempting to reference a config \"{value}\", which does not exist in your config library");
          leafNode.Children = PropertyNodesFromConfig(matchingLibraryConfig, configLibrary).Select(n => n as Node).ToList();
        }
        leafNodes.Add(leafNode);
      }
      return leafNodes;
    }

    private IEnumerable<DymeCase> MergeCorrelatedCases(IEnumerable<DymeCase> caseSet1, IEnumerable<DymeCase> caseSet2)
    {
      var finalLeafNodeSet = new List<DymeCase>();
      var caseSet1L = caseSet1.ToList();
      var caseSet2L = caseSet2.ToList();
      for (var caseIndex = 0; caseIndex < caseSet1L.Count(); caseIndex++)
      {
        var newMergedCase = OverlayCases(caseSet1.ElementAt(caseIndex),caseSet2.ElementAt(caseIndex) );
        finalLeafNodeSet.Add(newMergedCase);
      }
      return finalLeafNodeSet;
    }

    private IEnumerable<DymeCase> MergeCaseSets(IEnumerable<DymeCase> caseSet1, IEnumerable<DymeCase> caseSet2)
    {
      var finalLeafNodeSet = new List<DymeCase>();
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

    private DymeCase OverlayCases(DymeCase baseCase, DymeCase overlayCase)
    {
      var newPropertySet = new List<DymeCaseProperty>();
      newPropertySet.AddRange(overlayCase.Properties.ToList());
      newPropertySet.AddRange(baseCase.Properties.Where(bc => !overlayCase.Properties.Any(op => op.Name == bc.Name)));
      return new DymeCase(){Properties = newPropertySet };
    }

  }
}

