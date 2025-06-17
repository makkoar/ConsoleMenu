namespace ConsoleMenu;
/// <summary>Класс, отвечающий за элементы меню.</summary>
public class MenuItem()
{
    #region Поля
    /// <summary>Текст, который будет отображён на экране.</summary>
    public string Text { get; set; } = string.Empty;
    /// <summary>Функция, которая будет запушена, при выборе элемента меню.</summary>
    public Action? Function { get; set; }
    #endregion

    #region Конструкторы
    /// <summary>Инициализирует новый экземпляр класса <see cref="MenuItem"/> с указанным текстом.</summary>
    /// <param name="text">Текст элемента меню.</param>
    public MenuItem(string text) : this() => Text = text;

    /// <summary>Инициализирует новый экземпляр класса <see cref="MenuItem"/> с указанной функцией.</summary>
    /// <param name="function">Функция, которая будет выполнена при выборе этого элемента меню.</param>
    public MenuItem(Action? function) : this() => Function = function;

    /// <summary>Инициализирует новый экземпляр класса <see cref="MenuItem"/> с указанным текстом и функцией.</summary>
    /// <param name="text">Текст элемента меню.</param>
    /// <param name="function">Функция, которая будет выполнена при выборе этого элемента меню.</param>
    public MenuItem(string text, Action? function) : this(function) => Text = text;
    #endregion

    #region Строитель
    /// <summary>Заменяет текст элемента меню на новый.</summary>
    /// <param name="text">Новый текст элемента меню.</param>
    /// <returns>Элемент меню с изменённым текстом.</returns>
    public MenuItem SetText(string text)
    {
        Text = text;
        return this;
    }
    /// <summary>Заменяет функцию, которая будет запускаться при выборе этого элемента меню.</summary>
    /// <param name="function">Новая функция.</param>
    /// <returns>Элемент меню с изменённым текстом.</returns>
    public MenuItem SetAction(Action function)
    {
        Function = function;
        return this;
    }
    #endregion
}
