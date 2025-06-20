using System.Globalization;
using System.Numerics;
using System.Text;

namespace ConsoleMenu;

/// <summary>Класс, реализующий консольное меню для ввода данных пользователем по нескольким полям с поддержкой темизации, проверки уникальности идентификаторов и гибкой настройки элементов.<br/>Позволяет организовать пошаговый ввод значений с валидацией и возвратом результатов в виде словаря.</summary>
public class InputMenu()
{
    #region Приватные поля
    /// <summary>Хранит использованные ID для обеспечения уникальности среди элементов меню.<br/>Используется для предотвращения дублирования идентификаторов.</summary>
    private readonly HashSet<string> usedIds = [];

    /// <summary>Счётчик для генерации уникальных ID по умолчанию для новых элементов меню.<br/>Автоматически увеличивается при добавлении элементов без явного ID.</summary>
    private int nextDefaultId = 0;

    /// <summary>Высота меню с учётом количества строк заголовка и количества элементов меню.</summary>
    private int menuHeight = 0;
    #endregion

    #region Поля и свойства
    /// <summary>Тема, применяемая к данному меню.<br/>Определяет цвета заголовка, выбранного и невыбранных полей, а также поля ввода.</summary>
    public Theme Theme { get; set; } = Themes.Classic;

    /// <summary>Заголовок меню, отображаемый над полями ввода.<br/>Может быть изменён через <see cref="SetTitle(string)"/>.</summary>
    public string Title { get; set; } = "Введите значение:";

    /// <summary>Список элементов меню для ввода.<br/>Каждый элемент описывается объектом <see cref="InputMenuItem"/> и содержит текст-приглашение, значение по умолчанию, тип и уникальный идентификатор.</summary>
    public List<InputMenuItem> MenuItems { get; set; } = [];
    #endregion

    #region Конструкторы
    /// <summary>Создаёт новый экземпляр класса <see cref="InputMenu"/> с указанным заголовком.<br/>Меню будет содержать пустой список элементов.</summary>
    /// <param name="title">Заголовок меню, который будет отображён над полями ввода.</param>
    public InputMenu(string title) : this() => Title = title;

    /// <summary>Создаёт новый экземпляр класса <see cref="InputMenu"/> с заданным набором элементов.<br/>Проверяет уникальность их идентификаторов.</summary>
    /// <param name="menuItems">Список элементов меню <see cref="InputMenuItem"/> для добавления в меню.</param>
    public InputMenu(params List<InputMenuItem> menuItems) : this() => menuItems.ForEach(item => AddMenuItem(item));

    /// <summary>Создаёт новый экземпляр класса <see cref="InputMenu"/> с указанным заголовком и набором элементов.<br/>Проверяет уникальность идентификаторов элементов.</summary>
    /// <param name="title">Заголовок меню.</param>
    /// <param name="menuItems">Список элементов меню <see cref="InputMenuItem"/> для добавления.</param>
    public InputMenu(string title, params List<InputMenuItem> menuItems) : this(menuItems) => Title = title;
    #endregion

    #region Основная логика
    /// <summary>Разбивает текст на строки по ширине окна консоли, перенося по словам и разбивая слишком длинные слова.</summary>
    /// <param name="text">Исходная строка, которую требуется разбить на несколько строк.</param>
    /// <param name="maxWidth">Максимальная ширина строки (обычно ширина окна консоли).</param>
    /// <returns>Список строк, каждая из которых не превышает указанную ширину.</returns>
    private static List<string> WrapText(string text, int maxWidth)
    {
        List<string> lines = [];
        string[] words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        StringBuilder currentLine = new();

        foreach (string word in words)
        {
            // Если слово не помещается в текущую строку
            if (currentLine.Length > 0)
            {
                if (currentLine.Length + 1 + word.Length > maxWidth)
                {
                    lines.Add(currentLine.ToString());
                    _ = currentLine.Clear();
                }
                else
                {
                    _ = currentLine.Append(' ');
                }
            }
            // Если слово длиннее maxWidth, разбиваем его
            int wordPos = 0;
            while (word.Length - wordPos > maxWidth)
            {
                if (currentLine.Length > 0)
                {
                    lines.Add(currentLine.ToString());
                    _ = currentLine.Clear();
                }
                lines.Add(word.Substring(wordPos, maxWidth));
                wordPos += maxWidth;
            }
            if (wordPos < word.Length)
            {
                _ = currentLine.Append(word[wordPos..]);
            }
        }
        if (currentLine.Length > 0)
            lines.Add(currentLine.ToString());
        return lines;
    }

