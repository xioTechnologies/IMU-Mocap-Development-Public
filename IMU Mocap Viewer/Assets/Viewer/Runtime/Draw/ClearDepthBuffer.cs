using UnityEngine;
using UnityEngine.Rendering;

namespace Viewer.Runtime.Draw
{
    public sealed class ClearDepthBuffer : ICommandSource
    {
        public int Order { get; set; } = 0;

        public void PopulateCommands(CommandBuffer buffer) => buffer.ClearRenderTarget(true, false, Color.clear);

        public static void AddCommands(CommandBuffer buffer) => buffer.ClearRenderTarget(true, false, Color.clear);
    }
}
