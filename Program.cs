using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;
using System.Text.RegularExpressions;

public class Composition
{
    public string Title { get; set; }
    public string Genre { get; set; }
    public string Artist { get; set; }
    public string Lyrics { get; set; }
    public DateTime CreationDate { get; set; }
    public double Duration { get; set; }
    public string Format { get; set; }
    public Dictionary<string, double> Ratings { get; set; } = new Dictionary<string, double>();

    public void AddRating(string propertyName, double value)
    {
        Ratings[propertyName] = value;
    }

    public double GetAverageRating()
    {
        if (Ratings.Count == 0)
        {
            return 0;
        }

        return Ratings.Values.Average();
    }

    public override string ToString()
    {
        return $"Composition(Назва='{Title}', Жанр='{Genre}', Артист='{Artist}', , Текст:='{Lyrics}', " +
            $"Дата створення='{CreationDate}', Тривалiсть={Duration}, Формат='{Format}', " +
            $"Рейтинг={GetAverageRating()})";
    }
}

public class MyLinkedList<T> : List<T>
{
    public void RemoveIf(Predicate<T> predicate)
    {
        this.RemoveAll(predicate);
    }
}

class Program
{
    private static MyLinkedList<Composition> compositions = new MyLinkedList<Composition>();
    private const string FileName = "compositions.json";

    static void Main(string[] args)
    {
        LoadCompositions();

        while (true)
        {
            Console.WriteLine("Меню:");
            Console.WriteLine("1. Додайти нову композицiю");
            Console.WriteLine("2. Переглянути список композицiй");
            Console.WriteLine("3. Знайти пiсню");
            Console.WriteLine("4. Редагувати композицiю");
            Console.WriteLine("5. Сортувати композицiї за назвою");
            Console.WriteLine("6. Сортувати композицiї за виконавцем");
            Console.WriteLine("7. Сортувати композицiї за середнiм рейтингом");
            Console.WriteLine("8. Видалити композицiю");
            Console.WriteLine("9. Вийти з програми");
            Console.Write("Виберiть опцiю: ");

            int choice = int.Parse(Console.ReadLine());

            switch (choice)
            {
                case 1:
                    AddComposition();
                    break;

                case 2:
                    DisplayCompositions();
                    break;

                case 3:
                    Search();
                    break;

                case 4:
                    EditComposition();
                    break;

                case 5:
                    compositions.Sort((c1, c2) => string.Compare(c1.Title, c2.Title, StringComparison.OrdinalIgnoreCase));
                    Console.WriteLine("Композицiї вiдсортованi за назвою.");
                    break;

                case 6:
                    compositions.Sort((c1, c2) => string.Compare(c1.Artist, c2.Artist, StringComparison.OrdinalIgnoreCase));
                    Console.WriteLine("Композицiї вiдсортованi за виконавцями.");
                    break;

                case 7:
                    compositions.Sort((c1, c2) => c2.GetAverageRating().CompareTo(c1.GetAverageRating()));
                    Console.WriteLine("Композицiї вiдсортованi за середнiм рейтингом (у порядку спадання).");
                    break;

                case 8:
                    RemoveComposition();
                    break;

                case 9:
                    SaveCompositions();
                    Console.WriteLine("Програму припинено.");
                    return;

                default:
                    Console.WriteLine("Невiрний вибiр. Спробуйте знову.");
                    break;
            }
        }
    }

    private static void RemoveComposition()
    {
        Console.WriteLine("Введiть назву композицiї, яку бажаєте видалити: ");
        string compositionTitle = Console.ReadLine();


        compositions.RemoveIf(composition => composition.Title.Equals(compositionTitle, StringComparison.OrdinalIgnoreCase));

        Console.WriteLine($"Композицiя з назвою '{compositionTitle}' видалена.");
    }

