namespace qt1
{
    partial class qtMain
    {
        /// <summary>
        /// 必要なデザイナー変数です。
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// 使用中のリソースをすべてクリーンアップします。
        /// </summary>
        /// <param name="disposing">マネージ リソースを破棄する場合は true を指定し、その他の場合は false を指定します。</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region コンポーネント デザイナーで生成されたコード

        /// <summary>
        /// デザイナー サポートに必要なメソッドです。このメソッドの内容を
        /// コード エディターで変更しないでください。
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            this.menu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.timer = new System.Windows.Forms.Timer(this.components);
            // 
            // menu
            // 
            this.menu.Name = "menu";
            this.menu.Size = new System.Drawing.Size(61, 4);
            this.menu.Closed += new System.Windows.Forms.ToolStripDropDownClosedEventHandler(this.menu_Closed);
            this.menu.MouseClick += new System.Windows.Forms.MouseEventHandler(this.menu_MouseClick);
            this.menu.PreviewKeyDown += new System.Windows.Forms.PreviewKeyDownEventHandler(this.menu_PreviewKeyDown);
            this.menu.KeyDown += new System.Windows.Forms.KeyEventHandler(this.menu_KeyDown);
            this.menu.KeyUp += new System.Windows.Forms.KeyEventHandler(this.menu_KeyUp);
            // 
            // timer
            // 
            this.timer.Interval = 10;
            this.timer.Tick += new System.EventHandler(this.timer_Tick);
        }

        #endregion

        private System.Windows.Forms.ContextMenuStrip menu;
        private System.Windows.Forms.Timer timer;
    }
}
