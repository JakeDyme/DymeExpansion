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

    private IEnumerable<DymeCase> ResultTestCasesForAllMocks()
    {
      return new List<DymeCase>{
        new DymeCase(){
          Properties = new List<DymeCaseProperty>()
          {
            new DymeCaseProperty("Username","Tom"),
            new DymeCaseProperty("SeleniumHub","Saucelabs"),
            new DymeCaseProperty("Platform","Ios"),
            new DymeCaseProperty("SiteName","google"),
            new DymeCaseProperty("Sid","23")
          }
        },
        new DymeCase(){
          Properties = new List<DymeCaseProperty>()
          {
            new DymeCaseProperty("Username","Tom"),
            new DymeCaseProperty("SeleniumHub","Saucelabs"),
            new DymeCaseProperty("Platform","Ios"),
            new DymeCaseProperty("SiteName","facebook"),
            new DymeCaseProperty("Sid","41"),
          },
        },
        new DymeCase(){
          Properties = new List<DymeCaseProperty>()
          {
            new DymeCaseProperty("Username","Tom"),
          new DymeCaseProperty("SeleniumHub","Saucelabs"),
          new DymeCaseProperty("Platform","Ios"),
          new DymeCaseProperty("SiteName","facebook"),
          new DymeCaseProperty("Sid","42"),
        },
        },
        new DymeCase(){
          Properties = new List<DymeCaseProperty>()
          {
            new DymeCaseProperty("Username","Tom"),
          new DymeCaseProperty("SeleniumHub","Saucelabs"),
          new DymeCaseProperty("Platform","Ios"),
          new DymeCaseProperty("SiteName","facebook"),
          new DymeCaseProperty("Sid","43"),
          },
        },
        new DymeCase(){
          Properties = new List<DymeCaseProperty>()
          {
            new DymeCaseProperty("Username","Tom"),
            new DymeCaseProperty("SeleniumHub","Saucelabs"),
            new DymeCaseProperty("Platform","Ios"),
            new DymeCaseProperty("SiteName","twitter"),
            new DymeCaseProperty("Sid","41"),
          },
        },
        new DymeCase(){
          Properties = new List<DymeCaseProperty>()
          {
            new DymeCaseProperty("Username","Tom"),
          new DymeCaseProperty("SeleniumHub","Saucelabs"),
          new DymeCaseProperty("Platform","Ios"),
          new DymeCaseProperty("SiteName","twitter"),
          new DymeCaseProperty("Sid","42"),
        },},
        new DymeCase(){
          Properties = new List<DymeCaseProperty>()
          {
            new DymeCaseProperty("Username","Tom"),
          new DymeCaseProperty("SeleniumHub","Saucelabs"),
          new DymeCaseProperty("Platform","Ios"),
          new DymeCaseProperty("SiteName","twitter"),
          new DymeCaseProperty("Sid","43"),
        },},

        new DymeCase(){
          Properties = new List<DymeCaseProperty>()
          {
            new DymeCaseProperty("Username","Bob"),
          new DymeCaseProperty("SeleniumHub","Saucelabs"),
          new DymeCaseProperty("Platform","Ios"),
          new DymeCaseProperty("SiteName","google"),
          new DymeCaseProperty("Sid","23"),
        },  },
        new DymeCase(){
          Properties = new List<DymeCaseProperty>()
          {
            new DymeCaseProperty("Username","Bob"),
          new DymeCaseProperty("SeleniumHub","Saucelabs"),
          new DymeCaseProperty("Platform","Ios"),
          new DymeCaseProperty("SiteName","facebook"),
          new DymeCaseProperty("Sid","41"),
        },},
        new DymeCase(){
          Properties = new List<DymeCaseProperty>()
          {
            new DymeCaseProperty("Username","Bob"),
          new DymeCaseProperty("SeleniumHub","Saucelabs"),
          new DymeCaseProperty("Platform","Ios"),
          new DymeCaseProperty("SiteName","facebook"),
          new DymeCaseProperty("Sid","42"),
        },},
        new DymeCase(){
          Properties = new List<DymeCaseProperty>()
          {
            new DymeCaseProperty("Username","Bob"),
          new DymeCaseProperty("SeleniumHub","Saucelabs"),
          new DymeCaseProperty("Platform","Ios"),
          new DymeCaseProperty("SiteName","facebook"),
          new DymeCaseProperty("Sid","43"),
        }, },
        new DymeCase(){
          Properties = new List<DymeCaseProperty>()
          {
            new DymeCaseProperty("Username","Bob"),
          new DymeCaseProperty("SeleniumHub","Saucelabs"),
          new DymeCaseProperty("Platform","Ios"),
          new DymeCaseProperty("SiteName","twitter"),
          new DymeCaseProperty("Sid","41"),
        },},
        new DymeCase(){
          Properties = new List<DymeCaseProperty>()
          {
            new DymeCaseProperty("Username","Bob"),
          new DymeCaseProperty("SeleniumHub","Saucelabs"),
          new DymeCaseProperty("Platform","Ios"),
          new DymeCaseProperty("SiteName","twitter"),
          new DymeCaseProperty("Sid","42"),
        },},
        new DymeCase(){
          Properties = new List<DymeCaseProperty>()
          {
            new DymeCaseProperty("Username","Bob"),
          new DymeCaseProperty("SeleniumHub","Saucelabs"),
          new DymeCaseProperty("Platform","Ios"),
          new DymeCaseProperty("SiteName","twitter"),
          new DymeCaseProperty("Sid","43"),
        },},

        new DymeCase(){
          Properties = new List<DymeCaseProperty>()
          {
            new DymeCaseProperty("Username","Jake"),
          new DymeCaseProperty("SeleniumHub","LocalFarm"),
          new DymeCaseProperty("Platform","Android"),
          new DymeCaseProperty("SiteName","google"),
          new DymeCaseProperty("Sid","23"),
        }, },
        new DymeCase(){
          Properties = new List<DymeCaseProperty>()
          {
            new DymeCaseProperty("Username","Jake"),
          new DymeCaseProperty("SeleniumHub","LocalFarm"),
          new DymeCaseProperty("Platform","Android"),
          new DymeCaseProperty("SiteName","facebook"),
          new DymeCaseProperty("Sid","41"),
        },},
        new DymeCase(){
          Properties = new List<DymeCaseProperty>()
          {
            new DymeCaseProperty("Username","Jake"),
          new DymeCaseProperty("SeleniumHub","LocalFarm"),
          new DymeCaseProperty("Platform","Android"),
          new DymeCaseProperty("SiteName","facebook"),
          new DymeCaseProperty("Sid","42"),
        },},
        new DymeCase(){
          Properties = new List<DymeCaseProperty>()
          {
            new DymeCaseProperty("Username","Jake"),
          new DymeCaseProperty("SeleniumHub","LocalFarm"),
          new DymeCaseProperty("Platform","Android"),
          new DymeCaseProperty("SiteName","facebook"),
          new DymeCaseProperty("Sid","43"),
        },   },
        new DymeCase(){
          Properties = new List<DymeCaseProperty>()
          {
            new DymeCaseProperty("Username","Jake"),
          new DymeCaseProperty("SeleniumHub","LocalFarm"),
          new DymeCaseProperty("Platform","Android"),
          new DymeCaseProperty("SiteName","twitter"),
          new DymeCaseProperty("Sid","41"),
        },},
        new DymeCase(){
          Properties = new List<DymeCaseProperty>()
          {
            new DymeCaseProperty("Username","Jake"),
          new DymeCaseProperty("SeleniumHub","LocalFarm"),
          new DymeCaseProperty("Platform","Android"),
          new DymeCaseProperty("SiteName","twitter"),
          new DymeCaseProperty("Sid","42"),
        },},
        new DymeCase(){
          Properties = new List<DymeCaseProperty>()
          {
            new DymeCaseProperty("Username","Jake"),
          new DymeCaseProperty("SeleniumHub","LocalFarm"),
          new DymeCaseProperty("Platform","Android"),
          new DymeCaseProperty("SiteName","twitter"),
          new DymeCaseProperty("Sid","43"),
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
      var configLibrary = new List<DymeConfig>();
      var config = DymeConfig.New("TestConfig")
        .AddProperty("a", "1")
        .AddProperty("b", "1")
        .AddProperty("c", "1");
      var expectedCases = new[]{
        "p:a(1) p:b(1) p:c(1)"
      };

      // Act...
      var nodeTree = new DymeCaseLoader().NodeTreeFromConfigs(config, configLibrary);
      var testCases = new DymeCaseLoader().CasesFromNodeTree(nodeTree);

      // Assert...
      var testCaseString = testCases.Select(tc => new DymeCaseLoader().CaseToString(tc)).ToList();
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
      var actualTestCases = new DymeCaseLoader().CasesFromNodeTree(mockNodeTree).Select(tc => CaseToString(tc)).OrderBy(s => s).ToList();
      //File.WriteAllText(Directory.GetCurrentDirectory() + "\\ExpectedCases.xls", expectedTestCases.Aggregate((a,b) => $"{a}\n{b}"));
      //File.WriteAllText(Directory.GetCurrentDirectory() + "\\ActualCases.xls", actualTestCases.Aggregate((a,b) => $"{a}\n{b}"));
      CollectionAssert.AreEquivalent(expectedTestCases, actualTestCases);
    }

    [Test]
    public void GetCaseSet_GivenConfigs()
    {
      var configLibrary = GetMockConfigsLibrary();
      var mainConfig = configLibrary.Single(c => c.Name == "TestConfig");
      var compiledNodeTree = new DymeCaseLoader().NodeTreeFromConfigs(mainConfig, configLibrary);
      var expectedTestCases = ResultTestCasesForAllMocks().Select(tc => CaseToString(tc)).OrderBy(s => s).ToList();
      var actualTestCases = new DymeCaseLoader().CasesFromNodeTree(compiledNodeTree).Select(tc => CaseToString(tc)).OrderBy(s => s).ToList();
      //File.WriteAllText(Directory.GetCurrentDirectory() + "\\ExpectedCases.xls", expectedTestCases.Aggregate((a,b) => $"{a}\n{b}"));
      //File.WriteAllText(Directory.GetCurrentDirectory() + "\\ActualCases.xls", actualTestCases.Aggregate((a,b) => $"{a}\n{b}"));
      CollectionAssert.AreEquivalent(expectedTestCases, actualTestCases);
    }

  }
}
