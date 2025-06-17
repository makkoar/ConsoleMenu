# Библиотека ConsoleMenu
Библиотека ConsoleMenu предоставляет простой способ создания интерактивных текстовых меню в консольных приложениях на C#. Она поддерживает два основных типа меню:
- **`SelectMenu`**: для выбора одного из предложенных вариантов.
- **`InputMenu`**: для организации ввода нескольких полей данных от пользователя.

## Установка
Для использования библиотеки ConsoleMenu, добавьте следующий `using` в вашем коде, например, в файл `Usings.cs`:
```csharp
global using ConsoleMenu;
```

## Использование `SelectMenu` (Меню выбора)

Класс `SelectMenu` позволяет создавать меню, в которых пользователь может выбрать один из предложенных вариантов с помощью клавиш со стрелками.

### 1. Создание меню
Вы можете создать экземпляр `SelectMenu`, используя конструктор и передав ему заголовок и элементы меню. Элементы меню — это экземпляры `SelectMenuItem`, которые содержат текст для отображения и необязательное действие (`Action`), выполняемое при выборе.

```csharp
// Создание меню с заголовком и начальными элементами
SelectMenu mainMenu = new("Основное меню",
    new SelectMenuItem("Пункт 1", () => Console.WriteLine("Выбран пункт 1")),
    new SelectMenuItem("Пункт 2", () => Console.WriteLine("Выбран пункт 2"))
);

// Добавление дополнительных элементов с помощью Fluent API
mainMenu.AddMenuItem("Пункт 3", () => Console.WriteLine("Выбран пункт 3"))
        .AddMenuItem("Выход"); // Элемент без действия
```

### 2. Отображение меню и обработка выбора
Метод `Apply()` отображает меню, ожидает выбора пользователя (клавиши `Enter` или `Escape`) и возвращает индекс (`ushort`) выбранного элемента. Если для выбранного `SelectMenuItem` было задано действие (`Action`), оно будет автоматически выполнено при нажатии `Enter`.

```csharp
// Способ 1: Автоматическое выполнение действия
// Метод Apply() отобразит меню и вызовет Action, связанный с выбранным пунктом.
mainMenu.Apply();

// Способ 2: Ручная обработка по индексу
// Метод Apply() все равно вызовет Action (если он есть), но также вернёт индекс.
// Это полезно для дополнительной логики, например, в операторе switch.
ushort choice = mainMenu.Apply();
switch (choice)
{
    case 0:
        Console.WriteLine("Дополнительная логика для пункта 1...");
        break;
    case 3: // Индекс пункта "Выход"
        return; // Выходим из цикла или программы
    // ...
}
```

### 3. Настройка тем
Вы можете изменить внешний вид меню, настроив его темы. Библиотека предоставляет несколько встроенных шаблонов в `Themes.Templates`.

```csharp
mainMenu.SetThemes(
    title:      Themes.Templates.Yellow, // Тема для заголовка
    selected:   Themes.Templates.Green,  // Тема для выбранного пункта
    unselected: Themes.Templates.Black   // Тема для остальных пунктов
);

// Также можно создать свою тему
var customTheme = new Theme(ConsoleColor.DarkMagenta, ConsoleColor.White);
mainMenu.Theme.Selected = customTheme;
```

## Использование `InputMenu` (Меню ввода)

`InputMenu` позволяет организовать удобный ввод данных от пользователя по нескольким полям.

### 1. Создание меню
Вы создаете экземпляр `InputMenu` и добавляете в него элементы `InputMenuItem`. Каждый элемент имеет текст-приглашение, значение по умолчанию и необязательный уникальный идентификатор (`Id`), который важен для последующего получения данных.

```csharp
// Создание меню ввода с помощью Fluent API
var inputMenu = new InputMenu("Введите данные для подключения")
    .AddMenuItem("Адрес сервера", "127.0.0.1", id: "server_ip")
    .AddMenuItem("Порт", "8080", id: "port")
    .AddMenuItem("Логин", "admin", id: "login");
```
Если `Id` не указан, он будет сгенерирован автоматически (начиная с "0").

### 2. Отображение и ввод данных
Метод `Apply()` отображает все поля ввода и последовательно активирует их. Пользователь вводит данные в каждое поле, нажимая `Enter` для перехода к следующему. Нажатие `Escape` в любой момент отменит весь ввод и вернёт исходные значения.

```csharp
// Отобразить меню и запустить процесс ввода
inputMenu.Apply();
```

### 3. Получение результатов
После того как пользователь заполнил все поля (или отменил ввод), вы можете получить введённые значения несколькими способами.

```csharp
// Способ 1: Получить словарь с результатами
// Ключ - это ID, который вы задали, значение - введённая строка.
Dictionary<string, string?> results = inputMenu.GetInputs();
Console.WriteLine($"Подключаемся к {results["server_ip"]}:{results["port"]}");

// Способ 2: Обратиться напрямую к элементам меню
// Это удобно для использования методов-помощников для парсинга.
var portItem = inputMenu.MenuItems.First(item => item.Id == "port");
if (portItem.TryGetInt(out int portNumber))
{
    Console.WriteLine($"Порт в числовом формате: {portNumber}");
}
else
{
    Console.WriteLine("Не удалось преобразовать порт в число.");
}
```

### 4. Настройка тем
Как и `SelectMenu`, `InputMenu` поддерживает настройку тем для заголовка, активного (редактируемого) поля и неактивных полей.

```csharp
inputMenu.SetThemes(
    title:      Themes.Templates.Blue,   // Тема для заголовка
    selected:   Themes.Templates.Yellow, // Тема для активного поля ввода
    unselected: Themes.Templates.Black   // Тема для остальных полей
);
```

## Пример использования в коде

Ниже приведён полный пример консольного приложения, демонстрирующий совместное использование `SelectMenu` и `InputMenu`.

```csharp
using System;
using System.Linq;

// Предполагается, что 'global using ConsoleMenu;' находится в файле Usings.cs

public static class Program
{
    // Хранилище "настроек" для примера
    private static string playerName = "User";
    
    public static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        // Создаем меню для ввода имени игрока
        var nameInputMenu = new InputMenu("Настройки игрока")
            .AddMenuItem("Введите новое имя", playerName, "player_name");

        // Создаем меню настроек, которое может вызывать меню ввода
        var settingsMenu = new SelectMenu("Настройки")
            .AddMenuItem("Изменить имя игрока", () => 
            {
                nameInputMenu.Apply(); // Запускаем меню ввода
                // После ввода обновляем нашу переменную
                playerName = nameInputMenu.GetInputs()["player_name"] ?? playerName;
                Console.WriteLine($"Имя изменено на: {playerName}");
                Console.ReadKey();
            })
            .AddMenuItem("Назад");

        // Создаем главное меню
        var mainMenu = new SelectMenu("Главное меню")
            .SetThemes(Themes.Templates.Blue, Themes.Templates.Green, Themes.Templates.Black)
            .AddMenuItem("Начать игру", () => Console.WriteLine($"Игра началась! Привет, {playerName}!"))
            .AddMenuItem("Настройки", () => settingsMenu.Apply()) // Вложенное меню выбора
            .AddMenuItem("Выход");
            
        // Запускаем главный цикл меню
        while (true)
        {
            Console.Clear();
            ushort choice = mainMenu.Apply(clear: false); // Отображаем меню

            if (mainMenu.MenuItems[choice].Text == "Выход")
            {
                Environment.Exit(0);
            }
            
            Console.WriteLine("\nНажмите любую клавишу для возврата в меню...");
            Console.ReadKey();
        }
    }
}
```