using System;
using System.Diagnostics;
using UnityEngine;
using System.IO;
using System.Collections.Generic;

namespace Atto.Services
{
	public class WindowsFileBrowserService : IFileBrowserService
	{
		private static readonly string EXECUTABLE_PATH = Application.dataPath + "/StreamingAssets/FileBrowser.exe";
		private static readonly string ACTION_PARAMETER = "/a";
		private static readonly string ACTION_SAVE = "save";
		private static readonly string ACTION_OPEN_FOLDER = "openFolder";
		private static readonly string DIALOG_TITLE_PARAMETER = "/t";
		private static readonly string MULTIPLE_SELECTION_PARAMETER = "/m";
		private static readonly string EXTENSIONS_FILTER_PARAMETER = "/f";
		private static readonly string INITIAL_DIRECTORY_PARAMETER = "/i";

		public void OpenFile(string dialogTitle, Action<FileBrowserResponseReason, string> onBrowserResponse)
		{
			var arguments = string.Format
			(
				"{0} \"{1}\"",
				DIALOG_TITLE_PARAMETER, dialogTitle
			);

			OpenFileBrowserHandler(arguments, (StreamReader output) =>
			{
				SingleFileOutputProcessor(output, onBrowserResponse);
			});
		}

		public void OpenFile(string dialogTitle, string extensionsFilter, Action<FileBrowserResponseReason, string> onBrowserResponse)
		{
			var arguments = string.Format
			(
				"{0} \"{1}\" {2} \"{3}\"",
				DIALOG_TITLE_PARAMETER, dialogTitle,
				EXTENSIONS_FILTER_PARAMETER, extensionsFilter
			);

			OpenFileBrowserHandler(arguments, (StreamReader output) =>
			{
				SingleFileOutputProcessor(output, onBrowserResponse);
			});
		}

		public void OpenFile(string dialogTitle, string extensionsFilter, string initialDirectory, Action<FileBrowserResponseReason, string> onBrowserResponse)
		{
			var arguments = string.Format
			(
				"{0} \"{1}\" {2} \"{3}\" {4} \"{5}\"",
				DIALOG_TITLE_PARAMETER, dialogTitle,
				EXTENSIONS_FILTER_PARAMETER, extensionsFilter,
				INITIAL_DIRECTORY_PARAMETER, initialDirectory
			);

			OpenFileBrowserHandler(arguments, (StreamReader output) =>
			{
				SingleFileOutputProcessor(output, onBrowserResponse);
			});
		}

		public void OpenMultipleFiles(string dialogTitle, Action<FileBrowserResponseReason, string[]> onBrowserResponse)
		{
			var arguments = string.Format
			(
				"{0} \"{1}\" {2}",
				DIALOG_TITLE_PARAMETER, dialogTitle,
				MULTIPLE_SELECTION_PARAMETER
			);

			OpenFileBrowserHandler(arguments, (StreamReader output) =>
			{
				MultipleFilesOutputProcessor(output, onBrowserResponse);
			});
		}

		public void OpenMultipleFiles(string dialogTitle, string extensionsFilter, Action<FileBrowserResponseReason, string[]> onBrowserResponse)
		{
			var arguments = string.Format
			(
				"{0} \"{1}\" {2} \"{3}\" {4}",
				DIALOG_TITLE_PARAMETER, dialogTitle,
				EXTENSIONS_FILTER_PARAMETER, extensionsFilter,
				MULTIPLE_SELECTION_PARAMETER
			);

			OpenFileBrowserHandler(arguments, (StreamReader output) =>
			{
				MultipleFilesOutputProcessor(output, onBrowserResponse);
			});
		}

		public void OpenMultipleFiles(string dialogTitle, string extensionsFilter, string initialDirectory, Action<FileBrowserResponseReason, string[]> onBrowserResponse)
		{
			var arguments = string.Format
			(
				"{0} \"{1}\" {2} \"{3}\" {4} {5} \"{6}\"",
				DIALOG_TITLE_PARAMETER, dialogTitle,
				EXTENSIONS_FILTER_PARAMETER, extensionsFilter,
				MULTIPLE_SELECTION_PARAMETER,
				INITIAL_DIRECTORY_PARAMETER, initialDirectory
			);

			OpenFileBrowserHandler(arguments, (StreamReader output) =>
			{
				MultipleFilesOutputProcessor(output, onBrowserResponse);
			});
		}

