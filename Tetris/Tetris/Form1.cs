using System;
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
        const int gameLenX = 10;
        const int gameLenY = 20;
        Label[,] grids = new Label[gameLenY, gameLenX]; // gamefield
        Label[,,] pic = new Label[4, 4, 6];
        Block block = null;
        Block[] blockPics = new Block[7];
        TableLayoutPanel[] table = new TableLayoutPanel[6]; // collect tables of HOLD and NEXT on form1

        BlockTypes holdType = BlockTypes.NULL;
        bool isHold = false;

        public Form1() {
            InitializeComponent();
            init();
            block = newBlock((BlockTypes) r.Next(6));
            print_block(block.color);
            timer1.Start();
        }

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
                }
            }

            public void rotateCounter() { // -90 deg
                foreach (coord c in shape) {
                    int temp = c.x;
                    c.x = c.y;
                    c.y = -temp;
                }
            }
        };

        public void init() {
            // add grid[,] to Contorl
            for (int i = 0; i < gameLenY; i++) {
                for (int j = 0; j < gameLenX; j++) {
                    grids[i, j] = new Label();
                    grids[i, j].BackColor = tableLayoutPanel2.BackColor;
                    grids[i, j].Dock = DockStyle.Fill;
                    grids[i, j].Margin = new Padding(0);

                    tableLayoutPanel2.Controls.Add(grids[i, j], j, i);
                }
            }

            // add pic[,,] to Contorl
            table = new TableLayoutPanel[] {
                tableLayoutPanel3,
                tableLayoutPanel5,
                tableLayoutPanel6,
                tableLayoutPanel7,
                tableLayoutPanel8,
                tableLayoutPanel9
            };

            for (int i = 0; i < 4; i++) {
                for (int j = 0; j < 4; j++) {
                    for (int k = 0; k < 6; k++) {
                        pic[i, j, k] = new Label();
                        pic[i, j, k].BackColor = tableLayoutPanel2.BackColor;
                        pic[i, j, k].Dock = DockStyle.Fill;
                        pic[i, j, k].Margin = new Padding(0);

                        table[i].Controls.Add(pic[i, j, k], j, i);
                    }
                }
            }

            // block Picture
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

        private void move(Keys key) {
            print_block(tableLayoutPanel2.BackColor);
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
            print_block(block.color);
        }

        public void moveLeft() {
            block.pos.x--;

            if (collision_check() != 0) {
                block.pos.x++;
            }
        }

        public void moveRight() {
            block.pos.x++;

            if (collision_check() != 0) {
                block.pos.x--;
            }
        }

        public bool moveDown() {
            block.pos.y++;

            int check = collision_check();
            if (check == 4 || check == 5) {
                block.pos.y--;
                print_block(block.color);

                block = newBlock((BlockTypes) r.Next(6));
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

        private void hold() {
            if (isHold) {
                return;
            }

            BlockTypes t = holdType;
            holdType = block.type; // store type of current block
            show_block(holdType, 1);
            isHold = true;

            if (t == BlockTypes.NULL) { // there is no hold block (first hold)
                block = newBlock((BlockTypes)r.Next(6));
            }
            else {
                block = newBlock(t); // put hold block into gamefield
            }         
        }

        private void lineEliminated_check() {
            // Check if exist line to be eliminated, if not, store it to oldLines[]
            int[] oldLines = new int[gameLenY];
            int k = gameLenY - 1;
            for (int i = gameLenY - 1; i >= 0; i--) {
                int j;
                for (j = 0; j < gameLenX; j++) {
                    if (grids[i, j].BackColor == tableLayoutPanel2.BackColor) {
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
                        grids[i, j].BackColor = tableLayoutPanel2.BackColor; // Pad gamefield's color
                    }
                }
            }
        }

        public void rotate() {
            block.rotate();

           if (collision_check() != 0) {
               block.rotateCounter();
           }
        }

        public void rotateCounter() {
            block.rotateCounter();

            if (collision_check() != 0) {
                block.rotate();
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
                if (grids[y, x].BackColor != tableLayoutPanel2.BackColor) return 5;
            }

            return 0; // no collision
        }

        private void print_block(Color c) {
            for (int i = 0; i < 4; i++) {
                int x = block.pos.x + block.shape[i].x;
                int y = block.pos.y + block.shape[i].y;
                //Console.WriteLine("x = {0}, y = {1}", x, y);
                grids[y, x].BackColor = c;
            }
        }

        private void show_block(BlockTypes type, int where) { // show pic on HOLD or NEXT
            int k = (int) type;
            foreach (coord c in blockPics[k].shape) {
                int x = c.x;
                int y = c.y;

                pic[y, x, where].BackColor = blockPics[k].color;
            }
        }

        private void Form1_Load(object sender, EventArgs e) {
        }


        private void timer1_Tick(object sender, EventArgs e) {        
            move(Keys.Down);
        }

        private void Form1_KeyDown(object sender, KeyEventArgs e) {
            move(e.KeyCode);
        }
    }
}
