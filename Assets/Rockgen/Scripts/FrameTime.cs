using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace RockGen.Unity
{
[RequireComponent(typeof(Text))]
public class FrameTime : MonoBehaviour
{
    public static FrameTime Instance { get; private set; }

    readonly Dictionary<string, double> jobs      = new Dictionary<string, double>();
    readonly StringBuilder              sb        = new StringBuilder();
    readonly Stopwatch                  stopwatch = new Stopwatch();

    Text text;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(this);
    }

    void Start()
    {
        text = GetComponent<Text>();
    }

    public void StartWork(string name)
    {
        jobs.Add(name, 0);
        stopwatch.Restart();
    }

    public void EndWork(string name)
    {
        stopwatch.Stop();
        jobs[name] = stopwatch.Elapsed.TotalMilliseconds;
    }

    int MaxNameLength;

    void Update()
    {
        sb.Clear();

        AppendTime("Total", Time.deltaTime);

        MaxNameLength = jobs.Select(j => j.Key).Append("Total").Max(n => n.Length) + 2;
        foreach (var job in jobs)
        {
            AppendTime(job.Key, job.Value);
        }

        jobs.Clear();

        text.text = sb.ToString();
    }

    void AppendTime(string name, double duration)
    {
        sb.Append(name);
        sb.Append(": ");
        sb.AppendLine(duration.ToString("F2").PadLeft(MaxNameLength));
    }
}
}
