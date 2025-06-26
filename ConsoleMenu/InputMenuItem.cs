namespace ConsoleMenu;

/// <summary>Класс, представляющий один элемент для ввода данных в консольном меню <see cref="InputMenu"/>.<br/>Содержит текст-приглашение, тип ожидаемого значения и само значение, введённое пользователем.</summary>
public class InputMenuItem
{
    #region Поля и свойства
    /// <summary>Уникальный идентификатор элемента меню.<br/>Устанавливается автоматически или вручную через <see cref="InputMenu.AddMenuItem(InputMenuItem, string)"/>.</summary>
    public string? Id { get; internal set; }

    /// <summary>Текст-приглашение для ввода, который будет отображён на экране (например, "Введите ваше имя").</summary>
    public string Text { get; set; }

    /// <summary>Строковое представление значения элемента.<br/>Изначально содержит значение по умолчанию, а после работы меню — итоговый ввод пользователя.<br/>Изменяется внутри библиотеки во время работы <see cref="InputMenu"/>.</summary>
    public string InputValue { get; internal set; }

    /// <summary>Тип значения, ожидаемого для этого элемента меню.<br/>Определяется конструктором на основе типа значения по умолчанию.</summary>
    internal EInputMenuItemType Type { get; set; }

    /// <summary>Минимально допустимое значение (только для числовых типов).</summary>
    public string? MinValue { get; set; }

    /// <summary>Максимально допустимое значение (только для числовых типов).</summary>
    public string? MaxValue { get; set; }
    #endregion

    #region Конструкторы
    /// <summary>Инициализирует новый экземпляр класса <see cref="InputMenuItem"/> с указанным текстом.<br/>Этот конструктор предназначен для внутреннего использования.</summary>
    /// <param name="text">Текст-приглашение для ввода.</param>
    private InputMenuItem(string text) { Text = text; InputValue = string.Empty; }

    /// <summary>Инициализирует новый элемент меню для ввода строкового значения.</summary>
    /// <param name="text">Текст-приглашение для ввода.</param>
    /// <param name="defaultValue">Начальное значение поля.</param>
    public InputMenuItem(string text, string defaultValue = "") : this(text)
    {
        InputValue = defaultValue;
        Type = EInputMenuItemType.String;
    }

    /// <summary>Инициализирует новый элемент меню для ввода целочисленного значения (<see cref="int"/>).</summary>
    /// <param name="text">Текст-приглашение для ввода.</param>
    /// <param name="defaultValue">Начальное значение поля.</param>
    public InputMenuItem(string text, int defaultValue = default) : this(text)
    {
        InputValue = defaultValue.ToString();
        Type = EInputMenuItemType.Int;
    }

    /// <summary>Инициализирует новый элемент меню для ввода целочисленного значения (<see cref="int"/>) с ограничениями.</summary>
    /// <param name="text">Текст-приглашение для ввода.</param>
    /// <param name="defaultValue">Начальное значение поля.</param>
    /// <param name="minValue">Минимально допустимое значение.</param>
    /// <param name="maxValue">Максимально допустимое значение.</param>
    public InputMenuItem(string text, int defaultValue, int minValue, int maxValue) : this(text, defaultValue)
    {
        MinValue = minValue.ToString();
        MaxValue = maxValue.ToString();
    }

    /// <summary>Инициализирует новый элемент меню для ввода беззнакового целочисленного значения (<see cref="uint"/>).</summary>
    /// <param name="text">Текст-приглашение для ввода.</param>
    /// <param name="defaultValue">Начальное значение поля.</param>
    public InputMenuItem(string text, uint defaultValue = default) : this(text)
    {
        InputValue = defaultValue.ToString();
        Type = EInputMenuItemType.UInt;
    }

    /// <summary>Инициализирует новый элемент меню для ввода беззнакового целочисленного значения (<see cref="uint"/>) с ограничениями.</summary>
    /// <param name="text">Текст-приглашение для ввода.</param>
    /// <param name="defaultValue">Начальное значение поля.</param>
    /// <param name="minValue">Минимально допустимое значение.</param>
    /// <param name="maxValue">Максимально допустимое значение.</param>
    public InputMenuItem(string text, uint defaultValue, uint minValue, uint maxValue) : this(text, defaultValue)
    {
        MinValue = minValue.ToString();
        MaxValue = maxValue.ToString();
    }

