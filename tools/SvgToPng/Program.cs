using SkiaSharp;
using Svg.Skia;

if (args.Length < 1)
{
    Console.WriteLine("SVG to PNG Converter");
    Console.WriteLine("Usage: SvgToPng <input.svg> [output.png] [size]");
    Console.WriteLine("  size: Output size in pixels (default: 128)");
    Console.WriteLine();
    Console.WriteLine("Example: SvgToPng icon.svg icon.png 128");
    return 1;
}

var inputPath = args[0];
var outputPath = args.Length > 1 ? args[1] : Path.ChangeExtension(inputPath, ".png");
var size = args.Length > 2 ? int.Parse(args[2]) : 128;

if (!File.Exists(inputPath))
{
    Console.WriteLine($"Error: File not found: {inputPath}");
    return 1;
}

try
{
    using var svg = new SKSvg();
    svg.Load(inputPath);

    if (svg.Picture == null)
    {
        Console.WriteLine("Error: Failed to load SVG");
        return 1;
    }

    // Calculate scale to fit target size
    var bounds = svg.Picture.CullRect;
    var scale = Math.Min(size / bounds.Width, size / bounds.Height);

    using var bitmap = new SKBitmap(size, size);
    using var canvas = new SKCanvas(bitmap);

    canvas.Clear(SKColors.Transparent);

    // Center the image
    var offsetX = (size - bounds.Width * scale) / 2;
    var offsetY = (size - bounds.Height * scale) / 2;

    canvas.Translate((float)offsetX, (float)offsetY);
    canvas.Scale((float)scale);
    canvas.DrawPicture(svg.Picture);

    using var image = SKImage.FromBitmap(bitmap);
    using var data = image.Encode(SKEncodedImageFormat.Png, 100);
    using var stream = File.OpenWrite(outputPath);
    data.SaveTo(stream);

    Console.WriteLine($"Converted: {inputPath} -> {outputPath} ({size}x{size})");
    return 0;
}
catch (Exception ex)
{
    Console.WriteLine($"Error: {ex.Message}");
    return 1;
}
