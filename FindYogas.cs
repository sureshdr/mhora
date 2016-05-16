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
using System.Diagnostics;
using System.Text.RegularExpressions;
using System.Windows.Forms;

namespace mhora
{
	public class YogasParseException : Exception 
	{
		public string status;
		public YogasParseException () : base () 
		{
			status = null;
		}
		public YogasParseException (string message)
		{
			status = message;
		}
	}

	/*
	 * Here's the syntax that we take
	 */

	/* Here's a call-stack type walkthrough of this class
	 This is particularly useful because some of the functions are poorly names
		evaluate yoga ( full user-specified rule )
		Phase 1: Generate basic parse tree. i.e. each bracketed portion forms one node
			- FindYogas.generateSimpleParseTree (wrapper)
				- FindYogas.generateSimpleParseTreeForNode (worker function)
		
		Phase 2: Expand each of these nodes. This involves taking each leaf node, which
			may contain implicit if-blocks ex <graha:sun,moon> in <rasi:ari,2nd>,
			evaluating some values (2nd=gem), and expanding this into its 4 node equivalent
			- FindYogas.expandSimpleNodes (wrapper)
				- FindYogas.simplifyBasicNode (simplify these <lordof:<rasi:blah>>) exps
					- FindYogas.simplifyBasicNodeTerm (simplify each single term)
						- FindYogas.replaceBasicNodeTerm (replacement parser)
				- FindYogas.expandSimpleNode (implicit binary expansion)
				
		Phase 3: The real evaluation
			Here we simply walk the parse tree, calling ourserves recursively and evaluating
			our &&, ||, ! and (true, false) semantics
			- ReduceTree
				- Recursively call ReduceTree as needed
				- evaluateNode (take simple node, and authoritatively return trueor false
	*/




	public class FindYogas
	{	

		Horoscope h = null;
		public Node rootNode = null;
		ZodiacHouse zhLagna = null;
		Division _dtype = null;

		static public void Test (Horoscope h, Division dtype)
		{
			FindYogas fy = new FindYogas(h, dtype);
			//fy.evaluateYoga ("gr<sun> in hse <1st>");
			//fy.evaluateYoga ("  gr<sun> in hse <1st>  ");
			//fy.evaluateYoga ("( gr<sun> in   hse <1st> )");
			//fy.evaluateYoga ("(gr<sun> in hse <1st>)");
			//fy.evaluateYoga ("(gr<sun> in hse <1st> )  ");

			//fy.evaluateYoga ("<gr:sun,moon,mars,ketu> in <rasi:1st,2nd,3rd,4th,5th,6th,7th,8th>");
			//fy.evaluateYoga ("<gr:mer> with <gr:<lordof:ari>>");
			//fy.evaluateYoga ("&&(<gr:mer> with <gr:<lordof:ari>>)(birth in <time:day>)");
			//fy.evaluateYoga ("&&(<gr:mer> with <gr:<lordof:ari>>)(birth in <time:night>)");
 
			//fy.evaluateYoga ("<gr:mer> in <rasi:leo>");
			//fy.evaluateYoga ("rasi@(<gr:mer> in <rasi:leo>)");
			//fy.evaluateYoga ("navamsa@(<gr:mer> in <rasi:leo>)");
			//fy.evaluateYoga ("rasi@(<gr:mer> in <rasi:can>)");
			//fy.evaluateYoga ("navamsa@(<gr:mer> in <rasi:can>)");
			//fy.evaluateYoga ("&&(rasi@(<gr:mer> in <rasi:leo>))(d9@(<gr:mer> in <rasi:can>)))");

			
			//fy.evaluateYoga ("<gr:<dispof:mer>> is <gr:moon>");
			//fy.evaluateYoga ("d9@(<gr:<dispof:<dispof:mer>>> is <gr:moon>)");
			//fy.evaluateYoga ("<gr:<d9@dispof:merc>> with <gr:sun>");
			//fy.evaluateYoga ("&&(<gr:sun,moon,mars> in <rasi:1st,1st,ari> with <gr:moon> and <gr:jup,pis>)(<gr:moon> in <rasi:2nd>)");
			//fy.evaluateYoga ("(&& (gr<sun> in hse<1st>) (mid term) (gr<moon> in  hse<2nd> ) )");
		}


