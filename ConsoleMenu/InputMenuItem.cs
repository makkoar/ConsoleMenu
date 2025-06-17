namespace ConsoleMenu;

/// <summary>Класс, представляющий один элемент для ввода данных в консольном меню.</summary>
public class InputMenuItem(string text, string? defaultValue = "", string? id = null)
{
    #region Свойства
    /// <summary>Уникальный идентификатор элемента меню.</summary>
    public string Id { get; internal set; } = id ?? string.Empty;

    /// <summary>Текст-приглашение для ввода, который будет отображён на экране (например, "Введите ваше имя").</summary>
    public string Text { get; set; } = text;

    /// <summary>Значение, которое будет использоваться как значение по умолчанию,<br/>а также будет содержать итоговый ввод пользователя.</summary>
    public string? InputValue { get; set; } = defaultValue;
    #endregion

    #region Вспомогательные методы для получения значений
    /// <summary>Пытается преобразовать введённое значение в <see cref="int"/>.</summary>
    /// <param name="value">Выходной параметр, содержащий преобразованное число, если операция прошла успешно.</param>
    /// <returns><c>true</c>, если преобразование успешно; иначе <c>false</c>.</returns>
    public bool TryGetInt(out int value) => int.TryParse(InputValue, out value);

    /// <summary>Пытается преобразовать введённое значение в <see cref="float"/>, используя инвариантную культуру.</summary>
    /// <param name="value">Выходной параметр, содержащий преобразованное число, если операция прошла успешно.</param>
    /// <returns><c>true</c>, если преобразование успешно; иначе <c>false</c>.</returns>
    public bool TryGetFloat(out float value) => float.TryParse(InputValue, NumberStyles.Float, CultureInfo.InvariantCulture, out value);

    /// <summary>Пытается преобразовать введённое значение в <see cref="double"/>, используя инвариантную культуру.</summary>
    /// <param name="value">Выходной параметр, содержащий преобразованное число, если операция прошла успешно.</param>
    /// <returns><c>true</c>, если преобразование успешно; иначе <c>false</c>.</returns>
    public bool TryGetDouble(out double value) => double.TryParse(InputValue, NumberStyles.Float, CultureInfo.InvariantCulture, out value);

    /// <summary>Пытается преобразовать введённое значение в <see cref="decimal"/>, используя инвариантную культуру.</summary>
    /// <param name="value">Выходной параметр, содержащий преобразованное число, если операция прошла успешно.</param>
    /// <returns><c>true</c>, если преобразование успешно; иначе <c>false</c>.</returns>
    public bool TryGetDecimal(out decimal value) => decimal.TryParse(InputValue, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
    #endregion
}