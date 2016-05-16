/******
Copyright (C) 2005 Ajit Krishnan (http://www.mudgala.com)

This program is free software; you can redistribute it and/or
modify it under the terms of the GNU General Public License
as published by the Free Software Foundation; either version 2
of the License, or (at your option) any later version.

This program is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program; if not, write to the Free Software
Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
******/

using System;
using System.Drawing;
using System.Collections;
using System.ComponentModel;
using System.Windows.Forms;

namespace mhora
{
	public delegate object ApplyOptions (Object sender);
	/// <summary>
	/// Display a PropertyGrid for any object, and deal with
	/// event handlers to perform any requested updates
	/// </summary>
	public class MhoraOptions : System.Windows.Forms.Form
	{
		public System.Windows.Forms.PropertyGrid pGrid;
		private System.Windows.Forms.Button bApply;
		private System.Windows.Forms.Button bCancel;
		private ApplyOptions applyEvent;
		private System.Windows.Forms.Button bOK;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public MhoraOptions(Object a, ApplyOptions o, bool NoCancel)
		{
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			pGrid.SelectedObject = new GlobalizedPropertiesWrapper(a);
			pGrid.HelpVisible = true;
			applyEvent = o;
			this.bCancel.Enabled = false;
		}
		public MhoraOptions(Object a, ApplyOptions o)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
			pGrid.SelectedObject = new GlobalizedPropertiesWrapper(a);
			applyEvent = o;
			//this.applyEvent(pGrid.SelectedObject);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if(components != null)
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Windows Form Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.pGrid = new System.Windows.Forms.PropertyGrid();
			this.bApply = new System.Windows.Forms.Button();
			this.bCancel = new System.Windows.Forms.Button();
			this.bOK = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// pGrid
			// 
			this.pGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.pGrid.CommandsVisibleIfAvailable = true;
			this.pGrid.LargeButtons = false;
			this.pGrid.LineColor = System.Drawing.SystemColors.ScrollBar;
			this.pGrid.Location = new System.Drawing.Point(8, 8);
			this.pGrid.Name = "pGrid";
			this.pGrid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
			this.pGrid.Size = new System.Drawing.Size(284, 216);
			this.pGrid.TabIndex = 1;
			this.pGrid.Text = "propertyGrid1";
			this.pGrid.ToolbarVisible = false;
			this.pGrid.ViewBackColor = System.Drawing.SystemColors.Window;
			this.pGrid.ViewForeColor = System.Drawing.SystemColors.WindowText;
			this.pGrid.Click += new System.EventHandler(this.pGrid_Click);
			// 
			// bApply
			// 
			this.bApply.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
			this.bApply.Location = new System.Drawing.Point(8, 232);
			this.bApply.Name = "bApply";
			this.bApply.TabIndex = 0;
			this.bApply.Text = "Apply";
			this.bApply.Click += new System.EventHandler(this.bApply_Click);
			// 
			// bCancel
			// 
			this.bCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.bCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.bCancel.Location = new System.Drawing.Point(204, 232);
			this.bCancel.Name = "bCancel";
			this.bCancel.TabIndex = 2;
			this.bCancel.Text = "Cancel";
			this.bCancel.Click += new System.EventHandler(this.bCancel_Click);
			// 
			// bOK
			// 
			this.bOK.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.bOK.Location = new System.Drawing.Point(104, 232);
			this.bOK.Name = "bOK";
			this.bOK.Size = new System.Drawing.Size(79, 23);
			this.bOK.TabIndex = 3;
			this.bOK.Text = "OK";
			this.bOK.Click += new System.EventHandler(this.bOK_Click);
			// 
			// MhoraOptions
			// 
			this.AcceptButton = this.bApply;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.CancelButton = this.bCancel;
			this.ClientSize = new System.Drawing.Size(296, 273);
			this.Controls.Add(this.bOK);
			this.Controls.Add(this.bCancel);
			this.Controls.Add(this.bApply);
			this.Controls.Add(this.pGrid);
			this.Name = "MhoraOptions";
			this.Text = "Options";
			this.Load += new System.EventHandler(this.MhoraOptions_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void MhoraOptions_Load(object sender, System.EventArgs e)
		{
		
		}

		private void pGrid_Click(object sender, System.EventArgs e)
		{
		
		}

		private void bCancel_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void Apply (bool bKeepOpen)
		{
			GlobalizedPropertiesWrapper wrapper = (GlobalizedPropertiesWrapper)this.pGrid.SelectedObject;
			object objApplied = applyEvent(wrapper.GetWrappedObject());
			if (bKeepOpen)
				pGrid.SelectedObject = new GlobalizedPropertiesWrapper(objApplied);
		}
		private void bApply_Click(object sender, System.EventArgs e)
		{
			this.Apply(true);
		}

		private void bOK_Click(object sender, System.EventArgs e)
		{
			this.Apply(false);
			this.Close();
		}
	}

