using UnityEngine;
using System.Collections;
using System;

namespace Atto.Services
{

	public class EditorLogService : LogService
	{
		override public void Debug(string message, params object[] args)
		{
			UnityEngine.Debug.Log(string.Format(message, args));
		}

		override public void Error(string message, params object[] args)
		{
			UnityEngine.Debug.LogError(string.Format(message, args));
		}

		override public void Info(string message, params object[] args)
		{
			UnityEngine.Debug.Log(string.Format(message, args));
		}

		override public void Notice(string message, params object[] args)
		{
			UnityEngine.Debug.Log(string.Format(message, args));
		}

		override public void Warning(string message, params object[] args)
		{
			UnityEngine.Debug.LogWarning(string.Format(message, args));
		}
	}
}
