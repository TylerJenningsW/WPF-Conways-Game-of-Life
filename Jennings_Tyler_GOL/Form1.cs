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
        // Second universe array to copy from
        bool[,] scratchPad = new bool[40, 40];

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
            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // Implement the game logic
                    int count = CountNeighborsFinite(x, y);
                    if (count < 2)
                    {
                        scratchPad[x, y] = false;
                    }
                    else if (count > 3)
                    {
                        scratchPad[x, y] = false;
                    }
                    else if (universe[x, y] == true && (count == 2 || count == 3))
                    {
                        scratchPad[x, y] = true;
                    }
                    else if (universe[x, y] == false && (count == 3))
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
            NextGeneration();
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
                    cellRect.X = (float)x * cellWidth;
                    cellRect.Y = (float)y * cellHeight;
                    cellRect.Width = cellWidth;
                    cellRect.Height = cellHeight;

                    // font for displaying neighbor count
                    #region font
                    Font font = new Font("Arial", 24, FontStyle.Bold, GraphicsUnit.Point);
                    StringFormat drawFormat = new StringFormat();
                    drawFormat.LineAlignment = StringAlignment.Center;
                    drawFormat.Alignment = StringAlignment.Center;
                    int neighbors = CountNeighborsFinite(x, y);
                    #endregion

                    // Fill the cell with a brush if alive
                    if (universe[x, y] == true && neighbors == 1 || neighbors > 3)
                    {
                        // alive
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                        e.Graphics.DrawString(neighbors.ToString(), font, cellBrushDead, cellRect, drawFormat);
                    }
                    else if (universe[x, y] == true && neighbors <= 3 && neighbors != 0)
                    {
                        // alive
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                        e.Graphics.DrawString(neighbors.ToString(), font, cellBrushAlive, cellRect, drawFormat);
                    }
                    else if (universe[x, y] == true)
                    {
                        // alive
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                    }
                    else if (neighbors == 3)
                    {
                        e.Graphics.DrawString(neighbors.ToString(), font, cellBrushAlive, cellRect, drawFormat);
                    }
                    else if (universe[x, y] == false && neighbors != 0)
                    {
                        // dead
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
            this.Close();
        }

        private void cellDimensionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
        }

        private void Run_Click(object sender, EventArgs e)
        {
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
                    universe[x, y] = false;
                }
            }
            graphicsPanel1.Invalidate();
        }

        private void Stop_Click(object sender, EventArgs e)
        {
            timer.Enabled = false;
        }

        private void Next_Click(object sender, EventArgs e)
        {
            NextGeneration();
            graphicsPanel1.Invalidate();
        }
    }
}
