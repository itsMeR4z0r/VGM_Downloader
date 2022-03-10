using System.Net;
using AngleSharp;
using AngleSharp.Dom;
using VGM_Downloader.Models;

namespace VGM_Downloader;

public class Scrapper
{
    public async Task<List<Category>?> GetCategories()
    {
        var categories = new List<Category>();
        var config = Configuration.Default.WithDefaultLoader();
        using var context = BrowsingContext.New(config);

        var url = "https://www.vgmusic.com/";

        using var doc = await context.OpenAsync(url);
        var nodeSystens = doc.QuerySelector("#menu > form:nth-child(2) > select:nth-child(3)");
        foreach (var optGroup in nodeSystens?.QuerySelectorAll("optgroup").ToList()!)
        {
            var systens = optGroup.QuerySelectorAll("option").ToList().Select(option =>
                    new Models.System(option.Text().Trim(), option.Attributes.GetNamedItem("value")?.Value.Trim()!))
                .ToList();
            systens.Sort((x, y) => string.Compare(x.Name, y.Name, StringComparison.Ordinal));
            categories.Add(new Category(optGroup.Attributes.GetNamedItem("label")?.Value.Trim()!, systens));
            categories.Sort((x, y) => string.Compare(x.Name, y.Name, StringComparison.Ordinal));
        }

        return categories;
    }

    public async Task<List<Game>> GetGames(string url, string query = "")
    {
        var games = new List<Game>();
        var config = Configuration.Default.WithDefaultLoader();
        using var context = BrowsingContext.New(config);

        using var doc = await context.OpenAsync(url);

        var nodeTable = doc.QuerySelector("body > table:nth-child(7) > tbody");

        var songs = new List<Song>();

        IElement gameName = null!;

        foreach (var tds in (nodeTable?.QuerySelectorAll("tr").ToList() ?? throw new InvalidOperationException())
                 .Select(tr => tr.QuerySelectorAll("td")))
        {
            switch (tds.Length)
            {
                case 1 when tds[0].Text().Trim() != "":
                {
                    if (songs.Count > 0)
                    {
                        songs.Sort((x, y) => string.Compare(x.Name, y.Name, StringComparison.Ordinal));
                        games.Add(new Game(gameName.Text().Trim(), songs));
                        songs = new List<Song>();
                    }

                    gameName = tds[0];
                    break;
                }
                case > 1:
                {
                    var song = new Song(tds[0].Text().Trim(), tds[2].Text().Trim(), tds[1].Text().Trim(),
                        tds[0].Children[0].Attributes.GetNamedItem("href")?.Value.Trim()!,
                        url + tds[0].Children[0].Attributes.GetNamedItem("href")?.Value.Trim());
                    songs.Add(song);
                    break;
                }
            }
        }

        games.Add(new Game(gameName.Text().Trim(), songs));
        games.Sort((x, y) => string.Compare(x.Name, y.Name, StringComparison.Ordinal));
        if (query.Trim() != "")
        {
            games = games.Where(m => m.Name.ToUpper().Contains(query.ToUpper())).ToList();
        }

        return games;
    }

    public Task GetMidi(Song song)
    {
        var downloadPath =
            Path.GetFullPath(AppDomain.CurrentDomain.BaseDirectory + Path.DirectorySeparatorChar + "Downloads" +
                             Path.DirectorySeparatorChar);
        if (!Directory.Exists(downloadPath))
        {
            Directory.CreateDirectory(downloadPath);
        }

        if (File.Exists(downloadPath + song.Filename))
        {
            File.Delete(downloadPath + song.Filename);
        }
        WebClient webClient = new WebClient();
        webClient.DownloadFile(song.Link, downloadPath + song.Filename);
        return Task.CompletedTask;
    }
}