		public FindYogas(Horoscope _h, Division __dtype)
		{
			h = _h;
			_dtype = __dtype;
			zhLagna = h.getPosition(Body.Name.Lagna).toDivisionPosition(_dtype).zodiac_house;
		}


		XmlYogaNode xmlNode = null;
		
		public string getRuleName ()
		{
			if (xmlNode == null || xmlNode.mhoraRule == null)
				return "";
			return xmlNode.mhoraRule;
			
		}
		public bool evaluateYoga (XmlYogaNode n)
		{
			xmlNode = n;
			return this.evaluateYoga(n.mhoraRule);
		}

		public bool evaluateYoga (string rule)
		{
			rootNode = new Node (null, rule, this._dtype);

			//Console.WriteLine ("");
			//Console.WriteLine ("Evaluating yoga .{0}.", rule);
			this.generateSimpleParseTree();
			this.expandSimpleNodes();
			bool bRet = this.reduceTree();

			//Console.WriteLine ("Final: {0} = {1}", bRet, rule);
			//Console.WriteLine ("");
			return bRet;
		}


		public string trimWhitespace (string sCurr)
		{
			// remove leading & trailing whitespaces
			sCurr = Regex.Replace(sCurr, @"^\s*(.*?)\s*$", "$1");

			// remove contiguous whitespace
			sCurr = Regex.Replace(sCurr, @"(\s+)", " ");

			return sCurr;
		}

		public string peelOuterBrackets (string sCurr)
		{
			// remove leading "(" and whitespace
			sCurr = Regex.Replace(sCurr, @"^\s*\(\s*", "");
			// remove trailing ")" and whitespace
			sCurr = Regex.Replace(sCurr, @"\s*\)\s*$", "");
			return sCurr;
		}

		public string[] getComplexTerms (string sInit)
		{
			ArrayList al = new ArrayList();

			int level = 0;
			int start = 0;
			int end = 0;

			for (int i=0; i<sInit.Length; i++)
			{
				char curr = sInit[i];

				// we're only concerned about the grouping
				if (curr != '(' && curr != ')')
					continue;

				if (curr == '(')
				{
					if (++level == 1)
						start = i;
				}

				if (curr == ')')
				{
					if (level-- == 1)
					{
						end = i;
						string sInner = sInit.Substring(start, end-start+1);
						al.Add(sInner);
					}
				}

				if (level == 0 && curr != '(' && curr != ')')
					throw new YogasParseException("Found unexpected char outside parantheses");
			}

			if (level > 0)
				throw new YogasParseException("Unmatched parantheses");

			return (string[])al.ToArray(typeof(string));
		}

