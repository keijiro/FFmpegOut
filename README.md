FFmpegOut
=========

![gif](http://i.imgur.com/bkQlFxX.gif)

**FFmpegOut** is a Unity plugin that records and exports rendered frames in
Unity to a video file by using [FFmpeg] as a video encoder.

The main scope of FFmpegOut is to reduce rendering time when using Unity for
pre-rendering. It greatly reduces the amount of file I/O compared to exporting
raw image sequences, so that it can be an effective solution when the bandwidth
is the most significant bottleneck. On the other hand, FFmpegOut is not
optimized for real-time capturing. It's not strongly recommended to use it in
an interactive application.

[FFmpeg]: https://ffmpeg.org/

System Requirements
-------------------

- Unity 5.6.0 or later

FFmpegOut only supports desktop platforms (Windows/macOS/Linux).

How to Use
----------

Download the latest package from the [Releases] page and import it to the
project. Then, add the **CameraCapture** component (Add Component -> FFmpegOut
-> Camera Capture) to a camera that is used to render frames to be exported.

The CameraCapture component has a few settings.

![inspector](https://i.imgur.com/JdBquo4.png)

- The size of frames can be changed with the **Set Resolution** properties.
  This is handy for exporting videos in a specific format.
- The frame rate of an exported video is determined from the **Frame Rate** 
  value rather than the actual application frame rate. The **Allow Slow Down**
  switch is useful to keep the frame rate of the exported video stable. It
  fixes the delta time between frames and stops the application from syncing
  with real time (wall-clock time). In most cases it makes the application runs
  slower than real time speed.

[Releases]: https://github.com/keijiro/FFmpegOut/releases

Encoding Presets
----------------

#### H.264 Default (MP4)

Highly optimized encoder with a moderate quality and a mid-level bit rate.
**Recemmended for general use.**

#### H.264 Lossless 420 (MP4)

Not actually lossless but the quality is high enough for most use cases.
**Recommended for pre-rendering use.**

#### H.264 Lossless 444 (MP4)

The highest quality preset. Most software can't decode videos encoded with this
preset (e.g. Premiere crashes when importing them).

#### ProRes 422 (QuickTime)

ProRes is an intra-frame codec that is gradually phased out but still widely
used in video editing. The ProRes codec used in FFmpeg is not aggressively
optimized so that it tends to be slower than other codecs.

#### ProRes 4444 (QuickTime)

**Only this preset supports alpha channel.** Use this when you needs alpha
channel for composition in editing software (Premiere, After Effects, etc.).

#### VP8 (WebM)

Very low bit rate encoding, optimized for web browser use.

Troubleshooting: Win32Exception
-------------------------------

Sometimes on macOS and Linux, FFmpegOut throws Win32Exception and fails to
start recording. This is caused because ffmpeg binary doesn't have the
executable permission. To solve the problem, the executable permission should
be given to ffmpeg (e.g. `chmod a+x ffmpeg`).

TIPS
----

- Some codecs don't support arbitrary resolutions and cause errors when
  capturing the game view in free aspect mode. It's recommended to use the Set
  Resolution properties in such a case.

Using This Repository
---------------------

The binary executables of FFmpeg are not included in this repository because
they're too large to be included. They have to be added manually after cloning
the repository.

Any recent versions of FFmpeg would work, but it's recommended to use
[KeatsPeeks]' static-linked executables because they're used for testing.

- Windows:
  https://github.com/KeatsPeeks/ffmpeg-static/blob/master/bin/win32/x64/ffmpeg.exe

- macOS:
  https://github.com/KeatsPeeks/ffmpeg-static/blob/master/bin/darwin/x64/ffmpeg

- Linux:
  https://github.com/KeatsPeeks/ffmpeg-static/tree/master/bin/linux/x64/ffmpeg

Download these files and copy them into
`Assets/StreamingAssets/FFmpegOut/(platform name)/`.

[KeatsPeeks]: https://github.com/KeatsPeeks/ffmpeg-static

License
-------

[MIT](LICENSE.md)

Note that FFmpeg itself is not placed under this license. For instance, the
FFmpeg binaries included in the release packages are depending on some GPL
libraries. This should be taken into account when distributing them within a
product. See the [FFmpeg License] page for futher details.

[FFmpeg License]: https://www.ffmpeg.org/legal.html
