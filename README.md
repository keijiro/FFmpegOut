FFmpegOut
=========

**FFmpegOut** is a Unity plugin that records and exports rendered frames in
Unity to a video file with using [FFmpeg] as a video encoder.

[FFmpeg]: https://ffmpeg.org/

System Requirements
-------------------

FFmpegOut only supports desktop platforms (Windows/macOS/Linux).

Supported Codecs
----------------

The following video formats are supported within FFmpegOut.

- QuickTime/ProRes 422
- MP4/H.264
- WebM/VP8

Installation
------------

The binary executable files of FFmpeg is not included in this repository
because they're too large to be included. They have to be added manually after
an installation of FFmpegOut.

Any recent versions of FFmpeg would work, but it's recommended to use
[KeatsPeeks]' static-linked executables because they're used for development
and testing.

- Windows:
  https://github.com/KeatsPeeks/ffmpeg-static/blob/master/bin/win32/x64/ffmpeg.exe

- macOS:
  https://github.com/KeatsPeeks/ffmpeg-static/blob/master/bin/darwin/x64/ffmpeg

- Linux:
  https://github.com/KeatsPeeks/ffmpeg-static/tree/master/bin/linux/x64/ffmpeg

Download these files and copy them into
`Assets/StreamingAssets/FFmpegOut/(platform name)/`.

### Additional note on macOS/Linux

The execute permission should be added to these binaries before using
(`chmod a+x ffmpeg` from command line).

[KeatsPeeks]: https://github.com/KeatsPeeks

How to Use
----------

Add the `CameraCapture` script to a camera that is going to render frames to be
exported.

The `CameraCapture` component has a few settings.

![inspector](http://i.imgur.com/WUUhTuK.png)

- The size of frames can be changed with the *Set Resolution* properties. This
  is handy for exporting videos in a specific format.
- The frame rate of exported videos is determined with the *Frame Rate*
  property rather than the actual application frame rate. It's recommended to
  use the *Allow Slow Down* property to keep the frame rate of the exported
  video stable. It fixes the delta time between frames and stops syncing with
  real time (wall-clock time).

TIPS
----

- Some codecs don't support arbitrary resolution and cause errors when
  capturing the game view in free aspect mode. It's recommended to use the Set
  Resolution property in such a case.
- FFmpeg H.264 encoder is highly optimized and has great quality/bandwidth
  balance compared to other encoders. It's recommended to use H.264 when there
  is no specific reason to choose other codecs.

Performance Considerations
--------------------------

FFmpeg is not optimized for real-time capturing. The main scope of the plugin
is to reduce rendering time when using Unity for pre-rendering. Using it in
interactive applications is not recommended.

License
-------

[MIT](LICENSE.md)

Note that ffmpeg itself is not placed under this license. For instance, the
ffmpeg binaries listed in the Installation section are released under the GPL.
It should be taken into account when distributing products with them.