		public bool checkBirthTime (string sTime)
		{
			switch (sTime)
			{
				case "day":
					return h.isDayBirth();
				case "night":
					return !h.isDayBirth();
				default:
					MessageBox.Show("Unknown birth time: " + sTime + this.getRuleName());
					return false;
			}
		}
		public bool evaluateNode (Node n)
		{
			Debug.Assert (n.type == Node.EType.Single);

			string cats = "";
			string[] simpleTerms = n.term.Split(new char[] {' '});
			string[] simpleVals = new string[simpleTerms.Length];
			for (int i=0; i<simpleTerms.Length; i++)
			{
				cats += " " + this.getCategory(simpleTerms[i]);
				simpleVals[i] = (string)this.getValues(simpleTerms[i])[0];
			}
			cats = this.trimWhitespace(cats);

			Body.Name b1, b2, b3;
			ZodiacHouse.Name zh1, zh2;
			int hse1, hse2;

			Division evalDiv = n.dtype;
			switch (cats)
			{
				case "gr: in rasi:":
				case "gr: in house:":
					b1 = this.stringToBody(simpleVals[0]);
					zh1 = this.stringToRasi(simpleVals[2]);
					if (h.getPosition(b1).toDivisionPosition(evalDiv).zodiac_house.value == zh1)
						return true;
					return false;
				case "gr: in mt":
				case "gr: in moolatrikona":
					b1 = this.stringToBody(simpleVals[0]);
					return h.getPosition(b1).toDivisionPosition(evalDiv).isInMoolaTrikona();
				case "gr: in exlt":
				case "gr: in exaltation":
					b1 = this.stringToBody(simpleVals[0]);
					return h.getPosition(b1).toDivisionPosition(evalDiv).isExaltedPhalita();
				case "gr: in deb":
				case "gr: in debilitation":
					b1 = this.stringToBody(simpleVals[0]);
					return h.getPosition(b1).toDivisionPosition(evalDiv).isDebilitatedPhalita();
				case "gr: in own":
				case "gr: in ownhouse":
				case "gr: in own house":
					b1 = this.stringToBody(simpleVals[0]);
					return h.getPosition(b1).toDivisionPosition(evalDiv).isInOwnHouse();
				case "gr: is gr:":
					b1 = this.stringToBody(simpleVals[0]);
					b2 = this.stringToBody(simpleVals[2]);
					if (b1 == b2)
						return true;
					return false;
				case "gr: with gr:":
					b1 = this.stringToBody(simpleVals[0]);
					b2 = this.stringToBody(simpleVals[2]);
					if (h.getPosition(b1).toDivisionPosition(evalDiv).zodiac_house.value ==
						h.getPosition(b2).toDivisionPosition(evalDiv).zodiac_house.value)
						return true;
					return false;
				case "gr: asp gr:":
					b1 = this.stringToBody(simpleVals[0]);
					b2 = this.stringToBody(simpleVals[2]);
					if (h.getPosition(b1).toDivisionPosition(evalDiv).GrahaDristi(
						h.getPosition(b2).toDivisionPosition(evalDiv).zodiac_house))
						return true;
					return false;
				case "gr: in house: from rasi:":
					b1 = this.stringToBody(simpleVals[0]);
					hse1 = this.stringToHouse(simpleVals[2]);
					zh1 = this.stringToRasi(simpleVals[4]);
					if (h.getPosition(b1).toDivisionPosition(evalDiv).zodiac_house.value ==
						new ZodiacHouse(zh1).add(hse1).value)
						return true;
					return false;
				case "gr: in house: from gr:":
					b1 = this.stringToBody(simpleVals[0]);
					hse1 = this.stringToHouse(simpleVals[2]);
					b2 = this.stringToBody(simpleVals[4]);
					return h.getPosition(b1).toDivisionPosition(evalDiv).zodiac_house.value ==
						h.getPosition(b2).toDivisionPosition(evalDiv).zodiac_house.add(hse1).value;
				case "graha in house: from gr: except gr:":
					hse1 = this.stringToHouse(simpleVals[2]);
					b1 = this.stringToBody(simpleVals[4]);
					b2 = this.stringToBody(simpleVals[6]);
					zh1 = h.getPosition(b1).toDivisionPosition(evalDiv).zodiac_house.add(hse1).value;
					for (int i = (int)Body.Name.Sun; i<= (int)Body.Name.Lagna; i++)
					{
						Body.Name bExc = (Body.Name) i;
						if (bExc != b2 &&
							h.getPosition(bExc).toDivisionPosition(evalDiv).zodiac_house.value == zh1)
							return true;
					}
					return false;
				case "rasi: in house: from rasi:":
					zh1 = this.stringToRasi(simpleVals[0]);
					hse1 = this.stringToHouse(simpleVals[2]);
					zh2 = this.stringToRasi(simpleVals[4]);
					if (new ZodiacHouse(zh1).add(hse1).value == zh2)
						return true;
					return false;
				case "birth in time:":
					return this.checkBirthTime(simpleVals[2]);
				default:
					MessageBox.Show("Unknown rule: " + cats + this.getRuleName());
					return false;
			}
		}

		public bool reduceTree (Node n)
		{
			//Console.WriteLine ("Enter ReduceTree {0} {1}", n.type, n.term);
			bool bRet = false;
			switch (n.type)
			{
				case Node.EType.Not:
					Debug.Assert(n.children.Length == 1);
					bRet = !(this.reduceTree(n.children[0]));
					goto reduceTreeDone;
				case Node.EType.Or:
					for (int i=0; i<n.children.Length; i++)
					{
						if (this.reduceTree(n.children[i]) == true)
						{
							bRet = true; 
							goto reduceTreeDone;
						}
					}
					bRet = false;
					goto reduceTreeDone;
				case Node.EType.And:
					for (int i=0; i<n.children.Length; i++)
					{
						if (this.reduceTree(n.children[i]) == false)
						{
							bRet = false;
							goto reduceTreeDone;
						}
					}
					bRet = true;
					goto reduceTreeDone;
				default:
				case Node.EType.Single:
					bRet = this.evaluateNode(n);
					goto reduceTreeDone;
			}
			reduceTreeDone:
			//Console.WriteLine ("Exit ReduceTree {0} {1} {2}", n.type, n.term, bRet);
			return bRet;
		}

