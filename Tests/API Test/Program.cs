using System.Net.Http.Headers;
Console.OutputEncoding = System.Text.Encoding.UTF8;
var baseUrl = "https://localhost:7157/api/LifeInsurance";
var client = new HttpClient();

var testCases = new[]
{
    new { DateOfBirth = "2000-01-01", Sex = "male", Token = "1D0B194488C091852597B9AF7D1AA8F23D55C9784815489CF0A488B6F2C6D5C4C569AD51231FACB9920E5A763FE388247E03131D1601AC234E86BC0D266EB6A7", Description = "✅ Valid Request M -30" },
    new { DateOfBirth = "2000-01-01", Sex = "female", Token = "1D0B194488C091852597B9AF7D1AA8F23D55C9784815489CF0A488B6F2C6D5C4C569AD51231FACB9920E5A763FE388247E03131D1601AC234E86BC0D266EB6A7", Description = "✅ Valid Request F -30" },
    new { DateOfBirth = "1990-01-01", Sex = "male", Token = "1D0B194488C091852597B9AF7D1AA8F23D55C9784815489CF0A488B6F2C6D5C4C569AD51231FACB9920E5A763FE388247E03131D1601AC234E86BC0D266EB6A7", Description = "✅ Valid Request M 30-50" },
    new { DateOfBirth = "1990-01-01", Sex = "female", Token = "1D0B194488C091852597B9AF7D1AA8F23D55C9784815489CF0A488B6F2C6D5C4C569AD51231FACB9920E5A763FE388247E03131D1601AC234E86BC0D266EB6A7", Description = "✅ Valid Request F 30-50" },
    new { DateOfBirth = "1950-01-01", Sex = "male", Token = "1D0B194488C091852597B9AF7D1AA8F23D55C9784815489CF0A488B6F2C6D5C4C569AD51231FACB9920E5A763FE388247E03131D1601AC234E86BC0D266EB6A7", Description = "✅ Valid Request M +50" },
    new { DateOfBirth = "1950-01-01", Sex = "female", Token = "1D0B194488C091852597B9AF7D1AA8F23D55C9784815489CF0A488B6F2C6D5C4C569AD51231FACB9920E5A763FE388247E03131D1601AC234E86BC0D266EB6A7", Description = "✅ Valid Request F +50" },
    new { DateOfBirth = "1990-01-01", Sex = "hombre", Token = "1D0B194488C091852597B9AF7D1AA8F23D55C9784815489CF0A488B6F2C6D5C4C569AD51231FACB9920E5A763FE388247E03131D1601AC234E86BC0D266EB6A7", Description = "✅ Valid Request Spanish Male" },
    new { DateOfBirth = "1990-01-01", Sex = "mujer", Token = "1D0B194488C091852597B9AF7D1AA8F23D55C9784815489CF0A488B6F2C6D5C4C569AD51231FACB9920E5A763FE388247E03131D1601AC234E86BC0D266EB6A7", Description = "✅ Valid Request Spanish Female" },
    new { DateOfBirth = "", Sex = "", Token = "1D0B194488C091852597B9AF7D1AA8F23D55C9784815489CF0A488B6F2C6D5C4C569AD51231FACB9920E5A763FE388247E03131D1601AC234E86BC0D266EB6A7", Description = "❌ Missing Both Parameters" },
    new { DateOfBirth = "1990-01-01", Sex = "", Token = "1D0B194488C091852597B9AF7D1AA8F23D55C9784815489CF0A488B6F2C6D5C4C569AD51231FACB9920E5A763FE388247E03131D1601AC234E86BC0D266EB6A7", Description = "❌ Missing Sex" },
    new { DateOfBirth = "", Sex = "female", Token = "1D0B194488C091852597B9AF7D1AA8F23D55C9784815489CF0A488B6F2C6D5C4C569AD51231FACB9920E5A763FE388247E03131D1601AC234E86BC0D266EB6A7", Description = "❌ Missing DOB" },
    new { DateOfBirth = "1990-01-41", Sex = "male", Token = "1D0B194488C091852597B9AF7D1AA8F23D55C9784815489CF0A488B6F2C6D5C4C569AD51231FACB9920E5A763FE388247E03131D1601AC234E86BC0D266EB6A7", Description = "❌ Invalid DOB" },
    new { DateOfBirth = "1990-01-01", Sex = "robot", Token = "1D0B194488C091852597B9AF7D1AA8F23D55C9784815489CF0A488B6F2C6D5C4C569AD51231FACB9920E5A763FE388247E03131D1601AC234E86BC0D266EB6A7", Description = "❌ Invalid Sex" },
    new { DateOfBirth = "1990-01-01", Sex = "male", Token = "WRONG-TOKEN", Description = "❌ Invalid Token" },
    new { DateOfBirth = "1990-01-01", Sex = "mujer", Token = "", Description = "❌ Invalid Token" },
};

foreach (var test in testCases)
{
    var dateOfBirthParam = Uri.EscapeDataString("date of birth") + "=" + Uri.EscapeDataString(test.DateOfBirth);
    var sexParam = "sex=" + Uri.EscapeDataString(test.Sex);

    var queryString = string.Join("&",
        new[] { dateOfBirthParam, sexParam }.Where(p => !p.EndsWith("=")));

    var url = $"{baseUrl}?{queryString}";
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