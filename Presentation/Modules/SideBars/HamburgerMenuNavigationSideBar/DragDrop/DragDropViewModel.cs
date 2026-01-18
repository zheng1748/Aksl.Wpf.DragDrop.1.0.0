using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;

using Prism.Mvvm;

using Aksl.Toolkit.UI;

namespace Aksl.Modules.HamburgerMenuNavigationSideBar.ViewModels
{
    public class DragDropViewModel : BindableBase
    {
        #region Members
        private DragDropItemViewModel _selectedDragDropItem;
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

        #region MouseMove Event
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
                  
                    if (!_selectedDragDropItem.IsDragging)
                    {
                        DragStarted();
                    }

                    if (_selectedDragDropItem.IsDragging)
                    {
                        DragMoved();
                    }
                }

                void DragStarted()
                {
                    _selectedDragDropItem.IsDragging = true;

                    var originalElement = _selectedDragDropItem.OriginalElement;

                    var overlayElement = new SimpleCircleAdorner(originalElement);
                    _selectedDragDropItem.OverlayElement = overlayElement;
                    var layer = AdornerLayer.GetAdornerLayer(originalElement);
                    layer.Add(overlayElement);
                }

                void DragMoved()
                {
                    var currentPosition = Mouse.GetPosition(canvas);

                    _selectedDragDropItem.OverlayElement.LeftOffset = currentPosition.X - _selectedDragDropItem.StartPoint.X;
                    _selectedDragDropItem.OverlayElement.TopOffset = currentPosition.Y - _selectedDragDropItem.StartPoint.Y;
                }
            }
        }
        #endregion

        #region MouseLeftButtonUp Event
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

                if (_selectedDragDropItem.IsDragging)
                {
                    var overlayElement = _selectedDragDropItem.OverlayElement;
                    AdornerLayer.GetAdornerLayer(overlayElement.AdornedElement).Remove(overlayElement);

                    _selectedDragDropItem.X = _selectedDragDropItem.X + _selectedDragDropItem.OverlayElement.LeftOffset;
                    _selectedDragDropItem.Y = _selectedDragDropItem.Y +_selectedDragDropItem.OverlayElement.TopOffset;

                    _selectedDragDropItem.OverlayElement = null;
                }
                _selectedDragDropItem.IsDragging = false;
                _selectedDragDropItem.IsDown = false;
            }
        }
        #endregion
    }
}