		public bool reduceTree ()
		{
			return this.reduceTree(this.rootNode);
		}

		public void generateSimpleParseTreeForNode (Queue q, Node n)
		{
			string text = n.term;

			// remove general whitespace
			text = this.trimWhitespace(text);
		
			bool bOpen = Regex.IsMatch(text, @"\(");
			bool bClose = Regex.IsMatch(text, @"\)");

			Match mDiv = Regex.Match(text, @"^([^&!<\(]*@)");
			if (mDiv.Success)
			{
				n.dtype = this.stringToDivision(mDiv.Groups[1].Value);
				text = text.Replace(mDiv.Groups[1].Value, "");
//				Console.WriteLine ("Match. Replaced {0}. Text now {1}", 
//					mDiv.Groups[1].Value, text);
			}

			// already in simple format
			if (false == bOpen && false == bClose)
			{
				n.type = Node.EType.Single;
				n.term = text;
				//Console.WriteLine ("Need to evaluate simple node {0}", text);
				return;
			}

			// Find operator. One of !, &&, ||
			if (text[0] == '!')
			{
				Node notChild = new Node(n, text.Substring(1, text.Length-1), n.dtype);
				q.Enqueue(notChild);

				n.type = Node.EType.Not;
				n.addChild(notChild);
				return;
			}

			if (text[0] == '&' && text[1] == '&')
				n.type = Node.EType.And;
			
			else if (text[0] == '|' && text[1] == '|')
				n.type = Node.EType.Or;
			
			// non-binary term with brackets. Peel & reparse
			else
			{
				n.term = this.peelOuterBrackets(text);
				q.Enqueue(n);
			}


			// Parse terms with more than one subterm
			if (n.type == Node.EType.And ||
				n.type == Node.EType.Or)
			{
				string[] subTerms = this.getComplexTerms(text);
				foreach (string subTerm in subTerms)
				{
					Node subChild = new Node(n, subTerm, n.dtype);
					q.Enqueue(subChild);
					n.addChild(subChild);
				}
			}

			//Console.WriteLine ("Need to evaluate complex node {0}", text);
		}

		public void generateSimpleParseTree ()
		{
			Queue q = new Queue();
			q.Enqueue(rootNode);

			while (q.Count > 0)
			{
				Node n = (Node)q.Dequeue();
				if (n == null)
					throw new Exception("FindYogas::generateSimpleParseTree. Dequeued null");

				this.generateSimpleParseTreeForNode(q, n);
			}
		}