		public void OpenFolder(string dialogDescription, Action<FileBrowserResponseReason, string> onBrowserResponse)
		{
			var arguments = string.Format
			(
				"{0} {1} {2} \"{3}\"",
				ACTION_PARAMETER, ACTION_OPEN_FOLDER,
				DIALOG_TITLE_PARAMETER, dialogDescription
			);

			OpenFileBrowserHandler(arguments, (StreamReader output) =>
			{
				SingleFileOutputProcessor(output, onBrowserResponse);
			});
		}

		public void SaveFile(string dialogTitle, Action<FileBrowserResponseReason, string> onBrowserResponse)
		{
			var arguments = string.Format
			(
				"{0} {1} {2} \"{3}\"",
				ACTION_PARAMETER, ACTION_SAVE,
				DIALOG_TITLE_PARAMETER, dialogTitle
			);

			OpenFileBrowserHandler(arguments, (StreamReader output) =>
			{
				SingleFileOutputProcessor(output, onBrowserResponse);
			});
		}

		public void SaveFile(string dialogTitle, string extensionsFilter, Action<FileBrowserResponseReason, string> onBrowserResponse)
		{
			var arguments = string.Format
			(
				"{0} {1} {2} \"{3}\" {4} \"{5}\"",
				ACTION_PARAMETER, ACTION_SAVE,
				DIALOG_TITLE_PARAMETER, dialogTitle,
				EXTENSIONS_FILTER_PARAMETER, extensionsFilter
			);

			OpenFileBrowserHandler(arguments, (StreamReader output) =>
			{
				SingleFileOutputProcessor(output, onBrowserResponse);
			});
		}

		public void SaveFile(string dialogTitle, string extensionsFilter, string initialDirectory, Action<FileBrowserResponseReason, string> onBrowserResponse)
		{
			var arguments = string.Format
			(
				"{0} {1} {2} \"{3}\" {4} \"{5}\" {6} \"{7}\"",
				ACTION_PARAMETER, ACTION_SAVE,
				DIALOG_TITLE_PARAMETER, dialogTitle,
				EXTENSIONS_FILTER_PARAMETER, extensionsFilter,
				INITIAL_DIRECTORY_PARAMETER, initialDirectory
			);

			OpenFileBrowserHandler(arguments, (StreamReader output) =>
			{
				SingleFileOutputProcessor(output, onBrowserResponse);
			});
		}

		private void OpenFileBrowserHandler(string arguments, Action<StreamReader> processOutput)
		{
			var startInfo = new ProcessStartInfo(EXECUTABLE_PATH);

			startInfo.Arguments = arguments;
			startInfo.UseShellExecute = false;
			startInfo.RedirectStandardOutput = true;

			var process = new Process();
			process.StartInfo = startInfo;
			process.Start();

			processOutput(process.StandardOutput);

			process.WaitForExit();
		}

		private void SingleFileOutputProcessor(StreamReader output, Action<FileBrowserResponseReason, string> onBrowserResponse)
		{
			if (onBrowserResponse != null)
			{
				string file = output.ReadLine();

				if (!string.IsNullOrEmpty(file))
				{
					onBrowserResponse(FileBrowserResponseReason.FileSelected, file);
				}
				else
				{
					onBrowserResponse(FileBrowserResponseReason.UserCancelled, null);
				}
			}
		}

		private void MultipleFilesOutputProcessor(StreamReader output, Action<FileBrowserResponseReason, string[]> onBrowserResponse)
		{
			if (onBrowserResponse != null)
			{
				var files = new List<string>();
				string newFile;

				while ((newFile = output.ReadLine()) != null)
				{
					files.Add(newFile);
				}

				if (files.Count > 0)
				{
					onBrowserResponse(FileBrowserResponseReason.FileSelected, files.ToArray());
				}
				else
				{
					onBrowserResponse(FileBrowserResponseReason.UserCancelled, null);
				}
			}
		}
	}
}
