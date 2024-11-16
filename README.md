Piper Read is a small GUI utility for Windows that utilizes [Piper](https://github.com/rhasspy/piper). It will read aloud the contents of the input window.

<img width="576" alt="piperread" src="https://github.com/user-attachments/assets/143661a2-ba7a-437b-8442-50070984556f">


## Features:
* Read content aloud
* Many voices to choose from
* Change Piper TTS voice model
* Control Piper TTS speech rate
* Pause / Resume / Stop
* Prevent keywords being read (ignore.dict)
* Prevent lines with keywords being read (banned.dict)
* Replace keywords with alternatives (replace.dict)
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

## Dictionary Rules:

Keywords found in the **ignore.dict** file are skipped over. 

If a keyword in the **banned.dict** file is detected, the entire line is skipped.

**replace.dict** functions as a replacement for a keyword or phrase, i.e LHC=Large Hadron Collider

## Support:

If you find this project helpful and would like to support its development, you can buy me a coffee on Ko-Fi:

[![ko-fi](https://ko-fi.com/img/githubbutton_sm.svg)](https://ko-fi.com/jame25)
