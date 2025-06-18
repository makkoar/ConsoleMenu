namespace ConsoleMenu;

/// <summary>Класс, отвечающий за консольное меню выбора.</summary>
public class SelectMenu()
{
    #region Поля и свойства
    /// <summary>Тема, которая будет применена к данному меню.</summary>
    public Theme Theme { get; set; } = Themes.Presets[EThemes.Classic];
    /// <summary>Заголовок меню.</summary>
    public string Title { get; set; } = "Выберите пункт меню:";
    /// <summary>Элементы меню.</summary>
    public List<SelectMenuItem> MenuItems { get; set; } = [];
    #endregion

    #region Конструкторы
    /// <summary>Инициализирует новый экземпляр класса <see cref="SelectMenu"/> с указанным заголовком.</summary>
    /// <param name="title">Заголовок меню.</param>
    public SelectMenu(string title) : this()
        => Title = title;

    /// <summary>Инициализирует новый экземпляр класса <see cref="SelectMenu"/> с заданным списком элементов.</summary>
    /// <param name="menuItems">Список элементов меню <see cref="SelectMenuItem"/>.</param>
    public SelectMenu(params List<SelectMenuItem> menuItems)
        : this() => menuItems.ForEach(MenuItems.Add);

    /// <summary>Инициализирует новый экземпляр класса <see cref="SelectMenu"/> с указанным заголовком и списком элементов.</summary>
    /// <param name="title">Заголовок меню.</param>
    /// <param name="menuItems">Список элементов меню <see cref="SelectMenuItem"/>.</param>
    public SelectMenu(string title, params List<SelectMenuItem> menuItems)
        : this(menuItems) => Title = title;

    /// <summary>Инициализирует новый экземпляр класса <see cref="SelectMenu"/> из списка строк, где каждая строка становится элементом меню.</summary>
    /// <param name="menuItems">Список строк для создания элементов меню.</param>
    public SelectMenu(params List<string> menuItems)
        : this() => menuItems.ForEach(item => MenuItems.Add(new(item)));

    /// <summary>Инициализирует новый экземпляр класса <see cref="SelectMenu"/> с указанным заголовком и списком строк, где каждая строка становится элементом меню.</summary>
    /// <param name="title">Заголовок меню.</param>
    /// <param name="menuItems">Список строк для создания элементов меню.</param>
    public SelectMenu(string title, params List<string> menuItems)
        : this(menuItems) => Title = title;
    #endregion

