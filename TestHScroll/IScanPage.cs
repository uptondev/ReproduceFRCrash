using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace TestHScroll
{
    public interface IScanPage
    {
        Task RotateAsync(int deg);
        Guid PageId { get; }
        ImageSource AvailablePreview { get; }
        ImageSource ThumbPreview { get; }
        string FilePath { get; set; }
        int Width { get; set; }
    }
}
