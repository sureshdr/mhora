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
using System.Drawing;
using System.Drawing.Printing;
using System.IO;
using System.Threading;

namespace mhora
{
	
	public class MhoraPrintDocument: PrintDocument
	{
		protected Horoscope h;
		public MhoraPrintDocument (Horoscope _h)
		{
			h = _h;
		}

		ArrayList alVargas = null;
		protected override void OnBeginPrint(PrintEventArgs e)
		{
			alVargas = new ArrayList();
			for (int i = (int)Basics.DivisionType.HoraParasara; 
				i <= (int)Basics.DivisionType.DwadasamsaDwadasamsa; i++)
				alVargas.Add(new Division((Basics.DivisionType)i));

			this.numVargaPages = (int)Math.Ceiling((double)alVargas.Count / 6.0);

			base.OnBeginPrint (e);
		}

		protected override void OnEndPrint(PrintEventArgs e)
		{
			base.OnEndPrint (e);
		}

		int pageNumber = 0;
		int numVargaPages = 0;
		//int numDasaPages = 0;
		int baseNavamsaPage = 2;
		int baseChanchaPage = 3;
		int baseDasaPage = 4;
		int baseVargaPage = 8;

		protected override void OnPrintPage(PrintPageEventArgs e)
		{
			base.OnPrintPage (e);
			e.HasMorePages = true;
			pageNumber++;


			this.PrintHeader(e);

			if (pageNumber == 1)
			{
				PrintCoverPage(e);
				return;
			}

			if (pageNumber == baseDasaPage)
			{
				PrintNarayanaDasa(e);
				return;
			}
			else if (pageNumber == baseNavamsaPage)
			{
				PrintNavamsaChakra(e);
				return;
			}
			else if (pageNumber == baseChanchaPage)
			{
				PrintChanchaChakra(e);
				return;
			}
			else if (pageNumber == baseDasaPage + 1)
			{
				PrintSuDasa(e);
				return;
			}
			else if (pageNumber == baseDasaPage + 2)
			{
				PrintShoolaDasa(e);
				return;
			}
			else if (pageNumber == baseDasaPage + 3)
			{
				PrintDrigDasa(e);
				return;
			}

			if (pageNumber >= baseVargaPage && pageNumber < baseVargaPage + this.numVargaPages - 1)
			{
				PrintVargas(e);
				return;
			}

			if (pageNumber == baseVargaPage + this.numVargaPages - 1)
			{
				e.HasMorePages = false;
				PrintVargas(e);
				return;
			}

		}

		private void PrintHeader (PrintPageEventArgs e)
		{
			string s = ". ïI ram jy<.";
			e.Graphics.ResetTransform();
			SizeF sz = e.Graphics.MeasureString(s, this.f_sanskrit);
			e.Graphics.TranslateTransform (e.MarginBounds.Left, e.MarginBounds.Top);
			e.Graphics.DrawString(s, this.f_sanskrit, Brushes.Black, 
				e.MarginBounds.Width / 2 - sz.Width/2, 
				-1*f_sanskrit.Height);
			e.Graphics.ResetTransform();

			e.Graphics.ResetTransform();
			e.Graphics.TranslateTransform(e.PageBounds.Width/2, e.MarginBounds.Bottom + f.Height*2);
			s = string.Format("Page {0}", this.pageNumber);
			sz = e.Graphics.MeasureString (s, f);
			e.Graphics.DrawString(s, f, Brushes.Black, -sz.Width/2, 0);
			e.Graphics.ResetTransform();
		}
		private void PrintBody (BodyPosition bp)
		{
			Brush b = Brushes.Black;
			g.ResetTransform();
			g.TranslateTransform(left, top);
			g.DrawString(bp.name.ToString(), f, b, 0, 0);
			g.DrawString(bp.longitude.ToString(), f_fix, b, width/6, 0);

			string s = "";
			Nakshatra nak = bp.longitude.toNakshatra();
			int nak_pada = bp.longitude.toNakshatraPada();
			s = string.Format("{0} {1}", nak.toShortString(), nak_pada);
			g.DrawString(s, f, b, (float)((width/6)*2.5), 0);

			top += f.Height;
		}
		private void PrintString (string s)
		{
			g.ResetTransform();
			g.TranslateTransform(left, top);
			g.DrawString(s, f, Brushes.Black, 0, 0);
			top += f.Height;
		}


