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
        #region Properties
        public int UniverseWidth
        {
            get
            {
                // get the value from the numeric up/down universe width window
                return (int)universeWidthUpDown.Value;
            }
            set
            {
                // set the value from the numeric up/down universe width window equal to a value
                universeWidthUpDown.Value = value;
            }
        }
        public int UniverseHeight
        {
            get
            {
                // get the value from the numeric up/down universe height window
                return (int)universeHeightUpDown.Value;
            }
            set
            {
                // set the value from the numeric up/down universe height window equal to a value
                universeHeightUpDown.Value = value;
            }
        }
        public int TimeInterval
        {
            get
            {
                // get the value from the numeric up/down timer interval window
                return (int)intervalUpDown.Value;
            }
            set
            {
                // set the value from the numeric up/down timer interval window equal to a value
                intervalUpDown.Value = value;
            }
        }
        #endregion
    }
}
