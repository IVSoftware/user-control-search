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
            textBoxSearch.TextChanged += onSearchTextChanged;
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

        private void onSearchTextChanged(object sender, EventArgs e)
        {
            if(string.IsNullOrWhiteSpace(textBoxSearch.Text))
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
                    userControl.Visible = userControl.FileName.Contains(textBoxSearch.Text, StringComparison.OrdinalIgnoreCase);
                }
            }
        }
    }
}
