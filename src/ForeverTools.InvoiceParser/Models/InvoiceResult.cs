using System.Text.Json.Serialization;

namespace ForeverTools.InvoiceParser.Models;

/// <summary>
/// The result of an invoice parsing operation, containing all extracted fields.
/// </summary>
public class InvoiceResult
{
    /// <summary>Invoice number or identifier.</summary>
    [JsonPropertyName("invoice_number")]
    public string? InvoiceNumber { get; set; }

    /// <summary>Invoice date.</summary>
    [JsonPropertyName("date")]
    public string? Date { get; set; }

    /// <summary>Payment due date.</summary>
    [JsonPropertyName("due_date")]
    public string? DueDate { get; set; }

    /// <summary>Vendor / supplier information.</summary>
    [JsonPropertyName("vendor")]
    public string? Vendor { get; set; }

    /// <summary>Customer / buyer information.</summary>
    [JsonPropertyName("customer")]
    public string? Customer { get; set; }

    /// <summary>Line items listed on the invoice.</summary>
    [JsonPropertyName("line_items")]
    public List<InvoiceLineItem> LineItems { get; set; } = new();

    /// <summary>Subtotal before tax.</summary>
    [JsonPropertyName("subtotal")]
    public decimal? Subtotal { get; set; }

    /// <summary>Tax amount.</summary>
    [JsonPropertyName("tax")]
    public decimal? Tax { get; set; }

    /// <summary>Grand total including tax.</summary>
    [JsonPropertyName("total")]
    public decimal? Total { get; set; }

    /// <summary>Currency code (e.g. USD, EUR, GBP).</summary>
    [JsonPropertyName("currency")]
    public string? Currency { get; set; }
}
