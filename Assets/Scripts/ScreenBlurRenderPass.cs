using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;

class ScreenBlurRenderPass : ScriptableRenderPass
    {
        Material _material;
        SettingParams _params;

        static readonly int _idBlurPower   = Shader.PropertyToID("_BlurPower");
        static readonly int _grabTexID     = Shader.PropertyToID("_GrabTex");
        static readonly int _grabBlurTexID = Shader.PropertyToID("_GrabBlurTex");


        public ScreenBlurRenderPass(Material material) => _material = material;
        public void SetParams(SettingParams p) => _params = p;

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData)
        {
            var cameraData   = frameData.Get<UniversalCameraData>();
            var resourceData = frameData.Get<UniversalResourceData>();

            if (cameraData.camera.cameraType != CameraType.Game) return;
            if (_material == null) return;

            _material.SetFloat(_idBlurPower, _params.BlurValue);

            TextureHandle activeColor = resourceData.activeColorTexture;

                 var desc = renderGraph.GetTextureDesc(activeColor);
            desc.depthBufferBits = 0;
            desc.clearBuffer     = false;
            desc.msaaSamples     = MSAASamples.None;

            desc.name = "_GrabTex";
            TextureHandle grab = renderGraph.CreateTexture(desc);
            desc.name = "_BlurTex1";
            TextureHandle blur1 = renderGraph.CreateTexture(desc);
            desc.name = "_BlurTex2";
            TextureHandle blur2 = renderGraph.CreateTexture(desc);

            using (var builder = renderGraph.AddRasterRenderPass<BlitPassData>("GrabCopy", out var data))
            {
                data.source    = activeColor;
                data.material  = null;
                data.passIndex = -1;

                builder.UseTexture(activeColor, AccessFlags.Read);
                builder.SetRenderAttachment(grab, 0, AccessFlags.Write);
                builder.SetGlobalTextureAfterPass(grab, _grabTexID);
                builder.AllowPassCulling(false);

                builder.SetRenderFunc((BlitPassData d, RasterGraphContext ctx) =>
                {
                    Blitter.BlitTexture(ctx.cmd, d.source, new Vector4(1, 1, 0, 0), 0, false);
                });
            }

            using (var builder = renderGraph.AddRasterRenderPass<BlitPassData>("BlurH", out var data))
            {
                data.source    = activeColor;
                data.material  = _material;
                data.passIndex = 0;

                builder.UseTexture(activeColor, AccessFlags.Read);
                builder.SetRenderAttachment(blur1, 0, AccessFlags.Write);
                builder.AllowPassCulling(false);

                builder.SetRenderFunc((BlitPassData d, RasterGraphContext ctx) =>
                {
                    Blitter.BlitTexture(ctx.cmd, d.source, new Vector4(1, 1, 0, 0), d.material, d.passIndex);
                });
            }


            using (var builder = renderGraph.AddRasterRenderPass<BlitPassData>("BlurV", out var data))
            {
                data.source    = blur1;
                data.material  = _material;
                data.passIndex = 1;

                builder.UseTexture(blur1, AccessFlags.Read);
                builder.SetRenderAttachment(blur2, 0, AccessFlags.Write);
                builder.SetGlobalTextureAfterPass(blur2, _grabBlurTexID); 
                builder.AllowPassCulling(false);

                builder.SetRenderFunc((BlitPassData d, RasterGraphContext ctx) =>
                {
                    Blitter.BlitTexture(ctx.cmd, d.source, new Vector4(1, 1, 0, 0), d.material, d.passIndex);
                });
            }
      
            /*using (var builder = renderGraph.AddRasterRenderPass<BlitPassData>("ApplyScreen", out var data))
              {
                    data.source    = blur2;
                    data.material  = null;
                    data.passIndex = -1;

                    builder.UseTexture(blur2, AccessFlags.Read);
                    builder.SetRenderAttachment(activeColor, 0, AccessFlags.Write);
                    builder.AllowPassCulling(false);

                    builder.SetRenderFunc((BlitPassData d, RasterGraphContext ctx) =>
                    {
                        Blitter.BlitTexture(ctx.cmd, d.source, new Vector4(1, 1, 0, 0), 0, false);
                    });
               }*/
              }
    }
