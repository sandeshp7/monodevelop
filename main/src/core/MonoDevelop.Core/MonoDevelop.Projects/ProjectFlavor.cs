//
// ProjectFlavor.cs
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
using System;
using Mono.Addins;

namespace MonoDevelop.Projects
{
	/// <summary>
	/// Projects may have multiple flavors, identified by GUIDs. Flavors extend the project with additional behaviors.
	/// </summary>
	/// <remarks>Each ProjectFlavor instance is bound to a project instance.</remarks>
	public abstract class ProjectFlavor : ProjectServiceExtension
	{
		internal void Bind (SolutionItem item)
		{
			Item = item;
		}

		/// <summary>
		/// The item to which this flavor instance is bound.
		/// </summary>
		protected SolutionItem Item { get; private set; }

		/// <summary>
		/// The flavor GUID.
		/// </summary>
		public Guid Guid { get; internal set; }

		/// <summary>
		/// Human-readable ID, may be used instead of the GUID within the MonoDevelop API.
		/// </summary>
		public string Id { get; internal set; }
	}

	[AttributeUsage (AttributeTargets.Class, AllowMultiple = false)]
	public class ProjectFlavorAttribute : CustomExtensionAttribute
	{
		public ProjectFlavorAttribute ([NodeAttribute ("guid")] string guid)
		{
			Guid = guid;
		}

		[NodeAttribute ("guid")]
		public string Guid { get; private set; }
	}
}

