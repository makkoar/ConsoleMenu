namespace ConsoleMenu.Managers;

/// <summary>Управляет всеми операциями отрисовки и компоновки элементов меню в консоли.</summary>
/// <remarks>Этот статический класс инкапсулирует логику взаимодействия с <see cref="System.Console"/>,<br/> предоставляя высокоуровневые методы для отрисовки заголовков, пунктов меню, полей ввода,<br/> а также утилиты для компоновки текста.</remarks>
internal static class RenderManager
{
    #region Свойства
    /// <summary>Возвращает текущую ширину окна консоли минус один символ для предотвращения переноса.<br/>Минимальное возвращаемое значение — 64.</summary>
    public static int WindowWidth => Console.WindowWidth > 1 ? Console.WindowWidth - 1 : 64;
    #endregion

    #region Управление состоянием консоли
    /// <summary>Устанавливает позицию курсора в консоли.</summary>
    /// <param name="left">Позиция столбца (от левого края).</param>
    /// <param name="top">Позиция строки (от верхнего края).</param>
    public static void SetCursorPosition(int left, int top) => Console.SetCursorPosition(left, top);

    /// <summary>Устанавливает видимость курсора в консоли.</summary>
    /// <param name="visible"><c>true</c>, чтобы сделать курсор видимым; иначе <c>false</c>.</param>
    public static void SetCursorVisibility(bool visible) => Console.CursorVisible = visible;

    /// <summary>Сбрасывает цвета консоли к значениям по умолчанию.</summary>
    public static void ResetColor() => Console.ResetColor();
    #endregion

