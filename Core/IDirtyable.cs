namespace MinecraftClone.Core;

public interface IDirtyable
{
    public bool Dirty { get; }

    public void MarkDirty();
}