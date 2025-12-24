# ReferenceCop Benchmarks

This project contains performance benchmarks for the ReferenceCop library using BenchmarkDotNet.

## Running the Benchmarks

To run the benchmarks, execute the following command from the repository root:

```powershell
dotnet run -c Release --project src/ReferenceCop.Benchmarks/ReferenceCop.Benchmarks.csproj
```

## AssemblyNameViolationDetectorBenchmarks

Compares the performance of the original `GetViolationsFrom` method against the optimized `GetViolationsFromExperimental` method.

## Understanding the Results

BenchmarkDotNet will display:
- **Mean**: Average execution time
- **Error**: Standard error of all measurements
- **StdDev**: Standard deviation of all measurements
- **Rank**: Relative ranking of methods
- **Gen0/1/2**: Garbage collection statistics
- **Allocated**: Total memory allocated

Look for the **Rank** column and **Mean** times to compare performance.
