# SampleSecurityTest — Intentional Findings App

.NET Core Web API built **only for code-scanner accuracy testing**.  
It intentionally embeds **20 findings** across four scan engines.
TEst 3472384287482374

> **WARNING:** Do not deploy this application. It contains deliberate security flaws.

## Expected issue counts

| Engine | Count | Description |
|--------|------:|-------------|
| SAST (Security) | **5** | Injection, secrets, weak crypto, path traversal |
| Code Quality | **5** | Empty catch, unused vars, complexity, null deref, resource leak |
| Coding Standards | **5** | Public fields, bad naming, magic numbers, too many params |
| SCA (Dependencies) | **5** | Known-vulnerable NuGet packages |
| **Total** | **20** | |

---

## SAST — 5 issues (`UserController` / `OrderController`)

| # | Issue | Location | Pattern |
|---|--------|----------|---------|
| 1 | Hardcoded secrets | `UserController` — `ApiKey`, `AdminPassword`, connection string | Secret / credential in source |
| 2 | SQL Injection | `UserController.GetUser` | String-concatenated SQL |
| 3 | Command Injection | `UserController.SearchUsers` | User input in `cmd.exe` args |
| 4 | Path Traversal | `UserController.GetProfileFile` | Unvalidated file path |
| 5 | Weak cryptography | `UserController.HashPassword` | MD5 used for password hashing |

*(Bonus insecure pattern: SSRF via `WebClient.DownloadString(url)` in `OrderController.DownloadInvoice` — may be flagged by some SAST engines.)*

---

## Code Quality — 5 issues

| # | Issue | Location |
|---|--------|----------|
| 1 | Empty `catch` block | `UserController.ValidateUser` |
| 2 | Unused local variables | `UserController.ValidateUser` (`unusedCounter`, `unusedMessage`) |
| 3 | High cognitive / nesting complexity | `UserController.check_user_age` |
| 4 | Null dereference | `OrderController.GetOrder` (`_lastOrderId.Length` after null assign) |
| 5 | Resource leak (no Dispose) | `OrderController.ProcessOrder` (`FileStream` / `StreamWriter`) |

---

## Coding Standards — 5 issues

| # | Issue | Location |
|---|--------|----------|
| 1 | Public instance field | `UserController.ConnectionString` |
| 2 | Non-conventional field name | `UserController.Max_Retry_Count` |
| 3 | Method not PascalCase | `check_user_age` / `delete_order` |
| 4 | Magic numbers / public static fields | `OrderController.timeout`, `tax`, inline `15`, `250`, `10000` |
| 5 | Too many method parameters | `OrderController.ProcessOrder` (6 string params) |

---

## SCA — 5 vulnerable NuGet packages

Pinned in `SampleSecurityTest.csproj` with known advisories (ground truth = these **5**):

| # | Package | Version | Typical advisory class |
|---|---------|---------|------------------------|
| 1 | `Newtonsoft.Json` | 12.0.1 | DoS / insecure deserialization related |
| 2 | `System.Net.Http` | 4.3.3 | Security bypass / spoofing |
| 3 | `SharpZipLib` | 1.3.2 | Path traversal / DoS |
| 4 | `System.Drawing.Common` | 4.7.0 | DoS / info disclosure |
| 5 | `System.Text.RegularExpressions` | 4.3.0 | ReDoS |

`Microsoft.Data.SqlClient` is also referenced (patched) only so the SQL-injection SAST sample uses a real `SqlCommand` API — do **not** count it toward the SCA 5.

Verify locally:

```bash
dotnet list package --vulnerable
```

---

## Controllers

1. **`api/User`** — SAST-heavy + some quality/standards  
2. **`api/Order`** — Quality/standards-heavy + SSRF sample  

## Run

```bash
dotnet restore
dotnet run
```

## How to use for scanner testing

1. Point your SAST / quality / standards / SCA scanners at this repo.
2. Compare findings to the tables above.
3. Measure true positives, false negatives, and any unexpected extras.

Scanner rule IDs and severity labels differ by vendor; use the **pattern descriptions** (SQL injection, MD5, empty catch, etc.) as the ground truth, not exact CWE/rule numbers.
