// 
// FontChooserPanelWidget.cs
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
using MonoDevelop.Core;
using Gtk;
using System.Diagnostics;

namespace MonoDevelop.Ide.Fonts
{
	public partial class FontChooserPanelWidget : Gtk.Bin
	{
		Dictionary<string, string> customFonts = new Dictionary<string, string> ();

		
		public void SetFont (string fontName, string fontDescription)
		{
			customFonts [fontName] = fontDescription;
		}

		
		public string GetFont (string fontName)
		{
			if (customFonts.ContainsKey (fontName))
				return customFonts [fontName];
			switch (fontName) {
			case FontService.EditorKey:
				return FontService.EditorFontName;
			case FontService.PadKey:
				return FontService.PadFontName;
			case FontService.OutputPadKey:
				return FontService.OutputPadFontName;
			default:
				throw new InvalidOperationException ("Font " + fontName + " not found.");
			}
		}

		string GetDefaultFont (string name)
		{
			switch (name) {
			case FontService.EditorKey:
				return FontService.DefaultEditorFontName;
			case FontService.PadKey:
				return FontService.DefaultPadFontName;
			case FontService.OutputPadKey:
				return FontService.DefaultOutputPadFontName;
			default:
				throw new InvalidOperationException ("Font " + name + " not found.");
			}
		}

		public void Store ()
		{
			foreach (var val in customFonts) {
				switch (val.Key) {
				case FontService.EditorKey:
					FontService.EditorFontName = val.Value;
					break;
				case FontService.PadKey:
					FontService.PadFontName = val.Value;
					break;
				case FontService.OutputPadKey:
					FontService.OutputPadFontName = val.Value;
					break;
				default:
					throw new InvalidOperationException ("Font " + val.Key + " not found.");
				}
			}
		}

		void AddFont (string name, string displayName)
		{
			var fontNameLabel = new Label (GettextCatalog.GetString (displayName));
			fontNameLabel.Justify = Justification.Left;
			fontNameLabel.Xalign = 0;
			mainBox.PackStart (fontNameLabel, false, false, 0);
			var hBox = new HBox ();
			var setFontButton = new Button ();
			setFontButton.Label = GetFont (name);
			setFontButton.Clicked += delegate {
				var selectionDialog = new FontSelectionDialog (GettextCatalog.GetString ("Select Font")) {
					Modal = true,
					DestroyWithParent = true,
					TransientFor = this.Toplevel as Gtk.Window
				};
				try {
					selectionDialog.SetFontName (GetFont (name));
					if (MessageService.RunCustomDialog (selectionDialog) != (int)Gtk.ResponseType.Ok) {
						return;
					}
					SetFont (name, selectionDialog.FontName);
					setFontButton.Label = selectionDialog.FontName;
				} finally {
					selectionDialog.Destroy ();
				}
			};
			hBox.PackStart (setFontButton, true, true, 0);
			var setDefaultFontButton = new Button ();
			setDefaultFontButton.Label = GettextCatalog.GetString ("Set To Default");
			setDefaultFontButton.Clicked += delegate {
				SetFont (name, GetDefaultFont (name));
				setFontButton.Label = GetDefaultFont (name);
			};
			hBox.PackStart (setDefaultFontButton, false, false, 0);
			mainBox.PackStart (hBox, false, false, 0);
		}

		public FontChooserPanelWidget ()
		{
			this.Build ();

			AddFont (FontService.EditorKey, GettextCatalog.GetString ("Text Editor"));
			AddFont (FontService.PadKey, GettextCatalog.GetString ("General Pad Text"));
			AddFont (FontService.OutputPadKey, GettextCatalog.GetString ("Output Pad Contents"));

			mainBox.ShowAll ();
		}
	}
}

