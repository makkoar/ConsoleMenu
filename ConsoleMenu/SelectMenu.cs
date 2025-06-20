namespace ConsoleMenu;

/// <summary>Класс, реализующий консольное меню выбора с поддержкой темизации, гибкой настройкой элементов и обработкой пользовательского ввода.<br/>Позволяет создавать интерактивные списки для выбора одного из вариантов с выполнением связанных действий.</summary>
public class SelectMenu()
{
    #region Поля и свойства
    /// <summary>Тема, применяемая к данному меню.<br/>Определяет цвета заголовка, выбранного и невыбранных пунктов.</summary>
    public Theme Theme { get; set; } = Themes.Classic;
    /// <summary>Заголовок меню, отображаемый над списком пунктов.<br/>Может быть изменён через <see cref="SetTitle(string)"/>.</summary>
    public string Title { get; set; } = "Выберите пункт меню:";
    /// <summary>Список элементов меню.<br/>Каждый элемент описывается объектом <see cref="SelectMenuItem"/> и содержит текст и действие.</summary>
    public List<SelectMenuItem> MenuItems { get; set; } = [];
    #endregion

    #region Конструкторы
    /// <summary>Создаёт новый экземпляр класса <see cref="SelectMenu"/> с указанным заголовком.<br/>Меню будет содержать пустой список элементов.</summary>
    /// <param name="title">Заголовок меню, который будет отображён над списком пунктов.</param>
    public SelectMenu(string title) : this() => Title = title;

    /// <summary>Создаёт новый экземпляр класса <see cref="SelectMenu"/> с заданным списком элементов.<br/>Каждый элемент добавляется в меню.</summary>
    /// <param name="menuItems">Список элементов меню <see cref="SelectMenuItem"/> для добавления.</param>
    public SelectMenu(params List<SelectMenuItem> menuItems) : this() => menuItems.ForEach(MenuItems.Add);

    /// <summary>Создаёт новый экземпляр класса <see cref="SelectMenu"/> с указанным заголовком и списком элементов.<br/>Позволяет задать заголовок и сразу добавить пункты меню.</summary>
    /// <param name="title">Заголовок меню.</param>
    /// <param name="menuItems">Список элементов меню <see cref="SelectMenuItem"/> для добавления.</param>
    public SelectMenu(string title, params List<SelectMenuItem> menuItems) : this(menuItems) => Title = title;

    /// <summary>Создаёт новый экземпляр класса <see cref="SelectMenu"/> из списка строк, где каждая строка становится отдельным пунктом меню.<br/>Действие для таких пунктов не задаётся.</summary>
    /// <param name="menuItems">Список строк для создания элементов меню.</param>
    public SelectMenu(params List<string> menuItems) : this() => menuItems.ForEach(item => MenuItems.Add(new(item)));

    /// <summary>Создаёт новый экземпляр класса <see cref="SelectMenu"/> с указанным заголовком и списком строк, где каждая строка становится отдельным пунктом меню.<br/>Действие для таких пунктов не задаётся.</summary>
    /// <param name="title">Заголовок меню.</param>
    /// <param name="menuItems">Список строк для создания элементов меню.</param>
    public SelectMenu(string title, params List<string> menuItems) : this(menuItems) => Title = title;
    #endregion

