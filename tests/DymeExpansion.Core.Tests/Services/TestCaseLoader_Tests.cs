using DymeExpansion.Core.Enums;
using DymeExpansion.Core.Models;
using DymeExpansion.Core.Services;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace DymeExpansion.Core.Tests.Services
{
  public class TestCaseLoader_Tests
  {

    public List<Config> GetMockConfigsLibrary()
    {
      return new List<Config>()
      {
        new Config()
        {
          Name = "TestConfig",
          Properties = new List<ConfigProperty>(){
            new ConfigProperty("IMPORT.Devices",new[]{"IPhoneX-Setup", "SamusungS7-Setup" }),
            new ConfigProperty("IMPORT.SiteInfo",new[]{"SearchSites-Setup", "SocialSites-Setup" })
          }
        },

        new Config
        {
          Name = "IPhoneX-Setup",
          Properties = new List<ConfigProperty>(){
            new ConfigProperty("IMPORT.Accounts","TeamAccount-Setup"),
            new ConfigProperty("Platform", "Ios")
          }
        },

        new Config
        {
          Name = "SamusungS7-Setup",
          Properties = new List<ConfigProperty>(){
            new ConfigProperty("IMPORT", "JakesAccount-Setup"),
            new ConfigProperty("Platform",new[]{"Android"})
          }
        },

        new Config
        {
          Name = "TeamAccount-Setup",
          Properties = new List<ConfigProperty>(){
            new ConfigProperty("Username",new[]{"Tom", "Bob" }),
            new ConfigProperty("SeleniumHub","Saucelabs")
          }
        },

        new Config
        {
          Name = "JakesAccount-Setup",
          Properties = new List<ConfigProperty>(){
            new ConfigProperty("Username", "Jake"),
            new ConfigProperty("SeleniumHub", "LocalFarm")
          },
        },

        new Config
        {
          Name = "SearchSites-Setup",
          Properties = new List<ConfigProperty>(){
            new ConfigProperty("SiteName", "google"),
            new ConfigProperty("Sid","23")
          }
        },

        new Config
        {
          Name = "SocialSites-Setup",
          Properties = new List<ConfigProperty>(){
            new ConfigProperty("SiteName",new[]{"facebook", "twitter" }),
            new ConfigProperty("Sid",new[]{"41", "42", "43" })
          }
        }

      };
    }

    private string CaseToString(Case caseX, string separator = ",")
    {
      return caseX.Properties.OrderBy(i => i.Name).Select(s => s.Value).Aggregate((a, b) => $"{a}{separator}{b}");
    }

    public void GetCaseSet_GivenSimpleConfig()
    {
      var y = Config.New("TestConfig")
        .AddProperty("IMPORT.Devices", new[] { "IPhoneX-Setup", "SamusungS7-Setup" });
    }

    [Test]
    public void ExpandSetup_GivenPermutationSet_Expect2Permutations()
    {

      // Arrange...
      var configLibrary = new List<Config>();
      var config = Config.New("TestConfig")
        .AddProperty("a", new[] { "1", "2" })
        .AddProperty("b", "1")
        .AddProperty("c", "1");
      var expectedCases = new[]{
        "p:a(1) p:b(1) p:c(1)",
        "p:a(2) p:b(1) p:c(1)"
      };

      // Act...
      var testCases = new TestCaseLoader().CasesFromConfigs(config, configLibrary);

      // Assert...
      var testCaseString = testCases.Select(tc => new TestCaseLoader().CaseToString(tc)).ToList();
      Assert.AreEqual(expectedCases, testCaseString);
    }

    [Test]
    public void ExpandSetup_GivenPermutationSet_Expect4Permutations()
    {

      // Arrange...
      var configLibrary = new List<Config>();
      var config = Config.New("TestConfig")
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
      var testCases = new TestCaseLoader().CasesFromConfigs(config, configLibrary);

      // Assert
      var testCaseString = testCases.Select(tc => new TestCaseLoader().CaseToString(tc)).ToList();
      Assert.AreEqual(4, testCases.Count());
      CollectionAssert.AreEquivalent(expectedCases, testCaseString);
    }

    [Test]
    public void ExpandSetup_GivenPermutationSet_Expect2CorrelatedPermutations()
    {
      // Arrange...
      var configLibrary = new List<Config>();
      var config = Config.New("TestConfig")
        .AddProperty("a", new[] { "1", "2" }, "1")
        .AddProperty("b", new[] { "1", "2" }, "1")
        .AddProperty("c", "1", "2");

      var expectedCases = new[]{
        "p:a(1) p:b(1) p:c(1)",
        "p:a(2) p:b(2) p:c(1)"
      };

      // Act...
      var testCases = new TestCaseLoader().CasesFromConfigs(config, configLibrary);

      // Assert
      var testCaseString = testCases.Select(tc => new TestCaseLoader().CaseToString(tc)).ToList();
      Assert.AreEqual(2, testCases.Count());
      CollectionAssert.AreEquivalent(expectedCases, testCaseString);
    }

    [Test]
    public void ResolveSetup()
    {
      // Arrange...
      var configLibrary = new List<Config>(){
        Config.New("D[1]")
          .AddProperty("a", "1")
      };

      var testConfig = Config.New("TestConfig")
        .AddProperty("IMPORT.DefaultSetup", "D[1]")
        .AddProperty("b", "1");

      var expectedCases = new[]{
        "p:a(1) p:b(1)"
      };

      // Act...
      var testCases = new TestCaseLoader().CasesFromConfigs(testConfig, configLibrary);

      // Assert
      var testCaseString = testCases.Select(tc => new TestCaseLoader().CaseToString(tc)).ToList();
      CollectionAssert.AreEquivalent(expectedCases, testCaseString);
    }


    [Test]
    public void ResolveSetup_2DefaultProperties()
    {
      // Arrange...
      var configLibrary = new List<Config>(){
        Config.New("D[1]")
          .AddProperty("a", "1")
          .AddProperty("c", "1")
      };

      var testConfig = Config.New("TestConfig")
        .AddProperty("IMPORT.DefaultSetup", "D[1]")
        .AddProperty("b", "1");

      var expectedCases = new[]{
        "p:a(1) p:b(1) p:c(1)"
      };

      // Act...
      var testCases = new TestCaseLoader().CasesFromConfigs(testConfig, configLibrary);

      // Assert
      var testCaseString = testCases.Select(tc => new TestCaseLoader().CaseToString(tc)).ToList();
      CollectionAssert.AreEquivalent(expectedCases, testCaseString);
    }

    [Test]
    public void ResolveSetup_1ExpandedDefaultSetup()
    {
      // Arrange...
      var configLibrary = new List<Config>(){
        Config.New("D[1]")
          .AddProperty("a", "1")
          .AddProperty("c", "1"),
        Config.New("D[2]")
          .AddProperty("m", "1")
          .AddProperty("n", "1")
      };

      var testConfig = Config.New("TestConfig")
        .AddProperty("IMPORT.DefaultSetup", new[] { "D[1]", "D[2]" })
        .AddProperty("b", "1");

      var expectedCases = new[]{
        "p:a(1) p:b(1) p:c(1)",
        "p:b(1) p:m(1) p:n(1)"
      };

      // Act...
      var testCases = new TestCaseLoader().CasesFromConfigs(testConfig, configLibrary);

      // Assert
      var testCaseString = testCases.Select(tc => new TestCaseLoader().CaseToString(tc)).ToList();
      CollectionAssert.AreEquivalent(expectedCases, testCaseString);
    }

    [Test]
    public void ResolveSetup_1DefaultExpandedProperty()
    {
      // Arrange...
      var configLibrary = new List<Config>(){
        Config.New("D[1]")
          .AddProperty("a", new[]{ "1", "2"})
          .AddProperty("c", "1"),
      };

      var testConfig = Config.New("TestConfig")
        .AddProperty("IMPORT.DefaultSetup", "D[1]")
        .AddProperty("b", "1");

      var expectedCases = new[]{
        "p:a(1) p:b(1) p:c(1)",
        "p:a(2) p:b(1) p:c(1)"
      };

      // Act...
      var testCases = new TestCaseLoader().CasesFromConfigs(testConfig, configLibrary);

      // Assert
      var testCaseString = testCases.Select(tc => new TestCaseLoader().CaseToString(tc)).ToList();
      CollectionAssert.AreEquivalent(expectedCases, testCaseString);
    }


    [Test]
    public void ResolveSetup_1ExpandedDefaultSetupsEachWith2ExpandedProperties()
    {
      // Arrange...
      var configLibrary = new List<Config>(){
        Config.New("D[1]")
          .AddProperty("a", new[]{ "T", "F" }),
        Config.New("D[2]")
          .AddProperty("b", new[]{ "T", "F"})
      };

      var testConfig = Config.New("TestConfig")
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
      var testCases = new TestCaseLoader().CasesFromConfigs(testConfig, configLibrary);

      // Assert
      var testCaseString = testCases.Select(tc => new TestCaseLoader().CaseToString(tc)).ToList();
      CollectionAssert.AreEquivalent(expectedCases, testCaseString);

    }

    [Test]
    public void ResolveSetup_2DefaultSetups()
    {
      // Arrange...
      var configLibrary = new List<Config>(){
        Config.New("D[1]")
          .AddProperty("a", "1")
          .AddProperty("b", "1"),
        Config.New("D[2]")
          .AddProperty("a", "2")
          .AddProperty("c", "1")
      };

      var testConfig = Config.New("TestConfig")
        .AddProperty("IMPORT.DefaultSetup", new[] { "D[1]" })
        .AddProperty("IMPORT.DefaultSetup2", new[] { "D[2]" })
        .AddProperty("d", "1");

      var expectedCases = new[]{
        "p:a(2) p:b(1) p:c(1) p:d(1)"
      };

      // Act...
      var testCases = new TestCaseLoader().CasesFromConfigs(testConfig, configLibrary);

      // Assert
      var testCaseString = testCases.Select(tc => new TestCaseLoader().CaseToString(tc)).ToList();
      CollectionAssert.AreEquivalent(expectedCases, testCaseString);
    }

    [Test]
    public void ResolveSetup_2DefaultSetupsWithExpandedProperties()
    {
      // Arrange...
      var configLibrary = new List<Config>(){
        Config.New("D[1]")
          .AddProperty("a", new[]{ "1", "2", "3" })
          .AddProperty("b", "1"),
        Config.New("D[2]")
          .AddProperty("a", new[]{ "4", "5" })
          .AddProperty("c", "1")
      };

      var testConfig = Config.New("TestConfig")
        .AddProperty("IMPORT.DefaultSetup", new[] { "D[1]" })
        .AddProperty("IMPORT.DefaultSetup2", new[] { "D[2]" })
        .AddProperty("d", "1");

      var expectedCases = new[]{
        "p:a(4) p:b(1) p:c(1) p:d(1)",
        "p:a(5) p:b(1) p:c(1) p:d(1)"
      };

      // Act...
      var testCases = new TestCaseLoader().CasesFromConfigs(testConfig, configLibrary);

      // Assert
      var testCaseString = testCases.Select(tc => new TestCaseLoader().CaseToString(tc)).ToList();
      CollectionAssert.AreEquivalent(expectedCases, testCaseString);
    }


    [Test]
    public void ResolveSetup_3DefaultSetups2LevelsDeep()
    {
      // Arrange...
      var configLibrary = new List<Config>(){
        Config.New("D[1]")
          .AddProperty("a", "1"),
        Config.New("D[2]")
          .AddProperty("a", "2"),
        Config.New("D[3]")
          .AddProperty("IMPORT.DefaultSetup", "D[2]")
          .AddProperty("a", "3")
      };

      var testConfig = Config.New("TestConfig")
        .AddProperty("IMPORT.DefaultSetup", "D[1]")
        .AddProperty("IMPORT.DefaultSetup2", "D[3]");

      var expectedCases = new[]{
        "p:a(3)"
      };

      // Act...
      var testCases = new TestCaseLoader().CasesFromConfigs(testConfig, configLibrary);

      // Assert
      var testCaseString = testCases.Select(tc => new TestCaseLoader().CaseToString(tc)).ToList();
      CollectionAssert.AreEquivalent(expectedCases, testCaseString);
    }


    [Test]
    public void ResolveSetup_3DefaultSetups2LevelsDeepWithExpansions()
    {
      // Arrange...
      var configLibrary = new List<Config>(){
        Config.New("D[1]")
          .AddProperty("a", "1"),
        Config.New("D[2]")
          .AddProperty("a", new[] { "2.1", "2.2" }),
        Config.New("D[3]")
          .AddProperty("IMPORT.DefaultSetup", "D[2]")
      };

      var testConfig = Config.New("TestConfig")
        .AddProperty("IMPORT.DefaultSetup", "D[1]")
        .AddProperty("IMPORT.DefaultSetup2", new[] { "D[3]" });

      var expectedCases = new[]{
        "p:a(2.1)",
        "p:a(2.2)",
      };

      // Act...
      var testCases = new TestCaseLoader().CasesFromConfigs(testConfig, configLibrary);

      // Assert
      var testCaseString = testCases.Select(tc => new TestCaseLoader().CaseToString(tc)).ToList();
      CollectionAssert.AreEquivalent(expectedCases, testCaseString);
    }

    [Test]
    public void ResolveSetup_3DefaultSetups2LevelsDeepWithExpansionsReversePriority()
    {
      // Arrange...
      var configLibrary = new List<Config>(){
        Config.New("D[1]")
          .AddProperty("a", "1"),
        Config.New("D[2]")
          .AddProperty("a", new[] { "2.1", "2.2" }),
        Config.New("D[3]")
          .AddProperty("IMPORT.DefaultSetup", "D[2]")
      };

      var testConfig = Config.New("TestConfig")
        .AddProperty("IMPORT.DefaultSetup", "D[3]")
        .AddProperty("IMPORT.DefaultSetup2", new[] { "D[1]" });

      var expectedCases = new[]{
        "p:a(1)",
      };

      // Act...
      var testCases = new TestCaseLoader().CasesFromConfigs(testConfig, configLibrary);

      // Assert
      var testCaseString = testCases.Select(tc => new TestCaseLoader().CaseToString(tc)).ToList();
      CollectionAssert.AreEquivalent(expectedCases, testCaseString);
    }

  }
}