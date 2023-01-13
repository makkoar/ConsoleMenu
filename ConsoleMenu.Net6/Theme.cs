namespace ConsoleMenu
{
    public class Theme
    {
        public ConsoleColor TextColor { get; set; }
        public ConsoleColor BackgroundColor { get; set;}

        public Theme(ConsoleColor BackgroundColor, ConsoleColor TextColor)
        {
            this.BackgroundColor = BackgroundColor;
            this.TextColor = TextColor;
        }

        public void Apply()
        {
            Console.BackgroundColor = BackgroundColor;
            Console.ForegroundColor = TextColor;
        }
    }
}