    /// <summary>Инициализирует новый элемент меню для ввода короткого целочисленного значения (<see cref="short"/>).</summary>
    /// <param name="text">Текст-приглашение для ввода.</param>
    /// <param name="defaultValue">Начальное значение поля.</param>
    public InputMenuItem(string text, short defaultValue = default) : this(text)
    {
        InputValue = defaultValue.ToString();
        Type = EInputMenuItemType.Short;
    }

    /// <summary>Инициализирует новый элемент меню для ввода короткого целочисленного значения (<see cref="short"/>) с ограничениями.</summary>
    /// <param name="text">Текст-приглашение для ввода.</param>
    /// <param name="defaultValue">Начальное значение поля.</param>
    /// <param name="minValue">Минимально допустимое значение.</param>
    /// <param name="maxValue">Максимально допустимое значение.</param>
    public InputMenuItem(string text, short defaultValue, short minValue, short maxValue) : this(text, defaultValue)
    {
        MinValue = minValue.ToString();
        MaxValue = maxValue.ToString();
    }

    /// <summary>Инициализирует новый элемент меню для ввода беззнакового короткого целочисленного значения (<see cref="ushort"/>).</summary>
    /// <param name="text">Текст-приглашение для ввода.</param>
    /// <param name="defaultValue">Начальное значение поля.</param>
    public InputMenuItem(string text, ushort defaultValue = default) : this(text)
    {
        InputValue = defaultValue.ToString();
        Type = EInputMenuItemType.UShort;
    }

    /// <summary>Инициализирует новый элемент меню для ввода беззнакового короткого целочисленного значения (<see cref="ushort"/>) с ограничениями.</summary>
    /// <param name="text">Текст-приглашение для ввода.</param>
    /// <param name="defaultValue">Начальное значение поля.</param>
    /// <param name="minValue">Минимально допустимое значение.</param>
    /// <param name="maxValue">Максимально допустимое значение.</param>
    public InputMenuItem(string text, ushort defaultValue, ushort minValue, ushort maxValue) : this(text, defaultValue)
    {
        MinValue = minValue.ToString();
        MaxValue = maxValue.ToString();
    }

    /// <summary>Инициализирует новый элемент меню для ввода байтового значения (<see cref="byte"/>).</summary>
    /// <param name="text">Текст-приглашение для ввода.</param>
    /// <param name="defaultValue">Начальное значение поля.</param>
    public InputMenuItem(string text, byte defaultValue = default) : this(text)
    {
        InputValue = defaultValue.ToString();
        Type = EInputMenuItemType.Byte;
    }

    /// <summary>Инициализирует новый элемент меню для ввода байтового значения (<see cref="byte"/>) с ограничениями.</summary>
    /// <param name="text">Текст-приглашение для ввода.</param>
    /// <param name="defaultValue">Начальное значение поля.</param>
    /// <param name="minValue">Минимально допустимое значение.</param>
    /// <param name="maxValue">Максимально допустимое значение.</param>
    public InputMenuItem(string text, byte defaultValue, byte minValue, byte maxValue) : this(text, defaultValue)
    {
        MinValue = minValue.ToString();
        MaxValue = maxValue.ToString();
    }

    /// <summary>Инициализирует новый элемент меню для ввода знакового байтового значения (<see cref="sbyte"/>).</summary>
    /// <param name="text">Текст-приглашение для ввода.</param>
    /// <param name="defaultValue">Начальное значение поля.</param>
    public InputMenuItem(string text, sbyte defaultValue = default) : this(text)
    {
        InputValue = defaultValue.ToString();
        Type = EInputMenuItemType.SByte;
    }

