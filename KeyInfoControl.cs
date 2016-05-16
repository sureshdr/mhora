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
	/// Summary description for KeyInfoControl.
	/// </summary>
	public class KeyInfoControl : MhoraControl
	{
		private System.Windows.Forms.ListView mList;
		private System.Windows.Forms.ColumnHeader Key;
		private System.Windows.Forms.ColumnHeader Info;
		private System.Windows.Forms.ContextMenu mKeyInfoMenu;
		private System.Windows.Forms.MenuItem menuItem1;
		private System.Windows.Forms.MenuItem menuItem2;

		/// <summary> 
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.Container components = null;

		public KeyInfoControl(Horoscope _h)
		{
			// This call is required by the Windows.Forms Form Designer.
			InitializeComponent();

			// TODO: Add any initialization after the InitForm call
			h = _h;
			h.Changed += new EvtChanged(OnRecalculate);
			MhoraGlobalOptions.DisplayPrefsChanged += new EvtChanged(OnRedisplay);
			Repopulate();
			this.AddViewsToContextMenu(this.mKeyInfoMenu);

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
			this.mList = new System.Windows.Forms.ListView();
			this.Key = new System.Windows.Forms.ColumnHeader();
			this.Info = new System.Windows.Forms.ColumnHeader();
			this.mKeyInfoMenu = new System.Windows.Forms.ContextMenu();
			this.menuItem1 = new System.Windows.Forms.MenuItem();
			this.menuItem2 = new System.Windows.Forms.MenuItem();
			this.SuspendLayout();
			// 
			// mList
			// 
			this.mList.AllowColumnReorder = true;
			this.mList.BackColor = System.Drawing.Color.Lavender;
			this.mList.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
																					this.Key,
																					this.Info});
			this.mList.ContextMenu = this.mKeyInfoMenu;
			this.mList.Dock = System.Windows.Forms.DockStyle.Fill;
			this.mList.Font = new System.Drawing.Font("Microsoft Sans Serif", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((System.Byte)(0)));
			this.mList.ForeColor = System.Drawing.SystemColors.HotTrack;
			this.mList.FullRowSelect = true;
			this.mList.Location = new System.Drawing.Point(0, 0);
			this.mList.Name = "mList";
			this.mList.Size = new System.Drawing.Size(496, 240);
			this.mList.TabIndex = 0;
			this.mList.View = System.Windows.Forms.View.Details;
			this.mList.SelectedIndexChanged += new System.EventHandler(this.mList_SelectedIndexChanged);
			// 
			// Key
			// 
			this.Key.Text = "Key";
			this.Key.Width = 136;
			// 
			// Info
			// 
			this.Info.Text = "Info";
			this.Info.Width = 350;
			// 
			// mKeyInfoMenu
			// 
			this.mKeyInfoMenu.MenuItems.AddRange(new System.Windows.Forms.MenuItem[] {
																						 this.menuItem1,
																						 this.menuItem2});
			// 
			// menuItem1
			// 
			this.menuItem1.Index = 0;
			this.menuItem1.Text = "-";
			// 
			// menuItem2
			// 
			this.menuItem2.Index = 1;
			this.menuItem2.Text = "-";
			// 
			// KeyInfoControl
			// 
			this.Controls.Add(this.mList);
			this.Name = "KeyInfoControl";
			this.Size = new System.Drawing.Size(496, 240);
			this.ResumeLayout(false);

		}
		#endregion

		private void mList_SelectedIndexChanged(object sender, System.EventArgs e)
		{
		
		}
		protected override void copyToClipboard ()
		{
			int iMaxDescLength = 0;
			for (int i=0; i<this.mList.Items.Count; i++)
				iMaxDescLength = Math.Max(mList.Items[i].Text.Length, iMaxDescLength);
			iMaxDescLength += 2;

			String s = "Key Info: " + "\r\n\r\n";

			for (int i=0; i<mList.Items.Count; i++) 
			{
				ListViewItem li = mList.Items[i];
				s += li.Text.PadRight(iMaxDescLength, ' ');
				s += "- ";
				for (int j=1; j<li.SubItems.Count; j++) 
				{
					s += li.SubItems[j].Text;
				}
				s += "\r\n";
			}
			Clipboard.SetDataObject(s);
		}

		void Repopulate ()
		{
			string[] weekdays = new string[]
			{
				"Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday"
			};

			mList.Items.Clear();

			ListViewItem li;

			li = new ListViewItem ("Date of Birth");
			li.SubItems.Add (h.info.tob.ToString());
			mList.Items.Add (li);

			li = new ListViewItem ("Time Zone");
			li.SubItems.Add (h.info.tz.ToString());
			mList.Items.Add (li);

			li = new ListViewItem ("Latitude");
			li.SubItems.Add (h.info.lat.ToString());
			mList.Items.Add (li);

			li = new ListViewItem ("Longitude");
			li.SubItems.Add (h.info.lon.ToString());
			mList.Items.Add (li);

			li = new ListViewItem ("Altitude");
			li.SubItems.Add (h.info.alt.ToString());
			mList.Items.Add (li);

			{
				HMSInfo hms_srise = new HMSInfo (h.sunrise);
				li = new ListViewItem ("Sunrise");
				string fmt = String.Format ("{0:00}:{1:00}:{2:00}",
					hms_srise.degree, hms_srise.minute, hms_srise.second);
				li.SubItems.Add(fmt);
				mList.Items.Add (li);
			}
			{
				HMSInfo hms_sset = new HMSInfo (h.sunset);
				li = new ListViewItem ("Sunset");
				string fmt = String.Format ("{0:00}:{1:00}:{2:00}",
                    hms_sset.degree, hms_sset.minute, hms_sset.second);
				li.SubItems.Add(fmt);
				mList.Items.Add (li);
			}
			{
				li = new ListViewItem ("Weekday");
				string fmt = String.Format ("{0}", h.wday);
				li.SubItems.Add (fmt);
				mList.Items.Add (li);

			}
			{
				Longitude ltithi = h.getPosition(Body.Name.Moon).longitude.sub(h.getPosition(Body.Name.Sun).longitude);
				double offset = (360.0/30.0) - ltithi.toTithiOffset();
				Tithi ti = ltithi.toTithi();
				Body.Name tiLord = ti.getLord();
				li = new ListViewItem("Tithi");
				string fmt = String.Format ("{0} ({1}) {2:N}% left", ti.ToString(), tiLord, offset / 12.0 * 100);
				li.SubItems.Add (fmt);
				mList.Items.Add (li);
			}
			{
				Longitude lmoon = h.getPosition(Body.Name.Moon).longitude;
				Nakshatra nmoon = lmoon.toNakshatra();
				Body.Name nmoonLord = VimsottariDasa.LordOfNakshatra(nmoon);
				double offset = (360.0/27.0)-lmoon.toNakshatraOffset();
				int pada = lmoon.toNakshatraPada();
				string fmt = String.Format ("{0} {1} ({2}) {3:N}% left",
					nmoon.value, pada, nmoonLord, offset/(360.0/27.0)*100);
				li = new ListViewItem("Nakshatra");
				li.SubItems.Add(fmt);
				mList.Items.Add(li);
			}
			{
				li = new ListViewItem("Karana");
				Longitude lkarana = h.getPosition(Body.Name.Moon).longitude.sub(h.getPosition(Body.Name.Sun).longitude);
				double koffset = (360.0/60.0) - lkarana.toKaranaOffset ();
				Karana k = lkarana.toKarana();
				Body.Name kLord = k.getLord();
				string fmt = string.Format("{0} ({1}) {2:N}% left", k.value, kLord, koffset / 6.0 * 100);
				li.SubItems.Add (fmt);
				mList.Items.Add(li);
			}
			{
				li = new ListViewItem("Yoga");
				Longitude smLon = h.getPosition(Body.Name.Sun).longitude.add(h.getPosition(Body.Name.Moon).longitude);
				double offset = (360.0/27.0) - smLon.toSunMoonYogaOffset();
				SunMoonYoga smYoga = smLon.toSunMoonYoga();
				Body.Name smLord = smYoga.getLord();
				string fmt = string.Format("{0} ({1}) {2:N}% left", smYoga, smLord, offset / (360.0/27.0) * 100);
				li.SubItems.Add(fmt);
				mList.Items.Add(li);
			}
			{
				li = new ListViewItem("Hora");
				Body.Name b = h.calculateHora();
				string fmt = String.Format("{0}", b);
				li.SubItems.Add(fmt);
				mList.Items.Add(li);
			}
			{
				li = new ListViewItem("Kala");
				Body.Name b = h.calculateKala();
				string fmt = String.Format("{0}", b);
				li.SubItems.Add(fmt);
				mList.Items.Add(li);
			}			
			{
				li = new ListViewItem("Muhurta");
				int mIndex = (int)(Math.Floor((h.hoursAfterSunrise() / h.lengthOfDay())*30.0)+1);
				Basics.Muhurta m = (Basics.Muhurta)mIndex;
				string fmt = String.Format("{0} ({1})", m, Basics.NakLordOfMuhurta(m));
				li.SubItems.Add(fmt);
				mList.Items.Add(li);
				
			}
			{
				double ghatisSr = h.hoursAfterSunrise() * 2.5;
				double ghatisSs = h.hoursAfterSunRiseSet() * 2.5;
				li = new ListViewItem ("Ghatis");
				string fmt = String.Format ("{0:0.0000} / {1:0.0000}", ghatisSr, ghatisSs);
				li.SubItems.Add (fmt);
				mList.Items.Add (li);
			}
			{
				int vgOff = (int)Math.Ceiling(h.hoursAfterSunRiseSet() * 150.0);
				vgOff = vgOff % 9;
				if (vgOff == 0) vgOff = 9;
				Body.Name b = (Body.Name) ((int)Body.Name.Sun + vgOff -1);
				li = new ListViewItem("Vighatika Graha");
				string fmt = String.Format ("{0}", b);
				li.SubItems.Add (fmt);
				mList.Items.Add (li);
			}
			{
				li = new ListViewItem("LMT Offset");
				double e=h.lmt_offset;
				double orig_e = e;
				e = e < 0 ? -e : e;
				e *= 24.0 ;
				int hour = (int)Math.Floor(e);
				e = (e-Math.Floor(e))* 60.0;
				int min = (int)Math.Floor(e);
				e = (e-Math.Floor(e))*60.0;
				string prefix = "";
				if (orig_e < 0) prefix="-";
				string fmt = String.Format ("{0}{1:00}:{2:00}:{3:00.00}", 
					prefix, hour, min, e);
				string fmt2 = String.Format (" ({0:00.00} minutes)", h.lmt_offset*24.0*60.0);
				li.SubItems.Add (fmt+fmt2);
				mList.Items.Add(li);			
			}
			{
				sweph.obtainLock(h);
				li = new ListViewItem("Ayanamsa");
				double aya = sweph.swe_get_ayanamsa_ut(h.baseUT);
				int aya_hour = (int)Math.Floor(aya);
				aya = (aya - Math.Floor(aya)) * 60.0;
				int aya_min = (int)Math.Floor(aya);
				aya = (aya - Math.Floor(aya)) * 60.0;
				string fmt = String.Format("{0:00}-{1:00}-{2:00.00}", aya_hour, aya_min, aya);
				li.SubItems.Add (fmt);
				mList.Items.Add(li);
				sweph.releaseLock(h);
			}
			{
				li = new ListViewItem("Universal Time");
				li.SubItems.Add (h.baseUT.ToString());
				mList.Items.Add (li);
			}


			this.ColorAndFontRows(mList);
		}
		void OnRedisplay (Object o)
		{
			this.ColorAndFontRows(mList);
		}

		void OnRecalculate (Object o)
		{
			Repopulate();
		}

	}
}
