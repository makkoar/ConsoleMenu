namespace ConsoleMenu;
/// <summary>Класс, описывающий темы.</summary>
public class Theme
{
    #region Поля
    /// <summary>Цвет текста консоли.</summary>
    public ConsoleColor TextColor { get; set; }
    /// <summary>Цвет фона консоли.</summary>
    public ConsoleColor BackgroundColor { get; set; }
    #endregion

    #region Конструкторы
    public Theme(ConsoleColor backgroundColor, ConsoleColor textColor)
    {
        BackgroundColor = backgroundColor;
        TextColor = textColor;
    }
    #endregion

    #region Функции активации темы
    /// <summary>Активирует тему.</summary>
    public void Apply()
    {
        Console.BackgroundColor = BackgroundColor;
        Console.ForegroundColor = TextColor;
    }
    #endregion
}

/// <summary>Класс, содержащий информацию о темах меню.</summary>
public class Themes
{
    /// <summary>Шаблоны тем.</summary>
    public static class Templates
    {
        /// <summary>Тема с чёрным фоном и белым текстом.</summary>
        public static Theme Black { get; } = new(ConsoleColor.Black, ConsoleColor.White);
        /// <summary>Тема с зелёным фоном и чёрным текстом.</summary>
        public static Theme Green { get; } = new(ConsoleColor.Green, ConsoleColor.Black);
        /// <summary>Тема с жёлтым фоном и тёмно-синим текстом.</summary>
        public static Theme Yellow { get; } = new(ConsoleColor.Yellow, ConsoleColor.DarkBlue);
        /// <summary>Тема с красным фоном и белым текстом.</summary>
        public static Theme Red { get; } = new(ConsoleColor.Red, ConsoleColor.White);
        /// <summary>Тема с синим фоном и жёлтым текстом.</summary>
        public static Theme Blue { get; } = new(ConsoleColor.Blue, ConsoleColor.Yellow);
    }

    #region Поля
    /// <summary>Тема заголовка меню.</summary>
    public Theme Title { get; set; } = Templates.Black;
    /// <summary>Тема выбранного элемента меню.</summary>
    public Theme Selected { get; set; } = Templates.Green;
    /// <summary>Тема невыбранных элементов меню.</summary>
    public Theme Unselected { get; set; } = Templates.Black;
    #endregion
}
