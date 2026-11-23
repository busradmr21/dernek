namespace SigortaDernegi.Models;

public class PressArticle
{
    public string Slug { get; set; } = string.Empty;
    public string ImageUrl { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Summary { get; set; } = string.Empty;
    public DateTime PublishedAt { get; set; }
    public IReadOnlyList<string> Paragraphs { get; set; } = Array.Empty<string>();
    public IReadOnlyList<PressSource> Sources { get; set; } = Array.Empty<PressSource>();
}

public class PressSource
{
    public string Name { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}
