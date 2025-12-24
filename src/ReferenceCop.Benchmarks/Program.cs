namespace ReferenceCop.Benchmarks
{
    using BenchmarkDotNet.Running;

    public class Program
    {
        public static void Main(string[] args)
        {
            BenchmarkRunner.Run<AssemblyNameViolationDetectorBenchmarks>();
        }
    }
}
