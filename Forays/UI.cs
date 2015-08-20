﻿//
using System;
using System.Collections.Generic;
using Utilities;
namespace Forays{
	public static class UI{
		public static Actor player{get{ return Actor.player; } }
		public static Map M{get{ return Actor.M; } }

		public static int viewing_commands_idx = 0; //used in DisplayStats to show extra commands
		public static bool viewing_map_shrine_info = false; //used in DisplayStats to show map info

		public static List<PhysicalObject> sidebar_objects = new List<PhysicalObject>();

		public static List<colorstring> ItemDescriptionBox(Item item,bool lookmode,bool mouselook,int max_string_length){
			List<string> text = item.Description().GetWordWrappedList(max_string_length,false);
			Color box_edge_color = Color.DarkGreen;
			Color box_corner_color = Color.Green;
			Color text_color = Color.Gray;
			int widest = 31; // length of "[Press any other key to cancel]"
			if(lookmode){
				widest = 20; // length of "[=] Hide description"
			}
			foreach(string s in text){
				if(s.Length > widest){
					widest = s.Length;
				}
			}
			if((!lookmode || mouselook) && item.Name(true).Length > widest){
				widest = item.Name(true).Length;
			}
			widest += 2; //one space on each side
			List<colorstring> box = new List<colorstring>();
			box.Add(new colorstring("+",box_corner_color,"".PadRight(widest,'-'),box_edge_color,"+",box_corner_color));
			if(!lookmode || mouselook){
				box.Add(new colorstring("|",box_edge_color) + item.Name(true).PadOuter(widest).GetColorString(Color.White) + new colorstring("|",box_edge_color));
				box.Add(new colorstring("|",box_edge_color,"".PadRight(widest),Color.Gray,"|",box_edge_color));
			}
			foreach(string s in text){
				box.Add(new colorstring("|",box_edge_color) + s.PadOuter(widest).GetColorString(text_color) + new colorstring("|",box_edge_color));
			}
			if(!mouselook){
				box.Add(new colorstring("|",box_edge_color,"".PadRight(widest),Color.Gray,"|",box_edge_color));
				if(lookmode){
					box.Add(new colorstring("|",box_edge_color) + "[=] Hide description".PadOuter(widest).GetColorString(text_color) + new colorstring("|",box_edge_color));
				}
				else{
					box.Add(new colorstring("|",box_edge_color) + "[a]pply  [f]ling  [d]rop".PadOuter(widest).GetColorString(text_color) + new colorstring("|",box_edge_color));
					//box.Add(new colorstring("|",box_edge_color) + "[Press any other key to cancel]".PadOuter(widest).GetColorString(text_color) + new colorstring("|",box_edge_color));
				}
			}
			box.Add(new colorstring("+",box_corner_color,"".PadRight(widest,'-'),box_edge_color,"+",box_corner_color));
			return box;
		}
		public static void DisplayStats(){ DisplayStats(false); }
		public static void DisplayStats(bool cyan_letters){
			bool buttons = MouseUI.AutomaticButtonsFromStrings;
			MouseUI.AutomaticButtonsFromStrings = false;
			Screen.CursorVisible = false;
			int row = 0;
			if(!viewing_map_shrine_info){
				string s = ("Health: " + player.curhp.ToString()).PadOuter(20);
				int idx = Math.Max(0,20 * (player.curhp + 4) / player.maxhp);
				/*Color hpcolor = Color.DarkRed;
				if(player.curhp * 3 >= player.maxhp * 2){
					hpcolor = Color.DarkGreen;
				}
				else{
					if(player.curhp * 3 >= player.maxhp){
						hpcolor = Color.DarkYellow;
					}
				}
				hpcolor = Color.DarkRed;*/
				Screen.WriteString(row,0,new colorstring(new cstr(s.Substring(0,idx),Color.Gray,Color.DarkRed),new cstr(s.Substring(idx),Color.Gray,Color.Black)));
				++row;
				if(player.maxmp > 0){
					s = ("Mana: " + player.curmp.ToString()).PadOuter(20);
					idx = Math.Max(0,20 * (player.curmp) / player.maxmp);
					Screen.WriteString(row,0,new colorstring(new cstr(s.Substring(0,idx),Color.Gray,Color.DarkCyan),new cstr(s.Substring(idx),Color.Gray,Color.Black)));
					++row;
				}
				if(player.exhaustion > 0){
					s = ("Exhaustion: " + player.exhaustion.ToString() + "%").PadOuter(20);
					idx = Math.Max(0,20 * (player.exhaustion) / 100);
					Screen.WriteString(row,0,new colorstring(new cstr(s.Substring(0,idx),Color.Gray,Color.DarkYellow),new cstr(s.Substring(idx),Color.Gray,Color.Black)));
					++row;
				}
				cstr cs = player.EquippedWeapon.StatsName();
				cs.s = ("-- " + cs.s + " --").PadOuter(20);
				Screen.WriteString(row,0,cs);
				colorstring statuses = new colorstring();
				for(int i=0;i<(int)EquipmentStatus.NUM_STATUS;++i){
					if(player.EquippedWeapon.status[(EquipmentStatus)i]){
						statuses.strings.Add(new cstr("*",Weapon.StatusColor((EquipmentStatus)i))); //todo check all this --v--^
						if(player.EquippedWeapon.StatsName().s.Length + statuses.Length() >= 19){
							break;
						}
					}
				}
				Screen.WriteString(row,player.EquippedWeapon.StatsName().s.Length + 1,statuses);
				++row;
				cs = player.EquippedArmor.StatsName();
				cs.s = ("-- " + cs.s + " --").PadOuter(20);
				Screen.WriteString(row,0,cs);
				statuses = new colorstring();
				for(int i=0;i<(int)EquipmentStatus.NUM_STATUS;++i){
					if(player.EquippedArmor.status[(EquipmentStatus)i]){
						statuses.strings.Add(new cstr("*",Weapon.StatusColor((EquipmentStatus)i)));
						if(player.EquippedArmor.StatsName().s.Length + statuses.Length() >= 19){
							break;
						}
					}
				}
				Screen.WriteString(row,player.EquippedArmor.StatsName().s.Length + 1,statuses);
				++row;
				Screen.WriteString(row,0,("Depth: " + M.current_level.ToString()).PadOuter(20));
				++row;
				Screen.WriteString(row,0,"".PadRight(20));
				++row;
				int row_cutoff = Global.SCREEN_H - 9;
				if(viewing_commands_idx == 2){
					row_cutoff = Global.SCREEN_H - 2;
				}
				foreach(PhysicalObject o in UI.sidebar_objects){
					var strings = o.GetStatusBarInfo();
					if(row + strings.Count > row_cutoff){
						break;
					}
					foreach(colorstring cs2 in strings){
						Screen.WriteString(row,0,cs2);
						++row;
					}
					Screen.WriteString(row,0,"".PadRight(20));
					++row;
				}
				while(row <= row_cutoff){
					Screen.WriteString(row,0,"".PadRight(20));
					++row;
				}
			}
			else{ //todo fix this:
				Color[] colors = new Color[5];
				string[] shrines = new string[]{"  Combat    ","  Defense   ","  Magic     ","  Spirit    ","  Stealth   "};
				for(int i=0;i<5;++i){
					if(Map.shrine_locations[i].BoundsCheck(M.tile,true) && M.tile[Map.shrine_locations[i]].seen){
						if(M.tile[Map.shrine_locations[i]].type == TileType.RUINED_SHRINE){
							colors[i] = Color.DarkGray;
						}
						else{
							colors[i] = Color.Gray;
						}
					}
					else{
						colors[i] = Color.DarkGray;
						//shrines[i] = "    ---     ";
						shrines[i] = "  -------   ";
					}
				}
				Screen.WriteStatsString(row,0,"            ");
				++row;
				Screen.WriteStatsString(row,0,"            ");
				++row;
				Screen.WriteStatsString(row,0," -Shrines-  ",Color.Yellow);
				for(int i=0;i<5;++i){
					Screen.WriteStatsString(row,0,shrines[i],colors[i]);
					++row;
				}
				/*Screen.WriteStatsString(5,0,"  Combat    ",colors[0]);
				Screen.WriteStatsString(6,0,"  Defense   ",colors[1]);
				Screen.WriteStatsString(7,0,"  Magic     ",colors[2]);
				Screen.WriteStatsString(8,0,"  Spirit    ",colors[3]);
				Screen.WriteStatsString(9,0,"  Stealth   ",colors[4]);*/
				++row;
				Screen.WriteStatsString(row,0,"            "); //todo test
			}
			string[] commandhints = null;
			List<int> blocked_commands = new List<int>();
			switch(viewing_commands_idx){
			case 0:
				commandhints = new string[]{
					"Look around [Tab]   ",
					"[a]pply item        ",
					"[g]et item          ",
					"[f]ling item        ",
					"Wait a turn [.]     ",
					"Travel somewhere [X]",
					"Descend stairs [>]  ",
					"[v]iew more         ",
				};
			break;
			case 1:
				commandhints = new string[]{
					"[w]alk              ",
					"[o]perate something ",
					"View known items [\\]",
					"[p]revious messages ",
					"Help [?]            ",
					"Options [=]         ",
					"[q]uit              ",
					"[v]iew more         ",
				};
			break;
			case 2:
			commandhints = new string[]{
				"[v]iew more         ",
			};
			break;
			}
			/*if(player.attrs[AttrType.RESTING] == -1){
				blocked_commands.Add(5);
			}
			if(M.wiz_dark || M.wiz_lite){
				blocked_commands.Add(3);
			}*/
			Color wordcolor = cyan_letters? Color.Gray : Color.DarkGray;
			Color lettercolor = cyan_letters? Color.Cyan : Color.DarkCyan;
			for(int i=0;i<commandhints.Length;++i){
				if(blocked_commands.Contains(i)){
					Screen.WriteString(row+i,0,commandhints[i].GetColorString(Color.DarkGray,Color.DarkCyan));
				}
				else{
					Screen.WriteString(row+i,0,commandhints[i].GetColorString(wordcolor,lettercolor));
				}
			}
			Screen.WriteString(Global.SCREEN_H - 3,Global.MAP_OFFSET_COLS,"You are in a maze of twisty passages, all alike. ".PadOuter(Global.COLS),Color.Gray,Color.DarkerGray);
			//Screen.WriteString(Global.SCREEN_H - 2,Global.MAP_OFFSET_COLS,"   E[x]plore   [t]orch   [s]hoot bow   Cast spell [z]   [r]est    ".GetColorString(wordcolor,lettercolor));
			Color hack_color_todo = Color.Black; //so, darker gray looks pretty decent. Not sure what to do in 16 color mode.
			//dark green is also not completely terrible. 
			Color hackcolor2 = Color.Gray; //todo
			Screen.WriteString(Global.SCREEN_H - 2,Global.MAP_OFFSET_COLS,"E[x]plore     [t]orch     [s]hoot bow    [r]est     Cast spell [z]".GetColorString(hackcolor2,lettercolor,hack_color_todo));
			Screen.WriteString(Global.SCREEN_H - 1,Global.MAP_OFFSET_COLS,"[i]nventory   [e]quipment [c]haracter    [m]ap              [Menu]".GetColorString(hackcolor2,lettercolor,hack_color_todo));
			//Screen.WriteString(Global.SCREEN_H - 2,Global.MAP_OFFSET_COLS,"   E[x]plore   [t]orch   [s]hoot bow   [r]est   Cast spell [z]    ".GetColorString(wordcolor,lettercolor,hack_color_todo));
			//Screen.WriteString(Global.SCREEN_H - 1,Global.MAP_OFFSET_COLS,"     [i]nventory   [e]quipment   [c]haracter   [m]ap   [Menu]     ".GetColorString(wordcolor,lettercolor,hack_color_todo));
			Screen.ResetColors();
			MouseUI.AutomaticButtonsFromStrings = buttons;
		}
		public static void CreateDefaultStatsButtons(){
			MouseUI.PushButtonMap(MouseMode.Map);
			MouseUI.CreateStatsButton(ConsoleKey.I,false,12,1);
			MouseUI.CreateStatsButton(ConsoleKey.E,false,13,1);
			MouseUI.CreateStatsButton(ConsoleKey.C,false,14,1);
			MouseUI.CreateStatsButton(ConsoleKey.T,false,15,1);
			MouseUI.CreateStatsButton(ConsoleKey.Tab,false,16,1);
			MouseUI.CreateStatsButton(ConsoleKey.R,false,17,1);
			MouseUI.CreateStatsButton(ConsoleKey.A,false,18,1);
			MouseUI.CreateStatsButton(ConsoleKey.G,false,19,1);
			MouseUI.CreateStatsButton(ConsoleKey.F,false,20,1);
			MouseUI.CreateStatsButton(ConsoleKey.S,false,21,1);
			MouseUI.CreateStatsButton(ConsoleKey.Z,false,22,1);
			MouseUI.CreateStatsButton(ConsoleKey.X,false,23,1);
			MouseUI.CreateStatsButton(ConsoleKey.V,false,24,1);
			MouseUI.CreateStatsButton(ConsoleKey.E,false,7,2);
			MouseUI.CreateMapButton(ConsoleKey.P,false,0,3);
		}
		public static void SortSidebarObjects(){
			sidebar_objects.Sort((o1,o2)=>{
				int n1 = 3;
				if(o1 is Item){
					n1 = 2;
				}
				else{
					if(o1 is Actor){
						n1 = 1;
					}
				}
				int n2 = 3;
				if(o2 is Item){
					n2 = 2;
				}
				else{
					if(o2 is Actor){
						n2 = 1;
					}
				}
				int result = n1.CompareTo(n2);
				if(result == 0){
					result = o1.DistanceFrom(player).CompareTo(o2.DistanceFrom(player));
					if(result == 0){
						return o1.ApproximateEuclideanDistanceFromX10(player).CompareTo(o2.ApproximateEuclideanDistanceFromX10(player));
					}
					return result;
				}
				return result;
			});
		}
	}
}
