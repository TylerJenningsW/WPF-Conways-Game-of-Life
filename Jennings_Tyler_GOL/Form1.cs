using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Jennings_Tyler_GOL
{
    public partial class Form1 : Form
    {
        #region Data members
        // The universe array
        bool[,] universe;
        // universe dimensions
        public int uWidth = Properties.Settings.Default.uWidth;
        public int uHeight = Properties.Settings.Default.uHeight;
        // timer interval
        public int interval = Properties.Settings.Default.interval;
        // universe seed
        int seed = Properties.Settings.Default.Seed;
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

        // Living cell count
        int cellCount = 0;

        // Bools for clarity in view menu, see LoadSettings()
        bool isToroidal = Properties.Settings.Default.isToroidal;
        bool showNeighbors = Properties.Settings.Default.showNeighbors;
        bool gridCheck = Properties.Settings.Default.gridCheck;
        bool hudCheck = Properties.Settings.Default.hudCheck;
        #endregion

        #region Form constructor
        public Form1()
        {
            InitializeComponent();
            // Read the properties
            LoadSettings();
            // Setup conditionals
            ConditionChecks();
            // Setup the timer
            timer.Interval = interval; // milliseconds
            timer.Tick += Timer_Tick;
            timer.Enabled = false; // start timer
        }
        #endregion

        #region Game Startup
        private void ConditionChecks()
        {
            #region Toroidal & Finite
            toroidalToolStripMenuItem.Checked = isToroidal;
            if (isToroidal == false)
            {
                finiteToolStripMenuItem.Checked = true;
            }
            else
            {
                finiteToolStripMenuItem.Checked = false;
            }
            if (finiteToolStripMenuItem.Checked == false)
            {
                toroidalToolStripMenuItem.Checked = true;
                isToroidal = true;
            }
            #endregion

            #region Show Neighbors
            neighborCountToolStripMenuItem.Checked = showNeighbors;
            neighborCountContextMenuItem.Checked = showNeighbors;
            #endregion

            #region Grid
            gridToolStripMenuItem.Checked = gridCheck;
            gridContextMenuItem.Checked = gridCheck;
            #endregion

            #region HUD
            hUDToolStripMenuItem.Checked = hudCheck;
            hUDContextMenuItem.Checked = hudCheck;
            #endregion
        }
        private void LoadSettings()
        {
            // All of the settings to initialize at the start
            universe = new bool[uWidth, uHeight]; // the universe array
            uWidth = Properties.Settings.Default.uWidth; // universe width
            uHeight = Properties.Settings.Default.uHeight; // universe height
            interval = Properties.Settings.Default.interval; // timer speed
            graphicsPanel1.BackColor = Properties.Settings.Default.PanelColor; // panel background
            gridColor = Properties.Settings.Default.gridColor; // inner grid
            gridColorx10 = Properties.Settings.Default.gridColorx10; // outer grid
            cellColor = Properties.Settings.Default.cellColor; // living cell color
            seed = Properties.Settings.Default.Seed; // seed to generate from
            isToroidal = Properties.Settings.Default.isToroidal; // toroidal neighbor method
            showNeighbors = Properties.Settings.Default.showNeighbors; // display neighbors
            gridCheck = Properties.Settings.Default.gridCheck; // display grid
            hudCheck = Properties.Settings.Default.hudCheck; // display hud
            toolStripStatusLabelInterval.Text = $"Interval = {interval}"; // timer speed
            toolStripStatusLabelAlive.Text = $"Alive: {cellCount}"; // live cells
            toolStripStatusLabelSeed.Text = $"Seed: {seed}"; // seed to generate from
        }
        #endregion

        #region Display
        public static Font GetAdjustedFont(Graphics graphic, string str, Font originalFont, SizeF containerSize)
        {
            // We utilize MeasureString which we get via a control instance           
            for (int adjustedSize = (int)originalFont.Size; adjustedSize >= 1; adjustedSize-=4)
            {
                Font testFont = new Font(originalFont.Name, adjustedSize, originalFont.Style, GraphicsUnit.Pixel);

                // Test the string with the new size
                SizeF adjustedSizeNew = graphic.MeasureString(str, testFont, (int)containerSize.Width);
                
                if (containerSize.Height > Convert.ToInt32(adjustedSizeNew.Height))
                {
                    // Good font, return it
                    return testFont;
                }
            }
            return new Font(originalFont.Name, 1, originalFont.Style, GraphicsUnit.Pixel);
        }
        
        private void graphicsPanel1_Paint(object sender, PaintEventArgs e)
        {
            // Calculate the width and height of each cell in pixels
            // CELL WIDTH = WINDOW WIDTH / NUMBER OF CELLS IN X
            float cellWidth = ((float)graphicsPanel1.ClientSize.Width) / ((float)universe.GetLength(0));
            // CELL HEIGHT = WINDOW HEIGHT / NUMBER OF CELLS IN Y
            float cellHeight = ((float)graphicsPanel1.ClientSize.Height) / ((float)universe.GetLength(1));

            #region Pens and brushes
            // A Pen for drawing the grid lines (color, width)
            Pen gridPen = new Pen(gridColor, 1);
            Pen gridPenx10 = new Pen(gridColorx10, 4);

            // A Brush for filling living cells interiors (color)
            Brush cellBrush = new SolidBrush(cellColor);
            Brush cellBrushAlive = new SolidBrush(cellAlive);

            // A Brush for filling dead cells interiors (color)
            Brush cellBrushDead = new SolidBrush(cellDead);
            #endregion

            // font for displaying neighbor count
            #region font
            Font font = new Font(new FontFamily("Arial"), 18);
            
            // text format for centering text
            StringFormat drawFormat = new StringFormat();
            // the centering
            drawFormat.LineAlignment = StringAlignment.Center;
            drawFormat.Alignment = StringAlignment.Center;
            // the neighbors to count
            #endregion
            cellCount = 0;
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

                    int neighbors;
                    if (isToroidal == true)
                    {
                        neighbors = CountNeighborsToroidal(x, y);
                    }
                    else
                    {
                        neighbors = CountNeighborsFinite(x, y);
                    }
                    if (universe[x, y] == true)
                    {
                        cellCount++;
                    }
                    // Update living cell status
                    toolStripStatusLabelAlive.Text = $"Alive: {cellCount}";
                    // Fill the cell with a brush if alive
                    // rectangle fill indicates alive aka true
                    // draw string indicates neighbor count and dead (red) or alive (green)
                    if (showNeighbors == false && gridCheck == true)
                    {
                        e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                    }
                    if (showNeighbors == false && universe[x, y] == true)
                    {
                        e.Graphics.FillRectangle(cellBrush, cellRect);
                    }
                    // if show neighbors is checked, draw them
                    else if (showNeighbors == true)
                    {
                        Font f = GetAdjustedFont(e.Graphics, neighbors.ToString(), font, cellRect.Size);

                        if (universe[x, y] == true && neighbors == 0)
                        {
                            // alive now and dead in the next gen, hide '0' string
                            e.Graphics.FillRectangle(cellBrush, cellRect);
                        }
                        else if ((universe[x, y] == true && neighbors < 2))
                        {
                            // alive now and dead in the next gen
                            e.Graphics.FillRectangle(cellBrush, cellRect);
                            e.Graphics.DrawString(neighbors.ToString(), f, cellBrushDead, cellRect, drawFormat);
                        }
                        else if ((universe[x, y] == true && neighbors > 3))
                        {
                            // alive now and dead in the next gen
                            e.Graphics.FillRectangle(cellBrush, cellRect);
                            e.Graphics.DrawString(neighbors.ToString(), f, cellBrushDead, cellRect, drawFormat);
                        }
                        else if (universe[x, y] == true && neighbors == 2)
                        {
                            // alive now and in the next gen
                            e.Graphics.FillRectangle(cellBrush, cellRect);
                            e.Graphics.DrawString(neighbors.ToString(), f, cellBrushAlive, cellRect, drawFormat);
                        }
                        else if (universe[x, y] == true && neighbors == 3)
                        {
                            // alive now and in the next gen
                            e.Graphics.FillRectangle(cellBrush, cellRect);
                            e.Graphics.DrawString(neighbors.ToString(), f, cellBrushAlive, cellRect, drawFormat);
                        }
                        else if (universe[x, y] == false && neighbors == 3)
                        {
                            // dead now and alive in the next gen
                            e.Graphics.DrawString(neighbors.ToString(), f, cellBrushAlive, cellRect, drawFormat);
                        }
                        else if (universe[x, y] == false && neighbors > 0)
                        {
                            // dead now and in the next gen, avoid '0' spam
                            e.Graphics.DrawString(neighbors.ToString(), f, cellBrushDead, cellRect, drawFormat);
                        }
                        f.Dispose();
                    }
                    // Outline the cell with a pen
                    // if grid is enabled
                    if (gridCheck == true)
                    {
                        e.Graphics.DrawRectangle(gridPen, cellRect.X, cellRect.Y, cellRect.Width, cellRect.Height);
                    }
                    // The x10 grid to display
                    // Draw a line for every tenth y cell in the grid, if grid is checked
                    if (y % 10 == 0 && gridCheck == true)
                    {
                        e.Graphics.DrawLine(gridPenx10, 0, cellRect.Y, graphicsPanel1.Width, cellRect.Y);
                    }
                    // Draw a line for every tenth x cell in the grid, if grid is checked
                    if (x % 10 == 0 && gridCheck == true)
                    {
                        e.Graphics.DrawLine(gridPenx10, cellRect.X, 0, cellRect.X, graphicsPanel1.Height);

                    }
                }
            }
            // The type of boundary system we're currently using
            string boundaryType;
            if (isToroidal)
            {
                boundaryType = "Toroidal";
            }
            else
            {
                boundaryType = "Finite";
            }
            // The HUD string to display
            string HUD =
                $"Generations: {generations}\n" +
                $"Cell Count: {cellCount}\n" +
                $"Boundary Type: {boundaryType}\n" +
                $"Universe Size: Width={uWidth}, Height={uHeight}";
            // Display the hud if checked
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
            font.Dispose();

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
                // revert back to integers for the universe array;
                int x = (int)tempX;
                int y = (int)tempY;
                // Toggle the cell's state
                universe[x, y] = !universe[x, y];
                // Tell Windows you need to repaint
                graphicsPanel1.Invalidate();
            }
        }
        #endregion

        #region File Menu
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
            generations = 0;
            // Update status strip generations
            toolStripStatusLabelGenerations.Text = $"Generations: {generations}";
            // repaint
            graphicsPanel1.Invalidate();
        }
        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Construct save dialog
            SaveFileDialog dlg = new SaveFileDialog();
            // Filter file extension to cells file
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2; dlg.DefaultExt = "cells";
            // If we press ok
            if (DialogResult.OK == dlg.ShowDialog())
            {
                // Construct writer object to stream to
                StreamWriter writer = new StreamWriter(dlg.FileName);

                // Write any comments you want to include first.
                // Prefix all comment strings with an exclamation point.
                // Use WriteLine to write the strings to the file. 
                // It appends a CRLF for you.
                writer.WriteLine($"!{DateTime.SpecifyKind(DateTime.Now, DateTimeKind.Local)}");

                // Iterate through the universe one row at a time.
                for (int y = 0; y < universe.GetLength(1); y++)
                {
                    // Create a string to represent the current row.
                    String currentRow = string.Empty;

                    // Iterate through the current row one cell at a time.
                    for (int x = 0; x < universe.GetLength(0); x++)
                    {
                        // If the universe[x,y] is alive then append 'O' (capital O)
                        // to the row string.
                        if (universe[x, y] == true)
                        {
                            currentRow += "O";
                        }
                        // Else if the universe[x,y] is dead then append '.' (period)
                        // to the row string.
                        else if (universe[x, y] == false)
                        {
                            currentRow += ".";
                        }
                    }

                    // Once the current row has been read through and the 
                    // string constructed then write it to the file using WriteLine.
                    writer.WriteLine(currentRow);
                }
                // After all rows and columns have been written then close the file.
                writer.Close();
            }
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Construct open dialog
            OpenFileDialog dlg = new OpenFileDialog();
            // Filter file extension to cells file
            dlg.Filter = "All Files|*.*|Cells|*.cells";
            dlg.FilterIndex = 2;

            if (DialogResult.OK == dlg.ShowDialog())
            {
                StreamReader reader = new StreamReader(dlg.FileName);

                // Create a couple variables to calculate the width and height
                // of the data in the file.
                int maxWidth = 0;
                int maxHeight = 0;


                // Iterate through the file once to get its size.
                while (!reader.EndOfStream)
                {
                    // Read one row at a time.
                    string row = reader.ReadLine();

                    // If the row begins with '!' then it is a comment
                    // and should be ignored.
                    if (row.StartsWith("!"))
                    {
                        continue;
                    }
                    // If the row is not a comment then it is a row of cells.
                    // Increment the maxHeight variable for each row read.
                    else
                    {
                        maxHeight++;
                    }

                    // Get the length of the current row string
                    // and adjust the maxWidth variable if necessary.
                    if (row.Length > maxWidth)
                    {
                        maxWidth = row.Length;
                    }
                }

                int yPos = 0;
                // Resize the current universe and scratchPad
                // to the width and height of the file calculated above.
                uWidth = maxWidth;
                uHeight = maxHeight;
                // Reset the file pointer back to the beginning of the file.
                reader.BaseStream.Seek(0, SeekOrigin.Begin);

                // Iterate through the file again, this time reading in the cells.
                while (!reader.EndOfStream)
                {
                    // Read one row at a time.
                    string row = reader.ReadLine();
                    // If the row begins with '!' then
                    // it is a comment and should be ignored.
                    if (row.StartsWith("!"))
                    {
                        continue;
                    }
                    else
                    {
                        // If the row is not a comment then 
                        // it is a row of cells and needs to be iterated through.
                        for (int xPos = 0; xPos < row.Length; xPos++)
                        {
                            // If row[xPos] is a 'O' (capital O) then
                            // set the corresponding cell in the universe to alive.
                            if (row[xPos] == 'O')
                            {
                                universe[xPos, yPos] = true;
                            }
                            // If row[xPos] is a '.' (period) then
                            // set the corresponding cell in the universe to dead.
                            else if (row[xPos] == '.')
                            {

                                universe[xPos, yPos] = false;
                            }
                        }
                        yPos++;
                    }
                }

                // Close the file.
                reader.Close();

                // repaint
                graphicsPanel1.Invalidate();
            }
        }
        #endregion

        #region View Menu
        private void hUDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Keep the tool strip equal to the context menu
            hUDContextMenuItem.Checked = hUDToolStripMenuItem.Checked;
            hudCheck = hUDToolStripMenuItem.Checked;
            // repaint
            graphicsPanel1.Invalidate();
        }

        private void HUDContextMenuItem_Click(object sender, EventArgs e)
        {
            // Keep the context menu equal to the tool strip
            hUDToolStripMenuItem.Checked = hUDContextMenuItem.Checked;
            hudCheck = hUDContextMenuItem.Checked;
            // repaint
            graphicsPanel1.Invalidate();
        }
        private void neighborCountToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Keep the tool strip equal to the context menu
            neighborCountContextMenuItem.Checked = neighborCountToolStripMenuItem.Checked;
            showNeighbors = neighborCountToolStripMenuItem.Checked;
            // repaint
            graphicsPanel1.Invalidate();
        }
        private void neighborCountContextMenuItem_Click(object sender, EventArgs e)
        {
            // Keep the context menu equal to the tool strip
            neighborCountToolStripMenuItem.Checked = neighborCountContextMenuItem.Checked;
            showNeighbors = neighborCountContextMenuItem.Checked;
            // repaint
            graphicsPanel1.Invalidate();
        }

        private void gridToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Keep the tool strip equal to the context menu
            gridContextMenuItem.Checked = gridToolStripMenuItem.Checked;
            gridCheck = gridToolStripMenuItem.Checked;
            // repaint
            graphicsPanel1.Invalidate();
        }
        private void gridContextMenuItem_Click(object sender, EventArgs e)
        {
            // Keep the context menu equal to the tool strip
            gridToolStripMenuItem.Checked = gridContextMenuItem.Checked;
            gridCheck = gridContextMenuItem.Checked;
            // repaint
            graphicsPanel1.Invalidate();
        }
        private void finiteToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Keep Toroidal and Finite opposite
            if (finiteToolStripMenuItem.Checked == false)
            {
                toroidalToolStripMenuItem.Checked = true;
                isToroidal = true;
            }
            else
            {
                toroidalToolStripMenuItem.Checked = false;
                isToroidal = false;

            }
        }

        private void toroidalToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Keep Toroidal and Finite opposite
            if (toroidalToolStripMenuItem.Checked == false)
            {
                finiteToolStripMenuItem.Checked = true;
                isToroidal = false;
            }
            else
            {
                finiteToolStripMenuItem.Checked = false;
                isToroidal = true;

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
        #endregion

        #region Run Menu
        // Calculate the next generation of cells
        private void NextGeneration()
        {
            // Second universe array to copy from
            bool[,] scratchPad = new bool[uWidth, uHeight];
            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    int count;
                    // Determinant for 
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
            toolStripStatusLabelGenerations.Text = $"Generations: {generations}";
        }

        // The event called by the timer every Interval milliseconds.
        private void Timer_Tick(object sender, EventArgs e)
        {
            // Call the next gen for every tie the clock ticks
            NextGeneration();
            // repaint to update
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
        #endregion

        #region Random Menu
        private void Randomize()
        {
            // Construct Random object and seed it
            Random rand = new Random(seed);
            // Iterate through the universe in the y, top to bottom
            for (int y = 0; y < universe.GetLength(1); y++)
            {
                // Iterate through the universe in the x, left to right
                for (int x = 0; x < universe.GetLength(0); x++)
                {
                    // Set 1 of 3 Cells to alive
                    int rng = rand.Next(-1, 2);
                    if (rng == -1)
                    {
                        universe[x, y] = true; // alive
                    }
                    else
                    {
                        universe[x, y] = false; // dead
                    }
                }
            }
        }

        private void fromSeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Construct Seed dialog object from custom form
            SeedDialog seedDialog = new SeedDialog();
            // Set / mutate
            seedDialog.Seed = seed;
            if (DialogResult.OK == seedDialog.ShowDialog())
            {
                // Get / access
                seed = seedDialog.Seed;
                // Call randomize method to fill the universe using the new seed
                Randomize();
            }
            // Display the new seed status
            toolStripStatusLabelSeed.Text = $"Seed: {seed}";
            // repaint
            graphicsPanel1.Invalidate();
        }

        private void fromCurrentSeedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Randomize from whatever the seed is currently
            Randomize();
            // repaint
            graphicsPanel1.Invalidate();
        }

        private void fromTimeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // set the seen equal to the time
            seed = (int)DateTime.Now.Ticks;
            // pass in the new seed
            Randomize();
            // Display the new seed
            toolStripStatusLabelSeed.Text = $"Seed: {seed}";
            // repaint
            graphicsPanel1.Invalidate();
        }
        #endregion

        #region Settings Menu
        private void colorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Construct color dialog object
            ColorDialog colorDialog = new ColorDialog();
            // setter
            colorDialog.Color = graphicsPanel1.BackColor; // panel background color
            if (DialogResult.OK == colorDialog.ShowDialog())
            {
                // getter
                graphicsPanel1.BackColor = colorDialog.Color;// panel background color
            }
            // repaint
            graphicsPanel1.Invalidate();

        }
        private void cellColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Construct color dialog object
            ColorDialog colorDialog = new ColorDialog();
            // setter
            colorDialog.Color = cellColor; // cell color to fill rectangle with
            if (DialogResult.OK == colorDialog.ShowDialog())
            {
                // getter
                cellColor = colorDialog.Color; // cell color to fill rectangle with
                graphicsPanel1.Invalidate();
            }
        }

        private void gridColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Construct color dialog object
            ColorDialog colorDialog = new ColorDialog();
            // setter
            colorDialog.Color = gridColor; // inner grid color
            if (DialogResult.OK == colorDialog.ShowDialog())
            {
                // getter
                gridColor = colorDialog.Color; // inner grid color
                // repaint
                graphicsPanel1.Invalidate();
            }
        }

        private void gridx10ColorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Construct color dialog object
            ColorDialog colorDialog = new ColorDialog();
            // setter
            colorDialog.Color = gridColorx10; // outer grid color
            if (DialogResult.OK == colorDialog.ShowDialog())
            {
                // getter
                gridColorx10 = colorDialog.Color; // outer grid color
                // repaint
                graphicsPanel1.Invalidate();
            }
        }
        private void optionsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            // Construct the options form
            OptionsDialog optionsDialog = new OptionsDialog();
            // We only want a new universe if we actually updated it
            // So use these variables to check if we did
            int tempW = uWidth;
            int tempH = uHeight;
            //setters
            optionsDialog.TimeInterval = interval;  // timer speed
            optionsDialog.UniverseWidth = uWidth;   // universe width
            optionsDialog.UniverseHeight = uHeight; // universe height
            if (DialogResult.OK == optionsDialog.ShowDialog())
            {
                // getters
                interval = optionsDialog.TimeInterval;  // timer speed
                uWidth = optionsDialog.UniverseWidth;   // universe width
                uHeight = optionsDialog.UniverseHeight; // universe height
                // Update the timer speed
                timer.Interval = interval;
                // Update the timer status Label
                toolStripStatusLabelInterval.Text = $"Interval: {interval}";
            }
            // Is the universe different?
            if (tempW != uWidth || tempH != uHeight)
            {
                // Get a copy
                bool[,] temp = universe;
                for (int y = 0; y < temp.GetLength(1); y++)
                {
                    // Iterate through the universe in the x, left to right
                    for (int x = 0; x < temp.GetLength(0); x++)
                    {
                        // And refill the array
                        temp[x, y] = universe[x, y];
                    }
                }
                // Then new
                universe = new bool[uWidth, uHeight];
                for (int y = 0; y < universe.GetLength(1); y++)
                {
                    // Iterate through the universe in the x, left to right
                    for (int x = 0; x < universe.GetLength(0); x++)
                    {
                        // Makesure it's in range
                        if (temp.GetLength(1) > y && temp.GetLength(0) > x)
                        {
                            // And refill the array
                            universe[x, y] = temp[x, y];
                        }
                    }
                }
            }
            // repaint
            graphicsPanel1.Invalidate();
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Reset to defaults
            Properties.Settings.Default.Reset();
            // Read back in the settings
            LoadSettings();
            // repaint
            graphicsPanel1.Invalidate();
        }

        private void reloadToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Reload to saved state
            Properties.Settings.Default.Reload();
            // Read back in the settings
            LoadSettings();
            // repaint
            graphicsPanel1.Invalidate();

        }
        #endregion

        #region Closing
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // exit the game
            this.Close();
        }
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
            Properties.Settings.Default.isToroidal = isToroidal;
            Properties.Settings.Default.showNeighbors = showNeighbors;
            Properties.Settings.Default.gridCheck = gridCheck;
            Properties.Settings.Default.hudCheck = hudCheck;

            Properties.Settings.Default.Save();
        }
        #endregion
       
    }
}
