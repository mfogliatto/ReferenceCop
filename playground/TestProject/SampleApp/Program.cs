using SampleLibrary;

namespace SampleApp;

/// <summary>
/// Sample application to test ReferenceCop.
/// </summary>
public class Program
{
    public static void Main(string[] args)
    {
        Console.WriteLine("ReferenceCop Playground Test Application");
        Console.WriteLine("=========================================");

        var calculator = new Calculator();

        Console.WriteLine($"5 + 3 = {calculator.Add(5, 3)}");
        Console.WriteLine($"10 - 4 = {calculator.Subtract(10, 4)}");
        Console.WriteLine($"6 * 7 = {calculator.Multiply(6, 7)}");
        Console.WriteLine($"20 / 5 = {calculator.Divide(20, 5)}");

        Console.WriteLine("\nTest completed successfully!");

        // Uncomment to test violation detection:
        // 1. Add a PackageReference to Newtonsoft.Json in the .csproj
        // 2. Add: using Newtonsoft.Json;
        // 3. Build to see the warning
    }
}
