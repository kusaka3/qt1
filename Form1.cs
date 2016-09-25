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

        public Form1()
        {
            InitializeComponent();

            List<string> list = new List<string>(); //プロセスのフルパスを格納する
            list = GetProcess(); //プロセスのリストを取得
            list.Sort(); //プロセスのパスの並べ替え

            foreach (string s in list)
            {
                Icon ico = Icon.ExtractAssociatedIcon(s);   //プロセスの実行ファイルのアイコンを取得
                menu.Items.Add(s, ico.ToBitmap());
            }

            menu.Items.Add(new ToolStripSeparator()); //最後にセパレーターとCancelを追加
            menu.Items.Add("Cancel");

            timer.Enabled = true; //非同期処理用でタイマー起動
        }

        private void timer_Tick(object sender, EventArgs e)
        {
            menu.Show(Cursor.Position); //マウスカーソルの位置にメニューを表示
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
                    KillProcess(System.IO.Path.GetFileNameWithoutExtension(item.Text));
                    break;
                case MouseButtons.Right: //メニューを右クリック
                    System.Diagnostics.Debug.WriteLine("Right Click");
                    StartProcess(appSettings.rightClick,
                                 System.IO.Path.GetDirectoryName(item.Text));
                    break;
                case MouseButtons.Middle: //メニュー中クリック
                    System.Diagnostics.Debug.WriteLine("Middle Click");
                    StartProcess(appSettings.middleClick,
                                 System.IO.Path.GetDirectoryName(item.Text));
                    menu.Close(); //手動でメニューを閉じる
                    break;
            }
        }

        /// <summary>
        /// 重複を除いたプロセスの一覧を取得
        /// </summary>
        /// <returns>List<string></returns>
        private List<string> GetProcess()
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