		private string GetDasaString (ToDate td, DasaEntry deAntar, bool bGraha)
		{
			string s;
			if (bGraha) 
				s = string.Format("{0} {1}", Body.toShortString(deAntar.graha),
					td.AddYears(deAntar.startUT).ToDateString());
			else
				s = string.Format("{0} {1}", ZodiacHouse.ToShortString(deAntar.zodiacHouse), 
					td.AddYears(deAntar.startUT).ToDateString());
			return s;
		}
		private void PrintDasa (IDasa id, bool bGraha)
		{
			Brush b = Brushes.Black;
			ArrayList alDasa = id.Dasa(0);
			ToDate td = new ToDate(h.baseUT, 360, 0, h);

			int num_entries_per_line = 6;
			int entry_width = width / 6;

			g.ResetTransform();
			g.TranslateTransform(left, top);
			g.DrawString(id.Description(), f_u, b, 0, 0);
			top += f.Height*2;

			foreach (DasaEntry de in alDasa)
			{
				g.ResetTransform();
				g.TranslateTransform(left, top);
				string s = "";
				if (bGraha) s = de.graha.ToString();
				else s = de.zodiacHouse.ToString();

				g.DrawString(s, f, b, 0, 0);
				ArrayList alAntar = id.AntarDasa(de);
				for (int j = 0; 
					j < (int)Math.Ceiling((double)alAntar.Count / (double)num_entries_per_line); 
					j++)
				{
					g.ResetTransform();
					g.TranslateTransform(left, top);
					for (int i = 0; i < num_entries_per_line; i++)
					{
						if (j*num_entries_per_line+i >= alAntar.Count)
							continue;
						DasaEntry deAntar = (DasaEntry)alAntar[j*num_entries_per_line+i];
						s = this.GetDasaString(td, deAntar, bGraha);
						g.DrawString(s, f_fix_s, b, (i+1)*entry_width - (float)(entry_width*.5), 0);
					}
					top += f_fix_s.Height;
				}
				top += 5;
			}

		}

		private void PrintNarayanaDasa (PrintPageEventArgs e)
		{
			left = e.MarginBounds.Left;
			top = e.MarginBounds.Top;
			width = e.MarginBounds.Width;

			IDasa id = new NarayanaDasa(h);
			g = e.Graphics;
			g.ResetTransform();
			g.TranslateTransform(left, top);
			PrintDasa (id, false);
		}
		private void PrintDrigDasa (PrintPageEventArgs e)
		{
			left = e.MarginBounds.Left;
			top = e.MarginBounds.Top;
			width = e.MarginBounds.Width;

			IDasa id = new DrigDasa(h);
			g = e.Graphics;
			g.ResetTransform();
			g.TranslateTransform(left, top);
			PrintDasa (id, false);

			VimsottariDasa vd = new VimsottariDasa(h);
			vd.options.SeedBody = VimsottariDasa.UserOptions.StartBodyType.Lagna;
			vd.SetOptions(vd.options);
			id = vd;
			PrintDasa (id, true);

		}
		private void PrintShoolaDasa (PrintPageEventArgs e)
		{
			left = e.MarginBounds.Left;
			top = e.MarginBounds.Top;
			width = e.MarginBounds.Width;

			IDasa id = new ShoolaDasa(h);
			g = e.Graphics;
			g.ResetTransform();
			g.TranslateTransform(left, top);
			PrintDasa (id, false);

			id = new NirayaanaShoolaDasa(h);
			PrintDasa (id, false);
		}
		private void PrintSuDasa (PrintPageEventArgs e)
		{
			left = e.MarginBounds.Left;
			top = e.MarginBounds.Top;
			width = e.MarginBounds.Width;

			IDasa id = new SuDasa(h);
			g = e.Graphics;
			g.ResetTransform();
			g.TranslateTransform(left, top);
			PrintDasa (id, false);

		}


