using System.Collections.Generic;
using System.Linq;

namespace MinecraftClone.Core;

// Source - https://stackoverflow.com/a/20679895
// Posted by craftworkgames, modified by community. See post 'Timeline' for change history
// Retrieved 2026-06-21, License - CC BY-SA 4.0

public class FrameCounter
{
    public long TotalFrames { get; private set; }
    public float TotalSeconds { get; private set; }
    public float AverageFramesPerSecond { get; private set; }
    public float CurrentFramesPerSecond { get; private set; }

    public const int MaximumSamples = 100;

    private Queue<float> _sampleBuffer = new();

    public void Update(float deltaTime)
    {
        CurrentFramesPerSecond = 1.0f / deltaTime;

        _sampleBuffer.Enqueue(CurrentFramesPerSecond);

        if (_sampleBuffer.Count > MaximumSamples)
        {
            _sampleBuffer.Dequeue();
            AverageFramesPerSecond = _sampleBuffer.Average(i => i);
        }
        else
        {
            AverageFramesPerSecond = CurrentFramesPerSecond;
        }

        TotalFrames++;
        TotalSeconds += deltaTime;
    }
}
