using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IFileBrowserService {


    void OpenFile(string dialogTitle, Action<FileBrowserResponseReason, string> onBrowserResponse);
    void OpenFile(string dialogTitle, string extensionsFilter, Action<FileBrowserResponseReason, string> onBrowserResponse);
    void OpenFile(string dialogTitle, string extensionsFilter, string initialDirectory, Action<FileBrowserResponseReason, string> onBrowserResponse);

    void OpenMultipleFiles(string dialogTitle, Action<FileBrowserResponseReason, string[]> onBrowserResponse);
    void OpenMultipleFiles(string dialogTitle, string extensionsFilter, Action<FileBrowserResponseReason, string[]> onBrowserResponse);
    void OpenMultipleFiles(string dialogTitle, string extensionsFilter, string initialDirectory, Action<FileBrowserResponseReason, string[]> onBrowserResponse);

    void OpenFolder(string dialogDescription, Action<FileBrowserResponseReason, string> onBrowserResponse);

    void SaveFile(string dialogTitle, Action<FileBrowserResponseReason, string> onBrowserResponse);
    void SaveFile(string dialogTitle, string extensionsFilter, Action<FileBrowserResponseReason, string> onBrowserResponse);
    void SaveFile(string dialogTitle, string extensionsFilter, string initialDirectory, Action<FileBrowserResponseReason, string> onBrowserResponse);

}

public enum FileBrowserResponseReason { Unknown, FileSelected, UserCancelled }

