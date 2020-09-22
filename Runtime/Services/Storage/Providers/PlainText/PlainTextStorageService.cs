using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

namespace Pixelatto.Services
{

	public class PlainTextStorageService : IStorageService
	{

		string path = "";

		bool isPathDefined => !string.IsNullOrEmpty(path) && File.Exists(path);

		public void Start()
		{

		}

		public void Setup()
		{

		}

		public void Store(string data)
		{
			if (isPathDefined)
			{
				File.WriteAllText(path, data);
			}
		}

		public void Locate(string path)
		{
			if (!File.Exists(path))
			{
				File.Create(path);
			}
			this.path = path;
		}

		public string Retrieve()
		{
			if (isPathDefined)
			{
				return File.ReadAllText(path);
			}
			else
			{
				return "";
			}
		}
	}
}
