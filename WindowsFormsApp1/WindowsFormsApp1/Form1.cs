using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsFormsApp1 {
    public partial class Form1 : Form {
        Random r = new Random();
        List<string> icons = new List<string>() {
        "!", "!", "N", "N", ",", ",", "k", "k",
        "b", "b", "v", "v", "w", "w", "z", "z"
        };

        // Label reference
        Label firstClicked = null;
        Label secondClicked = null;

        private void AssignIconsToSquares() {
            foreach (Control c in tableLayoutPanel1.Controls) {
                Label l = c as Label;
                if (l != null) {
                    int randNum = r.Next(icons.Count);
                    l.Text = icons[randNum];
                    icons.RemoveAt(randNum); // draw out
                }

                l.ForeColor = l.BackColor;
            }
        }

        public Form1() {
            InitializeComponent();
            AssignIconsToSquares();
        }

        private void tableLayoutPanel1_Paint(object sender, PaintEventArgs e) {

        }

        private void label1_Click(object sender, EventArgs e) {
            // ignore any clicks if the timer is running
            if (timer1.Enabled) {
                return;
            }

            Label l = sender as Label;
            
            if (l != null) {
                if (l.ForeColor == Color.Black) {
                    return;
                }

                if (firstClicked == null) {
                    firstClicked = l;
                    firstClicked.ForeColor = Color.Black;

                    return;
                }

                secondClicked = l;
                secondClicked.ForeColor = Color.Black;

                CheckForWinner();

                // two icons are matched
                if (firstClicked.Text == secondClicked.Text) {
                    firstClicked = secondClicked = null;

                    return;
                }

                timer1.Start();
            }
        }

        private void timer1_Tick(object sender, EventArgs e) {
            // Stop the timer
            timer1.Stop();

            // Hide both icons
            firstClicked.ForeColor = firstClicked.BackColor;
            secondClicked.ForeColor = secondClicked.BackColor;

            // Reset
            firstClicked = null;
            secondClicked = null;
        }

        private void CheckForWinner() {
            // Go through all of the labels in the TableLayoutPanel, 
            // checking each one to see if its icon is matched
            foreach (Control control in tableLayoutPanel1.Controls) {
                Label iconLabel = control as Label;

                if (iconLabel != null) {
                    if (iconLabel.ForeColor == iconLabel.BackColor) // still have covered cards
                        return;
                }
            }

            // If the loop didn’t return, it didn't find
            // any unmatched icons
            // That means the user won. Show a message and close the form
            MessageBox.Show("You matched all the icons!", "Congratulations");
            Close();
        }
    }
}
