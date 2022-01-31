# N0vaMac
Desktop Live Wallpaper tool for macOS 

https://user-images.githubusercontent.com/958174/151774363-44f1ddfd-2279-442c-9c31-34970937d52d.mov


## できること
* WindowsとAndroidしかサポートしていない[N0va Desktop](https://n0vadp.mihoyo.com/)の一部ライブ背景をMacのデスクトップ背景として使えるようにします。
* 現在利用できるのは原神のパーツアニメにより作成されている背景のみです。Lumi、他のゲーム、原神でもゲーム内3Dを動画した音声付きのものは対応していません。

## 動作環境
* MacBookPro Intel macOS Catalina 10.15
* MacBookPro Intel macOS Mojave 10.14

当方M1 Mac未所持のため現状未検証です。

## ダウンロード
[こちらから](https://github.com/yoshidan/N0vaMac/releases)最新版のN0vaMac.zipをダウンロードして開いてください。解凍すると以下の二つのアプリがあります。
* N0vaMac
  - 動画を壁紙にするツールです。
* N0vaMacSetting 
  - N0vaDesktopのライブ壁紙をダウンロードしたり壁紙を変更するためのツールです。

## N0vaMacの使い方
* N0vaMacを起動してください。デフォルトで背景に動く胡桃が画面全体の背景に表示されればOKです。
* Dockに珊瑚宮心海が表示されます。
  - 右クリックして「オプション」→ 「全てのデスクトップ」を選択してください。全デスクトップの背景に適用されます。
  - 「ログイン時に開く」を選択しておくと、再起動時に自動的にアプリケーションを起動します。
  <img src="./images/setting.jpg" width="320"/>

## N0vaMacSettingの使い方
* N0vaMacSettingを起動したら、背景設定したい動画のサムネイルのDownloadボタンを押してください。
* 完了したらDownloadの表記がSelectに変わります。Selectを選択すると背景が切り替わります。
* N0vaMacSettingは背景変更する時だけ起動しておけばいいです。常時起動しておく必要はありません。

https://user-images.githubusercontent.com/958174/151774833-051e6864-429e-45cb-9072-f6228979aa06.mov
  
## その他
* デスクトップを追加した場合、一度Dockのアイコンを選択すると追加したデスクトップにも反映されます。
* 外付けデスクトップによる拡張は対応していません。拡張先には背景が適用されないです。ミラーリングは解像度が同じであれば問題ありません。
* N0vaMacSettingでSelectしても反映されない場合、localhost:9025が他のプロセスに使われている可能性があります。
  - 頻繁に発生するようでしたらissue立ててください。ポート変更できるようにします。 
