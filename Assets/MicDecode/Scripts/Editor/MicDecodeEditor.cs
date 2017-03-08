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

using System;
using System.Collections;
using UnityEditor;
using UnityEngine;

namespace JulianSchoenbaechler.MicDecode
{
	[CustomEditor(typeof(MicDecode))]
	public class MicDecodeEditor : Editor
	{
		#region Variables

		private SerializedProperty _inputDeviceProp;
		private SerializedProperty _microphoneSampleRateProp;
		private SerializedProperty _calculationsPerSecondProp;
		private SerializedProperty _spectrumFFTWindowProp;

		private EditorWindow _settingsWindow;

		#endregion

		#region Unity Behaviour

		/// <summary>
		/// On enable.
		/// </summary>
		private void OnEnable()
		{
			// Get serialized references
			_inputDeviceProp = serializedObject.FindProperty("_inputDevice");
			_microphoneSampleRateProp = serializedObject.FindProperty("_microphoneSampleRate");
			_calculationsPerSecondProp = serializedObject.FindProperty("_calculationsPerSecond");
			_spectrumFFTWindowProp = serializedObject.FindProperty("_spectrumFFTWindow");
		}

		/// <summary>
		/// Inspector GUI event.
		/// </summary>
		public override void OnInspectorGUI()
		{
			int index = 0;

			// Create GUI style for button
			GUIStyle settingsBtnStyle = new GUIStyle(GUI.skin.button);
			settingsBtnStyle.fontStyle = FontStyle.Bold;

			// Update serialized property values
			serializedObject.Update();


			// Settings window
			if(GUILayout.Button("Calculation Settings", settingsBtnStyle))
				OpenSettingsWindow();

			EditorGUILayout.Space();


			// Input device prop
			index = Array.IndexOf(Microphone.devices, _inputDeviceProp.stringValue);
			index = index < 0 ? 0 : index;
			
			_inputDeviceProp.stringValue = Microphone.devices[EditorGUILayout.Popup("Input Device", index, Microphone.devices)];
			EditorGUILayout.Space();


			// Mic sample rate prop
			_microphoneSampleRateProp.intValue = EditorGUILayout.IntField("Mic Sample Rate",
				(_microphoneSampleRateProp.intValue <= 0) ?
				AudioSettings.outputSampleRate :
				_microphoneSampleRateProp.intValue
			);
			EditorGUILayout.Space();


			// FFT Window prop
			_spectrumFFTWindowProp.enumValueIndex = (int)(FFTWindow)EditorGUILayout.EnumPopup("FFT Window", (FFTWindow)_spectrumFFTWindowProp.enumValueIndex);
			EditorGUILayout.Space();

			// Calculations prop
			EditorGUILayout.IntSlider(_calculationsPerSecondProp, 1, 30);
			EditorGUILayout.Space();


			// Apply property changes
			serializedObject.ApplyModifiedProperties();
		}

		#endregion

		#region Private Methods

		/// <summary>
		/// Opens the calculation settings window.
		/// </summary>
		private void OpenSettingsWindow()
		{
			_settingsWindow = EditorWindow.GetWindow<CalculationSettings>(true, "MicDecode: Calculation Settings", true);
			_settingsWindow.Show();
		}

		#endregion
	}
}
