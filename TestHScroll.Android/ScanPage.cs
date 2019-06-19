using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TestHScroll.Droid
{
    public class ScanPage : IScanPage
    {
        private readonly Guid _pageId;
        private string _path = string.Empty;
        public const int THUMBNAIL_WIDTH = 100;
        private ImageSource ThumbSource { get; set; } = null;
        private ImageSource PreviewSource { get; set; } = null;

        public ScanPage(string path)
        {
            _pageId = Guid.NewGuid();
            _path = path;
        }

        public Guid PageId
        {
            get { return _pageId; }
        }

        public string FilePath
        {
            get { return _path; }
            set { _path = value; }
        }

        private int _width;
        public int Width
        {
            get { return _width; }
            set { _width = value; }
        }

        private int _thumbWidth = THUMBNAIL_WIDTH;
        public int ThumbWidth
        {
            get { return _thumbWidth; }
            set { _thumbWidth = value; }
        }

        public async Task RotateAsync(int degrees)
        {
            throw new NotImplementedException();
            return;
        }

        public ImageSource AvailablePreview
        {
            get
            {
                if (PreviewSource == null)
                    PreviewSource = ScaledPreview(this.Width);
                return PreviewSource;
            }
        }

        public ImageSource ThumbPreview
        {
            get
            {
                if (ThumbSource == null)
                    ThumbSource = ScaledPreview(this.ThumbWidth);
                return ThumbSource;
            }
        }

        public ImageSource ScaledPreview(int Width)
        {
            if (string.IsNullOrWhiteSpace(_path))
                throw new Exception("FilePath property not set");
            return TestHScroll.Droid.ImageService.ScaleToFitWidth(_path, Width);
        }
    }
}
