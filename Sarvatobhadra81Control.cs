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
	public class Sarvatobhadra81Control : mhora.MhoraControl
	{
		private System.ComponentModel.IContainer components = null;

		private Bitmap bmpBuffer = null;
		private Pen pn_black = null;
		private Pen pn_grey = null;
		private Brush b_black = null;
		private System.Windows.Forms.ContextMenu mContextMenu;
		private Font f = null;
		int bufferOffset = 10;
		int bufferCellSize = 50;

		public Sarvatobhadra81Control(Horoscope _h)
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();
			h = _h;
			h.Changed += new EvtChanged(OnRecalculate);
			MhoraGlobalOptions.DisplayPrefsChanged += new EvtChanged(OnRedisplay);
			pn_black = new Pen (Color.Black, (float)0.1);
			pn_grey = new Pen (Color.Gray, (float)0.1);
			b_black = new SolidBrush(Color.Black);
			this.AddViewsToContextMenu(mContextMenu);
			this.OnRedisplay(MhoraGlobalOptions.Instance);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		protected override void Dispose( bool disposing )
		{
			MhoraGlobalOptions.DisplayPrefsChanged -= new EvtChanged(this.OnRedisplay);
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
			this.mContextMenu = new System.Windows.Forms.ContextMenu();
			// 
			// Sarvatobhadra81Control
			// 
			this.ContextMenu = this.mContextMenu;
			this.Name = "Sarvatobhadra81Control";
			this.Size = new System.Drawing.Size(512, 376);
			this.Resize += new System.EventHandler(this.Sarvatobhadra81Control_Resize);
			this.Load += new System.EventHandler(this.Sarvatobhadra81Control_Load);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.Sarvatobhadra81Control_Paint);

		}
		#endregion

		private void Sarvatobhadra81Control_Load(object sender, System.EventArgs e)
		{
		
		}
		public void OnRedisplay (object o)
		{
			f = new Font (MhoraGlobalOptions.Instance.GeneralFont.FontFamily,
				MhoraGlobalOptions.Instance.GeneralFont.SizeInPoints-2);
			this.DrawToBuffer(true);
			this.Invalidate();
		}
		public void OnRecalculate (Object o)
		{
			this.Invalidate();
		}
		private Point GetItemOffset (int item)
		{
			int oi4 = (int)((float)this.bufferCellSize/(float)4);
			int oi2 = (int)((float)this.bufferCellSize/(float)2);
			switch (item)
			{
				case 4: return new Point(0, oi4*1);
				case 1: return new Point(0, oi4*2);
				case 5: return new Point(0, oi4*3);
				case 3: return new Point(oi2, oi4*1);
				case 6: return new Point(oi2, oi4*2);
				case 2: return new Point(oi2, oi4*3);
			}
			return new Point (0,0);
		}
		private Point GetItemOffsetCenter ()
		{
			return new Point(this.bufferCellSize/2, this.bufferCellSize/2);
		}
		private Point GetCell (int x, int y)
		{
			return new Point((x-1)*bufferCellSize, (y-1)*bufferCellSize);
		}
		private Point GetCellInRectangle (int width, int offset, int cellNumber)
		{
			//Debug.Assert(cellNumber < (width-2)*4, "GetCellInRectangle: Cell# outside range");

			cellNumber = cellNumber + offset;
			while (cellNumber > (width-2)*4)
				cellNumber -= (width-2)*4;

			int off = ((9-width)/2)+1;
			int nit = width-2;
			
			if (cellNumber <= nit)
				return GetCell (off+cellNumber, off);
			if (cellNumber <= 2*nit)
			{
				cellNumber = cellNumber-(nit);
				return GetCell (9-off+1, off+cellNumber);
			}
			if (cellNumber <= 3*nit)
			{
				cellNumber = cellNumber-(2*nit);
				return GetCell (9-off-cellNumber+1, 9-off+1);
			}
			if (cellNumber <= 4*nit)
			{
				cellNumber = cellNumber-(3*nit);
				return GetCell(off, 9-off-cellNumber+1);
			}
			return new Point(0,0);

		}
		private void DrawMoveableText (Graphics g)
		{
			Font f = new Font (MhoraGlobalOptions.Instance.GeneralFont.FontFamily,
				MhoraGlobalOptions.Instance.GeneralFont.SizeInPoints-2);

			int[] items = new int[29];
			for (int i=0; i<29; i++)
				items[i] = 0;
			foreach (BodyPosition bp in h.positionList)
			{
				if (bp.type != BodyType.Name.Graha &&
					bp.type != BodyType.Name.Lagna) continue;

				Nakshatra28 n = bp.longitude.toNakshatra28();
				items[(int)n.value]++;
				Point pxBase = GetCellInRectangle(9, 26, (int)n.value);
				Point pxOff = GetItemOffset(items[(int)n.value]);
				Point px = new Point(pxBase.X + pxOff.X, pxBase.Y+pxOff.Y);
				string s = Body.toShortString(bp.name);
				g.DrawString(s, f, Brushes.Maroon, px.X, px.Y);
			}
		}
		private void DrawTithiItem (Graphics g, int x, int y, int item, string s)
		{

			Brush b = Brushes.DarkGreen;
			Point pxBase = GetCell(x, y);
			Point pxOff = GetItemOffset(item);
			Point px = new Point(pxBase.X + pxOff.X, pxBase.Y+pxOff.Y);
			SizeF sz = g.MeasureString(s, f);
			g.DrawString(s, f, b, px.X, px.Y);
		}
		private void DrawFixedText (Graphics g)
		{
			Font f_sounds = new Font(MhoraGlobalOptions.Instance.GeneralFont.FontFamily,
				MhoraGlobalOptions.Instance.GeneralFont.SizeInPoints-2);
			Font f_sanskrit = new Font("Sanskrit 99", 
				MhoraGlobalOptions.Instance.GeneralFont.SizeInPoints+5);
			for (int i=1; i<=12; i++)
			{
				ZodiacHouse zh = new ZodiacHouse(ZodiacHouse.Name.Ari).add(i);
				Point pxBase = GetCellInRectangle (5, 11, i);
				Point pxOff = GetItemOffsetCenter();
				Point px = new Point(pxBase.X + pxOff.X, pxBase.Y + pxOff.Y);
				string s = zh.value.ToString();
				SizeF sz = g.MeasureString(s, f);
				g.DrawString(s, f, Brushes.Purple, px.X-sz.Width/2, px.Y-sz.Height/2);
			}
			for (int i=1; i<=28; i++)
			{
				Nakshatra28 na = new Nakshatra28(Nakshatra28.Name.Aswini).add(i);
				Point pxBase = GetCellInRectangle (9, 26, i);
				Point pxOff = GetItemOffsetCenter();
				Point px = new Point(pxBase.X + pxOff.X, pxBase.Y);
				string s = na.value.ToString().Substring(0, 3);
				SizeF sz = g.MeasureString(s, f);
				g.DrawString(s, f, Brushes.DarkGreen, px.X-sz.Width/2, px.Y);
			}

			{
				Point pxOff = GetItemOffsetCenter();

				Brush b = Brushes.DarkGreen;
				Point pxBase = GetCell(5, 4);
				Point px = new Point(pxBase.X + pxOff.X, pxBase.Y);
				string s = "Nanda";
				SizeF sz = g.MeasureString(s, f);
				g.DrawString(s, f, b, px.X-sz.Width/2, px.Y);

				pxBase = GetCell(4, 5);
				px = new Point(pxBase.X + pxOff.X, pxBase.Y);
				s = "Rikta";
				sz = g.MeasureString(s, f);
				g.DrawString(s, f, b, px.X-sz.Width/2, px.Y);

				pxBase = GetCell(5, 5);
				px = new Point(pxBase.X + pxOff.X, pxBase.Y);
				s = "Poorna";
				sz = g.MeasureString(s, f);
				g.DrawString(s, f, b, px.X-sz.Width/2, px.Y);

				pxBase = GetCell(6, 5);
				px = new Point(pxBase.X + pxOff.X, pxBase.Y);
				s = "Bhadra";
				sz = g.MeasureString(s, f);
				g.DrawString(s, f, b, px.X-sz.Width/2, px.Y);

				pxBase = GetCell(5, 6);
				px = new Point(pxBase.X + pxOff.X, pxBase.Y);
				s = "Jaya";
				sz = g.MeasureString(s, f);
				g.DrawString(s, f, b, px.X-sz.Width/2, px.Y);

				DrawTithiItem (g, 5, 4, 1, "Sun");
				DrawTithiItem (g, 5, 4, 6, "Tue");
				DrawTithiItem (g, 4, 5, 1, "Fri");
				DrawTithiItem (g, 5, 5, 1, "Sat");
				DrawTithiItem (g, 6, 5, 1, "Mon");
				DrawTithiItem (g, 6, 5, 6, "Wed");
				DrawTithiItem (g, 5, 6, 1, "Thu");

				DrawTithiItem (g, 5, 4, 4, "1,6,11");
				DrawTithiItem (g, 4, 5, 4, "4,9,14");
				DrawTithiItem (g, 5, 5, 4, "5,10,15");
				DrawTithiItem (g, 5, 6, 4, "3,8,13");
				DrawTithiItem (g, 6, 5, 4, "2,7,12");
				
			}
			string[][] strs = new string[][]
			{
				new string[] { "a","","","","","","","","aa" },
				new string[] { "", "u", "a", "va", "ka", "ha", "da", "uu", "" },
				new string[] { "", "la", "lu", "", "", "", "lu", "ma", "" },
				new string[] { "", "cha", "", "o", "", "au", "", "Ta", "" },
				new string[] {"", "da", "", "", "", "", "", "pa", ""},
				new string[] {"", "sa", "", "ah", "", "an", "", "ra", "" },
				new string[] {"", "ga", "ai", "", "", "", "e", "ta", "" },
				new string[] {"", "r^ii", "kha", "ja", "ba", "ya", "na", "r^i", "" },
				new string[] {"ii", "", "", "", "", "", "", "", "i"}
			};

			string[][] strs_san = new string[][]
			{
				new string[] { "A","","","","","","","","Aa" },
				new string[] { "", "%", "A", "v", "k", "h", "d", "^", "" },
				new string[] { "", "l", "l&", "", "", "", "l&", "m", "" },
				new string[] { "", "c", "", "Aae", "", "AaE", "", "q", "" },
				new string[] {"", "d", "", "", "", "", "", "p", ""},
				new string[] {"", "s", "", "A>", "", "A<", "", "r", "" },
				new string[] {"", "g", "@e", "", "", "", "@", "t", "" },
				new string[] {"", "\\", "o", "j", "b", "y", "n", "\\", "" },
				new string[] {"$", "", "", "", "", "", "", "", "#"}
			};

			for (int i=0; i<9; i++) 
			{
				for (int j=0; j<9; j++)
				{
					Point pxBase = this.GetCell(j+1, i+1);
					Point pxOff = GetItemOffsetCenter();
					Point px = new Point(pxBase.X + pxOff.X, pxBase.Y + pxOff.Y);
					string s = strs_san[i][j];
					SizeF sz = g.MeasureString(s, f);
					g.DrawString(s, f_sanskrit, Brushes.CadetBlue, px.X-sz.Width/2, px.Y-sz.Height*(float)1.5);
				}
			}

		}

		private void DrawChakra (Graphics g)
		{
			int bufferSize = (bufferCellSize*9)+bufferOffset*2;
			int size = Math.Min(bmpBuffer.Width, bmpBuffer.Height);
			float scale = (float)size/bufferSize;


			g.Clear(MhoraGlobalOptions.Instance.ChakraBackgroundColor);
			g.ResetTransform();
			g.TranslateTransform(bufferOffset, bufferOffset);
			g.ScaleTransform(scale, scale);
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;


			for (int i=0; i<=9; i++)
			{
				g.DrawLine(pn_grey, bufferCellSize*i, 0, bufferCellSize*i, bufferCellSize*9);
				g.DrawLine(pn_grey, 0, bufferCellSize*i, bufferCellSize*9, bufferCellSize*i);
			}

			DrawFixedText (g);
			DrawMoveableText (g);

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
		protected override void copyToClipboard ()
		{
			Clipboard.SetDataObject(this.bmpBuffer);
		}

		private void Sarvatobhadra81Control_Resize(object sender, System.EventArgs e)
		{
			this.DrawToBuffer(true);
			this.Invalidate();			
		
		}

		private void Sarvatobhadra81Control_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			this.DrawChakra(e.Graphics);
		}
	}
}

