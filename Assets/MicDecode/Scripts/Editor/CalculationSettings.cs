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
	public class CalculationSettings : EditorWindow
	{
		#region Variables

		private string[] sampleResolutionsVal = new string[] {
			"128",
			"256",
			"512",
			"1024",
			"2048",
			"4096",
			"8192"
		};

		private int[] sampleResolutions = new int[] {
			128,
			256,
			512,
			1024,
			2048,
			4096,
			8192
		};

		#endregion

		#region Unity Behaviour


		/// <summary>
		/// GUI event.
		/// </summary>
		private void OnGUI()
		{
			// Title style
			GUIStyle titleStyle = new GUIStyle(GUI.skin.label);
			titleStyle.alignment = TextAnchor.MiddleCenter;
			titleStyle.fontSize = 14;
			titleStyle.fontStyle = FontStyle.Bold;

			// Title
			EditorGUILayout.Space();
			GUILayout.Label("MicDecode Calculation Settings", titleStyle);

			EditorGUILayout.Separator();

			// Audio sample resolution
			EditorGUILayout.Space();
			MicDecodeSettings.audioSamples = EditorGUILayout.IntPopup("Audio Sample Resolution", MicDecodeSettings.audioSamples, sampleResolutionsVal, sampleResolutions);
			EditorPrefs.SetInt("audioSamples", MicDecodeSettings.audioSamples);

			// Decibel reference
			EditorGUILayout.Space();
			MicDecodeSettings.decibelReference = EditorGUILayout.FloatField("Decibel Reference", MicDecodeSettings.decibelReference);
			EditorPrefs.SetFloat("decibelReference", MicDecodeSettings.decibelReference);

			// Decoding threshold
			EditorGUILayout.Space();
			MicDecodeSettings.decodeThreshold = EditorGUILayout.Slider("Decoding Threshold", MicDecodeSettings.decodeThreshold, 0.001f, 0.2f);
			EditorPrefs.SetFloat("decodeThreshold", MicDecodeSettings.decodeThreshold);

			EditorGUILayout.Separator();

			// Debug state
			EditorGUILayout.Space();
			MicDecodeSettings.debugState = (MicDecodeSettings.Debug)EditorGUILayout.EnumPopup("Debug", MicDecodeSettings.debugState);
			EditorPrefs.SetInt("debugState", (int)MicDecodeSettings.debugState);

			EditorGUILayout.Separator();
			EditorGUILayout.Space();

			// Restore defaults
			if(GUILayout.Button("Restore Defaults"))
			{
				// Audio sample resolution
				MicDecodeSettings.audioSamples = 2048;
				EditorPrefs.SetInt("audioSamples", MicDecodeSettings.audioSamples);

				// Decibel reference
				MicDecodeSettings.decibelReference = 1f;
				EditorPrefs.SetFloat("decibelReference", MicDecodeSettings.decibelReference);

				// Decoding threshold
				MicDecodeSettings.decodeThreshold = 0.01f;
				EditorPrefs.SetFloat("decodeThreshold", MicDecodeSettings.decodeThreshold);

				// Debug state
				MicDecodeSettings.debugState = MicDecodeSettings.Debug.Off;
				EditorPrefs.SetInt("debugState", (int)MicDecodeSettings.debugState);
			}

			EditorGUILayout.Separator();

			// Footer
			EditorGUILayout.Space();
			GUILayout.Label("MicDecode v" + MicDecodeSettings.VERSION + " - Julian Schönbächler", EditorStyles.centeredGreyMiniLabel);
		}

		#endregion
	}
}
