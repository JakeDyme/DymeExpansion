using DymeExpansion.Core.Models;
using DymeExpansion.Core.Services;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

namespace DymeExpansion.Samples.BrowserTests
{
  public class SimpleTestCaseExpansion
  {
    
    // It might not look like it, but the following config generates over thirty three thousand test cases...
    [Test]
    public void CasesFromConfigs_GivenSmallAmountOfConfig_ExpectManyTestCases()
    {
      // Arrange...
      var sut = new TestCaseLoader();
      var configLibrary = new List<Config> {

        Config.New("Application")
          .AddProperty("version", new[]{"1.0","1.5","2.0" }),

        Config.New("User")
          .AddProperty("IMPORT", "Application")
          .AddProperty("user", new[]{"alice","bob","cathy","dave","eve","frank","grant","harry", "ivan" }),

        Config.New("Vehicle")
          .AddProperty("IMPORT", "User")
          .AddProperty("make", new[]{"Audi", "Bugatti", "Chrysler","Dodge", "Ferrari" })
          .AddProperty("year", new[]{"2012", "2013", "2014","2015", "2016" } )
          .AddProperty("condition", new[]{"new", "used" })
          .AddProperty("type", new[]{"convertible", "suv", "4x4", "hatchback", "sudan" })
          .AddProperty("feature", new[]{"airbags", "electric_windows", "seat_warmer", "adjustable_steering", "backwiper" })
      };
      var topLevelConfig = configLibrary.Single(c => c.Name == "Vehicle");

      // Act...
      var testCases = sut.CasesFromConfigs(topLevelConfig, configLibrary);

      // Assert...
      var launchUrls = testCases
        .Select(t => $"http://cars/{t["version"]}/{t["condition"]}/{t["make"]}?user={t["user"]}&year={t["year"]}&with={t["feature"]}&type={t["type"]}")
        .ToList();

      // Check that there are over 33000 test cases...
      Assert.AreEqual(testCases.Count(), 33750);
      // Pick some random urls to check that the permutations were created...
      CollectionAssert.Contains(launchUrls, "http://cars/1.5/new/Dodge?user=harry&year=2013&with=adjustable_steering&type=4x4");
      CollectionAssert.Contains(launchUrls, "http://cars/1.0/new/Audi?user=eve&year=2016&with=electric_windows&type=suv");
      CollectionAssert.Contains(launchUrls, "http://cars/2.0/used/Ferrari?user=cathy&year=2012&with=airbags&type=sudan");
      CollectionAssert.Contains(launchUrls, "http://cars/2.0/used/Chrysler?user=ivan&year=2015&with=backwiper&type=convertible");
    }

  }
}