namespace VGM_Downloader.Models;

public class Game
{
    public Game(string name, List<Song> songs)
    {
        Name = name;
        Songs = songs;
    }

    public string Name { get; set; }
    public List<Song> Songs { get; set; }
}