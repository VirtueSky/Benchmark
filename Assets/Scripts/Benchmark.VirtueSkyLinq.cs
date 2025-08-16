using System.Collections;
using VirtueSky.Linq;
using Unity.Profiling;

public partial class Benchmark
{
    public bool isBenchmarkVirtueSkyLinqDone = false;

    public void StartBenchmarkVirtueSkyLinq()
    {
        isBenchmarkVirtueSkyLinqDone = false;
        uiControl.UpdateStatusVirtueSkyLinq(isBenchmarkVirtueSkyLinqDone);
        // Chạy benchmark theo coroutine để tách frame, đo GC Alloc chuẩn
        StartCoroutine(VirtueSkyLinqRunAll());
    }

    IEnumerator VirtueSkyLinqRunAll()
    {
        UnityEngine.Debug.Log(
            $"[Benchmark] VirtueSky.Linq N={N}, Warmup={WarmupIters}, Measure={MeasureIters}, InnerLoops={InnerLoops}");
        _gcAllocRecorder = ProfilerRecorder.StartNew(ProfilerCategory.Memory, "GC Allocated In Frame");
        // 1. Aggregate
        yield return Bench("VirtueSky.Linq Aggregate",
            () =>
            {
                var s = _data.Reduce(0, (acc, x) => acc + x);
                _ = s;
            });

        // 2. Any
        yield return Bench("VirtueSky.Linq Any",
            () =>
            {
                bool anyEven = _data.Any(x => (x & 1) == 0);
                _ = anyEven;
            });

        // 3. All
        yield return Bench("VirtueSky.Linq All",
            () =>
            {
                bool allPositive = _data.AllF(x => x >= 0);
                _ = allPositive;
            });

        // 4. Average
        yield return Bench("VirtueSky.Linq Average",
            () =>
            {
                var avg = _data.Average();
                _ = avg;
            });

        // 6. Contains
        yield return Bench("VirtueSky.Linq Contains",
            () =>
            {
                bool hasVal = _data.Contains(N / 2);
                _ = hasVal;
            });

        // 7. Count
        yield return Bench("VirtueSky.Linq Count",
            () =>
            {
                var c = _data.Count(x => x > 10);
                _ = c;
            });


        // 9. First
        yield return Bench("VirtueSky.Linq First",
            () =>
            {
                var f = _data.First(x => x > 10);
                _ = f;
            });
        // 10. Last (có predicate). LƯU Ý: O(n) trên IEnumerable
        yield return Bench("VirtueSky.Linq Last(predicate)",
            () =>
            {
                var last = _data.Last(static x => x >= 0);
                _ = last;
            });

        // 11. Max
        yield return Bench("VirtueSky.Linq Max",
            () =>
            {
                var m = _data.Max();
                _ = m;
            });

        // 12. Min
        yield return Bench("VirtueSky.Linq Min",
            () =>
            {
                var m = _data.Min();
                _ = m;
            });

        // 13. OrderBy (materialize để ép sort thật sự)
        yield return Bench("VirtueSky.Linq OrderBy -> ToArray",
            () =>
            {
                var sorted = _data.OrderBy(static x => x);
                _ = sorted.Length;
            });

        // 16. Reverse
        yield return Bench("VirtueSky.Linq Reverse -> ToArray",
            () =>
            {
                var rev = _data.Reverse();
                _ = rev.Length;
            });

        // 17. Select (map) -> Sum để ép pipeline
        yield return Bench("VirtueSky.Linq Select -> Sum",
            () =>
            {
                var s = _data.Map(static x => x * 2).Sum(x => (long)x);
                _ = s;
            });

// // --- Single (đúng 1 phần tử thỏa điều kiện) ---
// // predicate tìm sentinel đã cài sẵn
//         yield return Bench("VirtueSky.Linq Single(predicate)",
//             () =>
//             {
//                 var v = _data.Single(static x => x == int.MaxValue);
//                 _ = v;
//             });

// --- Skip ---
// ép thực thi bằng Sum
        yield return Bench("VirtueSky.Linq Skip -> Sum",
            () =>
            {
                var s = _data.Skip(123).Sum(x => (long)x);
                _ = s;
            });

// --- Sum (trực tiếp) ---
        yield return Bench("VirtueSky.Linq Sum",
            () =>
            {
                var s = _data.Sum(x => (long)x);
                _ = s;
            });

// --- Take ---
// ép thực thi bằng Sum
        yield return Bench("VirtueSky.Linq Take -> Sum",
            () =>
            {
                var s = _data.Take(123).Sum(x => (long)x);
                _ = s;
            });

// --- Where ---
// ép thực thi bằng Count để tiêu thụ pipeline
        yield return Bench("VirtueSky.Linq Where -> Count",
            () =>
            {
                var c = _data.Filter(static x => (x & 1) == 0).Length;
                _ = c;
            });

// --- WhereAggregate (lọc rồi Aggregate) ---
        yield return Bench("VirtueSky.Linq Where+Aggregate",
            () =>
            {
                var s = _data.FilterReduce(static x => x >= 10, 0, static (acc, x) => acc + (x - 10));
                _ = s;
            });

// --- WhereSelect (lọc rồi map) ---> Sum để ép thực thi
        yield return Bench("VirtueSky.Linq Where+Select -> Sum",
            () =>
            {
                var s = _data.FilterMap(static x => (x & 1) == 0, static x => x * 2)
                    .Sum(x => (long)x);
                _ = s;
            });

// --- WhereSum (lọc rồi Sum) ---
        yield return Bench("VirtueSky.Linq Where -> Sum",
            () =>
            {
                var s = _data.FilterSum(static x => (long)x > 10);
                _ = s;
            });

        _gcAllocRecorder.Dispose();
        UnityEngine.Debug.Log("[Benchmark] VirtueSky.Linq Done.");
        isBenchmarkVirtueSkyLinqDone = true;
        uiControl.UpdateStatusVirtueSkyLinq(isBenchmarkVirtueSkyLinqDone);
    }
}