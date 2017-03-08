using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Using MicDecode
using JulianSchoenbaechler.MicDecode;

public class BasicUsage : MonoBehaviour
{
	public MicDecode microphone;		// The MicDecode component (assign in inspector)
	public Text buttonText;				// The Text component of the start/stop button (assign in inspector)
	public Text displayText;			// The Text component of the displayed text (assign in inspector)

	// Initialization
	void Start()
	{
		buttonText.text = "Start";
	}

	// Every frame
	void Update()
	{
		// Display calculated values
		displayText.text =	"Volume: " + microphone.VolumeDecibel.ToString("F1") + "dB\n" +
							"Frequency: " + microphone.Frequency.ToString("F0") + "Hz\n";
	}

	// Button pressed
	public void ButtonPressed()
	{
		if(microphone.IsRecording)
		{
			// Stop recording
			microphone.StopRecording();
			buttonText.text = "Start";
		}
		else
		{
			// Start recording
			microphone.StartRecording();
			buttonText.text = "Stop";
		}
	}
}
