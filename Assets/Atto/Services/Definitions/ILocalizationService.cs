using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface ILocalizationService
{
    string CurrentLanguage { get; set; }
    string GetLocalizedString(string key);
    List<string> GetAvailableLanguages();
}

