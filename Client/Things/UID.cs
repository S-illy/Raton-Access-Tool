namespace Client.Things
{
    internal class UID
    {
        public static string Value { get; set; }
        public static string Get()
        {
            if (Value == null)
            {
                string prefix = "Raton_";
                string suffix = Stuff.Helpers.Random(7);
                Value = prefix + suffix;
            }
            return Value;
        }
    }
}
