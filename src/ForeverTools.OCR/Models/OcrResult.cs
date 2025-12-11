using System.Text.Json.Serialization;

namespace ForeverTools.OCR.Models;

/// <summary>
/// Result of an OCR text extraction operation.
/// </summary>
public class OcrResult
{
    /// <summary>
    /// Whether the extraction was successful.
    /// </summary>
    public bool Success { get; set; }

    /// <summary>
    /// The extracted text content.
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// The model used for extraction.
    /// </summary>
    public string Model { get; set; } = string.Empty;

    /// <summary>
    /// Number of tokens used in the request.
    /// </summary>
    public int TokensUsed { get; set; }

    /// <summary>
    /// Processing time in milliseconds.
    /// </summary>
    public long ProcessingTimeMs { get; set; }

    /// <summary>
    /// Error message if extraction failed.
    /// </summary>
    public string? Error { get; set; }

    /// <summary>
    /// Confidence level of the extraction (0.0 to 1.0).
    /// Note: This is estimated based on model response, not a precise metric.
    /// </summary>
    public double? Confidence { get; set; }

    /// <summary>
    /// Creates a successful result.
    /// </summary>
    public static OcrResult Successful(string text, string model, int tokensUsed, long processingTimeMs)
    {
        return new OcrResult
        {
            Success = true,
            Text = text,
            Model = model,
            TokensUsed = tokensUsed,
            ProcessingTimeMs = processingTimeMs
        };
    }

    /// <summary>
    /// Creates a failed result.
    /// </summary>
    public static OcrResult Failed(string error, string model = "")
    {
        return new OcrResult
        {
            Success = false,
            Error = error,
            Model = model
        };
    }
}

/// <summary>
/// Result of structured text extraction with layout information.
/// </summary>
public class StructuredOcrResult : OcrResult
{
    /// <summary>
    /// Text blocks extracted from the image with position information.
    /// </summary>
    public List<TextBlock> Blocks { get; set; } = new();

    /// <summary>
    /// Paragraphs detected in the document.
    /// </summary>
    public List<string> Paragraphs { get; set; } = new();

    /// <summary>
    /// Individual lines of text.
    /// </summary>
    public List<string> Lines { get; set; } = new();
}

/// <summary>
/// A block of text with optional position information.
/// </summary>
public class TextBlock
{
    /// <summary>
    /// The text content of this block.
    /// </summary>
    public string Text { get; set; } = string.Empty;

    /// <summary>
    /// Type of text block (paragraph, heading, list, etc.).
    /// </summary>
    public string BlockType { get; set; } = "paragraph";

    /// <summary>
    /// Confidence score for this block (0.0 to 1.0).
    /// </summary>
    public double? Confidence { get; set; }

    /// <summary>
    /// Reading order index (0 = first).
    /// </summary>
    public int Order { get; set; }
}

/// <summary>
/// Result of table extraction from an image.
/// </summary>
public class TableOcrResult : OcrResult
{
    /// <summary>
    /// Tables extracted from the image.
    /// </summary>
    public List<ExtractedTable> Tables { get; set; } = new();
}

/// <summary>
/// A table extracted from an image.
/// </summary>
public class ExtractedTable
{
    /// <summary>
    /// Table headers (first row).
    /// </summary>
    public List<string> Headers { get; set; } = new();

    /// <summary>
    /// Table rows (excluding headers).
    /// </summary>
    public List<List<string>> Rows { get; set; } = new();

    /// <summary>
    /// Number of columns in the table.
    /// </summary>
    public int ColumnCount => Headers.Count > 0 ? Headers.Count : (Rows.Count > 0 ? Rows[0].Count : 0);

    /// <summary>
    /// Number of rows (excluding headers).
    /// </summary>
    public int RowCount => Rows.Count;

    /// <summary>
    /// Gets the table as a CSV string.
    /// </summary>
    public string ToCsv()
    {
        var lines = new List<string>();

        if (Headers.Count > 0)
        {
            lines.Add(string.Join(",", Headers.Select(EscapeCsvField)));
        }

        foreach (var row in Rows)
        {
            lines.Add(string.Join(",", row.Select(EscapeCsvField)));
        }

        return string.Join(Environment.NewLine, lines);
    }

    private static string EscapeCsvField(string field)
    {
        if (field.Contains(",") || field.Contains("\"") || field.Contains("\n"))
        {
            return $"\"{field.Replace("\"", "\"\"")}\"";
        }
        return field;
    }
}

/// <summary>
/// Result of form field extraction.
/// </summary>
public class FormOcrResult : OcrResult
{
    /// <summary>
    /// Form fields extracted from the document.
    /// </summary>
    public Dictionary<string, string> Fields { get; set; } = new();

    /// <summary>
    /// Gets a field value by key (case-insensitive).
    /// </summary>
    public string? GetField(string key)
    {
        var match = Fields.FirstOrDefault(f =>
            f.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
        return match.Key != null ? match.Value : null;
    }
}

/// <summary>
/// Result of receipt/invoice extraction.
/// </summary>
public class ReceiptOcrResult : OcrResult
{
    /// <summary>
    /// Merchant/vendor name.
    /// </summary>
    public string? MerchantName { get; set; }

    /// <summary>
    /// Transaction date.
    /// </summary>
    public string? Date { get; set; }

    /// <summary>
    /// Total amount.
    /// </summary>
    public string? Total { get; set; }

    /// <summary>
    /// Subtotal (before tax).
    /// </summary>
    public string? Subtotal { get; set; }

    /// <summary>
    /// Tax amount.
    /// </summary>
    public string? Tax { get; set; }

    /// <summary>
    /// Currency code (USD, EUR, etc.).
    /// </summary>
    public string? Currency { get; set; }

    /// <summary>
    /// Line items on the receipt.
    /// </summary>
    public List<ReceiptLineItem> Items { get; set; } = new();

    /// <summary>
    /// Payment method used.
    /// </summary>
    public string? PaymentMethod { get; set; }
}

/// <summary>
/// A line item from a receipt.
/// </summary>
public class ReceiptLineItem
{
    /// <summary>
    /// Item description/name.
    /// </summary>
    public string Description { get; set; } = string.Empty;

    /// <summary>
    /// Quantity purchased.
    /// </summary>
    public string? Quantity { get; set; }

    /// <summary>
    /// Unit price.
    /// </summary>
    public string? UnitPrice { get; set; }

    /// <summary>
    /// Total price for this line.
    /// </summary>
    public string? TotalPrice { get; set; }
}
