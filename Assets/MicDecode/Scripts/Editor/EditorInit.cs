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
	[InitializeOnLoad]
	public class EditorInit
	{
		/// <summary>
		/// Load the value of static variables set in the editor on initialization.
		/// </summary>
		static EditorInit()
		{
			if(EditorPrefs.GetInt("audioSamples", -1) >= 0)
				MicDecodeSettings.audioSamples = EditorPrefs.GetInt("audioSamples");

			if(EditorPrefs.GetFloat("decibelReference", -1f) >= 0f)
				MicDecodeSettings.decibelReference = EditorPrefs.GetFloat("decibelReference");

			if(EditorPrefs.GetFloat("decodeThreshold", -1f) >= 0f)
				MicDecodeSettings.decodeThreshold = EditorPrefs.GetFloat("decodeThreshold");

			if(EditorPrefs.GetInt("debugState", -1) >= 0)
				MicDecodeSettings.debugState = (MicDecodeSettings.Debug)EditorPrefs.GetInt("debugState");
		}
	}
}

