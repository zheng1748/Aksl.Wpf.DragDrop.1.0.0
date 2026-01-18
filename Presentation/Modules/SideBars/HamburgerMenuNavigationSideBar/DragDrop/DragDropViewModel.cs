using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Xml.Linq;
using Aksl.Toolkit.UI;
using Prism.Events;
using Prism.Mvvm;
using Unity;

namespace Aksl.Modules.HamburgerMenuNavigationSideBar.ViewModels
{
    public class DragDropViewModel : BindableBase
    {
        #region Members
        private DragDropItemViewModel _selectedDragDropItem;
        private bool _isDown;
        private bool _isDragging;
        private UIElement _originalElement;
        private double _originalLeft;
        private double _originalTop;
        private SimpleCircleAdorner _overlayElement;
        private Point _startPoint;
        #endregion

        #region Constructors
        public DragDropViewModel()
        {
            DragDropItems = new();
        }
        #endregion

        #region Properties
        public ObservableCollection<DragDropItemViewModel> DragDropItems { get; }

        public DragDropItemViewModel PreviewSelectedDragDropItem { get; private set; }
        #endregion

        #region Drop Event
        public void ExecuteDrop(object sender, DragEventArgs e)
        {
            var menuItem = e.Data.GetData(typeof(Infrastructure.MenuItem)) as Infrastructure.MenuItem;

            var point = e.GetPosition((IInputElement)e.Source);

            DragDropItem dragDropItem = new() { X = point.X, Y = point.Y, Width = menuItem.Width, Height = menuItem.Height, ViewName = menuItem.ViewName };
            DragDropItemViewModel dragDropItemViewModel = new(dragDropItem);
            AddPropertyChanged();

            void AddPropertyChanged()
            {
                dragDropItemViewModel.PropertyChanged += (sender, e) =>
                {
                    if (sender is DragDropItemViewModel ddivm)
                    {
                        if (e.PropertyName == nameof(DragDropItemViewModel.IsSelected))
                        {
                            if (ddivm.IsSelected)
                            {
                                if (_selectedDragDropItem is null)
                                {
                                    _selectedDragDropItem = ddivm;
                                }

                                if (_selectedDragDropItem is not null && _selectedDragDropItem != ddivm)
                                {
                                    var previewSelectedDragDropItem = _selectedDragDropItem;
                                    previewSelectedDragDropItem.IsSelected = false;

                                    _selectedDragDropItem = ddivm;
                                }

                                _isDown = _selectedDragDropItem.IsDown;
                                _startPoint = _selectedDragDropItem.StartPoint;
                                _originalElement = _selectedDragDropItem.OriginalElement;
                            }
                            else
                            {

                            }
                        }
                    }
                };
            }

            DragDropItems.Add(dragDropItemViewModel);
        }
        #endregion

        #region Methods
        public System.Windows.DependencyObject GetStoreViewElement(Type viewType)
        {
            var storeDragDropItemViewModel = DragDropItems.FirstOrDefault(ti => ti.ViewElementType == viewType);

            return storeDragDropItemViewModel?.ViewElement;
        }
        #endregion

