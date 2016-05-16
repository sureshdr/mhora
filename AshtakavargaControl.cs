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
using System.IO;
using System.Runtime.Serialization;

namespace mhora
{
	public class AshtakavargaControl : mhora.MhoraControl
	{
		public enum EChartStyle { SouthIndian, EastIndian };
		public enum EDisplayStyle { Chancha, NavaSav };
		public enum ESavType { Normal, Rao };

		public class AshtakavargaOptions: ICloneable
		{
			private Division mDtype;
			private EChartStyle mChartStyle;
			private ESavType mSavType;
			
			public AshtakavargaOptions ()
			{
				this.mDtype = new Division(Basics.DivisionType.Rasi);
				this.mChartStyle = (EChartStyle)MhoraGlobalOptions.Instance.VargaStyle;
			}

			[PGNotVisible]
			public Division VargaType
			{
				get { return this.mDtype; }
				set { this.mDtype = value; }
			}

			[PGDisplayName("Varga Type")]
			public Basics.DivisionType UIVargaType
			{
				get { return this.mDtype.MultipleDivisions[0].Varga; }
				set { this.mDtype = new Division(value); }
			}

			[PGDisplayName("SAV Type")]
			public ESavType SavType
			{
				get { return this.mSavType; }
				set { this.mSavType = value; }
			}

			public EChartStyle ChartStyle
			{
				get { return this.mChartStyle; }
				set { this.mChartStyle = value; }
			}
			public object Clone ()
			{
				AshtakavargaOptions ao = new AshtakavargaOptions();
				ao.mDtype = this.mDtype;
				ao.mChartStyle = this.mChartStyle;
				ao.mSavType = this.mSavType;
				return ao;
			}
			public void SetOptions (AshtakavargaOptions ao)
			{
				this.mDtype = ao.mDtype;
				this.mChartStyle = ao.mChartStyle;
				this.mSavType = ao.mSavType;
			}
		}


		private System.ComponentModel.IContainer components = null;
		private System.Windows.Forms.ContextMenu contextMenu;
		private System.Windows.Forms.MenuItem menuSav;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;
		private System.Windows.Forms.MenuItem menuPavSun;

		private System.Windows.Forms.MenuItem menuPavMoon;
		private System.Windows.Forms.MenuItem menuPavMars;
		private System.Windows.Forms.MenuItem menuPavMercury;
		private System.Windows.Forms.MenuItem menuPavJupiter;
		private System.Windows.Forms.MenuItem menuPavVenus;
		private System.Windows.Forms.MenuItem menuPavSaturn;
		private System.Windows.Forms.MenuItem menuPavLagna;

		Ashtakavarga av = null;
		Body.Name[] outerBodies = null;
		Body.Name[] innerBodies = null;
		private System.Windows.Forms.MenuItem menuOptions;
		Bitmap bmpBuffer = null;
		private System.Windows.Forms.MenuItem menuJhoraSav;
		AshtakavargaOptions userOptions = null;
		EDisplayStyle mDisplayStyle = EDisplayStyle.Chancha;
		Font fBig = null;
		Font fBigBold = null;
		Brush b_black = null;
		Brush b_red = null;
		public AshtakavargaControl(Horoscope _h)
		{
			// This call is required by the Windows Form Designer.
			InitializeComponent();
			userOptions = new AshtakavargaOptions();
			h = _h;
			h.Changed += new EvtChanged(this.OnRecalculate);
			MhoraGlobalOptions.DisplayPrefsChanged += new EvtChanged(this.onRedisplay);
			av = new Ashtakavarga(h, userOptions.VargaType);
			outerBodies = new Body.Name[]
			{
				Body.Name.Sun, Body.Name.Moon, Body.Name.Mars,
				Body.Name.Mercury, Body.Name.Jupiter, Body.Name.Venus,
				Body.Name.Saturn, Body.Name.Lagna
			};
				
			b_black = new SolidBrush(Color.Black);

			innerBodies = (Body.Name[]) outerBodies.Clone();
			this.resetContextMenuChecks(this.menuSav);
			this.onRedisplay(MhoraGlobalOptions.Instance);
		}

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// 

