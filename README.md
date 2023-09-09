# Библиотека ConsoleMenu
Библиотека ConsoleMenu предоставляет простой способ создания текстовых меню в консоли для интерактивных приложений на C#.
## Установка

Для использования библиотеки ConsoleMenu, добавьте следующий `using` в вашем коде:
```csharp
global using ConsoleMenu;
```
## Использование
1. Создайте экземпляр класса Menu с помощью конструктора, указав заголовок и элементы меню:
```csharp
Menu MainMenu = new Menu("Выберите элемент меню:", 
    new MenuItem("Метка 1", Test("Метка 1")),
    new MenuItem("Метка 2", Test("Метка 2")),
    new MenuItem("Метка 3", Test("Метка 3"))
)
.AddMenuItem("Метка 4", Test("Метка 4"))
.AddMenuItem("Метка 5", Test("Метка 5"));
```
2. В конструкторе элементов меню первый параметр - текст, который будет отображаться в консоли, а второй параметр - функция Action, которая будет выполнена при выборе элемента меню.
3. В menu.GetIndex() и menu.Apply() можно указать начальное значение, на котором будет находиться селектор выбора элемента меню.
## Пример использования в коде:
```csharp
public static void Main()
{
    Menu MainMenu = new Menu("Выберите элемент меню:",
            new MenuItem("Метка 1", () => { Console.WriteLine("Метка 1!"); Console.ReadKey(); }),
            new MenuItem("Метка 2", () => { Console.WriteLine("Метка 2!"); Console.ReadKey(); }),
            new MenuItem("Метка 3", () => { Console.WriteLine("Метка 3!"); Console.ReadKey(); })
        )
    .AddMenuItem("Метка 4", () => { Console.WriteLine("Метка 4!"); Console.ReadKey(); })
    .AddMenuItem("Метка 5", () => { Console.WriteLine("Метка 5!"); Console.ReadKey(); });

MainMenu:
    if (IsNewMenu)
    {
        MainMenu.Apply();
        goto MainMenu;
    }
    else switch (MainMenu.GetIndex())
    {
        case 0: goto label1;
        case 1: goto label2;
        case 2: goto label3;
        case 3: goto label4;
        case 4: goto label5;
    };

label1:
    Console.WriteLine("Метка 1!");
    Console.ReadKey();
    goto MainMenu;

label2:
    Console.WriteLine("Метка 2!");
    Console.ReadKey();
    goto MainMenu;

label3:
    Console.WriteLine("Метка 3!");
    Console.ReadKey();
    goto MainMenu;

label4:
    Console.WriteLine("Метка 4!");
    Console.ReadKey();
    goto MainMenu;

label5:
    Console.WriteLine("Метка 5!");
    Console.ReadKey();
    goto MainMenu;
}
```
