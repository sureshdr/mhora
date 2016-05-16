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
	public class VaraChakra : mhora.MhoraControl
	{
		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.ContextMenu contextMenu;
		private Bitmap bmpBuffer = null;
		private Pen pn_black = null;
		private Pen pn_grey = null;
		private Brush b_black = null;
		private Font f = null;

		public VaraChakra(Horoscope _h)
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();
			h = _h;
			h.Changed += new EvtChanged(OnRecalculate);
			MhoraGlobalOptions.DisplayPrefsChanged += new EvtChanged(OnResize);
			pn_black = new Pen (Color.Black, (float)0.1);
			pn_grey = new Pen (Color.Gray, (float)0.1);
			b_black = new SolidBrush(Color.Black);
			this.AddViewsToContextMenu(contextMenu);
			this.OnResize(MhoraGlobalOptions.Instance);
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
			this.contextMenu = new System.Windows.Forms.ContextMenu();
			// 
			// VaraChakra
			// 
			this.ContextMenu = this.contextMenu;
			this.Name = "VaraChakra";
			this.Size = new System.Drawing.Size(456, 240);
			this.Resize += new System.EventHandler(this.VaraChakra_Resize);
			this.Load += new System.EventHandler(this.VaraChakra_Load);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.VaraChakra_Paint);

		}
		#endregion

		private void VaraChakra_Load(object sender, System.EventArgs e)
		{
		
		}
		public void OnResize (object o)
		{
			f = new Font(
				MhoraGlobalOptions.Instance.GeneralFont.FontFamily, 
				MhoraGlobalOptions.Instance.GeneralFont.SizeInPoints-4
				);
			this.DrawToBuffer(true);
			this.Invalidate();
		}
		public void OnRecalculate (Object o)
		{
			this.Invalidate();
		}
		private void ResetChakra (Graphics g, double rot)
		{
			int size = Math.Min(bmpBuffer.Width, bmpBuffer.Height);
			float scale = (float)size/310;
			g.ResetTransform();
			g.TranslateTransform(bmpBuffer.Width/2, bmpBuffer.Height/2);
			g.ScaleTransform(scale, scale);
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			g.RotateTransform((float)(270.0+(360.0/(9.0*2.0))-rot));
		}
		private void DrawChakra (Graphics g)
		{
			Body.Name[] bodies = new Body.Name[]
			{
				Body.Name.Sun, Body.Name.Moon, Body.Name.Mars, Body.Name.Mercury,
				Body.Name.Jupiter, Body.Name.Venus, Body.Name.Saturn,
				Body.Name.Rahu, Body.Name.Ketu
			};

			g.Clear(MhoraGlobalOptions.Instance.ChakraBackgroundColor);

			this.ResetChakra(g, 0.0);
			g.DrawEllipse(pn_grey, -150, -150, 300, 300);
			g.DrawEllipse(pn_grey, -140, -140, 280, 280);

			for (int i=0; i<9; i++)
			{
				this.ResetChakra(g, i*(360.0/9.0));
				g.DrawLine(pn_grey, 0, 0, 150, 0);
			}

			for (int i=0; i<9; i++)
			{
				this.ResetChakra(g, i*(360.0/9.0)+(360.0/(9.0*2.0)));
				g.TranslateTransform(135, 0);
				g.RotateTransform((float)90.0);
				SizeF sz = g.MeasureString(Body.toString(bodies[i]), f);
				g.DrawString(Body.toString(bodies[i]), f, b_black, -sz.Width/2, 0);
			}

			if (h.isDayBirth())
			{

			}

		}

		private Image DrawToBuffer(bool bRecalc)
		{
			if (bmpBuffer != null && bmpBuffer.Size != this.Size)
			{
				bmpBuffer.Dispose();
				bmpBuffer = null;
			}
		
			if (this.Width == 0 || this.Height == 0)
				return bmpBuffer;

			if (bRecalc == false && this.Width == bmpBuffer.Width && this.Height == bmpBuffer.Height)
				return bmpBuffer;

			Graphics displayGraphics = this.CreateGraphics();
			bmpBuffer = new Bitmap(this.Width, this.Height, displayGraphics);
			Graphics imageGraphics = Graphics.FromImage(bmpBuffer);
			DrawChakra(imageGraphics);
			displayGraphics.Dispose();
			return bmpBuffer;
		}

		private void VaraChakra_Resize(object sender, System.EventArgs e)
		{
			this.DrawToBuffer(true);
			this.Invalidate();			
		}

		private void VaraChakra_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			this.DrawChakra(e.Graphics);
		}
		protected override void copyToClipboard ()
		{
			Clipboard.SetDataObject(this.bmpBuffer);
		}

	}
}

