using System.Text.Json.Serialization;

namespace ForeverTools.InvoiceParser.Models;

/// <summary>
/// Represents a single line item on a parsed invoice.
/// </summary>
public class InvoiceLineItem
{
    /// <summary>Description of the product or service.</summary>
    [JsonPropertyName("description")]
    public string Description { get; set; } = string.Empty;

    /// <summary>Quantity of units.</summary>
    [JsonPropertyName("quantity")]
    public decimal? Quantity { get; set; }

    /// <summary>Unit price per item.</summary>
    [JsonPropertyName("unit_price")]
    public decimal? UnitPrice { get; set; }

    /// <summary>Total cost for this line item (quantity × unit price).</summary>
    [JsonPropertyName("total")]
    public decimal? Total { get; set; }
}
