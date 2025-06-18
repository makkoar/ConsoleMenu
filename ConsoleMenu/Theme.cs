namespace ConsoleMenu;

/// <summary>Класс, описывающий темы оформления для консольного меню.</summary>
/// <param name="titleText">Цвет текста заголовка.</param>
/// <param name="titleBg">Цвет фона заголовка.</param>
/// <param name="selectedText">Цвет текста выбранного элемента.</param>
/// <param name="selectedBg">Цвет фона выбранного элемента.</param>
/// <param name="unselectedText">Цвет текста невыбранного элемента.</param>
/// <param name="unselectedBg">Цвет фона невыбранного элемента.</param>
/// <param name="inputText">Цвет текста ввода.</param>
/// <param name="inputBg">Цвет фона ввода.</param>
/// <param name="fieldText">Цвет текста поля.</param>
/// <param name="fieldBg">Цвет фона поля.</param>
public class Theme(ConsoleColor titleText, ConsoleColor titleBg, ConsoleColor selectedText, ConsoleColor selectedBg, ConsoleColor unselectedText, ConsoleColor unselectedBg, ConsoleColor inputText, ConsoleColor inputBg, ConsoleColor fieldText, ConsoleColor fieldBg)
{
    #region Поля и свойства
    /// <summary>Цвет текста заголовка.</summary>
    public ConsoleColor TitleTextColor { get; set; } = titleText;
    /// <summary>Цвет фона заголовка.</summary>
    public ConsoleColor TitleBackgroundColor { get; set; } = titleBg;
    /// <summary>Цвет текста выбранного элемента.</summary>
    public ConsoleColor SelectedTextColor { get; set; } = selectedText;
    /// <summary>Цвет фона выбранного элемента.</summary>
    public ConsoleColor SelectedBackgroundColor { get; set; } = selectedBg;
    /// <summary>Цвет текста невыбранного элемента.</summary>
    public ConsoleColor UnselectedTextColor { get; set; } = unselectedText;
    /// <summary>Цвет фона невыбранного элемента.</summary>
    public ConsoleColor UnselectedBackgroundColor { get; set; } = unselectedBg;
    /// <summary>Цвет текста ввода.</summary>
    public ConsoleColor InputTextColor { get; set; } = inputText;
    /// <summary>Цвет фона ввода.</summary>
    public ConsoleColor InputBackgroundColor { get; set; } = inputBg;
    /// <summary>Цвет текста поля.</summary>
    public ConsoleColor FieldTextColor { get; set; } = fieldText;
    /// <summary>Цвет фона поля.</summary>
    public ConsoleColor FieldBackgroundColor { get; set; } = fieldBg;
    #endregion
}

/// <summary>Класс, содержащий предустановленные темы оформления для меню.</summary>
public static class Themes
{
    #region Готовые темы (шаблоны)
    /// <summary>Классическая тема (серый по чёрному).</summary>
    public static readonly Theme Classic = new(
        ConsoleColor.Gray, ConsoleColor.Black,
        ConsoleColor.Black, ConsoleColor.Gray,
        ConsoleColor.Gray, ConsoleColor.Black,
        ConsoleColor.White, ConsoleColor.DarkGray,
        ConsoleColor.DarkGray, ConsoleColor.Black
    );
    /// <summary>Синяя тема (голубой/тёмно-синий).</summary>
    public static readonly Theme Blue = new(
        ConsoleColor.Cyan, ConsoleColor.Black,
        ConsoleColor.Black, ConsoleColor.Cyan,
        ConsoleColor.Gray, ConsoleColor.Black,
        ConsoleColor.White, ConsoleColor.DarkBlue,
        ConsoleColor.DarkCyan, ConsoleColor.Black
    );
    /// <summary>Жёлтая тема (жёлтый/чёрный).</summary>
    public static readonly Theme Yellow = new(
        ConsoleColor.Yellow, ConsoleColor.Black,
        ConsoleColor.Black, ConsoleColor.Yellow,
        ConsoleColor.Gray, ConsoleColor.Black,
        ConsoleColor.Black, ConsoleColor.Yellow,
        ConsoleColor.DarkYellow, ConsoleColor.Black
    );
    /// <summary>Зелёная тема (зелёный/чёрный).</summary>
    public static readonly Theme Green = new(
        ConsoleColor.Green, ConsoleColor.Black,
        ConsoleColor.Black, ConsoleColor.Green,
        ConsoleColor.Gray, ConsoleColor.Black,
        ConsoleColor.White, ConsoleColor.DarkGreen,
        ConsoleColor.DarkGreen, ConsoleColor.Black
    );
    /// <summary>Красная тема (красный/чёрный).</summary>
    public static readonly Theme Red = new(
        ConsoleColor.Red, ConsoleColor.Black,
        ConsoleColor.White, ConsoleColor.Red,
        ConsoleColor.Gray, ConsoleColor.Black,
        ConsoleColor.White, ConsoleColor.DarkRed,
        ConsoleColor.DarkRed, ConsoleColor.Black
    );
    /// <summary>Белая тема (чёрный по белому, светлый стиль).</summary>
    public static readonly Theme White = new(
        ConsoleColor.Black, ConsoleColor.White,
        ConsoleColor.White, ConsoleColor.Gray,
        ConsoleColor.Black, ConsoleColor.White,
        ConsoleColor.Black, ConsoleColor.Gray,
        ConsoleColor.DarkGray, ConsoleColor.White
    );
    /// <summary>Чёрная тема (белый по чёрному, строгий минимализм).</summary>
    public static readonly Theme Black = new(
        ConsoleColor.White, ConsoleColor.Black,
        ConsoleColor.Black, ConsoleColor.White,
        ConsoleColor.Gray, ConsoleColor.Black,
        ConsoleColor.White, ConsoleColor.Black,
        ConsoleColor.Gray, ConsoleColor.Black
    );
    /// <summary>Пурпурная тема (яркий акцент).</summary>
    public static readonly Theme Magenta = new(
        ConsoleColor.Magenta, ConsoleColor.Black,
        ConsoleColor.Black, ConsoleColor.Magenta,
        ConsoleColor.Gray, ConsoleColor.Black,
        ConsoleColor.White, ConsoleColor.DarkMagenta,
        ConsoleColor.DarkMagenta, ConsoleColor.Black
    );
    /// <summary>Бирюзовая тема (голубой акцент).</summary>
    public static readonly Theme Cyan = new(
        ConsoleColor.Cyan, ConsoleColor.Black,
        ConsoleColor.Black, ConsoleColor.Cyan,
        ConsoleColor.Gray, ConsoleColor.Black,
        ConsoleColor.White, ConsoleColor.DarkCyan,
        ConsoleColor.DarkCyan, ConsoleColor.Black
    );
    /// <summary>Высококонтрастная тема (жёлтый по чёрному).</summary>
    public static readonly Theme HighContrast = new(
        ConsoleColor.Yellow, ConsoleColor.Black,
        ConsoleColor.Black, ConsoleColor.Yellow,
        ConsoleColor.White, ConsoleColor.Black,
        ConsoleColor.Yellow, ConsoleColor.Black,
        ConsoleColor.Yellow, ConsoleColor.Black
    );
    /// <summary>Серая тема (разные оттенки серого).</summary>
    public static readonly Theme Gray = new(
        ConsoleColor.Gray, ConsoleColor.Black,
        ConsoleColor.Black, ConsoleColor.Gray,
        ConsoleColor.DarkGray, ConsoleColor.Black,
        ConsoleColor.Gray, ConsoleColor.DarkGray,
        ConsoleColor.DarkGray, ConsoleColor.Black
    );
    #endregion
}
