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

    /// <summary>Пытается преобразовать введённое значение в <see cref="uint"/>.</summary>
    /// <param name="value">Выходной параметр, содержащий преобразованное число, если операция прошла успешно.</param>
    /// <returns><c>true</c>, если преобразование успешно; иначе <c>false</c>.</returns>
    public bool TryGetUInt(out uint value) => uint.TryParse(InputValue, out value);

    /// <summary>Пытается преобразовать введённое значение в <see cref="short"/>.</summary>
    /// <param name="value">Выходной параметр, содержащий преобразованное число, если операция прошла успешно.</param>
    /// <returns><c>true</c>, если преобразование успешно; иначе <c>false</c>.</returns>
    public bool TryGetShort(out short value) => short.TryParse(InputValue, out value);

    /// <summary>Пытается преобразовать введённое значение в <see cref="ushort"/>.</summary>
    /// <param name="value">Выходной параметр, содержащий преобразованное число, если операция прошла успешно.</param>
    /// <returns><c>true</c>, если преобразование успешно; иначе <c>false</c>.</returns>
    public bool TryGetUShort(out ushort value) => ushort.TryParse(InputValue, out value);

    /// <summary>Пытается преобразовать введённое значение в <see cref="byte"/>.</summary>
    /// <param name="value">Выходной параметр, содержащий преобразованное число, если операция прошла успешно.</param>
    /// <returns><c>true</c>, если преобразование успешно; иначе <c>false</c>.</returns>
    public bool TryGetByte(out byte value) => byte.TryParse(InputValue, out value);

    /// <summary>Пытается преобразовать введённое значение в <see cref="sbyte"/>.</summary>
    /// <param name="value">Выходной параметр, содержащий преобразованное число, если операция прошла успешно.</param>
    /// <returns><c>true</c>, если преобразование успешно; иначе <c>false</c>.</returns>
    public bool TryGetSByte(out sbyte value) => sbyte.TryParse(InputValue, out value);

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

    /// <summary>Преобразует введённое значение в <see cref="uint"/> или выбрасывает исключение.</summary>
    public uint GetUInt() => uint.TryParse(InputValue, out uint value) ? value : throw new FormatException($"Значение \"{InputValue}\" не может быть преобразовано в uint.");

    /// <summary>Преобразует введённое значение в <see cref="short"/> или выбрасывает исключение.</summary>
    public short GetShort() => short.TryParse(InputValue, out short value) ? value : throw new FormatException($"Значение \"{InputValue}\" не может быть преобразовано в short.");

    /// <summary>Преобразует введённое значение в <see cref="ushort"/> или выбрасывает исключение.</summary>
    public ushort GetUShort() => ushort.TryParse(InputValue, out ushort value) ? value : throw new FormatException($"Значение \"{InputValue}\" не может быть преобразовано в ushort.");

    /// <summary>Преобразует введённое значение в <see cref="byte"/> или выбрасывает исключение.</summary>
    public byte GetByte() => byte.TryParse(InputValue, out byte value) ? value : throw new FormatException($"Значение \"{InputValue}\" не может быть преобразовано в byte.");

    /// <summary>Преобразует введённое значение в <see cref="sbyte"/> или выбрасывает исключение.</summary>
    public sbyte GetSByte() => sbyte.TryParse(InputValue, out sbyte value) ? value : throw new FormatException($"Значение \"{InputValue}\" не может быть преобразовано в sbyte.");

    /// <summary>Преобразует введённое значение в <see cref="float"/> или выбрасывает исключение.</summary>
    public float GetFloat() => float.TryParse(InputValue, NumberStyles.Float, CultureInfo.InvariantCulture, out float value) ? value : throw new FormatException($"Значение \"{InputValue}\" не может быть преобразовано в float.");

    /// <summary>Преобразует введённое значение в <see cref="double"/> или выбрасывает исключение.</summary>
    public double GetDouble() => double.TryParse(InputValue, NumberStyles.Float, CultureInfo.InvariantCulture, out double value) ? value : throw new FormatException($"Значение \"{InputValue}\" не может быть преобразовано в double.");

    /// <summary>Преобразует введённое значение в <see cref="decimal"/> или выбрасывает исключение.</summary>
    public decimal GetDecimal() => decimal.TryParse(InputValue, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal value) ? value : throw new FormatException($"Значение \"{InputValue}\" не может быть преобразовано в decimal.");
    #endregion

    #region Строитель
    /// <summary>Устанавливает тип ввода для элемента меню.</summary>
    /// <param name="type">Тип ввода, который будет установлен.</param>
    /// <returns>Текущий объект <see cref="InputMenuItem"/> с обновлённым типом ввода.</returns>
    public InputMenuItem SetInputType(EInputMenuItemType type)
    {
        Type = type;
        return this;
    }
    #endregion

    #region Переопределённые методы
    /// <summary>Возвращает строковое представление объекта.</summary>
    public override string ToString() => InputValue ?? string.Empty;
    #endregion
}