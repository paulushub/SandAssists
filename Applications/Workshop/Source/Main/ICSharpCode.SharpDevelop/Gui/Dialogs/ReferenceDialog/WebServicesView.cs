// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 3287 $</version>
// </file>

using System;
using System.Web.Services.Description;
using System.Windows.Forms;

using ICSharpCode.Core;
using ICSharpCode.Core.WinForms;

namespace ICSharpCode.SharpDevelop.Gui
{
    public partial class WebServicesView : System.Windows.Forms.UserControl
	{
		const int ServiceDescriptionImageIndex = 0;
		const int ServiceImageIndex = 1;
		const int PortImageIndex = 2;
		const int OperationImageIndex = 3;
		
		public WebServicesView()
		{
			InitializeComponent();
			AddImages();
			AddStringResources();
		}
		
		/// <summary>
		/// Removes all web services currently on display.
		/// </summary>
		public void Clear()
		{
			webServicesListView.Items.Clear();
			webServicesTreeView.Nodes.Clear();
		}
		
		public void Add(ServiceDescriptionCollection serviceDescriptions)
		{
			if (serviceDescriptions.Count == 0) {
				return;
			}

			webServicesListView.BeginUpdate();
			try {
				foreach (ServiceDescription description in serviceDescriptions) {
					Add(description);
				}
			} finally {
				webServicesListView.EndUpdate();
			}
		}
		
		void WebServicesTreeViewAfterSelect(object sender, TreeViewEventArgs e)
		{
			ListViewItem item;
			webServicesListView.Items.Clear();
			
			if(e.Node.Tag is ServiceDescription) {
				ServiceDescription desc = (ServiceDescription)e.Node.Tag;
				item = new ListViewItem();
				item.Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.RetrievalUriProperty}");
				item.SubItems.Add(desc.RetrievalUrl);
				webServicesListView.Items.Add(item);
			}
			else if(e.Node.Tag is Service) {
				Service service = (Service)e.Node.Tag;
				item = new ListViewItem();
				item.Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.DocumentationProperty}");
				item.SubItems.Add(service.Documentation);
				webServicesListView.Items.Add(item);
			}
			else if(e.Node.Tag is Port) {
				Port port = (Port)e.Node.Tag;

				item = new ListViewItem();
				item.Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.DocumentationProperty}");
				item.SubItems.Add(port.Documentation);
				webServicesListView.Items.Add(item);
				
				item = new ListViewItem();
				item.Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.BindingProperty}");
				item.SubItems.Add(port.Binding.Name);
				webServicesListView.Items.Add(item);
				
				item = new ListViewItem();
				item.Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.ServiceNameProperty}");
				item.SubItems.Add(port.Service.Name);
				webServicesListView.Items.Add(item);
			}
			else if(e.Node.Tag is Operation) {
				Operation operation = (Operation)e.Node.Tag;
				
				item = new ListViewItem();
				item.Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.DocumentationProperty}");
				item.SubItems.Add(operation.Documentation);
				webServicesListView.Items.Add(item);

				item = new ListViewItem();
				item.Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.ParametersProperty}");
				item.SubItems.Add(operation.ParameterOrderString);
				webServicesListView.Items.Add(item);
			}
		}
		
		void Add(ServiceDescription description)
		{
			TreeNode rootNode = new TreeNode(GetName(description));
			rootNode.Tag = description;
			rootNode.ImageIndex = ServiceDescriptionImageIndex;
			rootNode.SelectedImageIndex = ServiceDescriptionImageIndex;
			webServicesTreeView.Nodes.Add(rootNode);

			foreach(Service service in description.Services) {
				// Add a Service node
				TreeNode serviceNode = new TreeNode(service.Name);
				serviceNode.Tag = service;
				serviceNode.ImageIndex = ServiceImageIndex;
				serviceNode.SelectedImageIndex = ServiceImageIndex;
				rootNode.Nodes.Add(serviceNode);
				
				foreach(Port port in service.Ports) {
					TreeNode portNode = new TreeNode(port.Name);
					portNode.Tag = port;
					portNode.ImageIndex = PortImageIndex;
					portNode.SelectedImageIndex = PortImageIndex;
					serviceNode.Nodes.Add(portNode);
					
					// Get the operations
					System.Web.Services.Description.Binding binding = description.Bindings[port.Binding.Name];
					if (binding != null) {
						PortType portType = description.PortTypes[binding.Type.Name];
						if (portType != null) {
							foreach(Operation operation in portType.Operations) {
								TreeNode operationNode = new TreeNode(operation.Name);
								operationNode.Tag = operation;
								operationNode.ImageIndex = OperationImageIndex;
								operationNode.SelectedImageIndex = OperationImageIndex;
								portNode.Nodes.Add(operationNode);
							}
						}
					}
				}
			}
			webServicesTreeView.ExpandAll();
		}
		
		string GetName(ServiceDescription description)
		{
			if (description.Name != null) {
				return description.Name;
			} else if (description.RetrievalUrl != null) {
				Uri uri = new Uri(description.RetrievalUrl);
				if (uri.Segments.Length > 0) {
					return uri.Segments[uri.Segments.Length - 1];
				} else {
					return uri.Host;
				}
			}
			return String.Empty;
		}
		
		void AddImages()
		{
			try {
				ImageList imageList = new ImageList();
				imageList.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.Library"));
				imageList.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.Interface"));
				imageList.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.Class"));
				imageList.Images.Add(WinFormsResourceService.GetBitmap("Icons.16x16.Method"));
				
				webServicesTreeView.ImageList = imageList;
			} catch (ResourceNotFoundException) {
				// in design mode, the core is not initialized -> the resources cannot be found
			}
		}
		
		void AddStringResources()
		{
			valueColumnHeader.Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.ValueColumnHeader}");
			propertyColumnHeader.Text = StringParser.Parse("${res:ICSharpCode.SharpDevelop.Gui.Dialogs.AddWebReferenceDialog.PropertyColumnHeader}");
		}
	}
}
