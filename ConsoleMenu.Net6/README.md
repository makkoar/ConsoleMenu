Добавляем все необходимые using:
```
global using ConsoleMenu;
global using static ConsoleMenu.Menu;
```
Создаём экземпляр класса Menu
```
public static Menu MainMenu = new("Главное меню:", "Пункт 1", "Пункт 2");
```
где Первый параметр - заголовок, а все последующие пункты меню.

Пример использования в коде:
```
public static void Main()
{
MainMenu:
    switch (MainMenu.GetIndex()) //Возвращает Index последнего выбранного пункта меню до нажатия на Enter. (Нажание на Esc закроет программу.)
    {
        case 0: F1(); Console.ReadKey(); break; //Действие при выборе первого пункта меню.
        case 1: F2(); Console.ReadKey(); break; //Действие при выборе второго пункта меню.
    }
    goto MainMenu;
}
```
В Menu.GetIndex() можно в качестве параметров можно указать строки, которые заменят в {0} в тексте, например при:
```
public static Menu MainMenu = new("{0} {1}:", "Пункт 1", "Пункт 2");
```
при
```
_ = MainMenu.GetIndex("Главное", "меню");
``` 
выведет
```
Главное меню:
* Пункт 1
* Пункт 2
```
