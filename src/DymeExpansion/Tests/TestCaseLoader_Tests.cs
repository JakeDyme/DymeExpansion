using DymeExpansion.Core.Enums;
using DymeExpansion.Core.Models;
using DymeExpansion.Core.Services;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace DymeExpansion.Core.Tests
{

  internal class TestCaseLoader_Tests
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

    private IEnumerable<Case> ResultTestCasesForAllMocks()
    {
      return new List<Case>{
        new Case(){
          Properties = new List<CaseProperty>()
          {
            new CaseProperty("Username","Tom"),
            new CaseProperty("SeleniumHub","Saucelabs"),
            new CaseProperty("Platform","Ios"),
            new CaseProperty("SiteName","google"),
            new CaseProperty("Sid","23")
          }
        },
        new Case(){
          Properties = new List<CaseProperty>()
          {
            new CaseProperty("Username","Tom"),
            new CaseProperty("SeleniumHub","Saucelabs"),
            new CaseProperty("Platform","Ios"),
            new CaseProperty("SiteName","facebook"),
            new CaseProperty("Sid","41"),
          },
        },
        new Case(){
          Properties = new List<CaseProperty>()
          {
            new CaseProperty("Username","Tom"),
          new CaseProperty("SeleniumHub","Saucelabs"),
          new CaseProperty("Platform","Ios"),
          new CaseProperty("SiteName","facebook"),
          new CaseProperty("Sid","42"),
        },
        },
        new Case(){
          Properties = new List<CaseProperty>()
          {
            new CaseProperty("Username","Tom"),
          new CaseProperty("SeleniumHub","Saucelabs"),
          new CaseProperty("Platform","Ios"),
          new CaseProperty("SiteName","facebook"),
          new CaseProperty("Sid","43"),
          },
        },
        new Case(){
          Properties = new List<CaseProperty>()
          {
            new CaseProperty("Username","Tom"),
            new CaseProperty("SeleniumHub","Saucelabs"),
            new CaseProperty("Platform","Ios"),
            new CaseProperty("SiteName","twitter"),
            new CaseProperty("Sid","41"),
          },
        },
        new Case(){
          Properties = new List<CaseProperty>()
          {
            new CaseProperty("Username","Tom"),
          new CaseProperty("SeleniumHub","Saucelabs"),
          new CaseProperty("Platform","Ios"),
          new CaseProperty("SiteName","twitter"),
          new CaseProperty("Sid","42"),
        },},
        new Case(){
          Properties = new List<CaseProperty>()
          {
            new CaseProperty("Username","Tom"),
          new CaseProperty("SeleniumHub","Saucelabs"),
          new CaseProperty("Platform","Ios"),
          new CaseProperty("SiteName","twitter"),
          new CaseProperty("Sid","43"),
        },},

        new Case(){
          Properties = new List<CaseProperty>()
          {
            new CaseProperty("Username","Bob"),
          new CaseProperty("SeleniumHub","Saucelabs"),
          new CaseProperty("Platform","Ios"),
          new CaseProperty("SiteName","google"),
          new CaseProperty("Sid","23"),
        },  },
        new Case(){
          Properties = new List<CaseProperty>()
          {
            new CaseProperty("Username","Bob"),
          new CaseProperty("SeleniumHub","Saucelabs"),
          new CaseProperty("Platform","Ios"),
          new CaseProperty("SiteName","facebook"),
          new CaseProperty("Sid","41"),
        },},
        new Case(){
          Properties = new List<CaseProperty>()
          {
            new CaseProperty("Username","Bob"),
          new CaseProperty("SeleniumHub","Saucelabs"),
          new CaseProperty("Platform","Ios"),
          new CaseProperty("SiteName","facebook"),
          new CaseProperty("Sid","42"),
        },},
        new Case(){
          Properties = new List<CaseProperty>()
          {
            new CaseProperty("Username","Bob"),
          new CaseProperty("SeleniumHub","Saucelabs"),
          new CaseProperty("Platform","Ios"),
          new CaseProperty("SiteName","facebook"),
          new CaseProperty("Sid","43"),
        }, },
        new Case(){
          Properties = new List<CaseProperty>()
          {
            new CaseProperty("Username","Bob"),
          new CaseProperty("SeleniumHub","Saucelabs"),
          new CaseProperty("Platform","Ios"),
          new CaseProperty("SiteName","twitter"),
          new CaseProperty("Sid","41"),
        },},
        new Case(){
          Properties = new List<CaseProperty>()
          {
            new CaseProperty("Username","Bob"),
          new CaseProperty("SeleniumHub","Saucelabs"),
          new CaseProperty("Platform","Ios"),
          new CaseProperty("SiteName","twitter"),
          new CaseProperty("Sid","42"),
        },},
        new Case(){
          Properties = new List<CaseProperty>()
          {
            new CaseProperty("Username","Bob"),
          new CaseProperty("SeleniumHub","Saucelabs"),
          new CaseProperty("Platform","Ios"),
          new CaseProperty("SiteName","twitter"),
          new CaseProperty("Sid","43"),
        },},

        new Case(){
          Properties = new List<CaseProperty>()
          {
            new CaseProperty("Username","Jake"),
          new CaseProperty("SeleniumHub","LocalFarm"),
          new CaseProperty("Platform","Android"),
          new CaseProperty("SiteName","google"),
          new CaseProperty("Sid","23"),
        }, },
        new Case(){
          Properties = new List<CaseProperty>()
          {
            new CaseProperty("Username","Jake"),
          new CaseProperty("SeleniumHub","LocalFarm"),
          new CaseProperty("Platform","Android"),
          new CaseProperty("SiteName","facebook"),
          new CaseProperty("Sid","41"),
        },},
        new Case(){
          Properties = new List<CaseProperty>()
          {
            new CaseProperty("Username","Jake"),
          new CaseProperty("SeleniumHub","LocalFarm"),
          new CaseProperty("Platform","Android"),
          new CaseProperty("SiteName","facebook"),
          new CaseProperty("Sid","42"),
        },},
        new Case(){
          Properties = new List<CaseProperty>()
          {
            new CaseProperty("Username","Jake"),
          new CaseProperty("SeleniumHub","LocalFarm"),
          new CaseProperty("Platform","Android"),
          new CaseProperty("SiteName","facebook"),
          new CaseProperty("Sid","43"),
        },   },
        new Case(){
          Properties = new List<CaseProperty>()
          {
            new CaseProperty("Username","Jake"),
          new CaseProperty("SeleniumHub","LocalFarm"),
          new CaseProperty("Platform","Android"),
          new CaseProperty("SiteName","twitter"),
          new CaseProperty("Sid","41"),
        },},
        new Case(){
          Properties = new List<CaseProperty>()
          {
            new CaseProperty("Username","Jake"),
          new CaseProperty("SeleniumHub","LocalFarm"),
          new CaseProperty("Platform","Android"),
          new CaseProperty("SiteName","twitter"),
          new CaseProperty("Sid","42"),
        },},
        new Case(){
          Properties = new List<CaseProperty>()
          {
            new CaseProperty("Username","Jake"),
          new CaseProperty("SeleniumHub","LocalFarm"),
          new CaseProperty("Platform","Android"),
          new CaseProperty("SiteName","twitter"),
          new CaseProperty("Sid","43"),
        }},
      };
    }

    private ValueNode GetMockNodeTreeUsingFluentTreeBuilder()
    {

      var mainSetup = new ValueNode("SetupX", NodeTypeEnum.ValueNode);
      mainSetup.WithProperty("IMPORT.Devices")
        .WithImported("IPhoneX-Setup")
          .WithProperty("Platform").WithValue("Ios")
          .AndProperty("IMPORT.Accounts")
            .WithImported("TeamAccount-Setup")
              .WithProperty("Username").WithValue("Tom").AndValue("Bob")
              .AndProperty("SeleniumHub").WithValue("Saucelabs");
      mainSetup.WithProperty("IMPORT.Devices")
        .WithImported("SamusungS7-Setup")
          .WithProperty("Platform").WithValue("Android")
          .AndProperty("IMPORT.Accounts")
            .WithValue("JakesAccount-Setup")
              .WithProperty("Username").WithValue("Jake")
              .AndProperty("SeleniumHub").WithValue("LocalFarm");
      mainSetup.WithProperty("IMPORT.SiteInfo")
        .WithImported("SearchSites-Setup")
          .WithProperty("SiteName").WithValue("google")
          .AndProperty("Sid").WithValue("23");
      mainSetup.WithProperty("IMPORT.SiteInfo")
        .WithImported("SocialSites-Setup")
          .WithProperty("SiteName").WithValue("facebook").AndValue("twitter")
          .AndProperty("Sid").WithValue("41").AndValue("42").AndValue("43");
      return mainSetup;
    }

    [Test]
    public void ExpandSetup_GivenPermutationSet_Expect1Permutation()
    {
      // Arrange...
      var configLibrary = new List<Config>();
      var config = Config.New("TestConfig")
        .AddProperty("a", "1")
        .AddProperty("b", "1")
        .AddProperty("c", "1");
      var expectedCases = new[]{
        "p:a(1) p:b(1) p:c(1)"
      };

      // Act...
      var nodeTree = new TestCaseLoader().NodeTreeFromConfigs(config, configLibrary);
      var testCases = new TestCaseLoader().CasesFromNodeTree(nodeTree);

      // Assert...
      var testCaseString = testCases.Select(tc => new TestCaseLoader().CaseToString(tc)).ToList();
      Assert.AreEqual(expectedCases, testCaseString);
    }


    [Test]
    public void HashExpression_GivenMockSetup1()
    {
      var mockSetup = GetMockNodeTreeUsingFluentTreeBuilder();
      var actualValue = mockSetup.HashAsExpression();
      var expectedValue = "((((Platform:Ios) AND (((Username:Tom OR Username:Bob) AND (SeleniumHub:Saucelabs)))) OR ((Platform:Android) AND (((Username:Jake) AND (SeleniumHub:LocalFarm))))) AND (((SiteName:google) AND (Sid:23)) OR ((SiteName:facebook OR SiteName:twitter) AND (Sid:41 OR Sid:42 OR Sid:43))))";
      Assert.AreEqual(expectedValue, actualValue);
    }


    [Test]
    public void CalculateTestCaseCount_GivenMockSetup1()
    {
      var mockSetup = GetMockNodeTreeUsingFluentTreeBuilder();
      var testCaseCount = mockSetup.CalculateTestCaseCount();
      Assert.AreEqual(21, testCaseCount);
    }

    [Test]
    public void GetCaseSet_GivenMockSetup1()
    {
      var mockNodeTree = GetMockNodeTreeUsingFluentTreeBuilder();
      var expectedTestCases = ResultTestCasesForAllMocks().Select(tc => CaseToString(tc)).OrderBy(s => s).ToList();
      var actualTestCases = new TestCaseLoader().CasesFromNodeTree(mockNodeTree).Select(tc => CaseToString(tc)).OrderBy(s => s).ToList();
      //File.WriteAllText(Directory.GetCurrentDirectory() + "\\ExpectedCases.xls", expectedTestCases.Aggregate((a,b) => $"{a}\n{b}"));
      //File.WriteAllText(Directory.GetCurrentDirectory() + "\\ActualCases.xls", actualTestCases.Aggregate((a,b) => $"{a}\n{b}"));
      CollectionAssert.AreEquivalent(expectedTestCases, actualTestCases);
    }

    [Test]
    public void GetCaseSet_GivenConfigs()
    {
      var configLibrary = GetMockConfigsLibrary();
      var mainConfig = configLibrary.Single(c => c.Name == "TestConfig");
      var compiledNodeTree = new TestCaseLoader().NodeTreeFromConfigs(mainConfig, configLibrary);
      var expectedTestCases = ResultTestCasesForAllMocks().Select(tc => CaseToString(tc)).OrderBy(s => s).ToList();
      var actualTestCases = new TestCaseLoader().CasesFromNodeTree(compiledNodeTree).Select(tc => CaseToString(tc)).OrderBy(s => s).ToList();
      //File.WriteAllText(Directory.GetCurrentDirectory() + "\\ExpectedCases.xls", expectedTestCases.Aggregate((a,b) => $"{a}\n{b}"));
      //File.WriteAllText(Directory.GetCurrentDirectory() + "\\ActualCases.xls", actualTestCases.Aggregate((a,b) => $"{a}\n{b}"));
      CollectionAssert.AreEquivalent(expectedTestCases, actualTestCases);
    }

  }
}
