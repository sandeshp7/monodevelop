// 
// FontService.cs
//  
// Author:
//       Mike Krüger <mkrueger@novell.com>
// 
// Copyright (c) 2010 Novell, Inc (http://www.novell.com)
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.

using System;
using System.Collections.Generic;
using System.Linq;
using Mono.Addins;
using MonoDevelop.Core;
using Pango;

namespace MonoDevelop.Ide.Fonts
{
	public static class FontService
	{
		internal const string EditorKey = "Editor";
		internal const string PadKey = "Pad";
		internal const string OutputPadKey = "OutputPad";

		static Properties fontProperties;

		static string editorFontName, padFontName, outputPadFontName;
		static string defaultMonospaceFontName, defaultSansFontName, defaultEditorFontName, defaultPadFontName, defaultOutputPadFontName;
		static FontDescription defaultMonospaceFont, defaultSansFont, editorFont, padFont, outputPadFont;

		static void LoadDefaults ()
		{
			if (defaultMonospaceFont != null) {
				defaultMonospaceFont.Dispose ();
				defaultSansFont.Dispose ();
			}

			#pragma warning disable 618
			defaultMonospaceFontName = DesktopService.DefaultMonospaceFont;
			defaultMonospaceFont = FontDescription.FromString (defaultMonospaceFontName);
			#pragma warning restore 618

			var label = new Gtk.Label ("");
			defaultSansFont = label.Style.FontDescription.Copy ();
			label.Destroy ();
			defaultSansFontName = defaultSansFont.ToString ();

			defaultEditorFontName = defaultMonospaceFontName;
			editorFontName = fontProperties.Get <string> (EditorKey, defaultMonospaceFontName);
			editorFont = FontDescription.FromString (editorFontName);

			if (Platform.IsMac) {
				//On OSX we are using 1 unit smaller font then _DEFAULT_SANS for default PadFont
				var tempFont = FontDescription.FromString (defaultSansFontName);
				if (tempFont.SizeIsAbsolute) {
					tempFont.AbsoluteSize = tempFont.Size - Pango.Scale.PangoScale;
				} else {
					tempFont.Size -= (int)Pango.Scale.PangoScale;
				}
				defaultPadFontName = tempFont.ToString ();
			} else {
				defaultPadFontName = defaultSansFontName;
			}
			padFontName = fontProperties.Get <string> (PadKey, defaultPadFontName);
			padFont = FontDescription.FromString (padFontName);

			defaultOutputPadFontName = defaultMonospaceFontName;
			outputPadFontName = fontProperties.Get <string> (OutputPadKey, defaultSansFontName);
			outputPadFont = FontDescription.FromString (outputPadFontName);
		}

		internal static void Initialize ()
		{
			if (fontProperties != null)
				throw new InvalidOperationException ("Already initialized");

			fontProperties = PropertyService.Get ("FontProperties", new Properties ());

			LoadDefaults ();
		}

		internal static string DefaultEditorFontName{ get { return defaultEditorFontName; } }

		internal static string DefaultPadFontName{ get { return defaultPadFontName; } }

		internal static string DefaultOutputPadFontName{ get { return defaultOutputPadFontName; } }

		public static FontDescription MonospaceFont { get { return defaultMonospaceFont; } }

		public static FontDescription SansFont { get { return defaultSansFont; } }

		public static FontDescription EditorFont { get { return editorFont; } }

		public static FontDescription PadFont { get { return padFont; } }

		public static FontDescription OutputPadFont { get { return outputPadFont; } }

		public static string MonospaceFontName { get { return defaultMonospaceFontName; } }

		public static string SansFontName { get { return defaultSansFontName; } }

		public static string EditorFontName {
			get {
				return editorFontName;
			}
			set {
				if (value == null)
					value = defaultEditorFontName;
				if (value == editorFontName)
					return;
				if (value == defaultEditorFontName) {
					fontProperties.Set ("Editor", null);
				} else {
					fontProperties.Set ("Editor", value);
				}
				editorFontName = value;
				editorFont = FontDescription.FromString (value);
				EditorFontChanged ();
			}
		}

		public static string PadFontName {
			get {
				return padFontName;
			}
			set {
				if (value == null)
					value = defaultPadFontName;
				if (value == padFontName)
					return;
				if (value == defaultPadFontName) {
					fontProperties.Set (PadKey, null);
				} else {
					fontProperties.Set (PadKey, value);
				}
				padFontName = value;
				padFont = FontDescription.FromString (value);
				PadFontChanged ();
			}
		}

