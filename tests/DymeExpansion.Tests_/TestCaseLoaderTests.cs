using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;
using DymeExpansion.Core.Models;
using DymeExpansion.Core.Services;
using DymeExpansion.Core.Enums;

namespace Tests
{

  [TestFixture]
  public class TestCaseLoaderTests
  {

    private ValueNode GetMockNodeTreeUsingFluentTreeBuilder() {

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

    public List<Config> GetMockConfigsLibrary() { 
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

    private IEnumerable<Case> ResultTestCasesForAllMocks(){
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

    private string CaseToString(Case caseX, string separator = ",")
    {
      return caseX.Properties.OrderBy(i => i.Name).Select(s => s.Value).Aggregate((a,b) => $"{a}{separator}{b}");
    }

    [Test]
    public void HashExpression_GivenMockSetup1() { 
      var mockSetup = GetMockNodeTreeUsingFluentTreeBuilder();
      var actualValue = mockSetup.HashAsExpression();
      var expectedValue = "((((Platform:Ios) AND (((Username:Tom OR Username:Bob) AND (SeleniumHub:Saucelabs)))) OR ((Platform:Android) AND (((Username:Jake) AND (SeleniumHub:LocalFarm))))) AND (((SiteName:google) AND (Sid:23)) OR ((SiteName:facebook OR SiteName:twitter) AND (Sid:41 OR Sid:42 OR Sid:43))))";
      Assert.AreEqual(expectedValue, actualValue);
    }

    [Test]
    public void CalculateTestCaseCount_GivenMockSetup1() { 
      var mockSetup = GetMockNodeTreeUsingFluentTreeBuilder();
      var testCaseCount = mockSetup.CalculateTestCaseCount();
      Assert.AreEqual(21, testCaseCount);
    }

    [Test]
    public void GetCaseSet_GivenMockSetup1() { 
      var mockNodeTree = GetMockNodeTreeUsingFluentTreeBuilder();
      var expectedTestCases = ResultTestCasesForAllMocks().Select(tc => CaseToString(tc)).OrderBy(s=>s).ToList();
      var actualTestCases = TestCaseLoader.CasesFromNodeTree(mockNodeTree).Select(tc => CaseToString(tc)).OrderBy(s=>s).ToList();
      //File.WriteAllText(Directory.GetCurrentDirectory() + "\\ExpectedCases.xls", expectedTestCases.Aggregate((a,b) => $"{a}\n{b}"));
      //File.WriteAllText(Directory.GetCurrentDirectory() + "\\ActualCases.xls", actualTestCases.Aggregate((a,b) => $"{a}\n{b}"));
      CollectionAssert.AreEquivalent(expectedTestCases, actualTestCases);
    }

    [Test]
    public void GetCaseSet_GivenConfigs() {
      var configLibrary = GetMockConfigsLibrary();
      var mainConfig = configLibrary.Single(c => c.Name == "TestConfig");
      var compiledNodeTree = TestCaseLoader.NodeTreeFromConfigs(mainConfig, configLibrary);
      var expectedTestCases = ResultTestCasesForAllMocks().Select(tc => CaseToString(tc)).OrderBy(s=>s).ToList();
      var actualTestCases = TestCaseLoader.CasesFromNodeTree(compiledNodeTree).Select(tc => CaseToString(tc)).OrderBy(s=>s).ToList();
      //File.WriteAllText(Directory.GetCurrentDirectory() + "\\ExpectedCases.xls", expectedTestCases.Aggregate((a,b) => $"{a}\n{b}"));
      //File.WriteAllText(Directory.GetCurrentDirectory() + "\\ActualCases.xls", actualTestCases.Aggregate((a,b) => $"{a}\n{b}"));
      CollectionAssert.AreEquivalent(expectedTestCases, actualTestCases);
    }


    public void GetCaseSet_GivenSimpleConfig() {
      var y = Config.New("TestConfig")
        .AddProperty("IMPORT.Devices", new[]{"IPhoneX-Setup", "SamusungS7-Setup" });  
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
      var nodeTree = TestCaseLoader.NodeTreeFromConfigs(config, configLibrary);
      var testCases = TestCaseLoader.CasesFromNodeTree(nodeTree);      
      
      // Assert...
      var testCaseString = testCases.Select(tc => TestCaseLoader.CaseToString(tc)).ToList();
      Assert.AreEqual(expectedCases, testCaseString);
    }

    [Test]
    public void ExpandSetup_GivenPermutationSet_Expect2Permutations()
    {

      // Arrange...
      var configLibrary = new List<Config>();
      var config = Config.New("TestConfig")
        .AddProperty("a", new []{ "1", "2" })  
        .AddProperty("b", "1")
        .AddProperty("c", "1");
      var expectedCases = new[]{
        "p:a(1) p:b(1) p:c(1)",
        "p:a(2) p:b(1) p:c(1)"
      };

      // Act...
      var testCases = TestCaseLoader.CasesFromConfigs(config, configLibrary);

      // Assert...
      var testCaseString = testCases.Select(tc => TestCaseLoader.CaseToString(tc)).ToList();
      Assert.AreEqual(expectedCases, testCaseString);
    }

    [Test]
    public void ExpandSetup_GivenPermutationSet_Expect4Permutations()
    {

      // Arrange...
      var configLibrary = new List<Config>();
      var config = Config.New("TestConfig")
        .AddProperty("a", new []{ "1", "2" })  
        .AddProperty("b", new []{ "1", "2" })
        .AddProperty("c", "1");

      var expectedCases = new[]{
        "p:a(1) p:b(1) p:c(1)",
        "p:a(1) p:b(2) p:c(1)",
        "p:a(2) p:b(1) p:c(1)",
        "p:a(2) p:b(2) p:c(1)"
      };

      // Act...
      var testCases = TestCaseLoader.CasesFromConfigs(config, configLibrary);

      // Assert
      var testCaseString = testCases.Select(tc => TestCaseLoader.CaseToString(tc)).ToList();
      Assert.AreEqual(4, testCases.Count());
      CollectionAssert.AreEquivalent(expectedCases, testCaseString);
    }

    [Test]
    public void ExpandSetup_GivenPermutationSet_Expect2CorrelatedPermutations()
    {
      // Arrange...
      var configLibrary = new List<Config>();
      var config = Config.New("TestConfig")
        .AddProperty("a", new []{ "1", "2" }, "1")  
        .AddProperty("b", new []{ "1", "2" }, "1")
        .AddProperty("c", "1", "2");

      var expectedCases = new[]{
        "p:a(1) p:b(1) p:c(1)",
        "p:a(2) p:b(2) p:c(1)"
      };

      // Act...
      var testCases = TestCaseLoader.CasesFromConfigs(config, configLibrary);
      
      // Assert
      var testCaseString = testCases.Select(tc => TestCaseLoader.CaseToString(tc)).ToList();
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
      var testCases = TestCaseLoader.CasesFromConfigs(testConfig, configLibrary);
      
      // Assert
      var testCaseString = testCases.Select(tc => TestCaseLoader.CaseToString(tc)).ToList();
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
      var testCases = TestCaseLoader.CasesFromConfigs(testConfig, configLibrary);
      
      // Assert
      var testCaseString = testCases.Select(tc => TestCaseLoader.CaseToString(tc)).ToList();
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
        .AddProperty("IMPORT.DefaultSetup", new[]{"D[1]", "D[2]" })
        .AddProperty("b", "1");

      var expectedCases = new[]{
        "p:a(1) p:b(1) p:c(1)",
        "p:b(1) p:m(1) p:n(1)"
      };

      // Act...
      var testCases = TestCaseLoader.CasesFromConfigs(testConfig, configLibrary);

      // Assert
      var testCaseString = testCases.Select(tc => TestCaseLoader.CaseToString(tc)).ToList();
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
      var testCases = TestCaseLoader.CasesFromConfigs(testConfig, configLibrary);

      // Assert
      var testCaseString = testCases.Select(tc => TestCaseLoader.CaseToString(tc)).ToList();
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
        .AddProperty("IMPORT.DefaultSetup", new[]{"D[1]", "D[2]" })
        .AddProperty("c", new[]{ "T", "F" });

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
      var testCases = TestCaseLoader.CasesFromConfigs(testConfig, configLibrary);

      // Assert
      var testCaseString = testCases.Select(tc => TestCaseLoader.CaseToString(tc)).ToList();
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
        .AddProperty("IMPORT.DefaultSetup", new[]{"D[1]" })
        .AddProperty("IMPORT.DefaultSetup2", new[]{"D[2]" })
        .AddProperty("d", "1");

      var expectedCases = new[]{
        "p:a(2) p:b(1) p:c(1) p:d(1)"
      };

      // Act...
      var testCases = TestCaseLoader.CasesFromConfigs(testConfig, configLibrary);

      // Assert
      var testCaseString = testCases.Select(tc => TestCaseLoader.CaseToString(tc)).ToList();
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
        .AddProperty("IMPORT.DefaultSetup", new[]{"D[1]" })
        .AddProperty("IMPORT.DefaultSetup2", new[]{"D[2]" })
        .AddProperty("d", "1");

      var expectedCases = new[]{
        "p:a(4) p:b(1) p:c(1) p:d(1)",
        "p:a(5) p:b(1) p:c(1) p:d(1)"
      };

      // Act...
      var testCases = TestCaseLoader.CasesFromConfigs(testConfig, configLibrary);

      // Assert
      var testCaseString = testCases.Select(tc => TestCaseLoader.CaseToString(tc)).ToList();
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
      var testCases = TestCaseLoader.CasesFromConfigs(testConfig, configLibrary);

      // Assert
      var testCaseString = testCases.Select(tc => TestCaseLoader.CaseToString(tc)).ToList();
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
        .AddProperty("IMPORT.DefaultSetup", "D[1]" )
        .AddProperty("IMPORT.DefaultSetup2", new[]{ "D[3]" });

      var expectedCases = new[]{
        "p:a(2.1)",
        "p:a(2.2)",
      };

      // Act...
      var testCases = TestCaseLoader.CasesFromConfigs(testConfig, configLibrary);

      // Assert
      var testCaseString = testCases.Select(tc => TestCaseLoader.CaseToString(tc)).ToList();
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
        .AddProperty("IMPORT.DefaultSetup", "D[3]" )
        .AddProperty("IMPORT.DefaultSetup2", new[]{ "D[1]" });

      var expectedCases = new[]{
        "p:a(1)",
      };

      // Act...
      var testCases = TestCaseLoader.CasesFromConfigs(testConfig, configLibrary);

      // Assert
      var testCaseString = testCases.Select(tc => TestCaseLoader.CaseToString(tc)).ToList();
      CollectionAssert.AreEquivalent(expectedCases, testCaseString);
    }

  }
}

// PropertyNode parent will always be a ReferenceNode
// PropertyNode child will always be a LeafNode
// ReferenceNode parent will always be a ReferenceNode
// ReferenceNode child can be a PropertyNode or LeafNode
// LeafNode parent can be a ReferenceNode or PropertyNode
// LeafNode has no children

// You can add a ValueNode to a PropertyNode
// You can add a PropertyNode to a ValueNode
// The value of a PropertyNode represents a property name 
// The value of a ValueNode represents a property value

