namespace ConsoleMenu
{
    public class Menu
    {
        string Title { get; set; }
        List<(byte Index, string Text)> MenuItems { get; set; } = new();

        public Theme MainTheme { get; set; } = Data.BlackTheme;
        public Theme SideTheme { get; set; } = Data.GreenTheme;

        public Menu(string Title, params string[] MenuItems)
        {
            byte MenuIndex = 0;
            foreach (string Item in MenuItems) (this.MenuItems ??= new()).Add((MenuIndex++, Item));
            this.Title = Title;
        }

        public byte GetIndex(params object[] Texts)
        {
            byte MenuIndex = 0;
            while (true)
            {
                Console.Clear();
                MainTheme.Apply();
                string Title = this.Title;
                for (int i = 0; i < Texts.Length; i++) Title = Title.Replace($"{{{i}}}", $"{Texts[i]}");
                Console.WriteLine(Title);
                foreach ((byte Index, string Text) Item in MenuItems)
                {
                    if (Item.Index == MenuIndex) SideTheme.Apply();
                    Console.WriteLine($"* {Item.Text}".PadRight(64));
                    MainTheme.Apply();
                }
                switch (Console.ReadKey().Key)
                {
                    case ConsoleKey.UpArrow: MenuIndex = (byte)(MenuIndex != 0 ? MenuIndex - 1 : MenuItems.Count - 1); break;
                    case ConsoleKey.DownArrow: MenuIndex = (byte)(MenuIndex != MenuItems.Count - 1 ? MenuIndex + 1 : 0); break;
                    case ConsoleKey.Enter: Console.Clear(); return MenuIndex;
                    case ConsoleKey.Escape: Process.GetCurrentProcess().Kill(); break;
                }
            }
        }
    }
}
