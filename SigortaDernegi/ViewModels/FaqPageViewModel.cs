namespace SigortaDernegi.ViewModels;

public class FaqPageViewModel
{
    public string HeroEyebrow { get; set; } = "Merak Ettikleriniz";
    public string HeroTitle { get; set; } = "Sıkça Sorulan Sorular";
    public string HeroDescription { get; set; } = "Üyelik, etkinlikler ve dernek çalışmaları hakkında sık sorulan soruların yanıtlarını inceleyin.";
    public IReadOnlyList<FaqItemViewModel> Items { get; set; } = Array.Empty<FaqItemViewModel>();
}

public class FaqItemViewModel
{
    public int Id { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Question { get; set; } = string.Empty;
    public string Answer { get; set; } = string.Empty;
}
