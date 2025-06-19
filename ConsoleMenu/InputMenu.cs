namespace ConsoleMenu;

/// <summary>Класс, отвечающий за консольное меню для ввода данных пользователем.</summary>
public class InputMenu()
{
    #region Приватные поля
    /// <summary>Хранит использованные ID для обеспечения уникальности.</summary>
    private readonly HashSet<string> usedIds = [];

    /// <summary>Счётчик для генерации уникальных ID по умолчанию.</summary>
    private int nextDefaultId = 0;
    #endregion

    #region Поля и свойства
    /// <summary>Тема, которая будет применена к данному меню.</summary>
    public Theme Theme { get; set; } = Themes.Classic;

    /// <summary>Заголовок меню.</summary>
    public string Title { get; set; } = "Введите значение:";

    /// <summary>Элементы меню для ввода.</summary>
    public List<InputMenuItem> MenuItems { get; set; } = [];
    #endregion

    #region Конструкторы
    /// <summary>Инициализирует новый экземпляр класса <see cref="InputMenu"/> с указанным заголовком.</summary>
    /// <param name="title">Заголовок меню.</param>
    public InputMenu(string title) : this()
        => Title = title;

    /// <summary>Инициализирует новый экземпляр класса <see cref="InputMenu"/> с заданным набором элементов, проверяя их ID на уникальность.</summary>
    /// <param name="menuItems">Набор элементов меню <see cref="InputMenuItem"/>.</param>
    public InputMenu(params List<InputMenuItem> menuItems) : this() =>
        menuItems.ForEach(item => AddMenuItem(item));

    /// <summary>Инициализирует новый экземпляр класса <see cref="InputMenu"/> с указанным заголовком и набором элементов, проверяя их ID на уникальность.</summary>
    /// <param name="title">Заголовок меню.</param>
    /// <param name="menuItems">Набор элементов меню <see cref="InputMenuItem"/>.</param>
    public InputMenu(string title, params List<InputMenuItem> menuItems) : this(menuItems)
        => Title = title;
    #endregion

    #region Основная логика
    /// <summary>Отображает меню, обрабатывает ввод пользователя и обновляет значения в <see cref="MenuItems"/>.</summary>
    /// <param name="clear">Указывает, нужно ли очищать консоль перед отображением меню.</param>
    /// <returns>Словарь, где ключ — это <see cref="InputMenuItem.Id"/>, а значение — сам элемент <see cref="InputMenuItem"/>.</returns>
    public Dictionary<string, InputMenuItem> Apply(bool clear = false)
    {
        List<string?> initialValues = [.. MenuItems.Select(item => item.InputValue)];
        int selected = 0;
        if (clear) Console.Clear();

        int menuTop = Console.CursorTop;
        Console.CursorVisible = true;

        // Отрисовка заголовка
        Console.ForegroundColor = Theme.TitleTextColor;
        Console.BackgroundColor = Theme.TitleBackgroundColor;
        Console.SetCursorPosition(0, menuTop);
        Console.WriteLine(Title);

        void ClearMenuArea()
        {
            int lineWidth = Console.WindowWidth > 1 ? Console.WindowWidth - 1 : 64;
            string cleaner = new(' ', lineWidth);
            for (int i = 0; i < MenuItems.Count + 1; i++)
            {
                Console.SetCursorPosition(0, menuTop + i);
                Console.Write(cleaner);
            }
            Console.SetCursorPosition(0, menuTop);
            Console.ResetColor();
        }

        void DrawMenuLine(int i, bool isSelected)
        {
            int lineWidth = Console.WindowWidth > 1 ? Console.WindowWidth - 1 : 64;
            Console.SetCursorPosition(0, menuTop + 1 + i);

            InputMenuItem item = MenuItems[i];
            string value = item.InputValue ?? "";
            string prompt = item.Text + ": ";
            string line = prompt + value;

            if (isSelected)
            {
                Console.BackgroundColor = Theme.SelectedBackgroundColor;
                Console.ForegroundColor = Theme.SelectedTextColor;
                if (line.Length < lineWidth)
                    Console.Write(line.PadRight(lineWidth));
                else
                    Console.Write(line);
            }
            else
            {
                Console.BackgroundColor = Theme.UnselectedBackgroundColor;
                Console.ForegroundColor = Theme.FieldTextColor;
                if (prompt.Length < lineWidth)
                    Console.Write(prompt);
                else
                    Console.Write(prompt[..lineWidth]);

                int left = Math.Min(prompt.Length, lineWidth);
                Console.SetCursorPosition(left, menuTop + 1 + i);
                Console.ForegroundColor = Theme.UnselectedTextColor;
                int valueLen = Math.Min(value.Length, lineWidth - left);
                if (valueLen > 0)
                    Console.Write(value[..valueLen]);

                int pad = lineWidth - Math.Min(line.Length, lineWidth);
                if (pad > 0)
                    Console.Write(new string(' ', pad));
            }

            Console.ResetColor();
        }

        void Redraw()
        {
            for (int i = 0; i < MenuItems.Count; i++)
                DrawMenuLine(i, i == selected);
        }

        Redraw();

        int inputLeft = 0;
        int inputTop = 0;

        while (true)
        {
            InputMenuItem currentItem = MenuItems[selected];
            string prompt = $"{currentItem.Text}: ";
            StringBuilder inputBuilder = new(currentItem.InputValue ?? string.Empty);
            inputLeft = prompt.Length;
            inputTop = menuTop + 1 + selected;

            Console.SetCursorPosition(inputLeft + inputBuilder.Length, inputTop);
            Console.CursorVisible = true;

            ConsoleKeyInfo keyInfo = Console.ReadKey(true);

            if (keyInfo.Key == ConsoleKey.UpArrow)
            {
                selected = (selected - 1 + MenuItems.Count) % MenuItems.Count;
                Redraw();
                continue;
            }
            if (keyInfo.Key == ConsoleKey.DownArrow)
            {
                selected = (selected + 1) % MenuItems.Count;
                Redraw();
                continue;
            }
            if (keyInfo.Key == ConsoleKey.Escape)
            {
                for (int j = 0; j < MenuItems.Count; j++)
                    MenuItems[j].InputValue = initialValues[j];
                Console.CursorVisible = false;
                ClearMenuArea();
                return MenuItems.ToDictionary(item => item.Id, item => item);
            }
            if (keyInfo.Key == ConsoleKey.Enter)
            {
                MenuItems[selected].InputValue = inputBuilder.ToString();
                Console.CursorVisible = false;
                ClearMenuArea();
                return MenuItems.ToDictionary(item => item.Id, item => item);
            }
            if (keyInfo.Key == ConsoleKey.Backspace)
            {
                if (inputBuilder.Length > 0)
                {
                    _ = inputBuilder.Remove(inputBuilder.Length - 1, 1);
                    MenuItems[selected].InputValue = inputBuilder.ToString();
                    Redraw();
                    Console.SetCursorPosition(inputLeft + inputBuilder.Length, inputTop);
                }
                continue;
            }
            if (!char.IsControl(keyInfo.KeyChar))
            {
                _ = inputBuilder.Append(keyInfo.KeyChar);
                MenuItems[selected].InputValue = inputBuilder.ToString();
                Redraw();
                Console.SetCursorPosition(inputLeft + inputBuilder.Length, inputTop);
            }
        }
    }
    #endregion

