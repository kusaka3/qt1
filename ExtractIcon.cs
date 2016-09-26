using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;
using System.Drawing;

namespace qt1
{
    class ExtractIcon
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Auto)]
        struct SHFILEINFO
        {
            public IntPtr hIcon;
            public IntPtr iIcon;
            public uint dwAttributes;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
            public string szDisplayName;
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
            public string szTypeName;
        }

        const uint SHGFI_SMALLICON = 0x00000001;
        const uint SHGFI_ICON = 0x00000100;

        [DllImport("shell32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SHGetFileInfo(
          string pszPath,
          int dwFileAttributes,
          ref SHFILEINFO psfi,
          uint cbfileInfo,
          uint uFlags);

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool DestroyIcon(IntPtr hIcon);

        /// <summary>
        /// 指定したアプリケーションのアイコンを取得
        /// </summary>
        /// <param name="filename">アイコンを取得したいアプリケーションのパス</param>
        /// <returns>取得したアイコンのビットマップイメージ(16x16)を返す</returns>
        public static Image GetIcon(string filename)
        {
            SHFILEINFO shinfo = new SHFILEINFO();
            uint flags = SHGFI_ICON | SHGFI_SMALLICON;
            try
            {
                SHGetFileInfo(filename, 0, ref shinfo, (uint)Marshal.SizeOf(shinfo), flags);
                if (shinfo.hIcon == IntPtr.Zero)
                    return null;
                else
                    return Icon.FromHandle(shinfo.hIcon).ToBitmap();
            }
            finally
            {
                if (shinfo.hIcon != IntPtr.Zero)
                    DestroyIcon(shinfo.hIcon);
            }

        }
    }
}
