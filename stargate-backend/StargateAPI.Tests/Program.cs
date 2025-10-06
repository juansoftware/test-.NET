using StargateAPI.Tests.Commands;

namespace StargateAPI.Tests;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("🚀 StargateAPI Unit Test Runner");
        Console.WriteLine("===============================\n");

        var testRunner = new StandaloneTests();
        
        try
        {
            await testRunner.TestBasicFunctionality();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"\n❌ Test FAILED: {ex.Message}");
        }

        Console.WriteLine("\nPress any key to exit...");
        Console.ReadKey();
    }
}
