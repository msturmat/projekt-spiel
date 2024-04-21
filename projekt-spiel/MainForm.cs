/*
 * Created by SharpDevelop.
 * User: Michelle
 * Date: 21.04.2024
 * Time: 22:59
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using System.IO;

namespace projekt_spiel
{
	/// <summary>
	/// Description of MainForm.
	/// </summary>
	public partial class MainForm : Form
	{
		int[,] feld = new int[9,9];
		int level = 0;
		int positionx = 0;
		int positiony = 0;
		
		string dump;
		Bitmap tile;
		Bitmap door;
		Bitmap wand;
		Bitmap figur;
		
		public MainForm()
		{
			InitializeComponent();	
		}
		
		void MainFormLoad(object sender, EventArgs e)
		{
			tile = new Bitmap("asset_tile.bmp");
			figur = new Bitmap("figur.bmp");
			door = new Bitmap("asset_door.bmp");
			wand = new Bitmap("asset_wand.bmp");
			loadLevel(level);
			for(int i = 0; i < 9; i++)
			{
				for(int o = 0; o < 9; o++)
					dump = dump + Convert.ToString(feld[o, i]) + " ";
				dump = dump + "\r\n";
			}
			//MessageBox.Show(dump);
			displaySpielfeld();
		}
		
		public void loadLevel(int levels)
		{
			if(!File.Exists("level" + Convert.ToString(levels) + ".bmp") )
			{
				level = 0;
				levels = 0;
				MessageBox.Show("Du hast das Spiel geschafft!");
			}
			
			Bitmap bitmap = new Bitmap("level" + Convert.ToString(levels) + ".bmp");
			for(int x = 0; x < 9; x++)
				for (int y = 0; y < 9; y++)
				{
					Color color = bitmap.GetPixel(x, y);
					
					feld[x,y] = 1;//Mauer
					if(color.G > 150)
						feld[x,y] = 3;//Eingang
					if(color.R > 150)
						feld[x,y] = 2;//Ausgang / Tür
					if(color.B > 200)
						feld[x,y] = 0;//begehbare Fläche
				}
			//Spielerposition:
			for(int x = 0; x < 9; x++)
				for(int y = 0; y < 9; y++)
					if(feld[x,y] == 3) //Eingang
					{
						positionx = x;
						positiony = y;
						feld[x,y] = 0;
					}
			displaySpielfeld();
		}
		
		public void movePlayer(int movex, int movey)
		{
			//fragt ab, ob die Figur noch im Array (= Spielfeld) ist. Wenn nicht, wird returned:
			if(positionx + movex > 8 || positionx + movex < 0 || positiony + movey > 8 || positiony + movey < 0)
				return;
			
			//fragt ab, ob die Figur eine Wand berührt:
			if(feld[positionx + movex, positiony + movey] == 1)
				return;
			
			//fragt ab, ob die Figur auf der Tür steht:
			if(feld[positionx + movex, positiony + movey] == 2)
			{
				level++;
				loadLevel(level);
				displaySpielfeld();
				return;
			}
			
			int oldpositionx = positionx;
			int oldpositiony = positiony;
			positionx = positionx + movex;
			positiony = positiony + movey;
			
			Bitmap bmap = new Bitmap(pictureBox1.Image);
			bmap = render(oldpositionx, oldpositiony, bmap);
			bmap = render(positionx, positiony, bmap);
			pictureBox1.Image = bmap;
		}
		
		public void displaySpielfeld()
		{
			Bitmap renderSpielfeld = new Bitmap(576, 576);
			for(int x = 0; x < 9; x++)
				for(int y = 0; y < 9; y++)
					renderSpielfeld = render(x, y, renderSpielfeld);
			pictureBox1.Image = renderSpielfeld;
		}
		
		public Bitmap render (int x, int y, Bitmap renderSpielfeld)
		{
			Bitmap target = tile;
			if(feld[x,y] == 1)
				target = wand;
			if(feld[x,y] == 2)
				target = door;
			if(x == positionx && y == positiony)
				target = figur;
			
			for(int i = 0; i < 64; i++)
				for(int e = 0; e < 64; e++)
					renderSpielfeld.SetPixel((64 * x) + i, (64 * y) + e, target.GetPixel(i, e));
						
			return renderSpielfeld;
		}
		
		protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
		{
			if(keyData == Keys.Up)
			{
				movePlayer(0, -1);
				return true;
			}
			if(keyData == Keys.Down)
			{
				movePlayer(0, 1);
				return true;
			}
			if(keyData == Keys.Left)
			{
				movePlayer(-1, 0);
				return true;
			}
			if(keyData == Keys.Right)
			{
				movePlayer(1, 0);
				return true;
			}
			return base.ProcessCmdKey(ref msg, keyData);
		}

		
	}
}