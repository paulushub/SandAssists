﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Daniel Grunwald" email="daniel@danielgrunwald.de"/>
//     <version>$Revision: 4452 $</version>
// </file>

using System;
using System.IO;
using System.Text;

using Microsoft.Build.Framework;

using ICSharpCode.SharpDevelop.Util;
using ICSharpCode.SharpDevelop.Project;

namespace VBNetBinding
{
	/// <summary>
	/// Fixes SD2-995 : Special characters not correctly encoded for languages others than English
	/// </summary>
	public class VbcEncodingFixingLogger : IMSBuildAdditionalLogger
	{
		public ILogger CreateLogger(MSBuildEngine engineWorker)
		{
			return new VbcLoggerImpl(engineWorker);
		}
		
		private class VbcLoggerImpl : ILogger
		{
			MSBuildEngine engineWorker;
			
			public VbcLoggerImpl(MSBuildEngine engineWorker)
			{
				this.engineWorker = engineWorker;
			}
			
			public LoggerVerbosity Verbosity {
				get {
					throw new NotImplementedException();
				}
				set {
					throw new NotImplementedException();
				}
			}
			
			public string Parameters {
				get {
					throw new NotImplementedException();
				}
				set {
					throw new NotImplementedException();
				}
			}
			
			IEventSource eventSource;
			
			public void Initialize(IEventSource eventSource)
			{
				this.eventSource = eventSource;
				eventSource.ErrorRaised += OnError;
				eventSource.WarningRaised += OnWarning;
			}
			
			public void Shutdown()
			{
				if (eventSource != null) {
					eventSource.ErrorRaised -= OnError;
					eventSource.WarningRaised -= OnWarning;
					eventSource = null;
				}
			}
			
			void OnError(object sender, BuildErrorEventArgs e)
			{
				FixMessage();
			}
			
			void OnWarning(object sender, BuildWarningEventArgs e)
			{
				FixMessage();
			}
			
			void FixMessage()
			{
				engineWorker.CurrentErrorOrWarning.ErrorText = FixEncoding(engineWorker.CurrentErrorOrWarning.ErrorText);
				engineWorker.CurrentErrorOrWarning.FileName = FixEncoding(engineWorker.CurrentErrorOrWarning.FileName);
			}

            static string FixEncoding(string text)
			{
                return Encoding.Default.GetString(ProcessRunner.OemEncoding.GetBytes(text));
            }
		}
	}
}
