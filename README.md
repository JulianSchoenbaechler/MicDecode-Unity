# MicDecode for Unity3D #
MicDecode is a very basic audio input signal decoder for Unity 3D that can detect the loudness and frequency. Feel free to use these scripts in your own projects.
**Check out the [releases tab](https://github.com/JulianSchoenbaechler/MicDecode-Unity/releases) for the newest unity-package of this project!**


## License ##
This software is released under the GNU General Public License (GPL) which can be found in the [LICENSE](https://github.com/JulianSchoenbaechler/MicDecode-Unity/blob/master/LICENSE) file


## Features ##
 - Real-time audio input analysis
 - Calculates the RMS (root mean square) of audio waves
 - Calculates decibel values with given references
 - Calculates frequencies of audio waves
 - The amount of calculations and precision are adjustable


## Basic Usage ##
Drag the *Microphone* Prefab into your scene and get its *MicDecode* component.
```cs
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// Using MicDecode
using JulianSchoenbaechler.MicDecode;

public class BasicUsage : MonoBehaviour
{
	// The MicDecode component (assign in inspector)
	public MicDecode microphone;

	// Initialization
	void Start()
	{
		microphone.StartRecording();
	}
}

```
You may want to fiddle with the settings of the component in the inspector.


## Component Documentation ##
### Methods ###
| Name                    | Function                                                                                       |
|-------------------------|------------------------------------------------------------------------------------------------|
| StartRecording()        | Starts real-time recording and analysis from selected input device.                            |
| StopRecording()         | Stops running recording.                                                                       |
| CalculateRMSValue()     | Manually calculates RMS value. The value is accessible through the *VolumeRMS* property.         |
| CalculateDecibelValue() | Manually calculates decibel value. The value is accessible through the *VolumeDecibel* property. |
| CalculateFrequency()    | Manually calculates frequency. The value is accessible through the *Frequency* property.         |

### Properties ###
| Name              | Access    | Function                                                                     |
|-------------------|-----------|------------------------------------------------------------------------------|
| SpectrumFFTWindow | get / set | Specifies the FFT window used for the spectrum analysis of the input signal. |
| IsRecording       | get       | Returns true if microphone is currently recording.                           |
| VolumeRMS         | get       | Returns the calculated RMS value.                                            |
| VolumeDecibel     | get       | Returns the calculated decibel value.                                        |
| Frequency         | get       | Returns the calculated frequency in Hz.                                      |


## Author ##
Julian Schönbächler
https://julian-s.ch/
info[at]julian-s.ch

----------
Project started in March 2017
