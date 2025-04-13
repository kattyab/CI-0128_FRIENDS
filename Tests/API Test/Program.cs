﻿using System.Net.Http.Headers;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var baseUrl = "https://localhost:7157/api/LifeInsurance";
var client = new HttpClient();

// Test cases
var testCases = new[]
{
    new { Dob = "2000-01-01", Sex = "male", Token = "1D0B194488C091852597B9AF7D1AA8F23D55C9784815489CF0A488B6F2C6D5C4C569AD51231FACB9920E5A763FE388247E03131D1601AC234E86BC0D266EB6A7", Description = "✅ Valid Request M -30" },
    new { Dob = "2000-01-01", Sex = "female", Token = "1D0B194488C091852597B9AF7D1AA8F23D55C9784815489CF0A488B6F2C6D5C4C569AD51231FACB9920E5A763FE388247E03131D1601AC234E86BC0D266EB6A7", Description = "✅ Valid Request F -30" },
    new { Dob = "1990-01-01", Sex = "male", Token = "1D0B194488C091852597B9AF7D1AA8F23D55C9784815489CF0A488B6F2C6D5C4C569AD51231FACB9920E5A763FE388247E03131D1601AC234E86BC0D266EB6A7", Description = "✅ Valid Request M 30-50" },
    new { Dob = "1990-01-01", Sex = "female", Token = "1D0B194488C091852597B9AF7D1AA8F23D55C9784815489CF0A488B6F2C6D5C4C569AD51231FACB9920E5A763FE388247E03131D1601AC234E86BC0D266EB6A7", Description = "✅ Valid Request F 30-50" },
    new { Dob = "1950-01-01", Sex = "male", Token = "1D0B194488C091852597B9AF7D1AA8F23D55C9784815489CF0A488B6F2C6D5C4C569AD51231FACB9920E5A763FE388247E03131D1601AC234E86BC0D266EB6A7", Description = "✅ Valid Request M +50" },
    new { Dob = "1950-01-01", Sex = "female", Token = "1D0B194488C091852597B9AF7D1AA8F23D55C9784815489CF0A488B6F2C6D5C4C569AD51231FACB9920E5A763FE388247E03131D1601AC234E86BC0D266EB6A7", Description = "✅ Valid Request F +50" },
    new { Dob = "", Sex = "", Token = "1D0B194488C091852597B9AF7D1AA8F23D55C9784815489CF0A488B6F2C6D5C4C569AD51231FACB9920E5A763FE388247E03131D1601AC234E86BC0D266EB6A7", Description = "❌ Missing Both Parameters" },
    new { Dob = "1990-01-01", Sex = "", Token = "1D0B194488C091852597B9AF7D1AA8F23D55C9784815489CF0A488B6F2C6D5C4C569AD51231FACB9920E5A763FE388247E03131D1601AC234E86BC0D266EB6A7", Description = "❌ Missing Sex" },
    new { Dob = "", Sex = "female", Token = "1D0B194488C091852597B9AF7D1AA8F23D55C9784815489CF0A488B6F2C6D5C4C569AD51231FACB9920E5A763FE388247E03131D1601AC234E86BC0D266EB6A7", Description = "❌ Missing DOB" },
    new { Dob = "1990-01-41", Sex = "male", Token = "1D0B194488C091852597B9AF7D1AA8F23D55C9784815489CF0A488B6F2C6D5C4C569AD51231FACB9920E5A763FE388247E03131D1601AC234E86BC0D266EB6A7", Description = "❌ Invalid DOB" },
    new { Dob = "1990-01-01", Sex = "robot", Token = "1D0B194488C091852597B9AF7D1AA8F23D55C9784815489CF0A488B6F2C6D5C4C569AD51231FACB9920E5A763FE388247E03131D1601AC234E86BC0D266EB6A7", Description = "❌ Invalid Sex" },
    new { Dob = "1990-01-01", Sex = "male", Token = "WRONG-TOKEN", Description = "❌ Invalid Token" },
};

foreach (var test in testCases)
{
    var url = $"{baseUrl}?dob={test.Dob}&sex={test.Sex}";
    var request = new HttpRequestMessage(HttpMethod.Get, url);

    request.Headers.Add("FRIENDS-API-TOKEN", test.Token);

    Console.WriteLine($"\n--- {test.Description} ---");
    Console.WriteLine($"GET {url}");

    try
    {
        var response = await client.SendAsync(request);
        var content = await response.Content.ReadAsStringAsync();

        Console.WriteLine($"Status: {(int)response.StatusCode} {response.StatusCode}");
        Console.WriteLine("Response:");
        Console.WriteLine(content);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Error: " + ex.Message);
    }
}