    /// <summary>Разбивает строку на список "токенов", где каждый токен - это либо слово, либо последовательность пробелов.</summary>
    /// <param name="text">Входная строка.</param>
    /// <returns>Список токенов.</returns>
    private static List<string> Tokenize(string text)
    {
        var tokens = new List<string>();
        if (string.IsNullOrEmpty(text)) return tokens;

        int pos = 0;
        while (pos < text.Length)
        {
            int start = pos;
            bool isSpace = char.IsWhiteSpace(text[start]);
            while (pos < text.Length && char.IsWhiteSpace(text[pos]) == isSpace)
            {
                pos++;
            }
            tokens.Add(text.Substring(start, pos - start));
        }
        return tokens;
    }

    /// <summary>Разбивает значение на строки для отображения, учитывая длину prompt в первой строке и перенося по словам.</summary>
    private static List<string> WrapInputValue(string value, int promptLen, int maxWidth)
    {
        var lines = new List<string>();
        if (string.IsNullOrEmpty(value))
        {
            lines.Add("");
            return lines;
        }

        var tokens = Tokenize(value);
        var currentLine = new StringBuilder();
        int currentWidth = maxWidth - promptLen;

        foreach (var token in tokens)
        {
            if (token.Length > currentWidth && !char.IsWhiteSpace(token[0]))
            {
                if (currentLine.Length > 0)
                {
                    lines.Add(currentLine.ToString());
                    currentLine.Clear();
                    currentWidth = maxWidth;
                }
                int wordPos = 0;
                while (wordPos < token.Length)
                {
                    int take = Math.Min(currentWidth - currentLine.Length, token.Length - wordPos);
                    if (take <= 0)
                    {
                        lines.Add(currentLine.ToString());
                        currentLine.Clear();
                        currentWidth = maxWidth;
                        take = Math.Min(currentWidth, token.Length - wordPos);
                    }
                    currentLine.Append(token.Substring(wordPos, take));
                    wordPos += take;
                }
            }
            else if (currentLine.Length + token.Length > currentWidth)
            {
                lines.Add(currentLine.ToString());
                currentLine.Clear();
                currentWidth = maxWidth;
                if (!char.IsWhiteSpace(token[0]))
                {
                    currentLine.Append(token);
                }
            }
            else
            {
                currentLine.Append(token);
            }
        }

        if (currentLine.Length > 0)
        {
            lines.Add(currentLine.ToString());
        }

        if (lines.Count == 0)
        {
            lines.Add("");
        }

        return lines;
    }

