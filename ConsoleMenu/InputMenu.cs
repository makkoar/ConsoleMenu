namespace ConsoleMenu;

/// <summary>Класс, реализующий консольное меню для ввода данных пользователем по нескольким полям с поддержкой темизации, проверки уникальности идентификаторов и гибкой настройки элементов.<br/>Позволяет организовать пошаговый ввод значений с валидацией и возвратом результатов в виде словаря.</summary>
public class InputMenu()
{
    #region Приватные поля
    /// <summary>Хранит использованные ID для обеспечения уникальности среди элементов меню.<br/>Используется для предотвращения дублирования идентификаторов.</summary>
    private readonly HashSet<string> usedIds = [];

    /// <summary>Счётчик для генерации уникальных ID по умолчанию для новых элементов меню.<br/>Автоматически увеличивается при добавлении элементов без явного ID.</summary>
    private uint nextDefaultId = 0;

    /// <summary>Кэшированное значение общей высоты меню в строках.<br/>Рассчитывается динамически в методе <see cref="Apply(bool)"/> и используется для корректной очистки области меню при перерисовке.</summary>
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
    /// <summary>Отображает меню, обрабатывает ввод пользователя для каждого поля и возвращает итоговые значения.<br/>Управление в меню:<br/><list type="bullet"><item><description>Стрелки вверх/вниз: Перемещение между полями ввода. Если поле многострочное, сначала происходит навигация внутри него.</description></item><item><description>Стрелки влево/вправо, Backspace, Delete: Редактирование текста в выбранном поле.</description></item><item><description>Enter: Подтверждение всех введённых значений и выход из меню.</description></item><item><description>Escape: Отмена всех изменений (возврат к начальным значениям) и выход из меню.</description></item></list></summary>
    /// <param name="clear">Если <c>true</c>, очищает консоль перед отображением меню; если <c>false</c>, меню рисуется с текущей позиции курсора.</param>
    /// <returns>Словарь, где ключ — <see cref="InputMenuItem.Id"/>, а значение — соответствующий элемент <see cref="InputMenuItem"/> с обновлённым вводом пользователя.</returns>
    public Dictionary<string, InputMenuItem> Apply(bool clear = false)
    {
        List<string> initialValues = [.. MenuItems.Select(item => item.InputValue)];
        int selected = 0;
        int oldHeight;
        if (clear) Console.Clear();

        int menuTop = Console.CursorTop;

        List<string> titleLines = RenderManager.WrapText(Title, RenderManager.WindowWidth);

        List<List<string>> promptLinesList = [.. MenuItems
            .Select(item =>
            {
                List<string> lines = RenderManager.WrapText(item.Text, RenderManager.WindowWidth - 2);
                if (lines.Count is 0) lines.Add(string.Empty);
                return lines;
            })];

        menuHeight = titleLines.Count + promptLinesList.Sum(l => l.Count);

        RenderManager.DrawTitle(titleLines, Theme, menuTop, RenderManager.WindowWidth);

        void ClearMenuArea()
        {
            RenderManager.ClearArea(menuTop, menuHeight, RenderManager.WindowWidth);
            RenderManager.ResetColor();
        }

        int GetMenuItemHeight(int index)
        {
            List<string> promptLines = promptLinesList[index];
            string value = MenuItems[index].InputValue ?? string.Empty;
            List<string> valueLines = RenderManager.WrapInputValue(value, promptLines[^1].Length + 2, RenderManager.WindowWidth);
            return (promptLines.Count - 1) + valueLines.Count;
        }

        int GetMenuItemTop(int index)
        {
            int offset = titleLines.Count;
            for (int i = 0; i < index; i++)
                offset += GetMenuItemHeight(i);
            return menuTop + offset;
        }

        void Redraw()
        {
            menuHeight = titleLines.Count;
            for (int i = 0; i < MenuItems.Count; i++)
                menuHeight += GetMenuItemHeight(i);

            for (int i = 0; i < MenuItems.Count; i++)
                RenderManager.DrawInputMenuItem(MenuItems[i], i == selected, promptLinesList[i], Theme, GetMenuItemTop(i), RenderManager.WindowWidth);
        }

        Redraw();

        int cursorPos = 0;
        int savedHorizontalColumn = 0;

        if (MenuItems.Count > 0)
            cursorPos = (MenuItems[0].InputValue ?? string.Empty).Length;

        while (true)
        {
            InputMenuItem currentItem = MenuItems[selected];
            List<string> promptLines = promptLinesList[selected];
            StringBuilder inputBuilder = new(currentItem.InputValue ?? string.Empty);

            RenderManager.SetCursorVisibility(currentItem.Type is not EInputMenuItemType.Bool);

            if (currentItem.Type is not EInputMenuItemType.Bool)
            {
                if (cursorPos > inputBuilder.Length)
                    cursorPos = inputBuilder.Length;

                savedHorizontalColumn = RenderManager.UpdateCursorPosition(inputBuilder.ToString(), cursorPos, promptLines, GetMenuItemTop(selected), RenderManager.WindowWidth);
            }

            while (true)
            {
                ConsoleKeyInfo keyInfo = Console.ReadKey(true);

                bool breakInnerLoop = false;
                string currentValue = inputBuilder.ToString();
                int promptLen = promptLines[^1].Length + 2;

                if (keyInfo.Key is ConsoleKey.UpArrow)
                {
                    if (currentItem.Type is EInputMenuItemType.Bool)
                    {
                        selected = (selected - 1 + MenuItems.Count) % MenuItems.Count;
                        breakInnerLoop = true;
                    }
                    else
                    {
                        CursorPosition currentPos = RenderManager.GetCursorLineCol(currentValue, cursorPos, promptLen, RenderManager.WindowWidth);
                        if (currentPos.Line > 0)
                        {
                            int targetLine = currentPos.Line - 1;
                            int p = 0;
                            while (p <= currentValue.Length)
                            {
                                CursorPosition pos = RenderManager.GetCursorLineCol(currentValue, p, promptLen, RenderManager.WindowWidth);
                                int screenCol = pos.Column + (pos.Line is 0 ? promptLen : 0);
                                if (pos.Line == targetLine && screenCol >= savedHorizontalColumn) break;
                                if (pos.Line > targetLine) break;
                                p++;
                            }
                            if (p > 0 && RenderManager.GetCursorLineCol(currentValue, p, promptLen, RenderManager.WindowWidth).Line > targetLine) p--;
                            cursorPos = p;
                        }
                        else
                        {
                            MenuItems[selected].InputValue = currentValue;
                            selected = (selected - 1 + MenuItems.Count) % MenuItems.Count;
                            string prevValue = MenuItems[selected].InputValue ?? string.Empty;
                            int prevPromptLen = promptLinesList[selected][^1].Length + 2;
                            CursorPosition lastLinePos = RenderManager.GetCursorLineCol(prevValue, prevValue.Length, prevPromptLen, RenderManager.WindowWidth);
                            int p = 0;
                            while (p <= prevValue.Length)
                            {
                                CursorPosition pos = RenderManager.GetCursorLineCol(prevValue, p, prevPromptLen, RenderManager.WindowWidth);
                                int screenCol = pos.Column + (pos.Line is 0 ? prevPromptLen : 0);
                                if (pos.Line == lastLinePos.Line && screenCol >= savedHorizontalColumn) break;
                                if (pos.Line > lastLinePos.Line) break;
                                p++;
                            }
                            if (p > 0 && RenderManager.GetCursorLineCol(prevValue, p, prevPromptLen, RenderManager.WindowWidth).Line > lastLinePos.Line) p--;
                            cursorPos = p > prevValue.Length ? prevValue.Length : p;
                            breakInnerLoop = true;
                        }
                    }
                }
                else if (keyInfo.Key is ConsoleKey.DownArrow)
                {
                    if (currentItem.Type is EInputMenuItemType.Bool)
                    {
                        selected = (selected + 1) % MenuItems.Count;
                        breakInnerLoop = true;
                    }
                    else
                    {
                        CursorPosition lastLinePos = RenderManager.GetCursorLineCol(currentValue, currentValue.Length, promptLen, RenderManager.WindowWidth);
                        CursorPosition currentPos = RenderManager.GetCursorLineCol(currentValue, cursorPos, promptLen, RenderManager.WindowWidth);
                        if (currentPos.Line < lastLinePos.Line)
                        {
                            int targetLine = currentPos.Line + 1;
                            int p = 0;
                            while (p <= currentValue.Length)
                            {
                                CursorPosition pos = RenderManager.GetCursorLineCol(currentValue, p, promptLen, RenderManager.WindowWidth);
                                int screenCol = pos.Column + (pos.Line is 0 ? promptLen : 0);
                                if (pos.Line == targetLine && screenCol >= savedHorizontalColumn) break;
                                if (pos.Line > targetLine) break;
                                p++;
                            }
                            if (p > 0 && RenderManager.GetCursorLineCol(currentValue, p, promptLen, RenderManager.WindowWidth).Line > targetLine) p--;
                            cursorPos = p;
                        }
                        else
                        {
                            MenuItems[selected].InputValue = currentValue;
                            selected = (selected + 1) % MenuItems.Count;
                            string nextValue = MenuItems[selected].InputValue ?? string.Empty;
                            int nextPromptLen = promptLinesList[selected][^1].Length + 2;
                            int p = 0;
                            while (p <= nextValue.Length)
                            {
                                CursorPosition pos = RenderManager.GetCursorLineCol(nextValue, p, nextPromptLen, RenderManager.WindowWidth);
                                int screenCol = pos.Column + (pos.Line is 0 ? nextPromptLen : 0);
                                if (pos.Line is 0 && screenCol >= savedHorizontalColumn) break;
                                if (pos.Line > 0) break;
                                p++;
                            }
                            if (p > 0 && RenderManager.GetCursorLineCol(nextValue, p, nextPromptLen, RenderManager.WindowWidth).Line > 0) p--;
                            cursorPos = p > nextValue.Length ? nextValue.Length : p;
                            breakInnerLoop = true;
                        }
                    }
                }
                else if (keyInfo.Key is ConsoleKey.Escape)
                {
                    for (int j = 0; j < MenuItems.Count; j++)
                        MenuItems[j].InputValue = initialValues[j];
                    RenderManager.SetCursorVisibility(false);
                    ClearMenuArea();
                    return MenuItems.ToDictionary(item => item.Id ?? throw new ArgumentException("По каким-то причинам Id оказался не задан"), item => item);
                }
                else if (keyInfo.Key is ConsoleKey.Enter)
                {
                    MenuItems[selected].InputValue = inputBuilder.ToString();
                    RenderManager.SetCursorVisibility(false);
                    ClearMenuArea();
                    return MenuItems.ToDictionary(item => item.Id ?? throw new ArgumentException("По каким-то причинам Id оказался не задан"), item => item);
                }
                else if (keyInfo.Key is ConsoleKey.Backspace)
                {
                    if (currentItem.Type is not EInputMenuItemType.Bool && cursorPos > 0)
                    {
                        _ = inputBuilder.Remove(cursorPos - 1, 1);
                        cursorPos--;
                        MenuItems[selected].InputValue = inputBuilder.ToString();
                    }
                }
                else if (keyInfo.Key is ConsoleKey.Delete)
                {
                    if (currentItem.Type is not EInputMenuItemType.Bool && cursorPos < inputBuilder.Length)
                    {
                        _ = inputBuilder.Remove(cursorPos, 1);
                        MenuItems[selected].InputValue = inputBuilder.ToString();
                    }
                }
                else if (keyInfo.Key is ConsoleKey.LeftArrow)
                {
                    if (cursorPos > 0)
                        if ((keyInfo.Modifiers & ConsoleModifiers.Control) is not 0)
                        {
                            int p = cursorPos - 1;
                            while (p > 0 && char.IsWhiteSpace(inputBuilder[p])) p--;
                            while (p > 0 && !char.IsWhiteSpace(inputBuilder[p - 1])) p--;
                            cursorPos = p;
                        }
                        else cursorPos--;
                }
                else if (keyInfo.Key is ConsoleKey.RightArrow)
                {
                    if (cursorPos < inputBuilder.Length)
                        if ((keyInfo.Modifiers & ConsoleModifiers.Control) is not 0)
                        {
                            int p = cursorPos;
                            while (p < inputBuilder.Length && char.IsWhiteSpace(inputBuilder[p])) p++;
                            while (p < inputBuilder.Length && !char.IsWhiteSpace(inputBuilder[p])) p++;
                            cursorPos = p;
                        }
                        else cursorPos++;
                }
                else if (keyInfo.Key == ConsoleKey.Home)
                {
                    cursorPos = 0;
                }
                else if (keyInfo.Key == ConsoleKey.End)
                {
                    cursorPos = inputBuilder.Length;
                }
                else if (!char.IsControl(keyInfo.KeyChar))
                {
                    if (currentItem.Type is EInputMenuItemType.Bool)
                    {
                        if (keyInfo.Key is ConsoleKey.Spacebar)
                        {
                            _ = bool.TryParse(inputBuilder.ToString(), out bool currentBoolValue);
                            _ = inputBuilder.Clear().Append(!currentBoolValue);
                        }
                        else if (keyInfo.KeyChar is 't' or 'T')
                            _ = inputBuilder.Clear().Append("True");
                        else if (keyInfo.KeyChar is 'f' or 'F')
                            _ = inputBuilder.Clear().Append("False");
                        MenuItems[selected].InputValue = inputBuilder.ToString();
                    }
                    else
                    {
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
                            oldHeight = menuHeight;
                            Redraw();
                            if (menuHeight < oldHeight)
                                RenderManager.ClearArea(menuTop + menuHeight, oldHeight - menuHeight, RenderManager.WindowWidth);
                            savedHorizontalColumn = RenderManager.UpdateCursorPosition(inputBuilder.ToString(), cursorPos, promptLines, GetMenuItemTop(selected), RenderManager.WindowWidth);
                            continue;
                        }

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
                                else if (keyInfo.KeyChar is '-' && cursorPos is 0 && !inputBuilder.ToString().Contains('-'))
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
                                else if (keyInfo.KeyChar is '-' && cursorPos is 0 && !inputBuilder.ToString().Contains('-'))
                                    allowInput = true;
                                else if ((keyInfo.KeyChar is '.' or ',') &&
                                         !inputBuilder.ToString().Contains('.') && !inputBuilder.ToString().Contains(','))
                                {
                                    string digitsOnly = inputBuilder.ToString().Replace("-", string.Empty).Replace(".", string.Empty).Replace(",", string.Empty);
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
                            if (currentItem.Type is EInputMenuItemType.Float or EInputMenuItemType.Double or EInputMenuItemType.Decimal
                                && char.IsDigit(keyInfo.KeyChar))
                            {
                                string text = inputBuilder.ToString();
                                int dotIdx = text.IndexOfAny(['.', ',']);
                                int intPartLen = (dotIdx >= 0 ? dotIdx : text.Length);

                                int maxDigits = GetSignificantLimit(currentItem.Type);

                                intPartLen = text.StartsWith("-") ? intPartLen - 1 : intPartLen;
                                if (dotIdx is -1 && text.StartsWith("-"))
                                    intPartLen = text.Length - 1;

                                if (intPartLen >= maxDigits && cursorPos <= intPartLen)
                                    continue;

                                int limit = GetFractionLimit(currentItem.Type);
                                int fractionLen = text.Length - dotIdx - 1;
                                if (cursorPos == text.Length)
                                    fractionLen++;
                                else if (cursorPos > dotIdx)
                                    fractionLen = Math.Max(fractionLen, cursorPos - dotIdx);

                                if (fractionLen > limit)
                                    continue;

                                int maxSignificant = GetSignificantLimit(currentItem.Type);

                                string digitsOnly = text.Replace("-", string.Empty).Replace(".", string.Empty).Replace(",", string.Empty);
                                int significantCount = digitsOnly.Length;

                                if ((dotIdx is -1 || cursorPos <= dotIdx) && significantCount >= maxSignificant)
                                {
                                    if (dotIdx >= 0 && text.Length > dotIdx + 1)
                                    {
                                        int lastDigitIdx = text.Length - 1;
                                        while (lastDigitIdx > dotIdx && !char.IsDigit(text[lastDigitIdx]))
                                            lastDigitIdx--;
                                        if (lastDigitIdx > dotIdx)
                                        {
                                            _ = inputBuilder.Remove(lastDigitIdx, 1);
                                            if (cursorPos > lastDigitIdx) cursorPos--;
                                        }

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
                                        continue;
                                    }
                                }

                                if (dotIdx >= 0 && cursorPos > dotIdx && significantCount >= maxSignificant)
                                    continue;
                            }

                            minusInserted = false;
                            if ((currentItem.Type is
                                EInputMenuItemType.Int or EInputMenuItemType.Short or EInputMenuItemType.SByte or
                                EInputMenuItemType.Float or EInputMenuItemType.Double or EInputMenuItemType.Decimal)
                                && keyInfo.KeyChar is '-')
                            {
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
                                List<string> valLines = RenderManager.WrapInputValue(inputBuilder.ToString().Insert(cursorPos, keyInfo.KeyChar.ToString()), promptLen, RenderManager.WindowWidth);
                                if (valLines.Count + GetMenuItemTop(selected) + promptLines.Count - 1 >= Console.BufferHeight)
                                    continue;
                                if (valLines.Any(l => l.Length > RenderManager.WindowWidth))
                                    continue;

                                _ = inputBuilder.Insert(cursorPos, keyInfo.KeyChar);
                                cursorPos++;
                            }

                            string val;
                            bool isNegative, parsed;
                            int oldLength;
                            switch (currentItem.Type)
                            {
                                case EInputMenuItemType.Int:
                                case EInputMenuItemType.Short:
                                case EInputMenuItemType.SByte:
                                    {
                                        val = inputBuilder.ToString();
                                        if (val is "-")
                                            break;
                                        isNegative = val.StartsWith("-");
                                        string numberPart = val[(isNegative ? 1 : 0)..].TrimStart('0');
                                        if (numberPart is "" or "0")
                                            numberPart = "0";
                                        val = isNegative ? (numberPart is "0" ? "-0" : "-" + numberPart) : numberPart;

                                        if (BigInteger.TryParse(val, out BigInteger bigVal))
                                        {
                                            switch (currentItem.Type)
                                            {
                                                case EInputMenuItemType.Int:
                                                    if (bigVal < int.MinValue) bigVal = int.MinValue;
                                                    if (bigVal > int.MaxValue) bigVal = int.MaxValue;
                                                    val = ((int)bigVal).ToString();
                                                    if (isNegative && numberPart is "0") val = "-0";
                                                    break;
                                                case EInputMenuItemType.Short:
                                                    if (bigVal < short.MinValue) bigVal = short.MinValue;
                                                    if (bigVal > short.MaxValue) bigVal = short.MaxValue;
                                                    val = ((short)bigVal).ToString();
                                                    if (isNegative && numberPart is "0") val = "-0";
                                                    break;
                                                case EInputMenuItemType.SByte:
                                                    if (bigVal < sbyte.MinValue) bigVal = sbyte.MinValue;
                                                    if (bigVal > sbyte.MaxValue) bigVal = sbyte.MaxValue;
                                                    val = ((sbyte)bigVal).ToString();
                                                    if (isNegative && numberPart is "0") val = "-0";
                                                    break;
                                            }
                                        }

                                        if (inputBuilder.ToString() is "-")
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
                                        if (val is "") val = "0";
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

                                        bool endsWithDot = val.EndsWith(".") || val.EndsWith(",");

                                        isNegative = val.StartsWith("-");
                                        string rest = isNegative ? val[1..] : val;
                                        rest = rest.Replace(',', '.');

                                        int dotIndex = rest.IndexOf('.');
                                        if (dotIndex >= 0)
                                            rest = rest[..(dotIndex + 1)] + rest[(dotIndex + 1)..].Replace(".", string.Empty);

                                        if (rest.Length > 1 && rest.StartsWith("0") && rest[1] is '.')
                                        {
                                            int firstNonZero = 0;
                                            while (firstNonZero < rest.Length && rest[firstNonZero] == '0') firstNonZero++;
                                            rest = firstNonZero == rest.Length ? "0" : rest[firstNonZero..];
                                        }
                                        if (rest is "" || rest.StartsWith(".")) rest = "0" + rest;

                                        val = rest.StartsWith("0.") && isNegative ? "-" + rest : rest.StartsWith("0.") ? rest : isNegative ? "-" + rest : rest;

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
                        }
                    }
                }

                oldHeight = menuHeight;
                Redraw();
                if (menuHeight < oldHeight)
                    RenderManager.ClearArea(menuTop + menuHeight, oldHeight - menuHeight, RenderManager.WindowWidth);

                if (currentItem.Type is not EInputMenuItemType.Bool)
                    savedHorizontalColumn = RenderManager.UpdateCursorPosition(inputBuilder.ToString(), cursorPos, promptLines, GetMenuItemTop(selected), RenderManager.WindowWidth);

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
    /// <param name="id">Необязательный идентификатор, который будет присвоен элементу, переопределяя существующий в <paramref name="item"/>.</param>
    /// <returns>Текущий экземпляр <see cref="InputMenu"/> с добавленным элементом для дальнейшей настройки.</returns>
    /// <exception cref="ArgumentException">Выбрасывается, если элемент с таким идентификатором уже существует в меню.</exception>
    public InputMenu AddMenuItem(InputMenuItem item, string? id = null)
    {
        if (string.IsNullOrEmpty(item.Id = id))
            item.Id = nextDefaultId.ToString();

        if (!usedIds.Add(item.Id))
            throw new ArgumentException($"Элемент с ID '{item.Id}' уже существует в этом меню.", nameof(item));

        MenuItems.Add(item);

        if (uint.TryParse(item.Id, out uint numericId))
            nextDefaultId = Math.Max(nextDefaultId, numericId + 1);
        else if (item.Id == (nextDefaultId - 1 > 0 ? nextDefaultId - 1 : 0).ToString())
            nextDefaultId++;

        return this;
    }

    /// <summary>Добавляет новый элемент в меню ввода с заданными параметрами.<br/>Позволяет указать текст-приглашение, значение по умолчанию, идентификатор и тип поля.</summary>
    /// <param name="text">Текст-приглашение для добавляемого элемента.</param>
    /// <param name="defaultValue">Начальное значение поля ввода. По умолчанию — пустая строка.</param>
    /// <param name="id">Необязательный уникальный идентификатор. Если не указан, будет сгенерирован автоматически.</param>
    /// <returns>Текущий экземпляр <see cref="InputMenu"/> с добавленным элементом для дальнейшей настройки.</returns>
    public InputMenu AddMenuItem(string text, string defaultValue = "", string? id = null) => AddMenuItem(new InputMenuItem(text, defaultValue), id);

    /// <summary>Добавляет новый элемент в меню ввода с заданными параметрами.<br/>Позволяет указать текст-приглашение, значение по умолчанию, идентификатор и тип поля.</summary>
    /// <param name="text">Текст-приглашение для добавляемого элемента.</param>
    /// <param name="defaultValue">Начальное значение поля ввода. По умолчанию — 0.</param>
    /// <param name="id">Необязательный уникальный идентификатор. Если не указан, будет сгенерирован автоматически.</param>
    /// <returns>Текущий экземпляр <see cref="InputMenu"/> с добавленным элементом для дальнейшей настройки.</returns>
    public InputMenu AddMenuItem(string text, int defaultValue = default, string? id = null) => AddMenuItem(new InputMenuItem(text, defaultValue), id);

    /// <summary>Добавляет новый элемент в меню ввода с заданными параметрами.<br/>Позволяет указать текст-приглашение, значение по умолчанию, идентификатор и тип поля.</summary>
    /// <param name="text">Текст-приглашение для добавляемого элемента.</param>
    /// <param name="defaultValue">Начальное значение поля ввода. По умолчанию — 0.</param>
    /// <param name="id">Необязательный уникальный идентификатор. Если не указан, будет сгенерирован автоматически.</param>
    /// <returns>Текущий экземпляр <see cref="InputMenu"/> с добавленным элементом для дальнейшей настройки.</returns>
    public InputMenu AddMenuItem(string text, uint defaultValue = default, string? id = null) => AddMenuItem(new InputMenuItem(text, defaultValue), id);

    /// <summary>Добавляет новый элемент в меню ввода с заданными параметрами.<br/>Позволяет указать текст-приглашение, значение по умолчанию, идентификатор и тип поля.</summary>
    /// <param name="text">Текст-приглашение для добавляемого элемента.</param>
    /// <param name="defaultValue">Начальное значение поля ввода. По умолчанию — 0.</param>
    /// <param name="id">Необязательный уникальный идентификатор. Если не указан, будет сгенерирован автоматически.</param>
    /// <returns>Текущий экземпляр <see cref="InputMenu"/> с добавленным элементом для дальнейшей настройки.</returns>
    public InputMenu AddMenuItem(string text, short defaultValue = default, string? id = null) => AddMenuItem(new InputMenuItem(text, defaultValue), id);

    /// <summary>Добавляет новый элемент в меню ввода с заданными параметрами.<br/>Позволяет указать текст-приглашение, значение по умолчанию, идентификатор и тип поля.</summary>
    /// <param name="text">Текст-приглашение для добавляемого элемента.</param>
    /// <param name="defaultValue">Начальное значение поля ввода. По умолчанию — 0.</param>
    /// <param name="id">Необязательный уникальный идентификатор. Если не указан, будет сгенерирован автоматически.</param>
    /// <returns>Текущий экземпляр <see cref="InputMenu"/> с добавленным элементом для дальнейшей настройки.</returns>
    public InputMenu AddMenuItem(string text, ushort defaultValue = default, string? id = null) => AddMenuItem(new InputMenuItem(text, defaultValue), id);

    /// <summary>Добавляет новый элемент в меню ввода с заданными параметрами.<br/>Позволяет указать текст-приглашение, значение по умолчанию, идентификатор и тип поля.</summary>
    /// <param name="text">Текст-приглашение для добавляемого элемента.</param>
    /// <param name="defaultValue">Начальное значение поля ввода. По умолчанию — 0.</param>
    /// <param name="id">Необязательный уникальный идентификатор. Если не указан, будет сгенерирован автоматически.</param>
    /// <returns>Текущий экземпляр <see cref="InputMenu"/> с добавленным элементом для дальнейшей настройки.</returns>
    public InputMenu AddMenuItem(string text, byte defaultValue = default, string? id = null) => AddMenuItem(new InputMenuItem(text, defaultValue), id);

    /// <summary>Добавляет новый элемент в меню ввода с заданными параметрами.<br/>Позволяет указать текст-приглашение, значение по умолчанию, идентификатор и тип поля.</summary>
    /// <param name="text">Текст-приглашение для добавляемого элемента.</param>
    /// <param name="defaultValue">Начальное значение поля ввода. По умолчанию — 0.</param>
    /// <param name="id">Необязательный уникальный идентификатор. Если не указан, будет сгенерирован автоматически.</param>
    /// <returns>Текущий экземпляр <see cref="InputMenu"/> с добавленным элементом для дальнейшей настройки.</returns>
    public InputMenu AddMenuItem(string text, sbyte defaultValue = default, string? id = null) => AddMenuItem(new InputMenuItem(text, defaultValue), id);

    /// <summary>Добавляет новый элемент в меню ввода с заданными параметрами.<br/>Позволяет указать текст-приглашение, значение по умолчанию, идентификатор и тип поля.</summary>
    /// <param name="text">Текст-приглашение для добавляемого элемента.</param>
    /// <param name="defaultValue">Начальное значение поля ввода. По умолчанию — 0.</param>
    /// <param name="id">Необязательный уникальный идентификатор. Если не указан, будет сгенерирован автоматически.</param>
    /// <returns>Текущий экземпляр <see cref="InputMenu"/> с добавленным элементом для дальнейшей настройки.</returns>
    public InputMenu AddMenuItem(string text, float defaultValue = default, string? id = null) => AddMenuItem(new InputMenuItem(text, defaultValue), id);

    /// <summary>Добавляет новый элемент в меню ввода с заданными параметрами.<br/>Позволяет указать текст-приглашение, значение по умолчанию, идентификатор и тип поля.</summary>
    /// <param name="text">Текст-приглашение для добавляемого элемента.</param>
    /// <param name="defaultValue">Начальное значение поля ввода. По умолчанию — 0.</param>
    /// <param name="id">Необязательный уникальный идентификатор. Если не указан, будет сгенерирован автоматически.</param>
    /// <returns>Текущий экземпляр <see cref="InputMenu"/> с добавленным элементом для дальнейшей настройки.</returns>
    public InputMenu AddMenuItem(string text, double defaultValue = default, string? id = null) => AddMenuItem(new InputMenuItem(text, defaultValue), id);

    /// <summary>Добавляет новый элемент в меню ввода с заданными параметрами.<br/>Позволяет указать текст-приглашение, значение по умолчанию, идентификатор и тип поля.</summary>
    /// <param name="text">Текст-приглашение для добавляемого элемента.</param>
    /// <param name="defaultValue">Начальное значение поля ввода. По умолчанию — 0.0d.</param>
    /// <param name="id">Необязательный уникальный идентификатор. Если не указан, будет сгенерирован автоматически.</param>
    /// <returns>Текущий экземпляр <see cref="InputMenu"/> с добавленным элементом для дальнейшей настройки.</returns>
    public InputMenu AddMenuItem(string text, decimal defaultValue = default, string? id = null) => AddMenuItem(new InputMenuItem(text, defaultValue), id);

    /// <summary>Добавляет новый элемент в меню ввода с заданными параметрами.<br/>Позволяет указать текст-приглашение, значение по умолчанию, идентификатор и тип поля.</summary>
    /// <param name="text">Текст-приглашение для добавляемого элемента.</param>
    /// <param name="defaultValue">Начальное значение поля ввода. По умолчанию — False.</param>
    /// <param name="id">Необязательный уникальный идентификатор. Если не указан, будет сгенерирован автоматически.</param>
    /// <returns>Текущий экземпляр <see cref="InputMenu"/> с добавленным элементом для дальнейшей настройки.</returns>
    public InputMenu AddMenuItem(string text, bool defaultValue = default, string? id = null) => AddMenuItem(new InputMenuItem(text, defaultValue), id);

    /// <summary>Заменяет заголовок меню на новый.<br/>Позволяет динамически изменять отображаемый заголовок.</summary>
    /// <param name="newTitle">Новый заголовок меню.</param>
    /// <returns>Текущий экземпляр <see cref="InputMenu"/> с обновлённым заголовком.</returns>
    public InputMenu SetTitle(string newTitle)
    {
        Title = newTitle;
        return this;
    }

    /// <summary>Устанавливает цвета для меню, копируя их из предоставленных тем.<br/>Позволяет настроить оформление заголовка, выделенного и невыделенных полей.</summary>
    /// <param name="title">Тема-источник для цветов заголовка (<see cref="Theme.TitleTextColor"/>, <see cref="Theme.TitleBackgroundColor"/>).</param>
    /// <param name="selected">Тема-источник для цветов выделенного поля (<see cref="Theme.SelectedTextColor"/>, <see cref="Theme.SelectedBackgroundColor"/>).</param>
    /// <param name="unselected">Тема-источник для цветов невыделенного поля (<see cref="Theme.UnselectedTextColor"/>, <see cref="Theme.UnselectedBackgroundColor"/>).</param>
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