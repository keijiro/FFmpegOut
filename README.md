FFmpegOut
=========

**FFmpegOut** is a Unity plugin that records and exports rendered frames in
Unity to a video file with using [FFmpeg] as a video encoder.

[FFmpeg]: https://ffmpeg.org/

System Requirements
-------------------

FFmpegOut only supports desktop platforms (Windows/macOS/Linux).

Supported codecs
----------------

The following video formats are supported within FFmpegOut.

- QuickTime/ProRes 422
- MP4/H.264
- WebM/VP8

Installation
------------

The binary executable files of FFmpeg (ffmpeg.exe) is not included in this
repository nor the package because it's too large to be included. It has to be
added manually after installation of FFmpegOut.

Any recent versions of FFmpeg would work, but I recommend using the following
static-linked executables because I'm using this for testing.

- Windows:
  https://github.com/KeatsPeeks/ffmpeg-static/blob/master/bin/win32/x64/ffmpeg.exe

- macOS:
  https://github.com/KeatsPeeks/ffmpeg-static/blob/master/bin/darwin/x64/ffmpeg

- Linux:
  https://github.com/KeatsPeeks/ffmpeg-static/tree/master/bin/linux/x64/ffmpeg

Download one of these files above and copy it into
`Assets/StreamingAssets/FFmpegOut/(platform name)/`.

