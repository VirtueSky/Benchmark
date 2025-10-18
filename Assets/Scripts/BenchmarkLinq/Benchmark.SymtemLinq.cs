using System.Collections;
using System.Linq;
using Unity.Profiling;

public partial class Benchmark
{
    public bool isBenchmarkSystemLinqDone = false;


    public void StartBenchmarkSystemLinq()
    {
        isBenchmarkSystemLinqDone = false;
        uiControl.UpdateStatusSystemLinq(isBenchmarkSystemLinqDone);
        // Chạy benchmark theo coroutine để tách frame, đo GC Alloc chuẩn
        StartCoroutine(SystemLinqRunAll());
    }

    IEnumerator SystemLinqRunAll()
    {
        UnityEngine.Debug.Log(
            $"[Benchmark] System.Linq N={N}, Warmup={WarmupIters}, Measure={MeasureIters}, InnerLoops={InnerLoops}");
        _gcAllocRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Allocated In Frame");
        // 1. Aggregate
        yield return Bench("System.Linq Aggregate",
            () =>
            {
                var s = _data.Aggregate(0, (acc, x) => acc + x);
                _ = s;
            });

        // 2. Any
        yield return Bench("System.Linq Any",
            () =>
            {
                bool anyEven = _data.Any(x => (x & 1) == 0);
                _ = anyEven;
            });

        // 3. All
        yield return Bench("System.Linq All",
            () =>
            {
                bool allPositive = _data.All(x => x >= 0);
                _ = allPositive;
            });

        // 4. Average
        yield return Bench("System.Linq Average",
            () =>
            {
                var avg = _data.Average();
                _ = avg;
            });

        // 6. Contains
        yield return Bench("System.Linq Contains",
            () =>
            {
                bool hasVal = _data.Contains(N / 2);
                _ = hasVal;
            });

        // 7. Count
        yield return Bench("System.Linq Count",
            () =>
            {
                var c = _data.Count(x => x > 10);
                _ = c;
            });

        // 9. First
        yield return Bench("System.Linq First",
            () =>
            {
                var f = _data.First(x => x > 10);
                _ = f;
            });
        // 10. Last (có predicate). LƯU Ý: O(n) trên IEnumerable
        yield return Bench("System.Linq Last(predicate)",
            () =>
            {
                var last = _data.Last(static x => x >= 0);
                _ = last;
            });

        // 11. Max
        yield return Bench("System.Linq Max",
            () =>
            {
                var m = _data.Max();
                _ = m;
            });

        // 12. Min
        yield return Bench("System.Linq Min",
            () =>
            {
                var m = _data.Min();
                _ = m;
            });

        // 13. OrderBy (materialize để ép sort thật sự)
        yield return Bench("System.Linq OrderBy -> ToArray",
            () =>
            {
                var sorted = _data.OrderBy(static x => x).ToArray();
                _ = sorted.Length;
            });

        // 16. Reverse
        yield return Bench("System.Linq Reverse -> ToArray",
            () =>
            {
                var rev = _data.Reverse().ToArray();
                _ = rev.Length;
            });

        // 17. Select (map) -> Sum để ép pipeline
        yield return Bench("System.Linq Select -> Sum",
            () =>
            {
                var s = _data.Select(static x => x * 2).Sum(x => (long)x);
                _ = s;
            });

// // --- Single (đúng 1 phần tử thỏa điều kiện) ---
// // predicate tìm sentinel đã cài sẵn
//         yield return Bench("System.Linq Single(predicate)",
//             () =>
//             {
//                 var v = _data.Single(static x => x == int.MaxValue);
//                 _ = v;
//             });

// --- Skip ---
// ép thực thi bằng Sum
        yield return Bench("System.Linq Skip -> Sum",
            () =>
            {
                var s = _data.Skip(123).Sum(x => (long)x);
                _ = s;
            });

// --- Sum (trực tiếp) ---
        yield return Bench("System.Linq Sum",
            () =>
            {
                var s = _data.Sum(x => (long)x);
                _ = s;
            });

// --- Take ---
// ép thực thi bằng Sum
        yield return Bench("System.Linq Take -> Sum",
            () =>
            {
                var s = _data.Take(123).Sum(x => (long)x);
                _ = s;
            });

// --- Where ---
// ép thực thi bằng Count để tiêu thụ pipeline
        yield return Bench("System.Linq Where -> Count",
            () =>
            {
                var c = _data.Where(static x => (x & 1) == 0).Count();
                _ = c;
            });

// --- WhereAggregate (lọc rồi Aggregate) ---
        yield return Bench("System.Linq Where+Aggregate",
            () =>
            {
                var s = _data.Where(static x => x >= 10)
                    .Aggregate(0, static (acc, x) => acc + (x - 10));
                _ = s;
            });

// --- WhereSelect (lọc rồi map) ---> Sum để ép thực thi
        yield return Bench("System.Linq Where+Select -> Sum",
            () =>
            {
                var s = _data.Where(static x => (x & 1) == 0)
                    .Select(static x => x * 2)
                    .Sum(x => (long)x);
                _ = s;
            });

// --- WhereSum (lọc rồi Sum) ---
        yield return Bench("System.Linq Where -> Sum",
            () =>
            {
                var s = _data.Where(static x => x > 10).Sum(x => (long)x);
                _ = s;
            });

        _gcAllocRecorder.Dispose();
        UnityEngine.Debug.Log("[Benchmark] System.Linq Done.");
        isBenchmarkSystemLinqDone = true;
        uiControl.UpdateStatusSystemLinq(isBenchmarkSystemLinqDone);
    }
    /*
     * Aggregate, AnyAll, Average, Chunk, Contains, Count, Distinct, First,
     * Flatten, Last, Max, Min, OrderBy, Range, Repeat, Reverse, Select,
     * SelectMany, Single, Skip, Sum, Take, Where, WhereAggregate, WhereSelect, WhereSum, Zip
     */
}