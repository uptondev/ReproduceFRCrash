using System;
using System.Threading.Tasks;
using Plugin.Media.Abstractions;

namespace TestHScroll
{
    public interface IImageService
    {
        Task<IScanPage> CreatePageFromImageAsync(MediaFile doc);
    }
}
