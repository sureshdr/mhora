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
using System.Data;
using System.Windows.Forms;

namespace mhora
{
	/// <summary>
	/// Summary description for MhoraSplitContainer.
	/// </summary>
	public class MhoraSplitContainer : System.Windows.Forms.UserControl
	{
		private System.Windows.Forms.UserControl mControl2;
		private System.Windows.Forms.UserControl mControl1;
		private int nItems;
		public Splitter sp;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public enum DrawStyle: int
		{
			LeftRight, UpDown
		}
		private DrawStyle mDrawDock;
		public DrawStyle DrawDock
		{
			get { return mDrawDock; }
			set { 
				mDrawDock = value; 

				if (nItems < 1)
				{
					mControl1.Dock = DockStyle.Fill;
					return;
				}
				
				if (mDrawDock == DrawStyle.UpDown) 
				{
					mControl1.Dock = DockStyle.Top;
					sp.Dock = DockStyle.Top;
				}
				else
				{
					mControl1.Dock = DockStyle.Left;
					sp.Dock = DockStyle.Left;
				}
				mControl2.Dock = DockStyle.Fill;
			}
		}

		public System.Windows.Forms.UserControl Control1
		{
			get { return mControl1; }
			set { mControl1 = value; }
		}
		public System.Windows.Forms.UserControl Control2
		{
			get { return mControl2; }
			set 
			{ 
				mControl2 = value; 
				this.DrawDock = this.DrawDock;
				//mControl1.Dock = DockStyle.Left;
				//mControl2.Dock = DockStyle.Fill;
				if (nItems == 1) 
				{
					nItems++;
					this.Controls.Remove (mControl1);
					/*if (this.DrawDock == DrawStyle.UpDown)
						sp.SplitPosition = this.Width / 2;
					else
						sp.SplitPosition = this.Height / 2;
					*/
					this.Controls.AddRange(new Control[]{mControl2, sp, mControl1});
				}
			}
		}
		public DockStyle SplitterDockStyle
		{
			get { return sp.Dock; }
			set { sp.Dock = value; }
		}

		public MhoraSplitContainer(System.Windows.Forms.UserControl _mControl)
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitForm call
			mControl1 = _mControl;
			mControl1.Dock = DockStyle.Fill;
			this.Controls.Add(mControl1);
			sp = new Splitter();
			sp.BackColor = Color.LightGray;
			sp.Dock = DockStyle.Left;
			DrawDock = DrawStyle.LeftRight;
			nItems = 1;
			
			this.Dock = DockStyle.Fill;
			sp.Height += 2;
			sp.Width += 2;

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

		#region Component Designer generated code
		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			// 
			// MhoraSplitContainer
			// 
			this.Name = "MhoraSplitContainer";
			this.Resize += new System.EventHandler(this.MhoraSplitContainer_Resize);
			this.Load += new System.EventHandler(this.MhoraSplitContainer_Load);

		}
		#endregion

		private void MhoraSplitContainer_Load(object sender, System.EventArgs e)
		{
		
		}

		private void MhoraSplitContainer_Resize(object sender, System.EventArgs e)
		{
			
		}
	}
}