    #region Строитель
    /// <summary>Добавляет готовый элемент меню, проверяя уникальность его ID.</summary>
    /// <param name="item">Экземпляр <see cref="InputMenuItem"/> для добавления.</param>
    /// <returns>Меню с добавленным элементом для дальнейшей настройки.</returns>
    /// <exception cref="ArgumentException">Выбрасывается, если элемент с таким ID уже существует в меню.</exception>
    public InputMenu AddMenuItem(InputMenuItem item)
    {
        if (string.IsNullOrEmpty(item.Id))
            item.Id = nextDefaultId.ToString();

        if (!usedIds.Add(item.Id))
            throw new ArgumentException($"Элемент с ID '{item.Id}' уже существует в этом меню.", nameof(item));

        MenuItems.Add(item);

        // Если пользователь вручную задал числовой ID, убедимся, что наш авто-инкремент его не перезапишет.
        if (int.TryParse(item.Id, out int numericId))
            nextDefaultId = Math.Max(nextDefaultId, numericId + 1);
        else if (item.Id == (nextDefaultId - 1 > 0 ? nextDefaultId - 1 : 0).ToString())
            nextDefaultId++;

        return this;
    }

    /// <summary>Добавляет новый элемент в меню ввода.</summary>
    /// <param name="text">Текст-приглашение для добавляемого элемента.</param>
    /// <param name="defaultValue">Начальное значение поля ввода.</param>
    /// <param name="id">Необязательный уникальный идентификатор. Если не указан, будет сгенерирован автоматически.</param>
    /// <returns>Меню с добавленным элементом для дальнейшей настройки.</returns>
    public InputMenu AddMenuItem(string text, string? defaultValue = "", string? id = null) => AddMenuItem(new InputMenuItem(text, defaultValue, id));

    /// <summary>Заменяет заголовок меню на новый.</summary>
    /// <param name="newTitle">Новый заголовок меню.</param>
    /// <returns>Меню с изменённым заголовком.</returns>
    public InputMenu SetTitle(string newTitle)
    {
        Title = newTitle;
        return this;
    }

    /// <summary>Устанавливает темы для данного меню.</summary>
    /// <param name="title">Тема заголовка меню.</param>
    /// <param name="selected">Тема редактируемого поля ввода.</param>
    /// <param name="unselected">Тема остальных элементов.</param>
    /// <returns>Меню с изменёнными темами.</returns>
    public InputMenu SetThemes(Theme title, Theme selected, Theme unselected)
    {
        Theme.TitleTextColor = title.TitleTextColor;
        Theme.TitleBackgroundColor = title.TitleBackgroundColor;
        Theme.SelectedTextColor = selected.SelectedTextColor;
        Theme.SelectedBackgroundColor = selected.SelectedBackgroundColor;
        Theme.UnselectedTextColor = unselected.UnselectedTextColor;
        Theme.UnselectedBackgroundColor = unselected.UnselectedBackgroundColor;
        return this;
    }
    #endregion
}