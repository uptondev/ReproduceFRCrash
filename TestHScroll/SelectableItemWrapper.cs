using System;

namespace TestHScroll
{
    public class SelectableItemWrapper<T> : ObservableObject
    {
        bool _isSelected;
        public bool IsSelected
        {
            get { return _isSelected; }
            set { SetProperty(ref _isSelected, value); }
        }

        public T Item { get; set; }
    }
}
