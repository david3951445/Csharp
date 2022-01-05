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
        Random r = new Random();
        Queue<int> num7 = new Queue<int>();
        // gamefield
        const int gameLenX = 10;
        const int gameLenY = 20;
        Label[,] grids = new Label[gameLenY, gameLenX]; // background gamefield
        Block block = null; // block on gamefield

        // HOLD and NEXT
        Label[][,] picGrids = new Label[6][,]; // background of HOLD + NEXT*5
        BlockTypes holdType = BlockTypes.NULL; // block on HOLD
        BlockTypes[] nextType = new BlockTypes[5]; // block on NEXT
        Block[] blockPics = new Block[7]; // 7 types of block, constant (treat like pictures)
        bool isHold = false;

        private enum BlockTypes {
            Z ,L, O, S, I, J, T, NULL
        }

        //private enum toucj {

        //}

        private class coord {
            public int x, y;
            public coord() {
                x = y = 0;
            }
        };

        private class Block {
            /* Tetris block
                *     All 7 blocks are constructed by 4 "square" but in different shape.
                */

            public Color color = new Color(); // square
            public coord pos = new coord(); // block's "center" position
            public coord[] shape = new coord[4]; // squares' position relative to pos
            public BlockTypes type;


            public Block(Color c, int[,] s, BlockTypes t) {
                color = c;
                // pos.x = pos.y = 0;

                for (int i = 0; i < 4; i++) {
                    shape[i] = new coord();
                    shape[i].x = s[i, 0];
                    shape[i].y = s[i, 1];
                }

                type = t;
            }

            public void rotate() { // +90 deg
                foreach (coord c in shape) {
                    int temp = c.x;
                    c.x = -c.y;
                    c.y = temp;

                    // shift to match rotation rall in Tetris
                    switch (type) {
                        case BlockTypes.Z: case BlockTypes.S:
                            c.x += 2;
                            break; 
                        case BlockTypes.L: case BlockTypes.O:
                            c.x++;
                            break;
                        case BlockTypes.I: case BlockTypes.J:
                            c.x += 2;
                            c.y--;
                            break;
                        case BlockTypes.T:
                            c.x++;
                            c.y--;
                            break;
                    }
                }
            }

            public void rotateCounter() { // -90 deg
                foreach (coord c in shape) {
                    int temp = c.x;
                    c.x = c.y;
                    c.y = -temp;

                    // shift to match rotation rall in Tetris
                    switch (type) {
                        case BlockTypes.Z: case BlockTypes.S:
                            c.y += 2;
                            break;
                        case BlockTypes.L: case BlockTypes.O:
                            c.y++;
                            break;
                        case BlockTypes.I: case BlockTypes.J:
                            c.x++;
                            c.y += 2;
                            break;
                        case BlockTypes.T:
                            c.x++;
                            c.y++;
                            break;
                    }
                }
            }

            public void print(Label[,] table) { // print block on table
                foreach (coord c in shape) {
                    int x = pos.x + c.x;
                    int y = pos.y + c.y;
                    table[y, x].BackColor = color;
                }
            }

            public void erase(Label[,] table) { // erase block on table
                foreach (coord c in shape) {
                    int x = pos.x + c.x;
                    int y = pos.y + c.y;
                    table[y, x].BackColor = Color.Black;
                }
            }
        };

        public Form1() {
            InitializeComponent();
            init();
            block = generateBlock();
            block.print(grids);
            timer1.Start();
        }

        public void init() {
            // gamefield
            for (int i = 0; i < gameLenY; i++) {
                for (int j = 0; j < gameLenX; j++) {
                    grids[i, j] = new Label();
                    grids[i, j].BackColor = Color.Black;
                    grids[i, j].Dock = DockStyle.Fill;
                    grids[i, j].Margin = new Padding(0);

                    tableLayoutPanel2.Controls.Add(grids[i, j], j, i);
                }
            }

            // HOLD and NEXT
            TableLayoutPanel[] table = new TableLayoutPanel[] {
                tableLayoutPanel3,
                tableLayoutPanel5,
                tableLayoutPanel6,
                tableLayoutPanel7,
                tableLayoutPanel8,
                tableLayoutPanel9
            };

            for (int k = 0; k < 6; k++) {
                picGrids[k] = new Label[4, 4];

                for (int i = 0; i < 4; i++) {
                    for (int j = 0; j < 4; j++) {
                        picGrids[k][i, j] = new Label();
                        picGrids[k][i, j].BackColor = Color.Black;
                        picGrids[k][i, j].Dock = DockStyle.Fill;
                        picGrids[k][i, j].Margin = new Padding(0);

                        table[k].Controls.Add(picGrids[k][i, j], j, i);
                    }
                }
            }

            for (int i = 0; i < 7; i++) {
                blockPics[i] = newBlock((BlockTypes) i);
            }
        }

        private Block newBlock(BlockTypes type) {
            switch (type) {
                case BlockTypes.Z: return new Block(Color.Red, new int[,] { { 0, 0 }, { 1, 0 }, { 1, 1 }, { 2, 1 } }, BlockTypes.Z); // Z
                case BlockTypes.L: return new Block(Color.Orange, new int[,] { { 0, 0 }, { 1, 0 }, { 2, 0 }, { 0, 1 } }, BlockTypes.L); // L
                case BlockTypes.O: return new Block(Color.Yellow, new int[,] { { 0, 0 }, { 1, 0 }, { 0, 1 }, { 1, 1 } }, BlockTypes.O); // O
                case BlockTypes.S: return new Block(Color.Green, new int[,] { { 0, 1 }, { 1, 0 }, { 1, 1 }, { 2, 0 } }, BlockTypes.S); // S
                case BlockTypes.I: return new Block(Color.Blue, new int[,] { { 0, 0 }, { 1, 0 }, { 2, 0 }, { 3, 0 } }, BlockTypes.I); // I
                case BlockTypes.J: return new Block(Color.Indigo, new int[,] { { 0, 0 }, { 1, 0 }, { 2, 0 }, { 2, 1 } }, BlockTypes.J); // J
                case BlockTypes.T: return new Block(Color.Purple, new int[,] { { 0, 0 }, { 1, 0 }, { 2, 0 }, { 1, 1 } }, BlockTypes.T); // T
                default: return new Block(Color.Purple, new int[,] { { 0, 0 }, { 1, 0 }, { 2, 0 }, { 1, 1 } }, BlockTypes.T); // T
            }
        }

        private Block generateBlock() { // Generate blocks with Tetris’s generation rules
            if (num7.Count <= 7) {
                // generate random no repeat numbers from 0 to 6
                List<int> l = new List<int>() {0, 1, 2, 3, 4, 5, 6};
                for (int i = 7; i > 0; i--) {
                    int rand = r.Next(i);
                    num7.Enqueue(l[rand]);
                    Console.WriteLine(l[rand]);
                    l.RemoveAt(rand);
                }
            }

            // show 5 pics on NEXT
            int[] arr = num7.ToArray();
            for (int i = 0; i < 5; i++) {
                blockPics[arr[i]].erase(picGrids[i+1]);
                blockPics[arr[i+1]].print(picGrids[i+1]);
            }
            BlockTypes type = (BlockTypes)num7.Dequeue();
            
            Block b = newBlock(type);
            b.pos.x = gameLenX / 2 - 1;
            return b;
        }

        private void move(Keys key) {
            block.erase(grids); // erase old block
            switch (key) {
                case Keys.Left: moveLeft(); break;
                case Keys.Right: moveRight(); break;
                case Keys.Down: moveDown(); break;
                case Keys.Up: rotate(); break;
                case Keys.Z: rotateCounter(); break;
                case Keys.C: hold(); break;
                case Keys.Escape: Close(); break;
                case Keys.Space: while (moveDown()) ; break;
            }
            block.print(grids); // print new block
        }

        public void moveLeft() {
            block.pos.x--;

            if (collision_check() != 0) {
                block.pos.x++; // recovery
            }
        }

        public void moveRight() {
            block.pos.x++;

            if (collision_check() != 0) {
                block.pos.x--; // recovery
            }
        }

        public bool moveDown() {
            block.pos.y++;

            int check = collision_check();
            if (check == 4 || check == 5) {
                block.pos.y--; // recovery
                block.print(grids); // recovery

                block = generateBlock();
                if (collision_check() != 0) {
                    timer1.Stop();
                    MessageBox.Show("GAMEOVER");
                    Close();
                }

                lineEliminated_check();
                isHold = false;

                return false;
            }

            return true;
        }

        public void rotate() {
            block.rotate();

            if (collision_check() != 0) {
                block.rotateCounter(); // recovery
            }
        }

        public void rotateCounter() {
            block.rotateCounter();

            if (collision_check() != 0) {
                block.rotate(); // recovery
            }
        }

        private int collision_check() {
            foreach (coord c in block.shape) {
                int x = block.pos.x + c.x;
                int y = block.pos.y + c.y;

                // boundary
                if (x < 0) return 1;
                if (x >= gameLenX) return 2;
                if (y < 0) return 3;
                if (y >= gameLenY) return 4;

                // blocks on the field
                if (grids[y, x].BackColor != Color.Black) return 5;
            }

            return 0; // no collision
        }

        private void hold() {
            if (isHold) {
                return;
            }
            isHold = true;

            BlockTypes old = holdType;
            holdType = block.type; // store type of current block       

            if (old == BlockTypes.NULL) { // there is no hold block (first hold)
                blockPics[(int)holdType].print(picGrids[0]); // print current block on HOLD
                block = generateBlock();
            }
            else {
                blockPics[(int)old].erase(picGrids[0]); // erase old block on HOLD
                blockPics[(int)holdType].print(picGrids[0]); // print current block on HOLD
                block = newBlock(old); // put hold block into gamefield
                block.pos.x = gameLenX / 2 - 1;
            }         
        }

        private void lineEliminated_check() {
            // Check if exist line to be eliminated, if not, store it to oldLines[]
            int[] oldLines = new int[gameLenY];
            int k = gameLenY - 1;
            for (int i = gameLenY - 1; i >= 0; i--) {
                int j;
                for (j = 0; j < gameLenX; j++) {
                    if (grids[i, j].BackColor == Color.Black) {
                        break;
                    }
                }

                if (j == gameLenX) { // its a full square line
                    // score ++
                }
                else { // store lines that do not need to be eliminated
                    oldLines[k--] = i;
                }
            }


            for (int i = gameLenY - 1; i >= 0; i--) {
                for (int j = 0; j < gameLenX; j++) {
                    if (i >= k) { // Still have old lines
                        grids[i, j].BackColor = grids[oldLines[i], j].BackColor;
                    }
                    else {
                        grids[i, j].BackColor = Color.Black; // Pad gamefield's color
                    }
                }
            }
        }


        private void timer1_Tick(object sender, EventArgs e) {        
            move(Keys.Down);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e) {
            move(e.KeyCode);
        }
    }
}
