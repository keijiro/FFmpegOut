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

The binary executable files of FFmpeg is not included in this repository nor
the package because it's too large to be included. It has to be added manually
after installation of FFmpegOut.

Any recent versions of FFmpeg would work, but I recommend using [KeatsPeeks]'
static-linked executables because I'm using them for testing.

- Windows:
  https://github.com/KeatsPeeks/ffmpeg-static/blob/master/bin/win32/x64/ffmpeg.exe

- macOS:
  https://github.com/KeatsPeeks/ffmpeg-static/blob/master/bin/darwin/x64/ffmpeg

- Linux:
  https://github.com/KeatsPeeks/ffmpeg-static/tree/master/bin/linux/x64/ffmpeg

Download these files and copy it into
`Assets/StreamingAssets/FFmpegOut/(platform name)/`.

### Additional note on macOS/Linux

The execute permission should be added to these binaries before using
(`chmod a+x ffmpeg` from command line).

[KeatsPeeks]: https://github.com/KeatsPeeks

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
That should be taken into account when using them in your product.