		private string GetVimAntarString (ToDate td, DasaEntry de)
		{
			Moment mStart = td.AddYears(de.startUT);
			return string.Format ("{0} {1}", Body.toShortString(de.graha), mStart.ToDateString());
		}
		private void PrintVimDasa (VimsottariDasa vd)
		{
			Brush b = Brushes.Black;
			ArrayList al_dasa = vd.Dasa(0);
			ToDate td = new ToDate(h.baseUT, 360, 0, h);
			string s = "";

			g.ResetTransform();
			g.TranslateTransform(left, top);
			s = vd.Description();
			g.DrawString(s, f_u, b, 0, 0);
			top += f.Height + pad_height;

			foreach (DasaEntry de in al_dasa)
			{
				g.ResetTransform();
				g.TranslateTransform(left, top);
				Moment mStart = td.AddYears(de.StartUT);
				g.DrawString(Body.toString(de.graha), f, b, 0, 0);
				//s = string.Format("{0} ", mStart.ToDateString());
				//g.DrawString(s, f_fix, b, width / 6, 0);
				
				ArrayList al_antar = vd.AntarDasa(de);
				DasaEntry[] deAntar = (DasaEntry[])al_antar.ToArray(typeof(DasaEntry));

				int aw = width / 7;
				int off = -40;
				g.DrawString(this.GetVimAntarString(td, deAntar[0]), f_s, b, off+aw, 0);
				g.DrawString(this.GetVimAntarString(td, deAntar[1]), f_s, b, off+aw*2, 0);
				g.DrawString(this.GetVimAntarString(td, deAntar[2]), f_s, b, off+aw*3, 0);

				g.DrawString(this.GetVimAntarString(td, deAntar[3]), f_s, b, off+aw, f.Height);
				g.DrawString(this.GetVimAntarString(td, deAntar[4]), f_s, b, off+aw*2, f.Height);
				g.DrawString(this.GetVimAntarString(td, deAntar[5]), f_s, b, off+aw*3, f.Height);

				g.DrawString(this.GetVimAntarString(td, deAntar[6]), f_s, b, off+aw, f.Height*2);
				g.DrawString(this.GetVimAntarString(td, deAntar[7]), f_s, b, off+aw*2, f.Height*2);
				g.DrawString(this.GetVimAntarString(td, deAntar[8]), f_s, b, off+aw*3, f.Height*2);

				top += f.Height * 3 + 4;
			}
		}


		Graphics g = null;

		Font f_fix = new Font ("Courier New", 10);
		Font f_fix_s = new Font ("Courier New", 8);
		Font f = new Font ("Microsoft Sans Serif", 10);
		Font f_s = new Font ("Microsoft Sans Serif", 8);
		Font f_u = new Font ("Microsoft Sans Serif", 10, FontStyle.Underline);
		Font f_sanskrit = new Font("Sanskrit 99", 15);


		int left = 0;
		int top = 0;
		int width = 0;
		int pad_height = 10;

		int iVarga = 0;
		private bool GetNextVarga (ref Division dtype)
		{
			if (iVarga >= alVargas.Count)
				return false;
			
			dtype = (Division)alVargas[iVarga++];
			return true;
		}

		private void PrintVargas (PrintPageEventArgs e)
		{
			g = e.Graphics;

			left = e.MarginBounds.Left;
			top = e.MarginBounds.Top;
			width = e.MarginBounds.Width;

			Division dtype = new Division(Basics.DivisionType.Rasi);
			DivisionalChart dc = new DivisionalChart(h);
			dc.PrintMode = true;

			int dc_size = Math.Min(width / 2, e.MarginBounds.Height / 3);

			if (false == this.GetNextVarga(ref dtype)) return;
			dc.options.Varga = dtype;
			dc.SetOptions(dc.options);
			g.TranslateTransform(left, top);
			dc.DrawChart(g, dc_size, dc_size);

			g.ResetTransform();
			if (false == this.GetNextVarga(ref dtype)) return;
			dc.options.Varga = dtype;
			dc.SetOptions(dc.options);
			g.TranslateTransform(left+dc_size, top);
			dc.DrawChart(g, dc_size, dc_size);

			g.ResetTransform();
			if (false == this.GetNextVarga(ref dtype)) return;
			dc.options.Varga = dtype;
			dc.SetOptions(dc.options);
			g.TranslateTransform(left, top+dc_size);
			dc.DrawChart(g, dc_size, dc_size);

			g.ResetTransform();
			if (false == this.GetNextVarga(ref dtype)) return;
			dc.options.Varga = dtype;
			dc.SetOptions(dc.options);
			g.TranslateTransform(left+dc_size, top+dc_size);
			dc.DrawChart(g, dc_size, dc_size);

			g.ResetTransform();
			if (false == this.GetNextVarga(ref dtype)) return;
			dc.options.Varga = dtype;
			dc.SetOptions(dc.options);
			g.TranslateTransform(left, top+dc_size*2);
			dc.DrawChart(g, dc_size, dc_size);

			g.ResetTransform();
			if (false == this.GetNextVarga(ref dtype)) return;
			dc.options.Varga = dtype;
			dc.SetOptions(dc.options);
			g.TranslateTransform(left+dc_size, top+dc_size*2);
			dc.DrawChart(g, dc_size, dc_size);



		}

