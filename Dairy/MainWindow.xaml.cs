using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace Dairy {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {
        private DB db;
        private Storyboard storyboard;
        private int currentPage;
        private List<Button> pageButtons;

        public MainWindow() {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            db = new DB();
            InitAnimation();
            InitPage();
            LoadData();
        }

        private void InitAnimation() {
            storyboard = new Storyboard();
            var frame = new ThicknessAnimationUsingKeyFrames() { BeginTime = TimeSpan.Zero };
            frame.KeyFrames.Add(new SplineThicknessKeyFrame(new Thickness(0, -26, 0, 0), KeyTime.FromTimeSpan(TimeSpan.Zero)));
            frame.KeyFrames.Add(new SplineThicknessKeyFrame(new Thickness(0, -13, 0, 0), KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 250))));
            frame.KeyFrames.Add(new SplineThicknessKeyFrame(new Thickness(0, 0, 0, 0), KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 0, 500))));
            frame.KeyFrames.Add(new SplineThicknessKeyFrame(new Thickness(0, 0, 0, 0), KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 1, 500))));
            frame.KeyFrames.Add(new SplineThicknessKeyFrame(new Thickness(0, -13, 0, 0), KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 1, 750))));
            frame.KeyFrames.Add(new SplineThicknessKeyFrame(new Thickness(0, -26, 0, 0), KeyTime.FromTimeSpan(new TimeSpan(0, 0, 0, 2, 0))));
            storyboard.Children.Add(frame);
            Storyboard.SetTarget(frame, NoticeBar);
            Storyboard.SetTargetProperty(frame, new PropertyPath(MarginProperty));
            storyboard.Completed += new EventHandler((obj, arg) => {
                NoticeBar.Visibility = Visibility.Hidden;
            });
        }

        private void InitPage() {
            currentPage = 1;
            pageButtons = (from button in Pages.Children.OfType<StackPanel>().First().Children.OfType<Button>()
                           where button.Content is "0"
                           select button).ToList();
        }

        private void LoadData() {
            Details.ItemsSource = null;
            PageChanged();
            var value = db.GetData(currentPage);
            if (value == null) {
                Details.ItemsSource = null;
                NoDairy.Visibility = Visibility.Visible;
                return;
            }
            NoDairy.Visibility = Visibility.Hidden;
            Details.ItemsSource = value;
        }

        private void PageChanged() {
            var pageCount = db.GetPageCount();
            if (pageCount == 1) {
                Pages.Height = 0;
                return;
            }
            Pages.Height = 40;
            //上一页
            if (currentPage == 1) {
                PagePrevious.IsEnabled = false;
                PagePrevious.Tag = null;
            } else {
                PagePrevious.Tag = currentPage - 1;
                PagePrevious.IsEnabled = true;
            }
            //下一页
            if (currentPage == pageCount) {
                PageNext.IsEnabled = false;
                PageNext.Tag = null;
            } else {
                PageNext.IsEnabled = true;
                PageNext.Tag = currentPage + 1;
            }
            if (pageCount < 8) {
                PageLeft.Width = 0;
                PageRight.Width = 0;
                int i;
                for (i = 0; i < pageCount; i++) {
                    pageButtons[i].Content = i + 1;
                    pageButtons[i].Tag = i + 1;
                    pageButtons[i].Width = 32;
                }
                for (; i < pageButtons.Count; i++) {
                    pageButtons[i].Width = 0;
                }
            } else {
                if (currentPage - 1 > 3) {
                    PageLeft.Width = 32;
                } else {
                    PageLeft.Width = 0;
                }

                if (pageCount - currentPage > 3) {
                    PageRight.Width = 32;
                } else {
                    PageRight.Width = 0;
                }

                pageButtons[0].Content = 1;
                pageButtons[0].Tag = 1;
                pageButtons[0].Width = 32;
                pageButtons[0].Focusable = true;
                pageButtons[6].Content = pageCount;
                pageButtons[6].Tag = pageCount;
                pageButtons[6].Width = 32;
                pageButtons[6].Focusable = true;

                if (PageRight.Width != 0 && PageLeft.Width != 0) {
                    int startPage = currentPage - 3;
                    for (int i = 1; i < 6; i++) {
                        pageButtons[i].Content = i + startPage;
                        pageButtons[i].Tag = i + startPage;
                        pageButtons[i].Width = 32;
                        pageButtons[i].Focusable = true;
                    }
                } else if (PageRight.Width == 0) {
                    for (int i = 5; i > 0; i--) {
                        pageButtons[6 - i].Content = pageCount - i;
                        pageButtons[6 - i].Tag = pageCount - i;
                        pageButtons[6 - i].Width = 32;
                        pageButtons[6 - i].Focusable = true;
                    }
                } else if (PageLeft.Width == 0) {
                    for (int i = 1; i < 6; i++) {
                        pageButtons[i].Content = i + 1;
                        pageButtons[i].Tag = i + 1;
                        pageButtons[i].Width = 32;
                        pageButtons[i].Focusable = true;
                    }
                }

                (from btn in pageButtons
                 where ((int)btn.Content) == currentPage
                 select btn).First().Focusable = false;

            }
        }

        private void AppOption_Click(object sender, RoutedEventArgs e) {
            if (Options.Visibility == Visibility.Hidden) {
                FreezeAll.Visibility = Visibility.Visible;
                Options.Visibility = Visibility.Visible;
                ((Button)sender).Background = new SolidColorBrush(Color.FromArgb(255, 255, 0, 0));
            } else {
                FreezeAll.Visibility = Visibility.Hidden;
                Options.Visibility = Visibility.Hidden;
                ((Button)sender).Background = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
            }
        }

        private void PerPageCount_TextChanged(object sender, TextChangedEventArgs e) {
            if (PerPageCount.Text == Properties.Settings.Default.PerPageCount.ToString()) return;
            ApplyPerPageCount.IsEnabled = true;
        }

        private void ApplyPerPageCount_Click(object sender, RoutedEventArgs e) {
            int num;
            if (!int.TryParse(PerPageCount.Text, out num)) {
                NoticeMessage("请输入正确的数字");
                PerPageCount.Text = Properties.Settings.Default.PerPageCount.ToString();
                PerPageCount.Focus();
                ApplyPerPageCount.IsEnabled = false;
                return;
            }
            if (Properties.Settings.Default.PerPageCount != num) {
                Properties.Settings.Default.PerPageCount = num;
                NoticeMessage("修改成功");
                LoadData();
            }
            ApplyPerPageCount.IsEnabled = false;
        }

        private void OrderByAsc_Changed(object sender, RoutedEventArgs e) {
            var content = ((RadioButton)sender).Content.ToString();
            bool orderbyasc;
            if (content.ToLower() == "desc") {
                orderbyasc = false;
            } else {
                orderbyasc = true;
            }
            if (Properties.Settings.Default.OrderByAsc != orderbyasc) {
                Properties.Settings.Default.OrderByAsc = orderbyasc;
                NoticeMessage("修改成功");
                LoadData();
            }
        }

        private void PageJump_Click(object sender, RoutedEventArgs e) {
            currentPage = Convert.ToInt32(((Button)sender).Tag);
            PageJump.Text = "";
            LoadData();
        }

        private void MultiSelect_Click(object sender, RoutedEventArgs e) {
            if (NoDairy.Visibility == Visibility) return;
            var num = Convert.ToInt32(Details.Tag);
            if (num == 0) {
                ((Button)sender).Background = new SolidColorBrush(Color.FromArgb(150, 255, 0, 0));
                Details.Tag = 60;
                MultiCheckAll.Visibility = Visibility.Visible;
                MultiCheckOther.Visibility = Visibility.Visible;
                MultiDelete.Visibility = Visibility.Visible;
                MultiExport.Visibility = Visibility.Visible;
            } else {
                ((Button)sender).Background = new SolidColorBrush(Color.FromArgb(255, 0, 0, 0));
                Details.Tag = 0;
                MultiCheckAll.Visibility = Visibility.Hidden;
                MultiCheckOther.Visibility = Visibility.Hidden;
                MultiDelete.Visibility = Visibility.Hidden;
                MultiExport.Visibility = Visibility.Hidden;
                var value = (List<DB.Dairy>)Details.ItemsSource;
                for (int i = 0; i < value.Count; i++) {
                    value[i].IsSelected = false;
                }
                Details.ItemsSource = null;
                Details.ItemsSource = value;
                Select_Changed(null, null);
            }
        }

        private void Select_Changed(object sender, RoutedEventArgs e) {
            var count = (from dairy in (List<DB.Dairy>)Details.ItemsSource
                         where dairy.IsSelected
                         select dairy).Count();
            if (count > 0) {
                MultiDelete.IsEnabled = true;
                MultiExport.IsEnabled = true;
            } else {
                MultiDelete.IsEnabled = false;
                MultiExport.IsEnabled = false;
            }
        }

        private void MultiCheckAll_Click(object sender, RoutedEventArgs e) {
            var dates = (from dairy in (List<DB.Dairy>)Details.ItemsSource
                         where dairy.IsSelected
                         select dairy.WroteDate).ToList();
            if (dates.Count == ((List<DB.Dairy>)Details.ItemsSource).Count) {
                foreach (var item in (List<DB.Dairy>)Details.ItemsSource) {
                    item.IsSelected = false;
                }
            } else {
                foreach (var item in (List<DB.Dairy>)Details.ItemsSource) {
                    item.IsSelected = true;
                }
            }
            var data = Details.ItemsSource;
            Details.ItemsSource = null;
            Details.ItemsSource = data;
            Select_Changed(null, null);
        }

        private void MultiCheckOther_Click(object sender, RoutedEventArgs e) {
            foreach (var item in (List<DB.Dairy>)Details.ItemsSource) {
                if (item.IsSelected) {
                    item.IsSelected = false;
                } else {
                    item.IsSelected = true;
                }
            }
            var data = Details.ItemsSource;
            Details.ItemsSource = null;
            Details.ItemsSource = data;
            Select_Changed(null, null);
        }

        private void MultiDelete_Click(object sender, RoutedEventArgs e) {
            var dates = (from dairy in (List<DB.Dairy>)Details.ItemsSource
                         where dairy.IsSelected
                         select dairy.WroteDate).ToList();
            ShowMsgBox($"是否确定删除选中的{dates.Count()}条数据？",
                new Action(() => {
                    var result = db.DeleteData(dates);
                    if (result > 0) {
                        LoadData();
                        NoticeMessage("删除成功！");
                    } else {
                        NoticeMessage("删除失败！");
                    }
                }));
        }

        private void MultiExport_Click(object sender, RoutedEventArgs e) {
            var dates = (from dairy in (List<DB.Dairy>)Details.ItemsSource
                         where dairy.IsSelected
                         select dairy.WroteDate).ToList();
            ShowMsgBox($"确定导出选中的{dates.Count()}条数据？",
                new Action(() => {
                    Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                    dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop); ;
                    dlg.FileName = "Dairy";
                    dlg.DefaultExt = ".txt";
                    dlg.Filter = "文本文件(.txt)|*.txt|网页(.html)|*.html";
                    dlg.CheckPathExists = true;

                    bool? result = dlg.ShowDialog();
                    if (result == true) {
                        string filename = dlg.FileName;
                        var ext = System.IO.Path.GetExtension(filename);
                        var ecoding = Export.ExportEcoding.text;
                        if (ext == ".html") {
                            ecoding = Export.ExportEcoding.html;
                        }
                        var content = new Export(ecoding, db.GetDetail(dates)).Content;

                        System.IO.File.WriteAllText(filename, content, Encoding.UTF8);
                        NoticeMessage("导出成功");
                    }
                }));
        }

        private void AddNew_Click(object sender, RoutedEventArgs e) {
            var addNew = new Action(() => {
                EditArea.DataContext = new DB.Dairy() {
                    IsNew = true,
                    WroteDate = DateTime.Now.ToShortDateString(),
                    Thema = string.Empty,
                    Wheather = "晴",
                    Content = string.Empty,
                };

                Details.Visibility = Visibility.Hidden;
                ViewArea.Visibility = Visibility.Hidden;
                EditArea.Visibility = Visibility.Visible;
                EditArea.Tag = false;
            });

            if (EditArea.Visibility == Visibility.Visible && Convert.ToBoolean(EditArea.Tag)) {
                ShowMsgBox("是否放弃当前已修改的内容？", addNew);
            } else {
                addNew();
            }
        }

        private void ViewClose_Click(object sender, RoutedEventArgs e) {
            ViewArea.Visibility = Visibility.Hidden;
            Details.Visibility = Visibility.Visible;
        }

        private void Item_MouseDown(object sender, MouseButtonEventArgs e) {
            var date = sender.GetType().GetProperty("Tag").GetValue(sender)?.ToString();
            var value = db.GetDetail(date);
            db.GetDairyPosition(ref value);
            ViewArea.DataContext = value;
            ViewArea.Visibility = Visibility.Visible;
            Details.Visibility = Visibility.Hidden;
        }

        private void View_Click(object sender, RoutedEventArgs e) {
            Item_MouseDown(sender, null);
        }

        private void EditDairy_Click(object sender, RoutedEventArgs e) {
            if (ViewArea.Visibility == Visibility.Visible) ViewArea.IsEnabled = false;
            var date = ((Control)sender).Tag.ToString();
            EditArea.DataContext = db.GetDetail(date);
            Details.Visibility = Visibility.Hidden;
            ViewArea.Visibility = Visibility.Hidden;
            EditArea.Visibility = Visibility.Visible;
            EditArea.Tag = false;
        }

        private void RemoveDairy_Click(object sender, RoutedEventArgs e) {
            ShowMsgBox("是否确定删除？",
                new Action(() => {
                    var date = ((Control)sender).Tag.ToString();
                    var result = db.DeleteData(date);
                    if (result > 0) {
                        LoadData();
                        ViewArea.Visibility = Visibility.Hidden;
                        Details.Visibility = Visibility.Visible;
                        NoticeMessage("删除成功！");
                    } else {
                        NoticeMessage("删除失败！");
                    }
                }));
        }

        private void EditWheather_TextChanged(object sender, TextChangedEventArgs e) {
            if (((ComboBox)sender).Text.Length > 10) {
                ((ComboBox)sender).Text = ((ComboBox)sender).Text.Substring(0, 10);
            }
        }

        private void EditDrop_Click(object sender, RoutedEventArgs e) {
            //未编辑
            if (!Convert.ToBoolean(EditArea.Tag)) {
                EditArea.Visibility = Visibility.Hidden;
                if (ViewArea.IsEnabled == false) {
                    ViewArea.IsEnabled = true;
                    ViewArea.Visibility = Visibility.Visible;
                } else {
                    Details.Visibility = Visibility.Visible;
                }
                return;
            }

            ShowMsgBox("是否放弃当前已修改的内容？",
                new Action(() => {
                    EditArea.Visibility = Visibility.Hidden;
                    if (ViewArea.IsEnabled == false) {
                        ViewArea.IsEnabled = true;
                        ViewArea.Visibility = Visibility.Visible;
                    } else {
                        Details.Visibility = Visibility.Visible;
                    }
                    EditArea.Tag = false;
                }));
        }

        private void EditArea_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {
            if (EditArea.IsVisible) {
                EditWheather.SelectionChanged += EditSelectorChanged;
                EditThema.TextChanged += EditTextChanged;
                EditContent.TextChanged += EditTextChanged;
            } else if (Convert.ToBoolean(EditArea.Tag) == false) {
                RemoveAllChangedEvent();
            }
        }

        private void EditSelectorChanged(object sender, SelectionChangedEventArgs e) {
            if (string.IsNullOrEmpty(EditContent.Text)) return;
            EditArea.Tag = true;
            RemoveAllChangedEvent();
        }

        private void EditTextChanged(object sender, TextChangedEventArgs e) {
            if (((TextBox)sender).Name != "EditContent" && string.IsNullOrEmpty(EditContent.Text)) return;
            EditArea.Tag = true;
            RemoveAllChangedEvent();
        }

        private void RemoveAllChangedEvent() {
            EditWheather.SelectionChanged -= EditSelectorChanged;
            EditThema.TextChanged -= EditTextChanged;
            EditContent.TextChanged -= EditTextChanged;
        }

        private void EditSave_Click(object sender, RoutedEventArgs e) {
            if (Convert.ToBoolean(EditArea.Tag) == false) {
                NoticeMessage("当前没有进行任何编辑！");
                return;
            }
            ShowMsgBox("是否确定保存？",
            new Action(() => {
                int result = 0;
                var value = (DB.Dairy)EditArea.DataContext;

                var CheckSaveResult = new Action(() => {
                    if (result > 0) {
                        EditArea.Tag = false;
                        EditArea.Visibility = Visibility.Hidden;
                        LoadData();
                        if (ViewArea.IsEnabled == false) {
                            ViewArea.IsEnabled = true;
                            ViewArea.DataContext = EditArea.DataContext;
                            ViewArea.Visibility = Visibility.Visible;
                        } else {
                            Details.Visibility = Visibility.Visible;
                        }
                        NoticeMessage("保存成功！");
                    } else {
                        NoticeMessage("保存失败！");
                    }
                });

                if (value.IsNew) {
                    if (db.CheckIsExists(value.WroteDate)) {
                        ShowMsgBox("当前日期已存在，是否覆盖？",
                            new Action(() => {
                                result = db.ModifyData(value);
                                CheckSaveResult();
                            }));
                        return;
                    } else {
                        result = db.AddData(value);
                    }
                } else {
                    result = db.ModifyData(value);
                }

                CheckSaveResult();
            }));
        }

        private void ShowMsgBox(string message, Action actionForOk, Action actionForCancel = null) {
            MsgBox.DataContext = new { MsgText = message };
            MsgBox.Visibility = Visibility.Visible;
            RoutedEventHandler OkEvent = null, CancelEvent = null;
            var ClearEvent = new Action(() => {
                MsgBtnOk.Click -= OkEvent;
                MsgBtnCancel.Click -= CancelEvent;
            });
            OkEvent = (sender, args) => { ClearEvent(); actionForOk(); };
            CancelEvent = (sender, args) => { ClearEvent(); actionForCancel?.Invoke(); };
            MsgBtnOk.Click += OkEvent;
            MsgBtnCancel.Click += CancelEvent;
        }

        private void MsgBtn_Click(object sender, RoutedEventArgs e) {
            MsgBox.Visibility = Visibility.Hidden;
        }

        private void NoticeMessage(string message) {
            NoticeBar.Text = message;
            NoticeBar.Visibility = Visibility.Visible;
            storyboard.Stop();
            storyboard.Begin();
        }

        private void Details_PreviewKeyDown(object sender, KeyEventArgs e) {
            e.Handled = true;
        }

        private void Select_MouseLeftButtonDown(object sender, MouseButtonEventArgs e) {
            var checkbox = ((Grid)sender).Children.OfType<CheckBox>().First();
            checkbox.IsChecked = !checkbox.IsChecked;
        }

        private void Details_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e) {
            if (Details.Visibility == Visibility.Hidden) {
                NoOption.Visibility = Visibility.Visible;
            } else {
                NoOption.Visibility = Visibility.Hidden;
            }
        }

        private void Details_ScrollChanged(object sender, ScrollChangedEventArgs e) {
            var scrollviewer = (ScrollViewer)e.OriginalSource;
            if (scrollviewer.VerticalOffset == scrollviewer.ScrollableHeight) {
                Pages.Visibility = Visibility.Visible;
            } else {
                Pages.Visibility = Visibility.Hidden;
            }
        }

        private void DataExport_Click(object sender, RoutedEventArgs e) {
            ShowMsgBox($"确定导出全部数据？",
                new Action(() => {
                    Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
                    dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop); ;
                    dlg.FileName = "data";
                    dlg.DefaultExt = ".json";
                    dlg.Filter = "Json文件(.json)|*.json";
                    dlg.CheckPathExists = true;

                    bool? result = dlg.ShowDialog();
                    if (result == true) {
                        string filename = dlg.FileName;
                        var content = JsonData.ToJsonString(db.GetTable());
                        System.IO.File.WriteAllText(filename, content, Encoding.UTF8);
                        NoticeMessage("导出成功");
                    }
                }));
        }

        private void DataImport_Click(object sender, RoutedEventArgs e) {
            ShowMsgBox($"确定要从文件读取数据到存储库？",
                new Action(() => {
                    Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
                    dlg.InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.Desktop); ;
                    dlg.FileName = "";
                    dlg.DefaultExt = "";
                    dlg.Filter = "Json文件(.json)|*.json";
                    dlg.CheckPathExists = true;
                    dlg.CheckFileExists = true;

                    bool? result = dlg.ShowDialog();
                    if (result == true) {
                        string filename = dlg.FileName;
                        var content = System.IO.File.ReadAllText(filename);
                        var dt = db.GetTable(false);
                        if (!JsonData.ToDbData(content, ref dt)) {
                            NoticeMessage("导入失败，文件格式不符！");
                            return;
                        }
                        db.UpdateTable(dt);
                        NoticeMessage("导入成功");
                        LoadData();
                    }
                }));
        }
    }
}
