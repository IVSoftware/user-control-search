using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace user_control_search
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
            guna2TextBox5.TextChanged += guna2TextBox5_TextChanged;
        }
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            string folder = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "stackoverflow",
                typeof(MainForm).Assembly.GetName().Name,
                "Images");
            Directory.CreateDirectory(folder);
            foreach (var fullPath in Directory.GetFiles(folder))
            {
                flowLayoutPanel.Controls.Add(new Guna2Panel
                {
                    FullPath = fullPath,
                    Margin = new Padding(5),
                });
            }
        }

        private void guna2TextBox5_TextChanged(object sender, EventArgs e)
        {
            if(string.IsNullOrWhiteSpace(guna2TextBox5.Text))
            {
                foreach (var userControl in flowLayoutPanel.Controls.OfType<Guna2Panel>())
                {
                    userControl.Visible = true;
                }
            }
            else
            {
                foreach (var userControl in flowLayoutPanel.Controls.OfType<Guna2Panel>())
                {
                    userControl.Visible = userControl.TitleLabelText.Contains(guna2TextBox5.Text, StringComparison.OrdinalIgnoreCase);
                }
            }
        }
    }
}
