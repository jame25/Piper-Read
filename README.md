<p align="center"><img width="128" alt="icon" src="https://github.com/jame25/Piper-Read/assets/13631646/eed14486-eac3-4ed9-8e8e-871be32988bc"></p>

Piper Read is a small GUI utility for Windows, that utilizes [Piper](https://github.com/rhasspy/piper). It will read aloud the contents of the input window.

## Features:
* Read content aloud
* Many voices to choose from
* Change Piper TTS voice model
* Control Piper TTS speech rate
* Pause / Resume / Stop
* Prevent keywords being read (ignore.dict)
* Prevent sentences with keywords being read (banned.dict)
* Open text files

## Prerequisites:

[.Net 8.0 Desktop Runtime](https://dotnet.microsoft.com/en-us/download/dotnet/8.0) is required to be installed.

## Install:

- Download the latest version of Piper Read from [releases](https://github.com/jame25/Piper-Read/releases/).
- Grab the latest Windows binary for Piper from [here](https://github.com/rhasspy/piper/releases). Voice models are available [here](https://huggingface.co/rhasspy/piper-voices/tree/main).
- <b>Extract all of the above into the same directory</b>.

## Configuration:

Piper Tray should support all available Piper voice models, by default **en_US-libritts_r-medium.onnx** and .json are expected to be present in directory.

## Changing Voice Model:

Click on the voice name at the bottom of the application and select from another voice from the list. No restart necessary!

## Support:

If you find this project helpful and would like to support its development, you can buy me a coffee on Ko-Fi:

[![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/jame25)
