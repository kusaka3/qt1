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
    public partial class Form1 : Form
    {

        Settings appSettings = new Settings(); //app.configの読み込み
        CommandLineArgs CommandLine = new CommandLineArgs();
        System.Diagnostics.Stopwatch sw = new System.Diagnostics.Stopwatch(); //インクリメンタルサーチの入力期間を量るストップウォッチ
        string searchString = string.Empty; //インクリメンタルサーチ用の検索文字列

        public Form1()
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

        private void timer_Tick(object sender, EventArgs e)
        {
            int x, y;
            x = (CommandLine.left >= 0) ? CommandLine.left : Cursor.Position.X;
            y = (CommandLine.top >= 0) ? CommandLine.top : Cursor.Position.Y;
             
            menu.Show(x,y); //マウスカーソルの位置にメニューを表示
            timer.Enabled = false;
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

            if (item == null)
                return;

            switch (e.Button)
            {
                case MouseButtons.Left: //メニューを左クリック
                    System.Diagnostics.Debug.WriteLine("Left Click");
                    KillProcess(System.IO.Path.GetFileNameWithoutExtension(GetProcessPathFromExe(item.Text)));
                    break;
                case MouseButtons.Right: //メニューを右クリック
                    System.Diagnostics.Debug.WriteLine("Right Click");
                    StartProcess(appSettings.rightClick,
                                 System.IO.Path.GetDirectoryName(GetProcessPathFromExe(item.Text)));
                    break;
                case MouseButtons.Middle: //メニュー中クリック
                    System.Diagnostics.Debug.WriteLine("Middle Click");
                    StartProcess(appSettings.middleClick,
                                 System.IO.Path.GetDirectoryName(GetProcessPathFromExe(item.Text)));
                    menu.Close(); //手動でメニューを閉じる
                    break;
            }
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
                menu.Close(); //手動でメニューを閉じる
                return;
            }

            //インクリメンタルサーチ用の検索文字の入力
            if (e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z)
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

                searchString += e.KeyCode.ToString();
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
        /// <param name="s">プロセス名</param>
        private void KillProcess(string s)
        {
            System.Diagnostics.Process[] ps =
                System.Diagnostics.Process.GetProcessesByName(s);

            foreach (System.Diagnostics.Process p in ps)
            {
                p.Kill();
            }
        }

        /// <summary>
        /// プロセスを実行する
        /// </summary>
        /// <param name="path">プロセスのフルパス</param>
        /// <param name="arg">引数</param>
        private void StartProcess(string path, string arg)
        {
            try
            {
                System.Diagnostics.Process.Start(path, arg);
            }
            catch (FieldAccessException e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
            }
        }

        private void menu_Closed(object sender, ToolStripDropDownClosedEventArgs e)
        {
            Application.Exit(); //自分自身を終了させる
        }
    }
}
