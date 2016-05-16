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
	/// <summary>
	/// Summary description for LongStringEditor.
	/// </summary>
	public class LongStringEditor : System.Windows.Forms.Form
	{
		private System.Windows.Forms.TextBox mTextBox;
		private System.Windows.Forms.Button bOK;
		private System.Windows.Forms.Button bCancel;
		private System.Windows.Forms.Button bReset;
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		private string mTextOrig;
		public LongStringEditor(string _text)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();
			mTextOrig = _text;
			this.EditorText = mTextOrig;

			//
			// TODO: Add any constructor code after InitializeComponent call
			//
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
			this.mTextBox = new System.Windows.Forms.TextBox();
			this.bOK = new System.Windows.Forms.Button();
			this.bCancel = new System.Windows.Forms.Button();
			this.bReset = new System.Windows.Forms.Button();
			this.SuspendLayout();
			// 
			// mTextBox
			// 
			this.mTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.mTextBox.Location = new System.Drawing.Point(8, 8);
			this.mTextBox.Multiline = true;
			this.mTextBox.Name = "mTextBox";
			this.mTextBox.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.mTextBox.Size = new System.Drawing.Size(272, 224);
			this.mTextBox.TabIndex = 0;
			this.mTextBox.Text = "";
			this.mTextBox.TextChanged += new System.EventHandler(this.mTextBox_TextChanged);
			// 
			// bOK
			// 
			this.bOK.Location = new System.Drawing.Point(16, 240);
			this.bOK.Name = "bOK";
			this.bOK.TabIndex = 1;
			this.bOK.Text = "OK";
			this.bOK.Click += new System.EventHandler(this.bOK_Click);
			// 
			// bCancel
			// 
			this.bCancel.Location = new System.Drawing.Point(104, 240);
			this.bCancel.Name = "bCancel";
			this.bCancel.TabIndex = 2;
			this.bCancel.Text = "Cancel";
			this.bCancel.Click += new System.EventHandler(this.bCancel_Click);
			// 
			// bReset
			// 
			this.bReset.Location = new System.Drawing.Point(192, 240);
			this.bReset.Name = "bReset";
			this.bReset.TabIndex = 3;
			this.bReset.Text = "Reset";
			this.bReset.Click += new System.EventHandler(this.bReset_Click);
			// 
			// LongStringEditor
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(292, 266);
			this.Controls.Add(this.bReset);
			this.Controls.Add(this.bCancel);
			this.Controls.Add(this.bOK);
			this.Controls.Add(this.mTextBox);
			this.Name = "LongStringEditor";
			this.Load += new System.EventHandler(this.tData_Load);
			this.ResumeLayout(false);

		}
		#endregion


		public string EditorText
		{
			get { return this.mTextBox.Text; }
			set { this.mTextBox.Text = value; }
		}

		public string TitleText
		{
			set { this.Text = value; }
		}
		private void tData_Load(object sender, System.EventArgs e)
		{
		
		}

		private void bOK_Click(object sender, System.EventArgs e)
		{
			this.Close();
		}

		private void bCancel_Click(object sender, System.EventArgs e)
		{
			this.EditorText = this.mTextOrig;
			this.Close();
		}

		private void bReset_Click(object sender, System.EventArgs e)
		{
			this.EditorText = this.mTextOrig;
		}

		private void mTextBox_TextChanged(object sender, System.EventArgs e)
		{
		
		}
	}
}
