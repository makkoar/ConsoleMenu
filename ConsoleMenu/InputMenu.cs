namespace ConsoleMenu;

/// <summary>Класс, отвечающий за консольное меню для ввода данных пользователем.</summary>
public class InputMenu()
{
    #region Поля
    /// <summary>Темы, которые будут применены к данному меню.</summary>
    public Themes Themes { get; set; } = new();
    /// <summary>Заголовок меню.</summary>
    public string Title { get; set; } = "Введите значение:";
    /// <summary>Элементы меню ввода.</summary>
    public List<InputMenuItem> MenuItems { get; set; } = [];
    #endregion

    #region Конструкторы
    /// <summary>Инициализирует новый экземпляр класса <see cref="InputMenu"/> с указанным заголовком.</summary>
    /// <param name="title">Заголовок меню.</param>
    public InputMenu(string title) : this()
        => Title = title;

    /// <summary>Инициализирует новый экземпляр класса <see cref="InputMenu"/> с заданным списком элементов.</summary>
    /// <param name="menuItems">Список элементов меню <see cref="InputMenuItem"/>.</param>
    public InputMenu(params List<InputMenuItem> menuItems)
        : this() => menuItems.ForEach(MenuItems.Add);

    /// <summary>Инициализирует новый экземпляр класса <see cref="InputMenu"/> с указанным заголовком и списком элементов.</summary>
    /// <param name="title">Заголовок меню.</param>
    /// <param name="menuItems">Список элементов меню <see cref="InputMenuItem"/>.</param>
    public InputMenu(string title, params List<InputMenuItem> menuItems)
        : this(menuItems) => Title = title;

    /// <summary>Инициализирует новый экземпляр класса <see cref="InputMenu"/> из списка строк, где каждая строка становится элементом меню.</summary>
    /// <param name="menuItems">Список строк для создания элементов меню.</param>
    public InputMenu(params List<string> menuItems)
        : this() => menuItems.ForEach(item => MenuItems.Add(new(item)));

    /// <summary>Инициализирует новый экземпляр класса <see cref="InputMenu"/> с указанным заголовком и списком строк, где каждая строка становится элементом меню.</summary>
    /// <param name="title">Заголовок меню.</param>
    /// <param name="menuItems">Список строк для создания элементов меню.</param>
    public InputMenu(string title, params List<string> menuItems) 
        : this(menuItems) => Title = title;
    #endregion

    #region Строитель
    /// <summary>Добавляет элемент меню в данное меню.</summary>
    /// <param name="text">Текст, который будет отображаться у добавляемого элемента меню.</param>
    /// <param name="function">Функция, которая будет запушена, при выборе элемента меню.</param>
    /// <returns>Меню с добавленным элементом меню.</returns>
    public SelectMenu AddMenuItem(string text, string? defaultValue = null)
    {
        MenuItems.Add(new(text, defaultValue));
        return this;
    }
    /// <summary>Заменяет заголовок меню на новый.</summary>
    /// <param name="title">Новый заголовок меню.</param>
    /// <returns>Меню с изменённым заголовком.</returns>
    public SelectMenu SetTitle(string title)
    {
        Title = title;
        return this;
    }
    /// <summary>Устанавливает темы для данного меню.</summary>
    /// <param name="title">Тема заголовка меню.</param>
    /// <param name="selected">Тема выбранного элемента меню.</param>
    /// <param name="unselected">Тема невыбранных элементов меню.</param>
    /// <returns>Меню с изменёнными темами.</returns>
    public SelectMenu SetThemes(Theme title, Theme selected, Theme unselected)
    {
        Themes.Title = title;
        Themes.Selected = selected;
        Themes.Unselected = unselected;
        return this;
    }
    #endregion
}
