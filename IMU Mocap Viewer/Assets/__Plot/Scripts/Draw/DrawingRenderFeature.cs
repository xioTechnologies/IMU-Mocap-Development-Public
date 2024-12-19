using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

sealed class DrawingRenderFeature : ScriptableRendererFeature
{
    class Pass : ScriptableRenderPass
    {
        private readonly DrawingRenderFeature feature;

        public Pass(DrawingRenderFeature feature) => this.feature = feature;

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer commandBuffer = CommandBufferPool.Get(feature.profilerTag);

            ClearDepthBuffer.AddCommands(commandBuffer);

            foreach (var group in feature.groups) group.PopulateCommands(commandBuffer);
            
            context.ExecuteCommandBuffer(commandBuffer);
            
            CommandBufferPool.Release(commandBuffer);
        }
    }

    public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;

    public string profilerTag = "Draw Render Pass";

    private Pass pass;

    [SerializeField] private List<DrawingGroup> groups = new();

    public override void Create() => pass = new Pass(this);

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) => renderer.EnqueuePass(pass);
}