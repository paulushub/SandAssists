// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <author name="Daniel Grunwald"/>
//     <version>$Revision: 5731 $</version>
// </file>

using System;
using System.Runtime.InteropServices;
using System.Threading;

using ICSharpCode.Core.WinForms;
using ICSharpCode.SharpDevelop.Gui;
using ICSharpCode.SharpDevelop.Util;
using Microsoft.Win32.SafeHandles;

namespace ICSharpCode.SharpDevelop
{
	/// <summary>
	/// This class fixes SD2-1466: SharpDevelop freezes when debugged application sets clipboard text.
	/// The problem is that Clipboard.ContainsText may wait for the application owning the clipboard,
	/// which in turn may currently wait for SharpDevelop (through the debugger)
	/// </summary>
	internal static class ClipboardService
	{
        static volatile bool clipboardContainsText;

        static WorkerThread workerThread;
        static IAsyncResult currentWorker;
		
		public static void Initialize()
		{
			ICSharpCode.TextEditor.TextAreaClipboardHandler.ClipboardContainsText = GetClipboardContainsText;
			if (WorkbenchSingleton.MainForm != null) {
                WorkbenchSingleton.MainForm.Activated += OnMainFormActivated;
			} else {
				WorkbenchSingleton.WorkbenchCreated += delegate {
                    WorkbenchSingleton.MainForm.Activated += OnMainFormActivated;
				};
			}
		}

		static void OnMainFormActivated(object sender, EventArgs e)
		{
			UpdateClipboardContainsText();
		}
		
		public static bool GetClipboardContainsText()
		{
			WorkbenchSingleton.DebugAssertMainThread();
			if (WorkbenchSingleton.Workbench != null && WorkbenchSingleton.Workbench.IsActiveWindow) {
				UpdateClipboardContainsText();
			}
			return clipboardContainsText;
		}

        //static void UpdateClipboardContainsText()
        //{
        //    if (currentWorker != null && !currentWorker.IsCompleted)
        //        return;
        //    if (workerThread == null)
        //    {
        //        workerThread = new WorkerThread();
        //        Thread t = new Thread(new ThreadStart(workerThread.RunLoop));
        //        t.SetApartmentState(ApartmentState.STA);
        //        t.IsBackground = true;
        //        t.Name = "clipboard access";
        //        t.Start();
        //    }
        //    currentWorker = workerThread.Enqueue(DoUpdate);
        //    // wait a few ms in case the clipboard can be accessed without problems
        //    currentWorker.AsyncWaitHandle.WaitOne(50);
        //}

        static void UpdateClipboardContainsText()
        {
            if (currentWorker != null && !currentWorker.IsCompleted)
                return;
            if (workerThread == null)
            {
                workerThread = new WorkerThread();
                Thread t = new Thread(new ThreadStart(workerThread.RunLoop));
                t.SetApartmentState(ApartmentState.STA);
                t.IsBackground = true;
                t.Name = "clipboard access";
                t.Start();
            }
            currentWorker = workerThread.Enqueue(DoUpdate);
            // wait a few ms in case the clipboard can be accessed without problems
            WaitForSingleObject(currentWorker.AsyncWaitHandle.SafeWaitHandle, 50);
            // Using WaitHandle.WaitOne() pumps some Win32 messages.
            // To avoid unintended reentrancy, we need to avoid pumping messages,
            // so we directly call the Win32 WaitForSingleObject function.
            // See SD2-1638 for details.
        }

        [DllImport("kernel32", SetLastError = true, ExactSpelling = true)]
        static extern Int32 WaitForSingleObject(SafeWaitHandle handle, Int32 milliseconds);
		
		static void DoUpdate()
		{
            clipboardContainsText = ClipboardWrapper.ContainsText;
        }
	}
}
