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
    public Themes Theme { get; set; } = new();

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
    public void Apply()
    {
        List<string?> initialValues = [.. MenuItems.Select(item => item.InputValue)];

        int selected = 0;
        Console.CursorVisible = true;
        Console.Clear();

        Theme.Title.Apply();
        Console.WriteLine(Title);
        Console.WriteLine();

        // --- 1. Первичная отрисовка всего меню ---
        void Redraw()
        {
            for (int i = 0; i < MenuItems.Count; i++)
            {
                Console.SetCursorPosition(0, i + 2);
                if (i == selected)
                    Theme.Selected.Apply();
                else
                    Theme.Unselected.Apply();

                var value = MenuItems[i].InputValue ?? "";
                // Очищаем строку перед выводом
                Console.Write(new string(' ', Console.WindowWidth - 1));
                Console.SetCursorPosition(0, i + 2);
                Console.Write($"{MenuItems[i].Text}: {value}");
            }
            Theme.Unselected.Apply();
        }

        Redraw();

        int inputLeft = 0;
        int inputTop = 0;

        while (true)
        {
            var currentItem = MenuItems[selected];
            var prompt = $"{currentItem.Text}: ";
            var inputBuilder = new StringBuilder(currentItem.InputValue ?? "");
            inputLeft = prompt.Length;
            inputTop = selected + 2;

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
                Theme.Unselected.Apply();
                Console.Clear();
                return;
            }
            if (keyInfo.Key == ConsoleKey.Enter)
            {
                // Завершаем ввод и выходим
                MenuItems[selected].InputValue = inputBuilder.ToString();
                Console.CursorVisible = false;
                Theme.Unselected.Apply();
                Console.Clear();
                return;
            }
            if (keyInfo.Key == ConsoleKey.Backspace)
            {
                if (inputBuilder.Length > 0)
                {
                    inputBuilder.Remove(inputBuilder.Length - 1, 1);
                    MenuItems[selected].InputValue = inputBuilder.ToString();
                    Redraw();
                    Console.SetCursorPosition(inputLeft + inputBuilder.Length, inputTop);
                }
                continue;
            }
            if (!char.IsControl(keyInfo.KeyChar))
            {
                inputBuilder.Append(keyInfo.KeyChar);
                MenuItems[selected].InputValue = inputBuilder.ToString();
                Redraw();
                Console.SetCursorPosition(inputLeft + inputBuilder.Length, inputTop);
            }
        }
    }

    /// <summary>Возвращает текущие значения всех элементов меню в виде словаря.</summary>
    /// <returns>Словарь, где ключ — это <see cref="InputMenuItem.Id"/>, а значение — это <see cref="InputMenuItem.InputValue"/>.</returns>
    public Dictionary<string, string?> GetInputs()
    {
        return MenuItems.ToDictionary(item => item.Id, item => item.InputValue);
    }
    #endregion

    #region Строитель
    /// <summary>Добавляет новый элемент в меню ввода.</summary>
    /// <param name="text">Текст-приглашение для добавляемого элемента.</param>
    /// <param name="defaultValue">Начальное значение поля ввода.</param>
    /// <param name="id">Необязательный уникальный идентификатор. Если не указан, будет сгенерирован автоматически.</param>
    /// <returns>Меню с добавленным элементом для дальнейшей настройки.</returns>
    public InputMenu AddMenuItem(string text, string? defaultValue = "", string? id = null)
    {
        var newItem = new InputMenuItem(text, defaultValue, id);
        return AddMenuItem(newItem);
    }

    /// <summary>Добавляет готовый элемент меню, проверяя уникальность его ID.</summary>
    /// <param name="item">Экземпляр <see cref="InputMenuItem"/> для добавления.</param>
    /// <returns>Меню с добавленным элементом для дальнейшей настройки.</returns>
    /// <exception cref="ArgumentException">Выбрасывается, если элемент с таким ID уже существует в меню.</exception>
    public InputMenu AddMenuItem(InputMenuItem item)
    {
        if (string.IsNullOrEmpty(item.Id))
            item.Id = nextDefaultId.ToString();

        if (!usedIds.Add(item.Id))
        {
            throw new ArgumentException($"Элемент с ID '{item.Id}' уже существует в этом меню.", nameof(item));
        }

        MenuItems.Add(item);

        // Если пользователь вручную задал числовой ID, убедимся, что наш авто-инкремент его не перезапишет.
        if (int.TryParse(item.Id, out int numericId))
        {
            nextDefaultId = Math.Max(nextDefaultId, numericId + 1);
        }
        else if (item.Id == (nextDefaultId - 1 > 0 ? nextDefaultId - 1 : 0).ToString())
        {
            nextDefaultId++;
        }

        return this;
    }

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
        Theme.Title = title;
        Theme.Selected = selected;
        Theme.Unselected = unselected;
        return this;
    }
    #endregion
}