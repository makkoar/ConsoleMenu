namespace ConsoleMenu;
/// <summary>Класс, отвечающий за элементы меню.</summary>
public class MenuItem
{
    #region Поля
    /// <summary>Текст, который будет отображён на экране.</summary>
    public string Text { get; set; } = string.Empty;
    /// <summary>Функция, которая будет запушена, при выборе элемента меню.</summary>
    public Action? Function { get; set; }
    #endregion

    #region Конструкторы
    public MenuItem() { }
    public MenuItem(string text) : this() => Text = text;
    public MenuItem(Action? function) : this() => Function = function;
    public MenuItem(string text, Action? function) : this(function) => Text = text;
    #endregion

    #region Builder pattern
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
