namespace VGM_Downloader.Models;

public class Song
{
    public Song(string name, string sequencedBy, string size, string filename, string link)
    {
        Name = name;
        SequencedBy = sequencedBy;
        Size = size;
        Filename = filename;
        Link = link;
    }

    public string Name { get; set; }
    public string SequencedBy { get; set; }
    public string Size { get; set; }
    public string Filename { get; set; }
    public string Link { get; set; }
}