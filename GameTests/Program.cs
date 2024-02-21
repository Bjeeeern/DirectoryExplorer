using Xunit.Runner.InProc.SystemConsole;

namespace GameTests;

public class Program
{
    public static int Main(string[] args)
    {
        var result = ConsoleRunner.Run(args).GetAwaiter().GetResult();

        if (result != 0)
            Console.ReadKey();

        return result;
    }
}
