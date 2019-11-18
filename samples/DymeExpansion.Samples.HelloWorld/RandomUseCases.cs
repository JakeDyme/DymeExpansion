using DymeExpansion.Core.Models;
using DymeExpansion.Core.Services;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DymeExpansion.Samples.HelloWorld
{
  class RandomUseCases
  {

    [Test]
    public void CasesFromConfigs_GivenInheritingDeviceSetups_ExpectTestCase()
    {
      // Arrange...
      var sut = new DymeCaseLoader();
      var configLibrary = new List<DymeConfig> {

        DymeConfig.New("DeviceFarm")
          .AddProperty("Selenium.Hub.Url", "http://SuperStack/hub")
          .AddProperty("API.Key", "1234")
          .AddProperty("Username", "JakeDyme"),

        DymeConfig.New("IPhone8")
          .AddProperty("IMPORT.DeviceFarms", "DeviceFarm")
          .AddProperty("Capability: appiumVersion", "1.13.0")
          .AddProperty("Capability: deviceName", "iPhone 8 Simulator")
          .AddProperty("Capability: deviceOrientation", "portrait")
          .AddProperty("Capability: platformVersion", "12.2")
          .AddProperty("Capability: platformName", "iOS")
          .AddProperty("Capability: browserName", "Safari"),

        DymeConfig.New("Android")
          .AddProperty("IMPORT.DeviceFarms", "DeviceFarm")
          .AddProperty("Capability: appiumVersion", "1.9.0")
          .AddProperty("Capability: deviceName", "Android Emulator")
          .AddProperty("Capability: deviceOrientation", "portrait")
          .AddProperty("Capability: platformVersion", "8.0")
          .AddProperty("Capability: platformName", "Android")
          .AddProperty("Capability: browserName", "Chrome"),

        DymeConfig.New("Windows10Desktop")
          .AddProperty("IMPORT.DeviceFarms", "DeviceFarm")
          .AddProperty("Capability: browserVersion", "54.0")
          .AddProperty("Capability: platformName", "Windows 10")
          .AddProperty("Capability: browserName", "firefox"),

        DymeConfig.New("Sites")
          .AddProperty("Domain", new []{ "google", "ebay", "amazon" }, "urls&domains" )
          .AddProperty("Url", new[]{"https://www.google.com/search?p=", "http://www.ebay.com/sch/", "https://www.amazon.com/" }, "urls&domains")
          .AddProperty("Criteria", new[]{"laptop", "playstation" })
      };

      var testConfig = DymeConfig.New("TestConfig")
        .AddProperty("IMPORT.Devices", new[] { "IPhone8", "Android", "Windows10Desktop" })
        .AddProperty("IMPORT.Sites", "Sites");

      // Act...
      var testCases = sut.CasesFromConfigs(testConfig, configLibrary);

      // Assert...

      var actualInstructions = new List<string>();
      foreach (var testCase in testCases)
      {
        var url = testCase["Url"];
        var criteria = testCase["Criteria"];
        var domain = testCase["Domain"];
        var capabilities = testCase.Properties
          .Where(prop => prop.Name.StartsWith("Capability: "))
          .ToDictionary(prop => prop.Name.Substring(12), prop => prop.Value);
        actualInstructions.Add($"on {capabilities["platformName"]} device, search for {criteria} on {domain} with url: {url}{criteria}");
      }

      var expectedInstructions = new List<string>() {
        "on iOS device, search for laptop on google with url: https://www.google.com/search?p=laptop",
        "on iOS device, search for laptop on ebay with url: http://www.ebay.com/sch/laptop",
        "on iOS device, search for laptop on amazon with url: https://www.amazon.com/laptop",
        "on iOS device, search for playstation on google with url: https://www.google.com/search?p=playstation",
        "on iOS device, search for playstation on ebay with url: http://www.ebay.com/sch/playstation",
        "on iOS device, search for playstation on amazon with url: https://www.amazon.com/playstation",

        "on Android device, search for laptop on google with url: https://www.google.com/search?p=laptop",
        "on Android device, search for laptop on ebay with url: http://www.ebay.com/sch/laptop",
        "on Android device, search for laptop on amazon with url: https://www.amazon.com/laptop",
        "on Android device, search for playstation on google with url: https://www.google.com/search?p=playstation",
        "on Android device, search for playstation on ebay with url: http://www.ebay.com/sch/playstation",
        "on Android device, search for playstation on amazon with url: https://www.amazon.com/playstation",

        "on Windows 10 device, search for laptop on google with url: https://www.google.com/search?p=laptop",
        "on Windows 10 device, search for laptop on ebay with url: http://www.ebay.com/sch/laptop",
        "on Windows 10 device, search for laptop on amazon with url: https://www.amazon.com/laptop",
        "on Windows 10 device, search for playstation on google with url: https://www.google.com/search?p=playstation",
        "on Windows 10 device, search for playstation on ebay with url: http://www.ebay.com/sch/playstation",
        "on Windows 10 device, search for playstation on amazon with url: https://www.amazon.com/playstation",
      };

      CollectionAssert.AreEquivalent(expectedInstructions, actualInstructions);
    }
  }
}
