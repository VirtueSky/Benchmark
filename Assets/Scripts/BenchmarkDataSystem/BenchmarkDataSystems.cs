using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using UnityEngine;
using Debug = UnityEngine.Debug;
using VirtueSky.DataStorage;
using UnityEngine.UI;

public class BenchmarkDataSystems : MonoBehaviour
{
    [Header("Benchmark Settings")] public int numberOfOperations = 1000;
    public bool runOnStart = true;

    [Tooltip("Insert a small yield every N ops to avoid hitching on device")]
    public int yieldEveryNOps = 1000;

    [Header("Results (ms)")] public long playerPrefsWriteTime;
    public long playerPrefsReadTime;
    public long playerPrefsDeleteTime;
    public long gameDataWriteTime;
    public long gameDataReadTime;
    public long gameDataDeleteTime;

    public Text playerPrefsWriteText;
    public Text playerPrefsReadText;
    public Text playerPrefsDeleteText;
    public Text gameDataWriteText;
    public Text gameDataReadText;
    public Text gameDataDeleteText;

    // (optional) unique prefixes to avoid collisions between runs
    private string _ppKeyPrefix;
    private string _gdKeyPrefix;

    private void Start()
    {
        _ppKeyPrefix = $"PP_{DateTime.Now:HHmmss}_";
        _gdKeyPrefix = $"GD_{DateTime.Now:HHmmss}_";

        if (runOnStart)
        {
            StartCoroutine(RunBenchmark());
        }
    }

    [ContextMenu("Run Benchmark")]
    public void RunBenchmarkFromContextMenu()
    {
        StartCoroutine(RunBenchmark());
    }

    private IEnumerator RunBenchmark()
    {
        Debug.Log("Starting Data Systems Benchmark...");

        // Wait a frame to ensure everything is initialized
        yield return null;

        // Clear any existing data to ensure clean benchmark
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        GameData.DeleteAll();
        GameData.Save();

        // Benchmark PlayerPrefs
        Debug.Log("Benchmarking PlayerPrefs...");
        yield return StartCoroutine(BenchmarkPlayerPrefsWrite());
        yield return StartCoroutine(BenchmarkPlayerPrefsRead());
        yield return StartCoroutine(BenchmarkPlayerPrefsDelete());

        // Clear PlayerPrefs data
        // PlayerPrefs.DeleteAll();
        // PlayerPrefs.Save();

        // Benchmark GameData
        Debug.Log("Benchmarking GameData...");
        yield return StartCoroutine(BenchmarkGameDataWrite());
        yield return StartCoroutine(BenchmarkGameDataRead());
        yield return StartCoroutine(BenchmarkGameDataDelete());

        // GameData.DeleteAll();
        // GameData.Save();

        // Display results
        DisplayResults();

        Debug.Log("Benchmark completed!");
    }

    #region PlayerPrefs Benchmark Methods

    private IEnumerator BenchmarkPlayerPrefsWrite()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();

        Stopwatch stopwatch = Stopwatch.StartNew();

        for (int i = 0; i < numberOfOperations; i++)
        {
            string key = _ppKeyPrefix + $"PlayerPrefs_Test_Key_{i}";
            PlayerPrefs.SetInt(key, i);

            if (yieldEveryNOps > 0 && ((i + 1) % yieldEveryNOps == 0))
                yield return null;
        }

        PlayerPrefs.Save();
        stopwatch.Stop();

