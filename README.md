# Библиотека ConsoleMenu
Библиотека ConsoleMenu предоставляет простой способ создания интерактивных текстовых меню в консольных приложениях на C#. Она поддерживает два основных типа меню:
- **`SelectMenu`**: для выбора одного из предложенных вариантов.
- **`InputMenu`**: для организации ввода нескольких полей данных от пользователя с поддержкой валидации типов.

## Установка
Для использования библиотеки ConsoleMenu, добавьте следующий `using` в вашем коде, например, в файл `Usings.cs` или в начало вашего файла:
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
Вы можете легко изменить внешний вид меню, присвоив свойству `Theme` одну из готовых тем из статического класса `Themes` или создав свою.

```csharp
// Способ 1: Применить готовую тему из `Themes`
mainMenu.Theme = Themes.Green;

// Способ 2: Настроить цвета вручную с помощью метода SetThemes
mainMenu.SetThemes(
    titleTextColor:          ConsoleColor.Yellow,
    titleBackgroundColor:    ConsoleColor.Black,
    selectedTextColor:       ConsoleColor.Black,
    selectedBackgroundColor: ConsoleColor.Green,
    unselectedTextColor:     ConsoleColor.Gray,
    unselectedBackgroundColor: ConsoleColor.Black
);

// Способ 3: Создать и модифицировать свою тему на основе существующей
var customTheme = Themes.Classic; // Копируем классическую тему
customTheme.SelectedBackgroundColor = ConsoleColor.DarkMagenta;
customTheme.SelectedTextColor = ConsoleColor.White;
mainMenu.Theme = customTheme;
```

## Использование `InputMenu` (Меню ввода)

`InputMenu` позволяет организовать удобный ввод данных от пользователя по нескольким полям, с валидацией типов и гибкой навигацией.

### 1. Создание меню
Вы создаете экземпляр `InputMenu` и добавляете в него элементы `InputMenuItem`. Каждый элемент имеет текст-приглашение, значение по умолчанию, уникальный идентификатор (`Id`) и, что важно, тип вводимых данных (`EInputMenuItemType`), который ограничивает ввод пользователя.

```csharp
// Создание меню ввода с помощью Fluent API и указанием типов
var inputMenu = new InputMenu("Введите данные для подключения")
    .AddMenuItem(
        text: "Адрес сервера", 
        defaultValue: "127.0.0.1", 
        id: "server_ip") // Тип по умолчанию - String
    .AddMenuItem(
        text: "Порт", 
        defaultValue: "8080", 
        id: "port", 
        type: EInputMenuItemType.UShort) // Ввод будет ограничен числами от 0 до 65535
    .AddMenuItem(
        text: "Использовать SSL", 
        defaultValue: "true", 
        id: "use_ssl", 
        type: EInputMenuItemType.Bool); // Ожидается ввод true/false
```
Если `Id` не указан, он будет сгенерирован автоматически (начиная с "0").

### 2. Отображение и ввод данных
Метод `Apply()` отображает все поля ввода и управляет процессом:
- Используйте **стрелки вверх/вниз** для перемещения между полями.
- Редактируйте значение в выбранном поле. Клавиши **стрелки влево/вправо**, **Backspace**, **Delete** работают как в обычном текстовом редакторе.
- Если текст в поле ввода занимает несколько строк, стрелки вверх/вниз сначала будут перемещать курсор внутри поля.
- Нажмите **Enter**, чтобы подтвердить все введённые значения и завершить работу меню.
- Нажмите **Escape**, чтобы отменить все изменения и выйти из меню.

```csharp
// Отобразить меню и запустить процесс ввода. 
// Метод вернёт словарь с результатами.
var results = inputMenu.Apply();
```

### 3. Получение результатов
Метод `Apply()` возвращает `Dictionary<string, InputMenuItem>`. Ключ — это `Id` элемента, а значение — сам объект `InputMenuItem`, содержащий введённые данные. Это позволяет легко получить результат и безопасно преобразовать его к нужному типу.

