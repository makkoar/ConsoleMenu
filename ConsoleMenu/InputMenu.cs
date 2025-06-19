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
        int cursorPos = 0;

        // --- Устанавливаем каретку в конец первого поля при запуске меню ---
        if (MenuItems.Count > 0)
            cursorPos = (MenuItems[0].InputValue ?? string.Empty).Length;

        while (true)
        {
            InputMenuItem currentItem = MenuItems[selected];
            string prompt = $"{currentItem.Text}: ";
            StringBuilder inputBuilder = new(currentItem.InputValue ?? string.Empty);
            inputLeft = prompt.Length;
            inputTop = menuTop + 1 + selected;

            // --- Корректируем позицию курсора, если поле короче ---
            if (cursorPos > inputBuilder.Length)
                cursorPos = inputBuilder.Length;

            Console.SetCursorPosition(inputLeft + cursorPos, inputTop);
            Console.CursorVisible = true;

            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                if (keyInfo.Key == ConsoleKey.UpArrow)
                {
                    MenuItems[selected].InputValue = inputBuilder.ToString();
                    selected = (selected - 1 + MenuItems.Count) % MenuItems.Count;
                    Redraw();
                    break;
                }
                if (keyInfo.Key == ConsoleKey.DownArrow)
                {
                    MenuItems[selected].InputValue = inputBuilder.ToString();
                    selected = (selected + 1) % MenuItems.Count;
                    Redraw();
                    break;
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
                    string input = inputBuilder.ToString();
                    MenuItems[selected].InputValue = input;
                    Console.CursorVisible = false;
                    ClearMenuArea();
                    return MenuItems.ToDictionary(item => item.Id, item => item);
                }
                if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    if (cursorPos > 0)
                    {
                        _ = inputBuilder.Remove(cursorPos - 1, 1);
                        cursorPos--;
                        MenuItems[selected].InputValue = inputBuilder.ToString();
                        Redraw();
                        Console.SetCursorPosition(inputLeft + cursorPos, inputTop);
                    }
                    continue;
                }
                if (keyInfo.Key == ConsoleKey.Delete)
                {
                    if (cursorPos < inputBuilder.Length)
                    {
                        _ = inputBuilder.Remove(cursorPos, 1);
                        MenuItems[selected].InputValue = inputBuilder.ToString();
                        Redraw();
                        Console.SetCursorPosition(inputLeft + cursorPos, inputTop);
                    }
                    continue;
                }
                if (keyInfo.Key == ConsoleKey.LeftArrow)
                {
                    if (cursorPos > 0)
                        cursorPos--;
                    Console.SetCursorPosition(inputLeft + cursorPos, inputTop);
                    continue;
                }
                if (keyInfo.Key == ConsoleKey.RightArrow)
                {
                    if (cursorPos < inputBuilder.Length)
                        cursorPos++;
                    Console.SetCursorPosition(inputLeft + cursorPos, inputTop);
                    continue;
                }
                if (!char.IsControl(keyInfo.KeyChar))
                {
                    // --- ДО switch (currentItem.Type) ---
                    // Особая обработка для минуса в начале строки для знаковых типов
                    bool minusInserted = false;
                    if ((currentItem.Type is EInputMenuItemType.Int or EInputMenuItemType.Short or EInputMenuItemType.SByte
                        or EInputMenuItemType.Float or EInputMenuItemType.Double or EInputMenuItemType.Decimal)
                        && keyInfo.KeyChar is '-'
                        && (inputBuilder.ToString() is "0" or "" && cursorPos is 0 or 1))
                    {
                        _ = inputBuilder.Clear();
                        _ = inputBuilder.Append('-');
                        cursorPos = 1;
                        minusInserted = true;
                    }

                    if (minusInserted)
                    {
                        MenuItems[selected].InputValue = inputBuilder.ToString();
                        Redraw();
                        Console.SetCursorPosition(inputLeft + cursorPos, inputTop);
                        continue; // пропускаем остальную обработку, т.к. уже всё сделали
                    }

                    // --- далее обычная логика allowInput ---
                    bool allowInput = false;
                    switch (currentItem.Type)
                    {
                        case EInputMenuItemType.String:
                            allowInput = true;
                            break;
                        case EInputMenuItemType.Int:
                        case EInputMenuItemType.Short:
                        case EInputMenuItemType.SByte:
                            if (char.IsDigit(keyInfo.KeyChar))
                                allowInput = true;
                            else if (keyInfo.KeyChar == '-' && cursorPos == 0 && !inputBuilder.ToString().Contains('-'))
                                allowInput = true;
                            break;
                        case EInputMenuItemType.UInt:
                        case EInputMenuItemType.UShort:
                        case EInputMenuItemType.Byte:
                            if (char.IsDigit(keyInfo.KeyChar))
                                allowInput = true;
                            break;
                        case EInputMenuItemType.Float:
                        case EInputMenuItemType.Double:
                        case EInputMenuItemType.Decimal:
                            if (char.IsDigit(keyInfo.KeyChar))
                                allowInput = true;
                            else if (keyInfo.KeyChar == '-' && cursorPos == 0 && !inputBuilder.ToString().Contains('-'))
                                allowInput = true;
                            else if ((keyInfo.KeyChar == '.' || keyInfo.KeyChar == ',') &&
                                     !inputBuilder.ToString().Contains('.') && !inputBuilder.ToString().Contains(','))
                                allowInput = true;
                            break;
                        default:
                            allowInput = true;
                            break;
                    }

                    if (allowInput)
                    {
                        // Особая обработка для минуса в начале строки для знаковых типов
                        minusInserted = false;
                        if ((currentItem.Type is 
                            EInputMenuItemType.Int or EInputMenuItemType.Short or EInputMenuItemType.SByte or
                            EInputMenuItemType.Float or EInputMenuItemType.Double or EInputMenuItemType.Decimal)
                            && keyInfo.KeyChar is '-')
                        {
                            // Если строка "0" и курсор в начале или после "0" или на пустой строке — устанавливаем строку в "-"
                            if (inputBuilder.ToString() is "0" or "" && cursorPos is 0 or 1)
                            {
                                _ = inputBuilder.Clear();
                                _ = inputBuilder.Append('-');
                                cursorPos = 1;
                                minusInserted = true;
                            }
                        }

                        if (!minusInserted)
                        {
                            _ = inputBuilder.Insert(cursorPos, keyInfo.KeyChar);
                            cursorPos++;
                        }

                        string val, numberPart;
                        bool isNegative, parsed;
                        int oldLength;
                        switch (currentItem.Type)
                        {
                            case EInputMenuItemType.Int:
                            case EInputMenuItemType.Short:
                            case EInputMenuItemType.SByte:
                                {
                                    val = inputBuilder.ToString();
                                    // Если только "-", не трогаем строку
                                    if (val == "-")
                                        break;
                                    isNegative = val.StartsWith("-");
                                    numberPart = val[(isNegative ? 1 : 0)..].TrimStart('0');
                                    if (numberPart is "" or "0")
                                        numberPart = "0";
                                    val = isNegative ? (numberPart == "0" ? "-0" : "-" + numberPart) : numberPart;

                                    if (BigInteger.TryParse(val, out BigInteger bigVal))
                                    {
                                        switch (currentItem.Type)
                                        {
                                            case EInputMenuItemType.Int:
                                                if (bigVal < int.MinValue) bigVal = int.MinValue;
                                                if (bigVal > int.MaxValue) bigVal = int.MaxValue;
                                                val = ((int)bigVal).ToString();
                                                if (isNegative && numberPart == "0") val = "-0";
                                                break;
                                            case EInputMenuItemType.Short:
                                                if (bigVal < short.MinValue) bigVal = short.MinValue;
                                                if (bigVal > short.MaxValue) bigVal = short.MaxValue;
                                                val = ((short)bigVal).ToString();
                                                if (isNegative && numberPart == "0") val = "-0";
                                                break;
                                            case EInputMenuItemType.SByte:
                                                if (bigVal < sbyte.MinValue) bigVal = sbyte.MinValue;
                                                if (bigVal > sbyte.MaxValue) bigVal = sbyte.MaxValue;
                                                val = ((sbyte)bigVal).ToString();
                                                if (isNegative && numberPart == "0") val = "-0";
                                                break;
                                        }
                                    }

                                    // Если только "-", не трогаем строку и позицию курсора
                                    if (inputBuilder.ToString() == "-")
                                        break;

                                    int oldCursorPos = cursorPos;
                                    oldLength = inputBuilder.Length;
                                    int newLength = val.Length;
                                    int delta = newLength - oldLength;
                                    cursorPos = oldCursorPos + delta;
                                    if (cursorPos < 0) cursorPos = 0;
                                    if (cursorPos > newLength) cursorPos = newLength;

                                    _ = inputBuilder.Clear();
                                    _ = inputBuilder.Append(val);
                                    break;
                                }
                            case EInputMenuItemType.UInt:
                            case EInputMenuItemType.UShort:
                            case EInputMenuItemType.Byte:
                                {
                                    val = inputBuilder.ToString().TrimStart('0');
                                    if (val == "") val = "0";
                                    if (BigInteger.TryParse(val, out BigInteger bigVal))
                                    {
                                        switch (currentItem.Type)
                                        {
                                            case EInputMenuItemType.UInt:
                                                if (bigVal < uint.MinValue) bigVal = uint.MinValue;
                                                if (bigVal > uint.MaxValue) bigVal = uint.MaxValue;
                                                val = ((uint)bigVal).ToString();
                                                break;
                                            case EInputMenuItemType.UShort:
                                                if (bigVal < ushort.MinValue) bigVal = ushort.MinValue;
                                                if (bigVal > ushort.MaxValue) bigVal = ushort.MaxValue;
                                                val = ((ushort)bigVal).ToString();
                                                break;
                                            case EInputMenuItemType.Byte:
                                                if (bigVal < byte.MinValue) bigVal = byte.MinValue;
                                                if (bigVal > byte.MaxValue) bigVal = byte.MaxValue;
                                                val = ((byte)bigVal).ToString();
                                                break;
                                        }
                                    }
                                    int oldCursorPos = cursorPos;
                                    oldLength = inputBuilder.Length;
                                    int newLength = val.Length;
                                    int delta = newLength - oldLength;
                                    cursorPos = oldCursorPos + delta;
                                    if (cursorPos < 0) cursorPos = 0;
                                    if (cursorPos > newLength) cursorPos = newLength;

                                    _ = inputBuilder.Clear();
                                    _ = inputBuilder.Append(val);
                                    break;
                                }
                            case EInputMenuItemType.Float:
                            case EInputMenuItemType.Double:
                            case EInputMenuItemType.Decimal:
                                {
                                    val = inputBuilder.ToString().Replace(',', '.');
                                    isNegative = val.StartsWith("-");
                                    string rest = isNegative ? val[1..] : val;
                                    // Трим ведущих нулей перед точкой
                                    if (rest.StartsWith("0") && rest.Length > 1 && rest[1] != '.')
                                        rest = rest.TrimStart('0');
                                    if (rest == "" || rest.StartsWith(".")) rest = "0" + rest;
                                    val = isNegative ? "-" + rest : rest;

                                    // Проверка диапазона и нормализация
                                    parsed = decimal.TryParse(val, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal decVal);
                                    decimal min = 0, max = 0;
                                    switch (currentItem.Type)
                                    {
                                        case EInputMenuItemType.Float:
                                            min = Convert.ToDecimal(float.MinValue); max = Convert.ToDecimal(float.MaxValue); break;
                                        case EInputMenuItemType.Double:
                                            min = Convert.ToDecimal(double.MinValue); max = Convert.ToDecimal(double.MaxValue); break;
                                        case EInputMenuItemType.Decimal:
                                            min = decimal.MinValue; max = decimal.MaxValue; break;
                                    }
                                    if (parsed)
                                    {
                                        if (decVal < min) decVal = min;
                                        if (decVal > max) decVal = max;
                                        val = decVal.ToString(CultureInfo.InvariantCulture);
                                    }

                                    int oldCursorPos = cursorPos;
                                    oldLength = inputBuilder.Length;
                                    int newLength = val.Length;
                                    int delta = newLength - oldLength;
                                    cursorPos = oldCursorPos + delta;
                                    if (cursorPos < 0) cursorPos = 0;
                                    if (cursorPos > newLength) cursorPos = newLength;

                                    _ = inputBuilder.Clear();
                                    _ = inputBuilder.Append(val);
                                    break;
                                }
                        }

                        MenuItems[selected].InputValue = inputBuilder.ToString();
                        Redraw();
                        Console.SetCursorPosition(inputLeft + cursorPos, inputTop);
                    }
                }
            }
        }
    }
    #endregion

    #region Строитель
    /// <summary>Добавляет готовый элемент меню, проверяя уникальность его ID.</summary>
    /// <param name="item">Экземпляр <see cref="InputMenuItem"/> для добавления.</param>
    /// <returns>Меню с добавленным элементом для дальнейшей настройки.</returns>
    /// <exception cref="ArgumentException">Выбрасывается, если элемент с таким ID уже существует в меню.</exception>
    public InputMenu AddMenuItem(InputMenuItem item, EInputMenuItemType type = EInputMenuItemType.String)
    {
        if (string.IsNullOrEmpty(item.Id))
            item.Id = nextDefaultId.ToString();

        if (!usedIds.Add(item.Id))
            throw new ArgumentException($"Элемент с ID '{item.Id}' уже существует в этом меню.", nameof(item));

        MenuItems.Add(item.SetInputType(type));

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
    /// <param name="type">Тип вводимого значения, определяемый перечислением <see cref="EInputMenuItemType"/>.</param>
    /// <returns>Меню с добавленным элементом для дальнейшей настройки.</returns>
    public InputMenu AddMenuItem(string text, string? defaultValue = "", string? id = null, EInputMenuItemType type = EInputMenuItemType.String) => AddMenuItem(new InputMenuItem(text, defaultValue, id), type);

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