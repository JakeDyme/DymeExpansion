using System.Linq;
using DymeExpansion.Core.Enums;

namespace DymeExpansion.Core.Models
{
  internal class ValueNode : Node
  {
    public ValueNode(string value, NodeTypeEnum nodeType, Node parent = null) : 
      base(value, nodeType, parent, null){}
    
    public PropertyNode WithProperty(string propertyName, string correlationKey = null) { 
      var property =  Children.FirstOrDefault(n => n.Value == propertyName);
      if (property != null) return property as PropertyNode;
      var newPropertyNode = new PropertyNode(propertyName, NodeTypeEnum.PropertyNode, correlationKey, this);
      newPropertyNode.LeafType = ValueTypeEnum.Irrelevant;
      Children.Add(newPropertyNode);
      return newPropertyNode;
    }

    public ValueNode AndValue(string value, ValueTypeEnum leafType = ValueTypeEnum.Text) { 
      return (this.Parent as PropertyNode).WithValue(value);
    }

    public PropertyNode AndProperty(string propertyName) { 
      return (this.Parent.Parent as ValueNode).WithProperty(propertyName);
    }
  }
  
}
