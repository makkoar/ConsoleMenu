namespace ConsoleMenu;

/// <summary>Класс, описывающий темы оформления для консольного меню.</summary>
public class Theme
{
    #region Поля
    /// <summary>Цвет текста заголовка.</summary>
    public ConsoleColor TitleTextColor { get; set; }
    /// <summary>Цвет фона заголовка.</summary>
    public ConsoleColor TitleBackgroundColor { get; set; }
    /// <summary>Цвет текста выбранного элемента.</summary>
    public ConsoleColor SelectedTextColor { get; set; }
    /// <summary>Цвет фона выбранного элемента.</summary>
    public ConsoleColor SelectedBackgroundColor { get; set; }
    /// <summary>Цвет текста невыбранного элемента.</summary>
    public ConsoleColor UnselectedTextColor { get; set; }
    /// <summary>Цвет фона невыбранного элемента.</summary>
    public ConsoleColor UnselectedBackgroundColor { get; set; }
    /// <summary>Цвет текста ввода.</summary>
    public ConsoleColor InputTextColor { get; set; }
    /// <summary>Цвет фона ввода.</summary>
    public ConsoleColor InputBackgroundColor { get; set; }
    /// <summary>Цвет текста поля.</summary>
    public ConsoleColor FieldTextColor { get; set; }
    /// <summary>Цвет фона поля.</summary>
    public ConsoleColor FieldBackgroundColor { get; set; }
    #endregion

    #region Конструкторы
    /// <summary>Создаёт новую тему оформления для меню.</summary>
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
    public Theme(
        ConsoleColor titleText, ConsoleColor titleBg,
        ConsoleColor selectedText, ConsoleColor selectedBg,
        ConsoleColor unselectedText, ConsoleColor unselectedBg,
        ConsoleColor inputText, ConsoleColor inputBg,
        ConsoleColor fieldText, ConsoleColor fieldBg)
    {
        TitleTextColor = titleText;
        TitleBackgroundColor = titleBg;
        SelectedTextColor = selectedText;
        SelectedBackgroundColor = selectedBg;
        UnselectedTextColor = unselectedText;
        UnselectedBackgroundColor = unselectedBg;
        InputTextColor = inputText;
        InputBackgroundColor = inputBg;
        FieldTextColor = fieldText;
        FieldBackgroundColor = fieldBg;
    }
    #endregion
}

/// <summary>Перечисление доступных тем оформления меню.</summary>
public enum EThemes : byte
{
    /// <summary>Классическая тема (серый по чёрному).</summary>
    Classic,
    /// <summary>Тёмная тема (белый/зелёный по чёрному).</summary>
    Dark,
    /// <summary>Синяя тема.</summary>
    Blue,
    /// <summary>Жёлтая тема.</summary>
    Yellow,
    /// <summary>Зелёная тема.</summary>
    Green,
    /// <summary>Красная тема.</summary>
    Red,
}

/// <summary>
/// Класс, содержащий предустановленные темы оформления для меню.
/// </summary>
public static class Themes
{
    /// <summary>
    /// Словарь с предустановленными темами по ключу <see cref="EThemes"/>.
    /// </summary>
    public static readonly Dictionary<EThemes, Theme> Presets = new()
    {
        [EThemes.Classic] = new Theme(
            titleText: ConsoleColor.Gray, titleBg: ConsoleColor.Black,
            selectedText: ConsoleColor.Black, selectedBg: ConsoleColor.Gray,
            unselectedText: ConsoleColor.Gray, unselectedBg: ConsoleColor.Black,
            inputText: ConsoleColor.White, inputBg: ConsoleColor.DarkGray,
            fieldText: ConsoleColor.DarkGray, fieldBg: ConsoleColor.Black
        ),
        [EThemes.Dark] = new Theme(
            titleText: ConsoleColor.White, titleBg: ConsoleColor.Black,
            selectedText: ConsoleColor.Black, selectedBg: ConsoleColor.Green,
            unselectedText: ConsoleColor.Gray, unselectedBg: ConsoleColor.Black,
            inputText: ConsoleColor.White, inputBg: ConsoleColor.DarkGreen,
            fieldText: ConsoleColor.DarkGray, fieldBg: ConsoleColor.Black
        ),
        [EThemes.Blue] = new Theme(
            titleText: ConsoleColor.Cyan, titleBg: ConsoleColor.Black,
            selectedText: ConsoleColor.Black, selectedBg: ConsoleColor.Cyan,
            unselectedText: ConsoleColor.Gray, unselectedBg: ConsoleColor.Black,
            inputText: ConsoleColor.White, inputBg: ConsoleColor.DarkBlue,
            fieldText: ConsoleColor.DarkCyan, fieldBg: ConsoleColor.Black
        ),
        [EThemes.Yellow] = new Theme(
            titleText: ConsoleColor.Yellow, titleBg: ConsoleColor.Black,
            selectedText: ConsoleColor.Black, selectedBg: ConsoleColor.Yellow,
            unselectedText: ConsoleColor.Gray, unselectedBg: ConsoleColor.Black,
            inputText: ConsoleColor.Black, inputBg: ConsoleColor.Yellow,
            fieldText: ConsoleColor.DarkYellow, fieldBg: ConsoleColor.Black
        ),
        [EThemes.Green] = new Theme(
            titleText: ConsoleColor.Green, titleBg: ConsoleColor.Black,
            selectedText: ConsoleColor.Black, selectedBg: ConsoleColor.Green,
            unselectedText: ConsoleColor.Gray, unselectedBg: ConsoleColor.Black,
            inputText: ConsoleColor.White, inputBg: ConsoleColor.DarkGreen,
            fieldText: ConsoleColor.DarkGreen, fieldBg: ConsoleColor.Black
        ),
        [EThemes.Red] = new Theme(
            titleText: ConsoleColor.Red, titleBg: ConsoleColor.Black,
            selectedText: ConsoleColor.White, selectedBg: ConsoleColor.Red,
            unselectedText: ConsoleColor.Gray, unselectedBg: ConsoleColor.Black,
            inputText: ConsoleColor.White, inputBg: ConsoleColor.DarkRed,
            fieldText: ConsoleColor.DarkRed, fieldBg: ConsoleColor.Black
        ),
    };
}
