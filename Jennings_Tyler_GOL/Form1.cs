using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Jennings_Tyler_GOL
{
    public partial class Form1 : Form
    {
        // The universe array
        bool[,] universe = new bool[40, 40];

        // Drawing colors
        Color gridColor = Color.Black;
        Color cellColor = Color.Gray;
        Color cellAlive = Color.Green;
        Color cellDead = Color.Red;

        // The Timer class
        Timer timer = new Timer();

        // Generation count
        int generations = 0;

        public Form1()
        {
            InitializeComponent();

            // Setup the timer
            timer.Interval = 100; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // start timer running
        }

        // Calculate the next generation of cells
        private void NextGeneration()
        {

            // Second universe array to copy from
            bool[,] scratchPad = new bool[40, 40];
            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // Implement the game logic
                    // A. Living cells with less than 2 living neighbors die in the next generation.
                    // B. Living cells with more than 3 living neighbors die in the next generation.
                    // C. Living cells with 2 or 3 living neighbors live in the next generation.
                    // D. Dead cells with exactly 3 living neighbors live in the next generation.
                    int count = CountNeighborsFinite(x, y);
                    // A.
                    if (universe[x, y] == true && count < 2)
                    {
                        scratchPad[x, y] = false;
                    }
                    // B.
                    else if (universe[x, y] == true && count > 3)
                    {
                        scratchPad[x, y] = false;
                    }
                    // C.
                    else if (universe[x, y] == true && count == 2)
                    {
                        scratchPad[x, y] = true;
                    }
                    // C. 
                    else if (universe[x, y] == true && count == 3)
                    {
                        scratchPad[x, y] = true;
                    }
                    // D.
                    else if (universe[x, y] == false && count == 3)
                    {
                        scratchPad[x, y] = true;
                    }
                }
            }
            // swap
            bool[,] temp = universe;
            universe = scratchPad;
            scratchPad = temp;

            // Increment generation count
            generations++;

            // Update status strip generations
            toolStripStatusLabelGenerations.Text = "Generations = " + generations.ToString();
        }
        
        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            // Call the next gen for every tie the clock ticks
            NextGeneration();
            // repaint to update
            graphicsPanel1.Invalidate();
        }

        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
            // Calculate the width and height of each cell in pixels
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            float cellWidth = ((float)graphicsPanel1.ClientSize.Width) / ((float)universe.GetLength(0));
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            float cellHeight = ((float)graphicsPanel1.ClientSize.Height) / ((float)universe.GetLength(1));

            // A Pen for drawing the grid lines (color, width)
            Pen gridPen = new Pen(gridColor, 1);

            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);
            Brush cellBrushAlive = new SolidBrush(cellAlive);

            // A Brush for filling dead cells interiors (color)
            Brush cellBrushDead = new SolidBrush(cellDead);

            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // A rectangle to represent each cell in pixels
                    RectangleF cellRect = RectangleF.Empty;
                    // Edge case explicit float cast
                    cellRect.X = (float)x * cellWidth;
                    cellRect.Y = (float)y * cellHeight;
                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;

                    // font for displaying neighbor count
                    #region font
                    Font font = new Font("Arial", 24, FontStyle.Bold, GraphicsUnit.Point);
                    // text format for centering text
                    StringFormat drawFormat = new StringFormat();
                    // the centering
                    drawFormat.LineAlignment = StringAlignment.Center;
                    drawFormat.Alignment = StringAlignment.Center;
                    // the neighbors to count
                    int neighbors = CountNeighborsFinite(x, y);
                    #endregion

                    // Fill the cell with a brush if alive
                    // rectangle fill indicates alive aka true
                    // draw string indicates neighbor count and dead (red) or alive (green)
                    if (universe[x, y] == true && neighbors == 0)
                    {
                        // alive now and dead in the next gen, hide '0' string
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                    }
                    else if (universe[x, y] == true && neighbors < 2)
                    {
                        // alive now and dead in the next gen
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                        e.Graphics.DrawString(neighbors.ToString(), font, cellBrushDead, cellRect, drawFormat);
                    }
                    else if (universe[x, y] == true && neighbors > 3)
                    {
                        // alive now and dead in the next gen
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                        e.Graphics.DrawString(neighbors.ToString(), font, cellBrushDead, cellRect, drawFormat);
                    }
                    else if (universe[x, y] == true && neighbors == 2)
                    {
                        // alive now and in the next gen
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                        e.Graphics.DrawString(neighbors.ToString(), font, cellBrushAlive, cellRect, drawFormat);
                    }
                    else if (universe[x, y] == true && neighbors == 3)
                    {
                        // alive now and in the next gen
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                        e.Graphics.DrawString(neighbors.ToString(), font, cellBrushAlive, cellRect, drawFormat);
                    }
                    else if (universe[x, y] == false && neighbors == 3)
                    {
                        // dead now and alive in the next gen
                        e.Graphics.DrawString(neighbors.ToString(), font, cellBrushAlive, cellRect, drawFormat);
                    }
                    else if (universe[x, y] == false && neighbors > 0)
                    {
                        // dead now and in the next gen, avoid '0' spam
                        e.Graphics.DrawString(neighbors.ToString(), font, cellBrushDead, cellRect, drawFormat);
                    }

                    // Outline the cell with a pen
                    e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                }
            }

            // Cleaning up pens and brushes
            gridPen.Dispose();
            cellBrush.Dispose();
            cellBrushDead.Dispose();
            cellBrushAlive.Dispose();

        }


        private void graphicsPanel1_MouseClick(object sender, MouseEventArgs e)
        {
            // If the left mouse button was clicked
            if (e.Button == MouseButtons.Left)
            {
                // Calculate the width and height of each cell in pixels
                // convert to floats explicitly for edge cases
                float cellWidth = ((float)graphicsPanel1.ClientSize.Width) / ((float)universe.GetLength(0));
                float cellHeight = ((float)graphicsPanel1.ClientSize.Height) / ((float)universe.GetLength(1));

                // Calculate the cell that was clicked in
                // CELL X = MOUSE X / CELL WIDTH
                float tempX = (float)e.X / cellWidth;
                // CELL Y = MOUSE Y / CELL HEIGHT
                float tempY = (float)e.Y / cellHeight;
                int x = (int)tempX;
                int y = (int)tempY;
                // Toggle the cell's state
                universe[x, y] = !universe[x, y];
                // Tell Windows you need to repaint
                graphicsPanel1.Invalidate();
            }
        }

        private int CountNeighborsFinite(int x, int y)
        {
            int count = 0;
            int xLen = universe.GetLength(0);
            int yLen = universe.GetLength(1);
            for (int yOffset = -1; yOffset <= 1; yOffset++)
            {
                for (int xOffset = -1; xOffset <= 1; xOffset++)
                {
                    int xCheck = x + xOffset;
                    int yCheck = y + yOffset;
                    // if xOffset and yOffset are both equal to 0 then continue
                    if (xOffset == 0 && yOffset == 0)
                    {
                        continue;
                    }
                    // if xCheck is less than 0 then continue
                    if (xCheck < 0)
                    {
                        continue;
                    }
                    // if yCheck is less than 0 then continue
                    if (yCheck < 0)
                    {
                        continue;
                    }
                    // if xCheck is greater than or equal too xLen then continue
                    if (xCheck >= xLen)
                    {
                        continue;
                    }
                    // if yCheck is greater than or equal too yLen then continue
                    if (yCheck >= yLen)
                    {
                        continue;
                    }

                    if (universe[xCheck, yCheck] == true) count++;
                }
            }
            return count;
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // exit the game
            this.Close();
        }

        private void Run_Click(object sender, EventArgs e)
        {
            // begin the game
            timer.Enabled = true;
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // reset the entire array to false
                    universe[x, y] = false;
                }
            }
            // repaint
            graphicsPanel1.Invalidate();
        }

        private void Stop_Click(object sender, EventArgs e)
        {
            // pause the game
            timer.Enabled = false;
        }

        private void Next_Click(object sender, EventArgs e)
        {
            // jump to the next generation and repaint
            NextGeneration();
            graphicsPanel1.Invalidate();
        }

        private void colorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();

            if (DialogResult.OK == colorDialog.ShowDialog())
            {
                graphicsPanel1.BackColor = colorDialog.Color;
            }
            graphicsPanel1.Invalidate();

        }

        private void modalToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
}
