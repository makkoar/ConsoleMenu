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
    public InputMenu(params InputMenuItem[] menuItems) : this()
    {
        foreach (var item in menuItems)
        {
            AddMenuItem(item);
        }
    }

    /// <summary>Инициализирует новый экземпляр класса <see cref="InputMenu"/> с указанным заголовком и набором элементов, проверяя их ID на уникальность.</summary>
    /// <param name="title">Заголовок меню.</param>
    /// <param name="menuItems">Набор элементов меню <see cref="InputMenuItem"/>.</param>
    public InputMenu(string title, params InputMenuItem[] menuItems) : this(menuItems)
        => Title = title;
    #endregion

    #region Основная логика
    /// <summary>Отображает меню, обрабатывает ввод пользователя и обновляет значения в <see cref="MenuItems"/>.</summary>
    public void Apply()
    {
        if (!MenuItems.Any()) return;

        var initialValues = MenuItems.Select(item => item.InputValue).ToList();

        Console.CursorVisible = true;
        Console.Clear();

        Theme.Title.Apply();
        Console.WriteLine(Title);
        Console.WriteLine();

        // --- 1. Первичная отрисовка всего меню ---
        Theme.Unselected.Apply();
        for (int i = 0; i < MenuItems.Count; i++)
        {
            Console.WriteLine($"{MenuItems[i].Text}: {MenuItems[i].InputValue}");
        }

        int originalTop = Console.CursorTop;

        // --- 2. Цикл по каждому полю для ввода ---
        for (int i = 0; i < MenuItems.Count; i++)
        {
            var currentItem = MenuItems[i];
            var prompt = $"{currentItem.Text}: ";
            var inputBuilder = new StringBuilder(currentItem.InputValue ?? "");

            int inputLeft = prompt.Length;
            // +2: одна строка для заголовка, одна для пустой строки после него
            int inputTop = i + 2;

            // --- 2.1. Активируем текущее поле (подсветка) ---
            Console.SetCursorPosition(inputLeft, inputTop);
            Theme.Selected.Apply();
            Console.Write(inputBuilder.ToString());

            while (true)
            {
                Console.SetCursorPosition(inputLeft + inputBuilder.Length, inputTop);
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                switch (keyInfo.Key)
                {
                    case ConsoleKey.Enter:
                        currentItem.InputValue = inputBuilder.ToString();
                        Console.SetCursorPosition(inputLeft, inputTop);
                        Theme.Unselected.Apply();
                        Console.Write(currentItem.InputValue);
                        goto next_item;

                    case ConsoleKey.Escape:
                        for (int j = 0; j < MenuItems.Count; j++)
                        {
                            MenuItems[j].InputValue = initialValues[j];
                        }
                        Console.CursorVisible = false;
                        Theme.Unselected.Apply();
                        Console.SetCursorPosition(0, originalTop);
                        Console.Clear();
                        return;

                    case ConsoleKey.Backspace:
                        if (inputBuilder.Length > 0)
                        {
                            inputBuilder.Remove(inputBuilder.Length - 1, 1);
                            Console.SetCursorPosition(inputLeft + inputBuilder.Length, inputTop);
                            Console.Write(" ");
                            Console.SetCursorPosition(inputLeft + inputBuilder.Length, inputTop);
                        }
                        break;

                    default:
                        if (!char.IsControl(keyInfo.KeyChar))
                        {
                            inputBuilder.Append(keyInfo.KeyChar);
                            Theme.Selected.Apply();
                            Console.Write(keyInfo.KeyChar);
                        }
                        break;
                }
            }
        next_item:;
        }

        Console.CursorVisible = false;
        Theme.Unselected.Apply();
        Console.SetCursorPosition(0, originalTop);
        Console.Clear();
    }

    /// <summary>Возвращает текущие значения всех элементов меню в виде словаря.</summary>
    /// <returns>Словарь, где ключ — это <see cref="InputMenuItem.Id"/>, а значение — это <see cref="InputMenuItem.InputValue"/>.</returns>
    public Dictionary<string, string?> GetInputs()
    {
        return MenuItems.ToDictionary(item => item.Id, item => item.InputValue);
    }
    #endregion

    #region Строитель (Fluent API)
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
        {
            item.Id = nextDefaultId.ToString();
        }

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