    /// <summary>Инициализирует новый элемент меню для ввода знакового байтового значения (<see cref="sbyte"/>) с ограничениями.</summary>
    /// <param name="text">Текст-приглашение для ввода.</param>
    /// <param name="defaultValue">Начальное значение поля.</param>
    /// <param name="minValue">Минимально допустимое значение.</param>
    /// <param name="maxValue">Максимально допустимое значение.</param>
    public InputMenuItem(string text, sbyte defaultValue, sbyte minValue, sbyte maxValue) : this(text, defaultValue)
    {
        MinValue = minValue.ToString();
        MaxValue = maxValue.ToString();
    }

    /// <summary>Инициализирует новый элемент меню для ввода числа с плавающей точкой (<see cref="float"/>).</summary>
    /// <param name="text">Текст-приглашение для ввода.</param>
    /// <param name="defaultValue">Начальное значение поля.</param>
    public InputMenuItem(string text, float defaultValue = default) : this(text)
    {
        InputValue = defaultValue.ToString(CultureInfo.InvariantCulture);
        Type = EInputMenuItemType.Float;
    }

    /// <summary>Инициализирует новый элемент меню для ввода числа с плавающей точкой двойной точности (<see cref="double"/>).</summary>
    /// <param name="text">Текст-приглашение для ввода.</param>
    /// <param name="defaultValue">Начальное значение поля.</param>
    public InputMenuItem(string text, double defaultValue = default) : this(text)
    {
        InputValue = defaultValue.ToString(CultureInfo.InvariantCulture);
        Type = EInputMenuItemType.Double;
    }

    /// <summary>Инициализирует новый элемент меню для ввода десятичного числа (<see cref="decimal"/>).</summary>
    /// <param name="text">Текст-приглашение для ввода.</param>
    /// <param name="defaultValue">Начальное значение поля.</param>
    public InputMenuItem(string text, decimal defaultValue = default) : this(text)
    {
        InputValue = defaultValue.ToString(CultureInfo.InvariantCulture);
        Type = EInputMenuItemType.Decimal;
    }

    /// <summary>Инициализирует новый элемент меню для ввода логического значения (<see cref="bool"/>).</summary>
    /// <param name="text">Текст-приглашение для ввода.</param>
    /// <param name="defaultValue">Начальное значение поля.</param>
    public InputMenuItem(string text, bool defaultValue = default) : this(text)
    {
        InputValue = defaultValue.ToString();
        Type = EInputMenuItemType.Bool;
    }

    /// <summary>Инициализирует новый элемент меню для ввода длинного целочисленного значения (<see cref="long"/>).</summary>
    /// <param name="text">Текст-приглашение для ввода.</param>
    /// <param name="defaultValue">Начальное значение поля.</param>
    public InputMenuItem(string text, long defaultValue = default) : this(text)
    {
        Type = EInputMenuItemType.Long;
        InputValue = defaultValue.ToString();
    }

    /// <summary>Инициализирует новый элемент меню для ввода длинного целочисленного значения (<see cref="long"/>) с ограничениями.</summary>
    /// <param name="text">Текст-приглашение для ввода.</param>
    /// <param name="defaultValue">Начальное значение поля.</param>
    /// <param name="minValue">Минимально допустимое значение.</param>
    /// <param name="maxValue">Максимально допустимое значение.</param>
    public InputMenuItem(string text, long defaultValue, long minValue, long maxValue) : this(text, defaultValue)
    {
        MinValue = minValue.ToString();
        MaxValue = maxValue.ToString();
    }

    /// <summary>Инициализирует новый элемент меню для ввода беззнакового длинного целочисленного значения (<see cref="ulong"/>).</summary>
    /// <param name="text">Текст-приглашение для ввода.</param>
    /// <param name="defaultValue">Начальное значение поля.</param>
    public InputMenuItem(string text, ulong defaultValue = default) : this(text)
    {
        Type = EInputMenuItemType.ULong;
        InputValue = defaultValue.ToString();
    }

    /// <summary>Инициализирует новый элемент меню для ввода беззнакового длинного целочисленного значения (<see cref="ulong"/>) с ограничениями.</summary>
    /// <param name="text">Текст-приглашение для ввода.</param>
    /// <param name="defaultValue">Начальное значение поля.</param>
    /// <param name="minValue">Минимально допустимое значение.</param>
    /// <param name="maxValue">Максимально допустимое значение.</param>
    public InputMenuItem(string text, ulong defaultValue, ulong minValue, ulong maxValue) : this(text, defaultValue)
    {
        MinValue = minValue.ToString();
        MaxValue = maxValue.ToString();
    }
    #endregion

