using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using DymeExpansion.Core.Enums;
using DymeExpansion.Core.ExtensionMethods;
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
      var configLibrary = new List<DymeConfig>() {};
      return CasesFromConfigs(config, configLibrary);
    }

    public IEnumerable<DymeCase> CasesFromConfigs(DymeConfig mainConfig, IEnumerable<DymeConfig> configLibrary) {
      var postApplyConfigProperties = new List<DymeConfigProperty>();
      /// Ensure config naming and structure...
      var namedConfigLibrary = configLibrary.Select(c => c.ConfigWithNameFromConfigWithNameProperty(_regexMatchForSpecialPropertyConfigName));
      mainConfig = mainConfig.ConfigWithNameFromConfigWithNameProperty(_regexMatchForSpecialPropertyConfigName);
      /// Get relevant configs from library...
      var relevantConfigLibrary = GetRelevantConfigsFromLibrary(mainConfig, namedConfigLibrary);
      /// Pre-Process library configs (extract post-apply-cases)...
      postApplyConfigProperties.AddRange(PreProcessConfigAndReturnPostApplyConfigs(mainConfig, out DymeConfig processedMainConfig));
      var processedConfigLibrary = new List<DymeConfig>();
      foreach (var libraryConfig in relevantConfigLibrary)
      {
        postApplyConfigProperties.AddRange(PreProcessConfigAndReturnPostApplyConfigs(libraryConfig, out DymeConfig processedLibraryConfig));
        processedConfigLibrary.Add(processedLibraryConfig);
      }
      /// Convert configs to node tree...
      var nodeTree = NodeTreeFromConfigs(mainConfig, processedConfigLibrary);
      /// Convert node tree to cases...
      var cases = CasesFromNodeTree(nodeTree);
      ///Apply post-apply-cases...
      var finalCases = PostCasesProcessing(cases, postApplyConfigProperties);
      return finalCases.ToList();
    }

    private IEnumerable<DymeConfigProperty> PreProcessConfigAndReturnPostApplyConfigs(DymeConfig inConfig, out DymeConfig outConfig)
    {
      outConfig = inConfig.ConfigWithNameFromConfigWithNameProperty();
      var excludedProperties = inConfig.Properties.Where(p => p.ExpansionType == ExpansionTypeEnum.pool).ToList();
      excludedProperties.ForEach(p => p.CorrelationKey = p.CorrelationKey ?? Guid.NewGuid().ToString());
      var placeholderProperties = excludedProperties.Select(p => 
        new DymeConfigProperty(){
          Name = p.Name,
          Values = new[]{ p.Values.First() },
          ExpansionType = ExpansionTypeEnum.pool,
          CorrelationKey = p.CorrelationKey
        });
      var includedProperties = inConfig.Properties.Except(excludedProperties).Concat(placeholderProperties);
      outConfig.Properties = includedProperties.ToList();
      return excludedProperties;
    }

    private IEnumerable<DymeCase> PostCasesProcessing(IEnumerable<DymeCase> dymeCases, IEnumerable<DymeConfigProperty> postApplyProperties)
    {
      Dictionary<string, int> poolPropertyPickedCount = postApplyProperties.ToDictionary(i => i.CorrelationKey, i => 0);
      foreach (var postApplyProp in postApplyProperties)
      {
        foreach (var dymeCase in dymeCases)
        {
          /// Find the place-holder property in the case (if it exists)...
          var matchingProperty = dymeCase.Properties.SingleOrDefault(caseProp => caseProp.Name == postApplyProp.Name && caseProp.CorrelationKey == postApplyProp.CorrelationKey);
          var newProperty = new DymeCaseProperty();
          if (matchingProperty != null) {
            /// Remove the existing placeholder property...
            dymeCase.Properties = dymeCase.Properties.Except(new[] { matchingProperty }).ToList();
            /// Create a replacement for the placeholder property...
            newProperty = matchingProperty.Clone();
          }
          else {
            /// If an explicit placeholder doesn't exist then check for any correlated properties...
            matchingProperty = dymeCase.Properties.FirstOrDefault(caseProp => caseProp.CorrelationKey == postApplyProp.CorrelationKey);
            /// Create a new property to inject into the case...
            newProperty = new DymeCaseProperty(postApplyProp.Name, null, postApplyProp.CorrelationKey);
          }
          if (matchingProperty == null) continue;
          /// Pick the next value for this specific pool property...
          var caseIndex = poolPropertyPickedCount[postApplyProp.CorrelationKey];
          /// Update the picked tracking count...
          poolPropertyPickedCount[postApplyProp.CorrelationKey] += 1;
          /// Determine the actual pool index...
          var poolIndex = caseIndex % postApplyProp.Values.Count();
          /// Assign the appropriate pool value to the property...
          newProperty.Value = postApplyProp.Values.ElementAt(poolIndex);
          newProperty.ValueIndex = poolIndex;
          /// Add new the case property back to the case...
          dymeCase.Properties = dymeCase.Properties.Append(newProperty);
        }
      }
      return dymeCases;
    }

    /// <summary>
    /// Extract only the configs that are actually used in config tree...
    /// </summary>
    /// <param name="config"></param>
    /// <param name="receedingConfigLibrary" description="a list that gets its values removed as they are processed in order to make subsequent queries on the list more efficient"></param>
    /// <returns></returns>
    private IEnumerable<DymeConfig> GetRelevantConfigsFromLibrary(DymeConfig config, IEnumerable<DymeConfig> configLibrary)
    {
      var importConfigNames = GetImportTypeConfigProperties(config).SelectMany(prop => prop.Values);
      var importedConfigs = new List<DymeConfig>();
      foreach (var configName in importConfigNames)
      {
        var importedConfig = GetConfigFromLibrary(configName, configLibrary);
        if (importedConfigs.Any(c => c.Name == importedConfig.Name)) continue;
        importedConfigs.Add(importedConfig);
        var importedChildConfigs = GetRelevantConfigsFromLibrary(importedConfig, configLibrary)
          .Where(cc => !importedConfigs.Any(childConfig => childConfig.Name == cc.Name));
        importedConfigs.AddRange(importedChildConfigs);
      }
      return importedConfigs;
    }

    private DymeConfig GetConfigFromLibrary(string configName, IEnumerable<DymeConfig> configLibrary)
    {
      var foundConfigs = configLibrary.Where(c => c.Name == configName).ToList();
      if (foundConfigs.Count() > 1) throw new Exception($"There are multiple conflicting configs in your library by the name of: \"{configName}\". Please rename one of the configs.");
      if (foundConfigs.Count() < 1) throw new Exception($"The referenced config by the name of: \"{configName}\", does not exist in your config library.");
      return foundConfigs.Single();
    }

    private IEnumerable<DymeConfigProperty> GetImportTypeConfigProperties(DymeConfig config)
    {
      return config.Properties.Where(ConfigPropertyIsImportType);
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

    internal IEnumerable<DymeCase> CasesFromNodeTree(Node node, Node parentNode = null, string treePath = "", string[] configPath = null)
    {
      var mergedCases = new List<DymeCase>();
      if (configPath == null) configPath = new string[] { };
      if (IsPropertyNamePartNode(node))
      {
        treePath += "/" + node.Value;
        ValidatePropertyNode(node);
        /// Because its a property, the child cases will expand eachother (be added, or additively extracted using the "SelectMany")...
        mergedCases = node.Children
          .SelectMany(childNode => CasesFromNodeTree(childNode, node, treePath, configPath))
          .ToList();
      }
      
      else if (IsPropertyValueOrReferencePartNode(node))
      {
        treePath += ":" + node.Value;
        
        if (IsValueNodeWithNoChildren(node)) return CaseFromValueNode(node, parentNode, treePath, configPath);
        /// ...Is a reference node...
        configPath = configPath.Append(node.Value).ToArray();
        mergedCases = node.Children
          .Select(childNode => CasesFromNodeTree(childNode, node, treePath, configPath))
          .Aggregate((caseSet1, caseSet2) => MergeCaseSets(caseSet1, caseSet2))
          .ToList();
      }
      
      return Distinct(mergedCases);
    }

    /// <summary>
    /// This method gets called at the leaf-most parts of the node-tree.
    /// </summary>
    /// <param name="valueNode"></param>
    /// <param name="parent"></param>
    /// <param name="treePath"></param>
    /// <param name="configPath"></param>
    /// <returns></returns>
    private static IEnumerable<DymeCase> CaseFromValueNode(Node valueNode, Node parent, string treePath, string[] configPath)
    {
      var propertyNamePart = parent;
      var propertyValuePart = valueNode;
      var newProperty = new DymeCaseProperty(propertyNamePart.Value, propertyValuePart.Value, propertyNamePart.CorrelationKey);
      newProperty.PropertyIndex = propertyNamePart.ValueIndex;
      newProperty.ValueIndex = propertyValuePart.ValueIndex;
      newProperty.OriginPath = treePath;
      newProperty.OriginConfigPath = configPath;
      var newCase = new DymeCase();
      newCase.Properties = new[] { newProperty };
      return new List<DymeCase>() { newCase };
    }

    private bool IsValueNodeWithNoChildren(Node forNode)
    {
      return forNode.Children.Count == 0 && forNode.NodeType == NodeTypeEnum.ValueNode;
    }

    private IEnumerable<ValueNode> LeafNodesFromConfigProperty(IEnumerable<DymeConfig> configLibrary, DymeConfigProperty configProperty)
    {
      
      var leafNodes = new List<ValueNode>();
      for ( var i = 0; i < configProperty.Values.Count(); i++)
      {
        var value = configProperty.Values.ElementAt(i);
        var leafNode = new ValueNode(value, NodeTypeEnum.ValueNode);
        leafNode.LeafType = ValueTypeEnum.Text;
        leafNode.ValueIndex = i;
        if (ConfigPropertyIsImportType(configProperty))
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

    private bool ConfigPropertyIsImportType(DymeConfigProperty property)
    {
      Regex match = new Regex(_regexMatchForSpecialPropertyConfigReference);
      return match.IsMatch(property.Name);
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

