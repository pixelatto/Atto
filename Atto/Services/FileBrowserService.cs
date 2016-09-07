using System;

namespace Atto.Services
{
	public enum FileBrowserResponseReason
	{
		Unknown,
		UserCancelled,
		FileSelected
	}

	public abstract class FileBrowserService
	{
		public abstract void OpenFile(string dialogTitle, Action<FileBrowserResponseReason, string> onBrowserResponse);
		public abstract void OpenFile(string dialogTitle, string extensionsFilter, Action<FileBrowserResponseReason, string> onBrowserResponse);
		public abstract void OpenFile(string dialogTitle, string extensionsFilter, string initialDirectory, Action<FileBrowserResponseReason, string> onBrowserResponse);

		public abstract void OpenMultipleFiles(string dialogTitle, Action<FileBrowserResponseReason, string[]> onBrowserResponse);
		public abstract void OpenMultipleFiles(string dialogTitle, string extensionsFilter, Action<FileBrowserResponseReason, string[]> onBrowserResponse);
		public abstract void OpenMultipleFiles(string dialogTitle, string extensionsFilter, string initialDirectory, Action<FileBrowserResponseReason, string[]> onBrowserResponse);

		public abstract void OpenFolder(string dialogDescription, Action<FileBrowserResponseReason, string> onBrowserResponse);

		public abstract void SaveFile(string dialogTitle, Action<FileBrowserResponseReason, string> onBrowserResponse);
		public abstract void SaveFile(string dialogTitle, string extensionsFilter, Action<FileBrowserResponseReason, string> onBrowserResponse);
		public abstract void SaveFile(string dialogTitle, string extensionsFilter, string initialDirectory, Action<FileBrowserResponseReason, string> onBrowserResponse);
	}
}
