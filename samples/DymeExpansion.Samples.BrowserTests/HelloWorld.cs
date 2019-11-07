using DymeExpansion.Core.Models;
using DymeExpansion.Core.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace DymeExpansion.Samples.BrowserTests
{
  public class HelloWorld
  {
    [Test]
    public void HelloWorld_GivenSomeConfig_ExpectSomeBasicExpansion()
    {
      var actualGreetings = new List<string>();

      // First we'll create some config using the "Config" class...
      var testConfig = Config.New("HelloWorld")
        .AddProperty("Name", "Ali" )
        .AddProperty("Greeting", new[] { "Hello World", "Bonjour le monde" });

      // Then we'll automatically generate our test cases using the "TestCaseLoader" class...
      var testCases = TestCaseLoader.CasesFromConfig(testConfig);

      // Now we can extract the data from our test cases and use it to make friends...
      foreach (var testCase in testCases)
      {
        var greeting = testCase["Name"] + " says " + testCase["Greeting"];
        Debug.WriteLine(greeting);
        actualGreetings.Add(greeting);
      }

      // Just to make sure that we're saying the right stuff...
      var expectedGreetings = new[]
      {
        "Ali says Hello World",
        "Ali says Bonjour le monde",
      };
      
      // Lets double check the data that we generated from our test cases...
      CollectionAssert.AreEquivalent(expectedGreetings, actualGreetings);
    }


    /// <summary>
    /// In this test we'll separate whats being said, from who is saying it.
    /// Then we'll inheritence to bind the data back together.
    /// </summary>
    [Test]
    public void HelloWorld_GivenSomeConfig_ExpectSomeBasicInheritence()
    {
      var actualGreetings = new List<string>();

      // In this example we want to keep our data separated into neat bundles of related information...
      // We'll put all person related info into one config...
      var people = Config.New("PeopleConfig")
        .AddProperty("Name", new []{"Ali", "Brian" } );

      // ...and all greeting related info into another config.
      // To bind the data back together, we'll import the people config into the greeting config...
      var greetings = Config.New("GreetingConfig")
        .AddProperty("IMPORT", "PeopleConfig")
        .AddProperty("Greeting", new[] { "Hello World", "Bonjour le monde" });

      // We'll create a library of configs...
      var configLibrary = new[] { people, greetings };

      //...and pass it in along with the config that we want to interpret...
      var testCases = TestCaseLoader.CasesFromConfig(greetings, configLibrary); //...(the passed in config library is used to resolve any "IMPORT" references)

      // Extract the data from our shiny new test cases, and use it to start a party...
      foreach (var testCase in testCases)
      {
        var greeting = testCase["Name"] + " says " + testCase["Greeting"];
        Debug.WriteLine(greeting);
        actualGreetings.Add(greeting);
      }

      // Just to make sure that we're saying the right stuff...
      var expectedGreetings = new[]
      {
        "Ali says Hello World",
        "Ali says Bonjour le monde",
        "Brian says Hello World",
        "Brian says Bonjour le monde",
      };

      // Lets double check the data that we generated from our test cases...
      CollectionAssert.AreEquivalent(expectedGreetings, actualGreetings);
    }

    [Test]
    public void HelloWorld_GivenSomeConfig_ExpectSomeTestCases() { 
      
      var sut = new TestCaseLoader();

      var configLibrary = new List<Config>{ 

        Config.New("Places")
          .AddProperty("Place", new[] { "Statue of Liberty", "Eiffel tower", "Great pyramid of Giza" }),

        Config.New("Vocabularies")
          .AddProperty("IMPORT", "Places" )
          .AddProperty("Language", new[]{  "English", "French", "Spanish", "Dutch" }, "x" )
          .AddProperty("Greeting", new[]{ "Hello", "Salut", "Hola", "Hallo" }, "x" )
          .AddProperty("Punctuation", "!" ),

        Config.New("People")
          .AddProperty("Person", new[] { "Ali", "Brian", "Chien" })
          .AddProperty("Positioned", new[]{"standing", "sitting" } ),
      };

      var testConfig = Config.New("HelloWorld")
        .AddProperty("IMPORT", "People")
        .AddProperty("IMPORT", "Vocabularies");

      var testCases = new TestCaseLoader().CasesFromConfigs(testConfig, configLibrary);

      var generatedDataFromTestCases = testCases
        .Select(tc => $"{tc["Person"]} says {tc["Greeting"]} in {tc["Language"]} while {tc["Positioned"]} on the {tc["Place"]}" )
        .ToList();

      // Test some random values...
      var expectedCollection = new[]
      {
        "Ali says Hello in English while sitting on the Statue of Liberty",
        "Ali says Hello in English while sitting on the Eiffel tower",
        "Ali says Hello in English while sitting on the Great pyramid of Giza",
        "Ali says Hello in English while standing on the Statue of Liberty",
        "Ali says Hello in English while standing on the Eiffel tower",
        "Ali says Hello in English while standing on the Great pyramid of Giza",

        "Brian says Hallo in Dutch while sitting on the Statue of Liberty",
        "Brian says Hallo in Dutch while sitting on the Eiffel tower",
        "Brian says Hallo in Dutch while sitting on the Great pyramid of Giza",
        "Brian says Hallo in Dutch while standing on the Statue of Liberty",
        "Brian says Hallo in Dutch while standing on the Eiffel tower",
        "Brian says Hallo in Dutch while standing on the Great pyramid of Giza",

        "Chien says Salut in French while sitting on the Statue of Liberty",
        "Chien says Salut in French while sitting on the Eiffel tower",
        "Chien says Salut in French while sitting on the Great pyramid of Giza",
        "Chien says Salut in French while standing on the Statue of Liberty",
        "Chien says Salut in French while standing on the Eiffel tower",
        "Chien says Salut in French while standing on the Great pyramid of Giza",
      };

      CollectionAssert.IsSubsetOf(expectedCollection, generatedDataFromTestCases);

      // Ensure that correlated values did not permeate...
      var unexpectedCollection = new[]
      {
        "Ali says Hello in French while standing on the Great pyramid of Giza",
        "Brian says Hallo in Spanish while sitting on the Statue of Liberty",
        "Chien says Salut in English while sitting on the Eiffel tower",
      };
      CollectionAssert.IsNotSubsetOf(unexpectedCollection, generatedDataFromTestCases);

    }
  }
}
