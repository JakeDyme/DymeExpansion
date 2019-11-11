using DymeExpansion.Core.Models;
using DymeExpansion.Core.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DymeExpansion.Samples.BrowserTests
{
  public class HelloWorld
  {
    [Test]
    public void Expansion_Basic()
    {
      var actualGreetings = new List<string>();

      // First we'll create some config using the "Config" class...
      var testConfig = DymeConfig.New("HelloWorld")
        .AddProperty("Name", "Ali" )
        .AddProperty("Greeting", new[] { "Hello World", "Bonjour le monde" });

      // Then we'll automatically generate our test cases using the "TestCaseLoader" class...
      var testCases = DymeCaseLoader.CasesFromConfig(testConfig);

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
    public void Inheritence_Basic()
    {
      var actualGreetings = new List<string>();

      // In this example we want to keep our data separated into neat bundles of related information...
      // We'll put all person related info into one config...
      var people = DymeConfig.New("PeopleConfig")
        .AddProperty("Name", new []{"Ali", "Brian" } );

      // ...and all greeting related info into another config.
      // To bind the data back together, we'll import the people config into the greeting config...
      var greetings = DymeConfig.New("GreetingConfig")
        .AddProperty("IMPORT", "PeopleConfig")
        .AddProperty("Greeting", new[] { "Hello World", "Bonjour le monde" });

      // We'll create a library of configs...
      var configLibrary = new[] { people, greetings };

      //...and pass it in along with the config that we want to interpret...
      var testCases = DymeCaseLoader.CasesFromConfig(greetings, configLibrary); //...(the passed in config library is used to resolve any "IMPORT" references)

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

    /// <summary>
    /// In this test we want to make sure that certain data remains in discreet packets of information.
    /// We achieved this by expanding multiple imports...
    /// </summary>
    [Test]
    public void Encapsulation_Basic()
    {
      var actualGreetings = new List<string>();

      // Firstly lets define some people..
      var ali = DymeConfig.New("AliConfig")
        .AddProperty("Name", "Ali Mire")
        .AddProperty("Age", "40")
        .AddProperty("Gender", "male");

      var bernice = DymeConfig.New("BerniceConfig")
        .AddProperty("Name", "Bernice Newton")
        .AddProperty("Age", "35")
        .AddProperty("Gender", "female");

      // Next we'll pull those people into our greeting config...
      var greetings = DymeConfig.New("GreetingConfig")
        .AddProperty("IMPORT", new [] { "AliConfig", "BerniceConfig" })
        .AddProperty("Greeting", new[] { "Hello World", "Bonjour le monde" });
      
      // We'll create a library of configs...
      var configLibrary = new[] { ali, bernice, greetings };

      //...and pass it in along with the config that we want to interpret...
      var testCases = DymeCaseLoader.CasesFromConfig(greetings, configLibrary); //...(the passed in config library is used to resolve any "IMPORT" references)

      // Extract the data from our shiny new test cases, and use it to start a party...
      foreach (var testCase in testCases)
      {
        var name = testCase["Name"];
        var age = testCase["Age"];
        var gender = testCase["Gender"];
        var greeting = testCase["Greeting"];
        var finalGreeting = $"{name} (a {age} year old {gender}) says {greeting}";
        Debug.WriteLine(finalGreeting);
        actualGreetings.Add(finalGreeting);
      }

      // Just to make sure that we're saying the right stuff...
      var expectedGreetings = new[]
      {
        "Ali Mire (a 40 year old male) says Hello World",
        "Ali Mire (a 40 year old male) says Bonjour le monde",
        "Bernice Newton (a 35 year old female) says Hello World",
        "Bernice Newton (a 35 year old female) says Bonjour le monde",
      };
      CollectionAssert.AreEquivalent(expectedGreetings, actualGreetings);

      // Make sure we don't have any of the wrong stuff...
      var antiGreetings = new List<string>
      {
        "Ali Mire (a 40 year old female) says Hello World",
        "Ali Mire (a 35 year old male) says Bonjour le monde",
        "Ali Mire (a 35 year old female) says Bonjour le monde",
        "Bernice Newton (a 35 year old male) says Hello World",
        "Bernice Newton (a 40 year old female) says Bonjour le monde",
        "Bernice Newton (a 40 year old male) says Bonjour le monde",
      };
      antiGreetings.ForEach(antiGreeting => CollectionAssert.DoesNotContain(actualGreetings, antiGreeting));
    }

    [Test]
    public void Correlation_Basic()
    {
      var actualGreetings = new List<string>();

      // Lets define some people properties with multiple values, and then correlate those properties using a correlation key...
      var people = DymeConfig.New("PeopleConfig")
        .AddProperty("Name",   new[]{ "Ali Mire", "Bernice Newton" }, "someKey")
        .AddProperty("Age",    new[]{ "40",       "35" },             "someKey")
        .AddProperty("Gender", new[]{ "male",     "female" },         "someKey");
      // ..."someKey" will work fine here, but in larger or more complex config structures, you may want to consider using a GUID..

      // Next we'll pull those people into our greeting config...
      var greetings = DymeConfig.New("GreetingConfig")
        .AddProperty("IMPORT", "PeopleConfig")
        .AddProperty("Greeting", new[] { "Hello World", "Bonjour le monde" });

      // We'll create a library of configs...
      var configLibrary = new[] { people };

      //...and pass it in along with the config that we want to interpret...
      var testCases = DymeCaseLoader.CasesFromConfig(greetings, configLibrary); //...(the passed in config library is used to resolve any "IMPORT" references)

      // Extract the data from our new test cases...
      foreach (var testCase in testCases)
      {
        var name = testCase["Name"];
        var age = testCase["Age"];
        var gender = testCase["Gender"];
        var greeting = testCase["Greeting"];
        var finalGreeting = $"{name} (a {age} year old {gender}) says {greeting}";
        Debug.WriteLine(finalGreeting);
        actualGreetings.Add(finalGreeting);
      }

      // Just to make sure that we have the right stuff...
      var expectedGreetings = new[]
      {
        "Ali Mire (a 40 year old male) says Hello World",
        "Ali Mire (a 40 year old male) says Bonjour le monde",
        "Bernice Newton (a 35 year old female) says Hello World",
        "Bernice Newton (a 35 year old female) says Bonjour le monde",
      };
      CollectionAssert.AreEquivalent(expectedGreetings, actualGreetings);

      // Make sure that we don't have any of the wrong stuff...
      var antiGreetings = new List<string>
      {
        "Ali Mire (a 40 year old female) says Hello World",
        "Ali Mire (a 35 year old male) says Bonjour le monde",
        "Ali Mire (a 35 year old female) says Bonjour le monde",
        "Bernice Newton (a 35 year old male) says Hello World",
        "Bernice Newton (a 40 year old female) says Bonjour le monde",
        "Bernice Newton (a 40 year old male) says Bonjour le monde",
      };
      antiGreetings.ForEach(antiGreeting => CollectionAssert.DoesNotContain(actualGreetings, antiGreeting));
    }

    [Test]
    public void Correlation_AcrossConfigs_Basic()
    {
      var actualGreetings = new List<string>();

      // Lets define some people properties with multiple values, and then correlate those properties using a correlation key...
      var names = DymeConfig.New("NamesConfig")
        .AddProperty("Name", new[] { "Ali Mire", "Bernice Newton" }, "someKey");

      var ages = DymeConfig.New("AgeConfig")
        .AddProperty("Age", new[] { "40", "35" }, "someKey");
      // ..."someKey" will work fine here, but in larger or more complex config structures, you may want to consider using a GUID..

      var genders = DymeConfig.New("GenderConfig")
        .AddProperty("Gender", new[] { "male", "female" }, "someKey");

      // Next we'll pull those people into our greeting config...
      var greetings = DymeConfig.New("GreetingConfig")
        .AddProperty("IMPORT", "NamesConfig")
        .AddProperty("IMPORT", "AgeConfig")
        .AddProperty("IMPORT", "GenderConfig")
        .AddProperty("Greeting", new[] { "Hello World", "Bonjour le monde" });

      // We'll create a library of configs...
      var configLibrary = new[] { genders, names, ages, greetings };

      //...and pass it in along with the config that we want to interpret...
      var testCases = DymeCaseLoader.CasesFromConfig(greetings, configLibrary); //...(the passed in config library is used to resolve any "IMPORT" references)

      // Extract the data from our new test cases...
      foreach (var testCase in testCases)
      {
        var name = testCase["Name"];
        var age = testCase["Age"];
        var gender = testCase["Gender"];
        var greeting = testCase["Greeting"];
        var finalGreeting = $"{name} (a {age} year old {gender}) says {greeting}";
        Debug.WriteLine(finalGreeting);
        actualGreetings.Add(finalGreeting);
      }

      // Just to make sure that we have the right stuff...
      var expectedGreetings = new[]
      {
        "Ali Mire (a 40 year old male) says Hello World",
        "Ali Mire (a 40 year old male) says Bonjour le monde",
        "Bernice Newton (a 35 year old female) says Hello World",
        "Bernice Newton (a 35 year old female) says Bonjour le monde",
      };
      CollectionAssert.AreEquivalent(expectedGreetings, actualGreetings);

      // Make sure that we don't have any of the wrong stuff...
      var antiGreetings = new List<string>
      {
        "Ali Mire (a 40 year old female) says Hello World",
        "Ali Mire (a 35 year old male) says Bonjour le monde",
        "Ali Mire (a 35 year old female) says Bonjour le monde",
        "Bernice Newton (a 35 year old male) says Hello World",
        "Bernice Newton (a 40 year old female) says Bonjour le monde",
        "Bernice Newton (a 40 year old male) says Bonjour le monde",
      };
      antiGreetings.ForEach(antiGreeting => CollectionAssert.DoesNotContain(actualGreetings, antiGreeting));

    }
    [Test]
    public void Correlation_AcrossConfigs()
    {
      var actualGreetings = new List<string>();

      // Lets define some people properties with multiple values, and then correlate those properties using a correlation key...
      var names = DymeConfig.New("NamesConfig")
        .AddProperty("Name", new[] { "Ali Mire", "Bernice Newton" }, "someKey")
        .AddProperty("Age", new[] { "40", "35" }, "someKey");
        
      var genders = DymeConfig.New("GenderConfig")
        .AddProperty("Gender", new[] { "male", "female" }, "someKey");
      // ..."someKey" will work fine here, but in larger or more complex config structures, you may want to consider using a GUID..

      // Next we'll pull those people into our greeting config...
      var greetings = DymeConfig.New("GreetingConfig")
        .AddProperty("IMPORT", "NamesConfig")
        .AddProperty("IMPORT", "GenderConfig")
        .AddProperty("Greeting", new[] { "Hello World", "Bonjour le monde" });

      // We'll create a library of configs...
      var configLibrary = new[] { names, genders, greetings };

      //...and pass it in along with the config that we want to interpret...
      var testCases = DymeCaseLoader.CasesFromConfig(greetings, configLibrary); //...(the passed in config library is used to resolve any "IMPORT" references)

      // Extract the data from our new test cases...
      foreach (var testCase in testCases)
      {
        var name = testCase["Name"];
        var age = testCase["Age"];
        var gender = testCase["Gender"];
        var greeting = testCase["Greeting"];
        var finalGreeting = $"{name} (a {age} year old {gender}) says {greeting}";
        Debug.WriteLine(finalGreeting);
        actualGreetings.Add(finalGreeting);
      }

      // Just to make sure that we have the right stuff...
      var expectedGreetings = new[]
      {
        "Ali Mire (a 40 year old male) says Hello World",
        "Ali Mire (a 40 year old male) says Bonjour le monde",
        "Bernice Newton (a 35 year old female) says Hello World",
        "Bernice Newton (a 35 year old female) says Bonjour le monde",
      };
      CollectionAssert.AreEquivalent(expectedGreetings, actualGreetings);

      // Make sure that we don't have any of the wrong stuff...
      var antiGreetings = new List<string>
      {
        "Ali Mire (a 40 year old female) says Hello World",
        "Ali Mire (a 35 year old male) says Bonjour le monde",
        "Ali Mire (a 35 year old female) says Bonjour le monde",
        "Bernice Newton (a 35 year old male) says Hello World",
        "Bernice Newton (a 40 year old female) says Bonjour le monde",
        "Bernice Newton (a 40 year old male) says Bonjour le monde",
      };
      antiGreetings.ForEach(antiGreeting => CollectionAssert.DoesNotContain(actualGreetings, antiGreeting));
    }


    [Test]
    public void Composition_Basic()
    {
      var actualGreetings = new List<string>();

      var people = DymeConfig.New("PeopleConfig")
        .AddProperty("Greeting", new[] { "Goodbuy", "Bon achat" })
        .AddProperty("Name", new[] { "Ali", "Brian" });

      var greetings = DymeConfig.New("GreetingConfig")
        .AddProperty("Greeting", new[] { "Hello World", "Bonjour le monde" });

      // Create a config composed of many configs...
      var composedConfig = DymeConfig.New("ComposedConfig")
        .AddProperty("IMPORT", "PeopleConfig")
        .AddProperty("IMPORT", "GreetingConfig");
      // Note: the property "Greeting" from "GreetingConfig" will override "Greeting" from "PeopleConfig" 
      // because "GreetingConfig" is imported after "PeopleConfig" in "ComposedConfig"

      // Create a config library of configs...
      var configLibrary = new[] { people, greetings };

      //...and pass it in along with the config that we want to interpret...
      var testCases = DymeCaseLoader.CasesFromConfig(composedConfig, configLibrary);

      // Extract the data from our test cases, and use it to start a family...
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
      CollectionAssert.AreEquivalent(expectedGreetings, actualGreetings);

      // Make sure we don't have any of the wrong stuff...
      var antiGreetings = new List<string>
      {
        "Ali says Goodbuy",
        "Ali says Goodbuy",
        "Ali says Bon achat",
        "Ali says Bon achat",
      };
      antiGreetings.ForEach(antiGreeting => CollectionAssert.DoesNotContain(actualGreetings, antiGreeting));
    }

    [Test]
    public void Composition_WithMultiValueImports()
    {
      var actualGreetings = new List<string>();

      var ali = DymeConfig.New("AliConfig")
        .AddProperty("Greeting", new[] { "Goodbuy", "Bon achat" })
        .AddProperty("Name", new[] { "Ali", "Brian" });

      var bernice = DymeConfig.New("BerniceConfig")
        .AddProperty("Greeting", "What?")
        .AddProperty("Name", new[] { "Ali", "Brian" });

      var greetings = DymeConfig.New("GreetingConfig")
        .AddProperty("Greeting", new[] { "Hello World", "Bonjour le monde" });

      // Create a config composed of many configs...
      var composedConfig = DymeConfig.New("ComposedConfig")
        .AddProperty("IMPORT.People", new[] { "AliConfig", "BerniceConfig" }) //...using an optional suffix (.People) for readability
        .AddProperty("IMPORT", "GreetingConfig");
      // Note: the property "Greeting" from "GreetingConfig" will override "Greeting" from "PeopleConfig" 
      // because "GreetingConfig" is imported after "PeopleConfig" in "ComposedConfig"

      // Create a config library of configs...
      var configLibrary = new[] { ali, bernice, greetings };

      //...and pass it in along with the config that we want to interpret...
      var testCases = DymeCaseLoader.CasesFromConfig(composedConfig, configLibrary);

      // Extract the data from our test cases, and use it to start a family...
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
      CollectionAssert.AreEquivalent(expectedGreetings, actualGreetings);

      // Make sure we don't have any of the wrong stuff...
      var antiGreetings = new List<string>
      {
        "Ali says Goodbuy",
        "Ali says Goodbuy",
        "Ali says Bon achat",
        "Ali says Bon achat",
      };
      antiGreetings.ForEach(antiGreeting => CollectionAssert.DoesNotContain(actualGreetings, antiGreeting));
    }

    [Test]
    public void CompositionCorrelationInheritenceExpansion()
    {
      var actualEvents = new List<string>();

      var configLibrary = new List<DymeConfig>{
        // Device farm details...
        DymeConfig.New("DeviceFarm")
          .AddProperty("df.hubUrl", "https://devicefarm.com/hub")
          .AddProperty("df.login", "JakeD")
          .AddProperty("df.ApiKey", "SOME+API+KEY"),

        // Capabilities for an IPhone device...
        DymeConfig.New("IPhone7")
          .AddProperty("cap.deviceName", "iPhone 7 Simulator")
          .AddProperty("cap.appiumVersion", "1.15.0")
          .AddProperty("cap.deviceOrientation", "portrait")
          .AddProperty("cap.platformVersion", "13.0")
          .AddProperty("cap.platformName", "iOS")
          .AddProperty("cap.browserName", "Safari"),

        // Capabilities for a Samsung device...
        DymeConfig.New("SamsungGalaxyS7")
          .AddProperty("cap.deviceName", "Samsung Galaxy S7 Emulator")
          .AddProperty("cap.appiumVersion", "1.9.1")
          .AddProperty("cap.deviceOrientation", "portrait")
          .AddProperty("cap.platformVersion", "8.1")
          .AddProperty("cap.platformName", "Android")
          .AddProperty("cap.browserName", "Chrome")
      };

      // Create a composition config for your test that brings all your information together...
      var testConfig = DymeConfig.New("TestConfig")
        .AddProperty("IMPORT.Devices", new[] { "IPhone7", "SamsungGalaxyS7" }) // ...Optional suffixes (eg. ".Devices") can be added to improve readability.
        .AddProperty("IMPORT.EnvHosts", "DeviceFarm") // ...Trailing imported configs override properties from preceding configs. (.EnvHosts will override any conflicting properties from .Devices)
        .AddProperty("ApiKey", "USE+THIS+KEY+INSTEAD") //...Top level properties override properties from imported configs.
        .AddProperty("Url", new [] {"https://www.google.com", "https://www.facebook.com" }, "btn&Url") //...Properties with the same correlation ID will be correlated (by order).
        .AddProperty("SearchButtonId", new[]{ "btnGgleSearch", "fcbkSrchBtn" }, "btn&Url"); //...Correlated properties must have the same number of values (i.e. 2 Urls, 2 button Ids).

      // Generate test cases...
      var testCases = DymeCaseLoader.CasesFromConfig(testConfig, configLibrary); //...(the passed in config library is used to resolve any "IMPORT" references)

      // Extract the data from the test cases, and use it to change the world...
      foreach (var testCase in testCases)
      {
        var url = testCase["Url"];
        var btn = testCase["SearchButtonId"];
        var loginDetails = getDeviceFarmLoginFromTestCase(testCase);
        var capabilities = getDeviceDetailsFromTestCase(testCase);
        var browserInstance = getWebDriverInstance(capabilities, loginDetails);
        // Perform actions...
        browserInstance.LaunchUrl(url);
        browserInstance.ClickOnElement(btn);
        browserInstance.TakeScreenshot();
        // Create validation report...
        var deviceName = testCase["cap.deviceName"];
        actualEvents.Add($"On {deviceName}, launch {url}, and then click {btn}");
      }

      // Make sure that we're doing the right stuff...
      var expectedEvents = new[]
      {
        "On iPhone 7 Simulator, launch https://www.google.com, and then click btnGgleSearch",
        "On iPhone 7 Simulator, launch https://www.facebook.com, and then click fcbkSrchBtn",
        "On Samsung Galaxy S7 Emulator, launch https://www.google.com, and then click btnGgleSearch",
        "On Samsung Galaxy S7 Emulator, launch https://www.facebook.com, and then click fcbkSrchBtn",
      };
      CollectionAssert.AreEquivalent(expectedEvents, actualEvents);

      // Make sure that we're not doing the wrong stuff...
      var antiEvents = new List<string>
      {
        "On Samsung Galaxy S7 Emulator, launch https://www.google.com, and click fcbkSrchBtn",
        "On Samsung Galaxy S7 Emulator, launch https://www.facebook.com, and click btnGgleSearch",
        "On iPhone 7 Simulator, launch https://www.google.com, and click fcbkSrchBtn",
        "On iPhone 7 Simulator, launch https://www.facebook.com, and click btnGgleSearch",
      };
      antiEvents.ForEach(antiEvent => CollectionAssert.DoesNotContain(actualEvents, antiEvent));
    }

    private Dictionary<string,string> getDeviceDetailsFromTestCase(DymeCase testCase)
    {
      return testCase.Properties
        .Where(p => p.Name.StartsWith("cap."))
        .ToDictionary(i => i.Name, i => i.Name.Substring(4));
    }

    private Dictionary<string, string> getDeviceFarmLoginFromTestCase(DymeCase testCase)
    {
      return testCase.Properties
        .Where(p => p.Name.StartsWith("df."))
        .ToDictionary(i => i.Name, i => i.Name.Substring(3));
    }

    private dynamic getWebDriverInstance(Dictionary<string, string> capabilities, Dictionary<string, string> login)
    {
      // Create a web driver, using the appropriate capabilities and login creds.
      // I'm just returning a mock web driver here for the sake of demonstration...
      return new
      {
        LaunchUrl = new Action<string>((string url) => { /* Implement action */ }),
        ClickOnElement = new Action<string>((string elementName) => { /* Implement action */ }),
        TakeScreenshot = new Action(() => { /* Implement action */ })
      };
    }

    [Test]
    public void Expansion_Example2() { 
      
      var sut = new DymeCaseLoader();

      var configLibrary = new List<DymeConfig>{ 

        DymeConfig.New("Places")
          .AddProperty("Place", new[] { "Statue of Liberty", "Eiffel tower", "Great pyramid of Giza" }),

        DymeConfig.New("Vocabularies")
          .AddProperty("IMPORT", "Places" )
          .AddProperty("Language", new[]{  "English", "French", "Spanish", "Dutch" }, "x" )
          .AddProperty("Greeting", new[]{ "Hello", "Salut", "Hola", "Hallo" }, "x" )
          .AddProperty("Punctuation", "!" ),

        DymeConfig.New("People")
          .AddProperty("Person", new[] { "Ali", "Brian", "Chien" })
          .AddProperty("Positioned", new[]{"standing", "sitting" } ),
      };

      var testConfig = DymeConfig.New("HelloWorld")
        .AddProperty("IMPORT", "People")
        .AddProperty("IMPORT", "Vocabularies");

      var testCases = new DymeCaseLoader().CasesFromConfigs(testConfig, configLibrary);

      var generatedDataFromTestCases = testCases
        .Select(tc => $"{tc["Person"]} says {tc["Greeting"]} in {tc["Language"]} while {tc["Positioned"]} on the {tc["Place"]}" )
        .ToList();

      // Make sure that we have some of the right stuff...
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

      // Make sure we dont have any of the wrong stuff...
      var unexpectedCollection = new List<string>
      {
        "Ali says Hello in French while standing on the Great pyramid of Giza",
        "Brian says Hallo in Spanish while sitting on the Statue of Liberty",
        "Chien says Salut in English while sitting on the Eiffel tower",
      };
      unexpectedCollection.ForEach(antiGreeting => CollectionAssert.DoesNotContain(generatedDataFromTestCases, antiGreeting));

    }
  }
}
