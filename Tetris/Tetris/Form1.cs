﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Tetris {
    public partial class Form1 : Form {
        //TableLayoutPanel TableLayoutPanel1 = new TableLayoutPanel();
        Random r = new Random();
        public const int gameLenX = 10;
        public const int gameLenY = 20;
        Label[,] grids = new Label[gameLenY, gameLenX]; // background of game
        Block block = null; // = new Block();

        public Form1() {
            InitializeComponent();
            //game g = new game();
            initGrid();
            block = newBlock(r.Next(6));
            print_block(block.color);
            timer1.Start();
        }
        public enum BlockTypes {
            Z ,L, O, S, I, J, T
        }

        private class coord {
            public int x, y;
            public coord() {
                x = y = 0;
            }
        };

        private class Block {
            /* Tetris block
                *     All 7 blocks are constructed by 4 square but in different shape.
                */

            public Color color = new Color(); // square
            public coord pos = new coord(); // block's "center" position
            public coord[] shape = new coord[4]; // squares' position relative to pos

            public Block(Color c, int[,] s) {
                color = c;
                pos.x = 0;
                pos.y = 0;

                for (int i = 0; i < 4; i++) {
                    shape[i] = new coord();
                    shape[i].x = s[i, 0];
                    shape[i].y = s[i, 1];
                }
            }

            public void rotate() { // +90 deg
                int temp = pos.x;
                pos.x = -pos.y;
                pos.y = temp;
            }

            public void rotateCounter() { // -90 deg
                int temp = x;
                x = y;
                y = -temp;
            }
        };

        public void initGrid() {
            //TableLayoutPanel1.ColumnCount = gameLenX;
            //TableLayoutPanel1.RowCount = gameLenY;
            //TableLayoutPanel1.Size = new Size(400, 800);
            //TableLayoutPanel1.CellBorderStyle = TableLayoutPanelCellBorderStyle.Inset;
            //TableLayoutPanel1.Dock = DockStyle.Fill;

            for (int i = 0; i < gameLenY; i++) {
                for (int j = 0; j < gameLenX; j++) {
                    grids[i, j] = new Label();
                    grids[i, j].BackColor = tableLayoutPanel2.BackColor;
                    grids[i, j].Dock = DockStyle.Fill;
                    grids[i, j].Margin = new Padding(0);
                    //grids[i, j].TextAlign = ContentAlignment.MiddleCenter;
                    //grids[i, j].Width = 30;
                    //grids[i, j].Height = 30;
                    //grids[i, j].BorderStyle = BorderStyle.FixedSingle;
                    //grids[i, j].Left = 150 + 30 * j;
                    //grids[i, j].Top = 600 - i * 30;

                    tableLayoutPanel2.Controls.Add(grids[i, j], j, i);
                }
            }

        }

        private Block newBlock(int type) {
            switch ((BlockTypes) type) {
                case BlockTypes.Z: return new Block(Color.Red, new int[,] { { 0, 0 }, { 1, 0 }, { 1, 1 }, { 2, 1 } }); // Z
                case BlockTypes.L: return new Block(Color.Orange, new int[,] { { 0, 0 }, { 1, 0 }, { 2, 0 }, { 0, 1 } }); // L
                case BlockTypes.O: return new Block(Color.Yellow, new int[,] { { 0, 0 }, { 1, 0 }, { 0, 1 }, { 1, 1 } }); // O
                case BlockTypes.S: return new Block(Color.Green, new int[,] { { 0, 1 }, { 1, 0 }, { 1, 1 }, { 2, 0 } }); // S
                case BlockTypes.I: return new Block(Color.Blue, new int[,] { { 0, 0 }, { 1, 0 }, { 2, 0 }, { 3, 0 } }); // I
                case BlockTypes.J: return new Block(Color.Indigo, new int[,] { { 0, 0 }, { 1, 0 }, { 2, 0 }, { 2, 1 } }); // J
                case BlockTypes.T: return new Block(Color.Purple, new int[,] { { 0, 0 }, { 1, 0 }, { 2, 0 }, { 1, 1 } }); // T
                default: return new Block(Color.Purple, new int[,] { { 0, 0 }, { 1, 0 }, { 2, 0 }, { 1, 1 } }); // T
            }
        }

        public void moveLeft() {
            block.pos.x--;

            if (collision_check()) {
                block.pos.x++;
            }
        }

        public void moveRight() {
            block.pos.x++;

            if (collision_check()) {
                block.pos.x--;
            }
        }

        public void moveDown() {
            block.pos.y++;

            if (collision_check()) {
                block.pos.y--;
            }
        }

        public void rotate() {
            foreach (coord c in block.shape) {
                c.rotate();
            }

           if (collision_check()) {
                foreach (coord c in block.shape) {
                    c.rotateCounter();
                }
            }
        }

        public void rotateCounter() {
            foreach (coord c in block.shape) {
                c.rotateCounter();
            }

            if (collision_check()) {
                foreach (coord c in block.shape) {
                    c.rotate();
                }
            }
        }

        private bool collision_check() {
            foreach (coord c in block.shape) {
                int x = block.pos.x + c.x;
                int y = block.pos.y + c.y;

                // boundary
                if (x < 0 || x >= gameLenX || y < 0 || y >= gameLenY)
                    return true;

                // blocks on the field
            }

            return false; // no collision
        }

        // print block
        private void print_block(Color c) {
            for (int i = 0; i < 4; i++) {
                int x = block.pos.x + block.shape[i].x;
                int y = block.pos.y + block.shape[i].y;
                //Console.WriteLine("x = {0}, y = {1}", x, y);
                grids[y, x].BackColor = c;
            }
        }

        private void move(Keys key) {
            print_block(tableLayoutPanel2.BackColor);
            switch (key) {
                case Keys.Left: moveLeft(); break;
                case Keys.Right: moveRight(); break;
                case Keys.Down: moveDown(); break;
                case Keys.Up: rotate(); break;
                case Keys.Z: rotateCounter(); break;
                case Keys.Escape: Close(); break;
            }
            print_block(block.color);
        }


        private void Form1_Load(object sender, EventArgs e) {
        }

        // event

        private void timer1_Tick(object sender, EventArgs e) {
            
            move(Keys.Down);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e) {
            move(e.KeyCode);
        }
    }
}
