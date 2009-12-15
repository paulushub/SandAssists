﻿// <file>
//     <copyright see="prj:///doc/copyright.txt"/>
//     <license see="prj:///doc/license.txt"/>
//     <owner name="Matthew Ward" email="mrward@users.sourceforge.net"/>
//     <version>$Revision: 1636 $</version>
// </file>

using System;
using System.Collections;
using System.ComponentModel.Design.Serialization;

namespace ICSharpCode.FormsDesigner.Services
{
	/// <summary>
	/// This class implements the IDesignerSerializationService interface.
	/// A designer loader that does not derive from the CodeDOMDesignerLoader class 
	/// (e.g. the XmlDesignerLoader) need to create an instance of this class
	/// and it to the available services otherwise cut/copy and paste will not work.
	/// </summary>
	public class DesignerSerializationService : IDesignerSerializationService
	{
		IServiceProvider serviceProvider;
		
		public DesignerSerializationService(IServiceProvider serviceProvider)
		{
			this.serviceProvider = serviceProvider;
		}
		
		/// <summary>
		/// Deserializes the serialization data object and returns a 
		/// collection of objects represented by that data. 
		/// </summary>
		public ICollection Deserialize(object serializationData)
		{
			ComponentSerializationService serializationService = (ComponentSerializationService)serviceProvider.GetService(typeof(ComponentSerializationService));
			return serializationService.Deserialize((SerializationStore)serializationData);
		}
		
		/// <summary>
		/// Serializes a collection of objects and stores them in a 
		/// serialization data object. 
		/// </summary>
		public object Serialize(ICollection objects)
		{
			ComponentSerializationService serializationService = (ComponentSerializationService)serviceProvider.GetService(typeof(ComponentSerializationService));
			SerializationStore store = serializationService.CreateStore();
			
			foreach (object value in objects) {
				serializationService.Serialize(store, value);
			}
			store.Close();
			return store;
		}
	}
}
