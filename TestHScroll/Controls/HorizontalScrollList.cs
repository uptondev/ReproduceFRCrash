using System;
using System.Collections;
using System.Collections.Specialized;
using System.Windows.Input;
using Xamarin.Forms;

namespace TestHScroll.Controls
{
    public class HorizontalScrollList : ScrollView
    {
        public event EventHandler<ItemTappedEventArgs> ItemSelected;

        public static readonly BindableProperty ItemTemplateProperty = BindableProperty.Create(
            nameof(ItemTemplate),
            typeof(DataTemplate),
            typeof(HorizontalScrollList),
            null,
            propertyChanged: (bindable, value, newValue) => ((HorizontalScrollList)bindable).Populate());

        public static readonly BindableProperty ItemsSourceProperty = BindableProperty.Create(
            nameof(ItemsSource),
            typeof(IEnumerable),
            typeof(HorizontalScrollList),
            null,
            BindingMode.OneWay,
            propertyChanged: (bindable, value, newValue) =>
            {
                var obs = value as INotifyCollectionChanged;
                var self = (HorizontalScrollList)bindable;
                if (obs != null)
                    obs.CollectionChanged -= self.HandleItemChanged;

                self.Populate();

                obs = newValue as INotifyCollectionChanged;
                if (obs != null)
                    obs.CollectionChanged += self.HandleItemChanged;
            });

        public IEnumerable ItemsSource
        {
            get => (IEnumerable)this.GetValue(ItemsSourceProperty);
            set => this.SetValue(ItemsSourceProperty, value);
        }

        public DataTemplate ItemTemplate
        {
            get => (DataTemplate)this.GetValue(ItemTemplateProperty);
            set => this.SetValue(ItemTemplateProperty, value);
        }

        public static readonly BindableProperty SelectedCommandProperty =
            BindableProperty.Create(nameof(SelectedCommand), typeof(ICommand), typeof(HorizontalScrollList), null);

        public ICommand SelectedCommand
        {
            get { return (ICommand)GetValue(SelectedCommandProperty); }
            set { SetValue(SelectedCommandProperty, value); }
        }

        public static readonly BindableProperty SelectedCommandParameterProperty =
            BindableProperty.Create(nameof(SelectedCommandParameter), typeof(object), typeof(HorizontalScrollList), null);

        public object SelectedCommandParameter
        {
            get { return GetValue(SelectedCommandParameterProperty); }
            set { SetValue(SelectedCommandParameterProperty, value); }
        }

        public bool ToggleRefreshRequest
        {
            get { return (bool)GetValue(ToggleRefreshRequestProperty); }
            set { SetValue(ToggleRefreshRequestProperty, value); }
        }
        public static BindableProperty ToggleRefreshRequestProperty = BindableProperty.Create(nameof(ToggleRefreshRequest), typeof(bool), typeof(HorizontalScrollList), propertyChanged: (bindable, oldValue, newValue) =>
        {
            var webView = bindable as HorizontalScrollList;
            webView.FireRefresh();
        });
        public void FireRefresh()
        {
            if (!willUpdate)
            {
                willUpdate = true;
                Device.BeginInvokeOnMainThread(Populate);
            }
        }

        private bool willUpdate = true;
        private void HandleItemChanged(object sender, NotifyCollectionChangedEventArgs eventArgs)
        {
            if (!willUpdate)
            {
                willUpdate = true;
                Device.BeginInvokeOnMainThread(Populate);
            }
        }

        public HorizontalScrollList()
        {
            this.Orientation = ScrollOrientation.Horizontal;
            this.HorizontalOptions = new LayoutOptions(LayoutAlignment.Fill, true);
        }

        private void Populate()
        {
            willUpdate = false;

            Content = null;

            if (ItemsSource == null || ItemTemplate == null)
            {
                return;
            }

            var list = new StackLayout { Orientation = StackOrientation.Horizontal };

            foreach (var viewModel in ItemsSource)
            {
                var content = ItemTemplate.CreateContent();
                if (!(content is View) && !(content is ViewCell))
                {
                    throw new Exception($"Invalid visual object {nameof(content)}");
                }
                var command = SelectedCommand ?? new Command((obj) =>
                {
                    var index = (ItemsSource as TrulyObservableCollection<SelectableItemWrapper<IScanPage>>).IndexOf(viewModel as SelectableItemWrapper<IScanPage>);
                    var args = new ItemTappedEventArgs(ItemsSource, viewModel, index);
                    ItemSelected?.Invoke(this, args);
                });
                var commandParameter = SelectedCommandParameter ?? viewModel;
                var view = content is View ? content as View : ((ViewCell)content).View;
                view.BindingContext = viewModel;
                view.GestureRecognizers.Add(new TapGestureRecognizer
                {
                    Command = command,
                    CommandParameter = commandParameter,
                    NumberOfTapsRequired = 1
                });

                list.Children.Add(view);
            }

            if (list.Children.Count == 0)
            {
                list.Children.Add(new Label
                {
                    FontSize = Device.GetNamedSize(NamedSize.Micro, typeof(Label)),
                    HorizontalOptions = new LayoutOptions(LayoutAlignment.Fill, true),
                    VerticalOptions = new LayoutOptions(LayoutAlignment.Fill, true),
                    HorizontalTextAlignment = TextAlignment.Center,
                    VerticalTextAlignment = TextAlignment.Center,
                    TextColor = new Color(255, 0, 0),
                    Margin = new Thickness(10, 10, 10, 10),
                    Text = "* NO DOCUMENTS SCANNED YET *",
                    LineBreakMode = LineBreakMode.WordWrap
                });
            }

            Content = list;
        }
    }
}