    #region Основная логика
    /// <summary>Отображает меню и обрабатывает пользовательский ввод для выбора одного из пунктов.<br/>Управление осуществляется следующими клавишами:<br/><list type="bullet"><item><description>Стрелки вверх/вниз: Перемещение между пунктами меню.</description></item><item><description>Enter: Подтверждает выбор, выполняет связанное действие (<see cref="SelectMenuItem.Function"/>), если оно задано, и возвращает индекс выбранного элемента.</description></item><item><description>Escape: Выход из меню. Возвращает индекс элемента, который был выбран на момент нажатия, но не выполняет связанное с ним действие.</description></item></list></summary>
    /// <param name="startIndex">Индекс элемента, который будет выбран изначально. По умолчанию — 0.</param>
    /// <param name="clear">Если <c>true</c>, очищает консоль перед отображением меню; если <c>false</c>, меню рисуется с текущей позиции курсора.</param>
    /// <returns>Индекс выбранного элемента меню (<see cref="ushort"/>).</returns>
    public ushort Apply(ushort startIndex = 0, bool clear = false)
    {
        if (clear) Console.Clear();

        ushort selected = startIndex;
        int menuTop = Console.CursorTop;
        RenderManager.SetCursorVisibility(false);

        while (true)
        {
            List<string> titleLines = RenderManager.WrapText(Title, RenderManager.WindowWidth);
            List<List<string>> itemLinesList = [.. MenuItems.Select(item =>
            {
                List<string> lines = RenderManager.WrapText(item.Text, RenderManager.WindowWidth - 2);
                if (lines.Count is 0)
                    lines.Add(string.Empty);
                return lines;
            })];

            int menuHeight = titleLines.Count + itemLinesList.Sum(lines => lines.Count);

            RenderManager.DrawSelectMenu(titleLines, MenuItems, itemLinesList, selected, Theme, menuTop);

            ConsoleKeyInfo keyInfo = Console.ReadKey(true);
            switch (keyInfo.Key)
            {
                case ConsoleKey.UpArrow: selected = (ushort)(selected > 0 ? selected - 1 : MenuItems.Count - 1); break;
                case ConsoleKey.DownArrow: selected = (ushort)(selected < MenuItems.Count - 1 ? selected + 1 : 0); break;
                case ConsoleKey.Enter:
                case ConsoleKey.Escape:
                    {
                        RenderManager.ClearArea(menuTop, menuHeight, RenderManager.WindowWidth);
                        RenderManager.SetCursorVisibility(true);
                        RenderManager.ResetColor();

                        if (keyInfo.Key is ConsoleKey.Enter && MenuItems.Count > 0 && MenuItems[selected].Function is not null)
                            MenuItems[selected].Function!();
                        return selected;
                    }
            }
        }
    }
    #endregion

    #region Строитель
    /// <summary>Добавляет элемент меню в данное меню.<br/>Позволяет добавить готовый экземпляр <see cref="SelectMenuItem"/>.</summary>
    /// <param name="item">Экземпляр <see cref="SelectMenuItem"/> для добавления.</param>
    /// <returns>Текущий экземпляр <see cref="SelectMenu"/> с добавленным элементом.</returns>
    public SelectMenu AddMenuItem(SelectMenuItem item)
    {
        MenuItems.Add(item);
        return this;
    }
    /// <summary>Добавляет элемент меню в данное меню по тексту и действию.<br/>Создаёт новый <see cref="SelectMenuItem"/> с указанными параметрами.</summary>
    /// <param name="text">Текст, который будет отображаться у добавляемого элемента меню.</param>
    /// <param name="function">Функция, которая будет выполнена при выборе элемента меню. Необязательный параметр.</param>
    /// <returns>Текущий экземпляр <see cref="SelectMenu"/> с добавленным элементом.</returns>
    public SelectMenu AddMenuItem(string text, Action? function = null)
        => AddMenuItem(new(text, function));

    /// <summary>Заменяет заголовок меню на новый.<br/>Позволяет динамически изменять отображаемый заголовок.</summary>
    /// <param name="title">Новый заголовок меню.</param>
    /// <returns>Текущий экземпляр <see cref="SelectMenu"/> с обновлённым заголовком.</returns>
    public SelectMenu SetTitle(string title)
    {
        Title = title;
        return this;
    }
    /// <summary>Устанавливает темы для данного меню.<br/>Позволяет задать отдельные цвета для заголовка, выбранного и невыбранных пунктов.</summary>
    /// <param name="titleTextColor">Цвет текста заголовка меню.</param>
    /// <param name="titleBackgroundColor">Цвет фона заголовка меню.</param>
    /// <param name="selectedTextColor">Цвет текста выбранного элемента меню.</param>
    /// <param name="selectedBackgroundColor">Цвет фона выбранного элемента меню.</param>
    /// <param name="unselectedTextColor">Цвет текста невыбранных элементов меню.</param>
    /// <param name="unselectedBackgroundColor">Цвет фона невыбранных элементов меню.</param>
    /// <returns>Текущий экземпляр <see cref="SelectMenu"/> с обновлёнными темами.</returns>
    public SelectMenu SetThemes(ConsoleColor titleTextColor, ConsoleColor titleBackgroundColor, ConsoleColor selectedTextColor, ConsoleColor selectedBackgroundColor, ConsoleColor unselectedTextColor, ConsoleColor unselectedBackgroundColor)
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