# N0vaMac
Desktop Live Wallpaper tool for macOS 

https://user-images.githubusercontent.com/958174/151774363-44f1ddfd-2279-442c-9c31-34970937d52d.mov


## できること
* WindowsとAndroidしかサポートしていない[N0va Desktop]((https://n0vadp.mihoyo.com/)の一部ライブ背景をMacのデスクトップ背景として使えるようにします。
* 現在利用できるのは原神のパーツアニメにより作成されている背景のみです。Lumi、他のゲーム、原神でもゲーム内3Dを動画した音声付きのものは対応していません。

## ダウンロード
[最新のリリースパッケージ](./released/latest)をダウンロードして開いてください。以下の二つのファイルがあります。
* N0vaMac
  - N0va Desktopの動画を壁紙にするツールです。
* N0vaMacSetting 
  - ダウンロードしたり壁紙を変更するためのツールです。

## N0vaMacの使い方
* N0vaMacを起動してください。デフォルトで背景に動く胡桃が画面全隊の背景に表示されればOKです。
* Dockに珊瑚宮心海が表示されます。
  - 右クリックして「オプション」→ 「全てのデスクトップ」を選択してください。全デスクトップの背景に適用されます。
* 「ログイン時に開く」を選択しておくと、再起動時に自動的にアプリケーションを起動します。
* 起動中でもDockからアイコンを非表示にしたい場合は`N0vaMac.app/Content/Info.plist`を開いて、<dict>タグ内に以下の記述を追加して再起動してください。なおこの記述追加後は、アプリを停止するためにはActivty Monitor等からタスクキルが必要になります。
  ```
  <key>LSUIElement</key>
  <true/>
  ```

## N0vaMacSettingの使い方
* N0vaMacSettingを起動したら、背景設定したい動画のサムネイルのDownloadボタンを押してください。
* 完了したらDownloadの表記がSelectに変わります。Selectを選択すると背景が切り替わります。
* N0vaMacSettingは背景変更する時だけ起動しておけばいいです。常時起動しておく必要はありません。

## その他
* デスクトップを追加した場合、一度Dockのアイコンを選択すると追加したデスクトップにも反映されます。
* 外付けデスクトップによる拡張は対応していません。拡張先には背景が適用されないです。ミラーリングは解像度が同じであれば問題ありません。
* N0vaMacSettingでSelectしても反映されない場合、localhost:9025が他のプロセスに使われている可能性があります。
  - 頻繁に発生するようでしたらissue立ててください。ポート変更できるようにします。 
