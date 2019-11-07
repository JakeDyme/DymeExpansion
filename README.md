# DymeExpansion

#### What is it?
- A tool for generating many cases from a small amount of configuration.
- A C# dotnet core library, available on Nuget.
  
#### Where or when should I use it?
- When you're wanting to create many test-cases for the sake of testing.

# Usage 
#### Basic usage
1. Create a config or/and a config library.
2. Use **TestCaseLoader.CasesFromConfig** to generate test cases.
3. Love thy neighbour.

#### Terminology
- Expansion: The creation of many test cases from fewer configs.
- Inheritence: Importing one config into another (or many depending on the expansion).
- Interpolation: Using a value from your test case by injecting it into your content.
  
### Example: Simple Expansion
```
// Create some config using the "Config" class...
var testConfig = Config.New("HelloWorld")
  .AddProperty("Name", "Ali" )
  .AddProperty("Greeting", new[] { "Hello World", "Bonjour le monde" });

// Generate test cases using the "TestCaseLoader" class...
var testCases = TestCaseLoader.CasesFromConfig(testConfig);

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

### Example: Simple Inheritence
```
// Put all person related info into one config...
var people = Config.New("PeopleConfig")
  .AddProperty("Name", new []{"Ali", "Brian" } );

// ...and all greeting related info into another config.
// To bind the data back together, we'll import the people config...
var greetings = Config.New("GreetingConfig")
  .AddProperty("IMPORT", "PeopleConfig")
  .AddProperty("Greeting", new[] { "Hello World", "Bonjour le monde" });

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