		private void onRedisplay (object o)
		{
			this.userOptions.ChartStyle = (EChartStyle)MhoraGlobalOptions.Instance.VargaStyle;
			fBig = new Font(MhoraGlobalOptions.Instance.GeneralFont.FontFamily,
				MhoraGlobalOptions.Instance.GeneralFont.SizeInPoints+3);
			fBigBold = new Font(MhoraGlobalOptions.Instance.GeneralFont.FontFamily,
				MhoraGlobalOptions.Instance.GeneralFont.SizeInPoints+3, 
				System.Drawing.FontStyle.Bold | System.Drawing.FontStyle.Underline |
				System.Drawing.FontStyle.Italic);
			b_red = new SolidBrush(MhoraGlobalOptions.Instance.VargaGrahaColor);
			this.DrawToBuffer();
			this.Invalidate();
		}
		private void OnRecalculate (Object _h)
		{
			av = new Ashtakavarga(h, userOptions.VargaType);
			this.DrawToBuffer();
			this.Invalidate();
		}

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
			this.menuOptions = new System.Windows.Forms.MenuItem();
			this.menuJhoraSav = new System.Windows.Forms.MenuItem();
			this.menuSav = new System.Windows.Forms.MenuItem();
			this.menuPavSun = new System.Windows.Forms.MenuItem();
			this.menuPavMoon = new System.Windows.Forms.MenuItem();
			this.menuPavMars = new System.Windows.Forms.MenuItem();
			this.menuPavMercury = new System.Windows.Forms.MenuItem();
			this.menuPavJupiter = new System.Windows.Forms.MenuItem();
			this.menuPavVenus = new System.Windows.Forms.MenuItem();
			this.menuPavSaturn = new System.Windows.Forms.MenuItem();
			this.menuPavLagna = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			// 
			// contextMenu
			// 
			this.contextMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						this.menuOptions,
																						this.menuJhoraSav,
																						this.menuSav,
																						this.menuPavSun,
																						this.menuPavMoon,
																						this.menuPavMars,
																						this.menuPavMercury,
																						this.menuPavJupiter,
																						this.menuPavVenus,
																						this.menuPavSaturn,
																						this.menuPavLagna,
																						this.menuItem1,
																						this.menuItem2});
			// 
			// menuOptions
			// 
			this.menuOptions.Index = 0;
			this.menuOptions.Text = "Options";
			this.menuOptions.Click += new System.EventHandler(this.menuOptions_Click);
			// 
			// menuJhoraSav
			// 
			this.menuJhoraSav.Index = 1;
			this.menuJhoraSav.Text = "SAV, PAV";
			this.menuJhoraSav.Click += new System.EventHandler(this.menuJhoraSav_Click);
			// 
			// menuSav
			// 
			this.menuSav.Index = 2;
			this.menuSav.Text = "SAV, PAV, BAV";
			this.menuSav.Click += new System.EventHandler(this.menuSav_Click);
			// 
			// menuPavSun
			// 
			this.menuPavSun.Index = 3;
			this.menuPavSun.Text = "PAV - Sun";
			this.menuPavSun.Click += new System.EventHandler(this.menuPavSun_Click);
			// 
			// menuPavMoon
			// 
			this.menuPavMoon.Index = 4;
			this.menuPavMoon.Text = "PAV - Moon";
			this.menuPavMoon.Click += new System.EventHandler(this.menuPavMoon_Click);
			// 
			// menuPavMars
			// 
			this.menuPavMars.Index = 5;
			this.menuPavMars.Text = "PAV - Mars";
			this.menuPavMars.Click += new System.EventHandler(this.menuPavMars_Click);
			// 
			// menuPavMercury
			// 
			this.menuPavMercury.Index = 6;
			this.menuPavMercury.Text = "PAV - Mercury";
			this.menuPavMercury.Click += new System.EventHandler(this.menuPavMercury_Click);
			// 
			// menuPavJupiter
			// 
			this.menuPavJupiter.Index = 7;
			this.menuPavJupiter.Text = "PAV - Jupiter";
			this.menuPavJupiter.Click += new System.EventHandler(this.menuPavJupiter_Click);
			// 
			// menuPavVenus
			// 
			this.menuPavVenus.Index = 8;
			this.menuPavVenus.Text = "PAV - Venus";
			this.menuPavVenus.Click += new System.EventHandler(this.menuPavVenus_Click);
			// 
			// menuPavSaturn
			// 
			this.menuPavSaturn.Index = 9;
			this.menuPavSaturn.Text = "PAV - Saturn";
			this.menuPavSaturn.Click += new System.EventHandler(this.menuPavSaturn_Click);
			// 
			// menuPavLagna
			// 
			this.menuPavLagna.Index = 10;
			this.menuPavLagna.Text = "PAV - Lagna";
			this.menuPavLagna.Click += new System.EventHandler(this.menuPavLagna_Click);
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 11;
			this.menuItem1.Text = "-";
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 12;
			this.menuItem2.Text = "-";
			// 
			// AshtakavargaControl
			// 
			this.AllowDrop = true;
			this.ContextMenu = this.contextMenu;
			this.Name = "AshtakavargaControl";
			this.Size = new System.Drawing.Size(208, 128);
			this.DragEnter += new System.Windows.Forms.DragEventHandler(this.AshtakavargaControl_DragEnter);
			this.Resize += new System.EventHandler(this.AshtakavargaControl_Resize);
			this.Load += new System.EventHandler(this.AshtakavargaControl_Load);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.AshtakavargaControl_Paint);
			this.DragDrop += new System.Windows.Forms.DragEventHandler(this.AshtakavargaControl_DragDrop);

		}
		#endregion

		private void AshtakavargaControl_Load(object sender, System.EventArgs e)
		{
			this.AddViewsToContextMenu (this.contextMenu);
		}


		private void DrawJhoraChakra (Graphics g)
		{
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;
			if (false == this.PrintMode)
				g.Clear(this.BackColor);
			int offset = 5;
			int size = (Math.Min(this.bmpBuffer.Width, this.bmpBuffer.Height) / 3)-10;	
			IDrawChart dc = null;
			switch (this.userOptions.ChartStyle)
			{
				default:
				case EChartStyle.EastIndian:
					dc = new EastIndianChart();
					break;
				case EChartStyle.SouthIndian:
					dc = new SouthIndianChart();
					break;
			}

			Body.Name[] bin_body = new Body.Name[] {
											    Body.Name.Lagna,
												Body.Name.Lagna, Body.Name.Sun, Body.Name.Moon,
												Body.Name.Mars, Body.Name.Mercury, Body.Name.Jupiter,
												Body.Name.Venus, Body.Name.Saturn };
			int[][] bins = new int[9][];

			if (userOptions.SavType == ESavType.Normal)
				bins[0] = av.getSav();
			else
				bins[0] = av.getSavRao();

			bins[1] = av.getPav(Body.Name.Lagna);
			bins[2] = av.getPav(Body.Name.Sun);
			bins[3] = av.getPav(Body.Name.Moon);
			bins[4] = av.getPav(Body.Name.Mars);
			bins[5] = av.getPav(Body.Name.Mercury);
			bins[6] = av.getPav(Body.Name.Jupiter);
			bins[7] = av.getPav(Body.Name.Venus);
			bins[8] = av.getPav(Body.Name.Saturn);

			string[] strs = new string[9];
			strs[0] = "SAV";
			strs[1] = Body.toString(Body.Name.Lagna);
			strs[2] = Body.toString(Body.Name.Sun);
			strs[3] = Body.toString(Body.Name.Moon);
			strs[4] = Body.toString(Body.Name.Mars);
			strs[5] = Body.toString(Body.Name.Mercury);
			strs[6] = Body.toString(Body.Name.Jupiter);
			strs[7] = Body.toString(Body.Name.Venus);
			strs[8] = Body.toString(Body.Name.Saturn);

			Brush b_background = new SolidBrush(MhoraGlobalOptions.Instance.ChakraBackgroundColor);
			for (int i=0; i<3; i++)
			{
				for (int j=0; j<3; j++)
				{
					g.ResetTransform();
					g.TranslateTransform(i*size+((i+1)*offset), j*size+((j+1)*offset));
					float scale = (float)size / (float)dc.GetLength();
					g.ScaleTransform(scale, scale);
					if (false == this.PrintMode)
						g.FillRectangle(b_background, 0, 0, dc.GetLength(), dc.GetLength());
					dc.DrawOutline(g);
					int off = j*3+i;
					int[] bin = bins[off];
					Debug.Assert(bin.Length == 12, "PAV/SAV: unknown size");
					for (int z=0; z<12; z++)
					{
						Font f = fBig;
						int zh = (int)h.getPosition(bin_body[off]).toDivisionPosition(userOptions.VargaType).zodiac_house.value;
						if (z == zh-1)
							f = fBigBold;
						Point p = dc.GetSingleItemOffset(new ZodiacHouse((ZodiacHouse.Name)z+1));
						g.DrawString(bin[z].ToString(), f, b_black, p);						
					}
					SizeF sz = g.MeasureString(strs[off], fBig);
					g.DrawString(strs[off], fBig, b_red, 100-sz.Width/2, 100-sz.Height/2);

					if (off == 0 && userOptions.SavType == ESavType.Rao)
					{
						sz = g.MeasureString("Rao", fBig);
						g.DrawString("Rao", fBig, b_red, 100-sz.Width/2, 120-sz.Height/2);
					}
				}
			}
		}

		private Image DrawToBuffer ()
		{
			if (bmpBuffer != null)
				bmpBuffer.Dispose();
			if (this.Width == 0 || this.Height == 0)
				return bmpBuffer;
			Graphics displayGraphics = this.CreateGraphics();
			bmpBuffer = new Bitmap(this.Width, this.Height, displayGraphics);
			Graphics imageGraphics = Graphics.FromImage(bmpBuffer);

			switch (this.mDisplayStyle)
			{
				case EDisplayStyle.Chancha:
					DrawChanchaChakra (imageGraphics);			
					break;
				case EDisplayStyle.NavaSav:
					DrawJhoraChakra (imageGraphics);
					break;
			}
			
			displayGraphics.Dispose();
			return bmpBuffer;
		}

		public Bitmap DrawChanchaToImage (int size)
		{
			bmpBuffer = new Bitmap(size, size);
			Graphics imageGraphics = Graphics.FromImage(bmpBuffer);
			DrawChanchaChakra (imageGraphics);
			return bmpBuffer;
		}
		public Bitmap DrawNavaChakrasToImage (int size)
		{
			bmpBuffer = new Bitmap(size, size);
			Graphics imageGraphics = Graphics.FromImage(bmpBuffer);
			DrawJhoraChakra (imageGraphics);
			return bmpBuffer;
		}

		private void AshtakavargaControl_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			e.Graphics.DrawImage(bmpBuffer,0, 0);
		}

		private bool ChanchaReset (Graphics g, int ray)
		{
			int outerSize = Math.Min(this.bmpBuffer.Width, this.bmpBuffer.Height);
			float scaleOuter = (float)outerSize / 200;

			g.ResetTransform();
			g.TranslateTransform(this.bmpBuffer.Width/2, this.bmpBuffer.Height/2);
			g.ScaleTransform(scaleOuter, scaleOuter);

			int numEntries = 8*12;
			float rotDegree = (float)360.0 / (float)numEntries;
			float rotTotal = 0;

			switch (this.userOptions.ChartStyle)
			{
				case EChartStyle.SouthIndian:
					rotTotal = rotDegree * ray - 120;
					g.RotateTransform(rotTotal);
					break;
				case EChartStyle.EastIndian:
					rotTotal = -1*rotDegree*ray-75-rotDegree;
					g.RotateTransform(rotTotal);
					break;
			}

			while (rotTotal < 0) rotTotal += 360;
			while (rotTotal > 360) rotTotal -= 360;

			switch (this.userOptions.ChartStyle)
			{
				case EChartStyle.SouthIndian:
					if ((0<=rotTotal && rotTotal <90) || 
						(270<=rotTotal && rotTotal<360))
						return true;
					return false;
				case EChartStyle.EastIndian:
					if ((0<=rotTotal && rotTotal <105) || 
						(285<=rotTotal && rotTotal<360))
						return true;
					return false;
			}

			return true;
		}

		private void DrawChanchaInner (Graphics g)
		{

			IDrawChart dc = null;
			switch (this.userOptions.ChartStyle)
			{
				default:
				case EChartStyle.EastIndian:
					dc = new EastIndianChart();
					break;
				case EChartStyle.SouthIndian:
					dc = new SouthIndianChart();
					break;
			}
			int innerSize = (int)((float)Math.Min(this.bmpBuffer.Width, this.bmpBuffer.Height) / 3.15);	
			float scaleInner = (float)innerSize / (float)dc.GetLength();

			g.ResetTransform();
			g.TranslateTransform(this.bmpBuffer.Width/2, this.bmpBuffer.Height/2);
			g.TranslateTransform(-1*innerSize/2, -1*innerSize/2);
			g.ScaleTransform(scaleInner, scaleInner);

			dc.DrawOutline(g);

			int[] inner_bindus;

			if (this.outerBodies.Length > 1) 
			{
				if (this.userOptions.SavType == ESavType.Rao)
					inner_bindus = av.getSavRao();
				else
					inner_bindus = av.getSav();
			}
			else 
				inner_bindus = av.getPav(this.outerBodies[0]);

			for (int i=0; i<12; i++)
			{
				ZodiacHouse zh = new ZodiacHouse((ZodiacHouse.Name)i+1);
				Point p = dc.GetSingleItemOffset(zh);
				g.DrawString(inner_bindus[i].ToString(), fBig, b_black, p);
			}

			string av_desc = "SAV";
			if (outerBodies.Length == 1) av_desc = "PAV";
			SizeF sz = g.MeasureString(av_desc, fBig);
			g.DrawString(av_desc, fBig, b_black, 100-(sz.Width/2), 80-(sz.Height/2));

			if (outerBodies.Length == 1) {
				string desc = Body.toString(outerBodies[0]);
				sz = g.MeasureString(desc, fBig);
				g.DrawString(desc, fBig, b_black, 100-(sz.Width/2), 120-(sz.Height/2));
			}

			if (this.userOptions.SavType == ESavType.Rao)
			{
				string desc = "Rao";
				sz = g.MeasureString(desc, fBig);
				g.DrawString(desc, fBig, b_black, 100-(sz.Width/2), 120-(sz.Height/2));
			}

			{
				string desc = Basics.numPartsInDivisionString(this.userOptions.VargaType);;
				sz = g.MeasureString(desc, fBig);
				g.DrawString(desc, fBig, b_black, 100-(sz.Width/2), 100-(sz.Height/2));
			}

		}

		public bool PrintMode = false;
		private void DrawChanchaChakra(Graphics g)
		{
			string[] sBindus = new string[] {"Su", "Mo", "Ma", "Me", "Ju", "Ve", "Sa", "As"};
			Pen pn_black = new Pen(Color.Black, (float)0.01);
			Pen pn_grey = new Pen(Color.LightGray, (float)0.01);
			Pen pn_dgrey = new Pen(Color.Gray, (float)0.01);
			Brush b_black = new SolidBrush(Color.Black);
			Brush b_red = new SolidBrush(Color.Red);
			Font f = new Font(MhoraGlobalOptions.Instance.FixedWidthFont.FontFamily, 
				MhoraGlobalOptions.Instance.FixedWidthFont.SizeInPoints-6);

			
			g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.HighQuality;

			if (this.PrintMode == false)
				g.Clear(MhoraGlobalOptions.Instance.ChakraBackgroundColor);

			this.DrawChanchaInner(g);


			// inner and outer bounding circles
			this.ChanchaReset(g, 0);
			for (int i=1; i<=8; i++)
			{
				int w = 45+i*4;
				g.DrawEllipse(pn_grey, -w, -w, w*2, w*2);
			}
			g.DrawEllipse(pn_black, -45, -45, 90, 90);
			g.DrawEllipse(pn_black, -85, -85, 85*2, 85*2);
			g.DrawEllipse(pn_black, -98, -98, 98*2, 98*2);


			// draw per-spoke stuff: spoke, bindus
			int numEntries = 8*12;
			float rotDegree = (float)360.0 / (float)numEntries;
			for (int i=0; i<numEntries; i++)
			{
				bool bDir = this.ChanchaReset(g, i);
				Pen p = pn_grey;
				if (i%8==7 && this.userOptions.ChartStyle == EChartStyle.EastIndian) p = pn_black;
				if (i%8==0 && this.userOptions.ChartStyle == EChartStyle.SouthIndian) p = pn_black;
				g.DrawLine(p, 45, 0, 98, 0);

				Brush b = b_black;
				//if (this.outerBodies.Length == 1 &&	av.BodyToInt(this.outerBodies[0]) == i%8)
				//	b = b_red;
				if (this.outerBodies.Length > 1)
				{
					if (bDir == true)
						g.DrawString(sBindus[i%8], f, b, 49+9*4, 0);
					else
					{
						g.ScaleTransform((float)-1.0, (float)-1.0);
						g.DrawString(sBindus[i%8], f, b, -1*(49+11*4), -6);
					}
				}
				//g.DrawString(i.ToString(), f, b, 49+9*4, 0);
			}

			// write the pav values at the top of the circle
			foreach (Body.Name bOuter in outerBodies)
			{
				int[] pav = av.getPav(bOuter);
				for (int i=0; i<12; i++)
				{
					int iRing = i*8+av.BodyToInt(bOuter);
					bool bDir = this.ChanchaReset(g, iRing);
					SizeF sz = g.MeasureString(pav[i].ToString(), f);
					if (true == bDir)
						g.DrawString(pav[i].ToString(), f, b_black, 49+7*4, 0);
					else 
					{
						g.ScaleTransform((float)-1.0, (float)-1.0);
						g.DrawString(pav[i].ToString(), f, b_black,
							new RectangleF(-1*(49+7*4+sz.Width), -1*(sz.Height-1), sz.Width, sz.Height));
					}

				}
			}

			// draw the bindus
			foreach (Body.Name bOuter in outerBodies)
			{
				foreach (Body.Name bInner in innerBodies)
				{
					int iOuter = av.BodyToInt(bOuter);
					int iInner = av.BodyToInt(bInner);
					ZodiacHouse.Name[] zhBins = av.getBindus(bOuter, bInner);
					Brush br = new SolidBrush(MhoraGlobalOptions.Instance.getBinduColor(bInner));

					foreach (ZodiacHouse.Name zh in zhBins)
					{
						int iRing = (((int)zh)-1)*8+iInner;
						bool bDir = this.ChanchaReset(g, iRing);
						g.FillEllipse(br, 50+(iOuter-1)*4, 1, 2, 2);
						g.DrawEllipse(pn_dgrey, 50+(iOuter-1)*4, 1, 2, 2);

						if (this.outerBodies.Length == 1) 
						{
							if (true == bDir)
								g.DrawString(sBindus[iRing%8], f, b_black, 49+9*4, 0);
							else 
							{
								g.ScaleTransform((float)-1.0, (float)-1.0);
								g.DrawString(sBindus[iRing%8], f, b_black, -1*(49+11*4), -6);
							}
						}
					}
				}
			}


	}

		private void menuSav_Click(object sender, System.EventArgs e)
		{
			outerBodies = new Body.Name[]
			{
				Body.Name.Sun, Body.Name.Moon, Body.Name.Mars,
				Body.Name.Mercury, Body.Name.Jupiter, Body.Name.Venus,
				Body.Name.Saturn, Body.Name.Lagna
			};		
			this.mDisplayStyle = EDisplayStyle.Chancha;
			this.DrawToBuffer();
			this.Invalidate();
			this.resetContextMenuChecks(this.menuSav);
		}

		private void menuPavSun_Click(object sender, System.EventArgs e)
		{
			this.outerBodies = new Body.Name[] { Body.Name.Sun };
			this.mDisplayStyle = EDisplayStyle.Chancha;
			this.DrawToBuffer();
			this.Invalidate();
			this.resetContextMenuChecks(this.menuPavSun);
		}

		private void menuPavMoon_Click(object sender, System.EventArgs e)
		{
			this.outerBodies = new Body.Name[] { Body.Name.Moon };
			this.mDisplayStyle = EDisplayStyle.Chancha;
			this.DrawToBuffer();
			this.Invalidate();		
			this.resetContextMenuChecks(this.menuPavMoon);
		}

		private void menuPavJupiter_Click(object sender, System.EventArgs e)
		{
			this.outerBodies = new Body.Name[] { Body.Name.Jupiter };
			this.mDisplayStyle = EDisplayStyle.Chancha;
			this.DrawToBuffer();
			this.Invalidate();				
			this.resetContextMenuChecks(this.menuPavJupiter);
		}

		private void menuPavMars_Click(object sender, System.EventArgs e)
		{
			this.outerBodies = new Body.Name[] { Body.Name.Mars };
			this.mDisplayStyle = EDisplayStyle.Chancha;
			this.DrawToBuffer();
			this.Invalidate();				
			this.resetContextMenuChecks(this.menuPavMars);
		}

		private void menuPavMercury_Click(object sender, System.EventArgs e)
		{
			this.outerBodies = new Body.Name[] { Body.Name.Mercury };
			this.mDisplayStyle = EDisplayStyle.Chancha;
			this.DrawToBuffer();
			this.Invalidate();			
			this.resetContextMenuChecks(this.menuPavMercury);
		}

		private void menuPavVenus_Click(object sender, System.EventArgs e)
		{
			this.outerBodies = new Body.Name[] { Body.Name.Venus };
			this.mDisplayStyle = EDisplayStyle.Chancha;
			this.DrawToBuffer();
			this.Invalidate();		
			this.resetContextMenuChecks(this.menuPavVenus);
		}

		private void menuPavSaturn_Click(object sender, System.EventArgs e)
		{
			this.outerBodies = new Body.Name[] { Body.Name.Saturn };
			this.mDisplayStyle = EDisplayStyle.Chancha;
			this.DrawToBuffer();
			this.Invalidate();		
			this.resetContextMenuChecks(this.menuPavSaturn);
		}

		private void menuPavLagna_Click(object sender, System.EventArgs e)
		{
			this.outerBodies = new Body.Name[] { Body.Name.Lagna };
			this.mDisplayStyle = EDisplayStyle.Chancha;
			this.DrawToBuffer();
			this.Invalidate();		
			this.resetContextMenuChecks(this.menuPavLagna);				
		}

		private void AshtakavargaControl_Resize(object sender, System.EventArgs e)
		{
			this.DrawToBuffer();
			this.Invalidate();
		}
		protected override void copyToClipboard ()
		{
			Clipboard.SetDataObject(this.bmpBuffer);
		}

		private object SetOptions (Object o)
		{
			AshtakavargaOptions ao = (AshtakavargaOptions)o;
			if (ao.VargaType != this.userOptions.VargaType)
				av = new Ashtakavarga(h, ao.VargaType);
			this.userOptions.SetOptions(ao);
			this.DrawToBuffer();
			this.Invalidate();
			return this.userOptions.Clone();
		}
		private void menuOptions_Click(object sender, System.EventArgs e)
		{
			MhoraOptions f = new MhoraOptions(this.userOptions.Clone(), new ApplyOptions(SetOptions));
			f.ShowDialog();
		}

		private void menuJhoraSav_Click(object sender, System.EventArgs e)
		{
			this.mDisplayStyle = EDisplayStyle.NavaSav;
			this.DrawToBuffer();
			this.Invalidate();
			this.resetContextMenuChecks(this.menuJhoraSav);		
		}

		private void resetContextMenuChecks (MenuItem mi)
		{ 
			this.menuJhoraSav.Checked = false;
			this.menuSav.Checked = false;
			this.menuPavLagna.Checked = false;
			this.menuPavSun.Checked = false;
			this.menuPavMoon.Checked = false;
			this.menuPavMars.Checked = false;
			this.menuPavMercury.Checked = false;
			this.menuPavJupiter.Checked = false;
			this.menuPavVenus.Checked = false;
			this.menuPavSaturn.Checked = false;
			mi.Checked = true;
		}

		private void AshtakavargaControl_DragEnter(object sender, System.Windows.Forms.DragEventArgs e)
		{
			if (e.Data.GetDataPresent(typeof(mhora.DivisionalChart))) 
				e.Effect = DragDropEffects.Copy;
			else
				e.Effect = DragDropEffects.None;		
		}

		private void AshtakavargaControl_DragDrop(object sender, System.Windows.Forms.DragEventArgs e)
		{
			if (e.Data.GetDataPresent(typeof(mhora.DivisionalChart)))
			{
				Division div = Division.CopyFromClipboard();
				if (null == div) return;
				this.userOptions.VargaType = div;
				this.SetOptions(this.userOptions);
				this.OnRecalculate(h);
			}
		}
	}
}

