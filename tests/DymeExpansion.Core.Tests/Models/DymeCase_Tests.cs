using DymeExpansion.Core.Models;
using NUnit.Framework;
using System.Collections.Generic;

namespace DymeExpansion.Core.Tests.Models
{
  public class DymeCase_Tests
  {
    [Test]
    public void Property_GivenPropertyName_ExpectPropertyValue()
    {
      // Arrange...
      var sut = new DymeCase();
      sut.Properties = new List<DymeCaseProperty>()
      {
        new DymeCaseProperty("Name", "Bob")
      };
      // Act...
      var result = sut.Property("Name");
      // Assert...
      Assert.AreEqual("Bob", result);
    }

    [Test]
    public void SquareBrackets_GivenPropertyName_ExpectPropertyValue()
    {
      // Arrange...
      var sut = new DymeCase();
      sut.Properties = new List<DymeCaseProperty>()
      {
        new DymeCaseProperty("Name", "Bob")
      };
      // Act...
      var result = sut["Name"];
      // Assert...
      Assert.AreEqual("Bob", result);
    }


  }
}