using Microsoft.Build.Framework;
using System.Collections.Generic;
using Xunit.Abstractions;

public class TestOutputLogger : ILogger
{
	ITestOutputHelper output;

	public TestOutputLogger(ITestOutputHelper output, LoggerVerbosity verbosity = LoggerVerbosity.Normal)
	{
		this.output = output;
		Verbosity = verbosity;
	}

	public void Initialize(IEventSource eventSource)
	{
		eventSource.MessageRaised += (sender, e) =>
		{
			var shouldLog = e.Importance == MessageImportance.High &&
				Verbosity >= LoggerVerbosity.Minimal;
			shouldLog |= e.Importance == MessageImportance.Normal &&
				Verbosity >= LoggerVerbosity.Normal;
			shouldLog |= e.Importance == MessageImportance.Low &&
				Verbosity >= LoggerVerbosity.Detailed;

			if (shouldLog)
			{
				output.WriteLine(e.Message);
				Messages.Add(e);
			}
		};

		if (Verbosity >= LoggerVerbosity.Detailed)
			eventSource.AnyEventRaised += (sender, e) => output.WriteLine(e.Message);

		eventSource.ErrorRaised += (sender, e) =>
		{
			output.WriteLine(e.Message);
			Errors.Add(e);
		};

		eventSource.WarningRaised += (sender, e) =>
		{
			output.WriteLine(e.Message);
			Warnings.Add(e);
		};
	}

	public string Parameters { get; set; }

	public void Shutdown() { }

	public LoggerVerbosity Verbosity { get; set; }

	public IList<BuildMessageEventArgs> Messages { get; } = new List<BuildMessageEventArgs>();

	public IList<BuildWarningEventArgs> Warnings { get; } = new List<BuildWarningEventArgs>();

	public IList<BuildErrorEventArgs> Errors { get; } = new List<BuildErrorEventArgs>();
}