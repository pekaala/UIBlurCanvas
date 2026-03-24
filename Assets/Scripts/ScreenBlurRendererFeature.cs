using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Rendering.RenderGraphModule;

public class ScreenBlurRendererFeature : ScriptableRendererFeature
{
    public SettingParams Params;
    [SerializeField] Shader _shader;
    Material _material;
    ScreenBlurRenderPass _blurRenderPass;

    public override void Create()
    {
        if (_shader == null) return;
        _material = new Material(_shader);
        _blurRenderPass = new ScreenBlurRenderPass(_material);
        _blurRenderPass.renderPassEvent = Params.RenderPassEvent + Params.RenderPassEventOffset;
    }

    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        if (_material == null) return;
   if (renderingData.cameraData.cameraType == CameraType.Game ||
    renderingData.cameraData.cameraType == CameraType.SceneView)
        {
            _blurRenderPass.SetParams(Params);
            renderer.EnqueuePass(_blurRenderPass);
        }
    }

    protected override void Dispose(bool disposing)
    {
#if UNITY_EDITOR
        if (UnityEditor.EditorApplication.isPlaying)
            UnityEngine.Object.Destroy(_material);
        else
            UnityEngine.Object.DestroyImmediate(_material);
#else
        UnityEngine.Object.Destroy(_material);
#endif
    }

    
}