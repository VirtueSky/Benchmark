using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Profiling;
using UnityEngine;
using VirtueSky.Inspector;
using Debug = UnityEngine.Debug;

public partial class Benchmark : MonoBehaviour
{
    public UiControl uiControl;
    [Header("Data")] public int N = 100_000;
    public int Seed = 42;

    [Header("Run Config")] public int WarmupIters = 3;
    public int MeasureIters = 8;
    public int InnerLoops = 1; // lặp lại trong 1 lần đo để tăng thời lượng

    protected int[] _data;
    protected System.Random _rnd;
    protected ProfilerRecorder _gcAllocRecorder;
    [TableList] public List<ResultData> resultDatas = new List<ResultData>();

    public void GetData()
    {
        // Sinh dữ liệu
        _rnd = new System.Random(Seed);
        _data = new int[N];
        for (int i = 0; i < N; i++)
        {
            _data[i] = _rnd.Next(0, N / 2);
        }

        Debug.Log("[Benchmark] get data done");
    }

    public IEnumerator Bench(string name, System.Action action)
    {
        // WARMUP (mỗi lần ở frame riêng để tránh lẫn GC alloc)
        for (int i = 0; i < WarmupIters; i++)
        {
            yield return null; // sang frame mới
            RunOnce(action);
        }

        // MEASURE
        double bestMs = double.MaxValue, totalMs = 0;
        long bestAlloc = long.MaxValue, totalAlloc = 0;

        for (int i = 0; i < MeasureIters; i++)
        {
            yield return null; // đảm bảo mỗi lần đo là 1 frame riêng

            (double ms, long alloc) = RunOnce(action);

            if (ms < bestMs) bestMs = ms;
            if (alloc < bestAlloc) bestAlloc = alloc;
            totalMs += ms;
            totalAlloc += alloc;
        }

        double avgMs = totalMs / MeasureIters;
        double avgAllocKB = totalAlloc / (1024.0 * MeasureIters);
        double bestAllocKB = bestAlloc / 1024.0;

        UnityEngine.Debug.Log(
            $"[Benchmark] {name}  | best={bestMs:F3} ms, avg={avgMs:F3} ms, " +
            $"alloc(best)={bestAllocKB:F1} KiB, alloc(avg)={avgAllocKB:F1} KiB"
        );
        resultDatas.Add(new ResultData(name, bestMs, avgMs, bestAllocKB, avgAllocKB));
    }

    (double ms, long alloc) RunOnce(Action action)
    {
        // Chuẩn bị
        GC.Collect();
        GC.WaitForPendingFinalizers();
        GC.Collect();

#if UNITY_2020_2_OR_NEWER
        long allocBefore = _gcAllocRecorder.Valid ? _gcAllocRecorder.LastValue : 0;
#endif
        long memBefore = GC.GetTotalMemory(false);

        var sw = Stopwatch.StartNew();
        for (int r = 0; r < InnerLoops; r++) action();
        sw.Stop();

        long memAfter = GC.GetTotalMemory(false);
        long allocBytes = memAfter - memBefore;

#if UNITY_2020_2_OR_NEWER
        if (_gcAllocRecorder.Valid)
        {
            // GC Allocated In Frame đo alloc phát sinh trong frame này (độc lập với getTotalMemory).
            long frameAlloc = Math.Max(0, _gcAllocRecorder.LastValue - allocBefore);
            // Ưu tiên dùng số của ProfilerRecorder khi có Development Build
            allocBytes = frameAlloc;
        }
#endif
        return (sw.Elapsed.TotalMilliseconds, allocBytes);
    }
}