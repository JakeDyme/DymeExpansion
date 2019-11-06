using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using DymeExpansion.Core.Enums;
using DymeExpansion.Core.Interfaces;

namespace DymeExpansion.Core.Models
{
  [DebuggerDisplay("{Value}")]
  public class Node : INode
  {
    public string Value { get;set;}
    public ValueTypeEnum LeafType {get;set; } = ValueTypeEnum.Irrelevant;
    public NodeTypeEnum NodeType { get;set;}
    public string CorrelationId { get;set; }
    public List<Node> Children { get;set;} = new List<Node>();
    internal Node Parent { get;set; }
    
    public Node( string name, NodeTypeEnum nodeType, Node parent = null, string correlationId = null) { 
      Value = name;
      NodeType = nodeType;
      Parent = parent;
      CorrelationId = correlationId ?? Guid.NewGuid().ToString();
    }

    public string HashAsExpression(Node parent = null)
    {
      if (Children.Count == 0) return $"{parent.Value}:{Value}";
      switch (NodeType) {
        case NodeTypeEnum.PropertyNode: 
            return $"(" + string.Join(" OR ", Children.Select(c => c.HashAsExpression(this))) + ")";
        case NodeTypeEnum.ValueNode:
            return $"(" + string.Join(" AND ", Children.Select(c => c.HashAsExpression(this))) + ")";
      }
      throw new Exception("Should never reach here");
    }

    public int CalculateTestCaseCount()
    {
      if (!Children.Any()) return 1;
      switch (NodeType) {
        case NodeTypeEnum.ValueNode: return Children.Select(c => c.CalculateTestCaseCount()).Aggregate((a,b) => a * b);
        case NodeTypeEnum.PropertyNode: return Children.Select(c => c.CalculateTestCaseCount()).Aggregate((a,b) => a + b);
      }
      throw new Exception("Should never reach here");
    }

  }
}