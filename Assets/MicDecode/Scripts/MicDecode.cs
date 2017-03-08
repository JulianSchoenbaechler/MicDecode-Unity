/*
 * Copyright 2017 - Julian Schoenbaechler
 * https://github.com/JulianSchoenbaechler/MicDecode-Unity
 * 
 * This file is part of the MicDecode project.
 * 
 * The MicDecode project is free software: you can redistribute it
 * and/or modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation, either version 3 of
 * the License, or (at your option) any later version.
 * 
 * The MicDecode project is distributed in the hope that it will be
 * useful, but WITHOUT ANY WARRANTY; without even the implied
 * warranty of MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.
 * See the GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with the MicDecode project.
 * If not, see http://www.gnu.org/licenses/.
 */

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace JulianSchoenbaechler.MicDecode
{
	[RequireComponent(typeof(AudioSource))]
	public class MicDecode : MonoBehaviour
	{
		#region Variables

		// Serialized
		[SerializeField] protected string _inputDevice = null;									// Input device
		[SerializeField] protected int _microphoneSampleRate;									// Sample rate of the mic in Hz
		[SerializeField][Range(1, 30)] protected int _calculationsPerSecond = 5;				// Number of calculations per second
		[SerializeField] protected FFTWindow _spectrumFFTWindow = FFTWindow.BlackmanHarris;		// The used FFT window

		// Private
		protected AudioSource _audioSource;
		protected float[] _outputSamples;
		protected float[] _spectrum;

		protected float _rmsVal;
		protected float _dbVal;
		protected float _pitchVal;

		#endregion

		#region Unity Behaviour

		/// <summary>
		/// Initialization
		/// </summary>
		protected void Start()
		{
			_audioSource = GetComponent<AudioSource>();

			// Init the Mic
			_audioSource.loop = true;
			_audioSource.mute = false;

			// Prepare samples and spectrum arrays
			_outputSamples = new float[MicDecodeSettings.audioSamples];
			_spectrum = new float[MicDecodeSettings.audioSamples];

			if(MicDecodeSettings.debugState != MicDecodeSettings.Debug.Off)
				Debug.Log("[MicDecode] Analog input initialized. " + _calculationsPerSecond.ToString() + " calculation(s) per second.");
		}

		/// <summary>
		/// Every frame
		/// </summary>
		protected void Update()
		{
			// Sync audiosource with mic input
			if(!_audioSource.isPlaying)
			{
				if(Microphone.GetPosition(_inputDevice) > 0)
				{
					_audioSource.Play();
				}
			}
			else
			{
				if(!Microphone.IsRecording(_inputDevice))
				{
					_audioSource.Stop();
				}
			}
		}

		#endregion

		#region Public Methods

		/// <summary>
		/// Stops the recording.
		/// </summary>
		public void StopRecording()
		{
			_audioSource.Stop();

			if(Microphone.IsRecording(_inputDevice))
			{
				Microphone.End(_inputDevice);
			}

			// Stopping Calculations
			StopCoroutine("Calculate");

			// Reset values
			_rmsVal = 0f;
			_dbVal = 0f;
			_pitchVal = 0f;

			if(MicDecodeSettings.debugState != MicDecodeSettings.Debug.Off)
				Debug.Log("[MicDecode] Stopped recording.");
		}


		/// <summary>
		/// Starts the recording.
		/// </summary>
		public void StartRecording()
		{
			StopRecording();

			_audioSource.clip = Microphone.Start(_inputDevice, true, 10, 44100);

			// Starting calculations
			StartCoroutine("Calculate");

			if(MicDecodeSettings.debugState != MicDecodeSettings.Debug.Off)
				Debug.Log("[MicDecode] Started recording from input '" + _inputDevice + "'.");
		}


		/// <summary>
		/// Calculates the RMS (root mean square) value of the input signal.
		/// </summary>
		public void CalculateRMSValue()
		{
			float sum = 0;

			// Output
			_audioSource.GetOutputData(_outputSamples, 0);

			// Add squared samples -> first part of RMS calculation
			for(int i = 0; i < MicDecodeSettings.audioSamples; i++)
				sum += _outputSamples[i] * _outputSamples[i];

			// Suare root the ratio of the squared output samples to the maximal amount of samples
			_rmsVal = Mathf.Sqrt(sum / MicDecodeSettings.audioSamples);
		}

		/// <summary>
		/// Calculates the decibel value to its reference of the input signal.
		/// </summary>
		public void CalculateDecibelValue()
		{
			CalculateRMSValue();

			// Calculate decibels
			_dbVal = 20 * Mathf.Log10(_rmsVal / MicDecodeSettings.decibelReference);

			// Not smaller than -80db
			_dbVal = _dbVal < -80f ? -80f : _dbVal;
		}

		/// <summary>
		/// Calculates the frequency of the input signal.
		/// </summary>
		public void CalculateFrequency()
		{
			float[] maxVal = new float[6];				// Maximal values of the first six spikes
			int[] maxValIterator = new int[6];			// Iterator position (and therefore frequency spectrum) of the spikes
			float waveCorrect = 0f;						// Frequency calculated by the gaps between the spikes
			int[] valIteratorGaps = new int[5];			// The distance of the gaps between the spikes
			int finalValIterator = 0;					// The chosen iterator position used for calculations

			int j = 0;									// Counting variable

			// Spectrum
			_audioSource.GetSpectrumData(_spectrum, 0, _spectrumFFTWindow);

			// Iterate through spectrum
			for(int i = 0; i < MicDecodeSettings.audioSamples;i++)
			{
				if((MicDecodeSettings.debugState == MicDecodeSettings.Debug.Equalizer) ||
					(MicDecodeSettings.debugState == MicDecodeSettings.Debug.All))
					Debug.DrawLine(new Vector3(i, 0, 0 ), new Vector3( i, _spectrum[i] * 10000f, 0 ), Color.cyan);
				
				// Threshold reached, begin decoding
				if(_spectrum[i] > MicDecodeSettings.decodeThreshold)
				{
					// Search for spikes
					// Get maximal values
					if(maxVal[j] < _spectrum[i])
					{
						maxVal[j] = _spectrum[i];	// Save value
						maxValIterator[j] = i;		// Save iterator
					}
					else if(i > 3)
					{
						// Take the last four spectrum values to
						if(_spectrum[i - 4] < _spectrum[i])
						{
							j++;		// Count spikes

							// Take only six spikes
							if(j >= 6)
								i = MicDecodeSettings.audioSamples; // Break
						}
					}
				}
			}

			// Reset counting variable
			j = 0;

			// Calculate space (gap length) between the spikes
			for(int i = 0;i < 5;i++)
			{
				// Minimal length between spikes must be greater than 3
				valIteratorGaps[i] = (maxValIterator[i + 1] - maxValIterator[i]) > 3 ? maxValIterator[i + 1] - maxValIterator[i] : MicDecodeSettings.audioSamples;
			}

			// Get the smallest gap length for calculations
			// This will mostly cut out all double or tripple octave frequencies
			finalValIterator = Mathf.Min(valIteratorGaps);

			// Count how many gaps of approximately the same length occured
			for(int i = 0;i < 5;i++)
			{
				if((valIteratorGaps[i] <= finalValIterator + 1) &&
					(valIteratorGaps[i] >= finalValIterator - 1) &&
					(finalValIterator != MicDecodeSettings.audioSamples))
				{
					j++;
				}
			}

			// Only calculate if there are 3 or more spikes with the same gap length in between
			// This will guarantee that we are dealing with octaves only and getting the exact note
			finalValIterator = j >= 2 ? finalValIterator : 0;

			// Calculation with multiple spikes
			if(finalValIterator > 3)
			{
				// Calculate frequency based on the gap length (inaccurate)
				waveCorrect = (_microphoneSampleRate / (float)MicDecodeSettings.audioSamples) * finalValIterator;

				for(int i = 0; i < 6;i++) {

					// Calculate frequency for every spike
					_pitchVal = maxValIterator[i] * (_microphoneSampleRate / 2) / MicDecodeSettings.audioSamples;

					// Mask out the correct spike by comparing spike frequency with the frequency based on the gap length
					if(Mathf.Abs(_pitchVal - waveCorrect) <= ((float)MicDecodeSettings.audioSamples / 50f))
					{
						i = 6;	// Break
						// No need for further calculations
					}
				}
			}
			// Calculation with one single spike
			else if(Mathf.Max(maxValIterator) > 3)
			{
				// Calculate frequency for this spike
				_pitchVal = Mathf.Max(maxValIterator) * (_microphoneSampleRate / 2) / (float)MicDecodeSettings.audioSamples;
			}
		}

		#endregion

		#region Calculation Process (used as coroutine)

		/// <summary>
		/// Run all calculations. Used as coroutine to prevent framedrops.
		/// </summary>
		protected IEnumerator Calculate()
		{
			WaitForSeconds delay = new WaitForSeconds(1f / (float)_calculationsPerSecond);

			while(true)
			{
				// Do calculations
				CalculateDecibelValue();
				CalculateFrequency();

				if((MicDecodeSettings.debugState == MicDecodeSettings.Debug.Calculations) ||
					(MicDecodeSettings.debugState == MicDecodeSettings.Debug.All))
					Debug.Log("[MicDecode] Calculations finished. RMS: " + _rmsVal + " / " + _dbVal + "dB. Frequency: " + _pitchVal + "Hz.");

				yield return delay;
			}
		}

		#endregion

		#region Properties

		/// <summary>
		/// Gets or sets the spectrum FFT window.
		/// </summary>
		/// <value>The spectrum FFT window.</value>
		public FFTWindow SpectrumFFTWindow
		{
			get { return _spectrumFFTWindow; }
			set { _spectrumFFTWindow = value; }
		}

		/// <summary>
		/// Gets a value indicating whether this instance is recording.
		/// </summary>
		/// <value><c>true</c> if this instance is recording; otherwise, <c>false</c>.</value>
		public bool IsRecording
		{
			get
			{
				if(Microphone.IsRecording(_inputDevice))
					return true;
				else
					return false;
			}
		}

		/// <summary>
		/// Gets the RMS (root mean square) of the input signal.
		/// </summary>
		/// <value>The RMS.</value>
		public float VolumeRMS
		{
			get { return _rmsVal; }
		}

		/// <summary>
		/// Gets the decibel value of the input signal.
		/// </summary>
		/// <value>The decibel value.</value>
		public float VolumeDecibel
		{
			get { return _dbVal; }
		}

		/// <summary>
		/// Gets the frequency of the signal.
		/// </summary>
		/// <value>The frequency in Hz.</value>
		public float Frequency
		{
			get { return _pitchVal; }
		}

		#endregion
	}
}