    /// <summary>Вычисляет строку и столбец курсора для многострочного значения, используя ту же логику переноса по словам, что и WrapInputValue.</summary>
    /// <param name="value">Текущее значение поля ввода.</param>
    /// <param name="cursorPos">Позиция курсора в строке.</param>
    /// <param name="promptLen">Длина текста prompt.</param>
    /// <param name="maxWidth">Максимальная ширина строки.</param>
    /// <returns>Кортеж, содержащий строку и столбец курсора.</returns>
    private static (int line, int col) GetCursorLineCol(string value, int cursorPos, int promptLen, int maxWidth)
    {
        if (cursorPos == 0)
            return (0, 0);

        var tokens = Tokenize(value);

        int currentLineIndex = 0;
        var currentLineContent = new StringBuilder();
        int currentWidth = maxWidth - promptLen;
        int absolutePos = 0;

        foreach (var token in tokens)
        {
            if (cursorPos <= absolutePos + token.Length)
            {
                int remainingPos = cursorPos - absolutePos;
                int col = currentLineContent.Length;

                if (token.Length > currentWidth && !char.IsWhiteSpace(token[0]))
                {
                    int wordPartPos = 0;
                    while (wordPartPos < remainingPos)
                    {
                        int take = Math.Min(currentWidth - col, remainingPos - wordPartPos);
                        if (take <= 0)
                        {
                            currentLineIndex++;
                            col = 0;
                            currentWidth = maxWidth;
                            take = Math.Min(currentWidth, remainingPos - wordPartPos);
                        }
                        col += take;
                        wordPartPos += take;
                    }
                    return (currentLineIndex, col);
                }

                if (col + token.Length > currentWidth)
                {
                    if (!char.IsWhiteSpace(token[0]))
                    {
                        currentLineIndex++;
                        return (currentLineIndex, remainingPos);
                    }
                    else
                    {
                        currentLineIndex++;
                        return (currentLineIndex, 0);
                    }
                }

                col += remainingPos;
                return (currentLineIndex, col);
            }

            if (token.Length > currentWidth && !char.IsWhiteSpace(token[0]))
            {
                if (currentLineContent.Length > 0)
                {
                    currentLineIndex++;
                    currentLineContent.Clear();
                    currentWidth = maxWidth;
                }
                int wordPos = 0;
                while (wordPos < token.Length)
                {
                    int take = Math.Min(currentWidth - currentLineContent.Length, token.Length - wordPos);
                    if (take <= 0)
                    {
                        currentLineIndex++;
                        currentLineContent.Clear();
                        currentWidth = maxWidth;
                        take = Math.Min(currentWidth, token.Length - wordPos);
                    }
                    currentLineContent.Append(token.Substring(wordPos, take));
                    wordPos += take;
                }
            }
            else if (currentLineContent.Length + token.Length > currentWidth)
            {
                currentLineIndex++;
                currentLineContent.Clear();
                currentWidth = maxWidth;
                if (!char.IsWhiteSpace(token[0]))
                {
                    currentLineContent.Append(token);
                }
            }
            else
            {
                currentLineContent.Append(token);
            }

            absolutePos += token.Length;
        }

        return (currentLineIndex, currentLineContent.Length);
    }

