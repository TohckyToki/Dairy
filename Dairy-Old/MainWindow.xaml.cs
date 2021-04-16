using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Dairy
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private DB db;
        private Storyboard storyboard { get; set; }
        private int currentPage;
        private List<Button> pageButtons;

        public MainWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.db = new DB();
            InitAnimation();
            InitPage();
            LoadData();
        }

        private void InitAnimation()
        {
            this.storyboard = new Storyboard();
            var frame = new ThicknessAnimationUsingKeyFrames() { BeginTime = TimeSpan.Zero };
            frame.KeyFrames.Add(new SplineThicknessKeyFrame(new Thickness(0, -26, 0, 0), KeyTime.FromTimeSpan(TimeSpan.Zero)));
            frame.KeyFrames.Add(new SplineThicknessKeyFrame(new Thickness(0, -13, 0, 0), KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 250))));
            frame.KeyFrames.Add(new SplineThicknessKeyFrame(new Thickness(0, 0, 0, 0), KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 500))));
            frame.KeyFrames.Add(new SplineThicknessKeyFrame(new Thickness(0, 0, 0, 0), KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 1, 500))));
            frame.KeyFrames.Add(new SplineThicknessKeyFrame(new Thickness(0, -13, 0, 0), KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 1, 750))));
            frame.KeyFrames.Add(new SplineThicknessKeyFrame(new Thickness(0, -26, 0, 0), KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 2, 0))));
            this.storyboard.Children.Add(frame);
            Storyboard.SetTarget(frame, this.NoticeBar);
            Storyboard.SetTargetProperty(frame, new PropertyPath(MarginProperty));
            this.storyboard.Completed += new EventHandler((obj, arg) =>
            {
                this.NoticeBar.Visibility = Visibility.Hidden;
            });
        }

        private void InitPage()
        {
            this.currentPage = 1;
            this.pageButtons = (from button in this.Pages.Children.OfType<StackPanel>().First().Children.OfType<Button>()
                                where button.Content is "0"
                                select button).ToList();
        }

        private void LoadData()
        {
            this.Details.ItemsSource = null;
            PageChanged();
            var value = this.db.GetData(this.currentPage);
            if (value == null)
            {
                this.Details.ItemsSource = null;
                this.NoDairy.Visibility = Visibility.Visible;
                return;
            }
            this.NoDairy.Visibility = Visibility.Hidden;
            this.Details.ItemsSource = value;
        }

        private void PageChanged()
        {
            var pageCount = this.db.GetPageCount();
            if (pageCount == 1)
            {
                this.Pages.Height = 0;
                return;
            }
            this.Pages.Height = 40;
            //上一页
            if (this.currentPage == 1)
            {
                this.PagePrevious.IsEnabled = false;
                this.PagePrevious.Tag = null;
            }
            else
            {
                this.PagePrevious.Tag = this.currentPage - 1;
                this.PagePrevious.IsEnabled = true;
            }
            //下一页
            if (this.currentPage == pageCount)
            {
                this.PageNext.IsEnabled = false;
                this.PageNext.Tag = null;
            }
            else
            {
                this.PageNext.IsEnabled = true;
                this.PageNext.Tag = this.currentPage + 1;
            }
            if (pageCount < 8)
            {
                this.PageLeft.Width = 0;
                this.PageRight.Width = 0;
                int i;
                for (i = 0; i < pageCount; i++)
                {
                    this.pageButtons[i].Content = i + 1;
                    this.pageButtons[i].Tag = i + 1;
                    this.pageButtons[i].Width = 32;
                }
                for (; i < this.pageButtons.Count; i++)
                {
                    this.pageButtons[i].Width = 0;
                }
            }
            else
            {
                if (this.currentPage - 1 > 3)
                {
                    this.PageLeft.Width = 32;
                }
                else
                {
                    this.PageLeft.Width = 0;
                }

                if (pageCount - this.currentPage > 3)
                {
                    this.PageRight.Width = 32;
                }
                else
                {
                    this.PageRight.Width = 0;
                }

                this.pageButtons[0].Content = 1;
                this.pageButtons[0].Tag = 1;
                this.pageButtons[0].Width = 32;
                this.pageButtons[0].Focusable = true;
                this.pageButtons[6].Content = pageCount;
                this.pageButtons[6].Tag = pageCount;
                this.pageButtons[6].Width = 32;
                this.pageButtons[6].Focusable = true;

                if (this.PageRight.Width != 0 && this.PageLeft.Width != 0)
                {
                    var startPage = this.currentPage - 3;
                    for (var i = 1; i < 6; i++)
                    {
                        this.pageButtons[i].Content = i + startPage;
                        this.pageButtons[i].Tag = i + startPage;
                        this.pageButtons[i].Width = 32;
                        this.pageButtons[i].Focusable = true;
                    }
                }
                else if (this.PageRight.Width == 0)
                {
                    for (var i = 5; i > 0; i--)
                    {
                        this.pageButtons[6 - i].Content = pageCount - i;
                        this.pageButtons[6 - i].Tag = pageCount - i;
                        this.pageButtons[6 - i].Width = 32;
                        this.pageButtons[6 - i].Focusable = true;
                    }
                }
                else if (this.PageLeft.Width == 0)
                {
                    for (var i = 1; i < 6; i++)
                    {
                        this.pageButtons[i].Content = i + 1;
                        this.pageButtons[i].Tag = i + 1;
                        this.pageButtons[i].Width = 32;
                        this.pageButtons[i].Focusable = true;
                    }
                }

              (from btn in this.pageButtons
               where ((int)btn.Content) == this.currentPage
               select btn).First().Focusable = false;

            }
        }

        private void AppOption_Click(object sender, RoutedEventArgs e)
        {
            if (this.Options.Visibility == Visibility.Hidden)
            {
                this.FreezeAll.Visibility = Visibility.Visible;
                this.Options.Visibility = Visibility.Visible;
                ((Button)sender).Background = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
            }
            else
            {
                this.FreezeAll.Visibility = Visibility.Hidden;
                this.Options.Visibility = Visibility.Hidden;
                ((Button)sender).Background = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            }
        }

        private void PerPageCount_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (this.PerPageCount.Text == Properties.Settings.Default.PerPageCount.ToString())
            {
                return;
            }

            this.ApplyPerPageCount.IsEnabled = true;
        }

        private void ApplyPerPageCount_Click(object sender, RoutedEventArgs e)
        {
            if (!int.TryParse(this.PerPageCount.Text, out var num))
            {
                NoticeMessage("请输入正确的数字");
                this.PerPageCount.Text = Properties.Settings.Default.PerPageCount.ToString();
                this.PerPageCount.Focus();
                this.ApplyPerPageCount.IsEnabled = false;
                return;
            }
            if (Properties.Settings.Default.PerPageCount != num)
            {
                Properties.Settings.Default.PerPageCount = num;
                NoticeMessage("修改成功");
                LoadData();
            }
            this.ApplyPerPageCount.IsEnabled = false;
        }

        private void OrderByAsc_Changed(object sender, RoutedEventArgs e)
        {
            var content = ((RadioButton)sender).Content.ToString();
            bool orderbyasc;
            if (content.ToLower() == "desc")
            {
                orderbyasc = false;
            }
            else
            {
                orderbyasc = true;
            }
            if (Properties.Settings.Default.OrderByAsc != orderbyasc)
            {
                Properties.Settings.Default.OrderByAsc = orderbyasc;
                NoticeMessage("修改成功");
                LoadData();
            }
        }

        private void PageJump_Click(object sender, RoutedEventArgs e)
        {
            this.currentPage = Convert.ToInt32(((Button)sender).Tag);
            this.PageJump.Text = "";
            LoadData();
        }

        private void MultiSelect_Click(object sender, RoutedEventArgs e)
        {
            if (this.NoDairy.Visibility == this.Visibility)
            {
                return;
            }

            var num = Convert.ToInt32(this.Details.Tag);
            if (num == 0)
            {
                ((Button)sender).Background = new SolidColorBrush(Color.FromArgb(150, 255, 0, 0));
                this.Details.Tag = 60;
                this.MultiCheckAll.Visibility = Visibility.Visible;
                this.MultiCheckOther.Visibility = Visibility.Visible;
                this.MultiDelete.Visibility = Visibility.Visible;
                this.MultiExport.Visibility = Visibility.Visible;
            }
            else
            {
                ((Button)sender).Background = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                this.Details.Tag = 0;
                this.MultiCheckAll.Visibility = Visibility.Hidden;
                this.MultiCheckOther.Visibility = Visibility.Hidden;
                this.MultiDelete.Visibility = Visibility.Hidden;
                this.MultiExport.Visibility = Visibility.Hidden;
                var value = (List<DB.Dairy>)this.Details.ItemsSource;
                for (var i = 0; i < value.Count; i++)
                {
                    value[i].IsSelected = false;
                }
                this.Details.ItemsSource = null;
                this.Details.ItemsSource = value;
                Select_Changed(null, null);
            }
        }

        private void Select_Changed(object sender, RoutedEventArgs e)
        {
            var count = (from dairy in (List<DB.Dairy>)this.Details.ItemsSource
                         where dairy.IsSelected
                         select dairy).Count();
            if (count > 0)
            {
                this.MultiDelete.IsEnabled = true;
                this.MultiExport.IsEnabled = true;
            }
            else
            {
                this.MultiDelete.IsEnabled = false;
                this.MultiExport.IsEnabled = false;
            }
        }

        private void MultiCheckAll_Click(object sender, RoutedEventArgs e)
        {
            var dates = (from dairy in (List<DB.Dairy>)this.Details.ItemsSource
                         where dairy.IsSelected
                         select dairy.WroteDate).ToList();
            if (dates.Count == ((List<DB.Dairy>)this.Details.ItemsSource).Count)
            {
                foreach (var item in (List<DB.Dairy>)this.Details.ItemsSource)
                {
                    item.IsSelected = false;
                }
            }
            else
            {
                foreach (var item in (List<DB.Dairy>)this.Details.ItemsSource)
                {
                    item.IsSelected = true;
                }
            }
            var data = this.Details.ItemsSource;
            this.Details.ItemsSource = null;
            this.Details.ItemsSource = data;
            Select_Changed(null, null);
        }

        private void MultiCheckOther_Click(object sender, RoutedEventArgs e)
        {
            foreach (var item in (List<DB.Dairy>)this.Details.ItemsSource)
            {
                if (item.IsSelected)
                {
                    item.IsSelected = false;
                }
                else
                {
                    item.IsSelected = true;
                }
            }
            var data = this.Details.ItemsSource;
            this.Details.ItemsSource = null;
            this.Details.ItemsSource = data;
            Select_Changed(null, null);
        }

        private void MultiDelete_Click(object sender, RoutedEventArgs e)
        {
            var dates = (from dairy in (List<DB.Dairy>)this.Details.ItemsSource
                         where dairy.IsSelected
                         select dairy.WroteDate).ToList();
            ShowMsgBox($"是否确定删除选中的{dates.Count()}条数据？",
                new Action(() =>
                {
                    var result = this.db.DeleteData(dates);
                    if (result > 0)
                    {
                        LoadData();
                        NoticeMessage("删除成功！");
                    }
                    else
                    {
                        NoticeMessage("删除失败！");
                    }
                }));
        }

        private void MultiExport_Click(object sender, RoutedEventArgs e)
        {
            var dates = (from dairy in (List<DB.Dairy>)this.Details.ItemsSource
                         where dairy.IsSelected
                         select dairy.WroteDate).ToList();
            ShowMsgBox($"确定导出选中的{dates.Count()}条数据？",
                new Action(() =>
                {
                    var dlg = new Microsoft.Win32.SaveFileDialog
                    {
                        InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                    };
                    ;
                    dlg.FileName = "Dairy";
                    dlg.DefaultExt = ".txt";
                    dlg.Filter = "文本文件(.txt)|*.txt|网页(.html)|*.html";
                    dlg.CheckPathExists = true;

                    var result = dlg.ShowDialog();
                    if (result == true)
                    {
                        var filename = dlg.FileName;
                        var ext = System.IO.Path.GetExtension(filename);
                        var ecoding = Export.ExportEcoding.text;
                        if (ext == ".html")
                        {
                            ecoding = Export.ExportEcoding.html;
                        }
                        var content = new Export(ecoding, this.db.GetDetail(dates)).Content;

                        System.IO.File.WriteAllText(filename, content, Encoding.UTF8);
                        NoticeMessage("导出成功");
                    }
                }));
        }

        private void AddNew_Click(object sender, RoutedEventArgs e)
        {
            var addNew = new Action(() =>
            {
                this.EditArea.DataContext = new DB.Dairy()
                {
                    IsNew = true,
                    WroteDate = DateTime.Now.ToShortDateString(),
                    Thema = string.Empty,
                    Wheather = "晴",
                    Content = string.Empty,
                };

                this.Details.Visibility = Visibility.Hidden;
                this.ViewArea.Visibility = Visibility.Hidden;
                this.EditArea.Visibility = Visibility.Visible;
                this.EditArea.Tag = false;
            });

            if (this.EditArea.Visibility == Visibility.Visible && Convert.ToBoolean(this.EditArea.Tag))
            {
                ShowMsgBox("是否放弃当前已修改的内容？", addNew);
            }
            else
            {
                addNew();
            }
        }

        private void ViewClose_Click(object sender, RoutedEventArgs e)
        {
            this.ViewArea.Visibility = Visibility.Hidden;
            this.Details.Visibility = Visibility.Visible;
        }

        private void Item_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var date = sender.GetType().GetProperty("Tag").GetValue(sender)?.ToString();
            var value = this.db.GetDetail(date);
            this.db.GetDairyPosition(ref value);
            this.ViewArea.DataContext = value;
            this.ViewArea.Visibility = Visibility.Visible;
            this.Details.Visibility = Visibility.Hidden;
        }

        private void View_Click(object sender, RoutedEventArgs e)
        {
            Item_MouseDown(sender, null);
        }

        private void EditDairy_Click(object sender, RoutedEventArgs e)
        {
            if (this.ViewArea.Visibility == Visibility.Visible)
            {
                this.ViewArea.IsEnabled = false;
            }

            var date = ((Control)sender).Tag.ToString();
            this.EditArea.DataContext = this.db.GetDetail(date);
            this.Details.Visibility = Visibility.Hidden;
            this.ViewArea.Visibility = Visibility.Hidden;
            this.EditArea.Visibility = Visibility.Visible;
            this.EditArea.Tag = false;
        }

        private void RemoveDairy_Click(object sender, RoutedEventArgs e)
        {
            ShowMsgBox("是否确定删除？",
                new Action(() =>
                {
                    var date = ((Control)sender).Tag.ToString();
                    var result = this.db.DeleteData(date);
                    if (result > 0)
                    {
                        LoadData();
                        this.ViewArea.Visibility = Visibility.Hidden;
                        this.Details.Visibility = Visibility.Visible;
                        NoticeMessage("删除成功！");
                    }
                    else
                    {
                        NoticeMessage("删除失败！");
                    }
                }));
        }

        private void EditWheather_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (((ComboBox)sender).Text.Length > 10)
            {
                ((ComboBox)sender).Text = ((ComboBox)sender).Text.Substring(0, 10);
            }
        }

        private void EditDrop_Click(object sender, RoutedEventArgs e)
        {
            //未编辑
            if (!Convert.ToBoolean(this.EditArea.Tag))
            {
                this.EditArea.Visibility = Visibility.Hidden;
                if (this.ViewArea.IsEnabled == false)
                {
                    this.ViewArea.IsEnabled = true;
                    this.ViewArea.Visibility = Visibility.Visible;
                }
                else
                {
                    this.Details.Visibility = Visibility.Visible;
                }
                return;
            }

            ShowMsgBox("是否放弃当前已修改的内容？",
                new Action(() =>
                {
                    this.EditArea.Visibility = Visibility.Hidden;
                    if (this.ViewArea.IsEnabled == false)
                    {
                        this.ViewArea.IsEnabled = true;
                        this.ViewArea.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        this.Details.Visibility = Visibility.Visible;
                    }
                    this.EditArea.Tag = false;
                }));
        }

        private void EditArea_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.EditArea.IsVisible)
            {
                this.EditWheather.SelectionChanged += EditSelectorChanged;
                this.EditThema.TextChanged += EditTextChanged;
                this.EditContent.TextChanged += EditTextChanged;
            }
            else if (Convert.ToBoolean(this.EditArea.Tag) == false)
            {
                RemoveAllChangedEvent();
            }
        }

        private void EditSelectorChanged(object sender, SelectionChangedEventArgs e)
        {
            if (string.IsNullOrEmpty(this.EditContent.Text))
            {
                return;
            }

            this.EditArea.Tag = true;
            RemoveAllChangedEvent();
        }

        private void EditTextChanged(object sender, TextChangedEventArgs e)
        {
            if (((TextBox)sender).Name != "EditContent" && string.IsNullOrEmpty(this.EditContent.Text))
            {
                return;
            }

            this.EditArea.Tag = true;
            RemoveAllChangedEvent();
        }

        private void RemoveAllChangedEvent()
        {
            this.EditWheather.SelectionChanged -= EditSelectorChanged;
            this.EditThema.TextChanged -= EditTextChanged;
            this.EditContent.TextChanged -= EditTextChanged;
        }

        private void EditSave_Click(object sender, RoutedEventArgs e)
        {
            if (Convert.ToBoolean(this.EditArea.Tag) == false)
            {
                NoticeMessage("当前没有进行任何编辑！");
                return;
            }
            ShowMsgBox("是否确定保存？",
            new Action(() =>
            {
                var result = 0;
                var value = (DB.Dairy)this.EditArea.DataContext;

                var CheckSaveResult = new Action(() =>
                {
                    if (result > 0)
                    {
                        this.EditArea.Tag = false;
                        this.EditArea.Visibility = Visibility.Hidden;
                        LoadData();
                        if (this.ViewArea.IsEnabled == false)
                        {
                            this.ViewArea.IsEnabled = true;
                            value.HasNext = ((DB.Dairy)this.ViewArea.DataContext).HasNext;
                            value.HasPrevious = ((DB.Dairy)this.ViewArea.DataContext).HasPrevious;
                            this.ViewArea.DataContext = value;
                            this.ViewArea.Visibility = Visibility.Visible;
                        }
                        else
                        {
                            this.Details.Visibility = Visibility.Visible;
                        }
                        NoticeMessage("保存成功！");
                    }
                    else
                    {
                        NoticeMessage("保存失败！");
                    }
                });

                if (value.IsNew)
                {
                    if (this.db.CheckIsExists(value.WroteDate))
                    {
                        ShowMsgBox("当前日期已存在，是否覆盖？",
                            new Action(() =>
                            {
                                result = this.db.ModifyData(value);
                                CheckSaveResult();
                            }));
                        return;
                    }
                    else
                    {
                        result = this.db.AddData(value);
                    }
                }
                else
                {
                    result = this.db.ModifyData(value);
                }

                CheckSaveResult();
            }));
        }

        private void ShowMsgBox(string message, Action actionForOk, Action actionForCancel = null)
        {
            this.MsgBox.DataContext = new { MsgText = message };
            this.MsgBox.Visibility = Visibility.Visible;
            RoutedEventHandler OkEvent = null, CancelEvent = null;
            var ClearEvent = new Action(() =>
            {
                this.MsgBtnOk.Click -= OkEvent;
                this.MsgBtnCancel.Click -= CancelEvent;
            });
            OkEvent = (sender, args) => { ClearEvent(); actionForOk(); };
            CancelEvent = (sender, args) => { ClearEvent(); actionForCancel?.Invoke(); };
            this.MsgBtnOk.Click += OkEvent;
            this.MsgBtnCancel.Click += CancelEvent;
        }

        private void MsgBtn_Click(object sender, RoutedEventArgs e)
        {
            this.MsgBox.Visibility = Visibility.Hidden;
        }

        private void NoticeMessage(string message)
        {
            this.NoticeBar.Text = message;
            this.NoticeBar.Visibility = Visibility.Visible;
            this.storyboard.Stop();
            this.storyboard.Begin();
        }

        private void Details_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            e.Handled = true;
        }

        private void Select_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            var checkbox = ((Grid)sender).Children.OfType<CheckBox>().First();
            checkbox.IsChecked = !checkbox.IsChecked;
        }

        private void Details_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (this.Details.Visibility == Visibility.Hidden)
            {
                this.NoOption.Visibility = Visibility.Visible;
            }
            else
            {
                this.NoOption.Visibility = Visibility.Hidden;
            }
        }

        private void Details_ScrollChanged(object sender, ScrollChangedEventArgs e)
        {
            var scrollviewer = (ScrollViewer)e.OriginalSource;
            if (scrollviewer.VerticalOffset == scrollviewer.ScrollableHeight)
            {
                this.Pages.Visibility = Visibility.Visible;
            }
            else
            {
                this.Pages.Visibility = Visibility.Hidden;
            }
        }

        private void DataExport_Click(object sender, RoutedEventArgs e)
        {
            ShowMsgBox($"确定导出全部数据？",
                new Action(() =>
                {
                    var dlg = new Microsoft.Win32.SaveFileDialog
                    {
                        InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                    };
                    ;
                    dlg.FileName = "data";
                    dlg.DefaultExt = ".json";
                    dlg.Filter = "Json文件(.json)|*.json";
                    dlg.CheckPathExists = true;

                    var result = dlg.ShowDialog();
                    if (result == true)
                    {
                        var filename = dlg.FileName;
                        var content = JsonData.ToJsonString(this.db.GetTable());
                        System.IO.File.WriteAllText(filename, content, Encoding.UTF8);
                        NoticeMessage("导出成功");
                    }
                }));
        }

        private void DataImport_Click(object sender, RoutedEventArgs e)
        {
            ShowMsgBox($"确定要从文件读取数据到存储库？",
                new Action(() =>
                {
                    var dlg = new Microsoft.Win32.OpenFileDialog
                    {
                        InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop)
                    };
                    ;
                    dlg.FileName = "";
                    dlg.DefaultExt = "";
                    dlg.Filter = "Json文件(.json)|*.json";
                    dlg.CheckPathExists = true;
                    dlg.CheckFileExists = true;

                    var result = dlg.ShowDialog();
                    if (result == true)
                    {
                        var filename = dlg.FileName;
                        var content = System.IO.File.ReadAllText(filename);
                        var dt = this.db.GetTable(false);
                        if (!JsonData.ToDbData(content, ref dt))
                        {
                            NoticeMessage("导入失败，文件格式不符！");
                            return;
                        }
                        this.db.UpdateTable(dt);
                        NoticeMessage("导入成功");
                        LoadData();
                    }
                }));
        }
    }
}
