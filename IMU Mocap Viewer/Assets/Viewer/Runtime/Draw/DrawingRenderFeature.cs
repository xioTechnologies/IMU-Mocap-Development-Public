using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

namespace Viewer.Runtime.Draw
{
    sealed class DrawingRenderFeature : ScriptableRendererFeature
    {
        class PassData
        {
            public List<DrawingGroup> Groups;
        }

        class Pass : ScriptableRenderPass
        {
            private readonly DrawingRenderFeature feature;

            public Pass(DrawingRenderFeature feature) => this.feature = feature;

            // We have to include this because, in classic Unity style the made the old API obsolete before finishing the new one
            // When the Render Graph API is ready for the big time, enable in project settings and delete the is method 
#pragma warning disable CS0672 // Member overrides obsolete member
            public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData) 
#pragma warning restore CS0672 // Member overrides obsolete member
            { 
                CommandBuffer commandBuffer = CommandBufferPool.Get(feature.passName);

                ClearDepthBuffer.AddCommands(commandBuffer);

                foreach (var group in feature.groups) group.PopulateCommands(commandBuffer);
                
                context.ExecuteCommandBuffer(commandBuffer);
                
                CommandBufferPool.Release(commandBuffer);
            }

            public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
            {
                using var builder = renderGraph.AddRenderPass<PassData>(feature.passName, out var passData);
                
                passData.Groups = feature.groups;
                
                builder.UseColorBuffer(renderGraph.ImportBackbuffer(0), 0);
                
                builder.SetRenderFunc((PassData data, RenderGraphContext context) => ExecutePass(data, context));
            }
        }

        static void ExecutePass(PassData data, RenderGraphContext context)
        {
            CommandBuffer commandBuffer = context.cmd;

            ClearDepthBuffer.AddCommands(commandBuffer);

            foreach (var group in data.Groups) group.PopulateCommands(commandBuffer);
        }
        
        [SerializeField] private RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingOpaques;

        [SerializeField] private string passName = "Draw Render Pass";

        private Pass pass;

        [SerializeField] private List<DrawingGroup> groups = new();

        public override void Create()
        {
            pass = new Pass(this)
            {
                renderPassEvent = renderPassEvent
            };
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (groups == null || groups.Count == 0)
                return;
                
            renderer.EnqueuePass(pass);
        }

        protected override void Dispose(bool disposing)
        {
            pass = null;
        }
    }
}