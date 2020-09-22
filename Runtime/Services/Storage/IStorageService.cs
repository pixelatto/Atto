using Ju;
using Ju.Services;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography.X509Certificates;
using UnityEngine;

namespace Pixelatto.Services
{
	public interface IStorageService : IService
	{
		void Locate(string path);
		string Retrieve();
		void Store(string data);
	}
}