    /// <summary>Отображает меню, обрабатывает ввод пользователя по каждому полю и обновляет значения в <see cref="MenuItems"/>.<br/>Позволяет перемещаться между полями, редактировать значения, отменять ввод или подтверждать результат.</summary>
    /// <param name="clear">Если <c>true</c>, очищает консоль перед отображением меню; если <c>false</c>, меню рисуется с текущей позиции курсора.</param>
    /// <returns>Словарь, где ключ — <see cref="InputMenuItem.Id"/>, а значение — соответствующий элемент <see cref="InputMenuItem"/> с обновлённым вводом пользователя.</returns>
    public Dictionary<string, InputMenuItem> Apply(bool clear = false)
    {
        List<string?> initialValues = [.. MenuItems.Select(item => item.InputValue)];
        int selected = 0;
        if (clear) Console.Clear();

        int menuTop = Console.CursorTop;
        Console.CursorVisible = true;

        int lineWidth = Console.WindowWidth > 1 ? Console.WindowWidth - 1 : 64;
        // --- Переносим заголовок по словам ---
        List<string> titleLines = WrapText(Title, lineWidth);

        // --- Переносим текст каждого prompt в элементе меню ---
        List<List<string>> promptLinesList = [.. MenuItems
            .Select(item =>
            {
                List<string> lines = WrapText(item.Text, lineWidth - 2);
                if (lines.Count == 0) lines.Add("");
                return lines;
            })];

        // --- Сохраняем высоту меню ---
        menuHeight = titleLines.Count + promptLinesList.Sum(l => l.Count);

        // --- Отрисовка заголовка ---
        Console.ForegroundColor = Theme.TitleTextColor;
        Console.BackgroundColor = Theme.TitleBackgroundColor;
        Console.SetCursorPosition(0, menuTop);
        foreach (string line in titleLines)
            Console.WriteLine(line.PadRight(lineWidth));

        void ClearMenuArea()
        {
            string cleaner = new(' ', lineWidth);
            for (int i = 0; i < menuHeight; i++)
            {
                Console.SetCursorPosition(0, menuTop + i);
                Console.Write(cleaner);
            }
            Console.SetCursorPosition(0, menuTop);
            Console.ResetColor();
        }

        // --- Новый метод для вычисления высоты элемента меню с учётом многострочного значения ---
        int GetMenuItemHeight(int index)
        {
            var promptLines = promptLinesList[index];
            var value = MenuItems[index].InputValue ?? "";
            var valueLines = WrapInputValue(value, promptLines[^1].Length + 2, lineWidth); // +2 для ": "
            // promptLines.Count - 1 строк без значения, последняя строка prompt + valueLines.Count
            return (promptLines.Count - 1) + valueLines.Count;
        }

        // --- Новый метод для вычисления смещения по вертикали для i-го элемента ---
        int GetMenuItemTop(int index)
        {
            int offset = titleLines.Count;
            for (int i = 0; i < index; i++)
                offset += GetMenuItemHeight(i);
            return menuTop + offset;
        }

        void DrawMenuLine(int i, bool isSelected)
        {
            int itemTop = GetMenuItemTop(i);
            InputMenuItem item = MenuItems[i];
            string value = item.InputValue ?? "";
            List<string> promptLines = promptLinesList[i];

            for (int lineIdx = 0; lineIdx < promptLines.Count; lineIdx++)
            {
                Console.SetCursorPosition(0, itemTop + lineIdx);

                bool isLastPromptLine = (lineIdx == promptLines.Count - 1);
                string prompt = isLastPromptLine ? promptLines[lineIdx] + ": " : promptLines[lineIdx];

                if (isSelected)
                {
                    Console.BackgroundColor = Theme.SelectedBackgroundColor;
                    Console.ForegroundColor = Theme.SelectedTextColor;
                    if (isLastPromptLine)
                    {
                        var valueLines = WrapInputValue(value, prompt.Length, lineWidth);
                        for (int v = 0; v < valueLines.Count; v++)
                        {
                            int y = itemTop + lineIdx + v;
                            Console.SetCursorPosition(0, y);
                            if (v == 0)
                            {
                                // Сначала prompt, затем значение
                                string line = prompt + valueLines[0];
                                Console.Write(line.PadRight(lineWidth));
                            }
                            else
                            {
                                Console.Write(valueLines[v].PadRight(lineWidth));
                            }
                        }
                    }
                    else Console.Write(prompt.PadRight(lineWidth));
                }
                else
                {
                    Console.BackgroundColor = Theme.UnselectedBackgroundColor;
                    Console.ForegroundColor = Theme.FieldTextColor;
                    if (isLastPromptLine)
                    {
                        var valueLines = WrapInputValue(value, prompt.Length, lineWidth);
                        for (int v = 0; v < valueLines.Count; v++)
                        {
                            int y = itemTop + lineIdx + v;
                            Console.SetCursorPosition(0, y);
                            if (v == 0)
                            {
                                string line = prompt + valueLines[0];
                                int promptLen = Math.Min(prompt.Length, lineWidth);
                                Console.Write(line.PadRight(lineWidth));
                            }
                            else
                            {
                                Console.Write(valueLines[v].PadRight(lineWidth));
                            }
                        }
                    }
                    else Console.Write(prompt.PadRight(lineWidth));
                }
                Console.ResetColor();
            }
        }

        void Redraw()
        {
            // Пересчитываем menuHeight с учётом многострочных значений
            menuHeight = titleLines.Count;
            for (int i = 0; i < MenuItems.Count; i++)
                menuHeight += GetMenuItemHeight(i);

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
            List<string> promptLines = promptLinesList[selected];
            string prompt = promptLines[^1] + ": ";
            StringBuilder inputBuilder = new(currentItem.InputValue ?? string.Empty);
            inputLeft = prompt.Length;
            // --- Корректируем позицию курсора с учётом высоты заголовка и многострочности prompt ---
            inputTop = GetMenuItemTop(selected) + promptLines.Count - 1;

            // --- Корректируем позицию курсора, если поле короче ---
            if (cursorPos > inputBuilder.Length)
                cursorPos = inputBuilder.Length;

            var (cursorLine, cursorCol) = GetCursorLineCol(inputBuilder.ToString(), cursorPos, inputLeft, lineWidth);
            Console.SetCursorPosition((cursorLine == 0 ? inputLeft : 0) + cursorCol, inputTop + cursorLine);
            Console.CursorVisible = true;

            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                bool breakInnerLoop = false;

                if (keyInfo.Key == ConsoleKey.UpArrow)
                {
                    MenuItems[selected].InputValue = inputBuilder.ToString();
                    selected = (selected - 1 + MenuItems.Count) % MenuItems.Count;
                    breakInnerLoop = true;
                }
                else if (keyInfo.Key == ConsoleKey.DownArrow)
                {
                    MenuItems[selected].InputValue = inputBuilder.ToString();
                    selected = (selected + 1) % MenuItems.Count;
                    breakInnerLoop = true;
                }
                else if (keyInfo.Key == ConsoleKey.Escape)
                {
                    for (int j = 0; j < MenuItems.Count; j++)
                        MenuItems[j].InputValue = initialValues[j];
                    Console.CursorVisible = false;
                    ClearMenuArea();
                    return MenuItems.ToDictionary(item => item.Id, item => item);
                }
                else if (keyInfo.Key == ConsoleKey.Enter)
                {
                    string input = inputBuilder.ToString();
                    MenuItems[selected].InputValue = input;
                    Console.CursorVisible = false;
                    ClearMenuArea();
                    return MenuItems.ToDictionary(item => item.Id, item => item);
                }
                else if (keyInfo.Key == ConsoleKey.Backspace)
                {
                    if (cursorPos > 0)
                    {
                        _ = inputBuilder.Remove(cursorPos - 1, 1);
                        cursorPos--;
                        MenuItems[selected].InputValue = inputBuilder.ToString();
                    }
                }
                else if (keyInfo.Key == ConsoleKey.Delete)
                {
                    if (cursorPos < inputBuilder.Length)
                    {
                        _ = inputBuilder.Remove(cursorPos, 1);
                        MenuItems[selected].InputValue = inputBuilder.ToString();
                    }
                }
                else if (keyInfo.Key == ConsoleKey.LeftArrow)
                {
                    if (cursorPos > 0)
                        cursorPos--;
                }
                else if (keyInfo.Key == ConsoleKey.RightArrow)
                {
                    if (cursorPos < inputBuilder.Length)
                        cursorPos++;
                }
                else if (!char.IsControl(keyInfo.KeyChar))
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
                        // --- Корректно вычисляем позицию курсора ---
                        var (line, col) = GetCursorLineCol(inputBuilder.ToString(), cursorPos, inputLeft, lineWidth);
                        Console.SetCursorPosition((line == 0 ? inputLeft : 0) + col, inputTop + line);
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
                            {
                                // Блокируем ввод точки, если значащих цифр уже максимум
                                string digitsOnly = inputBuilder.ToString().Replace("-", "").Replace(".", "").Replace(",", "");
                                int maxSignificant = GetSignificantLimit(currentItem.Type);
                                allowInput = digitsOnly.Length < maxSignificant;
                            }
                            break;
                        default:
                            allowInput = true;
                            break;
                    }

                    if (allowInput)
                    {
                        // --- Ограничение количества знаков после точки для дробных типов ---
                        if (currentItem.Type is EInputMenuItemType.Float or EInputMenuItemType.Double or EInputMenuItemType.Decimal
                            && char.IsDigit(keyInfo.KeyChar))
                        {
                            string text = inputBuilder.ToString();
                            int dotIdx = text.IndexOfAny(['.', ',']);
                            int intPartLen = (dotIdx >= 0 ? dotIdx : text.Length);

                            // Ограничения для decimal: максимум 29 знаков (28 дробных + 1 целая, но реально 28-29 знаков)
                            int maxDigits = GetSignificantLimit(currentItem.Type);

                            // Проверяем длину целой части (без минуса)
                            intPartLen = text.StartsWith("-") ? intPartLen - 1 : intPartLen;
                            if (dotIdx == -1 && text.StartsWith("-"))
                                intPartLen = text.Length - 1;

                            if (intPartLen >= maxDigits && cursorPos <= intPartLen)
                                continue; // Не даём ввести больше цифр в целой части

                            int limit = GetFractionLimit(currentItem.Type);
                            int fractionLen = text.Length - dotIdx - 1;
                            // учесть вставку в конец или в середину дробной части
                            if (cursorPos == text.Length)
                                fractionLen++;
                            else if (cursorPos > dotIdx)
                                fractionLen = Math.Max(fractionLen, cursorPos - dotIdx);

                            if (fractionLen > limit)
                                continue;

                            int maxSignificant = GetSignificantLimit(currentItem.Type);

                            // Считаем количество значащих цифр (без минуса и точки)
                            string digitsOnly = text.Replace("-", "").Replace(".", "").Replace(",", "");
                            int significantCount = digitsOnly.Length;

                            // Если курсор в целой части и значащих цифр уже максимум
                            if ((dotIdx == -1 || cursorPos <= dotIdx) && significantCount >= maxSignificant)
                            {
                                // Если есть дробная часть — удаляем последнюю цифру дробной части
                                if (dotIdx >= 0 && text.Length > dotIdx + 1)
                                {
                                    // Удаляем последнюю цифру дробной части
                                    int lastDigitIdx = text.Length - 1;
                                    while (lastDigitIdx > dotIdx && !char.IsDigit(text[lastDigitIdx]))
                                        lastDigitIdx--;
                                    if (lastDigitIdx > dotIdx)
                                    {
                                        _ = inputBuilder.Remove(lastDigitIdx, 1);
                                        if (cursorPos > lastDigitIdx) cursorPos--;
                                    }

                                    // Если после удаления дробной части осталась только точка — удаляем её
                                    string newText = inputBuilder.ToString();
                                    int newDotIdx = newText.IndexOfAny(['.', ',']);
                                    if (newDotIdx >= 0 && newDotIdx == newText.Length - 1)
                                    {
                                        _ = inputBuilder.Remove(newDotIdx, 1);
                                        if (cursorPos > newDotIdx) cursorPos--;
                                    }
                                }
                                else
                                {
                                    // Если дробной части нет — не даём ввести больше
                                    continue;
                                }
                            }

                            // Если значащих цифр уже максимум — не даём вводить новые цифры после точки
                            if (dotIdx >= 0 && cursorPos > dotIdx && significantCount >= maxSignificant)
                                continue;
                        }

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
                            var valLines = WrapInputValue(inputBuilder.ToString().Insert(cursorPos, keyInfo.KeyChar.ToString()), inputLeft, lineWidth);
                            if (valLines.Count + inputTop >= Console.BufferHeight)
                                continue; // Не даём выйти за пределы буфера по высоте
                            if (valLines.Any(l => l.Length > lineWidth))
                                continue; // Не даём выйти за пределы по ширине

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
                                    val = inputBuilder.ToString();

                                    // --- Сохраняем количество завершающих нулей и наличие точки ---
                                    int trailingZeros = 0;
                                    bool hasDot = false;
                                    bool endsWithDot = false;
                                    for (int i = val.Length - 1; i >= 0; i--)
                                    {
                                        if (val[i] == '0') trailingZeros++;
                                        else break;
                                    }
                                    hasDot = val.Contains('.') || val.Contains(',');
                                    endsWithDot = val.EndsWith(".") || val.EndsWith(",");

                                    isNegative = val.StartsWith("-");
                                    string rest = isNegative ? val[1..] : val;
                                    rest = rest.Replace(',', '.');

                                    // Удаляем все лишние точки, кроме первой
                                    int dotIndex = rest.IndexOf('.');
                                    if (dotIndex >= 0)
                                        rest = rest[..(dotIndex + 1)] + rest[(dotIndex + 1)..].Replace(".", "");

                                    // Не удаляем ведущий 0, если после него идёт точка (например, "0.0" или "0.")
                                    if (rest.Length > 1 && rest.StartsWith("0") && rest[1] != '.')
                                    {
                                        int firstNonZero = 0;
                                        while (firstNonZero < rest.Length && rest[firstNonZero] == '0') firstNonZero++;
                                        rest = firstNonZero == rest.Length ? "0" : rest[firstNonZero..];
                                    }
                                    if (rest == "" || rest.StartsWith(".")) rest = "0" + rest;

                                    // Если строка начинается с "0." (или "-0."), оставляем как есть
                                    val = rest.StartsWith("0.") && isNegative ? "-" + rest : rest.StartsWith("0.") ? rest : isNegative ? "-" + rest : rest;

                                    // --- Не парсим и не округляем, если пользователь явно вводит дробную часть ---
                                    // (то есть если есть точка и после неё есть хотя бы одна цифра, или строка заканчивается на точку)
                                    bool userTypingFraction = false;
                                    int pointPos = val.IndexOf('.');
                                    if (pointPos >= 0 && (pointPos < val.Length - 1 || endsWithDot))
                                        userTypingFraction = true;

                                    if (!userTypingFraction)
                                    {
                                        parsed = false;
                                        string format = "0.############################";
                                        switch (currentItem.Type)
                                        {
                                            case EInputMenuItemType.Float:
                                                parsed = float.TryParse(val, NumberStyles.Float, CultureInfo.InvariantCulture, out float f);
                                                val = parsed
                                                    ? float.IsPositiveInfinity(f) ? float.MaxValue.ToString(format, CultureInfo.InvariantCulture) : float.IsNegativeInfinity(f) ? float.MinValue.ToString(format, CultureInfo.InvariantCulture) : f.ToString(format, CultureInfo.InvariantCulture)
                                                    : isNegative ? float.MinValue.ToString(format, CultureInfo.InvariantCulture) : float.MaxValue.ToString(format, CultureInfo.InvariantCulture);
                                                break;
                                            case EInputMenuItemType.Double:
                                                parsed = double.TryParse(val, NumberStyles.Float, CultureInfo.InvariantCulture, out double d);
                                                val = parsed
                                                    ? d.ToString(format, CultureInfo.InvariantCulture)
                                                    : (isNegative ? double.MinValue.ToString(format, CultureInfo.InvariantCulture) : double.MaxValue.ToString(format, CultureInfo.InvariantCulture));
                                                break;
                                            case EInputMenuItemType.Decimal:
                                                parsed = decimal.TryParse(val, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal m);
                                                val = parsed
                                                    ? m.ToString(format, CultureInfo.InvariantCulture)
                                                    : (isNegative ? decimal.MinValue.ToString(format, CultureInfo.InvariantCulture) : decimal.MaxValue.ToString(format, CultureInfo.InvariantCulture));
                                                break;
                                        }
                                    }

                                    // --- Не добавляем завершающие нули вручную, они уже есть в строке ---

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

                        // --- После изменения inputBuilder и cursorPos ---
                        var (line, col) = GetCursorLineCol(inputBuilder.ToString(), cursorPos, inputLeft, lineWidth);
                        Console.SetCursorPosition((line == 0 ? inputLeft : 0) + col, inputTop + line);
                    }
                }

