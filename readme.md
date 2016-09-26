# qt1

「qt0」を模倣した64bitプロセス対応メニュー式タスクマネージャ

![qt1](./Preview.png)

## 概要

10年以上愛用している「qt0」が64bit未対応なので、64bitプロセスを表示しつつも「qt0」っぽい動きをするアプリケーションを作ってみました。.NETFrameworkを使用した簡易版です。

## 対応OS

- Windows 10 .NETFramework v4.5.2

## 機能

- プロセスをパス付きでメニューに表示
- [未実装]メニューのリストをキーボードでインクリメンタルサーチ
- 左クリックで強制終了
- 右クリックで親フォルダを開く
- [未実装]左・右・中クリック時の動作設定

### 独自の機能

- 64bitアプリケーションを表示
- 重複したプロセスを省略
- 指定ワードを含むプロセスをリストから除外(例：system32)

## [未実装]起動オプション

- /ver バージョン情報を表示
- /top=Y メニュー出現位置Y
- /left=X メニュー出現位置X
- /nopathname プロセス表記をパス名ではなくファイル名にする
- /noicon アイコン非表示
- EXEのパス EXEが実行しているプロセスを全て強制終了

## 想定する使用方法

- クイックランチャー等にqt1を追加して起動

## 既知の不具合

- キーボードのEnterでメニューを選択できない

## 実装しなかった機能

- ウインドウタイトルの表示
 - ウインドウタイトルを持たないプロセスをどうするか思いつかない為
 - 重複したプロセスを省略している為

## ToDo

- 除外キーワードをパスに含むプロセスを表示しない
- キーボードでインクリメンタルサーチ
- コマンドラインオプションの実装
- ファイル名のみ？
- Enter(左クリック), Shift+Enter(右クリック), Ctrl+Enter(中クリック)の実装

## License

Copyright ©  2016 Kusaka

Licensed under the [MIT](./LICENSE) License.

## Author

[kusaka](https://github.com/kusaka3/)
