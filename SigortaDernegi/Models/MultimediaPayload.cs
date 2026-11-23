namespace SigortaDernegi.Models;

public class MultimediaPayload
{
    public string ContentType { get; set; } = "Press";
    public string Summary { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public string? SourceName { get; set; }
    public string? SourceUrl { get; set; }
}
