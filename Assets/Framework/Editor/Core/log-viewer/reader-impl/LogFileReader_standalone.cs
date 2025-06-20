using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public class LogFileReader_standalone : ILogFileReader
{
	private List<LogFileItem> logItems = new List<LogFileItem>();
	private LogFileItem processingLogItem = null;
	private bool isReadingMessage = true;
	private int stacktraceType;

	public List<LogFileItem> GetLogItems()
	{
		return logItems;
	}

	public void ProcessLine(string line)
	{
		if (processingLogItem == null)
		{
			if (string.IsNullOrEmpty(line))
			{
				processingLogItem = new LogFileItem();
			}
		}
		else
		{
			if (isReadingMessage)
			{
				var match = Regex.Match(line, "^\\(Filename: .* Line: \\d+\\)");
				if (!match.Success)
				{
					if (IsEncounterStacktrace(line))
					{
						isReadingMessage = false;
						processingLogItem.stacktraceSB.Append(line).Append('\n');
					}
					else
					{
						processingLogItem.messageSB.Append(line).Append('\n');
					}
				}
				else
				{
					var msg = processingLogItem.messageSB.ToString().Trim('\n');
					if (!string.IsNullOrEmpty(msg))
					{
						processingLogItem.EndProcess_exception(msg);
						logItems.Add(processingLogItem);
						processingLogItem = new LogFileItem();
					}
				}
			}
			else
			{
				if (string.IsNullOrEmpty(line))
				{
					processingLogItem.EndProcess();
					DetectLogType();
					logItems.Add(processingLogItem);
					processingLogItem = new LogFileItem();
					isReadingMessage = true;
				}
				else
				{
					processingLogItem.stacktraceSB.Append(line).Append('\n');
				}
			}
		}
	}

	private bool IsEncounterStacktrace(string line)
	{
		if (line.Equals("UnityEngine.DebugLogHandler:Internal_LogException(Exception, Object)"))
		{
			stacktraceType = 0;
			return true;
		}

		if (line.Equals("UnityEngine.StackTraceUtility:ExtractStackTrace ()"))
		{
			stacktraceType = 1;
			return true;
		}

		var match = Regex.Match(line, "^UnityEngine.StackTraceUtility:ExtractStackTrace \\(\\) \\(at .*\\)");
		if (match.Success)
		{
			stacktraceType = 1;
		}
		return match.Success;
	}

	private void DetectLogType()
	{
		var lLines = processingLogItem.stacktrace.Split('\n');

		var logtype = DetectLogType_1(lLines);
		if (logtype != null)
		{
			processingLogItem.logType = logtype.Value;
			return;
		}

		logtype = DetectLogType_2(lLines);
		if (logtype != null)
		{
			processingLogItem.logType = logtype.Value;
			return;
		}

		logtype = DetectLogType_3(lLines);
		if (logtype != null)
		{
			processingLogItem.logType = logtype.Value;
			return;
		}

		throw new Exception($"cannot detect logtype\nmessage={processingLogItem.message}\nstacktrace={processingLogItem.stacktrace}\n-------------------------------------------");
	}

	private LogType? DetectLogType_1(string[] lines)
	{
		var line = lines[3];
		var pattern = stacktraceType switch
		{
			0 => "^UnityEngine.Debug:(.*)\\(.*\\)",
			1 => "^UnityEngine.Debug:(.*) \\(.*\\)",
			_ => null
		};
		var match = Regex.Match(line, pattern);
		if (match.Success)
		{
			var funcName = match.Groups[1].Value;
			return funcName switch
			{
				"Log" => LogType.Log,
				"LogFormat" => LogType.Log,
				"LogWarning" => LogType.Warning,
				"LogWarningFormat" => LogType.Warning,
				"LogAssertion" => LogType.Assert,
				"LogAssertionFormat" => LogType.Assert,
				"LogError" => LogType.Error,
				"LogErrorFormat" => LogType.Error,
				"LogException" => LogType.Exception,
				_ => null,
			};
		}
		else
		{
			return null;
		}
	}

	private LogType? DetectLogType_2(string[] lines)
	{
		var line = lines[1];
		var match = Regex.Match(line, "^UnityEngine.MonoBehaviour:StartCoroutine \\(System.Collections.IEnumerator\\) \\(at .*\\)");
		if (match.Success)
		{
			return LogType.Log;
		}
		else
		{
			return null;
		}
	}

	private LogType? DetectLogType_3(string[] lines)
	{
		var line = lines[1];
		var match = Regex.Match(line, "^UnityEngine.Object:Instantiate \\(UnityEngine.Object,UnityEngine.Transform,bool\\) \\(at .*\\)");
		if (match.Success)
		{
			return LogType.Log;
		}
		else
		{
			return null;
		}
	}
}