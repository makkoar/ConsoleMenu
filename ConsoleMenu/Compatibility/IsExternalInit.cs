namespace System.Runtime.CompilerServices;

/// <summary>Позволяет использовать init-only свойства в целевых фреймворках, которые не включают этот тип по умолчанию, таких как .NET Standard 2.1.<br/>Компилятор C# 9.0 и более поздних версий ищет этот тип и использует его для включения поддержки свойств `init`.</summary>
internal static class IsExternalInit { }