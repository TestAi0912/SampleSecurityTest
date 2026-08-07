using System.Net;
using Microsoft.AspNetCore.Mvc;

namespace SampleSecurityTest.Controllers;

/// <summary>
/// Intentionally flawed controller for quality / standards / remaining SAST patterns.
/// DO NOT use in production.
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    // CODING STANDARD #4: magic numbers without named constants (S1192 / S109 style)
    // Also public mutable static field (standards)
    public static int timeout = 5000;
    public static double tax = 0.18;

    private string? _lastOrderId;

    [HttpGet("{orderId}")]
    public IActionResult GetOrder(string orderId)
    {
        // CODE QUALITY #4: Possible null dereference without guard
        _lastOrderId = null;
        var length = _lastOrderId.Length;

        // Dead / unreachable code after early return — quality smell
        if (string.IsNullOrEmpty(orderId))
        {
            return BadRequest();
            var neverReached = orderId.ToUpper();
            Console.WriteLine(neverReached);
        }

        // Magic numbers used inline
        var fee = orderId.Length * 15 + 250;
        if (fee > 10000)
        {
            fee = 10000;
        }

        return Ok(new { orderId, fee, length, timeout, tax });
    }

    [HttpGet("download")]
    public IActionResult DownloadInvoice(string url)
    {
        // Remaining insecure pattern often flagged: SSRF via user-controlled URL
        // (counts toward SAST-style findings if engine maps it; also quality smell)
        using var client = new WebClient();
        var data = client.DownloadString(url);
        return Content(data);
    }

    [HttpPost("process")]
    public IActionResult ProcessOrder(string customerId, string productId, string warehouseId, string coupon, string notes, string region)
    {
        // CODING STANDARD #5: too many parameters (S107) — should be a request DTO
        // CODE QUALITY #5: resource leak — IDisposable not disposed / no using
        var stream = new FileStream(@"C:\temp\orders.log", FileMode.Append, FileAccess.Write);
        var writer = new StreamWriter(stream);
        writer.WriteLine($"{customerId}|{productId}|{warehouseId}|{coupon}|{notes}|{region}");
        writer.Flush();
        // intentionally missing Dispose/Close

        var total = CalculateTotal(100, 2);
        return Ok(new { total, customerId, productId });
    }

    [HttpGet("total")]
    public IActionResult Calculate(int unitPrice, int qty)
    {
        return Ok(CalculateTotal(unitPrice, qty));
    }

    // Duplicate logic / copy-paste quality smell + non-static method that could be static
    private int CalculateTotal(int unitPrice, int qty)
    {
        var sub = unitPrice * qty;
        var withTax = (int)(sub + sub * tax);
        return withTax + 250;
    }

    private int CalculateTotalCopy(int unitPrice, int qty)
    {
        // CODE QUALITY: duplicated method body (copy-paste)
        var sub = unitPrice * qty;
        var withTax = (int)(sub + sub * tax);
        return withTax + 250;
    }

    [HttpDelete("{id}")]
    public IActionResult delete_order(string id)
    {
        // CODING STANDARD: non-PascalCase action name
        // Empty method body / incomplete implementation — quality
        if (id == null)
            return NotFound();

        return NoContent();
    }
}
