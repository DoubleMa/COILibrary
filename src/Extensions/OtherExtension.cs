using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Compression;
using System.Linq;

namespace COILib.Extensions;

public static class OtherExtension
{

	/// <remarks>
	/// Source: https://stackoverflow.com/a/14795752/3893208
	/// </remarks>
	public static void ExtractToDirectory(this ZipArchive archive, string destinationDirectoryName, bool overwrite)
	{
		if (!overwrite)
		{
			archive.ExtractToDirectory(destinationDirectoryName);
			return;
		}
		DirectoryInfo di = Directory.CreateDirectory(destinationDirectoryName);
		string destinationDirectoryFullPath = di.FullName;
		foreach (ZipArchiveEntry file in archive.Entries)
		{
			string completeFileName = Path.GetFullPath(Path.Combine(destinationDirectoryFullPath, file.FullName));
			if (!completeFileName.StartsWith(destinationDirectoryFullPath, StringComparison.OrdinalIgnoreCase))
			{
				throw new IOException("Trying to extract file outside of destination directory. See this link for more info: https://snyk.io/research/zip-slip-vulnerability");
			}

			if (file.Name == "")
			{
				Directory.CreateDirectory(Path.GetDirectoryName(completeFileName) ?? string.Empty);
				continue;
			}
			file.ExtractToFile(completeFileName, true);
		}
	}

	public static string[] GetFirstLevelDirectories(this ZipArchive archive)
	{
		var directories = new HashSet<string>();
		foreach (ZipArchiveEntry entry in archive.Entries)
		{
			string[] parts = entry.FullName.Split('/');

			if (parts.Length > 1)
			{
				directories.Add(parts[0]);
			}
		}
		return directories.ToArray();
	}
}