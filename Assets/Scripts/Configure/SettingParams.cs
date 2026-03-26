using System;
using UnityEngine;
using UnityEngine.Rendering.Universal;

[Serializable]
public class SettingParams
{
    [Range(0, 0.1f)] public float BlurNoise;
    public RenderPassEvent RenderPassEvent = RenderPassEvent.AfterRenderingTransparents;
    public int RenderPassEventOffset;
}