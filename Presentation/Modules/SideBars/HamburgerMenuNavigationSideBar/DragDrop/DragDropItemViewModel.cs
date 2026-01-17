using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

using Prism.Events;
using Prism.Mvvm;

using Aksl.Infrastructure;
using Aksl.Toolkit.Controls;
using Prism.Unity;
using Prism;
using Aksl.Toolkit.UI;
using System.Xml.Linq;
using System.Linq;

namespace Aksl.Modules.HamburgerMenuNavigationSideBar.ViewModels
{
    public class DragDropItemViewModel : BindableBase
    {
        #region Members
        private readonly IEventAggregator _eventAggregator;
        private readonly IMenuService _menuService;
        private DragDropItem _dragDropItem;
        #endregion

        #region Constructors
        public DragDropItemViewModel(DragDropItem dragDropItem)
        {
            _dragDropItem = dragDropItem;

            X = _dragDropItem.X;
            Y = _dragDropItem.Y;
        }
        #endregion

        #region Properties
        private double _x;
        public double X
        {
            get => _x;
            set => SetProperty<double>(ref _x, value);
        }

        private double _y;
        public double Y 
       {
            get => _y;
            set => SetProperty<double>(ref _y, value);
        }

        public double Width => _dragDropItem.Width;

        public double Height => _dragDropItem.Height;

        private Type _viewElementType = default;
        public Type ViewElementType
        {
            get
            {
                if (_viewElementType is null)
                {
                    string viewTypeAssemblyQualifiedName = _dragDropItem.ViewName;
                    _viewElementType = Type.GetType(viewTypeAssemblyQualifiedName);
                }

                return _viewElementType;
            }
        }

        private DependencyObject _viewElement = default;
        public DependencyObject ViewElement
        {
            get
            {
                if (_viewElement is null)
                {
                    if (ViewElementType is not null)
                    {
                        // viewElemen = Activator.CreateInstance(viewType) as DependencyObject;
                        _viewElement = (PrismApplication.Current as PrismApplicationBase).Container.Resolve(ViewElementType) as DependencyObject;
                    }
                }

                return _viewElement;
            }
            set
            {
                SetProperty<DependencyObject>(ref _viewElement, value);
            }
        }

        private bool _isSelected = false;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (SetProperty<bool>(ref _isSelected, value))
                {
                    if (!_isSelected)
                    {
                        IsDown = false;
                        StartPoint = new Point(0, 0);
                        OriginalElement = null;
                    }
                }
            }
        }

        private bool _isDown = false;
        public bool IsDown
        {
            get => _isDown;
            set => SetProperty<bool>(ref _isDown, value);
        }

        public Point StartPoint { get; set; }

        public UIElement OriginalElement { get; set; }

        public SimpleCircleAdorner OverlayElement { get; set; }
        #endregion


        #region MouseLeftButtonDown Event
        public void ExecuteMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            System.Windows.Controls.Canvas canvas=null;

            if (sender is FrameworkElement element)
            {
                VisualTreeFinder visualTreeFinder = new();

                var itemsControl = visualTreeFinder.FindVisualParent<ItemsControl>(element);

                var childs = visualTreeFinder.FindVisualChilds<System.Windows.DependencyObject>(itemsControl);
                canvas = childs.FirstOrDefault(d => d is System.Windows.Controls.Canvas) as System.Windows.Controls.Canvas;
            }

            IsDown = true;
            //StartPoint = e.GetPosition((IInputElement)e.Source);
            StartPoint = e.GetPosition(canvas);
            //X= StartPoint.X; Y= StartPoint.Y;
            OriginalElement = e.Source as UIElement;
        //   (sender as FrameworkElement).CaptureMouse();

            canvas?.CaptureMouse();
            e.Handled = true;

            IsSelected = true;
        }
        #endregion
    }
}