		public static string OutputPadFontName {
			get {
				return outputPadFontName;
			}
			set {
				if (value == null)
					value = defaultOutputPadFontName;
				if (value == outputPadFontName)
					return;
				if (value == defaultOutputPadFontName) {
					fontProperties.Set (OutputPadKey, null);
				} else {
					fontProperties.Set (OutputPadKey, value);
				}
				outputPadFontName = value;
				outputPadFont = FontDescription.FromString (value);
				OutputPadFontChanged ();
			}
		}

		[Obsolete ("Use MonospaceFont")]
		public static FontDescription DefaultMonospaceFontDescription {
			get {
				return defaultMonospaceFont;
			}
		}

		[Obsolete]
		static FontDescription LoadFont (string name)
		{
			var fontName = FilterFontName (name);
			return FontDescription.FromString (fontName);
		}

		[Obsolete ("Use EditorFontName, PadFontName or OutputPadFontName.")]
		public static string FilterFontName (string name)
		{
			switch (name) {
			case "_DEFAULT_MONOSPACE":
				return defaultMonospaceFontName;
			case "_DEFAULT_SANS":
				return defaultSansFontName;
			default:
				return name;
			}
		}

		[Obsolete ("Use EditorFontName, PadFontName or OutputPadFontName.")]
		public static string GetUnderlyingFontName (string name)
		{
			switch (name) {
			case EditorKey:
				return editorFontName;
			case PadKey:
				return padFontName;
			case OutputPadKey:
				return outputPadFontName;
			default:
				throw new InvalidOperationException ("Font " + name + " not found.");
			}
		}

		/// <summary>
		/// Gets the font description for the provided font id
		/// </summary>
		/// <returns>
		/// The font description.
		/// </returns>
		/// <param name='name'>
		/// Identifier of the font
		/// </param>
		/// <param name='createDefaultFont'>
		/// If set to <c>false</c> and no custom font has been set, the method will return null.
		/// </param>
		[Obsolete ("Use EditorFont, PadFont or OutputPadFont.")]
		public static FontDescription GetFontDescription (string name, bool createDefaultFont = true)
		{
			switch (name) {
			case EditorKey:
				return EditorFont;
			case PadKey:
				return PadFont;
			case OutputPadKey:
				return OutputPadFont;
			default:
				LoggingService.LogError ("Font " + name + " not found.");
				return null;
			}
		}

		[Obsolete ("Use EditorFontName, PadFontName or OutputPadFontName.")]
		public static void SetFont (string name, string value)
		{
			switch (name) {
			case EditorKey:
				EditorFontName = value;
				break;
			case PadKey:
				PadFontName = value;
				break;
			case OutputPadKey:
				OutputPadFontName = value;
				break;
			default:
				throw new InvalidOperationException ("Font " + name + " not found.");
			}
		}

		public static event Action EditorFontChanged;
		public static event Action PadFontChanged;
		public static event Action OutputPadFontChanged;

		[Obsolete ("Use EditorFontChanged, PadFontChanged or OutputPadFontChanged events.")]
		public static void RegisterFontChangedCallback (string fontName, Action callback)
		{
			switch (fontName) {
			case EditorKey:
				EditorFontChanged += callback;
				break;
			case PadKey:
				PadFontChanged += callback;
				break;
			case OutputPadKey:
				OutputPadFontChanged += callback;
				break;
			default:
				LoggingService.LogError ("Font " + fontName + " not found to add callback.");
				break;
			}
		}

		[Obsolete ("Use EditorFontChanged, PadFontChanged or OutputPadFontChanged events.")]
		public static void RemoveCallback (Action callback)
		{
			EditorFontChanged -= callback;
			PadFontChanged -= callback;
			OutputPadFontChanged -= callback;
		}
	}

	public static class FontExtensions
	{
		public static FontDescription CopyModified (this FontDescription font, double? scale = null, Pango.Weight? weight = null)
		{
			font = font.Copy ();

			if (scale.HasValue)
				Scale (font, scale.Value);

			if (weight.HasValue)
				font.Weight = weight.Value;

			return font;
		}

		static void Scale (FontDescription font, double scale)
		{
			if (font.SizeIsAbsolute) {
				font.AbsoluteSize = scale * font.Size;
			} else {
				var size = font.Size;
				if (size == 0)
					size = 10;
				font.Size = (int)(Pango.Scale.PangoScale * (int)(scale * size / Pango.Scale.PangoScale));
			}
		}
	}
}
