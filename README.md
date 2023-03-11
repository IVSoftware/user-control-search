There are three potential problems with your code for `guna2TextBox5_TextChanged1.
1. This method is looking for Label controls in the Controls collection of flowLayoutPanel1 that are actually in the Controls collection of the `Guna2Panel`. A others have pointed out, interating the control tree of flow layout panel _will_ find these labels, but not in a manner that is in any way efficient in terms of a search.
2. Your post says you want "to search for `titleLab` label" specifically. The current code for matches in "any" `Label`.
3. Your method is calling `Remove(c)`. But you say that "i want to keep all of the controls" and to do that you would want to set the `Visible` property of the Guna2Panel to `false` instead.


One way to address these issues is to create a `UserControl` that would be configured to match the image of "how the panel look like". This control could expose a public property `TitleLabelText` that will make filtering much more efficient.

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