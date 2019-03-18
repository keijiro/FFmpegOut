FFmpegOut
=========

![gif](http://i.imgur.com/bkQlFxX.gif)

**FFmpegOut** is a Unity plugin that allows the Unity editor and applications to
record video using [FFmpeg] as a video encoder.

[FFmpeg]: https://ffmpeg.org/

Differences between Unity Recorder
----------------------------------

First of all, note that **[Unity Recorder] would be a better choice in most
cases for the same purpose**. It's strongly recommended to check and try it out
before installing FFmpegOut.

### Unity Recorder

- Pros: Easy to use. Better UI/UX.
- Pros: Stable and robust. Officially supported by Unity.

### FFmpegOut

- Pros: Supports a wide variety of codecs.
- Cons: Non user friendly UI/UX.
- Cons: Complex legal factors (GPL/LGPL, patent risk)

In short, you should use Unity Recorder unless you need a special codec like
ProRes or lossless H.264.

[Unity Recorder]:
    https://assetstore.unity.com/packages/essentials/unity-recorder-94079

System Requirements
-------------------

- Unity 2018.3 or later
- Windows: Direct3D 11
- macOS: Metal
- Linux: Vulkan

FFmpegOut only supports desktop platforms.

FFmpegOut works not only on the legacy rendering paths (forward/deferred) but
also on the standard scriptable render pipelines (LWRP/HDRP).

Installation
------------

Download and import the following packages into your project.

- [FFmpegOut package] (MIT license)
- [FFmpegOutBinaries package] (GPL)

[FFmpegOut package]: https://github.com/keijiro/FFmpegOut/releases
[FFmpegOutBinaries package]:
    https://github.com/keijiro/FFmpegOutBinaries/releases

Camera Capture component
------------------------

The **Camera Capture component** (`CameraCapture`) is used to capture frames
rendered by an attached camera.

![inspector](https://i.imgur.com/M4fxPov.png)

It has a few properties for recording video: frame dimensions, preset and frame
rate.

### Frame Dimensions (width and height)

The dimensions of recorded video are specified with the **Width** and
**Height** properties. The size of the screen or the game view will be overridden
by these values.

### Presets

At the moment the following presets are available for use.

| Name               | Container | Description                             |
| ------------------ | --------- | --------------------------------------- |
| H.264 Default      | MP4       | **Recommended for general use.**        |
| H.264 NVIDIA       | MP4       | Highly optimized. Requires a NVIDIA GPU |
| H.264 Lossless 420 | MP4       | Recommended for pre-render use.         |
| H.264 Lossless 444 | MP4       | High quality but not widely supported.  |
| HEVC Default       | MP4       | High quality but slow.                  |
| HEVC NVIDIA        | MP4       | Highly optimized. Requires a NVIDIA GPU |
| ProRes 422         | QuickTime |                                         |
| ProRes 4444        | QuickTime | **Supports alpha channel.**             |
| VP8                | WebM      |                                         |
| VP9                | WebM      | High quality but slow.                  |
| HAP                | QuickTime |                                         |
| HAP Alpha          | QuickTime | Supports alpha channel                  |
| HAP Q              | QuickTime |                                         |

### Frame Rate

The **Frame Rate** property controls the sampling frequency of the capture
component. Note that it's independent from the application frame rate. It
drops/duplicates frames to fill gaps between the recording frame rate and the
application frame rate. To avoid frame dropping, consider using the **frame
rate controller** component (see below).

Frame Rate Controller component
-------------------------------

The **Frame Rate Controller** component is a small utility to control the
application frame rate.

![inspector](https://i.imgur.com/PYaFo38.png)

It tries controlling frame rate via [Application.targetFrameRate] and
[QualitySettings.vSyncCount]. Note that it only works in a best-effort fashion.
Although it's expected to provide a better result, it's not guaranteed to work
exactly as specified.

When the **Offline Mode** property is enabled, it explicitly controls the
application frame rate via [Time.captureFramerate]. In this mode, application
time is decoupled from wall-clock time so it's guaranteed that no frame
dropping happens. This is useful when using the capture component to output
pre-render footage.

[Application.targetFrameRate]:
    https://docs.unity3d.com/ScriptReference/Application-targetFrameRate.html
[QualitySettings.vSyncCount]:
    https://docs.unity3d.com/ScriptReference/QualitySettings-vSyncCount.html
[Time.captureFramerate]:
    https://docs.unity3d.com/ScriptReference/Time-captureFramerate.html

License
-------

[MIT](LICENSE.md)

Note that the [FFmpegOutBinaries package] is not placed under this license. 
When distributing an application with the package, it must be taken into
account that multiple licenses are involved. See the [FFmpeg License] page
for further details.

[FFmpeg License]: https://www.ffmpeg.org/legal.html
