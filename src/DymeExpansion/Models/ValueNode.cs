using System.Linq;
using DymeExpansion.Enums;

namespace DymeExpansion.Models
{
  public class ValueNode : Node
  {
    public ValueNode(string value, NodeTypeEnum nodeType, Node parent = null) : 
      base(value, nodeType, parent, null){}
    
    public PropertyNode WithProperty(string propertyName, string correlationId = null) { 
      var property =  Children.FirstOrDefault(n => n.Value == propertyName);
      if (property != null) return property as PropertyNode;
      var newPropertyNode = new PropertyNode(propertyName, NodeTypeEnum.PropertyNode, correlationId, this);
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
