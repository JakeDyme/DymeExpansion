# DymeExpansion

#### What is it?
- A tool for generating many cases from a small amount of configuration.
- A C# dotnet core library, available on Nuget.
  
#### Where or when should I use it?
- When you're wanting to create many test-cases for the sake of testing.

# Usage 
#### Basic usage
1. Create a config or/and a config library.
2. Use **DymeCaseLoader.CasesFromConfig** to generate test cases.
3. Love thy neighbour.

#### Terminology
- Expansion: The creation of many test cases from fewer configs.
- Inheritence: Importing one config into another (or many depending on the expansion).
- Interpolation: Using a value from your test case by injecting it into your content.
  
## Expansions
Expansions are properties in a config that have multiple values. If all properties only have one value, then exactly one test case will be generated, but if even one property has more than one value, then multple test cases will be generated. Expansions allow you to easily add data to your configs, and have the system generate more test cases as you do.

### Example: Simple Expansion
```C#
// Create some config using the "DymeConfig" class...
var testConfig = DymeConfig.New("HelloWorld")
  .AddProperty("Name", "Ali" )
  .AddProperty("Greeting", new[] { "Hello World", "Bonjour le monde" }); // Expansion

// Generate test cases using the "DymeCaseLoader" class...
var testCases = DymeCaseLoader.CasesFromConfig(testConfig);

// Use the data from test cases...
foreach (var testCase in testCases)
{
  var greeting = testCase["Name"] + " says " + testCase["Greeting"];
  Debug.WriteLine(greeting);
}

/*
Outputs:
--------------------------------
Ali says Hello World
Ali says Bonjour le monde
--------------------------------
/*
```
## Encapsulation & Inheritence
Encapsulation means bundeling data into discreet packets of related information. Doing this makes your configs much easier to maintain. You can then use inheritence to bind the data back together by referencing configs from other configs.
### Example: Simple Inheritence
```C#
// Put all person related info into one config...
var people = DymeConfig.New("PeopleConfig")
  .AddProperty("Name", new []{"Ali", "Brian" } );

// ...and all greeting related info into another config.
// To bind the data back together, we'll import the people config 
// using the IMPORT keyword as the property name, 
// and the config name as the property value...
var greetings = DymeConfig.New("GreetingConfig")
  .AddProperty("IMPORT", "PeopleConfig")
  .AddProperty("Greeting", new[] { "Hello World", "Bonjour le monde" });

// We'll create a library of configs...
var configLibrary = new[] { people, greetings };

//...and pass it in along with the config that we want to interpret...
var testCases = DymeCaseLoader.CasesFromConfig(greetings, configLibrary);

// Use the data from test cases...
foreach (var testCase in testCases)
{
  var greeting = testCase["Name"] + " says " + testCase["Greeting"];
  Debug.WriteLine(greeting);
}

/*
Outputs:
--------------------------------
Ali says Hello World
Ali says Bonjour le monde
Brian says Hello World
Brian says Bonjour le monde
--------------------------------
/*
```