```csharp
// Отображаем меню и получаем результаты
Dictionary<string, InputMenuItem> results = inputMenu.Apply();

// Способ 1: Получить значения по ID и использовать их как строки
if (results.TryGetValue("server_ip", out var ipItem) && results.TryGetValue("port", out var portItem))
{
    Console.WriteLine($"Подключаемся к {ipItem.InputValue}:{portItem.InputValue}");
}

// Способ 2: Использовать методы-помощники для безопасного преобразования типов
if (results.TryGetValue("port", out var portItem))
{
    if (portItem.TryGetUShort(out ushort portNumber))
    {
        Console.WriteLine($"Порт в числовом формате: {portNumber}");
    }
    else
    {
        Console.WriteLine($"Не удалось преобразовать '{portItem.InputValue}' в число.");
    }
}
```

### 4. Настройка тем
Как и `SelectMenu`, `InputMenu` поддерживает настройку тем через свойство `Theme`.

```csharp
// Способ 1: Применить готовую тему
inputMenu.Theme = Themes.Cyan;

// Способ 2: Создать и настроить свою тему
var customTheme = Themes.Classic;
customTheme.SelectedBackgroundColor = ConsoleColor.DarkBlue; // Цвет фона активного поля
customTheme.SelectedTextColor = ConsoleColor.White;       // Цвет текста активного поля
inputMenu.Theme = customTheme;
```

## Пример использования в коде

Ниже приведён полный пример консольного приложения, демонстрирующий совместное использование `SelectMenu` и `InputMenu`.

```csharp
using System;
using System.Collections.Generic;
using ConsoleMenu; // Убедитесь, что using добавлен

public static class Program
{
    // Хранилище "настроек" для примера
    private static string playerName = "User";
    private static byte playerAge = 25;
    
    public static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        // Создаем меню для ввода настроек
        var settingsInputMenu = new InputMenu("Настройки игрока")
            .AddMenuItem("Введите новое имя", playerName, "player_name")
            .AddMenuItem("Введите возраст", playerAge.ToString(), "player_age", EInputMenuItemType.Byte);
        settingsInputMenu.Theme = Themes.Yellow;

        // Создаем меню настроек, которое вызывает меню ввода
        var settingsMenu = new SelectMenu("Настройки")
            .AddMenuItem("Изменить данные игрока", () => 
            {
                // Запускаем меню ввода и получаем результат
                Dictionary<string, InputMenuItem> results = settingsInputMenu.Apply(clear: true); 

                // После ввода обновляем наши переменные
                if (results.TryGetValue("player_name", out var nameItem) && !string.IsNullOrEmpty(nameItem.InputValue))
                {
                    playerName = nameItem.InputValue;
                }
                if (results.TryGetValue("player_age", out var ageItem) && ageItem.TryGetByte(out byte newAge))
                {
                    playerAge = newAge;
                }
                
                Console.WriteLine($"\nНастройки обновлены. Имя: {playerName}, Возраст: {playerAge}");
                Console.ReadKey();
            })
            .AddMenuItem("Назад");
        settingsMenu.Theme = Themes.Cyan;

        // Создаем главное меню
        var mainMenu = new SelectMenu("Главное меню")
            .AddMenuItem("Начать игру", () => Console.WriteLine($"Игра началась! Привет, {playerName}! Тебе {playerAge} лет."))
            .AddMenuItem("Настройки", () => settingsMenu.Apply(clear: true)) // Вложенное меню выбора
            .AddMenuItem("Выход");
        mainMenu.Theme = Themes.Blue;
            
        // Запускаем главный цикл меню
        while (true)
        {
            Console.Clear();
            ushort choice = mainMenu.Apply(clear: false); // Отображаем меню

            if (mainMenu.MenuItems[choice].Text == "Выход")
            {
                break;
            }
            
            Console.WriteLine("\nНажмите любую клавишу для возврата в меню...");
            Console.ReadKey();
        }
        Console.Clear();
        Console.WriteLine("Программа завершена.");
    }
}
```