		public Body.Name stringToBody (string s)
		{
			switch (s)
			{
				case "su": case "sun": return Body.Name.Sun;
				case "mo": case "moo": case "moon": return Body.Name.Moon;
				case "ma": case "mar": case "mars": return Body.Name.Mars;
				case "me": case "mer": case "mercury": return Body.Name.Mercury;
				case "ju": case "jup": case "jupiter": return Body.Name.Jupiter;
				case "ve": case "ven": case "venus": return Body.Name.Venus;
				case "sa": case "sat": case "saturn": return Body.Name.Saturn;
				case "ra": case "rah": case "rahu": return Body.Name.Rahu;
				case "ke": case "ket": case "ketu": return Body.Name.Ketu;
				case "la": case "lag": case "lagna": case "asc": return Body.Name.Lagna;
				default: 
					MessageBox.Show("Unknown body: " + s + this.getRuleName());
					return Body.Name.Other;
			}
		}
		public Division stringToDivision (string s)
		{
			// trim trailing @
			s = s.Substring(0, s.Length-1);

			Basics.DivisionType _dtype;
			switch (s)
			{
				case "rasi": case "d-1": case "d1": _dtype = Basics.DivisionType.Rasi; break;
				case "navamsa": case "d-9": case "d9": _dtype = Basics.DivisionType.Navamsa; break;
				default:
					MessageBox.Show("Unknown division: " + s + this.getRuleName());
					_dtype = Basics.DivisionType.Rasi;
					break;
			}
			return new Division(_dtype);
		}
		public ZodiacHouse.Name stringToRasi (string s)
		{
			switch (s)
			{
				case "ari": return ZodiacHouse.Name.Ari;
				case "tau": return ZodiacHouse.Name.Tau;
				case "gem": return ZodiacHouse.Name.Gem;
				case "can": return ZodiacHouse.Name.Can;
				case "leo": return ZodiacHouse.Name.Leo;
				case "vir": return ZodiacHouse.Name.Vir;
				case "lib": return ZodiacHouse.Name.Lib;
				case "sco": return ZodiacHouse.Name.Sco;
				case "sag": return ZodiacHouse.Name.Sag;
				case "cap": return ZodiacHouse.Name.Cap;
				case "aqu": return ZodiacHouse.Name.Aqu;
				case "pis": return ZodiacHouse.Name.Pis;
				default:
					MessageBox.Show("Unknown rasi: " + s + this.getRuleName());
					return ZodiacHouse.Name.Ari;
			}
		}
		public int stringToHouse (string s)
		{
			int tempVal = 0;

			switch (s)
			{
				case "1": case "1st": tempVal = 1; break;
				case "2": case "2nd": tempVal = 2; break;
				case "3": case "3rd": tempVal = 3; break;
				case "4": case "4th": tempVal = 4; break;
				case "5": case "5th": tempVal = 5; break;
				case "6": case "6th": tempVal = 6; break;
				case "7": case "7th": tempVal = 7; break;
				case "8": case "8th": tempVal = 8; break;
				case "9": case "9th": tempVal = 9; break;
				case "10": case "10th": tempVal = 10; break;
				case "11": case "11th": tempVal = 11; break;
				case "12": case "12th": tempVal = 12; break;
			}
			return tempVal;
		}
		public string replaceBasicNodeCat (string cat)
		{
			switch (cat)
			{
				case "simplelordof:":
				case "lordof:":
				case "dispof:":
				//case "grahasin":
					return "";
				default:
					return cat;
			}
		}
		public string replaceBasicNodeTermHelper (Division d, string cat, string val)
		{
			int tempVal = 0;
			ZodiacHouse.Name zh;
			Body.Name b;
			switch (cat)
			{
				case "rasi:": case "house:": case "hse:":
					tempVal = this.stringToHouse(val);
					if (tempVal > 0)
						return zhLagna.add(tempVal).ToString().ToLower();
				switch (val)
				{
					case "kendra":
						return "1st,4th,7th,10th";
				}
					break;
				case "gr:": case "graha:":
				switch (val)
				{
					case "ben":
						return "mer,jup,ven,moo";
				}
					break;
				case "rasiof:":
					b = this.stringToBody(val);
					return h.getPosition(b).toDivisionPosition(d).zodiac_house.value
						.ToString().ToLower();
				case "lordof:":
					tempVal = this.stringToHouse(val);
					if (tempVal > 0)
						return h.LordOfZodiacHouse(zhLagna.add(tempVal), d).ToString().ToLower();
					zh = this.stringToRasi(val);
					return h.LordOfZodiacHouse(zh, d).ToString().ToLower();
				case "simplelordof:":
					tempVal = this.stringToHouse(val);
					if (tempVal > 0)
						return h.LordOfZodiacHouse(zhLagna.add(tempVal), d).ToString().ToLower();
					zh = this.stringToRasi(val);
					return Basics.SimpleLordOfZodiacHouse(zh).ToString().ToLower();
				case "dispof:":
					b = this.stringToBody(val);
					return h.LordOfZodiacHouse(
						h.getPosition(b).toDivisionPosition(d).zodiac_house, d)
						.ToString().ToLower();
			}
			return val;
		}