		private void PrintNavamsaChakra (PrintPageEventArgs e)
		{
			g = e.Graphics;
			
			left = e.MarginBounds.Left;
			top = e.MarginBounds.Top;
			width = e.MarginBounds.Width;

			NavamsaControl nc = new NavamsaControl(h);
			nc.PrintMode = true;
			Bitmap bmp = nc.DrawToBitmap(Math.Min(e.MarginBounds.Width, e.MarginBounds.Height));

			g.ResetTransform();
			g.DrawImage(bmp, left, top);
		}
		private void PrintChanchaChakra (PrintPageEventArgs e)
		{
			g = e.Graphics;
			
			left = e.MarginBounds.Left;
			top = e.MarginBounds.Top;
			width = e.MarginBounds.Width;
			int iChanchaSize = Math.Min(e.MarginBounds.Width, e.MarginBounds.Height)-100;
			int iNavaSize = 350;

			AshtakavargaControl ac = new AshtakavargaControl(h);
			ac.PrintMode = true;
			Image iChancha = ac.DrawChanchaToImage(iChanchaSize);
			Image iNava = ac.DrawNavaChakrasToImage(iNavaSize);

			g.ResetTransform();
			g.DrawImage(iChancha, 
				e.PageBounds.Width/2 - iChanchaSize/2, 
				top);
			g.DrawImage(iNava, 
				e.PageBounds.Width/2 - iNavaSize/2, 
				e.PageBounds.Height - iNavaSize - 80);
		}
		private void PrintCoverPage (PrintPageEventArgs e)
		{
			g = e.Graphics;
			
			left = e.MarginBounds.Left;
			top = e.MarginBounds.Top;
			width = e.MarginBounds.Width;

	

			DivisionalChart dc_rasi = new DivisionalChart(h);
			dc_rasi.PrintMode = true;

			DivisionalChart dc_nav = new DivisionalChart(h);
			dc_nav.options.Varga = new Division(Basics.DivisionType.Navamsa);
			dc_nav.PrintMode = true;
			dc_nav.SetOptions(dc_nav.options);

			// Rasi & Navamsa charts
			g.TranslateTransform(left, top);
			dc_rasi.DrawChart(g, width/2, width/2);
			g.ResetTransform();
			g.TranslateTransform(left + (width/2), top);
			dc_nav.DrawChart(g, width/2, width/2);
			top += (width/2) + pad_height;

			// Birth Details
			this.PrintString(string.Format("{0} {1}. {2}. {3}, {4}.", 
				h.wday, h.info.tob, h.info.tz, h.info.lat, h.info.lon));

			// Tithi
			Longitude ltithi = h.getPosition(Body.Name.Moon).longitude.sub(h.getPosition(Body.Name.Sun).longitude);
			double offset = (360.0/30.0) - ltithi.toTithiOffset();
			Tithi ti = ltithi.toTithi();
			this.PrintString (String.Format ("Tithi: {0} {1:N}% left", ti.value, offset / 12.0 * 100));

			// Nakshatra
			Longitude lmoon = h.getPosition(Body.Name.Moon).longitude;
			Nakshatra nmoon = lmoon.toNakshatra();
			offset = (360.0/27.0)-lmoon.toNakshatraOffset();
			int pada = lmoon.toNakshatraPada();
			this.PrintString(String.Format ("Nakshatra: {0} {1}  {2:N}% left",
				nmoon.value, pada, offset/(360.0/27.0)*100));

			// Yoga, Hora
			Longitude smLon = h.getPosition(Body.Name.Sun).longitude.add(h.getPosition(Body.Name.Moon).longitude);
			SunMoonYoga smYoga = smLon.toSunMoonYoga();
			Body.Name bHora = h.calculateHora();
			this.PrintString(string.Format("{0} Yoga, {1} Hora", smYoga.value, bHora));



			top += pad_height;

			// Calculation Details
			foreach (BodyPosition bp in h.positionList)
			{
				switch (bp.type)
				{
					case BodyType.Name.Graha:
					case BodyType.Name.Lagna:
					case BodyType.Name.SpecialLagna:
					case BodyType.Name.Upagraha:
						this.PrintBody(bp);
						break;
				}
			}

			top = e.MarginBounds.Top + (width/2) + pad_height + f.Height;
			left = e.MarginBounds.Left + (width / 2);
			// Vimsottari Dasa
			VimsottariDasa vd = new VimsottariDasa(h);
			vd.options.SeedBody = VimsottariDasa.UserOptions.StartBodyType.Moon;
			this.PrintVimDasa(vd);

		}

	}

}