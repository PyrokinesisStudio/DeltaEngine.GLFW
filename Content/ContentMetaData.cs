﻿using System;
using System.Collections.Generic;
using DeltaEngine.Extensions;

namespace DeltaEngine.Content
{
	/// <summary>
	/// Content files get this meta data enties generated by the ContentService or DiskContentLoader.
	/// </summary>
	public sealed class ContentMetaData
	{
		public string Name { get; set; }
		public ContentType Type { get; set; }
		public DateTime LastTimeUpdated { get; set; }
		public string Language { get; set; }
		public string LocalFilePath { get; set; }
		public int PlatformFileId { get; set; }
		public int FileSize { get; set; }

		public T Get<T>(string attributeName, T defaultValue)
		{
			return Values.ContainsKey(attributeName) ? Values[attributeName].Convert<T>() : defaultValue;
		}

		public readonly Dictionary<string, string> Values = new Dictionary<string, string>();
	}
}