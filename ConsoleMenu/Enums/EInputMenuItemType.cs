namespace ConsoleMenu.Enums;

/// <summary>������������ �������������� ����� ����� ��� InputMenuItem.</summary>
public enum EInputMenuItemType : byte
{
    /// <summary>��������� ��� �����.</summary>
    String,
    /// <summary>������������� ��� �����.</summary>
    Int,
    /// <summary>������������� ��� ����� ��� �����.</summary>
    UInt,
    /// <summary>�������� ������������� ��� �����.</summary>
    Short,
    /// <summary>�������� ������������� ��� ����� ��� �����.</summary>
    UShort,
    /// <summary>��� ����� ��� �����.</summary>
    Byte,
    /// <summary>��� ����� ��� ��������� �����.</summary>
    SByte,
    /// <summary>��� ����� ��� ����� � ��������� ������ (float).</summary>
    Float,
    /// <summary>��� ����� ��� ����� � ��������� ������ ������� �������� (double).</summary>
    Double,
    /// <summary>��� ����� ��� ����������� �����.</summary>
    Decimal,
    /// <summary>���������� ��� �����.</summary>
    Bool
}