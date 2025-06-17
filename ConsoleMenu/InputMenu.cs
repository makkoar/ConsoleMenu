namespace ConsoleMenu;

/// <summary>Класс, отвечающий за консольное меню ввода.</summary>
public class InputMenu
{
    #region Поля
    /// <summary>Темы, которые будут применены к данному меню.</summary>
    public Themes Themes { get; set; } = new();
    /// <summary>Заголовок меню.</summary>
    public string Title { get; set; } = "Введите значение:";
    #endregion
}
