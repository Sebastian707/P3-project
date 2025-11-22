using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class EyesClosingFeature : ScriptableRendererFeature
{
    class EyesPass : ScriptableRenderPass
    {
        private Material mat;

        public EyesPass(Material material)
        {
            mat = material;
            renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (mat == null)
                return;

            CommandBuffer cmd = CommandBufferPool.Get("EyesClosingPass");
            Blitter.BlitCameraTexture(cmd, ref renderingData, mat, 0);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }

    public Material effectMaterial;
    EyesPass pass;

    public override void Create()
    {
        pass = new EyesPass(effectMaterial);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(pass);
    }
}
