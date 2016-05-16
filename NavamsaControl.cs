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
using System.Diagnostics;

namespace mhora
{
	public class NavamsaControl : mhora.MhoraControl
	{
		private System.ComponentModel.IContainer components = null;
		public Bitmap bmpBuffer = null;
		private Pen pn_black = null;
		private Pen pn_grey = null;
		private Pen pn_lgrey = null;
		private System.Windows.Forms.ContextMenu contextMenu;
		private System.Windows.Forms.MenuItem menuItem1;
		private Font f = null;
		private string[] nak_s = null;

		public NavamsaControl(Horoscope _h)
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();
			h = _h;
			MhoraGlobalOptions.DisplayPrefsChanged += new EvtChanged(onRedisplay);
			h.Changed += new EvtChanged(onRecalculate);
			pn_black = new Pen(Color.Black, (float)0.1);
			pn_grey = new Pen (Color.Gray, (float)0.1);
			pn_lgrey = new Pen (Color.LightGray, (float)0.1);
			nak_s = new string[27]
			{
				"Asw", "Bha", "Kri", "Roh", "Mri", "Ard", "Pun", "Pus", "Asl",
				"Mag", "PPl", "UPh", "Has", "Chi", "Swa", "Vis", "Anu", "Jye",
				"Moo", "PAs", "UAs", "Sra", "Dha", "Sha", "PBh", "UBh", "Rev"
			};
			this.AddViewsToContextMenu(contextMenu);
			this.onRedisplay(MhoraGlobalOptions.Instance);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			MhoraGlobalOptions.DisplayPrefsChanged -= new EvtChanged(this.onRedisplay);
			h.Changed -= new EvtChanged(onRecalculate);
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
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			// 
			// contextMenu
			// 
			this.contextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						this.menuItem1});
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.Text = "-";
			// 
			// NavamsaControl
			// 
			this.ContextMenu = this.contextMenu;
			this.Name = "NavamsaControl";
			this.Size = new System.Drawing.Size(376, 240);
			this.Resize += new System.EventHandler(this.NavamsaControl_Resize);
			this.Load += new System.EventHandler(this.NavamsaControl_Load);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.NavamsaControl_Paint);

		}
		#endregion

		public void onRedisplay (object o)
		{
			f = new Font(
				MhoraGlobalOptions.Instance.GeneralFont.FontFamily, 
				MhoraGlobalOptions.Instance.GeneralFont.SizeInPoints-5);
			this.DrawToBuffer(true);
			this.Invalidate();
		}
		public void onRecalculate (object o)
		{
			this.DrawToBuffer(true);
			this.Invalidate();
		}
		private void ResetChakra (Graphics g, double rot)
		{
			int size = Math.Min(bmpBuffer.Width, bmpBuffer.Height);
			float scale = (float)size/300;
			g.ResetTransform();
			g.TranslateTransform(bmpBuffer.Width/2, bmpBuffer.Height/2);
			g.ScaleTransform(scale, scale);
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			g.RotateTransform((float)(270.0-rot));
		}

		private void DrawInnerChakra (Graphics g)
		{
			int size = Math.Min(bmpBuffer.Width, bmpBuffer.Height);
			float scale = (float)size/300/3;
			g.ResetTransform();
			g.TranslateTransform(bmpBuffer.Width/2, bmpBuffer.Height/2);
			g.ScaleTransform(scale, scale);
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			EastIndianChart dc = new EastIndianChart();
			g.TranslateTransform(-1*dc.GetLength()/2, -1*dc.GetLength()/2);
			dc.DrawOutline(g);

		}
		public bool PrintMode = false;

		public void DrawChakra (Graphics g)
		{
			//this.DrawInnerChakra(g);

			if (false == this.PrintMode)
				g.Clear(MhoraGlobalOptions.Instance.ChakraBackgroundColor);

			this.ResetChakra(g, 0.0);
			g.DrawEllipse(pn_grey, -40, -40, 80, 80);
			g.DrawEllipse(pn_grey, -125,-125,250,250);
			g.DrawEllipse(pn_grey, -105,-105,210,210);
			g.DrawEllipse(pn_grey, -115, -115, 230, 230);

			Body.Name[] bodies = new Body.Name[10]
			{
				Body.Name.Lagna, Body.Name.Sun, Body.Name.Moon, Body.Name.Mars, Body.Name.Mercury,
				Body.Name.Jupiter, Body.Name.Venus, Body.Name.Saturn, Body.Name.Rahu, Body.Name.Ketu
			};

			for (int i=0; i<12; i++)
			{
				ResetChakra (g, i*30);
				g.DrawLine(pn_lgrey, 40, 0, 125, 0);
			}
			for (int i=0; i<12; i++)
			{
				ResetChakra(g, i*30+15);
				ZodiacHouse.Name z = (ZodiacHouse.Name)(i+1);
				SizeF sz = g.MeasureString(z.ToString(), f);
				g.DrawString(z.ToString(), f, Brushes.Gray, 40-sz.Width , 0);
			}
			for (int i=0; i<27; i++)
			{
				ResetChakra(g, (i+1)*(360.0/27.0));//+((360.0/27.0)/2.0));
				g.TranslateTransform(105, 0);
				g.RotateTransform((float)90.0);
				SizeF sz = g.MeasureString(nak_s[i], f);
				g.DrawString(nak_s[i], f, Brushes.Gray, (float)(360.0/27.0)-(sz.Width/2), sz.Height/2);
			}
			for (int i=0; i<27*4; i++)
			{
				ResetChakra(g, i*(360.0/(27.0*4.0)));
				Pen p = pn_lgrey;
				if (i%12==0) p = pn_black;
				g.DrawLine(p, 115, 0, 125, 0);

				p = pn_lgrey;
				if (i%4==0) p = pn_black;
				g.DrawLine(p, 105, 0, 115, 0);


			}

			double dist_sat = h.getPosition(Body.Name.Saturn).distance;
			foreach (Body.Name b in bodies)
			{
				Pen pn_b = new Pen(MhoraGlobalOptions.Instance.getBinduColor(b));
				Brush br_b = new SolidBrush(MhoraGlobalOptions.Instance.getBinduColor(b));
				BodyPosition bp = h.getPosition(b);
				ResetChakra (g, bp.longitude.value);
				int chWidth=2;
				g.DrawEllipse (pn_black, 110-(chWidth), 0, 1, 1);
				g.FillEllipse(br_b, 120-chWidth, -chWidth, chWidth*2, chWidth*2);
				g.DrawEllipse(pn_grey, 120-chWidth, -chWidth, chWidth*2, chWidth*2);
				SizeF sz = g.MeasureString(b.ToString(), f);
				g.DrawString(b.ToString(), f, Brushes.Black, 125, -sz.Height/2);

				// current position with distance
				int dist = (int)(bp.distance / dist_sat * (105-40-chWidth*2));
				g.FillEllipse(br_b, 40+dist-chWidth, -chWidth, chWidth*2, chWidth*2);
				g.DrawEllipse(pn_grey, 40+dist-chWidth, -chWidth, chWidth*2, chWidth*2);

				// speed
				double dspSize = (bp.speed_longitude/360.0)*12000.0;
				if (bp.speed_longitude < 0) dspSize *= 2.0;
				int spSize = (int)dspSize;
				if (spSize > 40) spSize = 40;
				if (bp.speed_longitude > 0)
					g.DrawLine(pn_lgrey, 40+dist, -chWidth, 40+dist, -spSize);
				else
					g.DrawLine(pn_lgrey, 40+dist, chWidth, 40+dist, -spSize);
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

		public Bitmap DrawToBitmap (int size)
		{
			bmpBuffer = new Bitmap(size, size);
			Graphics imageGraphics = Graphics.FromImage(bmpBuffer);
			DrawChakra(imageGraphics);
			return bmpBuffer;
		}


		private void NavamsaControl_Load(object sender, System.EventArgs e)
		{
		}

		private void NavamsaControl_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			e.Graphics.DrawImage(bmpBuffer, 0, 0);
		}

		private void NavamsaControl_Resize(object sender, System.EventArgs e)
		{
			this.DrawToBuffer(true);
			this.Invalidate();
		}
		protected override void copyToClipboard ()
		{
			Clipboard.SetDataObject(this.bmpBuffer);
		}

	}
}

