namespace ConsoleMenu.Models;

/// <summary>Представляет позицию курсора в двумерном пространстве (строка и столбец).</summary>
/// <param name="Line">Координата строки (Y).</param>
/// <param name="Column">Координата столбца (X).</param>
public readonly record struct CursorPosition(int Line, int Column);