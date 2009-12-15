// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <owner name="David Srbeck�" email="dsrbecky@gmail.com"/>
//     <version>$Revision: 3648 $</version>
// </file>
#region License
//  
//  Copyright (c) 2007, ic#code
//  
//  All rights reserved.
//  
//  Redistribution  and  use  in  source  and  binary  forms,  with  or without
//  modification, are permitted provided that the following conditions are met:
//  
//  1. Redistributions  of  source code must retain the above copyright notice,
//     this list of conditions and the following disclaimer.
//  
//  2. Redistributions  in  binary  form  must  reproduce  the  above copyright
//     notice,  this  list  of  conditions  and the following disclaimer in the
//     documentation and/or other materials provided with the distribution.
//  
//  3. Neither the name of the ic#code nor the names of its contributors may be
//     used  to  endorse or promote products derived from this software without
//     specific prior written permission.
//  
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS"
//  AND  ANY  EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE
//  IMPLIED  WARRANTIES OF MERCHANTABILITY AND FITNESS FOR A PARTICULAR PURPOSE
//  ARE  DISCLAIMED.   IN NO EVENT SHALL THE COPYRIGHT OWNER OR CONTRIBUTORS BE
//  LIABLE  FOR  ANY  DIRECT,  INDIRECT,  INCIDENTAL,  SPECIAL,  EXEMPLARY,  OR
//  CONSEQUENTIAL  DAMAGES  (INCLUDING,  BUT  NOT  LIMITED  TO,  PROCUREMENT OF
//  SUBSTITUTE  GOODS  OR  SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS
//  INTERRUPTION)  HOWEVER  CAUSED  AND  ON ANY THEORY OF LIABILITY, WHETHER IN
//  CONTRACT,  STRICT  LIABILITY,  OR  TORT (INCLUDING NEGLIGENCE OR OTHERWISE)
//  ARISING  IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE
//  POSSIBILITY OF SUCH DAMAGE.
//  
#endregion

using System.ComponentModel;
using System.Windows.Forms;
using Debugger;
using ICSharpCode.Core;

namespace ICSharpCode.SharpDevelop.Gui.Pads
{
	public partial class RunningThreadsPad
	{
		ContextMenuStrip CreateContextMenuStrip()
		{
			ContextMenuStrip menu = new ContextMenuStrip();
			menu.Opening += FillContextMenuStrip;
			return menu;
		}
		
		void FillContextMenuStrip(object sender, CancelEventArgs e)
		{
			ListView.SelectedListViewItemCollection items = runningThreadsList.SelectedItems;
			
			if (items.Count == 0) {
				e.Cancel = true;
				return;
			}
			
			ListViewItem item = items[0];
			
			ContextMenuStrip menu = sender as ContextMenuStrip;
			menu.Items.Clear();
			
			ToolStripMenuItem freezeItem;
			freezeItem = new ToolStripMenuItem();
			freezeItem.Text = ResourceService.GetString("MainWindow.Windows.Debug.Threads.Freeze");
			freezeItem.Checked = (item.Tag as Thread).Suspended;
			freezeItem.Click +=
				delegate {
				ListView.SelectedListViewItemCollection selItems = runningThreadsList.SelectedItems;
				if (selItems.Count == 0) {
					return;
				}
				bool suspended = (selItems[0].Tag as Thread).Suspended;
				
				if (!debuggedProcess.IsPaused) {
					MessageService.ShowMessage("${res:MainWindow.Windows.Debug.Threads.CannotFreezeWhileRunning}", "${res:MainWindow.Windows.Debug.Threads.Freeze}");
					return;
				}
				
				foreach(ListViewItem i in selItems) {
					(i.Tag as Thread).Suspended = !suspended;
				}
				RefreshPad();
			};
			
			menu.Items.AddRange(new ToolStripItem[] {
			                    	freezeItem,
			                    });
			
			e.Cancel = false;
		}
	}
}
