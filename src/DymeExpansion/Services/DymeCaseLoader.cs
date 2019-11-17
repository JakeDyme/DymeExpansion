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
      var postApplyConfigs = new List<DymeConfig>();
      //var preProcessedConfigLibrary = new List<DymeConfig>();
      
      // Ensure config naming and structure...
      configLibrary = configLibrary.Select(c => c.ConfigWithNameFromConfigWithNameProperty(_regexMatchForSpecialPropertyConfigName));
      mainConfig = mainConfig.ConfigWithNameFromConfigWithNameProperty(_regexMatchForSpecialPropertyConfigName);
      
      // Get relevant configs from library...
      var receedingConfigLibrary = configLibrary.ToList();
      configLibrary = GetRelevantConfigsFromLibrary(mainConfig, ref receedingConfigLibrary);

      // Pre-Process library configs (extract post-apply-cases)...
      postApplyConfigs.AddRange(PreProcessConfigAndReturnPostApplyConfigs(mainConfig, out DymeConfig processedMainConfig));
      var processedConfigLibrary = new List<DymeConfig>();
      foreach (var libraryConfig in configLibrary)
      {
        postApplyConfigs.AddRange(PreProcessConfigAndReturnPostApplyConfigs(libraryConfig, out DymeConfig processedLibraryConfig));
        processedConfigLibrary.Add(processedLibraryConfig);
      }
      configLibrary = processedConfigLibrary;

      // Convert configs to node tree...
      var nodeTree = NodeTreeFromConfigs(mainConfig, configLibrary);
      
      // Convert node tree to cases...
      var cases = CasesFromNodeTree(nodeTree);
      
      //Apply post-apply-cases
      var finalCases = PostCasesProcessing(cases, postApplyConfigs);
      return finalCases;
    }

    private IEnumerable<DymeConfig> PreProcessConfigAndReturnPostApplyConfigs(DymeConfig inConfig, out DymeConfig outConfig)
    {
      outConfig = inConfig.ConfigWithNameFromConfigWithNameProperty();
      var excludedProperties = inConfig.Properties.Where(p => p.ExpansionType == ExpansionTypeEnum.pool);
      var configName = outConfig.Name;
      var placeholderProperties = excludedProperties.Select(p => new DymeConfigProperty(){ Name = p.Name, Values = new[]{ configName }, ExpansionType = ExpansionTypeEnum.pool });
      var includedProperties = inConfig.Properties.Except(excludedProperties).Concat(placeholderProperties);
      outConfig.Properties = includedProperties.ToList();
      if (excludedProperties.Any()) return new List<DymeConfig>(){ new DymeConfig(outConfig.Name, excludedProperties) };
      return new List<DymeConfig>();
    }

    /// <summary>
    /// Extract only the configs that are actually used in config tree...
    /// </summary>
    /// <param name="config"></param>
    /// <param name="receedingConfigLibrary" description="a list that gets its values removed as they are processed in order to make subsequent queries on the list more efficient"></param>
    /// <returns></returns>
    private IEnumerable<DymeConfig> GetRelevantConfigsFromLibrary(DymeConfig config, ref List<DymeConfig> receedingConfigLibrary)
    {
      var importConfigNames = GetImportTypeConfigProperties(config).SelectMany(prop => prop.Values);
      var importedConfigs = new List<DymeConfig>();
      foreach (var configName in importConfigNames)
      {
        var importedConfig = GetConfigFromReceedingLibrary(configName, ref receedingConfigLibrary);
        importedConfigs.Add(importedConfig);
        importedConfigs.AddRange(GetRelevantConfigsFromLibrary(importedConfig, ref receedingConfigLibrary));
      }
      return importedConfigs;
    }

    private DymeConfig GetConfigFromReceedingLibrary(string configName, ref List<DymeConfig> receedingConfigLibrary)
    {
      var count = 0;
      DymeConfig found = null;
      var remaining = new List<DymeConfig>();
      foreach (var config in receedingConfigLibrary)
      {
        if (config.Name == configName)
        {
          count++;
          if (count > 1) throw new Exception($"There are multiple conflicting configs in your library by the name of: \"{configName}\". Please rename one of the configs.");
          found = config;
        }
        else
        {
          remaining.Add(config);
        }
      }
      if (count == 0) throw new Exception($"The referenced config by the name of: \"{configName}\", does not exist in your config library.");
      receedingConfigLibrary = remaining;
      return found;
    }

    private IEnumerable<DymeConfigProperty> GetImportTypeConfigProperties(DymeConfig config)
    {
      return config.Properties.Where(ConfigPropertyIsImportType);
    }

    //private IEnumerable<DymeCase> GetPostApplyCases(DymeConfig processedConfig)
    //{
    //  List<DymeCase> returnCases = processedConfig.Properties
    //    .Where(p => p.ExpansionType == ExpansionTypeEnum.pool)
    //    .SelectMany(CasesFromConfigProperty)
    //    .ToList();
    //  return returnCases;
    //}

    private DymeCaseProperty CasePropertyFromConfigProperty(DymeConfigProperty prop, int propIndex, int valueIndex, string originConfigName)
    {
      //var newCaseProperties = new List<DymeCaseProperty>();
      //for (var valueIndex =0; valueIndex < prop.Values.Count(); valueIndex++)
      //{
      var newProperty = new DymeCaseProperty(prop.Name, prop.Values.ElementAt(valueIndex));
      newProperty.OriginConfigPath = new [] { originConfigName };
      newProperty.ExpansionType = prop.ExpansionType;
      newProperty.OriginPath = originConfigName;
      newProperty.ValueIndex = valueIndex;
      newProperty.PropertyIndex = propIndex;
      return newProperty;
        //newCaseProperties.Add(newProperty);
      //};
      //return newCaseProperties;
    }

    private IEnumerable<DymeCase> PostCasesProcessing(IEnumerable<DymeCase> dymeCases, IEnumerable<DymeConfigProperty> postApplyProperties)
    {
      
      Dictionary<string, int> postApplyPropertyValueTrackingIndexes = postApplyProperties.ToDictionary(i => i.CorrelationKey, i => 0);
      foreach (var dymeCase in dymeCases) 
      { 
        foreach (var caseProp in dymeCase.Properties)
        {
          foreach (var postApplyProp in postApplyProperties)
          {
            if (caseProp.CorrelationKey == postApplyProp.CorrelationKey)
            {
              caseProp.Value = postApplyPropertyValueTrackingIndexes[postApplyProp.CorrelationKey].ToString();
              postApplyPropertyValueTrackingIndexes[postApplyProp.CorrelationKey] += 1;
            }
          } 
        }   
      }
      //foreach (var postApplyConfig in postApplyConfigs)
      //{
      //  var relevantCases = dymeCases.Where(dc => DerivesFromOriginConfig(dc, postApplyConfig.Name));
      //  for (var propIndex = 0; propIndex < postApplyConfig.Properties.Count(); propIndex++) 
      //  {
      //    var postApplyConfigProperty = postApplyConfig.Properties[propIndex];
      //    var propertyValueCount = postApplyConfigProperty.Values.Count();
      //    var propertyToCaseIndexList = dymeCases.Select((dc, caseIndex) => caseIndex % propertyValueCount);
      //    for (var caseIndex = 0; caseIndex < relevantCases.Count(); caseIndex++)
      //    {
      //      var dymeCase = dymeCases.ElementAt(caseIndex);
      //      var valueIndex = caseIndex % propertyValueCount;
      //      var newCaseProperty = CasePropertyFromConfigProperty(postApplyConfigProperty, propIndex, valueIndex, postApplyConfig.Name);
      //      dymeCase.Properties = dymeCase.Properties.Append(newCaseProperty);
      //    }
      //  }
      //}
      //return dymeCases;
    }

    private DymeCase RemovePlaceholderFromCase(DymeCase dymeCase)
    {
      var excludedProperties = 
    }

    private bool DerivesFromOriginConfig(DymeCase dymeCase, string configName)
    {
      return dymeCase.Properties.Any(dcp => dcp.OriginConfigPath.Any(cnf => cnf == configName));
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
        /// Because its a property, the child cases will expand eachother. (be added, or additively extracted using the "SelectMany")
        ValidatePropertyNode(node);
        mergedCases = node.Children
          .SelectMany(childNode => CasesFromNodeTree(childNode, node, treePath, configPath))
          .ToList();
      }
      
      else if (IsPropertyValueOrReferencePartNode(node))
      {
        treePath += ":" + node.Value;
        
        if (IsValueNodeWithNoChildren(node)) return CaseFromValueNode(node, parentNode, treePath, configPath);
        /// Is Reference Node...
        configPath = configPath.Append(node.Value).ToArray();
        mergedCases = node.Children
          .Select(childNode => CasesFromNodeTree(childNode, node, treePath, configPath))
          .Aggregate((caseSet1, caseSet2) => MergeCaseSets(caseSet1, caseSet2))
          .ToList();
      }
      
      return Distinct(mergedCases);
    }

    private static IEnumerable<DymeCase> CaseFromValueNode(Node valueNode, Node parent, string treePath, string[] configPath)
    {
      // This method gets called at the leaf-most parts of the node-tree
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
      for ( var i = 0; i < configProperty.Values.Count(); i++) //(var value in configProperty.Values)
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