		public string getDivision (string sTerm)
		{
			Match mDiv = Regex.Match (sTerm, "<(.*)@");
			if (mDiv.Success)
				return mDiv.Groups[1].Value.ToLower();
			return "";
		}
		public string getCategory (string sTerm)
		{
			// Find categofy
			Match mCat = Regex.Match (sTerm, "<.*@(.*:)");
			if (mCat.Success == false)
				mCat = Regex.Match (sTerm, "<(.*:)");

			if (mCat.Success)
				return mCat.Groups[1].Value.ToLower();
			else
				return sTerm;
		}
		public ArrayList getValues (string sTerm)
		{
			// Find values. Find : or , on the left
			ArrayList alVals = new ArrayList();
			MatchCollection mVals = Regex.Matches (sTerm, "[:,]([^<:,>]*)");
			if (mVals.Count >= 1)
			{
				foreach (Match m in mVals)
					alVals.Add (m.Groups[1].Value.ToLower());
			}
			else
			{
				alVals.Add(sTerm);
			}
			return alVals;

		}
		public string replaceBasicNodeTerm (Division d, string sTerm)
		{
			string sDiv = this.getDivision(sTerm);
			string sCat = this.getCategory(sTerm);
			ArrayList alVals = this.getValues(sTerm);

			Hashtable hash = new Hashtable();
			foreach (string s in alVals)
			{
				string sRep = this.replaceBasicNodeTermHelper(d, sCat, s);
				if (!hash.ContainsKey(sRep))
					hash.Add(sRep, null);
			}

			bool bStart = false;
			string sNew = this.replaceBasicNodeCat(sCat);
			bool sPreserveCat = sNew.Length == 0;

			if (false == sPreserveCat) sNew = "<" + sNew;
			
			ArrayList alSort = new ArrayList();
			foreach (string s in hash.Keys)
				alSort.Add(s);
			alSort.Sort();

			foreach (string s in alSort)
			{
				if (bStart == true)
					sNew += "," + s;
				else
					sNew += s;
				bStart = true;
			}
			if (false == sPreserveCat)
				sNew += ">";
			
			//Console.WriteLine ("{0} evals to {1}", sTerm, sNew);
			return sNew;
		}

		public string simplifyBasicNodeTerm (Node n, string sTerm)
		{

			while (true)
			{
				//Console.WriteLine ("Simplifying basic term: .{0}.", sTerm);		
				Match m = Regex.Match(sTerm, "<[^<>]*>");

				// No terms found. Nothing to do.
				if (m.Success == false)
					return sTerm;

				Division d = n.dtype;
				string sInner = m.Value;

				// see if a varga was explicitly specified
				Match mDiv = Regex.Match(sInner, "<([^:<>]*@)");
				if (mDiv.Success == true)
				{
					d = this.stringToDivision(mDiv.Groups[1].Value);
					sInner.Replace(mDiv.Groups[1].Value, "");
				}

				// Found a term, evaluated it. Nothing happened. Done.
				string newInner = this.replaceBasicNodeTerm(d, sInner);
			
				//Console.WriteLine ("{0} && {1}", newInner.Length, m.Value.Length);

				if (newInner.ToString() == m.Value.ToLower())
					return sTerm;

				// Replace the current term and continue along merrily
				sTerm = sTerm.Replace(m.Value, newInner);
			}
		}

		public void simplifyBasicNode (Queue q, Node n)
		{
			// A simple wrapper that takes each individual whitespace
			// separated term, and tries to simplify it down to bare
			// bones single stuff ready for true / false evaluation
			//string cats = "";

			string sNew = "";
			string[] simpleTerms = n.term.Split(new char[] {' '});
			for (int i=0; i<simpleTerms.Length; i++)
			{
				simpleTerms[i] = this.simplifyBasicNodeTerm(n, simpleTerms[i]);
				sNew += " " + simpleTerms[i];
				//cats += " " + this.getCategory(simpleTerms[i]);
			}

			n.term = this.trimWhitespace(sNew);

			//cats = this.trimWhitespace(cats);
			//Console.WriteLine ("Cats = {0}", cats);

		}