	public class GlobalizedPropertiesWrapper : ICustomTypeDescriptor
	{
		private object obj = null;
		public GlobalizedPropertiesWrapper (object _obj)
		{
			obj = _obj;
		}
		public object GetWrappedObject ()
		{
			return obj;
		}
		public String GetClassName()
		{
			return TypeDescriptor.GetClassName(obj,true);
		}
		public AttributeCollection GetAttributes()
		{
			return TypeDescriptor.GetAttributes(obj,true);
		}
		public String GetComponentName()
		{
			return TypeDescriptor.GetComponentName(obj, true);
		}
		public TypeConverter GetConverter()
		{
			return TypeDescriptor.GetConverter(obj, true);
		}
		public EventDescriptor GetDefaultEvent()
		{
			return TypeDescriptor.GetDefaultEvent(obj, true);
		}
		public PropertyDescriptor GetDefaultProperty()
		{
			return TypeDescriptor.GetDefaultProperty(obj, true);
		}
		public object GetEditor(Type editorBaseType)
		{
			return TypeDescriptor.GetEditor(obj, editorBaseType, true);
		}
		public EventDescriptorCollection GetEvents(Attribute[] attributes)
		{
			return TypeDescriptor.GetEvents(obj, attributes, true);
		}
		public EventDescriptorCollection GetEvents()
		{
			return TypeDescriptor.GetEvents(obj, true);
		}
		public object GetPropertyOwner(PropertyDescriptor pd)
		{
			return obj;
		}
		public bool IsPropertyVisible (PropertyDescriptor prop)
		{
			if (null != prop.Attributes[typeof(PGNotVisible)])
				return false;

			return true;
			//	Console.WriteLine ("Property {0} is invisible", prop.DisplayName);
			//return true;
		}
		public PropertyDescriptorCollection GetProperties(Attribute[] attributes)
		{
			ArrayList orderedProperties = new ArrayList();
			PropertyDescriptorCollection retProps = new PropertyDescriptorCollection(null);
			PropertyDescriptorCollection baseProps = TypeDescriptor.GetProperties(obj, attributes, true);
			foreach (PropertyDescriptor oProp in baseProps)
			{
				Attribute attOrder = oProp.Attributes[typeof(PropertyOrderAttribute)];
				if (false == IsPropertyVisible(oProp))
					continue;

				if (attOrder != null)
				{
					//
					// If the attribute is found, then create an pair object to hold it
					//
					PropertyOrderAttribute poa = (PropertyOrderAttribute)attOrder;
					orderedProperties.Add(new PropertyOrderPair(oProp, oProp.Name,poa.Order));
				}
				else
				{
					//
					// If no order attribute is specifed then given it an order of 0
					//
					orderedProperties.Add(new PropertyOrderPair(oProp, oProp.Name,0));
				}
				//retProps.Add (new GlobalizedPropertyDescriptor(oProp));

				//Console.WriteLine ("Enumerating property {0}", oProp.DisplayName);
				//PGDisplayName invisible = (PGDisplayName)oProp.Attributes[typeof(PGNotVisible)];
				//if (invisible == null)
				//else
				//	Console.WriteLine ("Property {0} is invisible", oProp.DisplayName);
			}

			orderedProperties.Sort();
			foreach (PropertyOrderPair pop in orderedProperties)
			{
				Console.WriteLine ("Adding sorted {0}", pop.Name);
				retProps.Add (new GlobalizedPropertyDescriptor(pop.Property));
			}
			return retProps;
		}
		public PropertyDescriptorCollection GetProperties ()
		{
			PropertyDescriptorCollection baseProps = TypeDescriptor.GetProperties(obj, true);
			return baseProps;
		}

	}

	public class GlobalizedPropertyDescriptor : PropertyDescriptor
	{
		private PropertyDescriptor basePropertyDescriptor; 

		public GlobalizedPropertyDescriptor(PropertyDescriptor basePropertyDescriptor) : base(basePropertyDescriptor)
		{
			this.basePropertyDescriptor = basePropertyDescriptor;
		}

		public override bool CanResetValue(object component)
		{
			return basePropertyDescriptor.CanResetValue(component);
		}

		public override Type ComponentType
		{
			get { return basePropertyDescriptor.ComponentType; }
		}

		public override string DisplayName
		{
			get 
			{
				PGDisplayName dn = (PGDisplayName)basePropertyDescriptor.Attributes[typeof(PGDisplayName)];
				if (dn != null)
					return dn.DisplayName;
				return basePropertyDescriptor.DisplayName;
			}
		}

		public override string Description
		{
			get { return basePropertyDescriptor.Description; }
		}

		public override object GetValue(object component)
		{
			return this.basePropertyDescriptor.GetValue(component);
		}

		public override bool IsReadOnly
		{
			get { return this.basePropertyDescriptor.IsReadOnly; }
		}

		public override string Name
		{
			get { return this.basePropertyDescriptor.Name; }
		}

		public override Type PropertyType
		{
			get { return this.basePropertyDescriptor.PropertyType; }
		}

		public override void ResetValue(object component)
		{
			this.basePropertyDescriptor.ResetValue(component);
		}

		public override bool ShouldSerializeValue(object component)
		{
			return this.basePropertyDescriptor.ShouldSerializeValue(component);
		}

		public override void SetValue(object component, object value)
		{
			this.basePropertyDescriptor.SetValue(component, value);
		}
	}

	public class PropertyOrderPair : IComparable
	{
		private int _order;
		private string _name;
		private PropertyDescriptor _pdesc;
		public string Name
		{
			get
			{
				return _name;
			}
		}
		public PropertyDescriptor Property
		{
			get { return _pdesc; }
		}

		public PropertyOrderPair(PropertyDescriptor pdesc, string name, int order)
		{
			_pdesc = pdesc;
			_order = order;
			_name = name;
		}

		public int CompareTo(object obj)
		{
			//
			// Sort the pair objects by ordering by order value
			// Equal values get the same rank
			//
			int otherOrder = ((PropertyOrderPair)obj)._order;
			if (otherOrder == _order)
			{
				//
				// If order not specified, sort by name
				//
				string otherName = ((PropertyOrderPair)obj)._name;
				return string.Compare(_name,otherName);
			}
			else if (otherOrder > _order)
			{
				return -1;
			}
			return 1;
		}
	}

}
