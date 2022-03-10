namespace VGM_Downloader.Models;

public class System
{
    public System(string name, string link)
    {
        Name = name;
        Link = link;
    }

    public string Name { get; set; }

    public string Link { get; set; }
}