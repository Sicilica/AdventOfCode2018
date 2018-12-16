namespace AdventOfCode2018
{
    public abstract class Challenge
    {
        public abstract string Part1();
        public abstract string Part2();



        protected System.IO.Stream GetResource(string id)
        {
            return System.Reflection.Assembly.GetExecutingAssembly().GetManifestResourceStream($"AdventOfCode2018.{id.Replace('/', '.')}");
        }
    }
}
