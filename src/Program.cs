using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;

namespace PointerPerformanceTest
{
    /// <summary>
    /// Runs some simple tests to evaluate the performance difference between
    /// normal array access in managed code and unsafe pointer access.
    /// </summary>
    public class Program
    {
        private const int BufferSize = 1000000;
        private const int Repeats = 10000;

        public static void Main(string[] args)
        {
            int[] buffer = CreateBuffer();

            Console.WriteLine("Linear array access");
            int resultNormalArrayTest = RunTest(buffer, NormalArrayLinearAccess, "Normal");
            int resultUnsafeArrayTest = RunTest(buffer, UnsafeArrayLinearAccess, "Unsafe");
            Trace.Assert(resultNormalArrayTest == resultUnsafeArrayTest);

            Console.WriteLine("Linear array access - with pointer increment");
            resultNormalArrayTest = RunTest(buffer, NormalArrayLinearAccess, "Normal");
            resultUnsafeArrayTest = RunTest(buffer, UnsafeArrayLinearAccessWithPointerIncrement, "Unsafe (*p++)");
            Trace.Assert(resultNormalArrayTest == resultUnsafeArrayTest);

            Console.WriteLine("Random array access");
            resultNormalArrayTest = RunTest(buffer, NormalArrayRandomAccess, "Normal");
            resultUnsafeArrayTest = RunTest(buffer, UnsafeArrayRandomAccess, "Unsafe");
            Trace.Assert(resultNormalArrayTest == resultUnsafeArrayTest);

            Console.WriteLine("Random array access using Parallel.For(), with {0} processors", Environment.ProcessorCount);
            resultNormalArrayTest = RunParallelTest(buffer, NormalArrayRandomAccess, "Normal");
            resultUnsafeArrayTest = RunParallelTest(buffer, UnsafeArrayRandomAccess, "Unsafe");
            Trace.Assert(resultNormalArrayTest == resultUnsafeArrayTest);
        }

        private static int[] CreateBuffer()
        {
            var random = new Random();
            int[] buffer = new int[BufferSize];
            for (int i = 0; i < buffer.Length; i++)
            {
                buffer[i] = random.Next();
            }
            return buffer;
        }

        private static int RunTest(int[] buffer, Func<int[], int>testMethod, string name)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            int result = 0;
            for (int i = 0; i < Repeats; ++i)
            {
                result += testMethod(buffer);
            }
            stopwatch.Stop();
            Console.WriteLine(  " {0} for {1}",
                                stopwatch.Elapsed,
                                name);
            return result;
        }

        private static int RunParallelTest(int[] buffer, Func<int[], int> testMethod, string name)
        {
            var stopwatch = new Stopwatch();
            stopwatch.Start();
            int result = 0;
            Parallel.For<int>(0, Repeats,
                () =>   
                    {
                        return 0;
                    },
                (i, parallelLoopState, localSum) =>
                    {
                        localSum += testMethod(buffer);
                        return localSum;
                    },
                localSum =>
                    {
                        Interlocked.Add(ref result, localSum);
                    }
            );
            stopwatch.Stop();
            Console.WriteLine(  " {0} for {1}",
                                stopwatch.Elapsed,
                                name);
            return result;
        }

        private static int NormalArrayLinearAccess(int[] buffer)
        {
            int sum = 0;
            for(int i=0; i<buffer.Length; ++i)
            {
                sum += buffer[i];
            }
            return sum;
        }

        private unsafe static int UnsafeArrayLinearAccess(int[] buffer)
        {
            fixed(int*pointer = &buffer[0])
            {
                int* current = pointer;

                int sum = 0;
                for (int i = 0; i < buffer.Length; ++i)
                {
                    sum += *(current + i);
                }
                return sum;
            }
        }

        private unsafe static int UnsafeArrayLinearAccessWithPointerIncrement(int[] buffer)
        {
            fixed (int* pointer = &buffer[0])
            {
                int* current = pointer;

                int sum = 0;
                for (int i = 0; i < buffer.Length; ++i)
                {
                    sum += *(current++);
                }
                return sum;
            }
        }

        private static int NormalArrayRandomAccess(int[] buffer)
        {
            int sum = 0;
            for (int i = 0; i < buffer.Length; ++i)
            {
                int value = buffer[i];
                // Access a random location.
                int index = value % buffer.Length;
                sum += buffer[index];   
            }
            return sum;
        }

        private unsafe static int UnsafeArrayRandomAccess(int[] buffer)
        {
            fixed (int* pointer = &buffer[0])
            {
                int* current = pointer;

                int sum = 0;
                for (int i = 0; i < buffer.Length; ++i)
                {
                    int value = *(current + i);
                    // Access a random location.
                    int index = value % buffer.Length;
                    sum += *(current + index);
                }
                return sum;
            }
        }
    }
}
