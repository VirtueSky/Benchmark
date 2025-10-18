using TMPro;
using UnityEngine;

public class ItemResult : MonoBehaviour
{
    public TextMeshProUGUI txtName;
    public TextMeshProUGUI txtBestMs;
    public TextMeshProUGUI txtAvgMs;
    public TextMeshProUGUI txtBestAlloc;
    public TextMeshProUGUI txtAvgAlloc;
    private ResultData resultData;

    public void Init(ResultData data)
    {
        resultData = data;
        txtName.text = resultData.nameResult;
        txtBestMs.text = "bestMs: " + resultData.bestMs.ToString("F3") + " ms";
        txtAvgMs.text = "avgMs: " + resultData.avgMs.ToString("F3") + " ms";
        txtBestAlloc.text = "bestAlloc: " + resultData.bestAllocBytes.ToString("F1") + " bytes";
        txtAvgAlloc.text = "avgAlloc: " + resultData.avgAllocBytes.ToString("F1") + " bytes";
    }
}