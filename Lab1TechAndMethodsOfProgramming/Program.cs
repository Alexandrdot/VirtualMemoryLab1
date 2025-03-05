using System.Text;

public class Page
{
    public byte[] Str { get; set; }
    public bool Status { get; set; }
    public int NumberPage { get; set; }
    public Page(byte[] str, int number, bool status = false)
    {
        Str = str;
        NumberPage = number;
        Status = status;
    }
}

public class BigArray
{
    public static int SizePage { get; } = 1;
    public static int SizeArray { get; } = 2;
    public static List<Page> Pages { get; set; } = new();
    public static FileStream? Fstream { get; set; }
    public static void FirstWorkWithArray()
    {
        string path = "/Users/aleksandr/Desktop/test.txt";
        Fstream = new FileStream(path, FileMode.OpenOrCreate, FileAccess.ReadWrite);
        
        for (int i = 0; i < SizeArray; i++)
        {
            byte[] page = new byte[SizePage];
            Fstream.Seek(0, SeekOrigin.Current); //5 - отступ
            Fstream.Read(page, 0, page.Length);

            Pages.Add(new Page(page, i));
            //Console.WriteLine(Encoding.Default.GetString(Pages[i].Str));
        }
    }

    public static void UpdateArray(int index)
    {
        int numberPage = SearchNumberPage(index);
        foreach (Page o_o in Pages)
        {
            if (o_o.NumberPage == numberPage)
                return;
        }
    
        byte[] page = new byte[SizePage];
        Fstream?.Seek(0, SeekOrigin.Begin);
        Fstream?.Seek(numberPage * SizePage, SeekOrigin.Current); 
        Fstream?.Read(page, 0, page.Length);

        Page _page = new(page, numberPage); //страница, где лежит индекс
        //Console.WriteLine(numberPage);
        int summary_status = Pages.Count(o_0 => o_0.Status);
        if (summary_status == SizeArray) //если все изменены
        {
            Fstream?.Seek(0, SeekOrigin.Begin);
            Fstream?.Seek(Pages[0].NumberPage * SizePage, SeekOrigin.Current);
            Fstream?.Write(Pages[0].Str, 0, SizePage);
            Pages[0] = _page;
            Pages = Pages.OrderBy(x => x.NumberPage).ToList();
        }
        else if (summary_status > 0)
        {
            for (int i = 0; i < SizeArray; i++)
            {
                if (!Pages[i].Status)
                {

                    Pages[i] = _page;
                    Pages = Pages.OrderBy(x => x.NumberPage).ToList();
                    break;
                }
            }
        }
        else //если никто не изменен
        {

            Pages[0] = _page;
            Pages = Pages.OrderBy(x => x.NumberPage).ToList();
        }
        //for (int i = 0; i < SizeArray; i++)
        //{
        //    Console.WriteLine(Encoding.Default.GetString(Pages[i].Str));
        //    Console.WriteLine((Pages[i].NumberPage));
        //}
    }

    public static int SearchNumberPage(int index)
    {
        return index / SizePage;
    }

    public static (int, int) SearchIndex(int index)
    {
        foreach (var page in Pages)
        {
            int start = page.NumberPage * SizePage;
            int end = start + SizePage;

            if (index >= start && index < end)
            {
                return (Pages.IndexOf(page), index - start);
            }
        }
        return (-1, -1); // Если не нашли страницу
    }

    public static void PrintFoundElement(int index)
    {
       (int NumberPage, int NumberElement) foundInfo = SearchIndex(index);
        byte[] mybyte = new byte[1];
        mybyte[0] = Pages[foundInfo.NumberPage].Str[foundInfo.NumberElement];
        Console.Write("Найденный элемент: ");
        Console.WriteLine(Encoding.Default.GetString(mybyte));
    }

    public static void SetElement(int index)
    {
        (int NumberPage, int NumberElement) foundInfo = SearchIndex(index);
        PrintFoundElement(index);
        Console.Write("Введите новый элемент: ");
        char.TryParse(Console.ReadLine(), out char c);

        Pages[foundInfo.NumberPage].Str[foundInfo.NumberElement] = (byte) c; //меняем в массиве
        Pages[foundInfo.NumberPage].Status = true; 

        Console.WriteLine("Символ успешно изменен!");
    }
}

public class Program
{
    public static void Main()
    {
        BigArray.FirstWorkWithArray(); //подгрузка первых пяти страниц
        Console.ResetColor();
        do
        {
            Console.Write("Введите индекс элемента в файле: ");
            int.TryParse(Console.ReadLine(), out int index);
            BigArray.UpdateArray(index);
            Console.ForegroundColor = ConsoleColor.Yellow; // устанавливаем цвет

            Console.WriteLine("\nМеню:");
            Console.WriteLine("1. Посмотреть символ");
            Console.WriteLine("2. Изменить символ");
            //Console.WriteLine("3. Сохранить изменения");
            Console.Write("Выберите действие: ");

            string? choice = Console.ReadLine();
            Console.WriteLine();
            Console.ResetColor(); // сбрасываем в стандартный

            switch (choice) //что делаем
            {

                case "1":
                    BigArray.PrintFoundElement(index);
                    //...
                    break;

                case "2":
                    Console.WriteLine("\nЭлемент для изменения:");
                    BigArray.SetElement(index);

                    break;

                //case "3":
                //    //Console.WriteLine("\nЭлемент для изменения:");
                //    //BigArray.SetElement(index);

                //    break;


                default:
                    Console.WriteLine("Некорректный выбор. Пожалуйста, попробуйте снова.");
                    break;
            }
            Console.ForegroundColor = ConsoleColor.Yellow; // устанавливаем цвет
            Console.WriteLine("\nНажмите enter, чтобы продолжить");
            Console.WriteLine("Для выхода нажмите esc");
            Console.ResetColor(); // сбрасываем в стандартный
            
        } while (Console.ReadKey().Key != ConsoleKey.Escape); //реализовал выход по esc
    }
}
