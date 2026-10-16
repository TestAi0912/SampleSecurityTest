using System;
using Microsoft.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.Serialization.Formatters.Binary;
using System.Security.Cryptography;
using System.Text;

namespace CodeFxTest
{
    public class VulnerableCode
    {
        // 1. SQL Injection
        public string GetUser(string username)
        {
            using var connection =
                new SqlConnection("Server=localhost;Database=Users;Trusted_Connection=True;");

            connection.Open();

            string query =
                "SELECT * FROM Users WHERE Username = '" + username + "'";

            using var command = new SqlCommand(query, connection);

            return command.ExecuteScalar()?.ToString();
        }


        // 2. Command Injection
        public void PingHost(string host)
        {
            var process = new Process();

            process.StartInfo.FileName = "cmd.exe";
            process.StartInfo.Arguments = "/c ping " + host;
            process.StartInfo.UseShellExecute = false;

            process.Start();
        }


        // 3. Hardcoded Secret
        private const string ApiKey =
            "sk_test_123456789abcdef123456";


        // 4. Hardcoded Password
        public void ConnectDatabase()
        {
            string username = "admin";
            string password = "Admin@123";

            Console.WriteLine(
                $"Connecting using {username}:{password}");
        }


        // 5. Weak MD5 Hashing
        public string HashPassword(string password)
        {
            using var md5 = MD5.Create();

            byte[] input = Encoding.UTF8.GetBytes(password);
            byte[] hash = md5.ComputeHash(input);

            return Convert.ToHexString(hash);
        }


        // 6. Weak SHA1 Hashing
        public string GenerateHash(string value)
        {
            using var sha1 = SHA1.Create();

            byte[] bytes = Encoding.UTF8.GetBytes(value);
            byte[] hash = sha1.ComputeHash(bytes);

            return Convert.ToHexString(hash);
        }


        // 7. Path Traversal
        public string ReadUserFile(string fileName)
        {
            string path = "/var/app/uploads/" + fileName;

            return File.ReadAllText(path);
        }


        // 8. SSRF
        public string FetchUrl(string url)
        {
            using var client = new HttpClient();

            return client.GetStringAsync(url)
                         .GetAwaiter()
                         .GetResult();
        }


        // 9. Missing HTTP Timeout
        public async Task<string> CallExternalService(string url)
        {
            using var client = new HttpClient();

            return await client.GetStringAsync(url);
        }


        // 10. Unsafe Deserialization
        public object DeserializeData(byte[] data)
        {
#pragma warning disable SYSLIB0011
            using var stream = new MemoryStream(data);

            var formatter = new BinaryFormatter();

            return formatter.Deserialize(stream);
#pragma warning restore SYSLIB0011
        }


        // 11. Code Injection using dynamic compilation/evaluation concept
        public object ExecuteExpression(string expression)
        {
            // Example dangerous pattern for scanner testing.
            return System.Data.DataTable
                .Compute(expression, null);
        }


        // 12. Sensitive Information Logging
        public void Login(string username, string password)
        {
            Console.WriteLine(
                $"User login: {username}, password: {password}");
        }


        // 13. Empty Catch / Swallowed Exception
        public string ReadFile(string file)
        {
            try
            {
                return File.ReadAllText(file);
            }
            catch (Exception)
            {
                return null;
            }
        }


        // 14. Broad Exception Handling
        public void ProcessPayment()
        {
            try
            {
                ProcessTransaction();
            }
            catch (Exception)
            {
                Console.WriteLine("Payment failed");
            }
        }


        private void ProcessTransaction()
        {
            throw new Exception("Transaction failed");
        }


        // 15. Weak Randomness
        public int GenerateOtp()
        {
            Random random = new Random();

            return random.Next(100000, 999999);
        }


        // 16. Insecure TLS Certificate Validation
        public void DisableCertificateValidation()
        {
            ServicePointManager.ServerCertificateValidationCallback =
                (sender, certificate, chain, sslPolicyErrors) => true;
        }


        // 17. SQL Connection String with Credentials
        public void OpenConnection()
        {
            string connectionString =
                "Server=localhost;" +
                "Database=Bank;" +
                "User Id=admin;" +
                "Password=Password123;";

            using var connection =
                new SqlConnection(connectionString);

            connection.Open();
        }


        // 18. High Cyclomatic Complexity
        public string GetUserType(
            int age,
            string country,
            bool isAdmin,
            bool isActive)
        {
            if (age > 18)
            {
                if (country == "US")
                {
                    if (isAdmin)
                    {
                        if (isActive)
                        {
                            return "ADMIN_ACTIVE";
                        }
                        else
                        {
                            return "ADMIN_INACTIVE";
                        }
                    }
                    else
                    {
                        if (isActive)
                        {
                            return "USER_ACTIVE";
                        }
                        else
                        {
                            return "USER_INACTIVE";
                        }
                    }
                }
                else if (country == "IN")
                {
                    if (isAdmin)
                    {
                        return "INDIA_ADMIN";
                    }
                    else
                    {
                        return "INDIA_USER";
                    }
                }
                else
                {
                    if (isActive)
                    {
                        return "OTHER_ACTIVE";
                    }
                    else
                    {
                        return "OTHER_INACTIVE";
                    }
                }
            }

            return "MINOR";
        }


        // 19. Too Many Parameters
        public void CreateCustomer(
            string firstName,
            string lastName,
            string email,
            string phone,
            string address,
            string city,
            string state,
            string country,
            string zipCode,
            int age)
        {
            Console.WriteLine(
                $"{firstName} {lastName} {email} {phone}");
        }


        // 20. Inefficient Nested Loops
        public bool ContainsDuplicate(int[] numbers)
        {
            for (int i = 0; i < numbers.Length; i++)
            {
                for (int j = i + 1; j < numbers.Length; j++)
                {
                    if (numbers[i] == numbers[j])
                    {
                        return true;
                    }
                }
            }

            return false;
        }


        // 21. Unused Variable / Code Smell
        public int CalculateTotal(int[] values)
        {
            int total = 0;

            string unusedVariable = "test";

            foreach (int value in values)
            {
                total += value;
            }

            return total;
        }


        // 22. Magic Numbers
        public bool IsEligible(int age)
        {
            if (age >= 18 && age <= 65)
            {
                return true;
            }

            return false;
        }


        // 23. Duplicate Code
        public void CreateUser(string name)
        {
            Console.WriteLine("Validating user");
            Console.WriteLine("Saving user");
            Console.WriteLine("Sending notification");
        }


        public void CreateAdmin(string name)
        {
            Console.WriteLine("Validating user");
            Console.WriteLine("Saving user");
            Console.WriteLine("Sending notification");
        }


        // 24. Very Long Method / Code Smell
        public void ProcessOrder()
        {
            Console.WriteLine("Validate order");
            Console.WriteLine("Validate customer");
            Console.WriteLine("Validate address");
            Console.WriteLine("Calculate price");
            Console.WriteLine("Calculate tax");
            Console.WriteLine("Calculate discount");
            Console.WriteLine("Check inventory");
            Console.WriteLine("Reserve inventory");
            Console.WriteLine("Create payment");
            Console.WriteLine("Process payment");
            Console.WriteLine("Create invoice");
            Console.WriteLine("Send email");
            Console.WriteLine("Send notification");
            Console.WriteLine("Update database");
            Console.WriteLine("Write audit log");
            Console.WriteLine("Complete order");
        }


        // 25. Hardcoded Sensitive URL
        public string GetAdminService()
        {
            return "https://internal-admin.company.com/api/users";
        }
    }
}