    private static void EditComposition()
    {
        Console.Write("Введiть назву композицiї, яку бажаєте вiдредагувати: ");
        string compositionTitle = Console.ReadLine();

        Composition compositionToEdit = compositions.FirstOrDefault(composition => composition.Title.Equals(compositionTitle, StringComparison.OrdinalIgnoreCase));

        if (compositionToEdit == null)
        {
            Console.WriteLine($"Композицiя з назвою '{compositionTitle}' не знайдена.");
            return;
        }

        Console.WriteLine("Введiть новi данi для композицiї:");

        Console.Write("Назва: ");
        compositionToEdit.Title = Console.ReadLine();

        Console.Write("Жанр: ");
        compositionToEdit.Genre = Console.ReadLine();

        Console.Write("Артист: ");
        compositionToEdit.Artist = Console.ReadLine();

        Console.Write("Текст: ");
        compositionToEdit.Lyrics = Console.ReadLine();

        Console.Write("Дата створення (YYYY-MM-DD): ");
        DateTime creationDate;
        while (!DateTime.TryParse(Console.ReadLine(), out creationDate))
        {
            Console.Write("Недiйсний формат дати. Будь ласка, використовуйте YYYY-MM-DD: ");
        }
        compositionToEdit.CreationDate = creationDate;

        Console.Write("Тривалiсть (у хвилинах): ");
        double duration;
        while (!double.TryParse(Console.ReadLine(), out duration))
        {
            Console.Write("Недiйсний формат тривалостi. Введiть дiйсний номер: ");
        }
        compositionToEdit.Duration = duration;

        Console.Write("Формат: ");
        compositionToEdit.Format = Console.ReadLine();
        Console.Write("Введiть новий рейтинг: ");
        double rating;
        while (!double.TryParse(Console.ReadLine(), out rating))
        {
            Console.Write("Недiйсний формат оцiнки. Введiть дiйсний номер: ");
        }
        compositionToEdit.Ratings["rating"] = rating;

        Console.WriteLine("Композицiю вiдредаговано.");
    }
    

    private static void LoadCompositions()
    {
        if (File.Exists(FileName))
        {
            try
            {
                var json = File.ReadAllText(FileName);
                compositions = JsonSerializer.Deserialize<MyLinkedList<Composition>>(json);
                Console.WriteLine("Данi успiшно завантажено з compositions.json");
            }
            catch (Exception e)
            {
                Console.WriteLine($"Помилка пiд час завантаження даних iз compositions.json: {e.Message}.");
            }
        }
        else
        {
            Console.WriteLine("compositions.json не знайдено.");
        }
    }

    private static void AddComposition()
    {
        Console.WriteLine("Введiть деталi композицiї:");

        Console.Write("Назва: ");
        var title = Console.ReadLine();

        Console.Write("Жанр: ");
        var genre = Console.ReadLine();

        Console.Write("Артист: ");
        var artist = Console.ReadLine();

        Console.Write("Текст: ");
        var lyrics = Console.ReadLine();

        Console.Write("Дата створення (YYYY-MM-DD): ");
        DateTime creationDate;
        while (!DateTime.TryParse(Console.ReadLine(), out creationDate))
        {
            Console.Write("Недiйсний формат дати. Будь ласка, використовуйте YYYY-MM-DD: ");
        }

        Console.Write("Тривалiсть (у хвилинах): ");
        double duration;
        while (!double.TryParse(Console.ReadLine(), out duration))
        {
            Console.Write("Недiйсний формат тривалостi. Введiть дiйсний номер: ");
        }

        Console.Write("Формат: ");
        var format = Console.ReadLine();

        var composition = new Composition
        {
            Title = title,
            Genre = genre,
            Artist = artist,
            Lyrics = lyrics,
            CreationDate = creationDate,
            Duration = duration,
            Format = format
        };

        Console.Write("Введiть рейтинг: ");
        var propertyName = "rating";

        double rating;
        while (!double.TryParse(Console.ReadLine(), out rating))
        {
            Console.Write("Недiйсний формат оцiнки. Введiть дiйсний номер: ");
        }

        composition.AddRating(propertyName, rating);

        compositions.Add(composition);
        Console.WriteLine("Композицiя додана.");
    }

    private static void DisplayCompositions()
    {
        foreach (var composition in compositions)
        {
            Console.WriteLine(composition);
        }
    }

    private static void Search()
    {
            Console.Write("Введiть назву пісні або текст для пошуку : ");
            string searchPattern = Console.ReadLine();

            Console.WriteLine($"Список композицiй, якi мiстять слова, пов'язанi iз '{searchPattern}':");

            foreach (var composition in compositions)
            {
                if (Regex.IsMatch(composition.Title, searchPattern, RegexOptions.IgnoreCase) || Regex.IsMatch(composition.Lyrics, searchPattern, RegexOptions.IgnoreCase))
                {
                    Console.WriteLine(composition);
                }
            }
        
    }

    private static void SaveCompositions()
    {
        var json = JsonSerializer.Serialize(compositions, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(FileName, json);
    }
}
