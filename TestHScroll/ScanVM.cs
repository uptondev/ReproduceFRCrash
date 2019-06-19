using System;
using System.Diagnostics;
using System.Threading.Tasks;
using System.Windows.Input;
using Plugin.Media;
using Plugin.Media.Abstractions;
using Plugin.Permissions;
using Plugin.Permissions.Abstractions;
using Xamarin.Forms;

namespace TestHScroll
{
    public class ScanVM : ObservableObject
    {
        public ScanVM()
        { 
            Pages = new TrulyObservableCollection<SelectableItemWrapper<IScanPage>>();
        }

        private TrulyObservableCollection<SelectableItemWrapper<IScanPage>> _pages;
        public TrulyObservableCollection<SelectableItemWrapper<IScanPage>> Pages
        {
            get { return _pages; }
            set { this.SetProperty(ref _pages, value); }
        }

        IScanPage _selectedPage;
        public IScanPage SelectedPage
        {
            get { return _selectedPage; }
            set
            {
                var changed = _selectedPage != value;
                this.SetProperty(ref _selectedPage, value);
                if (changed)
                {
                    // Handle problem with disposed images
                    try
                    {
                        OnPropertyChanged(nameof(PreviewImage));
                    }
                    catch (Exception xx)
                    {
                        Debug.WriteLine("Exception happened when handling a PropertyChanged event:" + xx.ToString());
                    }
                }
            }
        }

        public ImageSource PreviewImage
        {
            get { return SelectedPage?.AvailablePreview; }
        }

        public ICommand AddClickCommand
        {
            get { return new Command(async () => await HandleAddClick()); }
        }
        private async Task HandleAddClick()
        {
            if (!CrossMedia.Current.IsPickPhotoSupported) return;
            await GetImagesFromGallery();
        }

        public ICommand PreviewSelectCommand
        {
            get { return new Command(HandlePreviewImageTapped); }
        }
        private void HandlePreviewImageTapped(object obj)
        {
            var selected = obj as SelectableItemWrapper<IScanPage>;
            SetTheSelectedPage(selected.Item);
        }

        private void SetTheSelectedPage(IScanPage page)
        {
            bool found = false;

            Device.BeginInvokeOnMainThread(() =>
            {
               foreach (var item in Pages)
               {
                   var itemPage = (IScanPage)item.Item;
                   if (itemPage.PageId.Equals(page.PageId))
                   {
                       item.IsSelected = true;
                       found = true;
                   }
                   else
                   {
                       item.IsSelected = false;
                   }
               }

               if (!found)
               {
                    page.Width = 700;
                    Pages.Add(new SelectableItemWrapper<IScanPage>() { IsSelected = true, Item = page });
               }

               this.SelectedPage = page;
            });
        }

        private async Task GetImagesFromGallery()
        {
            try
            {
                var status = await CrossPermissions.Current.CheckPermissionStatusAsync(Permission.Storage);
                if (status != PermissionStatus.Granted)
                {
                    if (await CrossPermissions.Current.ShouldShowRequestPermissionRationaleAsync(Permission.Storage))
                    {
                        Debug.WriteLine("This App needs permission to access picture storage");
                    }

                    var results = await CrossPermissions.Current.RequestPermissionsAsync(Permission.Storage);
                    //Best practice to always check that the key exists
                    if (results.ContainsKey(Permission.Storage))
                        status = results[Permission.Storage];
                }

                if (status == PermissionStatus.Granted)
                {
                    //await _svc.PickImagesAsync();
                    PickMediaOptions options = null;
                    var mediaFile = await CrossMedia.Current.PickPhotoAsync(options);
                    if (mediaFile != null)
                    {
                        var page = await App.ImageService.CreatePageFromImageAsync(mediaFile);
                        SetTheSelectedPage(page);
                    }
                }
                else if (status != PermissionStatus.Unknown)
                {
                    Debug.WriteLine("Permission to use picture storage was denied. Please try again.");
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Gallery Access Error: " + ex.ToString());
            }
        }
    }
}