        playerPrefsWriteTime = stopwatch.ElapsedMilliseconds;
        playerPrefsWriteText.text = $"Write Time: {playerPrefsWriteTime} ms";
        yield break;
    }

    private IEnumerator BenchmarkPlayerPrefsRead()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();

        Stopwatch stopwatch = Stopwatch.StartNew();

        for (int i = 0; i < numberOfOperations; i++)
        {
            string key = _ppKeyPrefix + $"PlayerPrefs_Test_Key_{i}";
            int value = PlayerPrefs.GetInt(key, -1);

            if (yieldEveryNOps > 0 && ((i + 1) % yieldEveryNOps == 0))
                yield return null;
        }

        stopwatch.Stop();

        playerPrefsReadTime = stopwatch.ElapsedMilliseconds;
        playerPrefsReadText.text = $"Read Time: {playerPrefsReadTime} ms";
        yield break;
    }

    private IEnumerator BenchmarkPlayerPrefsDelete()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();

        Stopwatch stopwatch = Stopwatch.StartNew();

        for (int i = 0; i < numberOfOperations; i++)
        {
            string key = _ppKeyPrefix + $"PlayerPrefs_Test_Key_{i}";
            PlayerPrefs.DeleteKey(key);

            if (yieldEveryNOps > 0 && ((i + 1) % yieldEveryNOps == 0))
                yield return null;
        }

        PlayerPrefs.Save();
        stopwatch.Stop();

        playerPrefsDeleteTime = stopwatch.ElapsedMilliseconds;
        playerPrefsDeleteText.text = $"Delete Time: {playerPrefsDeleteTime} ms";
        yield break;
    }

    #endregion

    #region GameData Benchmark Methods

    private IEnumerator BenchmarkGameDataWrite()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();

        Stopwatch stopwatch = Stopwatch.StartNew();

        for (int i = 0; i < numberOfOperations; i++)
        {
            string key = _gdKeyPrefix + $"GameData_Test_Key_{i}";
            GameData.Set(key, i);

            if (yieldEveryNOps > 0 && ((i + 1) % yieldEveryNOps == 0))
                yield return null;
        }

        //GameData.Save();
        stopwatch.Stop();

        gameDataWriteTime = stopwatch.ElapsedMilliseconds;
        gameDataWriteText.text = $"Write Time: {gameDataWriteTime} ms";
        yield break;
    }

    private IEnumerator BenchmarkGameDataRead()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();

        Stopwatch stopwatch = Stopwatch.StartNew();

        for (int i = 0; i < numberOfOperations; i++)
        {
            string key = _gdKeyPrefix + $"GameData_Test_Key_{i}";
            int value = GameData.Get<int>(key, -1);

            if (yieldEveryNOps > 0 && ((i + 1) % yieldEveryNOps == 0))
                yield return null;
        }

        stopwatch.Stop();

        gameDataReadTime = stopwatch.ElapsedMilliseconds;
        gameDataReadText.text = $"Read Time: {gameDataReadTime} ms";
        yield break;
    }

    private IEnumerator BenchmarkGameDataDelete()
    {
        GC.Collect();
        GC.WaitForPendingFinalizers();

        Stopwatch stopwatch = Stopwatch.StartNew();

        for (int i = 0; i < numberOfOperations; i++)
        {
            string key = _gdKeyPrefix + $"GameData_Test_Key_{i}";
            GameData.DeleteKey(key);

            if (yieldEveryNOps > 0 && ((i + 1) % yieldEveryNOps == 0))
                yield return null;
        }

        //GameData.Save();
        stopwatch.Stop();

        gameDataDeleteTime = stopwatch.ElapsedMilliseconds;
        gameDataDeleteText.text = $"Delete Time: {gameDataDeleteTime} ms";
        yield break;
    }

    #endregion

    #region Benchmark with Complex Data Types

    [ContextMenu("Run Complex Data Benchmark")]
    public void RunComplexDataBenchmark()
    {
        StartCoroutine(RunComplexDataBenchmarkCoroutine());
    }

    private IEnumerator RunComplexDataBenchmarkCoroutine()
    {
        Debug.Log("Starting Complex Data Benchmark...");

        // Clear any existing data to ensure clean benchmark
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        GameData.DeleteAll();
        GameData.Save();

        yield return null;

        // Create test data
        var complexData = new TestComplexData
        {
            id = 12345,
            name = "Test Player",
            score = 9999,
            level = 50,
            items = new List<string> { "Sword", "Shield", "Potion", "Armor" },
            position = new Vector3(10.5f, 20.3f, 15.7f),
            stats = new Dictionary<string, float>
            {
                { "strength", 95.5f },
                { "agility", 87.2f },
                { "intelligence", 92.8f }
            }
        };

        // Benchmark PlayerPrefs with JSON for complex data
        string json = JsonUtility.ToJson(complexData);
        Stopwatch stopwatch = Stopwatch.StartNew();
        PlayerPrefs.SetString("ComplexData_PlayerPrefs", json);
        PlayerPrefs.Save();
        stopwatch.Stop();
        long playerPrefsWriteComplex = stopwatch.ElapsedMilliseconds;

        stopwatch.Restart();
        string retrievedJson = PlayerPrefs.GetString("ComplexData_PlayerPrefs", json);
        TestComplexData retrievedPlayerPrefsData = JsonUtility.FromJson<TestComplexData>(retrievedJson);
        stopwatch.Stop();
        long playerPrefsReadComplex = stopwatch.ElapsedMilliseconds;

        // Benchmark GameData with complex data
        stopwatch.Restart();
        GameData.Set("ComplexData_GameData", complexData);
        GameData.Save();
        stopwatch.Stop();
        long gameDataWriteComplex = stopwatch.ElapsedMilliseconds;

        stopwatch.Restart();
        TestComplexData retrievedGameData = GameData.Get<TestComplexData>("ComplexData_GameData");
        stopwatch.Stop();
        long gameDataReadComplex = stopwatch.ElapsedMilliseconds;

        Debug.Log($"Complex Data - PlayerPrefs Write: {playerPrefsWriteComplex}ms, Read: {playerPrefsReadComplex}ms");
        Debug.Log($"Complex Data - GameData Write: {gameDataWriteComplex}ms, Read: {gameDataReadComplex}ms");
        Debug.Log(
            $"Complex data retrieved correctly from PlayerPrefs: {retrievedPlayerPrefsData != null && retrievedPlayerPrefsData.id == 12345}");
        Debug.Log(
            $"Complex data retrieved correctly from GameData: {retrievedGameData != null && retrievedGameData.id == 12345}");
    }

    #endregion

    private void DisplayResults()
    {
        Debug.Log("=== BENCHMARK RESULTS ===");
        Debug.Log($"Number of operations: {numberOfOperations}");
        Debug.Log("");
        Debug.Log("--- PlayerPrefs ---");
        Debug.Log(
            $"Write time: {playerPrefsWriteTime} ms  | {(numberOfOperations / Math.Max(0.0001, playerPrefsWriteTime / 1000.0)):0.0} ops/s");
        Debug.Log(
            $"Read time: {playerPrefsReadTime} ms   | {(numberOfOperations / Math.Max(0.0001, playerPrefsReadTime / 1000.0)):0.0} ops/s");
        Debug.Log(
            $"Delete time: {playerPrefsDeleteTime} ms | {(numberOfOperations / Math.Max(0.0001, playerPrefsDeleteTime / 1000.0)):0.0} ops/s");
        Debug.Log("");
        Debug.Log("--- GameData ---");
        Debug.Log(
            $"Write time: {gameDataWriteTime} ms  | {(numberOfOperations / Math.Max(0.0001, gameDataWriteTime / 1000.0)):0.0} ops/s");
        Debug.Log(
            $"Read time: {gameDataReadTime} ms   | {(numberOfOperations / Math.Max(0.0001, gameDataReadTime / 1000.0)):0.0} ops/s");
        Debug.Log(
            $"Delete time: {gameDataDeleteTime} ms | {(numberOfOperations / Math.Max(0.0001, gameDataDeleteTime / 1000.0)):0.0} ops/s");
        Debug.Log("");
        Debug.Log("--- Comparison ---");
        Debug.Log($"Write - {(playerPrefsWriteTime < gameDataWriteTime ? "PlayerPrefs faster" : "GameData faster")}");
        Debug.Log($"Read  - {(playerPrefsReadTime < gameDataReadTime ? "PlayerPrefs faster" : "GameData faster")}");
        Debug.Log($"Delete- {(playerPrefsDeleteTime < gameDataDeleteTime ? "PlayerPrefs faster" : "GameData faster")}");
    }

    [ContextMenu("Clear All Data")]
    public void ClearAllData()
    {
        PlayerPrefs.DeleteAll();
        PlayerPrefs.Save();
        GameData.DeleteAll();
        GameData.Save();
        Debug.Log("All PlayerPrefs and GameData have been cleared.");
    }
}

// Helper class for complex data testing
[Serializable]
public class TestComplexData
{
    public int id;
    public string name;
    public int score;
    public int level;
    public List<string> items;
    public Vector3 position;
    public Dictionary<string, float> stats;
}