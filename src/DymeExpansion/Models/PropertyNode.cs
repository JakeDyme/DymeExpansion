using System.Linq;
using DymeExpansion.Core.Enums;

namespace DymeExpansion.Core.Models
{
  internal class PropertyNode : Node
  {
    public PropertyNode(string name, NodeTypeEnum nodeType, string correlationKey = null, Node parent = null) : base(name, nodeType, parent, correlationKey){}
    

    public ValueNode WithImported(string name)
    {
      return WithValue(name, ValueTypeEnum.ImportedSetup);
    }

    public ValueNode WithValue(string value)
    {
      return WithValue(value, ValueTypeEnum.Text);
    }

    private ValueNode WithValue(string value, ValueTypeEnum leafType) { 
      var leafNode =  Children.FirstOrDefault(n => n.Value == value);
      if (leafNode != null) return leafNode as ValueNode;
      var newLeafNode = new ValueNode(value, NodeTypeEnum.ValueNode, this);
      newLeafNode.LeafType = leafType;
      Children.Add(newLeafNode);
      return newLeafNode as ValueNode;
    }
  }
}
