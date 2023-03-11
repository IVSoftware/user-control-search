There are three potential problems with your code for `guna2TextBox5_TextChanged1`:
1. This method is looking for `Label` controls in the `Controls` collection of `flowLayoutPanel1` that are actually in the `Controls` collection of the `Guna2Panel`. A others have pointed out, interating the control tree of flow layout panel _will_ find these labels, but not in a manner that is in any way efficient in terms of a search.
2. Your post says you want **"to search for `titleLab` label"** specifically. The current code yields a positive match if "any" `Label` meets the criteria.
3. Your method is calling `Remove(c)`. But you say that **"i want to keep all of the controls"** and to do that you would want to set the `Visible` property of the Guna2Panel to `false` instead.

One way to address all of these issues is to create a `UserControl` that would be configured to match the image you posted of **"how the panel look like"**. This control could expose a public property `TitleLabelText` that will make filtering much more efficient.

***
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
        public string TitleLabelText => _fileInfo.Name;
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
            titleLab.Text = _fileInfo.Name;
            labelDate.Text = _fileInfo.LastWriteTime.ToShortDateString();
            var img =  Image.FromFile(FullPath);
            textboxPic.Image = img;
            // https://stackoverflow.com/a/23400751/5438626
            if (Array.IndexOf(img.PropertyIdList, 274) > -1)
            {
                var orientation = (int)img.GetPropertyItem(274).Value[0];
                switch (orientation)
                {
                    case 1: break;
                    case 3: img.RotateFlip(RotateFlipType.Rotate180FlipNone);   break;
                    case 4: img.RotateFlip(RotateFlipType.Rotate180FlipX); break;
                    case 5: img.RotateFlip(RotateFlipType.Rotate90FlipX); break;
                    case 6: img.RotateFlip(RotateFlipType.Rotate90FlipNone); break;
                    case 7: img.RotateFlip(RotateFlipType.Rotate270FlipX); break;
                    case 8: img.RotateFlip(RotateFlipType.Rotate270FlipNone); break;
                }
                // This EXIF data is now invalid and should be removed.
                img.RemovePropertyItem(274);
            }
            base.Refresh();
        }
    }

***
**Search**

This makes a more efficient search in the main form to filter the `FlowLayoutPanel` for matches where `TitleLabelText` contains `guna2TextBox5.Text`. It also meets the requirement **"to keep all of the controls"** even those that are not currently visible.

[![search][2]][2]

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


  [1]: https://i.stack.imgur.com/nMAak.png
  [2]: https://i.stack.imgur.com/inVdC.png