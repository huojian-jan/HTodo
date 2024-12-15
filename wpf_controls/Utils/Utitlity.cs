using System.IO;
using System.Reflection;
using System.Security.AccessControl;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;

namespace ControlToolKits.Utils;

    public static class WpfHelper
    {
        public static SolidColorBrush HexToBrush(string hex)
        {
            var solidbrush = (SolidColorBrush)new BrushConverter().ConvertFrom(hex);
            solidbrush.Freeze();
            return solidbrush;
        }

        // public static ImageSource ImageSourceFromBitmap(Bitmap bmp)
        // {
        //     if (bmp == null || bmp.Width == 0 || bmp.Height == 0)
        //         return null;
        //     var handle = bmp.GetHbitmap();
        //     try
        //     {
        //         return Imaging.CreateBitmapSourceFromHBitmap(handle, IntPtr.Zero, Int32Rect.Empty,
        //             BitmapSizeOptions.FromEmptyOptions());
        //     }
        //     finally { PInvoke.DeleteObject((HGDIOBJ)handle); }
        // }

        //与直接ImageSource绑定路径不同的是这种方式不会导致文件被占用
        // public static ImageSource ImageSourceFromFile(string filename)
        // {
        //     using (var image = System.Drawing.Image.FromFile(filename))
        //     {
        //         using (var bitmap = new Bitmap(image))
        //         {
        //             return ImageSourceFromBitmap(bitmap);
        //         }
        //     }
        // }

        // public static ImageSource ImageSourceFromImage(System.Drawing.Image image)
        // {
        //     using (var bitmap = new Bitmap(image))
        //     {
        //         return ImageSourceFromBitmap(bitmap);
        //     }
        // }

        /// <summary>
        /// Returns the first ancester of specified type
        /// </summary>
        public static T FindAncestor<T>(DependencyObject current)
            where T : DependencyObject
        {
            while (current != null)
            {
                if (current is T target)
                    return target;
                current = GetDependencyObjectParent(current);
            };
            return null;
        }

        /// <summary>
        /// Returns a specific ancester of an object
        /// </summary>
        public static T FindAncestor<T>(DependencyObject current, T lookupItem)
            where T : DependencyObject
        {
            while (current != null)
            {
                if (current is T target && target == lookupItem)
                    return target;
                current = GetDependencyObjectParent(current);
            };
            return null;
        }

        /// <summary>
        /// Finds an ancestor object by name and type
        /// </summary>
        public static T FindAncestor<T>(DependencyObject current, string parentName)
            where T : DependencyObject
        {
            while (current != null)
            {
                if (!string.IsNullOrEmpty(parentName))
                {
                    if (current is T target
                        && current is FrameworkElement frameworkElement
                        && frameworkElement.Name == parentName)
                    {
                        return target;
                    }
                }
                else if (current is T target)
                {
                    return target;
                }
                current = GetDependencyObjectParent(current);
            };
            return null;
        }

        public static DependencyObject FindAncestor(DependencyObject current,
            Func<DependencyObject, bool> predicate)
        {
            while (current != null)
            {
                if (predicate(current))
                    return current;
                current = GetDependencyObjectParent(current);
            };
            return null;
        }

        /// <summary>
        /// Looks for a child control within a parent by name
        /// </summary>
        public static T FindChild<T>(DependencyObject parent, string childName)
            where T : DependencyObject
        {
            // Confirm parent and childName are valid.
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child, childName);

                    // If the child is found, break so we do not overwrite the found child.
                    if (foundChild != null) break;
                }
                else if (!string.IsNullOrEmpty(childName))
                {
                    var frameworkElement = child as FrameworkElement;
                    // If the child's name is set for search
                    if (frameworkElement != null && frameworkElement.Name == childName)
                    {
                        // if the child's name is of the request name
                        foundChild = (T)child;
                        break;
                    }
                    else
                    {
                        // recursively drill down the tree
                        foundChild = FindChild<T>(child, childName);

                        // If the child is found, break so we do not overwrite the found child.
                        if (foundChild != null) break;
                    }
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }

            return foundChild;
        }

        /// <summary>
        /// Looks for a child control within a parent by type
        /// </summary>
        public static T FindChild<T>(DependencyObject parent)
            where T : DependencyObject
        {
            // Confirm parent is valid.
            if (parent == null) return null;

            T foundChild = null;

            int childrenCount = VisualTreeHelper.GetChildrenCount(parent);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(parent, i);
                // If the child is not of the request child type child
                T childType = child as T;
                if (childType == null)
                {
                    // recursively drill down the tree
                    foundChild = FindChild<T>(child);

                    // If the child is found, break so we do not overwrite the found child.
                    if (foundChild != null) break;
                }
                else
                {
                    // child element found.
                    foundChild = (T)child;
                    break;
                }
            }
            return foundChild;
        }

        private static DependencyObject GetDependencyObjectParent(DependencyObject current)
        {
            //ContentElement不存在于可视化树中
            if (current is FrameworkContentElement run)
                return run.Parent;
            else
                return VisualTreeHelper.GetParent(current);
        }


        private static Thickness? _glassFrameThickness;
        public static Thickness GlassFrameThickness
        {
            get
            {
                if (!_glassFrameThickness.HasValue)
                {
                    _glassFrameThickness = new Thickness(-1);
                    try
                    {
                        using (var localMachine = Microsoft.Win32.Registry.CurrentUser)
                        {
                            var key = "Software\\Shadowbot";
                            using (var branch = localMachine.OpenSubKey(key,
                                Microsoft.Win32.RegistryKeyPermissionCheck.ReadSubTree,
                                RegistryRights.ReadKey))
                            {
                                if (branch != null)
                                {
                                    var value = branch.GetValue("GlassFrameThickness")?.ToString();
                                    if (int.TryParse(value, out int glassFrameThickness))
                                        _glassFrameThickness = new Thickness(glassFrameThickness);
                                }
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        // Logging.Warn("Failed to read GlassFrameThickness", ex);
                    }
                }
                return _glassFrameThickness.Value;
            }
        }


        // public static void EnableWindow(Window window)
        // {
        //     if (ComponentDispatcher.IsThreadModal)
        //         PInvoke.EnableWindow((HWND)new WindowInteropHelper(window).Handle, true);
        // }

        public static bool TryGetRenderMode(out RenderMode renderMode)
        {
            try
            {
                using (var localMachine = Microsoft.Win32.Registry.CurrentUser)
                {
                    var key = "Software\\Shadowbot";
                    using (var branch = localMachine.OpenSubKey(key, Microsoft.Win32.RegistryKeyPermissionCheck.ReadSubTree,
                        RegistryRights.ReadKey))
                    {
                        if (branch != null)
                        {
                            var v = branch.GetValue("RenderMode")?.ToString();
                            if (int.TryParse(v, out int renderModeValue) && renderModeValue > 0)
                            {
                                renderMode = RenderMode.SoftwareOnly;
                                return true;
                            }
                        }
                    }
                }
                renderMode = RenderMode.Default;
                return false;
            }
            catch (Exception ex)
            {
                // Logging.Warn("Failed to read RenderMode", ex);
                renderMode = RenderMode.Default;
                return false;
            }
        }


        /// <summary>
        /// 检查当前是否有同样Title的窗口
        /// </summary>
        /// <param name="window"></param>
        /// <param name="title"></param>
        /// <returns></returns>
        public static bool AnyWindow(out Window window, string title)
        {
            return AnyWindow(out window, p => p.Title == title);
        }

        public static bool AnyWindow(out Window window, Func<Window, bool> predicate)
        {
            window = Application.Current.Windows.OfType<Window>().FirstOrDefault(predicate);
            return window != null;
        }

        /// <summary>
        /// 获取当前应用程序所有的模态窗口
        /// </summary>
        /// <returns></returns>
        public static Window[] GetModalWindow()
        {
            var windows = Application.Current.Windows.OfType<Window>().Where(it => it.IsModal()).ToArray();
            return windows;
        }

        /// <summary>
        /// 确定当前窗口实例是否是模态窗口
        /// </summary>
        /// <param name="window"></param>
        /// <returns></returns>
        public static bool IsModal(this Window window)
        {
            return (bool)typeof(Window).GetField("_showingAsDialog", System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.NonPublic).GetValue(window);
        }

        /// <summary>
        /// 关闭所有的模态窗口
        /// </summary>
        public static void CloseAllDialogs()
        {
            try
            {
                var modalWindows = WpfHelper.GetModalWindow();
                foreach (var window in modalWindows)
                    window.Close();
            }
            catch (Exception ex)
            {
                // Logging.Warn("Close all dialog error: ", ex);
            }

        }

        public static bool IsClosed(this Window window)
        {
            try
            {
                var propertyInfo = typeof(Window).GetProperty("IsDisposed", BindingFlags.NonPublic | BindingFlags.Instance);
                var isDisposed = (bool)propertyInfo.GetValue(window);
                return isDisposed;
            }
            catch (Exception)
            {
                return false;
            }
        }

        //https://ying-dao.feishu.cn/docx/NFGQdk5Y1ovOvNxXVrecmWolnxf
        public static async Task CloseWindowAsync(this Window window, DispatcherPriority priority = DispatcherPriority.Normal)
        {
            if (window == null)
                return;

            foreach (Window sub in window.OwnedWindows)
            {
                await CloseWindowAsync(sub);
            }

            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                window.Close();
            }, priority);
        }

        public static bool TryGetActiveWindow(out Window activeWindow)
        {
            activeWindow = null;
            Application current = Application.Current;
            if (current == null)
            {
                return false;
            }
            Window window2 = current.Windows.OfType<Window>().FirstOrDefault((Window x) => x.IsActive);
            window2 = (window2 ?? ((PresentationSource.FromVisual(current.MainWindow) == null) ? null : current.MainWindow));
            if (window2 != null && window2.IsVisible)
            {
                activeWindow = window2;
                return true;
            }
            return false;
        }

        //获取当前窗口下是否有modal dialog
        public static bool TryGetTopModalDialog(this Window window, out Window topWindow)
        {
            if (window == null)
            {
                topWindow = null;
                return false;
            }

            foreach (Window childWindow in window.OwnedWindows)
            {
                if (childWindow.IsModal())
                {
                    if (!TryGetTopModalDialog(childWindow, out topWindow))
                    {
                        topWindow = childWindow;
                    }
                    return true;
                }
            }
            topWindow = null;
            return false;
        }

        public static bool ApplySettings(this FrameworkElement target, IEnumerable<KeyValuePair<string, object>> settings)
        {
            if (settings != null)
            {
                var type = target.GetType();

                foreach (var pair in settings)
                {
                    var propertyInfo = type.GetProperty(pair.Key);

                    if (propertyInfo != null)
                    {
                        propertyInfo.SetValue(target, pair.Value, null);
                    }
                }

                return true;
            }

            return false;
        }

        public static Stream LoadApplicationResourceStream(Uri uri)
        {
            var streamInfo = Application.GetResourceStream(uri);
            if (streamInfo == null)
                throw new ArgumentNullException(nameof(streamInfo));
            return streamInfo.Stream;
        }
    }