                Redraw();

                // Корректно вычисляем позицию курсора для многострочного значения
                var (lineFinal, colFinal) = GetCursorLineCol(inputBuilder.ToString(), cursorPos, inputLeft, lineWidth);
                Console.SetCursorPosition((lineFinal == 0 ? inputLeft : 0) + colFinal, inputTop + lineFinal);

                if (breakInnerLoop)
                    break;
            }
        }
    }

    /// <summary>Возвращает ограничение на количество дробных знаков для указанного типа поля.<br/>Используется для контроля точности при вводе числовых значений с плавающей точкой.</summary>
    /// <param name="type">Тип вводимого значения (<see cref="EInputMenuItemType"/>).</param>
    /// <returns>Максимально допустимое количество дробных знаков для данного типа.</returns>
    private static int GetFractionLimit(EInputMenuItemType type) => type switch
    {
        EInputMenuItemType.Float => 7,
        EInputMenuItemType.Double => 15,
        EInputMenuItemType.Decimal => 28,
        _ => int.MaxValue
    };

    /// <summary>Возвращает ограничение на количество значащих цифр для указанного типа поля.<br/>Используется для ограничения длины числового ввода.</summary>
    /// <param name="type">Тип вводимого значения (<see cref="EInputMenuItemType"/>).</param>
    /// <returns>Максимально допустимое количество значащих цифр для данного типа.</returns>
    private static int GetSignificantLimit(EInputMenuItemType type) => type switch
    {
        EInputMenuItemType.Float => 7,
        EInputMenuItemType.Double => 15,
        EInputMenuItemType.Decimal => 29,
        _ => int.MaxValue
    };
    #endregion

    #region Строитель
    /// <summary>Добавляет готовый элемент меню, проверяя уникальность его идентификатора.<br/>Если идентификатор не указан, он будет сгенерирован автоматически.</summary>
    /// <param name="item">Экземпляр <see cref="InputMenuItem"/> для добавления.</param>
    /// <param name="type">Тип вводимого значения для элемента меню. По умолчанию — <see cref="EInputMenuItemType.String"/>.</param>
    /// <returns>Текущий экземпляр <see cref="InputMenu"/> с добавленным элементом для дальнейшей настройки.</returns>
    /// <exception cref="ArgumentException">Выбрасывается, если элемент с таким идентификатором уже существует в меню.</exception>
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

    /// <summary>Добавляет новый элемент в меню ввода с заданными параметрами.<br/>Позволяет указать текст-приглашение, значение по умолчанию, идентификатор и тип поля.</summary>
    /// <param name="text">Текст-приглашение для добавляемого элемента.</param>
    /// <param name="defaultValue">Начальное значение поля ввода. По умолчанию — пустая строка.</param>
    /// <param name="id">Необязательный уникальный идентификатор. Если не указан, будет сгенерирован автоматически.</param>
    /// <param name="type">Тип вводимого значения (<see cref="EInputMenuItemType"/>). По умолчанию — строка.</param>
    /// <returns>Текущий экземпляр <see cref="InputMenu"/> с добавленным элементом для дальнейшей настройки.</returns>
    public InputMenu AddMenuItem(string text, string? defaultValue = "", string? id = null, EInputMenuItemType type = EInputMenuItemType.String) => AddMenuItem(new InputMenuItem(text, defaultValue, id), type);

    /// <summary>Заменяет заголовок меню на новый.<br/>Позволяет динамически изменять отображаемый заголовок.</summary>
    /// <param name="newTitle">Новый заголовок меню.</param>
    /// <returns>Текущий экземпляр <see cref="InputMenu"/> с обновлённым заголовком.</returns>
    public InputMenu SetTitle(string newTitle)
    {
        Title = newTitle;
        return this;
    }

    /// <summary>Устанавливает темы для данного меню.<br/>Позволяет задать отдельные темы для заголовка, активного (редактируемого) поля и неактивных полей.</summary>
    /// <param name="title">Тема для заголовка меню.</param>
    /// <param name="selected">Тема для активного (выделенного) поля ввода.</param>
    /// <param name="unselected">Тема для остальных (невыделенных) полей.</param>
    /// <returns>Текущий экземпляр <see cref="InputMenu"/> с обновлёнными темами.</returns>
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