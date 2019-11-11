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
      for (var i = 0; i < parentConfig.Properties.Count(); i++)
      {
        var prop = parentConfig.Properties.ElementAt(i);
        var newPropertyNode = new PropertyNode(prop.Name, NodeTypeEnum.PropertyNode, prop.CorrelationKey);
        newPropertyNode.Children = LeafNodesFromConfigProperty(configLibrary, prop).Select(n => n as Node).ToList();
        newPropertyNode.ValueIndex = i;
        propertyNodes.Add(newPropertyNode);
      }
      return propertyNodes;
    }

    private void ValidatePropertyNode(Node forNode)
    {
      if (forNode.Children.Count == 0) 
        throw new Exception("A property node must have child nodes, because the child nodes represent property values");
    }

    private static IEnumerable<DymeCase> CaseFromNode(Node forNode, Node parent)
    {
      var propertyNamePart = parent;
      var propertyValuePart = forNode;
      var newProperty = new DymeCaseProperty(propertyNamePart.Value, propertyValuePart.Value, propertyNamePart.CorrelationKey);
      newProperty.PropertyIndex = propertyNamePart.ValueIndex;
      newProperty.ValueIndex = propertyValuePart.ValueIndex;
      var newCase = new DymeCase();
      newCase.Properties = new[] { newProperty };
      return new List<DymeCase>() { newCase };
    }

    private static IEnumerable<DymeCase> Distinct(IEnumerable<DymeCase> cases)
    {
      return cases
                .GroupBy(c => c.Hash())
                .Select(g => g.First())
                .ToList();
    }

    private static bool IsPropertyNamePartNode(Node forNode)
    {
      return forNode.NodeType == NodeTypeEnum.PropertyNode;
    }

    private static bool IsPropertyValueOrReferencePartNode(Node forNode)
    {
      return forNode.NodeType == NodeTypeEnum.ValueNode;
    }

    internal IEnumerable<DymeCase> CasesFromNodeTree(Node node, Node parentNode = null)
    {
      var mergedCases = new List<DymeCase>();
      
      if (IsPropertyNamePartNode(node))
      {
        /// Because its a property, the child cases will expand eachother. (be added, or additively extracted)
        ValidatePropertyNode(node);
        mergedCases = node.Children
          .SelectMany(c => CasesFromNodeTree(c, node))
          .ToList();
      }
      
      else if (IsPropertyValueOrReferencePartNode(node))
      {
        if (IsValueNodeWithNoChildren(node)) return CaseFromNode(node, parentNode);
        /// Because its a leaf, the child cases will overlay eachother. (be multiplied, or aggregated)
        mergedCases = node.Children
          .Select(childNode => CasesFromNodeTree(childNode, node))
          .Aggregate((caseSet1, caseSet2) => MergeCaseSets(caseSet1, caseSet2))
          .ToList();
      }
      return Distinct(mergedCases);
      throw new Exception("Unknown NodeType");
    }

    private bool IsValueNodeWithNoChildren(Node forNode)
    {
      return forNode.Children.Count == 0 && forNode.NodeType == NodeTypeEnum.ValueNode;
    }

    private IEnumerable<ValueNode> LeafNodesFromConfigProperty(IEnumerable<DymeConfig> configLibrary, DymeConfigProperty configProperty)
    {
      Regex match = new Regex(_regexMatchForSpecialPropertyConfigReference);
      var leafNodes = new List<ValueNode>();
      for ( var i = 0; i < configProperty.Values.Count(); i++) //(var value in configProperty.Values)
      {
        var value = configProperty.Values.ElementAt(i);
        var leafNode = new ValueNode(value, NodeTypeEnum.ValueNode);
        leafNode.LeafType = ValueTypeEnum.Text;
        leafNode.ValueIndex = i;
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

    private IEnumerable<DymeCase> MergeCaseSets(IEnumerable<DymeCase> caseSet1, IEnumerable<DymeCase> caseSet2)
    {
      var finalCaseSet = new List<DymeCase>();
      foreach (var case1 in caseSet1)
      {
        foreach (var case2 in caseSet2)
        {
          /// check if merged case is allowed based on any correlations...
          if (PropertiesAreCorrelatedButValueIndexesDontMatch(case1, case2)) continue;
          /// This merges different properties into one case (or overrides properties if they share the same name)...
          var newMergedCase = OverlayCases(case1, case2);
          finalCaseSet.Add(newMergedCase);
        }
      }
      return finalCaseSet;
    }

    private bool PropertiesAreCorrelatedButValueIndexesDontMatch(DymeCase case1, DymeCase case2)
    {
      /// An implicit rule of correlation in this system is that for any given permutation of 
      /// property-values the value-indexes of correlated properties must match...
      return (case1.Properties.Any(p_c1 => case2.Properties.Any(p_c2 =>
            p_c1.CorrelationKey != null &&
            p_c1.CorrelationKey == p_c2.CorrelationKey &&
            p_c1.ValueIndex != p_c2.ValueIndex)));
    }

    private DymeCase OverlayCases(DymeCase baseCase, DymeCase overlayCase)
    {
      var newPropertySet = new List<DymeCaseProperty>();
      newPropertySet.AddRange(overlayCase.Properties.ToList());
      newPropertySet.AddRange(baseCase.Properties.Where(bc => !overlayCase.Properties.Any(op => op.Name == bc.Name)));
      return new DymeCase(){ Properties = newPropertySet };
    }

  }
}

