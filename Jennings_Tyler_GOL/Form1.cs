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
        bool[,] universe;
        // universe dimensions
        public int uWidth = Properties.Settings.Default.uWidth;
        public int uHeight = Properties.Settings.Default.uHeight;
        // timer interval
        public int interval = Properties.Settings.Default.interval;
        // universe seed
        int seed = 0;
        // Drawing colors
        Color gridColor = Color.Black;
        Color gridColorx10 = Color.Black;
        Color cellColor = Color.Gray;
        Color cellAlive = Color.Green;
        Color cellDead = Color.Red;

        // The Timer class
        Timer timer = new Timer();

        // Generation count
        int generations = 0;

        bool isToroidal;
        bool showNeighbors;
        bool gridCheck;
        bool hudCheck;

        public Form1()
        {
            InitializeComponent();
            // Read the properties
            LoadSettings();
            ConditionChecks();
            // Setup the timer
            timer.Interval = interval; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // start timer running
        }
        private void ConditionChecks()
        {
            if (toroidalToolStripMenuItem.Checked == true)
            {
                isToroidal = true;
            }
            else
            {
                isToroidal = false;
            }
            #region show neighbors
            if (neighborCountToolStripMenuItem.Checked == true)
            {
                showNeighbors = true;
                neighborCountContextMenuItem.Checked = true;
            }
            else
            {
                neighborCountContextMenuItem.Checked = true;
                showNeighbors = false;
            }
            if (neighborCountContextMenuItem.Checked == true)
            {
                neighborCountToolStripMenuItem.Checked = true;
            }
            else
            {
                neighborCountToolStripMenuItem.Checked = false;
                showNeighbors = false;

            }
            #endregion

            #region grid
            if (gridToolStripMenuItem.Checked == true)
            {
                gridCheck = true;
            }
            else
            {
                gridCheck = false;
            }
            #endregion
            if (hUDToolStripMenuItem.Checked == true)
            {
                hudCheck = true;
            }
            else
            {
                hudCheck = false;
            }
        }
        private void LoadSettings()
        {
            universe = new bool[uWidth, uHeight];
            uWidth = Properties.Settings.Default.uWidth;
            uHeight = Properties.Settings.Default.uHeight;
            interval = Properties.Settings.Default.interval;
            graphicsPanel1.BackColor = Properties.Settings.Default.PanelColor;
            gridColor = Properties.Settings.Default.gridColor;
            gridColorx10 = Properties.Settings.Default.gridColorx10;
            cellColor = Properties.Settings.Default.cellColor;
            seed = Properties.Settings.Default.Seed;
        }
        // Calculate the next generation of cells
        private void NextGeneration()
        {
            // Second universe array to copy from
            bool[,] scratchPad = new bool[uWidth, uHeight];
            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                ConditionChecks();
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    int count;
                    if (isToroidal)
                    {
                        count = CountNeighborsToroidal(x, y);
                    }
                    else
                    {
                        count = CountNeighborsFinite(x, y);
                    }
                    // Implement the game logic
                    // A. Living cells with less than 2 living neighbors die in the next generation.
                    // B. Living cells with more than 3 living neighbors die in the next generation.
                    // C. Living cells with 2 or 3 living neighbors live in the next generation.
                    // D. Dead cells with exactly 3 living neighbors live in the next generation.
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
            Pen gridPenx10 = new Pen(gridColorx10, 4);

            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);
            Brush cellBrushAlive = new SolidBrush(cellAlive);

            // A Brush for filling dead cells interiors (color)
            Brush cellBrushDead = new SolidBrush(cellDead);
            // font for displaying neighbor count
            #region font
            Font font = new Font(new FontFamily("Arial"), 20);

            // text format for centering text
            StringFormat drawFormat = new StringFormat();
            // the centering
            drawFormat.LineAlignment = StringAlignment.Center;
            drawFormat.Alignment = StringAlignment.Center;
            // the neighbors to count
            #endregion

            int cellCount = 0;

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

                    int neighbors = CountNeighborsFinite(x, y);
                    // Fill the cell with a brush if alive
                    // rectangle fill indicates alive aka true
                    // draw string indicates neighbor count and dead (red) or alive (green)
                    if (universe[x, y] == true)
                    {
                        cellCount++;
                    }
                    if (showNeighbors == false && universe[x, y] == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                    }
                    else if (showNeighbors == false)
                    {
                        e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                        continue;
                    }
                    else
                    {
                        if (universe[x, y] == true && neighbors == 0)
                        {
                            // alive now and dead in the next gen, hide '0' string
                            e.Graphics.FillRectangle(cellBrush, cellRect);
                        }
                        else if ((universe[x, y] == true && neighbors < 2))
                        {
                            // alive now and dead in the next gen
                            e.Graphics.FillRectangle(cellBrush, cellRect);
                            e.Graphics.DrawString(neighbors.ToString(), font, cellBrushDead, cellRect, drawFormat);
                        }
                        else if ((universe[x, y] == true && neighbors > 3))
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
                    }
                    // Outline the cell with a pen
                    if (gridCheck == true)
                    {
                        e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                    }

                    if (y % 10 == 0)
                    {
                        e.Graphics.DrawLine(gridPenx10, 0, cellRect.Y, graphicsPanel1.Width, cellRect.Y);
                    }
                    if (x % 10 == 0)
                    {
                        e.Graphics.DrawLine(gridPenx10, cellRect.X, 0, cellRect.X, graphicsPanel1.Height);

                    }
                }
            }
            string boundaryType;
            if (isToroidal)
            {
                boundaryType = "Toroidal";
            }
            else
            {
                boundaryType = "Finite";
            }
            string HUD =
                $"Generations: {generations}\n" +
                $"Cell Count: {cellCount}\n" +
                $"Boundary Type: {boundaryType}\n" +
                $"Universe Size: Width={uWidth}, Height={uHeight}";
            if (hudCheck == true)
            {
                e.Graphics.DrawString(HUD, font, Brushes.Aqua, 0, 0);
            }
            // Cleaning up pens and brushes
            gridPen.Dispose();
            gridPenx10.Dispose();
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
        private int CountNeighborsToroidal(int x, int y)
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
                    // if xCheck is less than 0 then set to xLen - 1
                    if (xCheck < 0)
                    {
                        xCheck = xLen - 1;
                    }
                    // if yCheck is less than 0 then set to yLen - 1
                    if (yCheck < 0)
                    {
                        yCheck = yLen - 1;
                    }
                    // if xCheck is greater than or equal too xLen then set to 0
                    if (xCheck >= xLen)
                    {
                        xCheck = 0;
                    }
                    // if yCheck is greater than or equal too yLen then set to 0
                    if (yCheck >= yLen)
                    {
                        yCheck = 0;
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

        private void Run_Click(object sender, EventArgs e)
        {
            // begin the game
            timer.Enabled = true;
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
            colorDialog.Color = graphicsPanel1.BackColor;
            if (DialogResult.OK == colorDialog.ShowDialog())
            {
                graphicsPanel1.BackColor = colorDialog.Color;
            }
            graphicsPanel1.Invalidate();

        }

        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OptionsDialog optionsDialog = new OptionsDialog();
            int tempW = uWidth;
            int tempH = uHeight;
            optionsDialog.TimeInterval = interval;
            optionsDialog.UniverseWidth = uWidth;
            optionsDialog.UniverseHeight = uHeight;
            if (DialogResult.OK == optionsDialog.ShowDialog())
            {
                interval = optionsDialog.TimeInterval;
                uWidth = optionsDialog.UniverseWidth;
                uHeight = optionsDialog.UniverseHeight;
                timer.Interval = interval;
            }
            if (tempW != uWidth || tempH != uHeight)
            {
                universe = new bool[uWidth, uHeight];
            }
            graphicsPanel1.Invalidate();
        }

        private void cellColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.Color = cellColor;
            if (DialogResult.OK == colorDialog.ShowDialog())
            {
                cellColor = colorDialog.Color;
                graphicsPanel1.Invalidate();
            }
        }

        private void gridColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ColorDialog colorDialog = new ColorDialog();
            colorDialog.Color = gridColor;
            if (DialogResult.OK == colorDialog.ShowDialog())
            {
                gridColor = colorDialog.Color;
                graphicsPanel1.Invalidate();
            }
        }

        private void gridx10ColorToolStripMenuItem_Click(object sender, EventArgs e)
        {

            ColorDialog colorDialog = new ColorDialog();
            colorDialog.Color = gridColorx10;
            if (DialogResult.OK == colorDialog.ShowDialog())
            {
                gridColorx10 = colorDialog.Color;
                graphicsPanel1.Invalidate();
            }
        }
        #region random menu methods
        private void Randomize()
        {
            Random rand = new Random(seed);
            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    int rng = rand.Next(-1, 2);
                    if (rng == -1)
                    {
                        universe[x, y] = true;
                    }
                    else
                    {
                        universe[x, y] = false;
                    }
                }
            }
        }

        private void fromSeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SeedDialog seedDialog = new SeedDialog();
            seedDialog.Seed = seed;
            if (DialogResult.OK == seedDialog.ShowDialog())
            {
                seed = seedDialog.Seed;
                Randomize();
            }
            graphicsPanel1.Invalidate();
        }

        private void fromCurrentSeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Randomize();
            graphicsPanel1.Invalidate();
        }

        private void fromTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            seed = DateTime.Now.Day;
            Randomize();
            graphicsPanel1.Invalidate();
        }
        #endregion


        #region View menu methods
        private void hUDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            hUDContextMenuItem.Checked = hUDToolStripMenuItem.Checked;
            ConditionChecks();
            graphicsPanel1.Invalidate();
        }

        private void HUDContextMenuItem_Click(object sender, EventArgs e)
        {
            hUDToolStripMenuItem.Checked = hUDContextMenuItem.Checked;
            ConditionChecks();
            graphicsPanel1.Invalidate();
        }
        private void neighborCountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            neighborCountContextMenuItem.Checked = neighborCountToolStripMenuItem.Checked;

            ConditionChecks();
            graphicsPanel1.Invalidate();
        }
        private void neighborCountContextMenuItem_Click(object sender, EventArgs e)
        {
            neighborCountToolStripMenuItem.Checked = neighborCountContextMenuItem.Checked;
            ConditionChecks();
            graphicsPanel1.Invalidate();
        }

        private void gridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            gridContextMenuItem.Checked = gridToolStripMenuItem.Checked;
            ConditionChecks();
            graphicsPanel1.Invalidate();
        }
        private void gridContextMenuItem_Click(object sender, EventArgs e)
        {
            gridToolStripMenuItem.Checked = gridContextMenuItem.Checked;
            ConditionChecks();
            graphicsPanel1.Invalidate();
        }
        #endregion

        private void Form1_FormClosed(object sender, FormClosedEventArgs e)
        {
            // Update the properties
            Properties.Settings.Default.uWidth = uWidth;
            Properties.Settings.Default.uHeight = uHeight;
            Properties.Settings.Default.interval = interval;
            Properties.Settings.Default.PanelColor = graphicsPanel1.BackColor;
            Properties.Settings.Default.gridColor = gridColor;
            Properties.Settings.Default.gridColorx10 = gridColorx10;
            Properties.Settings.Default.cellColor = cellColor;
            Properties.Settings.Default.Seed = seed;
            Properties.Settings.Default.Save();
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reset();
            LoadSettings();
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Properties.Settings.Default.Reload();
            LoadSettings();
        }

    }
}
