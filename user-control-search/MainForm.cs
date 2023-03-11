using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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

            #region T E S T I N G
            createTestData(folder);
            foreach (var fullPath in Directory.GetFiles(folder))
            {
                flowLayoutPanel.Controls.Add(new Guna2Panel
                {
                    FullPath = fullPath,
                    Margin = new Padding(5),
                });
            }
            #endregion T E S T I N G
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

        private void createTestData(string dir)
        {
            var assy = typeof(MainForm).Assembly;
            var token = ".Images.";
            foreach (var resource in assy.GetManifestResourceNames().Where(_=>_.Contains(token)))
            {
                var fileName = resource.Substring(resource.IndexOf(token) + token.Length);
                var fullPath = Path.Combine(dir, fileName);
                if(!File.Exists(fullPath))
                {
                    using (var stream = assy.GetManifestResourceStream(resource))
                    using (FileStream fileStream = new FileStream(fullPath, FileMode.Create))
                    {
                        var bytes = new byte[stream.Length];
                        stream.Read(bytes, 0, (int)stream.Length);
                        fileStream.Write(bytes, 0, bytes.Length);
                    }
                }
            }
        }
    }
}
