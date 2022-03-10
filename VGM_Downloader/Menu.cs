using ConsoleTables;
using VGM_Downloader.Models;

namespace VGM_Downloader;

public class Menu
{
    private readonly Scrapper _scrapper = new();
    private List<Game> _games = null!;
    private List<Game> _resultGames = null!;
    private List<Category>? _categories;

    private static void Loading()
    {
        Header();
        Console.WriteLine("\t  ==> Loading ... <==");
    }
    
    private static void Header(string error = "")
    {
        Console.Clear();
        Console.WriteLine("========================================");
        Console.WriteLine("|             VGM Downloader           |");
        Console.WriteLine("========================================");
        if (error != "")
        {
            Console.WriteLine($"\t --> {error}");
        }
    }

    private void ShowResult(int op, string error = "")
    {
        while (true)
        {
            Header(error);
            var table = new ConsoleTable("ID", "Game", "Song", "Sequenced By", "Size");
            foreach (var game in _resultGames)
            {
                foreach (var song in game.Songs)
                {
                    table.AddRow(_resultGames.IndexOf(game) + "-" + game.Songs.IndexOf(song), game.Name, song.Name, song.SequencedBy, song.Size);
                }
            }

            table.Write();
            Console.Write($"Enter the song id [b back]: ");
            var opSub = Console.ReadLine();
            if (opSub != null)
            {
                if (opSub is "b" or "B")
                {
                    SearchGame(op);
                }
                else
                {
                    var ids = opSub.Split("-");
                    if (ids.Length == 1)
                    {
                        error = "Invalid Option";
                        continue;
                    }

                    if (!ids[0].Trim().All(char.IsDigit) || !ids[1].Trim().All(char.IsDigit))
                    {
                        error = "Invalid Option";
                        continue;
                    }

                    if (int.Parse(ids[0].Trim()) <= _resultGames.Count - 1)
                    {
                        if (int.Parse(ids[1].Trim()) <= _resultGames[int.Parse(ids[0].Trim())].Songs.Count - 1)
                        {       
                            Loading();    
                            _scrapper.GetMidi(_resultGames[int.Parse(ids[0].Trim())]
                                .Songs[int.Parse(ids[1].Trim())]);
                            error = $"Download Completed: {_resultGames[int.Parse(ids[0].Trim())].Songs[int.Parse(ids[1].Trim())].Filename}";
                            continue;
                        }
                        else
                        {
                            error = "Invalid Option";
                            continue;
                        }
                    }
                    else
                    {
                        error = "Invalid Option";
                        continue; 
                    }
                }
            }
            else
            {
                error = "Invalid Option";
                continue;
            }

            break;
        }
    }

    private void SearchGame(int op, string error = "")
    {
        while (true)
        {
            Header(error);
            Console.Write("Enter game name [-b to go back]: ");
            var query = Console.ReadLine();
            if (query != null)
            {
                if (query is "-b" or "-B")
                {
                    SubCategories(op);
                }
                else
                {
                    _resultGames = null!;
                    _resultGames = _games.Where(m => m.Name.ToUpper().Contains(query.ToUpper())).ToList();
                    ShowResult(op);
                }
            }
            else
            {
                error = "Invalid Option";
                continue;
            }

            break;
        }
    }

    private void SubCategories(int op, string error = "")
    {
        while (true)
        {
            Header(error);
            foreach (var system in _categories?[op].Systens!)
            {
                Console.WriteLine($"[{_categories[op].Systens.IndexOf(system)}] {system.Name}");
            }
            Console.Write($"Select System [0-{_categories[op].Systens.Count - 1}] b back: ");
            var opSub = Console.ReadLine();
            if (opSub != null)
            {
                if (opSub.Trim().All(char.IsDigit))
                {
                    var opInt = int.Parse(opSub.Trim());
                    if (opInt <= _categories[op].Systens.Count - 1)
                    {
                        Loading();
                        _games = null!;
                        _games = _scrapper.GetGames(_categories[op].Systens[opInt].Link).Result;
                        SearchGame(op);
                    }
                    else
                    {
                        error = "Invalid Option";
                        continue;
                    }
                }
                else
                {
                    if(opSub is "b" or "B")
                    {
                        Categories();
                    }else
                    {
                        error = "Invalid Option";
                        continue;   
                    }
                }
            }

            break;
        }
    }

    public void Categories(string error = "")
    {
        while (true)
        {
            if (_categories == null)
            {
                Loading();
                _categories = _scrapper.GetCategories().Result;
            }

            Header(error);
            foreach (var category in _categories!)
            {
                Console.WriteLine($"[{_categories.IndexOf(category)}] {category.Name}");
            }

            Console.Write($"Select Category [0-{_categories.Count - 1}]: ");
            var op = Console.ReadLine();
            if (op != null)
            {
                if (op.Trim().All(char.IsDigit))
                {
                    var opInt = int.Parse(op.Trim());
                    if (opInt <= _categories.Count - 1)
                    {
                        SubCategories(opInt);
                    }
                    else
                    {
                        error = "Invalid Option";
                        continue;
                    }
                }
                else
                {
                    error = "Invalid Option";
                    continue;
                }
            }

            break;
        }
    }
}