using Xunit;
using Xunit.Abstractions;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WebApp;
public class UtilsTest
{
    private static readonly Arr mockUsers = JSON.Parse(File.ReadAllText(FilePath("json", "mock-users.json")));
    // The following lines are needed to get 
    // output to the Console to work in xUnit tests!
    // (also needs the using Xunit.Abstractions)
    // Note: You need to use the following command line command 
    // dotnet test --logger "console;verbosity=detailed"
    // for the logging to work
    private readonly ITestOutputHelper output;
    public UtilsTest(ITestOutputHelper output)
    {
        this.output = output;
    }


    [Fact]
    // A simple initial example
    public void TestSumInt()
    {
        Assert.Equal(12, Utils.SumInts(7, 5));
        Assert.Equal(-3, Utils.SumInts(6, -9));
    }

    [Fact]
    public void TestCreateMockUsers()
    {
        // Read all mock users from the JSON file
        var read = File.ReadAllText(FilePath("json", "mock-users.json"));
        Arr mockUsers = JSON.Parse(read);
        // Get all users from the database
        Arr usersInDb = SQLQuery("SELECT email FROM users");
        Arr emailsInDb = usersInDb.Map(user => user.email);
        // Only keep the mock users not already in db
        Arr mockUsersNotInDb = mockUsers.Filter(
            mockUser => !emailsInDb.Contains(mockUser.email)
        );
        // Get the result of running the method in our code
        var result = Utils.CreateMockUsers();
        // Assert that the CreateMockUsers only return
        // newly created users in the db
        output.WriteLine($"The test expected that {mockUsersNotInDb.Length} users should be added.");
        output.WriteLine($"And {result.Length} users were added.");
        output.WriteLine("The test also asserts that the users added " +
            "are equivalent (the same) to the expected users!");
        Assert.Equivalent(mockUsersNotInDb, result);
        output.WriteLine("The test passed!");
    }


    [Fact]
    public void TestRemoveMockUsers()
{
    // Hämta alla användare från databasen efter att mock-användarna har lagts till
    Arr usersInDbBeforeRemoval = SQLQuery("SELECT email FROM users");
    Arr emailsInDbBeforeRemoval = usersInDbBeforeRemoval.Map(user => user.email);

    // Filtrera ut de mock-användare som faktiskt finns i databasen
    Arr mockUsersInDb = mockUsers.Filter(
        mockUser => emailsInDbBeforeRemoval.Contains(mockUser.email)
    );

    // Kör metoden för att ta bort mock-användarna
    var result = Utils.RemoveMockUsers();

    // Kontrollera att antalet borttagna mock-användare är korrekt
    Console.WriteLine($"The test expected that {mockUsersInDb.Length} users should be removed.");
    Console.WriteLine($"And {result.Length} users were removed.");
    Assert.Equivalent(mockUsersInDb, result);
    Console.WriteLine("The test passed!");
}

 //Metod 2
    [Theory]
    [InlineData("shit happens", "****", "**** happens")]
    [InlineData("angel ass", "****", "angel ****")]

    public void TestRemoveBadWords(string badStrings, string censored, string cleanStrings)
    {
        Assert.Equal(cleanStrings, Utils.RemoveBadWords(badStrings, censored));
    }

[Theory]
    [InlineData("abC9#fgh", true)]  // ok
    [InlineData("stU5/xyz", true)]  // ok too
    [InlineData("abC9#fg", false)]  // too short
    [InlineData("abCd#fgh", false)] // no digit
    [InlineData("abc9#fgh", false)] // no capital letter
    [InlineData("abC9efgh", false)] // no special character
    public void TestIsPasswordGoodEnough(string password, bool expected)
    {
        Assert.Equal(expected, Utils.IsPasswordGoodEnough(password));
    }

    [Theory]
    [InlineData("abC9#fgh", true)]  // ok
    [InlineData("stU5/xyz", true)]  // ok too
    [InlineData("abC9#fg", false)]  // too short
    [InlineData("abCd#fgh", false)] // no digit
    [InlineData("abc9#fgh", false)] // no capital letter
    [InlineData("abC9efgh", false)] // no special character
    public void TestIsPasswordGoodEnoughRegexVersion(string password, bool expected)
    {
        Assert.Equal(expected, Utils.IsPasswordGoodEnoughRegexVersion(password));
    }


    
    [Fact]
    public void TestCountDomainsFromUserEmails()
    {
        Arr users = SQLQuery("SELECT email FROM users");
        Obj domainsindb = Obj();
        foreach(var user in users)
        {
            string domain = user.email.Split("@")[1];
            if(!domainsindb.HasKey(domain))
            {
                domainsindb[domain] = 1;
            }
            else
            {
                domainsindb[domain]++;
            }
        }
        Assert.Equivalent(domainsindb, Utils.CountDomainsFromUserEmails());

        Console.WriteLine("All test correct");
    }
    
            
}

