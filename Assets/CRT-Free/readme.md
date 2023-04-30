# CRT Effect

This is an asset for Unity's built-in render pipeline. It is a screen effect that runs on a `Camera`, so it will effect
everything rendered by the target `Camera`. 
The effect supports the following features

1. Curved monitor 
2. Scanline colorization
3. Color crunching by color channel
4. Color dithering (16 and 64)
5. Greyscale
6. Vignette
7. Monitor glow
8. Monitor border
9. Animitable properties 

## Using the Sample

There is a sample scene that will demonstrate some of the features.

1. Load the scene at `Assets/CRT-Free/Scenes/Sample-CRT.unity`
2. Observe that the `Main Camera` GameObject has a `CRTCameraBehaviour` component. This is the critical component for the effect. 
3. Observe that `Demo Object` GameObject has a `CRTDemoBehaviour` component. This script is used in the demo scene, and illustrates how to interact with the effect in runtime. 

## Getting Started

To setup the CRT effect in a new scene, 
1. Add the `CRTCameraBehaviour` component directly to the `Camera` that you want to effect.
    - On the new `CRTCameraBehaviour` component, select a value for `startConfig`. 

    - On the new `CRTCameraBehaviour` component, select a value for `crtRenderSettings`. The asset comes with a `CRTRenderSettings.asset` that you should use. This file just controls which `Material` is used. 

2. You can create a custom `CRTDataObject` by right clicking and clicking _Create/BrewedInk/CRT-DataConfig_. 

## FAQ

### How can I enable the CRT effect in SceneView?
In the `CRTCameraBehaviour` file, uncomment the `[ImageEffectAllowedInSceneView]` attribute.

### How can I disable the CRT effect?
You can disable the `CRTCameraBehaviour` component.

## Controls

When you create a `CRTDataObject`, you can control the following properties. The tooltips explain what each property does. 
```csharp
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
public ScreenDimensions monitorOutterSize = new ScreenDimensions{ height = .1f, width = .1f};

[Tooltip("Controls how zoomed in the camera is")]
[Range(0f, 2f)]
public float zoom = 1f;

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

```