using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace BrewedInk.CRT
{
	[CreateAssetMenu(menuName = "BrewedInk/CRT-DataConfig")]
	public class CRTDataObject : ScriptableObject
	{
		public CRTData data;

		[HideInInspector]
		public string validationId;
		
		private void OnValidate()
		{
			validationId = Guid.NewGuid().ToString();
		}
	}

	[Serializable]
	public partial class CRTData
	{
		[Tooltip("A value of 0 means no down-sampling. Any number above 1 will down-sample the texture by the value.")]
		public int pixelationAmount;

		[Tooltip("Each channel controls the maximum amount of values for that channel. A value of 0 means infinite values.")]
		public ColorChannels maxColorChannels;

		[Tooltip("0 means no dithering. 1 means all dithering. Dithering will help shade a color crunched image to look like it has more coloring that it really does. " +
		         "This dithering uses a 4x4 bayer dithering matrix.")]
		[Range(0,1f)]
		[FormerlySerializedAs("dithering")]
		public float dithering4 = 0f;

		[Tooltip("0 means no dithering. 1 means all dithering. Dithering will help shade a color crunched image to look like it has more coloring that it really does. " +
		         "This dithering uses a 8x8 bayer dithering matrix.")]
		[Range(0,1f)]
		public float dithering8 = 0f;
		
		[Tooltip("Controls the dark vignette around the inside of the screen")]
		[Range(0,1f)]
		public float vignette = .1f;

		[Tooltip("Controls how visible the inner picture is.")]
		[Range(0f, 20f)]
		public float innerCurve = 20;
		
		[Tooltip("Controls how curved the monitor is")]
		[Range(0f, .5f)]
		public float monitorCurve = .1f;

		[Tooltip("Controls how big the inner monitor is")]
		[HideInInspector]
		public ScreenDimensions monitorInnerSize = new ScreenDimensions{ height = 0, width = 0};
		
		[Tooltip("Controls how big the inner monitor is")]
		public ScreenDimensions monitorOutterSize = new ScreenDimensions{ height = .1f, width = .1f};
		
		[Tooltip("Controls how zoomed in the camera is")]
		[Range(0f, 2f)]
		public float zoom = 1f;

		[Tooltip("Controls how round the edge of the screen is")]
		[Range(0,1f)]
		[HideInInspector]
		public float monitorRoundness = .2f; // TODO: can't figure this out

		[Tooltip("A detail texture for the monitor")]
		public Texture2D monitorTexture;

		[Tooltip("A tint color for the monitor")]
		public Color monitorColor = Color.grey;

		[Tooltip("The higher the value, the darker the inner monitor section")]
		[Range(0,1f)]
		public float innerMonitorDarkness = .6f;
		
		[Tooltip("The higher the value, the shinier the inner monitor section")] 
		[Range(0, 1f)]
		public float innerMonitorShine = .1f;

		[Tooltip("Controls the horizontal scan color lines")]
		public ColorScan colorScans = new ColorScan
		{
			greenChannelMultiplier = .1f, redBlueChannelMultiplier = .15f, sizeMultiplier = 2
		};
		
		[Range(-.1f, .1f)]
		[HideInInspector]
		public float innerMonitorShineRadius = -0.087f;
		[Range(.1f, 10f)]
		[HideInInspector]
		public float innerMonitorShineCurve = 10;

		public CRTData Clone()
		{
			return JsonUtility.FromJson<CRTData>(JsonUtility.ToJson(this));
		}

		public static CRTData Lerp(CRTData a, CRTData b, float t)
		{
			var f = b.Clone(); // by starting with B, we'll automatically step to any values that aren't transitionable

			f.zoom = Mathf.Lerp(a.zoom, b.zoom, t);
			f.dithering4 = Mathf.Lerp(a.dithering4, b.dithering4, t);
			f.dithering8 = Mathf.Lerp(a.dithering8, b.dithering8, t);
			f.vignette = Mathf.Lerp(a.vignette, b.vignette, t);
			f.innerCurve = Mathf.Lerp(a.innerCurve, b.innerCurve, t);
			f.monitorCurve = Mathf.Lerp(a.monitorCurve, b.monitorCurve, t);
			f.monitorRoundness = Mathf.Lerp(a.monitorRoundness, b.monitorRoundness, t);
			f.innerMonitorDarkness = Mathf.Lerp(a.innerMonitorDarkness, b.innerMonitorDarkness, t);
			f.innerMonitorShine = Mathf.Lerp(a.innerMonitorShine, b.innerMonitorShine, t);
			f.innerMonitorShineCurve = Mathf.Lerp(a.innerMonitorShineCurve, b.innerMonitorShineCurve, t);
			f.innerMonitorShineRadius = Mathf.Lerp(a.innerMonitorShineRadius, b.innerMonitorShineRadius, t);
			f.colorScans.sizeMultiplier = Mathf.Lerp(a.colorScans.sizeMultiplier, b.colorScans.sizeMultiplier, t);
			f.colorScans.greenChannelMultiplier = Mathf.Lerp(a.colorScans.greenChannelMultiplier, b.colorScans.greenChannelMultiplier, t);
			f.colorScans.redBlueChannelMultiplier = Mathf.Lerp(a.colorScans.redBlueChannelMultiplier, b.colorScans.redBlueChannelMultiplier, t);
			f.monitorColor = Color.Lerp(a.monitorColor, b.monitorColor, t);
			f.maxColorChannels.greyScale = Mathf.Lerp(a.maxColorChannels.greyScale, b.maxColorChannels.greyScale, t);
			f.maxColorChannels.blue = (int)Mathf.Lerp(a.maxColorChannels.blue, b.maxColorChannels.blue, t);
			f.maxColorChannels.green = (int)Mathf.Lerp(a.maxColorChannels.green, b.maxColorChannels.green, t);
			f.maxColorChannels.red = (int)Mathf.Lerp(a.maxColorChannels.red, b.maxColorChannels.red, t);
			f.pixelationAmount = (int)Mathf.Lerp(a.pixelationAmount, b.pixelationAmount, t);
			f.monitorInnerSize.height = Mathf.Lerp(a.monitorInnerSize.height, b.monitorInnerSize.height, t);
			f.monitorInnerSize.width = Mathf.Lerp(a.monitorInnerSize.width, b.monitorInnerSize.width, t);
			f.monitorOutterSize.height = Mathf.Lerp(a.monitorOutterSize.height, b.monitorOutterSize.height, t);
			f.monitorOutterSize.width = Mathf.Lerp(a.monitorOutterSize.width, b.monitorOutterSize.width, t);
			
			return f;
		}
	}
	
}