    #region Строитель
    /// <summary>Устанавливает уникальный идентификатор для элемента меню.<br/>Этот метод следует использовать с осторожностью, так как <see cref="InputMenu"/> управляет уникальностью идентификаторов автоматически.</summary>
    /// <param name="id">Новый нечисловой идентификатор.</param>
    /// <exception cref="ArgumentException">Выбрасывается, если идентификатор пуст, состоит из пробелов или является числом.</exception>
    public void SetId(string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            throw new ArgumentException("Идентификатор не может быть пустым или состоять только из пробелов.", nameof(id));
        if (uint.TryParse(id, out uint value))
            throw new ArgumentException($"Идентификатор {value} не может быть числом, так как числа зарезервированы для авто-генерации Id.", nameof(id));

        Id = id;
    }
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

    /// <summary>Пытается преобразовать введённое значение в <see cref="long"/>.</summary>
    /// <param name="value">Выходной параметр, содержащий преобразованное число, если операция прошла успешно.</param>
    /// <returns><c>true</c>, если преобразование успешно; иначе <c>false</c>.</returns>
    public bool TryGetLong(out long value) => long.TryParse(InputValue, out value);

    /// <summary>Пытается преобразовать введённое значение в <see cref="ulong"/>.</summary>
    /// <param name="value">Выходной параметр, содержащий преобразованное число, если операция прошла успешно.</param>
    /// <returns><c>true</c>, если преобразование успешно; иначе <c>false</c>.</returns>
    public bool TryGetULong(out ulong value) => ulong.TryParse(InputValue, out value);

    /// <summary>Преобразует введённое значение в <see cref="int"/> или выбрасывает исключение.</summary>
    /// <returns>Целочисленное значение.</returns>
    /// <exception cref="FormatException">Выбрасывается, если <see cref="InputValue"/> имеет неверный формат или представляет число вне диапазона <see cref="int"/>.</exception>
    public int GetInt() => int.TryParse(InputValue, out int value) ? value : throw new FormatException($"Значение \"{InputValue}\" не может быть преобразовано в int.");

    /// <summary>Преобразует введённое значение в <see cref="uint"/> или выбрасывает исключение.</summary>
    /// <returns>Беззнаковое целочисленное значение.</returns>
    /// <exception cref="FormatException">Выбрасывается, если <see cref="InputValue"/> имеет неверный формат или представляет число вне диапазона <see cref="uint"/>.</exception>
    public uint GetUInt() => uint.TryParse(InputValue, out uint value) ? value : throw new FormatException($"Значение \"{InputValue}\" не может быть преобразовано в uint.");

    /// <summary>Преобразует введённое значение в <see cref="short"/> или выбрасывает исключение.</summary>
    /// <returns>Короткое целочисленное значение.</returns>
    /// <exception cref="FormatException">Выбрасывается, если <see cref="InputValue"/> имеет неверный формат или представляет число вне диапазона <see cref="short"/>.</exception>
    public short GetShort() => short.TryParse(InputValue, out short value) ? value : throw new FormatException($"Значение \"{InputValue}\" не может быть преобразовано в short.");

    /// <summary>Преобразует введённое значение в <see cref="ushort"/> или выбрасывает исключение.</summary>
    /// <returns>Беззнаковое короткое целочисленное значение.</returns>
    /// <exception cref="FormatException">Выбрасывается, если <see cref="InputValue"/> имеет неверный формат или представляет число вне диапазона <see cref="ushort"/>.</exception>
    public ushort GetUShort() => ushort.TryParse(InputValue, out ushort value) ? value : throw new FormatException($"Значение \"{InputValue}\" не может быть преобразовано в ushort.");

