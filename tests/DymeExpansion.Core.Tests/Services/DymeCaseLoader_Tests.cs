using DymeExpansion.Core.Enums;
using DymeExpansion.Core.Models;
using DymeExpansion.Core.Services;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace DymeExpansion.Core.Tests.Services
{
  public class DymeCaseLoader_Tests
  {

    public List<DymeConfig> GetMockConfigsLibrary()
    {
      return new List<DymeConfig>()
      {
        new DymeConfig()
        {
          Name = "TestConfig",
          Properties = new List<DymeConfigProperty>(){
            new DymeConfigProperty("IMPORT.Devices",new[]{"IPhoneX-Setup", "SamusungS7-Setup" }),
            new DymeConfigProperty("IMPORT.SiteInfo",new[]{"SearchSites-Setup", "SocialSites-Setup" })
          }
        },

        new DymeConfig
        {
          Name = "IPhoneX-Setup",
          Properties = new List<DymeConfigProperty>(){
            new DymeConfigProperty("IMPORT.Accounts","TeamAccount-Setup"),
            new DymeConfigProperty("Platform", "Ios")
          }
        },

        new DymeConfig
        {
          Name = "SamusungS7-Setup",
          Properties = new List<DymeConfigProperty>(){
            new DymeConfigProperty("IMPORT", "JakesAccount-Setup"),
            new DymeConfigProperty("Platform",new[]{"Android"})
          }
        },

        new DymeConfig
        {
          Name = "TeamAccount-Setup",
          Properties = new List<DymeConfigProperty>(){
            new DymeConfigProperty("Username",new[]{"Tom", "Bob" }),
            new DymeConfigProperty("SeleniumHub","Saucelabs")
          }
        },

        new DymeConfig
        {
          Name = "JakesAccount-Setup",
          Properties = new List<DymeConfigProperty>(){
            new DymeConfigProperty("Username", "Jake"),
            new DymeConfigProperty("SeleniumHub", "LocalFarm")
          },
        },

        new DymeConfig
        {
          Name = "SearchSites-Setup",
          Properties = new List<DymeConfigProperty>(){
            new DymeConfigProperty("SiteName", "google"),
            new DymeConfigProperty("Sid","23")
          }
        },

        new DymeConfig
        {
          Name = "SocialSites-Setup",
          Properties = new List<DymeConfigProperty>(){
            new DymeConfigProperty("SiteName",new[]{"facebook", "twitter" }),
            new DymeConfigProperty("Sid",new[]{"41", "42", "43" })
          }
        }

      };
    }

    private string CaseToString(DymeCase caseX, string separator = ",")
    {
      return caseX.Properties.OrderBy(i => i.Name).Select(s => s.Value).Aggregate((a, b) => $"{a}{separator}{b}");
    }

    public void GetCaseSet_GivenSimpleConfig()
    {
      var y = DymeConfig.New("TestConfig")
        .AddProperty("IMPORT.Devices", new[] { "IPhoneX-Setup", "SamusungS7-Setup" });
    }

    [Test]
    public void ExpandSetup_GivenPermutationSet_Expect2Permutations()
    {

      // Arrange...
      var configLibrary = new List<DymeConfig>();
      var config = DymeConfig.New("TestConfig")
        .AddProperty("a", new[] { "1", "2" })
        .AddProperty("b", "1")
        .AddProperty("c", "1");
      var expectedCases = new[]{
        "p:a(1) p:b(1) p:c(1)",
        "p:a(2) p:b(1) p:c(1)"
      };

      // Act...
      var testCases = new DymeCaseLoader().CasesFromConfigs(config, configLibrary);

      // Assert...
      var testCaseString = testCases.Select(tc => new DymeCaseLoader().CaseToString(tc)).ToList();
      Assert.AreEqual(expectedCases, testCaseString);
    }

    [Test]
    public void ExpandSetup_GivenPermutationSet_Expect4Permutations()
    {

      // Arrange...
      var configLibrary = new List<DymeConfig>();
      var config = DymeConfig.New("TestConfig")
        .AddProperty("a", new[] { "1", "2" })
        .AddProperty("b", new[] { "1", "2" })
        .AddProperty("c", "1");

      var expectedCases = new[]{
        "p:a(1) p:b(1) p:c(1)",
        "p:a(1) p:b(2) p:c(1)",
        "p:a(2) p:b(1) p:c(1)",
        "p:a(2) p:b(2) p:c(1)"
      };

      // Act...
      var testCases = new DymeCaseLoader().CasesFromConfigs(config, configLibrary);

      // Assert
      var testCaseString = testCases.Select(tc => new DymeCaseLoader().CaseToString(tc)).ToList();
      Assert.AreEqual(4, testCases.Count());
      CollectionAssert.AreEquivalent(expectedCases, testCaseString);
    }

    [Test]
    public void ExpandSetup_GivenPermutationSet_Expect2CorrelatedPermutations()
    {
      // Arrange...
      var configLibrary = new List<DymeConfig>();
      var config = DymeConfig.New("TestConfig")
        .AddProperty("a", new[] { "1", "2" }, "1")
        .AddProperty("b", new[] { "1", "2" }, "1")
        .AddProperty("c", "1");

      var expectedCases = new[]{
        "p:a(1) p:b(1) p:c(1)",
        "p:a(2) p:b(2) p:c(1)"
      };

      // Act...
      var testCases = new DymeCaseLoader().CasesFromConfigs(config, configLibrary);

      // Assert
      var testCaseString = testCases.Select(tc => new DymeCaseLoader().CaseToString(tc)).ToList();
      Assert.AreEqual(2, testCases.Count());
      CollectionAssert.AreEquivalent(expectedCases, testCaseString);
    }

    [Test]
    public void PoolProperties_GivenSameAmountOfValues_ExpectValuesAreCorrelated()
    {
      // Arrange...
      var config = DymeConfig.New("TestConfig")
        .AddProperty("a", new[] { "1", "2" })
        .AddProperty("b", new[] { "1", "2" }, ExpansionTypeEnum.pool);

      var expectedCases = new[]{
        "p:a(1) p:b(1)",
        "p:a(2) p:b(2)"
      };

      // Act...
      var testCases = DymeCaseLoader.CasesFromConfig(config);

      // Assert
      var testCaseString = testCases.Select(tc => new DymeCaseLoader().CaseToString(tc)).ToList();
      Assert.AreEqual(2, testCases.Count());
      CollectionAssert.AreEquivalent(expectedCases, testCaseString);
    }

    [Test]
    public void PoolProperties_GivenLessPoolProperties_ExpectPoolPropertiesAreRecycled()
    {
      // Arrange...
      var config = DymeConfig.New("TestConfig")
        .AddProperty("a", new[] { "1", "2", "3", "4" })
        .AddProperty("b", new[] { "1", "2" }, ExpansionTypeEnum.pool);

      var expectedCases = new[]{
        "p:a(1) p:b(1)",
        "p:a(2) p:b(2)",
        "p:a(3) p:b(1)",
        "p:a(4) p:b(2)"
      };

      // Act...
      var testCases = DymeCaseLoader.CasesFromConfig(config);

      // Assert
      var testCaseString = testCases.Select(tc => new DymeCaseLoader().CaseToString(tc)).ToList();
      CollectionAssert.AreEquivalent(expectedCases, testCaseString);
    }

    [Test]
    public void PoolProperties_GivenBuriedInConfig_AppropriateUsage()
    {
      // Arrange...
      var config1 = DymeConfig.New("TestConfig1")
        .AddProperty("b", new[] { "1", "2" }, ExpansionTypeEnum.pool);

      var config2 = DymeConfig.New("TestConfig2")
        .AddProperty("IMPORT", "TestConfig1")
        .AddProperty("a", new[] { "1", "2", "3", "4" });

      var expectedCases = new[]{
        "p:a(1) p:b(1)",
        "p:a(2) p:b(2)",
        "p:a(3) p:b(1)",
        "p:a(4) p:b(2)"
      };

      // Act...
      var testCases = DymeCaseLoader.CasesFromConfig(config2, new []{ config1 });

      // Assert
      var testCaseString = testCases.Select(tc => new DymeCaseLoader().CaseToString(tc)).ToList();
      CollectionAssert.AreEquivalent(expectedCases, testCaseString);
    }

    [Test]
    public void PoolProperties_GivenMorePoolProperties_ExpectExtraPoolPropertiesAreDropped()
    {
      // Arrange...
      var config = DymeConfig.New("TestConfig")
        .AddProperty("a", new[] { "1", "2" })
        .AddProperty("b", new[] { "1", "2", "3", "4" }, ExpansionTypeEnum.pool);

      var expectedCases = new[]{
        "p:a(1) p:b(1)",
        "p:a(2) p:b(2)"
      };

      // Act...
      var testCases = DymeCaseLoader.CasesFromConfig(config);

      // Assert
      var testCaseString = testCases.Select(tc => new DymeCaseLoader().CaseToString(tc)).ToList();
      CollectionAssert.AreEquivalent(expectedCases, testCaseString);
    }

    [Test]
    public void CorrelationSameConfigs()
    {
      // Arrange...
      var config = DymeConfig.New("TestConfig")
        .AddProperty("a", new[] { "1", "2" }, "correlationKey")
        .AddProperty("b", new[] { "1", "2" }, "correlationKey");

      var expectedCases = new[]{
        "p:a(1) p:b(1)",
        "p:a(2) p:b(2)"
      };

      // Act...
      var testCases = DymeCaseLoader.CasesFromConfig(config);

      // Assert
      var testCaseString = testCases.Select(tc => new DymeCaseLoader().CaseToString(tc)).ToList();
      CollectionAssert.AreEquivalent(expectedCases, testCaseString);
    }

    [Test]
    public void CorrelationImportedConfigs()
    {
      // Arrange...
      var configLibrary = new List<DymeConfig>(){
      DymeConfig.New("TestConfig1")
        .AddProperty("a", new[] { "1", "2" }, "correlationKey")
      };

      var config = DymeConfig.New("TestConfig2")
        .AddProperty("IMPORT", "TestConfig1")
        .AddProperty("b", new[] { "1", "2" }, "correlationKey");

      var expectedCases = new[]{
        "p:a(1) p:b(1)",
        "p:a(2) p:b(2)"
      };

      // Act...
      var testCases = DymeCaseLoader.CasesFromConfig(config, configLibrary);

      // Assert
      var testCaseString = testCases.Select(tc => new DymeCaseLoader().CaseToString(tc)).ToList();
      Assert.AreEqual(2, testCases.Count());
      CollectionAssert.AreEquivalent(expectedCases, testCaseString);
    }

    [Test]
    public void CorrelationComposedConfigs()
    {
      // Arrange...
      var configLibrary = new List<DymeConfig>(){
      DymeConfig.New("TestConfig1")
        .AddProperty("a", new[] { "1", "2" }, "correlationKey"),
      DymeConfig.New("TestConfig2")
        .AddProperty("b", new[] { "1", "2" }, "correlationKey")
      };

      var config = DymeConfig.New("TestConfig2")
        .AddProperty("IMPORT", "TestConfig1")
        .AddProperty("IMPORT", "TestConfig2");

      var expectedCases = new[]{
        "p:a(1) p:b(1)",
        "p:a(2) p:b(2)"
      };

      // Act...
      var testCases = DymeCaseLoader.CasesFromConfig(config, configLibrary);

      // Assert
      var testCaseString = testCases.Select(tc => new DymeCaseLoader().CaseToString(tc)).ToList();
      Assert.AreEqual(2, testCases.Count());
      CollectionAssert.AreEquivalent(expectedCases, testCaseString);
    }

    [Test]
    public void CorrelationComposedConfigAndField()
    {
      // Arrange...
      var configLibrary = new List<DymeConfig>(){
      DymeConfig.New("Composition1")
        .AddProperty("a", "1"),

      DymeConfig.New("Composition2")
        .AddProperty("a", "2"),
      };

      var config = DymeConfig.New("Super")
        .AddProperty("IMPORT", new[] { "Composition1", "Composition2" }, "correlationKeyX")
        .AddProperty("c", new[] { "1", "2" }, "correlationKeyX");

      var expectedCases = new[]{
        "p:a(1) p:c(1)",
        "p:a(2) p:c(2)"
      };

      // Act...
      var testCases = DymeCaseLoader.CasesFromConfig(config, configLibrary);

      // Assert
      var testCaseString = testCases.Select(tc => new DymeCaseLoader().CaseToString(tc)).ToList();
      Assert.AreEqual(2, testCases.Count());
      CollectionAssert.AreEquivalent(expectedCases, testCaseString);
    }

    [Test]
    public void ResolveSetup()
    {
      // Arrange...
      var configLibrary = new List<DymeConfig>(){
        DymeConfig.New("D[1]")
          .AddProperty("a", "1")
      };

      var testConfig = DymeConfig.New("TestConfig")
        .AddProperty("IMPORT.DefaultSetup", "D[1]")
        .AddProperty("b", "1");

      var expectedCases = new[]{
        "p:a(1) p:b(1)"
      };

      // Act...
      var testCases = new DymeCaseLoader().CasesFromConfigs(testConfig, configLibrary);

      // Assert
      var testCaseString = testCases.Select(tc => new DymeCaseLoader().CaseToString(tc)).ToList();
      CollectionAssert.AreEquivalent(expectedCases, testCaseString);
    }


    [Test]
    public void ResolveSetup_2DefaultProperties()
    {
      // Arrange...
      var configLibrary = new List<DymeConfig>(){
        DymeConfig.New("D[1]")
          .AddProperty("a", "1")
          .AddProperty("c", "1")
      };

      var testConfig = DymeConfig.New("TestConfig")
        .AddProperty("IMPORT.DefaultSetup", "D[1]")
        .AddProperty("b", "1");

      var expectedCases = new[]{
        "p:a(1) p:b(1) p:c(1)"
      };

      // Act...
      var testCases = new DymeCaseLoader().CasesFromConfigs(testConfig, configLibrary);

      // Assert
      var testCaseString = testCases.Select(tc => new DymeCaseLoader().CaseToString(tc)).ToList();
      CollectionAssert.AreEquivalent(expectedCases, testCaseString);
    }

    [Test]
    public void ResolveSetup_1ExpandedDefaultSetup()
    {
      // Arrange...
      var configLibrary = new List<DymeConfig>(){
        DymeConfig.New("D[1]")
          .AddProperty("a", "1")
          .AddProperty("c", "1"),
        DymeConfig.New("D[2]")
          .AddProperty("m", "1")
          .AddProperty("n", "1")
      };

      var testConfig = DymeConfig.New("TestConfig")
        .AddProperty("IMPORT.DefaultSetup", new[] { "D[1]", "D[2]" })
        .AddProperty("b", "1");

      var expectedCases = new[]{
        "p:a(1) p:b(1) p:c(1)",
        "p:b(1) p:m(1) p:n(1)"
      };

      // Act...
      var testCases = new DymeCaseLoader().CasesFromConfigs(testConfig, configLibrary);

      // Assert
      var testCaseString = testCases.Select(tc => new DymeCaseLoader().CaseToString(tc)).ToList();
      CollectionAssert.AreEquivalent(expectedCases, testCaseString);
    }

    [Test]
    public void ResolveSetup_1DefaultExpandedProperty()
    {
      // Arrange...
      var configLibrary = new List<DymeConfig>(){
        DymeConfig.New("D[1]")
          .AddProperty("a", new[]{ "1", "2"})
          .AddProperty("c", "1"),
      };

      var testConfig = DymeConfig.New("TestConfig")
        .AddProperty("IMPORT.DefaultSetup", "D[1]")
        .AddProperty("b", "1");

      var expectedCases = new[]{
        "p:a(1) p:b(1) p:c(1)",
        "p:a(2) p:b(1) p:c(1)"
      };

      // Act...
      var testCases = new DymeCaseLoader().CasesFromConfigs(testConfig, configLibrary);

      // Assert
      var testCaseString = testCases.Select(tc => new DymeCaseLoader().CaseToString(tc)).ToList();
      CollectionAssert.AreEquivalent(expectedCases, testCaseString);
    }


    [Test]
    public void ResolveSetup_1ExpandedDefaultSetupsEachWith2ExpandedProperties()
    {
      // Arrange...
      var configLibrary = new List<DymeConfig>(){
        DymeConfig.New("D[1]")
          .AddProperty("a", new[]{ "T", "F" }),
        DymeConfig.New("D[2]")
          .AddProperty("b", new[]{ "T", "F"})
      };

      var testConfig = DymeConfig.New("TestConfig")
        .AddProperty("IMPORT.DefaultSetup", new[] { "D[1]", "D[2]" })
        .AddProperty("c", new[] { "T", "F" });

      var expectedCases = new[]{
        "p:a(T) p:c(T)",
        "p:a(T) p:c(F)",
        "p:a(F) p:c(T)",
        "p:a(F) p:c(F)",
        "p:b(T) p:c(T)",
        "p:b(T) p:c(F)",
        "p:b(F) p:c(T)",
        "p:b(F) p:c(F)"
      };

      // Act...
      var testCases = new DymeCaseLoader().CasesFromConfigs(testConfig, configLibrary);

      // Assert
      var testCaseString = testCases.Select(tc => new DymeCaseLoader().CaseToString(tc)).ToList();
      CollectionAssert.AreEquivalent(expectedCases, testCaseString);

    }

    [Test]
    public void ResolveSetup_2DefaultSetups()
    {
      // Arrange...
      var configLibrary = new List<DymeConfig>(){
        DymeConfig.New("D[1]")
          .AddProperty("a", "1")
          .AddProperty("b", "1"),
        DymeConfig.New("D[2]")
          .AddProperty("a", "2")
          .AddProperty("c", "1")
      };

      var testConfig = DymeConfig.New("TestConfig")
        .AddProperty("IMPORT.DefaultSetup", new[] { "D[1]" })
        .AddProperty("IMPORT.DefaultSetup2", new[] { "D[2]" })
        .AddProperty("d", "1");

      var expectedCases = new[]{
        "p:a(2) p:b(1) p:c(1) p:d(1)"
      };

      // Act...
      var testCases = new DymeCaseLoader().CasesFromConfigs(testConfig, configLibrary);

      // Assert
      var testCaseString = testCases.Select(tc => new DymeCaseLoader().CaseToString(tc)).ToList();
      CollectionAssert.AreEquivalent(expectedCases, testCaseString);
    }

    [Test]
    public void ResolveSetup_2DefaultSetupsWithExpandedProperties()
    {
      // Arrange...
      var configLibrary = new List<DymeConfig>(){
        DymeConfig.New("D[1]")
          .AddProperty("a", new[]{ "1", "2", "3" })
          .AddProperty("b", "1"),
        DymeConfig.New("D[2]")
          .AddProperty("a", new[]{ "4", "5" })
          .AddProperty("c", "1")
      };

      var testConfig = DymeConfig.New("TestConfig")
        .AddProperty("IMPORT.DefaultSetup", new[] { "D[1]" })
        .AddProperty("IMPORT.DefaultSetup2", new[] { "D[2]" })
        .AddProperty("d", "1");

      var expectedCases = new[]{
        "p:a(4) p:b(1) p:c(1) p:d(1)",
        "p:a(5) p:b(1) p:c(1) p:d(1)"
      };

      // Act...
      var testCases = new DymeCaseLoader().CasesFromConfigs(testConfig, configLibrary);

      // Assert
      var testCaseString = testCases.Select(tc => new DymeCaseLoader().CaseToString(tc)).ToList();
      CollectionAssert.AreEquivalent(expectedCases, testCaseString);
    }


    [Test]
    public void ResolveSetup_3DefaultSetups2LevelsDeep()
    {
      // Arrange...
      var configLibrary = new List<DymeConfig>(){
        DymeConfig.New("D[1]")
          .AddProperty("a", "1"),
        DymeConfig.New("D[2]")
          .AddProperty("a", "2"),
        DymeConfig.New("D[3]")
          .AddProperty("IMPORT.DefaultSetup", "D[2]")
          .AddProperty("a", "3")
      };

      var testConfig = DymeConfig.New("TestConfig")
        .AddProperty("IMPORT.DefaultSetup", "D[1]")
        .AddProperty("IMPORT.DefaultSetup2", "D[3]");

      var expectedCases = new[]{
        "p:a(3)"
      };

      // Act...
      var testCases = new DymeCaseLoader().CasesFromConfigs(testConfig, configLibrary);

      // Assert
      var testCaseString = testCases.Select(tc => new DymeCaseLoader().CaseToString(tc)).ToList();
      CollectionAssert.AreEquivalent(expectedCases, testCaseString);
    }


    [Test]
    public void ResolveSetup_3DefaultSetups2LevelsDeepWithExpansions()
    {
      // Arrange...
      var configLibrary = new List<DymeConfig>(){
        DymeConfig.New("D[1]")
          .AddProperty("a", "1"),
        DymeConfig.New("D[2]")
          .AddProperty("a", new[] { "2.1", "2.2" }),
        DymeConfig.New("D[3]")
          .AddProperty("IMPORT.DefaultSetup", "D[2]")
      };

      var testConfig = DymeConfig.New("TestConfig")
        .AddProperty("IMPORT.DefaultSetup", "D[1]")
        .AddProperty("IMPORT.DefaultSetup2", new[] { "D[3]" });

      var expectedCases = new[]{
        "p:a(2.1)",
        "p:a(2.2)",
      };

      // Act...
      var testCases = new DymeCaseLoader().CasesFromConfigs(testConfig, configLibrary);

      // Assert
      var testCaseString = testCases.Select(tc => new DymeCaseLoader().CaseToString(tc)).ToList();
      CollectionAssert.AreEquivalent(expectedCases, testCaseString);
    }

    [Test]
    public void ResolveSetup_3DefaultSetups2LevelsDeepWithExpansionsReversePriority()
    {
      // Arrange...
      var configLibrary = new List<DymeConfig>(){
        DymeConfig.New("D[1]")
          .AddProperty("a", "1"),
        DymeConfig.New("D[2]")
          .AddProperty("a", new[] { "2.1", "2.2" }),
        DymeConfig.New("D[3]")
          .AddProperty("IMPORT.DefaultSetup", "D[2]")
      };

      var testConfig = DymeConfig.New("TestConfig")
        .AddProperty("IMPORT.DefaultSetup", "D[3]")
        .AddProperty("IMPORT.DefaultSetup2", new[] { "D[1]" });

      var expectedCases = new[]{
        "p:a(1)",
      };

      // Act...
      var testCases = new DymeCaseLoader().CasesFromConfigs(testConfig, configLibrary);

      // Assert
      var testCaseString = testCases.Select(tc => new DymeCaseLoader().CaseToString(tc)).ToList();
      CollectionAssert.AreEquivalent(expectedCases, testCaseString);
    }

  }
}