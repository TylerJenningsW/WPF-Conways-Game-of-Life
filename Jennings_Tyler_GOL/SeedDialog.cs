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
    public partial class SeedDialog : Form
    {
        public SeedDialog()
        {
            InitializeComponent();
        }
        #region Properties
        public int Seed
        {
            get
            {
                return (int)seedUpDown.Value;
            }
            set
            {
                seedUpDown.Value = (int)value;
            }
        }
        #endregion
        private void randomizeButton_Click(object sender, EventArgs e)
        {
            // random seed
            Random rng = new Random();
            Seed = rng.Next();
        }
    }
}
