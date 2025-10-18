using Cysharp.Threading.Tasks;
using TMPro;
using UnityEngine;
using VirtueSky.Inspector;


public class UiControl : MonoBehaviour
{
    public Benchmark benchmark;
    public ItemResult itemResultPrefab;
    public Transform content;
    public TextMeshProUGUI txtBenchSystemLinq;
    public TextMeshProUGUI txtBenchVirtueSkyLinq;

    public void UpdateStatusSystemLinq(bool status)
    {
        txtBenchSystemLinq.text = "Is Bench System.Linq: " + status;
    }

    public void UpdateStatusVirtueSkyLinq(bool status)
    {
        txtBenchVirtueSkyLinq.text = "Is Bench VirtueSky.Linq: " + status;
    }

    [Button]
    public void OnClickGetData()
    {
        Debug.Log("OnClickGetData");
        benchmark.GetData();
    }

    public void OnClickBenchmarkSystemLinq()
    {
        Debug.Log("OnClickBenchmarkSystemLinq");
        benchmark.StartBenchmarkSystemLinq();
    }

    public void OnClickBenchmarkVirtueSkyLinq()
    {
        Debug.Log("OnClickBenchmarkVirtueSkyLinq");
        benchmark.StartBenchmarkVirtueSkyLinq();
    }

    public async void OnClickShowResult()
    {
        Debug.Log("OnClickShowResult");
        await UniTask.WaitUntil(() => benchmark.isBenchmarkSystemLinqDone && benchmark.isBenchmarkVirtueSkyLinqDone);
        foreach (var benchmarkResultData in benchmark.resultDatas)
        {
            ItemResult item = Instantiate(itemResultPrefab, content);
            item.Init(benchmarkResultData);
        }
    }
}