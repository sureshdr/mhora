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
	/// Summary description for VargaRectificationForm.
	/// </summary>
	public class VargaRectificationForm : System.Windows.Forms.Form
	{
		public class UserOptions : ICloneable
		{
			Moment mStart;
			Moment mEnd;

			Division [] dtypes = new Division []
			{
				new Division(Basics.DivisionType.Rasi),
				new Division(Basics.DivisionType.DrekkanaParasara),
				new Division(Basics.DivisionType.Navamsa),
				new Division(Basics.DivisionType.Saptamsa),
				new Division(Basics.DivisionType.Dasamsa),
				new Division(Basics.DivisionType.Dwadasamsa),
				new Division(Basics.DivisionType.Shodasamsa)
			};

			public Division[] Divisions
			{
				get { return this.dtypes; }
				set { this.dtypes = value; }
			}
			public Moment StartTime
			{
				get { return this.mStart; }
				set { this.mStart = value; }
			}
			public Moment EndTime
			{
				get { return this.mEnd; }
				set { this.mEnd = value; }
			}
			public UserOptions (Moment _start, Moment _end, Division dtype)
			{
				mStart = _start;
				mEnd = _end;

				if (dtype.MultipleDivisions.Length == 1 &&
					(dtype.MultipleDivisions[0].Varga != Basics.DivisionType.Rasi &&
					(dtype.MultipleDivisions[0].Varga != Basics.DivisionType.Navamsa)))
				{
					dtypes = new Division [] 
					{
						new Division(Basics.DivisionType.Rasi), 
						new Division(Basics.DivisionType.Navamsa), 
						dtype
					};
				} 
				else
				{
					dtypes = new Division[]
					{
						new Division(Basics.DivisionType.Rasi), 
						new Division(Basics.DivisionType.Saptamsa), 
						new Division(Basics.DivisionType.Navamsa)
					};
				}

			}
			public UserOptions (Moment _start, Moment _end)
			{
				mStart = _start;
				mEnd = _end;
			}
			public object Clone ()
			{
				UserOptions uo = new UserOptions ((Moment)mStart.Clone(), (Moment)mEnd.Clone());
				uo.Divisions = (Division[])this.Divisions.Clone();
				return uo;
			}
			public object CopyFrom (Object _uo)
			{
				UserOptions uo = (UserOptions)_uo;
				this.StartTime = uo.StartTime;
				this.EndTime = uo.EndTime;
				this.Divisions = (Division[])uo.Divisions.Clone();
				return this.Clone();
			}

		}


		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		Horoscope h = null;
		DivisionalChart dc = null;
		UserOptions opts = null;
		CuspTransitSearch cs = null;
		Division dtypeRasi = new Division(Basics.DivisionType.Rasi);
		Body.Name mBody = Body.Name.Lagna;
		double ut_lower = 0;
		private System.Windows.Forms.ContextMenu mContext;
		private System.Windows.Forms.MenuItem menuOptions;
		double ut_higher = 0;
		private System.Windows.Forms.MenuItem menuReset;
		private System.Windows.Forms.MenuItem menuCopyToClipboard;
		private System.Windows.Forms.MenuItem menuCenter;
		private System.Windows.Forms.MenuItem menuHalve;
		private System.Windows.Forms.MenuItem menuDouble;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuShadvargas;
		private System.Windows.Forms.MenuItem menuSaptavargas;
		private System.Windows.Forms.MenuItem menuDasavargas;
		private System.Windows.Forms.MenuItem menuShodasavargas;
		private System.Windows.Forms.MenuItem menuNadiamsavargas;
		private System.Windows.Forms.MenuItem menuDisplaySeconds;
		Moment mOriginal = null;

		public VargaRectificationForm(Horoscope _h, DivisionalChart _dc, Division _dtype)
		{
			//
			// Required for Windows Form Designer support
			//
			InitializeComponent();

			if (false == MhoraGlobalOptions.Instance.VargaRectificationFormSize.IsEmpty)
				this.Size = MhoraGlobalOptions.Instance.VargaRectificationFormSize;

			h = _h;
			dc = _dc;
			mOriginal = (Moment)h.info.tob.Clone();
			cs = new CuspTransitSearch(h);
			this.PopulateOptionsInit(_dtype);
			//this.PopulateOptions();
			this.PopulateCache();
			this.HScroll = true;
			this.VScroll = true;
			this.Invalidate();
		}

		private Moment utToMoment (double found_ut)
		{
			// turn into horoscope
			int year=0, month=0, day=0;
			double hour =0;
			found_ut += (h.info.tz.toDouble() / 24.0);
			sweph.swe_revjul(found_ut, ref year, ref month, ref day, ref hour);
			Moment m = new Moment(year, month, day, hour);
			return m;
		}
		private double momentToUT (Moment m)
		{
			double local_ut = sweph.swe_julday(m.year, m.month, m.day, m.time);
			return local_ut - (h.info.tz.toDouble())/24.0;
		}
		private void PopulateOptions ()
		{
			ut_lower = this.momentToUT(opts.StartTime);
			ut_higher = this.momentToUT(opts.EndTime);
		}
		private void PopulateOptionsInit (Division dtype)
		{
			DivisionPosition dp = h.getPosition(mBody).toDivisionPosition(this.dtypeRasi);
			Longitude foundLon = new Longitude(0);
			bool bForward = true;
			ut_lower = cs.TransitSearch(mBody, h.info.tob, false, new Longitude(dp.cusp_lower), foundLon, ref bForward);
			ut_higher = cs.TransitSearch(mBody, h.info.tob, true, new Longitude(dp.cusp_higher), foundLon, ref bForward);


			double ut_span = (ut_higher - ut_lower) / (double)Basics.numPartsInDivision(dtype) * 5.0;
			double ut_curr = h.baseUT;
			ut_lower = ut_curr - (ut_span/2.0);
			ut_higher = ut_curr + (ut_span/2.0);
			
			//double ut_extra = (ut_higher-ut_lower)*(1.0/3.0);
			//ut_lower -= ut_extra;
			//ut_higher += ut_extra;


			//ut_lower = h.baseUT - 1.0/24.0;
			//ut_higher = h.baseUT + 1.0/24.0;
			this.opts = new UserOptions(this.utToMoment(ut_lower), this.utToMoment(ut_higher), dtype);
		}
		private double[][] momentCusps = null;
		private ZodiacHouse.Name[][] zhCusps = null;
		private void PopulateCache ()
		{
			momentCusps = new double[opts.Divisions.Length][];
			zhCusps = new ZodiacHouse.Name[opts.Divisions.Length][];
			for (int i=0; i<opts.Divisions.Length; i++)
			{
				Division dtype = opts.Divisions[i];
				ArrayList al = new ArrayList();
				ArrayList zal = new ArrayList();
				//Console.WriteLine ("Calculating cusps for {0} between {1} and {2}", 
				//	dtype, this.utToMoment(ut_lower), this.utToMoment(ut_higher));
				double ut_curr = ut_lower - (1.0 / (24.0*60.0));

				sweph.obtainLock(h);
				BodyPosition bp = Basics.CalculateSingleBodyPosition(ut_curr, sweph.BodyNameToSweph(mBody), mBody,
					BodyType.Name.Graha, h);
				sweph.releaseLock(h);
				//BodyPosition bp = (BodyPosition)h.getPosition(mBody).Clone();
				//DivisionPosition dp = bp.toDivisionPosition(this.dtypeRasi);

				DivisionPosition dp = bp.toDivisionPosition(dtype);

				//Console.WriteLine ("Longitude at {0} is {1} as is in varga rasi {2}",
				//	this.utToMoment(ut_curr), bp.longitude, dp.zodiac_house.value);

				//bp.longitude = new Longitude(dp.cusp_lower - 0.2);
				//dp = bp.toDivisionPosition(dtype);

				while (true)
				{
					Moment m = this.utToMoment(ut_curr);
					Longitude foundLon = new Longitude(0);
					bool bForward = true;

					//Console.WriteLine ("    Starting search at {0}", this.utToMoment(ut_curr));

					ut_curr = cs.TransitSearch(mBody, this.utToMoment(ut_curr), true,
						new Longitude(dp.cusp_higher), foundLon, ref bForward);

					bp.longitude = new Longitude(dp.cusp_higher + 0.1);
					dp = bp.toDivisionPosition(dtype);

					if (ut_curr >= ut_lower && ut_curr <= ut_higher+(1.0/(24.0*60.0*60.0))*5.0)
					{
					//	Console.WriteLine ("{0}: {1} at {2}",
					//		dtype, foundLon, this.utToMoment(ut_curr));
						al.Add(ut_curr);
						zal.Add(dp.zodiac_house.value);
					}
					else if (ut_curr > ut_higher)
					{
					//	Console.WriteLine ("- {0}: {1} at {2}",
					//		dtype, foundLon, this.utToMoment(ut_curr));						
						break;
					}
					ut_curr += ((1.0 / (24.0*60.0*60.0))*5.0);
				}
				momentCusps[i] = (double[])al.ToArray(typeof(double));
				zhCusps[i] = (ZodiacHouse.Name[])zal.ToArray(typeof(ZodiacHouse.Name));
			}


			//for (int i=0; i<opts.Divisions.Length; i++)
			//{
			//	for (int j=0; j<momentCusps[i].Length; j++)
			//	{
			//		Console.WriteLine ("Cusp for {0} at {1}", opts.Divisions[i], momentCusps[i][j]);
			//	}
			//}

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
			this.mContext = new System.Windows.Forms.ContextMenu();
			this.menuOptions = new System.Windows.Forms.MenuItem();
			this.menuReset = new System.Windows.Forms.MenuItem();
			this.menuCenter = new System.Windows.Forms.MenuItem();
			this.menuHalve = new System.Windows.Forms.MenuItem();
			this.menuDouble = new System.Windows.Forms.MenuItem();
			this.menuDisplaySeconds = new System.Windows.Forms.MenuItem();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuShadvargas = new System.Windows.Forms.MenuItem();
			this.menuSaptavargas = new System.Windows.Forms.MenuItem();
			this.menuDasavargas = new System.Windows.Forms.MenuItem();
			this.menuShodasavargas = new System.Windows.Forms.MenuItem();
			this.menuNadiamsavargas = new System.Windows.Forms.MenuItem();
			this.menuCopyToClipboard = new System.Windows.Forms.MenuItem();
			// 
			// mContext
			// 
			this.mContext.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					 this.menuOptions,
																					 this.menuReset,
																					 this.menuCenter,
																					 this.menuHalve,
																					 this.menuDouble,
																					 this.menuDisplaySeconds,
																					 this.menuItem1,
																					 this.menuCopyToClipboard});
			// 
			// menuOptions
			// 
			this.menuOptions.Index = 0;
			this.menuOptions.Text = "Options";
			this.menuOptions.Click += new System.EventHandler(this.menuOptions_Click);
			// 
			// menuReset
			// 
			this.menuReset.Index = 1;
			this.menuReset.Text = "Reset Original Time";
			this.menuReset.Click += new System.EventHandler(this.menuReset_Click);
			// 
			// menuCenter
			// 
			this.menuCenter.Index = 2;
			this.menuCenter.Text = "Center to Current Time";
			this.menuCenter.Click += new System.EventHandler(this.menuCenter_Click);
			// 
			// menuHalve
			// 
			this.menuHalve.Index = 3;
			this.menuHalve.Text = "Zoom In";
			this.menuHalve.Click += new System.EventHandler(this.menuHalve_Click);
			// 
			// menuDouble
			// 
			this.menuDouble.Index = 4;
			this.menuDouble.Text = "Zoom Out";
			this.menuDouble.Click += new System.EventHandler(this.menuDouble_Click);
			// 
			// menuDisplaySeconds
			// 
			this.menuDisplaySeconds.Index = 5;
			this.menuDisplaySeconds.Text = "Display Seconds";
			this.menuDisplaySeconds.Click += new System.EventHandler(this.menuDisplaySeconds_Click);
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 6;
			this.menuItem1.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																					  this.menuShadvargas,
																					  this.menuSaptavargas,
																					  this.menuDasavargas,
																					  this.menuShodasavargas,
																					  this.menuNadiamsavargas});
			this.menuItem1.Text = "Show Vargas";
			// 
			// menuShadvargas
			// 
			this.menuShadvargas.Index = 0;
			this.menuShadvargas.Text = "Shadvargas";
			this.menuShadvargas.Click += new System.EventHandler(this.menuShadvargas_Click);
			// 
			// menuSaptavargas
			// 
			this.menuSaptavargas.Index = 1;
			this.menuSaptavargas.Text = "Saptavargas";
			this.menuSaptavargas.Click += new System.EventHandler(this.menuSaptavargas_Click);
			// 
			// menuDasavargas
			// 
			this.menuDasavargas.Index = 2;
			this.menuDasavargas.Text = "Dasavargas";
			this.menuDasavargas.Click += new System.EventHandler(this.menuDasavargas_Click);
			// 
			// menuShodasavargas
			// 
			this.menuShodasavargas.Index = 3;
			this.menuShodasavargas.Text = "Shodasavargas";
			this.menuShodasavargas.Click += new System.EventHandler(this.menuShodasavargas_Click);
			// 
			// menuNadiamsavargas
			// 
			this.menuNadiamsavargas.Index = 4;
			this.menuNadiamsavargas.Text = "Nadiamsa vargas";
			this.menuNadiamsavargas.Click += new System.EventHandler(this.menuNadiamsavargas_Click);
			// 
			// menuCopyToClipboard
			// 
			this.menuCopyToClipboard.Index = 7;
			this.menuCopyToClipboard.Text = "Copy To Clipboard";
			this.menuCopyToClipboard.Click += new System.EventHandler(this.menuCopyToClipboard_Click);
			// 
			// VargaRectificationForm
			// 
			this.AutoScale = false;
			this.AutoScaleBaseSize = new System.Drawing.Size(5, 13);
			this.AutoScroll = true;
			this.ClientSize = new System.Drawing.Size(512, 142);
			this.ContextMenu = this.mContext;
			this.Name = "VargaRectificationForm";
			this.Text = "Lagna Based Rectification Helper";
			this.Resize += new System.EventHandler(this.VargaRectificationForm_Resize);
			this.Click += new System.EventHandler(this.VargaRectificationForm_Click);
			this.Load += new System.EventHandler(this.VargaRectificationForm_Load);
			this.DoubleClick += new System.EventHandler(this.VargaRectificationForm_DoubleClick);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.VargaRectificationForm_Paint);

		}
		#endregion

		private void VargaRectificationForm_Load(object sender, System.EventArgs e)
		{

		}

		int vname_width = 50;
		int unit_height = 30;
		int half_tick_height = 3;

		private void Draw (Graphics g)
		{
			Font f_time = MhoraGlobalOptions.Instance.GeneralFont;
			Pen p_black = new Pen(Brushes.Black);
			Pen p_lgray = new Pen(Brushes.LightGray);
			Pen p_orange = new Pen(Brushes.DarkOrange);
			Pen p_red = new Pen(Brushes.DarkRed);
			
			//int bar_width = this.Width - vname_width*2;
			int bar_width = zoomWidth - vname_width * 2;
			float x_offset = 0;
			string s;
			SizeF sz;
			
			g.Clear(Color.AliceBlue);

			x_offset = (float)((this.momentToUT(this.mOriginal)-ut_lower)/(ut_higher-ut_lower)*bar_width)+vname_width;
			g.DrawLine(p_lgray, x_offset, unit_height/2, x_offset, opts.Divisions.Length*unit_height+unit_height/2);
			

			x_offset = (float)((h.baseUT-ut_lower)/(ut_higher-ut_lower)*bar_width)+vname_width;
			float y_max = opts.Divisions.Length*unit_height+unit_height/2;
			g.DrawLine(p_red, x_offset, unit_height/2, x_offset, y_max);
			Moment mNow = this.utToMoment(h.baseUT);
			s = mNow.ToTimeString(this.menuDisplaySeconds.Checked);
			sz = g.MeasureString(s, f_time);
			g.DrawString(s, f_time, Brushes.DarkRed, x_offset-sz.Width/2, y_max);



			for (int iVarga=0; iVarga<opts.Divisions.Length; iVarga++)
			{
				int varga_y = (iVarga+1)*unit_height;
				g.DrawLine(p_black, vname_width, varga_y, vname_width + bar_width, varga_y);
				s = string.Format ("D-{0}", Basics.numPartsInDivision(opts.Divisions[iVarga]));
				sz = g.MeasureString(s, f_time);
				g.DrawString(s, f_time, Brushes.Gray, 4, varga_y-sz.Height/2);


				float old_x_offset = 0;
				for (int j=0; j<this.momentCusps[iVarga].Length; j++)
				{
					double ut_curr = this.momentCusps[iVarga][j];
					double perc = (ut_curr-ut_lower)/(ut_higher-ut_lower)*100.0;
					//Console.WriteLine ("Varga {0}, perc {1}", opts.Divisions[iVarga], perc);
					x_offset = (float)((ut_curr-ut_lower)/(ut_higher-ut_lower)*bar_width)+vname_width;		
						
						//(float)((ut_curr-ut_lower)/(ut_higher/ut_lower)*bar_width);
					Moment m = this.utToMoment(ut_curr);
					s = string.Format("{0} {1}",
                        m.ToTimeString(this.menuDisplaySeconds.Checked),
						ZodiacHouse.ToShortString(zhCusps[iVarga][j]));
					sz = g.MeasureString(s, f_time);
					if (old_x_offset + sz.Width < x_offset)
					{
						g.DrawLine (p_black, x_offset, varga_y-half_tick_height, x_offset, varga_y+half_tick_height);
						g.DrawString(s, f_time, Brushes.Gray ,
							x_offset-(sz.Width/2), varga_y-sz.Height-half_tick_height);
//							x_offset-(sz.Width/2), varga_y-sz.Height-half_tick_height);
						old_x_offset = x_offset;

//						s = zhCusps[iVarga][j].ToString();
//						sz = g.MeasureString(s, f_time);
//						g.DrawString(s, f_time, Brushes.Black,
//							x_offset, varga_y - sz.Height);

					} 
					else
					{
						g.DrawLine (p_orange, x_offset, varga_y-half_tick_height, x_offset, varga_y+half_tick_height);
					}
				}
			}

			


			//this.Height);
		}

		Bitmap bmpBuffer = null;
		int zoomWidth = 0;
		int zoomHeight = 0;
		private void VargaRectificationForm_Paint(object sender, System.Windows.Forms.PaintEventArgs e)
		{
			this.zoomHeight = (opts.Divisions.Length+1)*unit_height+unit_height/2;

			if (bmpBuffer != null && 
				bmpBuffer.Width == this.zoomWidth &&
				bmpBuffer.Height == this.zoomHeight)
			{
				e.Graphics.TranslateTransform(this.AutoScrollPosition.X, this.AutoScrollPosition.Y);
				e.Graphics.DrawImage(bmpBuffer, 0, 0);
				return;
			}
				
			this.zoomWidth = this.Width;
			Graphics displayGraphics = this.CreateGraphics();
			bmpBuffer = new Bitmap(zoomWidth, zoomHeight, displayGraphics);
			Graphics imageGraphics = Graphics.FromImage(bmpBuffer);
			this.Draw(imageGraphics);
			displayGraphics.Dispose();
			e.Graphics.TranslateTransform(this.AutoScrollPosition.X, this.AutoScrollPosition.Y);
			e.Graphics.DrawImage(bmpBuffer, 0, 0);

			this.AutoScrollMinSize = new Size(zoomWidth, zoomHeight);
		}

		private void VargaRectificationForm_Resize(object sender, System.EventArgs e)
		{
			MhoraGlobalOptions.Instance.VargaRectificationFormSize = this.Size;
			this.bmpBuffer = null;
			this.Invalidate();
		}

		public object SetOptions (object _uo)
		{
			//UserOptions uo = (UserOptions)_uo;
			//opts.StartTime = uo.StartTime;
			//opts.EndTime = uo.EndTime;

			object ret = this.opts.CopyFrom(_uo);
			this.PopulateOptions();
			this.PopulateCache();
			this.bmpBuffer = null;
			this.Invalidate();
			return ret;
		}
		private void menuOptions_Click(object sender, System.EventArgs e)
		{
			new MhoraOptions(this.opts, new ApplyOptions(SetOptions)).ShowDialog();
		}

		private void VargaRectificationForm_DoubleClick(object sender, System.EventArgs e)
		{
			Point pt = this.PointToClient(Control.MousePosition);
			
			double click_width = (double)pt.X - (double)vname_width;
			//double bar_width = (double)(1000 - vname_width*2);
			double bar_width = (double)(this.Width - vname_width*2);
			double perc = click_width / bar_width;

			if (perc < 0 && perc > 100)
				return;

			double ut_new = ut_lower + ((ut_higher - ut_lower) * perc);
			Moment mNew = this.utToMoment(ut_new);
			h.info.tob = mNew;
			h.OnChanged();
			this.bmpBuffer = null;
			this.Invalidate();

			//Console.WriteLine ("Click at {0}. Width at {1}. Percentage is {2}", 
			//	click_width, bar_width, perc);

		
		}

		private void menuReset_Click(object sender, System.EventArgs e)
		{
			h.info.tob = (Moment)this.mOriginal.Clone();
			h.OnChanged();
			this.bmpBuffer = null;
			this.Invalidate();
		}

		private void VargaRectificationForm_Click(object sender, System.EventArgs e)
		{
			//this.VargaRectificationForm_DoubleClick(sender,e);
		}

		private void menuCopyToClipboard_Click(object sender, System.EventArgs e)
		{
			//Graphics displayGraphics = this.CreateGraphics();
			//Bitmap bmpBuffer = new Bitmap(this.Width, this.Height, displayGraphics);
			//Graphics imageGraphics = Graphics.FromImage(bmpBuffer);
			//this.Draw(imageGraphics);
			//displayGraphics.Dispose();

			Clipboard.SetDataObject(bmpBuffer, true);
		}

		private void UpdateOptsFromUT ()
		{
			opts.StartTime = this.utToMoment(this.ut_lower);
			opts.EndTime = this.utToMoment(this.ut_higher);
		}
		private void menuCenter_Click(object sender, System.EventArgs e)
		{
			double ut_half = (ut_higher-ut_lower)/2.0;
			double ut_curr = this.momentToUT(h.info.tob);
			ut_lower = ut_curr - ut_half;
			ut_higher = ut_curr + ut_half;
			this.UpdateOptsFromUT();
			this.PopulateCache();
			this.bmpBuffer = null;
			this.Invalidate();
		}

		private void menuHalve_Click(object sender, System.EventArgs e)
		{
			double ut_curr = this.momentToUT(h.info.tob);
			double ut_quarter = (ut_higher-ut_lower)/4.0;
			ut_lower = ut_curr - ut_quarter;
			ut_higher = ut_curr + ut_quarter;
			this.UpdateOptsFromUT();
			this.PopulateCache();
			this.bmpBuffer = null;
			this.Invalidate();
		}

		private void menuDouble_Click(object sender, System.EventArgs e)
		{
			double ut_curr = this.momentToUT(h.info.tob);
			double ut_half = (ut_higher-ut_lower);
			ut_lower = ut_curr - ut_half;
			ut_higher = ut_curr + ut_half;
			this.UpdateOptsFromUT();
			this.PopulateCache();
			this.bmpBuffer = null;
			this.Invalidate();
		}

		private void menuShadvargas_Click(object sender, System.EventArgs e)
		{
			this.opts.Divisions = Basics.Shadvargas();
			this.PopulateCache();
			this.bmpBuffer = null;
			this.Invalidate();
		}

		private void menuSaptavargas_Click(object sender, System.EventArgs e)
		{
			this.opts.Divisions = Basics.Saptavargas();
			this.PopulateCache();
			this.bmpBuffer = null;
			this.Invalidate();
		}

		private void menuDasavargas_Click(object sender, System.EventArgs e)
		{
			this.opts.Divisions = Basics.Dasavargas();
			this.PopulateCache();
			this.bmpBuffer = null;
			this.Invalidate();
		}

		private void menuShodasavargas_Click(object sender, System.EventArgs e)
		{
			this.opts.Divisions = Basics.Shodasavargas();
			this.PopulateCache();
			this.bmpBuffer = null;
			this.Invalidate();
		}

		private void menuNadiamsavargas_Click(object sender, System.EventArgs e)
		{
			Division[] divs_shod = Basics.Shodasavargas();
			Division[] divs = new Division[divs_shod.Length+1];
			divs_shod.CopyTo(divs, 0);
			divs[divs_shod.Length] = new Division(Basics.DivisionType.NadiamsaCKN);
			this.opts.Divisions = divs;
			this.PopulateCache();
			this.bmpBuffer = null;
			this.Invalidate();
		}

		private void menuDisplaySeconds_Click(object sender, System.EventArgs e)
		{
			this.menuDisplaySeconds.Checked = !this.menuDisplaySeconds.Checked;
			this.bmpBuffer = null;
			this.Invalidate();
		}
	}
}
