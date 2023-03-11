How you **search for control in flowlayout based on the label text** is a matter of programming style. What I see here is that the object you call "**Generated Panel**" is a candidate to be a custom `UserControl` and this control should expose a public property `FileName` that is what you will use to search.

[![user control][1]][1]

    public partial class Guna2Panel : UserControl
    {
        public Guna2Panel() => InitializeComponent();

        private FileInfo _fileInfo;
        public string FullPath
        {
            get => _fileInfo.FullName;
            set
            {
                _fileInfo = new FileInfo(value);
                Refresh();
            }
        }
        public string FileName => _fileInfo.Name;
        public DateTime Date
        {
            get => _fileInfo.LastWriteTime;
            set 
            {
                _fileInfo.LastWriteTime = value;
                Refresh();
            }
        }
        public new void Refresh() 
        {
            labelPath.Text = _fileInfo.Name;
            labelDate.Text = _fileInfo.LastWriteTime.ToShortDateString();
            pictureBox.Image = Image.FromFile(FullPath);

            // https://stackoverflow.com/a/23400751/5438626
            if (Array.IndexOf(pictureBox.Image.PropertyIdList, 274) > -1)
            {
                var orientation = (int)pictureBox.Image.GetPropertyItem(274).Value[0];
                switch (orientation)
                {
                    case 1:
                        // No rotation required.
                        break;
                    case 3:
                        pictureBox.Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        break;
                    case 4:
                        pictureBox.Image.RotateFlip(RotateFlipType.Rotate180FlipX);
                        break;
                    case 5:
                        pictureBox.Image.RotateFlip(RotateFlipType.Rotate90FlipX);
                        break;
                    case 6:
                        pictureBox.Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        break;
                    case 7:
                        pictureBox.Image.RotateFlip(RotateFlipType.Rotate270FlipX);
                        break;
                    case 8:
                        pictureBox.Image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        break;
                }
                // This EXIF data is now invalid and should be removed.
                pictureBox.Image.RemovePropertyItem(274);
            }
            base.Refresh();
        }
    }

***
**Search**

This makes searching clean and easy in main form.

[![search][2]][2]

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


  [1]: https://i.stack.imgur.com/Xh6zV.png
  [2]: https://i.stack.imgur.com/inVdC.png