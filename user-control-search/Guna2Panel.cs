using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;

namespace user_control_search
{
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
            textboxPic.Image = Image.FromFile(FullPath);

            // https://stackoverflow.com/a/23400751/5438626
            if (Array.IndexOf(textboxPic.Image.PropertyIdList, 274) > -1)
            {
                var orientation = (int)textboxPic.Image.GetPropertyItem(274).Value[0];
                switch (orientation)
                {
                    case 1:
                        // No rotation required.
                        break;
                    case 3:
                        textboxPic.Image.RotateFlip(RotateFlipType.Rotate180FlipNone);
                        break;
                    case 4:
                        textboxPic.Image.RotateFlip(RotateFlipType.Rotate180FlipX);
                        break;
                    case 5:
                        textboxPic.Image.RotateFlip(RotateFlipType.Rotate90FlipX);
                        break;
                    case 6:
                        textboxPic.Image.RotateFlip(RotateFlipType.Rotate90FlipNone);
                        break;
                    case 7:
                        textboxPic.Image.RotateFlip(RotateFlipType.Rotate270FlipX);
                        break;
                    case 8:
                        textboxPic.Image.RotateFlip(RotateFlipType.Rotate270FlipNone);
                        break;
                }
                // This EXIF data is now invalid and should be removed.
                textboxPic.Image.RemovePropertyItem(274);
            }
            base.Refresh();
        }
    }
}
