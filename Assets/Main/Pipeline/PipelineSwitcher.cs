using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PipelineSwitcher : MonoBehaviour
{
    public UniversalRenderPipelineAsset highPipelineAsset;
    public UniversalRenderPipelineAsset mediumPipelineAsset;
    public UniversalRenderPipelineAsset lowPipelineAsset;

    public UniversalRendererData highPipelineRendererData;
    public UniversalRendererData mediumPipelineRendererData;
    public UniversalRendererData lowPipelineRendererData;

    void Start()
    {
        //SetPipelineBasedOnPerformance();
        GraphicsSettings.renderPipelineAsset = mediumPipelineAsset;
        GraphicsSettings.defaultRenderPipeline = mediumPipelineAsset;

        SetRenderFeatureActive(mediumPipelineRendererData, "CatchOpaqueRT", false); // 开启指定的RenderFeature
    }

    //void SetPipelineBasedOnPerformance()
    //{
    //    var devicePerformance = GetDevicePerformance();

    //    switch (devicePerformance)
    //    {
    //        case DevicePerformance.High:
    //            Debug.Log("Setting High Pipeline");
    //            GraphicsSettings.renderPipelineAsset = highPipelineAsset;
    //            break;
    //        case DevicePerformance.Medium:
    //            Debug.Log("Setting Medium Pipeline");
    //            GraphicsSettings.renderPipelineAsset = mediumPipelineAsset;
    //            break;
    //        case DevicePerformance.Low:
    //            Debug.Log("Setting Low Pipeline");
    //            GraphicsSettings.renderPipelineAsset = lowPipelineAsset;
    //            break;
    //        default:
    //            Debug.LogWarning("Unknown device performance, defaulting to Medium Pipeline");
    //            GraphicsSettings.renderPipelineAsset = mediumPipelineAsset;
    //            break;
    //    }
    //}

    void SetRenderFeatureActive(UniversalRendererData rendererData, string featureName, bool active)
    {
        foreach (var feature in rendererData.rendererFeatures)
        {
            if (feature.name == featureName)
            {
                feature.SetActive(active);
                return;
            }
        }
    }
}