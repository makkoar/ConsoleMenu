namespace Template;

public class Program
{
    public static Action Test(string text) => () =>
    {
        Console.WriteLine(text);
        Console.ReadKey();
    };

    public static bool IsNewMenu = true;

    private static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        Console.CursorVisible = false;

        Menu MainMenu = new Menu("Выберите элемент меню:", new MenuItem(Test("Метка 1")) { Text = "Метка 1" }, new("Метка 2", Test("Метка 2")), new("Метка 3", Test("Метка 3"))).AddMenuItem("Метка 4", Test("Метка 4")).AddMenuItem("Метка 5", Test("Метка 5"));
    MainMenu:
        if (IsNewMenu)
        {
            MainMenu.Apply();
            goto MainMenu;
        }
        else switch (MainMenu.GetIndex())
            {
                case 0: goto label1;
                case 1: goto label2;
                case 2: goto label3;
                case 3: goto label4;
                case 4: goto label5;
            };

    label1:
        Console.WriteLine("Метка 1!");
        Console.ReadKey();
        goto MainMenu;

    label2:
        Console.WriteLine("Метка 2!");
        Console.ReadKey();
        goto MainMenu;

    label3:
        Console.WriteLine("Метка 3!");
        Console.ReadKey();
        goto MainMenu;

    label4:
        Console.WriteLine("Метка 4!");
        Console.ReadKey();
        goto MainMenu;

    label5:
        Console.WriteLine("Метка 5!");
        Console.ReadKey();
        goto MainMenu;
    }
}