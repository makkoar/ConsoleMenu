namespace ConsoleMenu
{
    public static class Data
    {
        public static Theme BlackTheme { get; } = new(ConsoleColor.Black, ConsoleColor.White);
        public static Theme GreenTheme { get; } = new(ConsoleColor.Green, ConsoleColor.Black);
        public static Theme YellowTheme { get; } = new(ConsoleColor.Yellow, ConsoleColor.DarkBlue);
        public static Theme RedTheme { get; } = new(ConsoleColor.Red, ConsoleColor.White);
        public static Theme BlueTheme { get; } = new(ConsoleColor.Blue, ConsoleColor.Yellow);
    }
}
