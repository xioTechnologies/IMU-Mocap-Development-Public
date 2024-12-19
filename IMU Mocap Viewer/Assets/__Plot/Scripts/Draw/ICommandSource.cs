using UnityEngine.Rendering;

public interface ICommandSource
{
    int Order { get; }

    void PopulateCommands(CommandBuffer buffer);
}