### Example: Encapsulated Inheritence
```C#
// First lets define some people..
var ali = DymeConfig.New("AliConfig")
  .AddProperty("Name", "Ali Mire")
  .AddProperty("Age", "40")
  .AddProperty("Gender", "male");

var bernice = DymeConfig.New("BerniceConfig")
  .AddProperty("Name", "Bernice Newton")
  .AddProperty("Age", "35")
  .AddProperty("Gender", "female");

// Next we'll pull each person into our greeting config...
var greetings = DymeConfig.New("GreetingConfig")
  .AddProperty("IMPORT", new [] {"AliConfig", "BerniceConfig" })
  .AddProperty("Greeting", new[] { "Hello World", "Bonjour le monde" });

// We'll create a library of out of all the configs...
var configLibrary = new[] { ali, bernice, greetings };

//...and then pass the library into the case loader,
// along with the config that we want to interpret...
var testCases = DymeCaseLoader.CasesFromConfig(greetings, configLibrary); 

// Note: The passed in config library is used to resolve any "IMPORT" references,
// So if you know ahead of time which configs are going to be referenced, 
// then you could filter your library first. 
// Alternatively, you could just pass in all your configs 
// and let the engine pick them out as it needs.

// Process your test cases...
foreach (var testCase in testCases)
{
  // Extract the data from the current test case...
  var name = testCase["Name"];
  var age = testCase["Age"];
  var gender = testCase["Gender"];
  var greeting = testCase["Greeting"];
  // Compose the data in some meaningfull way...
  var finalGreeting = $"{name} (a {age} year old {gender}) says {greeting}";
  // Use the composition...
  Debug.WriteLine(finalGreeting);
}

/*
Outputs:
-------------------------------------------------------------
Ali Mire (a 40 year old male) says Hello World
Ali Mire (a 40 year old male) says Bonjour le monde
Bernice Newton (a 35 year old female) says Hello World
Bernice Newton (a 35 year old female) says Bonjour le monde
-------------------------------------------------------------
*/
```
So what exactly happenned that was important? <br>
What has encapsulation done for us? <br>
Well, because of encapsulation, we did not incorrectly output the following:
```C#
/*
-------------------------------------------------------------
Ali Mire (a 35 year old female) says Hello World
Bernice Newton (a 40 year old male) says Hello World
-------------------------------------------------------------
*/
```
## Correlation
Another way to bundle data is to correlate properties. This can be done by adding a common correlation key to the properties that you want to correlate. Not all properties in the same config have to be correlated, but properties can only be correlated inside one config. In other words, you cannot correlate properties across configs. Correlated proprties must have the same number of values.
### Example: Correlation
```C#
// Define properties with multiple values, and then correlate those properties with a correlation key...
var people = DymeConfig.New("PeopleConfig")
  .AddProperty("Name",   new[]{ "Ali Mire", "Bernice Newton" }, "someKey")
  .AddProperty("Age",    new[]{ "40",       "35" },             "someKey")
  .AddProperty("Gender", new[]{ "male",     "female" },         "someKey");
// ..."someKey" will work fine here, but in larger or more complex config structures, you may want to consider using a GUID.

// Pull those people into our greeting config...
var greetings = DymeConfig.New("GreetingConfig")
  .AddProperty("IMPORT", "PeopleConfig")
  .AddProperty("Greeting", new[] { "Hello World", "Bonjour le monde" });

// We'll create a library of configs...
var configLibrary = new[] { people };

//...and pass it in along with the config that we want to interpret...
var testCases = DymeCaseLoader.CasesFromConfig(greetings, configLibrary);

// Extract the data from our new test cases and use it start a family...
foreach (var testCase in testCases)
{
  var name = testCase["Name"];
  var age = testCase["Age"];
  var gender = testCase["Gender"];
  var greeting = testCase["Greeting"];
  var finalGreeting = $"{name} (a {age} year old {gender}) says {greeting}";
  Debug.WriteLine(finalGreeting);
}

/*
Outputs:
------------------------------------------------------------
Ali Mire (a 40 year old male) says Hello World
Ali Mire (a 40 year old male) says Bonjour le monde
Bernice Newton (a 35 year old female) says Hello World
Bernice Newton (a 35 year old female) says Bonjour le monde
------------------------------------------------------------
Does not output:
------------------------------------------------------------
Ali Mire (a 40 year old female) says Hello World
Ali Mire (a 35 year old male) says Bonjour le monde
Ali Mire (a 35 year old female) says Bonjour le monde
Bernice Newton (a 35 year old male) says Hello World
Bernice Newton (a 40 year old female) says Bonjour le monde
Bernice Newton (a 40 year old male) says Bonjour le monde
------------------------------------------------------------
*/
```

## Composition
You can import configs one after the other, creating a composition of imported configs.<br>
Conflicting properties will be overridded by subsequent configs. In other words, the first imported config can be thought of as a default config, over which the next config wll be overlayed. <br>
Properties in the current config will override conflicting properties from any imported config.  
```C#
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
// Note: the property "Greeting" from "GreetingConfig" will override "Greeting" from "PeopleConfig" because "GreetingConfig" is imported after "PeopleConfig" in "ComposedConfig"

// Create a config library of configs...
var configLibrary = new[] { people, greetings };

// Get your test cases...
var testCases = DymeCaseLoader.CasesFromConfig(composedConfig, configLibrary);

// Extract the data from the test cases, and use it to make your computer sentient...
foreach (var testCase in testCases)
{
  var greeting = testCase["Name"] + " says " + testCase["Greeting"];
  Debug.WriteLine(greeting);
}

/*
Outputs:
---------------------------------
Ali says Hello World
Ali says Bonjour le monde
Brian says Hello World
Brian says Bonjour le monde
---------------------------------
Does not output:
---------------------------------
Ali says Goodbuy
Ali says Goodbuy
Ali says Bon achat
Ali says Bon achat
*/
```