using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public interface IImageDownloadService
{
    void LoadTexture(string url, Action<Texture2D> onComplete);
}