    #region Функции активации меню
    /// <summary>Применяет текущее меню, отображая его и обрабатывая ввод пользователя и возвращая индекс выбранного элемента.</summary>
    /// <param name="startIndex">Индекс элемента, который будет выбран изначально.</param>
    /// <param name="clear"><see langword="true"/>, чтобы очистить консоль перед отображением; <see langword="false"/>, чтобы отрисовать меню с текущей позиции курсора.</param>
    /// <returns>Индекс выбранного элемента меню.</returns>
    public ushort Apply(ushort startIndex = 0, bool clear = false)
    {
        if (clear) Console.Clear();

        ushort selected = startIndex;
        int menuTop = Console.CursorTop;
        int menuHeight = MenuItems.Count + 1; // +1 для заголовка
        Console.CursorVisible = false;

        while (true)
        {
            // --- 1. Перерисовка меню на исходной позиции ---
            Console.SetCursorPosition(0, menuTop);

            Console.ForegroundColor = Theme.TitleTextColor;
            Console.BackgroundColor = Theme.TitleBackgroundColor;
            Console.WriteLine(Title.PadRight(64));
            for (ushort i = 0; i < MenuItems.Count; i++)
            {
                if (i == selected)
                {
                    Console.ForegroundColor = Theme.SelectedTextColor;
                    Console.BackgroundColor = Theme.SelectedBackgroundColor;
                }
                else
                {
                    Console.ForegroundColor = Theme.UnselectedTextColor;
                    Console.BackgroundColor = Theme.UnselectedBackgroundColor;
                }
                Console.WriteLine($"* {MenuItems[i].Text}".PadRight(64));
            }
            Console.ResetColor(); // Сброс темы для последующего вывода в консоль

            // --- 2. Обработка ввода ---
            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow: selected = (ushort)(selected != 0 ? selected - 1 : MenuItems.Count - 1); break;
                case ConsoleKey.DownArrow: selected = (ushort)(selected != MenuItems.Count - 1 ? selected + 1 : 0); break;
                case ConsoleKey.Enter:
                case ConsoleKey.Escape:
                    {
                        // --- 3. Очистка только области меню ---
                        string cleaner = new(' ', Console.WindowWidth > 1 ? Console.WindowWidth - 1 : 65);
                        for (int i = 0; i < menuHeight; i++)
                        {
                            Console.SetCursorPosition(0, menuTop + i);
                            Console.Write(cleaner);
                        }
                        Console.SetCursorPosition(0, menuTop);
                        Console.CursorVisible = true;
                        Console.ResetColor();

                        if (keyInfo.Key is ConsoleKey.Enter && MenuItems.Count > 0 && MenuItems[selected].Function is not null)
                            MenuItems[selected].Function!();
                        return selected;
                    }
            }
        }
    }
    #endregion

    #region Строитель
    /// <summary>Добавляет элемент меню в данное меню.</summary>
    /// <param name="item">Экземпляр <see cref="InputMenuItem"/> для добавления.</param>
    /// <returns>Меню с добавленным элементом меню.</returns>
    public SelectMenu AddMenuItem(SelectMenuItem item)
    {
        MenuItems.Add(item);
        return this;
    }
    /// <summary>Добавляет элемент меню в данное меню.</summary>
    /// <param name="text">Текст, который будет отображаться у добавляемого элемента меню.</param>
    /// <param name="function">Функция, которая будет запушена, при выборе элемента меню.</param>
    /// <returns>Меню с добавленным элементом меню.</returns>
    public SelectMenu AddMenuItem(string text, Action? function = null)
        => AddMenuItem(new(text, function));
    
    /// <summary>Заменяет заголовок меню на новый.</summary>
    /// <param name="title">Новый заголовок меню.</param>
    /// <returns>Меню с изменённым заголовком.</returns>
    public SelectMenu SetTitle(string title)
    {
        Title = title;
        return this;
    }
    /// <summary>Устанавливает темы для данного меню.</summary>
    /// <param name="titleTextColor">Цвет текста заголовка меню.</param>
    /// <param name="titleBackgroundColor">Цвет фона заголовка меню.</param>
    /// <param name="selectedTextColor">Цвет текста выбранного элемента меню.</param>
    /// <param name="selectedBackgroundColor">Цвет фона выбранного элемента меню.</param>
    /// <param name="unselectedTextColor">Цвет текста невыбранных элементов меню.</param>
    /// <param name="unselectedBackgroundColor">Цвет фона невыбранных элементов меню.</param>
    /// <returns>Меню с изменёнными темами.</returns>
    public SelectMenu SetThemes(
        ConsoleColor titleTextColor,
        ConsoleColor titleBackgroundColor,
        ConsoleColor selectedTextColor,
        ConsoleColor selectedBackgroundColor,
        ConsoleColor unselectedTextColor,
        ConsoleColor unselectedBackgroundColor)
    {
        Theme.TitleTextColor = titleTextColor;
        Theme.TitleBackgroundColor = titleBackgroundColor;
        Theme.SelectedTextColor = selectedTextColor;
        Theme.SelectedBackgroundColor = selectedBackgroundColor;
        Theme.UnselectedTextColor = unselectedTextColor;
        Theme.UnselectedBackgroundColor = unselectedBackgroundColor;
        return this;
    }
    #endregion
}
