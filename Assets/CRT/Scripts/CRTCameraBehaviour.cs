using System;
using UnityEngine;

namespace BrewedInk.CRT
{
	
	// [ImageEffectAllowedInSceneView] // ATTENTION: Uncomment this to see the effect in scene view.
	[RequireComponent(typeof(Camera))]
	[ExecuteAlways]
	public class CRTCameraBehaviour : MonoBehaviour
	{
		[Header("Configuration")] 
		public CRTDataObject startConfig;
		
		[Header("Asset References")]
		public Material CRTMaterial;
		
		[Header("Runtime Data (don't edit)")]
		public Material CRTRuntimeMaterial;
		public CRTData data;

		private string lastValidationId;

		private static readonly int PropMaxColorsRed = Shader.PropertyToID("_MaxColorsRed");
		private static readonly int PropMaxColorsGreen = Shader.PropertyToID("_MaxColorsGreen");
		private static readonly int PropMaxColorsBlue = Shader.PropertyToID("_MaxColorsBlue");
		private static readonly int PropDitheringAmount = Shader.PropertyToID("_Spread");
		private static readonly int PropDitheringAmount8 = Shader.PropertyToID("_Spread8");
		private static readonly int PropVignette = Shader.PropertyToID("_VigSize");
		private static readonly int PropMonitorRoundness = Shader.PropertyToID("_BorderOutterRound");
		private static readonly int PropMonitorTexture = Shader.PropertyToID("_BorderTex");
		private static readonly int PropMonitorColor = Shader.PropertyToID("_BorderTint");
		private static readonly int PropInnerDarkness = Shader.PropertyToID("_BorderInnerDarkerAmount");
		private static readonly int PropInnerGlow = Shader.PropertyToID("_CrtGlowAmount");
		private static readonly int PropInnerReflectionRadius = Shader.PropertyToID("_CrtReflectionRadius");
		private static readonly int PropInnerReflectionCurve = Shader.PropertyToID("_CrtReflectionCurve");
		private static readonly int PropMonitorCurve = Shader.PropertyToID("_Curvature2");
		private static readonly int PropInnerCurve = Shader.PropertyToID("_Curvature");
		private static readonly int PropZoom = Shader.PropertyToID("_BorderZoom");
		private static readonly int PropInnerSizeX = Shader.PropertyToID("_BorderInnerSizeX");
		private static readonly int PropInnerSizeY = Shader.PropertyToID("_BorderInnerSizeY");
		private static readonly int PropOutterSizeX = Shader.PropertyToID("_BorderOutterSizeX");
		private static readonly int PropOutterSizeY = Shader.PropertyToID("_BorderOutterSizeY");
		private static readonly int PropColorScans = Shader.PropertyToID("_ColorScans");
		private static readonly int PropDesaturation = Shader.PropertyToID("_Desaturation");

		
		[ContextMenu("Reset Material")]
		public void ResetMaterial()
		{
			DestroyMaterial();
			CreateMaterial();
		}
	
		private void OnDestroy()
		{
			DestroyMaterial();
		}

		void DestroyMaterial()
		{
			if (CRTRuntimeMaterial != null)
			{
				DestroyImmediate(CRTRuntimeMaterial);
				CRTRuntimeMaterial = null;
			}
		}

		void CreateMaterial()
		{
			if (CRTMaterial != null && CRTRuntimeMaterial == null)
			{
				CRTRuntimeMaterial = new Material(CRTMaterial);
			}

			if (startConfig != null)
			{
				if (!string.Equals(lastValidationId, startConfig.validationId))
				{
					lastValidationId = startConfig.validationId;
					data = startConfig.data.Clone();
				}
			}
		}

		private void Update()
		{
			CreateMaterial();
		}


		private void OnRenderImage(RenderTexture src, RenderTexture dest)
		{
			
			if (CRTRuntimeMaterial != null && data != null)
			{
				CRTRuntimeMaterial.SetFloat(PropMaxColorsRed, data.maxColorChannels.red);
				CRTRuntimeMaterial.SetFloat(PropMaxColorsGreen, data.maxColorChannels.green);
				CRTRuntimeMaterial.SetFloat(PropMaxColorsBlue, data.maxColorChannels.blue);
				CRTRuntimeMaterial.SetFloat(PropDitheringAmount, data.dithering4);
				CRTRuntimeMaterial.SetFloat(PropDitheringAmount8, data.dithering8);
				CRTRuntimeMaterial.SetFloat(PropVignette, data.vignette);
				CRTRuntimeMaterial.SetFloat(PropMonitorRoundness, data.monitorRoundness);
				CRTRuntimeMaterial.SetFloat(PropInnerDarkness, 1-data.innerMonitorDarkness);
				CRTRuntimeMaterial.SetFloat(PropInnerGlow, data.innerMonitorShine);
				CRTRuntimeMaterial.SetFloat(PropInnerReflectionRadius, data.innerMonitorShineRadius);
				CRTRuntimeMaterial.SetFloat(PropInnerReflectionCurve, data.innerMonitorShineCurve);
				CRTRuntimeMaterial.SetFloat(PropMonitorCurve, data.monitorCurve);
				CRTRuntimeMaterial.SetFloat(PropInnerCurve, data.innerCurve);
				CRTRuntimeMaterial.SetFloat(PropZoom, data.zoom);
				
				CRTRuntimeMaterial.SetFloat(PropInnerSizeX, data.monitorInnerSize.width);
				CRTRuntimeMaterial.SetFloat(PropInnerSizeY, data.monitorInnerSize.height);
				CRTRuntimeMaterial.SetFloat(PropDesaturation,data.maxColorChannels.greyScale);
				
				
				CRTRuntimeMaterial.SetFloat(PropOutterSizeX, data.monitorOutterSize.width);
				CRTRuntimeMaterial.SetFloat(PropOutterSizeY, data.monitorOutterSize.height);
				CRTRuntimeMaterial.SetVector(PropColorScans, new Vector4
				{
					x = data.colorScans.greenChannelMultiplier,
					y = data.colorScans.redBlueChannelMultiplier,
					z = data.colorScans.sizeMultiplier
				});
				CRTRuntimeMaterial.SetTexture(PropMonitorTexture, data.monitorTexture);
				CRTRuntimeMaterial.SetColor(PropMonitorColor, data.monitorColor);
				if (data.pixelationAmount > 1)
				{
					var downSample = Math.Min(300, data.pixelationAmount);
					var tempDesc = src.descriptor;
					tempDesc.width /= downSample;
					tempDesc.height /= downSample;
					var tempDest = RenderTexture.GetTemporary(tempDesc);
					tempDest.filterMode = FilterMode.Point;
					Graphics.Blit(src, tempDest);
					Graphics.Blit(tempDest, dest, CRTRuntimeMaterial);
					tempDest.Release();
					return;
				}
				
				Graphics.Blit(src, dest, CRTRuntimeMaterial);
				return;
			}
			
			Graphics.Blit(src, dest);
		}
	}
	
	[Serializable]
	public struct ColorChannels
	{
		[Range(0, 255)]
		public int red, green, blue;

		[Range(0, 1)] 
		public float greyScale;
	}

	[Serializable]
	public struct ScreenDimensions
	{
		[Range(0f,.5f)]
		public float width, height;
	}

	[Serializable]
	public struct ColorScan
	{
		[Range(-.5f,.5f)]
		public float greenChannelMultiplier;
		[Range(-.5f,.5f)]
		public float redBlueChannelMultiplier;
		[Range(0,10)]
		public float sizeMultiplier;
	}
}