		public void expandSimpleNode (Queue q, Node n)
		{
			
			// <a,b,> op <d,e> 
			// becomes
			// ||(<a> op <e>)(<a> op <e>)(<b> op <d>)(<b> op <e>)

			Node.EType eLogic = Node.EType.Or;

			//Console.WriteLine ("Inner logic: n.term is {0}", n.term);
			if (n.term[0] == '&' && n.term[1] == '&')
			{
				eLogic = Node.EType.And;
				n.term = this.trimWhitespace(n.term.Substring(2, n.term.Length-2));
			} 
			else if (n.term[0] == '|' && n.term[1] == '|')
			{
				n.term = this.trimWhitespace(n.term.Substring(2, n.term.Length-2));
			}
			//Console.WriteLine ("Inner logic: n.term is now {0}", n.term);

			// find num Vals etc
			string[] simpleTerms = n.term.Split(new char[] {' '});
			string[] catTerms = new string[simpleTerms.Length];
			int[] simpleTermsValues = new int[simpleTerms.Length];
			ArrayList[] simpleTermsRealVals = new ArrayList[simpleTerms.Length];

			int numExps = 1;

			// determine total # exps
			for (int i=0; i<simpleTerms.Length; i++)
			{
				catTerms[i] = this.getCategory(simpleTerms[i]);
				simpleTermsRealVals[i] = this.getValues(simpleTerms[i]);
				simpleTermsValues[i] = simpleTermsRealVals[i].Count;
				if (simpleTermsValues[i] > 1)
					numExps *= simpleTermsValues[i];
			}

			//Console.WriteLine ("Exp: {0} requires {1} exps", n.term, numExps);

			// done
			if (numExps <= 1)
				return;

			string[] sNew = new string[numExps];

			// use binary reduction. first term repeats n times, then n/2 etc.
			// "binary" actualy n-ary on number of possible values
			int _numConc = numExps;
			for (int i=0; i<simpleTerms.Length; i++)
			{
				// if more than one value, n-ary reduction
				if (simpleTermsValues[i] > 1)
					_numConc /= simpleTermsValues[i];

				// determine repeat count. with one value, assign to 1
				int numConc = _numConc;
				if (simpleTermsValues[i] == 1)
					numConc = 1;

				// baseIndex increments to numConc after each iteration
				// continue till we fill the list
				int baseIndex =0;
				int valIndex = 0;
				while (baseIndex < numExps)
				{
					for (int j=0; j<numConc; j++)
					{
						int ix = valIndex;
						if (simpleTermsValues[i] == 1) ix = 0;
						sNew[baseIndex+j] += " ";
						if (catTerms[i][catTerms[i].Length-1] == ':')
							sNew[baseIndex+j] += "<" + catTerms[i] + simpleTermsRealVals[i][ix] + ">";
						else
							sNew[baseIndex+j] += simpleTermsRealVals[i][ix];
					}
					baseIndex += numConc;
					valIndex++;
					if (valIndex == simpleTermsValues[i])
						valIndex=0;
				}			
			}

			n.type = eLogic;
			for (int i=0; i<sNew.Length; i++)
			{
				Node nChild = new Node(n, this.trimWhitespace(sNew[i]), n.dtype);
				n.addChild(nChild);
				//Console.WriteLine ("sNew[{0}]: {1}", i, sNew[i]);
			}
		}

		public void expandSimpleNodes ()
		{
			Queue q = new Queue();
			q.Enqueue(rootNode);

			while (q.Count > 0)
			{
				Node n = (Node)q.Dequeue();
				if (n == null)
					throw new Exception("FindYogas::expandSimpleNodes. Dequeued null");

				if (n.type == Node.EType.Single)
				{
					this.simplifyBasicNode(q, n);
				}
				else
				{
					foreach (Node nChild in n.children)
						q.Enqueue (nChild);
				}
			}


			q.Enqueue(rootNode);
			while (q.Count > 0)
			{
				Node n = (Node)q.Dequeue();
				if (n.type == Node.EType.Single)
				{
					this.expandSimpleNode(q,n);
				}
				else
				{
					foreach (Node nChild in n.children)
						q.Enqueue(nChild);
				}
			}

		}

		public class Node
		{
			public enum EType
			{
				And, Or, Not, Single
			}
			public string term;
			public Node[] children = null;
			public Node parent = null;
			public EType type;
			public Division dtype = null;

			public Node (Node _parent, string _term, Division _dtype)
			{
				parent = _parent;
				term = _term;
				dtype = _dtype;
				this.type = EType.Single;
				children = new Node[0];
			}
			public bool hasChildren ()
			{
				if (children != null)
					return true;
				return false;
			}
			public bool isRoot ()
			{
				if (parent == null)
					return true;
				return false;
			}
			public void addChild (Node nChild)
			{
				ArrayList al = null;
				
				if (children != null)
					al = new ArrayList(children);
				else
					al = new ArrayList ();

				al.Add(nChild);
				children = (Node[])al.ToArray(typeof(Node));
			}
		}

	}


}
