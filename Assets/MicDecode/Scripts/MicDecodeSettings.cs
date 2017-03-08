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
	public static class MicDecodeSettings
	{
		public const string VERSION = "1.0.0";

		public static int audioSamples = 2048;						// Number of samples to resolve input signal (resolution)
		public static float decibelReference = 1f;					// The reference value for decibel calculations
		public static float decodeThreshold = 0.01f;				// Threshold for input spectrum -> start calculations only when threshold reached

		public enum Debug { Off, Equalizer, Calculations, All };	// Debug options
		public static Debug debugState = Debug.Off;
	}
}
