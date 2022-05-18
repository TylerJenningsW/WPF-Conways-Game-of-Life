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
    public partial class OptionsDialog : Form
    {
        public OptionsDialog()
        {
            InitializeComponent();
        }
        public int UniverseWidth
        {
            get
            {
                return (int)universeWidthUpDown.Value;
            }
            set
            {
                universeWidthUpDown.Value = value;
            }
        }
        public int UniverseHeight
        {
            get
            {
                return (int)universeHeightUpDown.Value;
            }
            set
            {
                universeHeightUpDown.Value = value;
            }
        }
        public int TimeInterval
        {
            get
            {
                return (int)intervalUpDown.Value;
            }
            set
            {
                intervalUpDown.Value = value;
            }
        }
    }
}
