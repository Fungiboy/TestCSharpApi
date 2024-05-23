namespace WebApp;

public static class Utils
{
    private static readonly Arr mockUsers = JSON.Parse(File.ReadAllText(FilePath("json", "mock-users.json")));
    public static int SumInts(int a, int b)
    {
        return a + b;
    }

    public static Arr CreateMockUsers()
    {
    
        Arr successFullyWrittenUsers = Arr();
        foreach (var user in mockUsers)
        {
            //user.password = "12345678";
            var result = SQLQueryOne(
                @"INSERT INTO users(firstName,lastName,email,password)
                VALUES($firstName, $lastName, $email, $password)
            ", user);
            // If we get an error from the DB then we haven't added
            // the mock users, if not we have so add to the successful list
            if (!result.HasKey("error"))
            {
                // The specification says return the user list without password
                user.Delete("password");
                successFullyWrittenUsers.Push(user);
            }
        }
        return successFullyWrittenUsers;
    }

    public static Arr RemoveMockUsers()
    {
    // Lista för att hålla de användare som faktiskt togs bort
    Arr successfullyRemovedUsers = new Arr();

    foreach (var user in mockUsers)
    {
        // Kontrollera om användaren finns i databasen
        var result = SQLQueryOne(
            "SELECT * FROM users WHERE email = $email",
            new { email = user.email }
        );

        // Om användaren finns i databasen, ta bort användaren
        if (result != null && !result.HasKey("error"))
        {
            // Ta bort användaren
            var deleteResult = SQLQueryOne(
                "DELETE FROM users WHERE email = $email RETURNING firstName, lastName, email",
                new { email = user.email }
            );

            // Om borttagningen lyckades, lägg till användaren i listan
            if (deleteResult != null && !deleteResult.HasKey("error"))
            {
                successfullyRemovedUsers.Push(deleteResult);
            }
        }
    }

    return successfullyRemovedUsers;
    }

    public static Obj CountDomainsFromUserEmails()
    {
        Arr queryDomains = SQLQuery("SELECT SUBSTRING(email, instr(email, '@') + 1, length(email)) AS domain, count(id) AS count FROM users GROUP BY domain");
        Obj countedDomains = Obj();
        foreach(var domain in queryDomains)
        {
            countedDomains[domain.domain] = domain.count;
        }
        return countedDomains;
    }

    public static string RemoveBadWords(string badStrings, string censored)
        {
        var badWordsFromFile = File.ReadAllText(Path.Combine("json", "bad-words.json"));
        Arr badWords = JSON.Parse(badWordsFromFile);

        string cleanStrings = Regex.Replace(badStrings, "\\b" + string.Join("\\b|\\b", badWords) + "\\b", censored, RegexOptions
        .IgnoreCase);

        return cleanStrings;
        }

 public static bool IsPasswordGoodEnough(string password)
    {
        return password.Length >= 8
            && password.Any(Char.IsDigit)
            && password.Any(Char.IsLower)
            && password.Any(Char.IsUpper)
            && password.Any(x => !Char.IsLetterOrDigit(x));
    }

    public static bool IsPasswordGoodEnoughRegexVersion(string password)
    {
        // See: https://dev.to/rasaf_ibrahim/write-regex-password-validation-like-a-pro-5175
        var pattern = @"^(?=.*[0-9])(?=.*[a-zåäö])(?=.*[A-ZÅÄÖ])(?=.*\W).{8,}$";
        return Regex.IsMatch(password, pattern);
    }
}

