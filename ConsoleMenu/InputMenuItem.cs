namespace ConsoleMenu;

/// <summary>Класс, представляющий один элемент для ввода данных в консольном меню.</summary>
/// <param name="text">Текст-приглашение для ввода.</param>
/// <param name="defaultValue">Начальное значение поля ввода.</param>
public class InputMenuItem(string text, string? defaultValue = "")
{
    #region Свойства
    /// <summary>Текст-приглашение для ввода, который будет отображён на экране (например, "Введите ваше имя").</summary>
    public string Text { get; set; } = text;

    /// <summary>Значение, которое будет использоваться как значение по умолчанию, а также будет содержать итоговый ввод пользователя.</summary>
    public string? InputValue { get; set; } = defaultValue;
    #endregion

    #region Строитель (Fluent API)
    /// <summary>Устанавливает новый текст-приглашение для этого элемента меню.</summary>
    /// <param name="newText">Новый текст-приглашение.</param>
    /// <returns>Этот же экземпляр <see cref="InputMenuItem"/> для цепочки вызовов.</returns>
    public InputMenuItem SetText(string newText)
    {
        Text = newText;
        return this;
    }

    /// <summary>Устанавливает новое значение по умолчанию для этого элемента меню.</summary>
    /// <param name="newDefaultValue">Новое значение по умолчанию.</param>
    /// <returns>Этот же экземпляр <see cref="InputMenuItem"/> для цепочки вызовов.</returns>
    public InputMenuItem SetDefaultValue(string newDefaultValue)
    {
        InputValue = newDefaultValue;
        return this;
    }
    #endregion
}