        #region MouseLeftButtonDown Event
        public void ExecutePreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.Source is ItemsControl element)
            {

                VisualTreeFinder visualTreeFinder = new();

                var childs = visualTreeFinder.FindVisualChilds<System.Windows.DependencyObject>(element);
                var canvas = childs.FirstOrDefault(d => d is System.Windows.Controls.Canvas);

                //Type viewType = elementView.GetType();
                //var currentView = GetStoreViewElement(viewType);

                //if (elementView is not elementView)
                //{

                //}

                _isDown = true;
                _startPoint = e.GetPosition((IInputElement)e.Source);
                _originalElement = e.Source as UIElement;
                // MyCanvas.CaptureMouse();
                e.Handled = true;
            }
            else
            {

            }

        }
        #endregion

        public void ExecutePreviewMouseMove(object sender, MouseEventArgs e)
        {
            if ((_selectedDragDropItem is not null) && _selectedDragDropItem.IsDown)
            {
                System.Windows.Controls.Canvas canvas;

                if (e.Source is ItemsControl element)
                {
                    VisualTreeFinder visualTreeFinder = new();

                    var childs = visualTreeFinder.FindVisualChilds<System.Windows.DependencyObject>(element);
                    canvas = childs.FirstOrDefault(d => d is System.Windows.Controls.Canvas) as System.Windows.Controls.Canvas;


                    //if ((_isDragging == false) && ((Math.Abs(e.GetPosition(canvas).X - _selectedDragDropItem.StartPoint.X) > SystemParameters.MinimumHorizontalDragDistance) ||
                    //                              (Math.Abs(e.GetPosition(canvas).Y - _selectedDragDropItem.StartPoint.Y) > SystemParameters.MinimumVerticalDragDistance)))
                    if (_isDragging == false)
                    {
                        DragStarted();
                    }

                    if (_isDragging)
                    {
                        DragMoved();
                    }
                }

                void DragStarted()
                {
                    _isDragging = true;

                    // var originalElement = _selectedDragDropItem.ViewElement as UIElement;
                    var originalElement = _selectedDragDropItem.OriginalElement;

                    //_originalLeft = Canvas.GetLeft(originalElement);
                    //_originalTop = Canvas.GetTop(originalElement);
                    //_originalLeft = Canvas.GetLeft(_originalElement);
                    //_originalTop = Canvas.GetTop(_originalElement);

                    var overlayElement = new SimpleCircleAdorner(originalElement);
                    _selectedDragDropItem.OverlayElement = overlayElement;
                    var layer = AdornerLayer.GetAdornerLayer(originalElement);
                    layer.Add(overlayElement);

                    //  _overlayElement = new SimpleCircleAdorner(originalElement);
                    //var layer = AdornerLayer.GetAdornerLayer(originalElement);
                    //layer.Add(_overlayElement);
                }

                void DragMoved()
                {
                    var currentPosition = Mouse.GetPosition(canvas);

                    //_selectedDragDropItem.OverlayElement.LeftOffset = currentPosition.X - _selectedDragDropItem.X;
                    //_selectedDragDropItem.OverlayElement.TopOffset = currentPosition.Y - _selectedDragDropItem.Y;
                    _selectedDragDropItem.OverlayElement.LeftOffset = currentPosition.X - _selectedDragDropItem.StartPoint.X;
                    _selectedDragDropItem.OverlayElement.TopOffset = currentPosition.Y - _selectedDragDropItem.StartPoint.Y;
                    //_selectedDragDropItem.OverlayElement.LeftOffset = currentPosition.X - _startPoint.X;
                    //_selectedDragDropItem.OverlayElement.TopOffset = currentPosition.Y - _startPoint.Y;

                    //_overlayElement.LeftOffset = currentPosition.X - _startPoint.X;
                    //_overlayElement.TopOffset = currentPosition.Y - _startPoint.Y;
                }
            }
        }

        public void ExecutePreviewMouseLeftButtonUp(object sender, MouseEventArgs e)
        {
            if ((_selectedDragDropItem is not null) && _selectedDragDropItem.IsDown)
            {
                System.Windows.Controls.Canvas canvas;

                if (e.Source is ItemsControl element)
                {
                    VisualTreeFinder visualTreeFinder = new();

                    var childs = visualTreeFinder.FindVisualChilds<System.Windows.DependencyObject>(element);
                    canvas = childs.FirstOrDefault(d => d is System.Windows.Controls.Canvas) as System.Windows.Controls.Canvas;
                }

                DragFinished();

                e.Handled = true;
            }

            void DragFinished(bool cancelled = false)
            {
                Mouse.Capture(null);

                if (_isDragging)
                {
                    var overlayElement = _selectedDragDropItem.OverlayElement;
                    AdornerLayer.GetAdornerLayer(overlayElement.AdornedElement).Remove(overlayElement);

                    _selectedDragDropItem.X = _selectedDragDropItem.X + _selectedDragDropItem.OverlayElement.LeftOffset;
                    _selectedDragDropItem.Y = _selectedDragDropItem.Y +_selectedDragDropItem.OverlayElement.TopOffset;

                    //  var currentPosition = Mouse.GetPosition(sender as FrameworkElement);

                    //_selectedDragDropItem.X = currentPosition.X ;
                    //_selectedDragDropItem.Y = currentPosition.Y ;

                    //_selectedDragDropItem.X = currentPosition.X - _selectedDragDropItem.OverlayElement.LeftOffset;
                    //_selectedDragDropItem.Y = currentPosition.Y - _selectedDragDropItem.OverlayElement.TopOffset;

                    //if (cancelled == false)
                    //{
                    //    Canvas.SetTop(_originalElement, _originalTop + _overlayElement.TopOffset);
                    //    Canvas.SetLeft(_originalElement, _originalLeft + _overlayElement.LeftOffset);
                    //}
                    //_overlayElement = null;
                    _selectedDragDropItem.OverlayElement = null;
                }
                _isDragging = false;
                //  _isDown = false;
                _selectedDragDropItem.IsDown = false;
            }
        }
    }
}
