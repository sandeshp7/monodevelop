//
// WebSubtypeUtility.cs
//
// Author:
//       Michael Hutchinson <mhutch@xamarin.com>
//
// Copyright (c) 2014 Xamarin Inc.
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
using System.IO;
using MonoDevelop.Projects;

namespace MonoDevelop.AspNet.Projects
{
	public static class WebSubtypeUtility
	{
		public static WebSubtype DetermineWebSubtype (ProjectFile file)
		{
			var dnp = file.Project as DotNetProject;
			if (dnp != null && dnp.LanguageBinding != null && dnp.LanguageBinding.IsSourceCodeFile (file.FilePath))
				return WebSubtype.Code;
			return DetermineWebSubtype (file.Name);
		}

		public static WebSubtype DetermineWebSubtype (string fileName)
		{
			string extension = Path.GetExtension (fileName);
			if (extension == null)
				return WebSubtype.None;
			extension = extension.ToUpperInvariant ().TrimStart ('.');

			//NOTE: No way to identify WebSubtype.Code from here
			//use the ProjectFile overload for that
			switch (extension) {
			case "ASPX":
				return WebSubtype.WebForm;
			case "MASTER":
				return WebSubtype.MasterPage;
			case "ASHX":
				return WebSubtype.WebHandler;
			case "ASCX":
				return WebSubtype.WebControl;
			case "ASMX":
				return WebSubtype.WebService;
			case "ASAX":
				return WebSubtype.Global;
			case "GIF":
			case "PNG":
			case "JPG":
				return WebSubtype.WebImage;
			case "SKIN":
				return WebSubtype.WebSkin;
			case "CONFIG":
				return WebSubtype.Config;
			case "BROWSER":
				return WebSubtype.BrowserDefinition;
			case "AXD":
				return WebSubtype.Axd;
			case "SITEMAP":
				return WebSubtype.Sitemap;
			case "CSS":
				return WebSubtype.Css;
			case "XHTML":
			case "HTML":
			case "HTM":
				return WebSubtype.Html;
			case "JS":
				return WebSubtype.JavaScript;
			case "LESS":
				return WebSubtype.Less;
			case "SASS":
			case "SCSS":
				return WebSubtype.Sass;
			case "EOT":
			case "TTF":
			case "OTF":
			case "WOFF":
				return WebSubtype.Font;
			case "SVG":
				return WebSubtype.Svg;
			case "STYL":
				return WebSubtype.Stylus;
			case "CSHTML":
				return WebSubtype.Razor;
			default:
				return WebSubtype.None;
			}
		}
	}
}

