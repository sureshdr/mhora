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
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;

namespace mhora
{
	public class ChooseHoroscopeControl : System.Windows.Forms.Form
	{
		private System.Windows.Forms.ListBox lBox;
		private System.Windows.Forms.Button bOK;
		private System.ComponentModel.IContainer components = null;

		public ChooseHoroscopeControl()
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitializeComponent call
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			if( disposing )
			{
				if (components != null) 
				{
					components.Dispose();
				}
			}
			base.Dispose( disposing );
		}

		#region Designer generated code
		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.lBox = new System.Windows.Forms.ListBox();
			this.bOK = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// lBox
			// 
			this.lBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.lBox.Location = new System.Drawing.Point(8, 32);
			this.lBox.Name = "lBox";
			this.lBox.Size = new System.Drawing.Size(344, 160);
			this.lBox.TabIndex = 0;
			// 
			// bOK
			// 
			this.bOK.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.bOK.Location = new System.Drawing.Point(136, 208);
			this.bOK.Name = "bOK";
			this.bOK.Size = new System.Drawing.Size(75, 24);
			this.bOK.TabIndex = 1;
			this.bOK.Text = "OK";
			this.bOK.Click += new System.EventHandler(this.bOK_Click);
			// 
			// ChooseHoroscopeControl
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(352, 238);
			this.Controls.Add(this.bOK);
			this.Controls.Add(this.lBox);
			this.Name = "ChooseHoroscopeControl";
			this.Text = "Please Choose An Open Horoscope";
			this.Load += new System.EventHandler(this.ChooseHoroscopeControl_Load);
			this.ResumeLayout(false);

		}
		#endregion

		public string GetHoroscopeName ()
		{
			if (lBox.SelectedIndex < 0) return null;

			MhoraContainer mc = (MhoraContainer)MhoraGlobalOptions.mainControl;
			foreach (Form c in mc.MdiChildren)
			{
				if (c is MhoraChild)
				{
					if (((MhoraChild)c).Name == (string)lBox.Items[lBox.SelectedIndex])
					{
						return ((MhoraChild)c).Name;
					}
				}
			}	
			return null;			
		}
		public Horoscope GetHorsocope ()
		{
			if (lBox.SelectedIndex < 0) return null;

			MhoraContainer mc = (MhoraContainer)MhoraGlobalOptions.mainControl;
			foreach (Form c in mc.MdiChildren)
			{
				if (c is MhoraChild)
				{
					if (((MhoraChild)c).Name == (string)lBox.Items[lBox.SelectedIndex])
					{
						MhoraChild ch = (MhoraChild)c;
						return ch.getHoroscope();
					}
				}
			}	
			return null;
		}

		private void ChooseHoroscopeControl_Load(object sender, System.EventArgs e)
		{
			MhoraContainer mc = (MhoraContainer)MhoraGlobalOptions.mainControl;
			foreach (Form c in mc.MdiChildren)
			{
				if (c is MhoraChild)
				{
					lBox.Items.Add(((MhoraChild)c).Name);
				}
			}	
			if (lBox.Items.Count > 0)
				lBox.SelectedIndex = 0;
		}

		private void bOK_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}


	}
}

