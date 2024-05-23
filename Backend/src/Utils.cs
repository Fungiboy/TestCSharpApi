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
        Utils.CreateMockUsers();
        var qry = SQLQuery("SELECT * FROM EmailCounter ORDER BY counter desc");
        var result = Obj();
        qry.ForEach(row => result[row.domain] = row.counter);
        Utils.RemoveMockUsers();
        Log("Domain results: ", result);
        return result;
    }
}

