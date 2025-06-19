namespace ConsoleMenu;

/// <summary>Класс, представляющий один элемент для ввода данных в консольном меню.</summary>
public class InputMenuItem(string text, string? defaultValue = "", string? id = null)
{
    #region Поля и свойства
    /// <summary>Уникальный идентификатор элемента меню.</summary>
    public string Id { get; internal set; } = id ?? string.Empty;

    /// <summary>Текст-приглашение для ввода, который будет отображён на экране (например, "Введите ваше имя").</summary>
    public string Text { get; set; } = text;

    /// <summary>Значение, которое будет использоваться как значение по умолчанию,<br/>а также будет содержать итоговый ввод пользователя.</summary>
    public string? InputValue { get; set; } = defaultValue;

    /// <summary>Тип значения, ожидаемого для этого элемента меню.</summary>
    public EInputMenuItemType Type { get; set; } = EInputMenuItemType.String;
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

    /// <summary>Преобразует введённое значение в <see cref="int"/> или выбрасывает исключение.</summary>
    public int GetInt() => int.TryParse(InputValue, out int value) ? value : throw new FormatException($"Значение \"{InputValue}\" не может быть преобразовано в int.");

    /// <summary>Преобразует введённое значение в <see cref="float"/> или выбрасывает исключение.</summary>
    public float GetFloat() => float.TryParse(InputValue, NumberStyles.Float, CultureInfo.InvariantCulture, out float value) ? value : throw new FormatException($"Значение \"{InputValue}\" не может быть преобразовано в float.");

    /// <summary>Преобразует введённое значение в <see cref="double"/> или выбрасывает исключение.</summary>
    public double GetDouble() => double.TryParse(InputValue, NumberStyles.Float, CultureInfo.InvariantCulture, out double value) ? value : throw new FormatException($"Значение \"{InputValue}\" не может быть преобразовано в double.");

    /// <summary>Преобразует введённое значение в <see cref="decimal"/> или выбрасывает исключение.</summary>
    public decimal GetDecimal() => decimal.TryParse(InputValue, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal value) ? value : throw new FormatException($"Значение \"{InputValue}\" не может быть преобразовано в decimal.");
    #endregion

    #region Переопределённые методы
    /// <summary>Возвращает строковое представление объекта.</summary>
    public override string ToString() => InputValue ?? string.Empty;
    #endregion
}