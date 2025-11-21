using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CustomPassVolumeFeature : ScriptableRendererFeature
{
    class VolumePass : ScriptableRenderPass
    {
        private Material mat;
        private Camera cam;

        public VolumePass(Material m)
        {
            mat = m;
            renderPassEvent = RenderPassEvent.AfterRenderingPostProcessing;
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (CustomPassVolumeManager.Instance == null)
                return;

            cam = renderingData.cameraData.camera;

            var vol = CustomPassVolumeManager.Instance.GetActiveVolume(cam.transform.position);

            if (vol == null || vol.weight <= 0.001f)
                return;

            // Push volume params to shader
            mat.SetFloat("_CloseAmount", vol.closeAmount * vol.weight);
            mat.SetFloat("_Smoothness", vol.smoothness);

            // Render fullscreen
            CommandBuffer cmd = CommandBufferPool.Get("URP Custom Pass Volume");
            cmd.Blit(null, BuiltinRenderTextureType.CameraTarget, mat);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }
    }

    public Material effectMaterial;
    VolumePass pass;

    public override void Create()
    {
        if (effectMaterial != null)
            pass = new VolumePass(effectMaterial);
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        renderer.EnqueuePass(pass);
    }
}