    /// <summary>Преобразует введённое значение в <see cref="byte"/> или выбрасывает исключение.</summary>
    /// <returns>Байтовое значение.</returns>
    /// <exception cref="FormatException">Выбрасывается, если <see cref="InputValue"/> имеет неверный формат или представляет число вне диапазона <see cref="byte"/>.</exception>
    public byte GetByte() => byte.TryParse(InputValue, out byte value) ? value : throw new FormatException($"Значение \"{InputValue}\" не может быть преобразовано в byte.");

    /// <summary>Преобразует введённое значение в <see cref="sbyte"/> или выбрасывает исключение.</summary>
    /// <returns>Знаковое байтовое значение.</returns>
    /// <exception cref="FormatException">Выбрасывается, если <see cref="InputValue"/> имеет неверный формат или представляет число вне диапазона <see cref="sbyte"/>.</exception>
    public sbyte GetSByte() => sbyte.TryParse(InputValue, out sbyte value) ? value : throw new FormatException($"Значение \"{InputValue}\" не может быть преобразовано в sbyte.");

    /// <summary>Преобразует введённое значение в <see cref="float"/> или выбрасывает исключение.</summary>
    /// <returns>Значение с плавающей точкой.</returns>
    /// <exception cref="FormatException">Выбрасывается, если <see cref="InputValue"/> имеет неверный формат.</exception>
    public float GetFloat() => float.TryParse(InputValue, NumberStyles.Float, CultureInfo.InvariantCulture, out float value) ? value : throw new FormatException($"Значение \"{InputValue}\" не может быть преобразовано в float.");

    /// <summary>Преобразует введённое значение в <see cref="double"/> или выбрасывает исключение.</summary>
    /// <returns>Значение с плавающей точкой двойной точности.</returns>
    /// <exception cref="FormatException">Выбрасывается, если <see cref="InputValue"/> имеет неверный формат.</exception>
    public double GetDouble() => double.TryParse(InputValue, NumberStyles.Float, CultureInfo.InvariantCulture, out double value) ? value : throw new FormatException($"Значение \"{InputValue}\" не может быть преобразовано в double.");

    /// <summary>Преобразует введённое значение в <see cref="decimal"/> или выбрасывает исключение.</summary>
    /// <returns>Десятичное значение.</returns>
    /// <exception cref="FormatException">Выбрасывается, если <see cref="InputValue"/> имеет неверный формат.</exception>
    public decimal GetDecimal() => decimal.TryParse(InputValue, NumberStyles.Float, CultureInfo.InvariantCulture, out decimal value) ? value : throw new FormatException($"Значение \"{InputValue}\" не может быть преобразовано в decimal.");

    /// <summary>Преобразует введённое значение в <see cref="long"/> или выбрасывает исключение.</summary>
    /// <returns>Длинное целочисленное значение.</returns>
    /// <exception cref="FormatException">Выбрасывается, если <see cref="InputValue"/> имеет неверный формат или представляет число вне диапазона <see cref="long"/>.</exception>
    public long GetLong() => long.Parse(InputValue);

    /// <summary>Преобразует введённое значение в <see cref="ulong"/> или выбрасывает исключение.</summary>
    /// <returns>Беззнаковое длинное целочисленное значение.</returns>
    /// <exception cref="FormatException">Выбрасывается, если <see cref="InputValue"/> имеет неверный формат или представляет число вне диапазона <see cref="ulong"/>.</exception>
    public ulong GetULong() => ulong.Parse(InputValue);

    /// <summary>Преобразует введённое значение в <see cref="bool"/>.<br/>Возвращает <c>false</c>, если значение является пустой строкой или (без учёта регистра) равно "False". Во всех остальных случаях возвращает <c>true</c>.</summary>
    /// <returns><c>true</c>, если значение не является пустой строкой и не равно "False"; иначе <c>false</c>.</returns>
    public bool GetBool() => !(string.IsNullOrWhiteSpace(InputValue) || string.Equals(InputValue, "False", StringComparison.OrdinalIgnoreCase));
    #endregion

    #region Переопределённые методы
    /// <summary>Возвращает строковое представление объекта, которым является текущее введённое значение.</summary>
    /// <returns>Текущее значение <see cref="InputValue"/>.</returns>
    public override string ToString() => InputValue ?? string.Empty;
    #endregion
}