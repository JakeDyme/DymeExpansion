using DymeExpansion.Core.Models;
using NUnit.Framework;
using System.Collections.Generic;

namespace DymeExpansion.Core.Tests.Models
{
  public class Case_Tests
  {
    [Test]
    public void Property_GivenPropertyName_ExpectPropertyValue()
    {
      // Arrange...
      var sut = new Case();
      sut.Properties = new List<CaseProperty>()
      {
        new CaseProperty("Name", "Bob")
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
      var sut = new Case();
      sut.Properties = new List<CaseProperty>()
      {
        new CaseProperty("Name", "Bob")
      };
      // Act...
      var result = sut["Name"];
      // Assert...
      Assert.AreEqual("Bob", result);
    }

    [Test]
    public void PropertyOrDefault_GivenNonExistentPropertyName_ExpectNull()
    {
      // Arrange...
      var sut = new Case();
      sut.Properties = new List<CaseProperty>()
      {
        new CaseProperty("Name", "Bob")
      };
      // Act...
      var result = sut.PropertyOrDefault("SomeOtherField");
      // Assert...
      Assert.IsNull(result);
    }


  }
}