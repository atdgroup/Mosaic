using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace ImageStitching
{
    public partial class TextStringDialog : Form
    {
        public TextStringDialog()
        {
            InitializeComponent();
        }

        public string TextString
        {
            get
            {
                return this.textBox.Text;
            }
            set
            {
                this.textBox.Text = value;
            }
        }

        public string Description
        {
            set
            {
                this.descriptionLabel.Text = value;
            }
        }
    }
}
