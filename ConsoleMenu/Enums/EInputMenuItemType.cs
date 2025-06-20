namespace ConsoleMenu.Enums;

/// <summary>Перечисление поддерживаемых типов ввода для InputMenuItem.</summary>
internal enum EInputMenuItemType : byte
{
    /// <summary>Строковый тип ввода.</summary>
    String,
    /// <summary>Целочисленный тип ввода.</summary>
    Int,
    /// <summary>Целочисленный тип ввода без знака.</summary>
    UInt,
    /// <summary>Короткий целочисленный тип ввода.</summary>
    Short,
    /// <summary>Короткий целочисленный тип ввода без знака.</summary>
    UShort,
    /// <summary>Тип ввода для байта.</summary>
    Byte,
    /// <summary>Тип ввода для знакового байта.</summary>
    SByte,
    /// <summary>Тип ввода для числа с плавающей точкой (float).</summary>
    Float,
    /// <summary>Тип ввода для числа с плавающей точкой двойной точности (double).</summary>
    Double,
    /// <summary>Тип ввода для десятичного числа.</summary>
    Decimal,
    /// <summary>Логический тип ввода.</summary>
    Bool
}