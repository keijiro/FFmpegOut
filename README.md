FFmpegOut
=========

![gif](http://i.imgur.com/bkQlFxX.gif)

**FFmpegOut** is a Unity plugin that records and exports rendered frames in
Unity to a video file with using [FFmpeg] as a video encoder.

The main scope of FFmpegOut is to reduce rendering time when using Unity for
pre-rendering. It greatly reduces the amount of file I/O compared to exporting
raw image sequences, thus it could be an effective solution when the bandwidth
is the most significant bottleneck. On the other hand, FFmpegOut is not
optimized for real-time capturing. It's not strongly recommended to use it in
an interactive application.

[FFmpeg]: https://ffmpeg.org/

System Requirements
-------------------

- Unity 5.6.0 or later

FFmpegOut only supports desktop platforms (Windows/macOS/Linux).

Supported Codecs
----------------

At the moment, the following video formats are supported within FFmpegOut.

- QuickTime/ProRes 422
- MP4/H.264
- WebM/VP8

How to Use
----------

Download the latest package from the [Releases] page and import it to the
project. Then, add the **CameraCapture** component (Add Component -> FFmpegOut
-> Camera Capture) to a camera that is going to render frames to be exported.

The CameraCapture component has a few settings.

![inspector](http://i.imgur.com/WUUhTuK.png)

- The size of frames can be changed with the **Set Resolution** properties.
  This is handy for exporting videos in a specific format.
- The frame rate of an exported video is determined from the **Frame Rate** 
  value rather than the actual application frame rate. The **Allow Slow Down**
  switch is useful to keep the frame rate of the exported video stable. It
  fixes the delta time between frames and stops the application from syncing
  with real time (wall-clock time). In most cases it makes the application runs
  slower than real time speed.

[Releases]: https://github.com/keijiro/FFmpegOut/releases

TIPS
----

- Some codecs don't support arbitrary resolutions and cause errors when
  capturing the game view in free aspect mode. It's recommended to use the Set
  Resolution properties in such a case.
- MP4/H.264 is the most preferable option because the FFmpeg H.264 encoder is
  highly optimized and has a great quality/bandwidth balance compared to other
  codecs. Use it when there is no specific reason to choose other codecs.

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

License
-------

[MIT](LICENSE.md)

Note that FFmpeg itself is not placed under this license. For instance, the
FFmpeg binaries included in the release packages are depending on some GPL
libraries. This should be taken into account when distributing them within a
product. See the [FFmpeg License] page for futher details.

[FFmpeg License]: https://www.ffmpeg.org/legal.html
