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
		public CRTRenderSettingsObject crtRenderSettings;
		
		[Header("Runtime Data (edit with care!)")]
		public Material _runtimeMaterial;
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
		private static readonly int PropBrewedInkBayer4 = Shader.PropertyToID("_BrewedInk_Bayer4");
		private static readonly int PropBrewedInkBayer8 = Shader.PropertyToID("_BrewedInk_Bayer8");

		private static readonly float[] bayer4 = new float[]{
			0,8,2,10,
			12,4,14,6,
			3,11,1,9,
			15,7,13,5
		};
		private static readonly float[] bayer8 = new float []{
			0,32,8,40,2,34,10,42,
			48,16,56,24,50,18,58,26,
			12,44,4,36,14,46,6,38,
			60,28,52,20,62,30,54,22,
			3,35,11,43,1,33,9,41,
			51,19,59,27,49,17,57,25,
			15,47,7,39,13,45,5,37,
			63,31,55,23,61,29,53,21
		};

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
			if (_runtimeMaterial != null)
			{
				DestroyImmediate(_runtimeMaterial);
				_runtimeMaterial = null;
			}
		}

		void CreateMaterial()
		{
			if (crtRenderSettings != null && crtRenderSettings.crtMaterial != null && _runtimeMaterial == null)
			{
				_runtimeMaterial = new Material(crtRenderSettings.crtMaterial);
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
			if (_runtimeMaterial != null && data != null)
			{
				Shader.SetGlobalFloatArray(PropBrewedInkBayer4, bayer4);
				Shader.SetGlobalFloatArray(PropBrewedInkBayer8, bayer8);
				_runtimeMaterial.SetFloat(PropMaxColorsRed, data.maxColorChannels.red);
				_runtimeMaterial.SetFloat(PropMaxColorsGreen, data.maxColorChannels.green);
				_runtimeMaterial.SetFloat(PropMaxColorsBlue, data.maxColorChannels.blue);
				_runtimeMaterial.SetFloat(PropDitheringAmount, data.dithering4);
				_runtimeMaterial.SetFloat(PropDitheringAmount8, data.dithering8);
				_runtimeMaterial.SetFloat(PropVignette, data.vignette);
				_runtimeMaterial.SetFloat(PropMonitorRoundness, data.monitorRoundness);
				_runtimeMaterial.SetFloat(PropInnerDarkness, 1-data.innerMonitorDarkness);
				_runtimeMaterial.SetFloat(PropInnerGlow, data.innerMonitorShine);
				_runtimeMaterial.SetFloat(PropInnerReflectionRadius, data.innerMonitorShineRadius);
				_runtimeMaterial.SetFloat(PropInnerReflectionCurve, data.innerMonitorShineCurve);
				_runtimeMaterial.SetFloat(PropMonitorCurve, data.monitorCurve);
				_runtimeMaterial.SetFloat(PropInnerCurve, data.innerCurve);
				_runtimeMaterial.SetFloat(PropZoom, data.zoom);
				
				_runtimeMaterial.SetFloat(PropInnerSizeX, data.monitorInnerSize.width);
				_runtimeMaterial.SetFloat(PropInnerSizeY, data.monitorInnerSize.height);
				_runtimeMaterial.SetFloat(PropDesaturation,data.maxColorChannels.greyScale);
				
				
				_runtimeMaterial.SetFloat(PropOutterSizeX, data.monitorOutterSize.width);
				_runtimeMaterial.SetFloat(PropOutterSizeY, data.monitorOutterSize.height);
				_runtimeMaterial.SetVector(PropColorScans, new Vector4
				{
					x = data.colorScans.greenChannelMultiplier,
					y = data.colorScans.redBlueChannelMultiplier,
					z = data.colorScans.sizeMultiplier
				});
				_runtimeMaterial.SetTexture(PropMonitorTexture, data.monitorTexture);
				_runtimeMaterial.SetColor(PropMonitorColor, data.monitorColor);
				if (data.pixelationAmount > 1)
				{
					var downSample = Math.Min(300, data.pixelationAmount);
					var tempDesc = src.descriptor;
					tempDesc.width /= downSample;
					tempDesc.height /= downSample;
					var tempDest = RenderTexture.GetTemporary(tempDesc);
					tempDest.filterMode = FilterMode.Point;
					Graphics.Blit(src, tempDest);
					Graphics.Blit(tempDest, dest, _runtimeMaterial);
					tempDest.Release();
					return;
				}
				
				Graphics.Blit(src, dest, _runtimeMaterial);
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

		[Tooltip("A greyscale value of 1 will completely make the image grey. A value of 0 leaves the image untouched.")]
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
