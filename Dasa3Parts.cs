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
	/// Summary description for Dasa3Parts.
	/// </summary>
	public class Dasa3Parts : System.Windows.Forms.Form
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		ToDate td = null;
		DasaEntry de = null;
		Horoscope h = null;
		private System.Windows.Forms.ListView mList;
		private System.Windows.Forms.ColumnHeader columnHeader1;
		private System.Windows.Forms.Label txtDesc;
		

		public Dasa3Parts(Horoscope _h, DasaEntry _de, ToDate _td)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			td = _td;
			de = _de;
			h = _h;
			this.repopulate();
		}


		private void populateDescription ()
		{
			sweph.obtainLock(h);
			Moment start = td.AddYears(de.startUT);
			Moment end = td.AddYears(de.startUT + de.DasaLength);
			sweph.releaseLock(h);
			ZodiacHouse zh = new ZodiacHouse(de.ZHouse);
			if ((int)de.ZHouse != 0)
				this.txtDesc.Text = string.Format ("{0} - {1} to {2}", zh, start, end);
			else
				this.txtDesc.Text = string.Format ("{0} - {1} to {2}", de.graha, start, end);
		}



		private void repopulate ()
		{
			this.populateDescription();

			double partLength = de.DasaLength / 3.0;

			sweph.obtainLock(h);
			ArrayList alParts = new ArrayList();
			for (int i=0; i<4; i++)
			{
				Moment m = td.AddYears(de.startUT + partLength * i);
				alParts.Add(m);
			}
			Moment[] momentParts = (Moment[])alParts.ToArray(typeof(Moment));
			sweph.releaseLock(h);

			for (int i=1; i< momentParts.Length; i++)
			{
				string fmt = string.Format 
					("Equal Part {0} - {1} to {2}",
					i, momentParts[i-1], momentParts[i]);
				this.mList.Items.Add (fmt);
			}

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
			this.txtDesc = new System.Windows.Forms.Label();
			this.mList = new System.Windows.Forms.ListView();
			this.columnHeader1 = new System.Windows.Forms.ColumnHeader();
			this.SuspendLayout();
			// 
			// txtDesc
			// 
			this.txtDesc.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.txtDesc.Location = new System.Drawing.Point(8, 8);
			this.txtDesc.Name = "txtDesc";
			this.txtDesc.Size = new System.Drawing.Size(472, 23);
			this.txtDesc.TabIndex = 0;
			this.txtDesc.Text = "txtDesc";
			this.txtDesc.Click += new System.EventHandler(this.label1_Click);
			// 
			// mList
			// 
			this.mList.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
				| System.Windows.Forms.AnchorStyles.Left) 
				| System.Windows.Forms.AnchorStyles.Right)));
			this.mList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																					this.columnHeader1});
			this.mList.FullRowSelect = true;
			this.mList.Location = new System.Drawing.Point(8, 40);
			this.mList.Name = "mList";
			this.mList.Size = new System.Drawing.Size(472, 272);
			this.mList.TabIndex = 1;
			this.mList.View = System.Windows.Forms.View.Details;
			this.mList.SelectedIndexChanged += new System.EventHandler(this.listView1_SelectedIndexChanged);
			// 
			// columnHeader1
			// 
			this.columnHeader1.Width = 1000;
			// 
			// Dasa3Parts
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.ClientSize = new System.Drawing.Size(488, 318);
			this.Controls.Add(this.mList);
			this.Controls.Add(this.txtDesc);
			this.Name = "Dasa3Parts";
			this.Text = "Dasa 3 Parts Reckoner";
			this.Load += new System.EventHandler(this.Dasa3Parts_Load);
			this.ResumeLayout(false);

		}
		#endregion

		private void Dasa3Parts_Load(object sender, System.EventArgs e)
		{
		
		}

		private void label1_Click(object sender, System.EventArgs e)
		{
		
		}

		private void listView1_SelectedIndexChanged(object sender, System.EventArgs e)
		{
		
		}
	}
}
