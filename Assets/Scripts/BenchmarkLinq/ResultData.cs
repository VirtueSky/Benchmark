using System;

[Serializable]
public class ResultData
{
    public string nameResult;
    public double bestMs;
    public double avgMs;
    public double bestAllocBytes;
    public double avgAllocBytes;

    public ResultData(string nameResult, double bestMs, double avgMs, double bestAllocBytes, double avgAllocBytes)
    {
        this.nameResult = nameResult;
        this.bestMs = bestMs;
        this.avgMs = avgMs;
        this.bestAllocBytes = bestAllocBytes;
        this.avgAllocBytes = avgAllocBytes;
    }
}