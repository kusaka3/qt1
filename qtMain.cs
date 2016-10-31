using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace qt1
{
    public partial class qtMain : Component
    {
        Settings appSettings = new Settings(); //app.configの読み込み
        CommandLineArgs CommandLine = new CommandLineArgs();
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch(); //インクリメンタルサーチの入力期間を量るストップウォッチ
        string searchString = string.Empty; //インクリメンタルサーチ用の検索文字列

        public qtMain()
        {
            InitializeComponent();

            List<string> list = new List<string>(); //プロセスのフルパスを格納する
            list = GetProcessList(); //プロセスのリストを取得
            list.Sort(); //プロセスのパスの並べ替え

            foreach (string s in list)
            {
                string path = string.Empty;

                if (CommandLine.noPathName) //nopathnameオプション
                    path = System.IO.Path.GetFileName(s);
                else
                    path = s;

                if (CommandLine.noIcons) //noiconsオプション
                    menu.Items.Add(path);
                else
                    menu.Items.Add(path, ExtractIcon.GetIcon(s)); //プロセスのフルパスとアイコンをメニューに追加
            }

            menu.Items.Add(new ToolStripSeparator()); //最後にセパレーターとCancelを追加
            menu.Items.Add("Cancel");

            timer.Enabled = true; //非同期処理用でタイマー起動
        }

        public qtMain(IContainer container)
        {
            container.Add(this);

            InitializeComponent();
        }


        private void timer_Tick(object sender, EventArgs e)
        {
            timer.Enabled = false;

            if (System.IO.File.Exists(CommandLine.exePath)) //コマンドラインにexeのパスが渡された
            {
                KillProcess(CommandLine.exePath);
                Application.Exit(); //自分自身を終了させる
                return;
            }

            int x, y;
            x = (CommandLine.left >= 0) ? CommandLine.left : Cursor.Position.X;
            y = (CommandLine.top >= 0) ? CommandLine.top : Cursor.Position.Y;

            menu.Show(x, y); //マウスカーソルの位置にメニューを表示
            menu.BringToFront();
        }

        private void menu_MouseClick(object sender, MouseEventArgs e)
        {
            ToolStripItem item = null;
            foreach (ToolStripItem i in menu.Items)//選択されたメニューアイテムを探す
            {
                if (i.Selected)
                {
                    item = i;
                    break;
                }
            }

            if (item == null || item.Text == "Cancel")
            {
                Application.Exit(); //自分自身を終了させる
                return;
            }

            string command = string.Empty;

            switch (e.Button)
            {
                case MouseButtons.Left: //メニューを左クリック
                    System.Diagnostics.Debug.WriteLine("Left Click");
                    command = appSettings.leftClick;
                    break;
                case MouseButtons.Right: //メニューを右クリック
                    System.Diagnostics.Debug.WriteLine("Right Click");
                    command = appSettings.rightClick;
                    break;
                case MouseButtons.Middle: //メニュー中クリック
                    System.Diagnostics.Debug.WriteLine("Middle Click");
                    command = appSettings.middleClick;
                    break;
            }
            ExecuteCommand(command, GetProcessPathFromExe(item.Text)); //コマンドを実行
            menu.Close(); //手動でメニューを閉じる(中クリックではメニューを終了しないため)
        }

        private void menu_PreviewKeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            // 十字キーの左右やエンターが押されてもフォーカスを失わないようにする
            if (e.KeyCode == Keys.Left || e.KeyCode == Keys.Right || e.KeyCode == Keys.Enter)
                e.IsInputKey = true;

        }

        private void menu_KeyDown(object sender, KeyEventArgs e)
        {
            //デフォルトの挙動(C,D,Eを押すとそのドライブに移動するなど)を無効にする
            e.SuppressKeyPress = true;
        }

        private void menu_KeyUp(object sender, KeyEventArgs e)
        {
            //Enterキーを押された時の処理をマウスイベントに渡す
            if (e.KeyCode == Keys.Enter)
            {
                MouseButtons m = MouseButtons.Left;
                if (e.Shift)
                    m = MouseButtons.Right;
                if (e.Control)
                    m = MouseButtons.Middle;
                menu_MouseClick(null, new MouseEventArgs(m, 1, 0, 0, 0));
                return;
            }

            //インクリメンタルサーチ用の検索文字の入力[0-9][A-Z]
            if ((e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z) ||
                (e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9))
            {
                if (!sw.IsRunning)
                {
                    sw.Start();
                }
                else
                {
                    if (sw.ElapsedMilliseconds > 1500) //入力を開始してから1.5秒後に検索文字列を破棄
                    {
                        sw.Reset();
                        sw.Stop();
                        searchString = string.Empty;
                    }
                }
                searchString += Convert.ToChar(e.KeyValue);
                System.Diagnostics.Debug.WriteLine(searchString);
                foreach (ToolStripItem i in menu.Items) //検索文字列を含むメニューアイテムを探す
                {
                    if (i.Text.ToUpper().Contains(searchString))
                    {
                        i.Select();
                        break;
                    }
                }

            }
        }
        /// <summary>
        /// ファイル名からプロセス一覧にあるフルパスを返す
        /// </summary>
        /// <param name="exeName">プロセスのファイル名</param>
        /// <returns></returns>
        private string GetProcessPathFromExe(string exeName)
        {
            if (System.IO.File.Exists(exeName)) //元がフルパスの場合はそのまま返す
                return exeName;

            string path = string.Empty;
            System.Diagnostics.Process[] ps =
                System.Diagnostics.Process.GetProcessesByName(
                    System.IO.Path.GetFileNameWithoutExtension(exeName) //拡張子を除いたファイル名でプロセスリストを取得
                    );

            foreach (System.Diagnostics.Process p in ps)
            {
                path = p.MainModule.FileName;
                break;
            }
            return path;
        }

        /// <summary>
        /// 重複を除いたプロセスの一覧を取得
        /// </summary>
        /// <returns>List<string></returns>
        private List<string> GetProcessList()
        {
            List<string> lst = new List<string>();

            System.Diagnostics.Process[] ps = System.Diagnostics.Process.GetProcesses();
            foreach (System.Diagnostics.Process p in ps) //実行中のプロセスを取得
            {
                try
                {
                    foreach (string s in appSettings.exceptWords)
                    {
                        if (p.MainModule.FileName.ToString().ToLower().IndexOf(s.ToLower()) > 0)
                            throw new Exception();
                    }

                    //重複を除いてlistにプロセスのフルパスを追加
                    if (!lst.Contains(p.MainModule.FileName))
                    {
                        lst.Add(p.MainModule.FileName);
                    }
                }
                catch //(AccessViolationException ex) //アクセス権限が無い場合を無視
                {
                    continue;
                }
            }
            return lst;
        }

        /// <summary>
        /// プロセスを強制終了させる
        /// </summary>
        /// <param name="fullpath">プロセスのフルパス</param>
        private void KillProcess(string fullpath)
        {
            System.Diagnostics.Process[] ps =
                System.Diagnostics.Process.GetProcessesByName(
                    System.IO.Path.GetFileNameWithoutExtension(fullpath)
                    );

            foreach (System.Diagnostics.Process p in ps)
            {
                try
                {
                    p.Kill();
                }
                catch(Exception e)
                {
                    System.Diagnostics.Debug.WriteLine(e.ToString());
                    MessageBox.Show(e.Message, Application.ProductName +
                                               " ver" +
                                               Application.ProductVersion);
                }
}
        }

        /// <summary>
        /// プロセスを実行する
        /// </summary>
        /// <param name="fullpath">プロセスのフルパス</param>
        /// <param name="arg">引数</param>
        private void StartProcess(string fullpath, string arg)
        {
            try
            {
                System.Diagnostics.Process.Start(fullpath, arg);
            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                MessageBox.Show(e.Message, Application.ProductName +
                                           " ver" +
                                           Application.ProductVersion);
            }
        }

        /// <summary>
        /// コマンドを実行する
        /// </summary>
        /// <param name="command">実行するコマンドまたは引数付きのフルパス</param>
        /// <param name="pathFromMenu">メニューで選択したプロセスのフルパス</param>
        private void ExecuteCommand(string command, string pathFromMenu)
        {
            switch (command.ToLower())
            {
                case "terminate":
                    KillProcess(pathFromMenu);
                    break;
                case "explorer":
                    StartProcess(GetProcessPathFromExe("explorer"),
                                 System.IO.Path.GetDirectoryName(pathFromMenu));
                    break;
                case "reboot":
                    KillProcess(pathFromMenu);
                    StartProcess(pathFromMenu, string.Empty);
                    break;
                case "":
                    break;
                default: //コマンド以外の場合はapp.configで設定したClickのValue(例:C:\test\test.exe %D)
                    string c = string.Empty;
                    System.Text.RegularExpressions.Regex r =
                        new System.Text.RegularExpressions.Regex(@"([A-Z]\:\\|\\\\).*\.exe",
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase |
                        System.Text.RegularExpressions.RegexOptions.Singleline);
                    System.Text.RegularExpressions.Match m = r.Match(command); //パスと引数に分解
                    if (m.Success)
                        c = m.Value;
                    string args = command.Substring(m.Length).Trim().
                        Replace("%D", System.IO.Path.GetDirectoryName(pathFromMenu)).
                        Replace("%F", System.IO.Path.GetFileName(pathFromMenu));

                    StartProcess(c, args);
                    break;
            }
        }
    }
}