    #region Компоновка текста
    /// <summary>Разбивает текст на строки по ширине окна консоли, перенося по словам и разбивая слишком длинные слова.</summary>
    /// <param name="text">Исходная строка, которую требуется разбить на несколько строк.</param>
    /// <param name="maxWidth">Максимальная ширина строки.</param>
    /// <returns>Список строк, каждая из которых не превышает указанную ширину.</returns>
    public static List<string> WrapText(string text, int maxWidth)
    {
        List<string> lines = [];
        string[] words = text.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        StringBuilder currentLine = new();

        foreach (string word in words)
        {
            if (currentLine.Length > 0)
            {
                if (currentLine.Length + 1 + word.Length > maxWidth)
                {
                    lines.Add(currentLine.ToString());
                    _ = currentLine.Clear();
                }
                else
                    _ = currentLine.Append(' ');
            }
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
                _ = currentLine.Append(word[wordPos..]);
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
        List<string> tokens = [];
        if (string.IsNullOrEmpty(text)) return tokens;

        int pos = 0;
        while (pos < text.Length)
        {
            int start = pos;
            bool isSpace = char.IsWhiteSpace(text[start]);
            while (pos < text.Length && char.IsWhiteSpace(text[pos]) == isSpace)
                pos++;
            tokens.Add(text[start..pos]);
        }
        return tokens;
    }

    /// <summary>Разбивает значение на строки для отображения, учитывая длину prompt в первой строке и перенося по словам.</summary>
    /// <param name="value">Входное значение для переноса.</param>
    /// <param name="promptLen">Длина текста-приглашения, которая занимает место в первой строке.</param>
    /// <param name="maxWidth">Максимальная ширина строки.</param>
    /// <returns>Список строк, представляющих отформатированное значение.</returns>
    public static List<string> WrapInputValue(string value, int promptLen, int maxWidth)
    {
        List<string> lines = [];
        if (string.IsNullOrEmpty(value))
        {
            lines.Add(string.Empty);
            return lines;
        }

        List<string> tokens = Tokenize(value);
        StringBuilder currentLine = new();
        int currentWidth = maxWidth - promptLen;

        foreach (string token in tokens)
        {
            if (currentLine.Length > 0 && currentLine.Length + token.Length > currentWidth)
            {
                lines.Add(currentLine.ToString());
                _ = currentLine.Clear();
                currentWidth = maxWidth;
            }

            if (token.Length > currentWidth)
            {
                int tokenPos = 0;
                while (tokenPos < token.Length)
                {
                    int take = Math.Min(currentWidth, token.Length - tokenPos);
                    lines.Add(token.Substring(tokenPos, take));
                    tokenPos += take;
                }
            }
            else
                _ = currentLine.Append(token);
        }

        if (currentLine.Length > 0)
            lines.Add(currentLine.ToString());

        if (lines.Count is 0)
            lines.Add(string.Empty);

        return lines;
    }

    /// <summary>Вычисляет строку и столбец курсора для многострочного значения.</summary>
    /// <param name="value">Текущее значение поля ввода.</param>
    /// <param name="cursorPos">Позиция курсора в строке.</param>
    /// <param name="promptLen">Длина текста prompt.</param>
    /// <param name="maxWidth">Максимальная ширина строки.</param>
    /// <returns>Кортеж, содержащий строку и столбец курсора.</returns>
    public static (int line, int col) GetCursorLineCol(string value, int cursorPos, int promptLen, int maxWidth)
    {
        if (cursorPos <= 0)
            return (0, 0);

        List<string> tokens = Tokenize(value);

        int line = 0;
        int col = 0;
        int absolutePos = 0;
        int currentWidth = maxWidth - promptLen;

        foreach (string token in tokens)
        {
            if (cursorPos <= absolutePos + token.Length)
            {
                int localPos = cursorPos - absolutePos;

                if (col > 0 && col + token.Length > currentWidth)
                {
                    line++;
                    col = 0;
                    currentWidth = maxWidth;
                }

                if (token.Length > currentWidth)
                {
                    line += localPos / currentWidth;
                    col = localPos % currentWidth;
                }
                else
                    col += localPos;

                return (line, col);
            }

            if (col > 0 && col + token.Length > currentWidth)
            {
                line++;
                col = 0;
                currentWidth = maxWidth;
            }

            if (token.Length > currentWidth)
            {
                line += (token.Length - 1) / currentWidth;
                col = token.Length % currentWidth;
                if (col is 0 && token.Length > 0)
                {
                    col = currentWidth;
                    line--;
                }
            }
            else
                col += token.Length;

            absolutePos += token.Length;
        }

        return (line, col);
    }
    #endregion

    #region Отрисовка
    /// <summary>Очищает указанную область в консоли, заполняя её пробелами.</summary>
    /// <param name="top">Начальная строка для очистки.</param>
    /// <param name="height">Количество строк для очистки.</param>
    /// <param name="width">Ширина очищаемой области.</param>
    public static void ClearArea(int top, int height, int width)
    {
        string cleaner = new(' ', width);
        for (int i = 0; i < height; i++)
        {
            SetCursorPosition(0, top + i);
            Console.Write(cleaner);
        }
        SetCursorPosition(0, top);
    }

    /// <summary>Отрисовывает заголовок меню.</summary>
    /// <param name="titleLines">Список строк заголовка для отрисовки.</param>
    /// <param name="theme">Тема оформления.</param>
    /// <param name="menuTop">Начальная позиция меню по вертикали.</param>
    /// <param name="lineWidth">Ширина строки для отрисовки.</param>
    public static void DrawTitle(List<string> titleLines, Theme theme, int menuTop, int lineWidth)
    {
        Console.ForegroundColor = theme.TitleTextColor;
        Console.BackgroundColor = theme.TitleBackgroundColor;
        SetCursorPosition(0, menuTop);
        foreach (string line in titleLines)
            Console.WriteLine(line.PadRight(lineWidth));
        ResetColor();
    }

    /// <summary>Отрисовывает полное меню выбора (<see cref="SelectMenu"/>) с поддержкой многострочных элементов.</summary>
    /// <param name="titleLines">Список строк заголовка для отрисовки.</param>
    /// <param name="menuItems">Список элементов меню для отображения.</param>
    /// <param name="itemLinesList">Список, содержащий списки строк для каждого элемента меню.</param>
    /// <param name="selectedIndex">Индекс текущего выбранного элемента.</param>
    /// <param name="theme">Тема оформления.</param>
    /// <param name="menuTop">Начальная позиция меню по вертикали.</param>
    public static void DrawSelectMenu(List<string> titleLines, List<SelectMenuItem> menuItems, List<List<string>> itemLinesList, ushort selectedIndex, Theme theme, int menuTop)
    {
        SetCursorPosition(0, menuTop);

        Console.ForegroundColor = theme.TitleTextColor;
        Console.BackgroundColor = theme.TitleBackgroundColor;
        for (int i = 0; i < titleLines.Count; i++)
        {
            SetCursorPosition(0, menuTop + i);
            Console.Write(titleLines[i].PadRight(WindowWidth));
        }

        int currentItemTopOffset = titleLines.Count;
        for (ushort i = 0; i < menuItems.Count; i++)
        {
            Console.ForegroundColor = i == selectedIndex ? theme.SelectedTextColor : theme.UnselectedTextColor;
            Console.BackgroundColor = i == selectedIndex ? theme.SelectedBackgroundColor : theme.UnselectedBackgroundColor;

            List<string> lines = itemLinesList[i];
            for (int j = 0; j < lines.Count; j++)
            {
                SetCursorPosition(0, menuTop + currentItemTopOffset + j);
                string prefix = j is 0 ? "* " : "  ";
                Console.Write((prefix + lines[j]).PadRight(WindowWidth));
            }
            currentItemTopOffset += lines.Count;
        }
        ResetColor();
    }

    /// <summary>Отрисовывает один элемент меню ввода (<see cref="InputMenuItem"/>).</summary>
    /// <param name="item">Элемент меню для отрисовки.</param>
    /// <param name="isSelected"><c>true</c>, если элемент является выбранным в данный момент.</param>
    /// <param name="promptLines">Список строк текста-приглашения.</param>
    /// <param name="theme">Тема оформления.</param>
    /// <param name="itemTop">Начальная позиция элемента по вертикали.</param>
    /// <param name="lineWidth">Ширина строки для отрисовки.</param>
    public static void DrawInputMenuItem(InputMenuItem item, bool isSelected, List<string> promptLines, Theme theme, int itemTop, int lineWidth)
    {
        string value = item.InputValue ?? string.Empty;

        for (int lineIdx = 0; lineIdx < promptLines.Count; lineIdx++)
        {
            SetCursorPosition(0, itemTop + lineIdx);

            bool isLastPromptLine = (lineIdx == promptLines.Count - 1);
            string prompt = isLastPromptLine ? promptLines[lineIdx] + ": " : promptLines[lineIdx];

            if (isSelected)
            {
                Console.BackgroundColor = theme.SelectedBackgroundColor;
                Console.ForegroundColor = theme.SelectedTextColor;
                if (isLastPromptLine)
                {
                    List<string> valueLines = WrapInputValue(value, prompt.Length, lineWidth);
                    for (int v = 0; v < valueLines.Count; v++)
                    {
                        int y = itemTop + lineIdx + v;
                        SetCursorPosition(0, y);
                        string line = (v is 0) ? prompt + valueLines[0] : valueLines[v];
                        Console.Write(line.PadRight(lineWidth));
                    }
                }
                else
                    Console.Write(prompt.PadRight(lineWidth));
            }
            else
            {
                Console.BackgroundColor = theme.UnselectedBackgroundColor;
                Console.ForegroundColor = theme.FieldTextColor;
                if (isLastPromptLine)
                {
                    List<string> valueLines = WrapInputValue(value, prompt.Length, lineWidth);
                    for (int v = 0; v < valueLines.Count; v++)
                    {
                        int y = itemTop + lineIdx + v;
                        SetCursorPosition(0, y);
                        string line = (v is 0) ? prompt + valueLines[0] : valueLines[v];
                        Console.Write(line.PadRight(lineWidth));
                    }
                }
                else
                    Console.Write(prompt.PadRight(lineWidth));
            }
            ResetColor();
        }
    }
    #endregion
}