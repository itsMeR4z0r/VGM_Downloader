namespace VGM_Downloader.Models;

public class Category
{
    public Category(string name, List<System> systens)
    {
        Name = name ?? throw new ArgumentNullException(nameof(name));
        Systens = systens ?? throw new ArgumentNullException(nameof(systens));
    }

    public string Name { get; }
    public List<System> Systens { get; }
}