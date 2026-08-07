using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace SampleSecurityTest.Controllers;

/// <summary>
/// Intentionally vulnerable controller for SAST / quality / standards scanner testing.
/// DO NOT use in production.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    // CODING STANDARD #1: public field should be private (SA1401 / CA1051)
    public string ConnectionString = "Server=localhost;Database=Users;User Id=sa;Password=P@ssw0rd!;";

    // SAST #1: Hardcoded credentials / secrets
    private const string ApiKey = "sk_live_51HqT2kL9pQmN8xYzAbCdEfGhIjKlMnOp";
    private const string AdminPassword = "Admin@12345!";

    // CODING STANDARD #2: field naming — should be camelCase with underscore or private PascalCase
    public int Max_Retry_Count = 3;

    [HttpGet("{id}")]
    public IActionResult GetUser(string id)
    {
        // SAST #2: SQL Injection — user input concatenated into SqlCommand
        var query = "SELECT * FROM Users WHERE UserId = '" + id + "'";
        using var connection = new SqlConnection(ConnectionString);
        using var command = new SqlCommand(query, connection);
        // Intentionally do not open/execute — pattern is enough for SAST; avoids needing a live DB
        return Ok(new { query, command.CommandText });
    }

    [HttpGet("search")]
    public IActionResult SearchUsers(string term)
    {
        // SAST #3: Command Injection — unsanitized input passed to shell
        var process = new Process();
        process.StartInfo.FileName = "cmd.exe";
        process.StartInfo.Arguments = "/c findstr " + term + " C:\\logs\\users.log";
        process.StartInfo.RedirectStandardOutput = true;
        process.StartInfo.UseShellExecute = false;
        process.Start();
        var output = process.StandardOutput.ReadToEnd();
        process.WaitForExit();
        return Ok(output);
    }

    [HttpGet("profile")]
    public IActionResult GetProfileFile(string fileName)
    {
        // SAST #4: Path Traversal — user-controlled path without validation
        var path = Path.Combine(@"C:\app\profiles\", fileName);
        var content = System.IO.File.ReadAllText(path);
        return Content(content);
    }

    [HttpPost("hash-password")]
    public IActionResult HashPassword([FromBody] string password)
    {
        // SAST #5: Weak cryptography — MD5 used for password hashing
        using var md5 = MD5.Create();
        var hashBytes = md5.ComputeHash(Encoding.UTF8.GetBytes(password ?? string.Empty));
        var hash = Convert.ToHexString(hashBytes);
        return Ok(new { hash, ApiKey, AdminPassword });
    }

    [HttpGet("validate")]
    public IActionResult ValidateUser(string userName, int age)
    {
        // CODE QUALITY #1: Empty catch block swallows all exceptions
        try
        {
            if (string.IsNullOrEmpty(userName))
            {
                throw new ArgumentException("userName required");
            }
        }
        catch
        {
        }

        // CODE QUALITY #2: Unused local variable
        var unusedCounter = 42;
        var unusedMessage = "this variable is never used";

        // CODING STANDARD #3: method name not PascalCase (IDE1006 / SA1300)
        return check_user_age(age);
    }

    // CODING STANDARD #3 continued: non-PascalCase public method
    [HttpGet("age-check")]
    public IActionResult check_user_age(int age)
    {
        // CODE QUALITY #3: High cognitive complexity — deeply nested conditionals
        if (age > 0)
        {
            if (age < 18)
            {
                if (age < 13)
                {
                    if (age < 5)
                    {
                        if (age < 1)
                        {
                            return BadRequest("invalid");
                        }
                        else
                        {
                            return Ok("toddler");
                        }
                    }
                    else
                    {
                        return Ok("child");
                    }
                }
                else
                {
                    return Ok("teen");
                }
            }
            else
            {
                if (age > 65)
                {
                    if (age > 80)
                    {
                        return Ok("senior-plus");
                    }
                    else
                    {
                        return Ok("senior");
                    }
                }
                else
                {
                    return Ok("adult");
                }
            }
        }

        return BadRequest("invalid age");
    }
}
