using DymeExpansion.Core.Models;
using DymeExpansion.Core.Services;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DymeExpansion.Samples.BrowserTests
{
  public class BrowserBasedExpansionsTests
  {
    IWebDriver _driver;
    private string _chromeFolder = "Chrome78";
    private string _outputFolder = "TestResults";

    [SetUp]
    public void startBrowser()
    {
      var pathToDriverExe = Directory.GetCurrentDirectory() + "\\" + _chromeFolder;
      _driver = new ChromeDriver(pathToDriverExe);
    }

    [Test]
    public void SaveScreenshot_GivenSeleniumDriver_Expect()
    {
      // Arrange...
      _driver.Url = "http://www.google.com";
      // Act...
      SaveScreenshot("BasicDriverTest", "google");
      // Assert...
      var expectedFileLocation = $"{Directory.GetCurrentDirectory()}\\{_outputFolder}\\BasicDriverTest\\google.png";
      Assert.IsTrue(File.Exists(expectedFileLocation));
    }

    [Test]
    public void CasesFromConfigs_GivenTypicalDevicePermutation_ExpectTestCases()
    {

      // Arrange...
      var configLibrary = new List<Config> {

      Config.New("DeviceFarm")
        .AddProperty("Selenium.Hub.Url", "http://SuperStack/hub")
        .AddProperty("API.Key", "1234")
        .AddProperty("Username", "JakeDyme"),

      Config.New("IPhone8")
        .AddProperty("Capability: appiumVersion", "1.13.0")
        .AddProperty("Capability: deviceName", "iPhone 8 Simulator")
        .AddProperty("Capability: deviceOrientation", "portrait")
        .AddProperty("Capability: platformVersion", "12.2")
        .AddProperty("Capability: platformName", "iOS")
        .AddProperty("Capability: browserName", "Safari"),

      Config.New("Android")
        .AddProperty("Capability: appiumVersion", "1.9.0")
        .AddProperty("Capability: deviceName", "Android Emulator")
        .AddProperty("Capability: deviceOrientation", "portrait")
        .AddProperty("Capability: platformVersion", "8.0")
        .AddProperty("Capability: platformName", "Android")
        .AddProperty("Capability: browserName", "Chrome"),

      Config.New("Windows10Desktop")
        .AddProperty("Capability: browserVersion", "54.0")
        .AddProperty("Capability: platformName", "Windows 10")
        .AddProperty("Capability: browserName", "firefox"),

      Config.New("Google")
        .AddProperty("Domain", "google")
        .AddProperty("Url", "http://www.google.com/search?p=")
        .AddProperty("Criteria", "laptop"),

      Config.New("Ebay")
        .AddProperty("Domain", "ebay")
        .AddProperty("Url", "http://www.ebay.com/sch/")
        .AddProperty("Criteria", "xbox"),

      Config.New("Amazon")
        .AddProperty("Domain", "amazon")
        .AddProperty("Url", "https://www.amazon.com/")
        .AddProperty("Criteria", "playstation")
      };

      var testConfig = Config.New("TestConfig")
        .AddProperty("IMPORT.DeviceFarms", "DeviceFarm")
        .AddProperty("IMPORT.Devices", new[] { "IPhone8", "Android", "Windows10Desktop" })
        .AddProperty("IMPORT.Sites", new[] { "Google", "Ebay", "Amazon" });

      // Create an instance of the test case loader, and tell it what properties are reference properties "using".
      var sut = new TestCaseLoader();

      // Act, Generate test cases from the configs...
      var testCases = sut.CasesFromConfigs(testConfig, configLibrary);

      // Assert...
      Assert.AreEqual(testCases.Count(), 9);
      foreach (var testCase in testCases)
      {
        // Extract usefull values from flattened test case...
        var url = testCase["Url"];
        var criteria = testCase["Criteria"];
        var domain = testCase["Domain"];

        var launchUrl = $"{url}{criteria}";
        _driver.Url = launchUrl;
        SaveScreenshot(domain, criteria);
      }
    }

    private void SaveScreenshot(string groupName, string fileName)
    {
      var directoryPath = $"{Directory.GetCurrentDirectory()}\\{_outputFolder}\\{groupName}";
      var filePath = $"{directoryPath}\\{fileName}.png";
      Screenshot ss = ((ITakesScreenshot)_driver).GetScreenshot();
      if (!Directory.Exists(directoryPath)) Directory.CreateDirectory(directoryPath);
      ss.SaveAsFile(filePath, ScreenshotImageFormat.Png);
    }

    // In this variation I've realized that Devices all use the same device farm to run on, so instead of specifying the device farm details in the top level config, 
    // I'm simply going to embed the device farm details into all the devices.
    // I've also noticed that the site data is consistent and can be bundled into one config.
    // The only thing to note is that I need to correlate "Domain" and "Url"
    [Test]
    public void CasesFromConfigs_GivenInheritingDeviceSetups_ExpectTestCase()
    {
      // Arrange...
      var sut = new TestCaseLoader();
      var configLibrary = new List<Config> {

        Config.New("DeviceFarm")
          .AddProperty("Selenium.Hub.Url", "http://SuperStack/hub")
          .AddProperty("API.Key", "1234")
          .AddProperty("Username", "JakeDyme"),

        Config.New("IPhone8")
          .AddProperty("IMPORT.DeviceFarms", "DeviceFarm")
          .AddProperty("Capability: appiumVersion", "1.13.0")
          .AddProperty("Capability: deviceName", "iPhone 8 Simulator")
          .AddProperty("Capability: deviceOrientation", "portrait")
          .AddProperty("Capability: platformVersion", "12.2")
          .AddProperty("Capability: platformName", "iOS")
          .AddProperty("Capability: browserName", "Safari"),

        Config.New("Android")
          .AddProperty("IMPORT.DeviceFarms", "DeviceFarm")
          .AddProperty("Capability: appiumVersion", "1.9.0")
          .AddProperty("Capability: deviceName", "Android Emulator")
          .AddProperty("Capability: deviceOrientation", "portrait")
          .AddProperty("Capability: platformVersion", "8.0")
          .AddProperty("Capability: platformName", "Android")
          .AddProperty("Capability: browserName", "Chrome"),

        Config.New("Windows10Desktop")
          .AddProperty("IMPORT.DeviceFarms", "DeviceFarm")
          .AddProperty("Capability: browserVersion", "54.0")
          .AddProperty("Capability: platformName", "Windows 10")
          .AddProperty("Capability: browserName", "firefox"),

        Config.New("Sites")
          .AddProperty("Domain", new []{ "google", "ebay", "amazon" }, "urls&domains" )
          .AddProperty("Url", new[]{"https://www.google.com/search?p=", "http://www.ebay.com/sch/", "https://www.amazon.com/" }, "urls&domains")
          .AddProperty("Criteria", new[]{"laptop", "playstation" })
      };

      var testConfig = Config.New("TestConfig")
        .AddProperty("IMPORT.Devices", new[] { "IPhone8", "Android", "Windows10Desktop" })
        .AddProperty("IMPORT.Sites", "Sites");

      // Act...
      var testCases = sut.CasesFromConfigs(testConfig, configLibrary);

      // Assert...
      foreach (var testCase in testCases)
      {
        var url = testCase["Url"];
        var criteria = testCase["Criteria"];
        var domain = testCase["Domain"];

        _driver.Url = $"{url}/{criteria}";
        SaveScreenshot(domain, "Screenshot");
      }
    }

    [TearDown]
    public void closeBrowser()
    {
      _driver.Close();
    }

  }
}