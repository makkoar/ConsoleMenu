namespace ConsoleMenu;
/// <summary>Класс, отвечающий за консольное меню.</summary>
public class Menu
{
    #region Поля
    /// <summary>Темы, которые будут применены к данному меню.</summary>
    public Themes Themes { get; set; } = new();
    /// <summary>Заголовок меню.</summary>
    public string Title { get; set; } = "Выбирите пункт меню:";
    /// <summary>Элементы меню.</summary>
    public List<MenuItem> MenuItems { get; set; } = new();
    #endregion

    #region Конструкторы
    public Menu() { }
    public Menu(params MenuItem[] menuItems) : this()
    {
        for (ushort i = 0; i < menuItems.Length; i++)
            MenuItems.Add(menuItems[i]);
    }
    public Menu(string title, params MenuItem[] menuItems) : this(menuItems) => Title = title;
    public Menu(params string[] menuItems) : this()
    {
        for (ushort i = 0; i < menuItems.Length; i++)
            MenuItems.Add(new(menuItems[i], null));
    }
    public Menu(string title, params string[] menuItems) : this(menuItems) => Title = title;
    #endregion

    #region Функции активации меню
    /// <summary>Функция для получения выбранного элемента меню.</summary>
    /// <param name="startIndex">Начальное значение.</param>
    /// <returns>Выбранный элемента меню.</returns>
    public ushort GetIndex(ushort startIndex = 0)
    {
        ushort selected = startIndex;
        while (true)
        {
            Console.Clear();
            Themes.Title.Apply();
            Console.WriteLine(Title.PadRight(64));
            for (ushort i = 0; i < MenuItems.Count; i++)
            {
                if (i == selected) Themes.Selected.Apply();
                Console.WriteLine($"* {MenuItems[i].Text}".PadRight(64));
                Themes.Unselected.Apply();
            }
            switch (Console.ReadKey().Key)
            {
                case ConsoleKey.UpArrow: selected = (ushort)(selected != 0 ? selected - 1 : MenuItems.Count - 1); break;
                case ConsoleKey.DownArrow: selected = (ushort)(selected != MenuItems.Count - 1 ? selected + 1 : 0); break;
                case ConsoleKey.Enter: Console.Clear(); return selected;
                case ConsoleKey.Escape: Process.GetCurrentProcess().Kill(); break;
            }
        }
    }

    /// <summary>Применяет текущее меню.</summary>
    /// <param name="startIndex">Начальное значение.</param>
    public void Apply(ushort startIndex = 0)
    {
        ushort selected = startIndex;
        while (true)
        {
            Console.Clear();
            Themes.Title.Apply();
            Console.WriteLine(Title.PadRight(64));
            for (ushort i = 0; i < MenuItems.Count; i++)
            {
                if (i == selected) Themes.Selected.Apply();
                Console.WriteLine($"* {MenuItems[i].Text}".PadRight(64));
                Themes.Unselected.Apply();
            }
            switch (Console.ReadKey().Key)
            {
                case ConsoleKey.UpArrow: selected = (ushort)(selected != 0 ? selected - 1 : MenuItems.Count - 1); break;
                case ConsoleKey.DownArrow: selected = (ushort)(selected != MenuItems.Count - 1 ? selected + 1 : 0); break;
                case ConsoleKey.Enter:
                    {
                        Console.Clear();
                        MenuItems[selected].Function();
                    }
                    return;
                case ConsoleKey.Escape: Process.GetCurrentProcess().Kill(); break;
            }
        }
    }
    #endregion

    #region Builder pattern
    /// <summary>Добавляет элемент меню в данное меню.</summary>
    /// <param name="text">Текст, который будет отображаться у добавляемого элемента меню.</param>
    /// <param name="function">Функция, которая будет запушена, при выборе элемента меню.</param>
    /// <returns>Меню с добавленым элементом меню.</returns>
    public Menu AddMenuItem(string text, Action? function = null)
    {
        MenuItems.Add(new(text, function));
        return this;
    }
    /// <summary>Заменяет заголовок меню на новый.</summary>
    /// <param name="title">Новый заголовок меню.</param>
    /// <returns>Меню с изменённым заголовком.</returns>
    public Menu SetTitle(string title)
    {
        Title = title;
        return this;
    }
    /// <summary>Устанавливает темы для данного меню.</summary>
    /// <param name="title">Тема заголовка меню.</param>
    /// <param name="selected">Тема выбранного элемента меню.</param>
    /// <param name="unselected">Тема невыбранных элементов меню.</param>
    /// <returns>Меню с изменёнными темами.</returns>
    public Menu SetThemes(Theme title, Theme selected, Theme unselected)
    {
        Themes.Title = title;
        Themes.Selected = selected;
        Themes.Unselected = unselected;
        return this;
    }
    #endregion
}
