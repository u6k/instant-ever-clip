# InstantEverClip
WebページやURLを瞬時にEvernoteにクリップするアプリケーションです。

## 目的
Evernote公式アプリのWebクリップや、EverClipなどでWebページを簡単にクリップすることができますが、クリップした後にノートブックやタグを手動で設定する必要があり、けっこう面倒です。あらかじめ、ノートブック設定・タグ設定・クリップ品質をパターン登録しておいて、パターンをタップすることでクリップできると楽でいいなと思い、InstantEverClipを考えました。

## 対象OS
- Android 4.1以上
- iOS 9.0以上

## 機能
- 入力されたURLを新規ノートにクリップします。

## 使い方
- テキストボックスにURLを入力して、ボタンを押下します。

## ビルド方法
- Xamarinでソリューションを開きます。
- 開発者トークンをプロパティに設定します。

### Androidの場合
- apkをエクスポートします。

### iOSの場合
- Xcodeで`me.u6k.InstantEverClip`プロジェクトを作成して、フリー証明書を発行しておく。
- `InstantEverClip.iOS`プロジェクトに証明書を設定する。

## 依存ライブラリ
- [ReadJEnc](http://hp.vector.co.jp/authors/VA055804/) Ver 1.2.2.0309 (2015/03/09)
    - 著作権表示: Copyright (C) 2014-2015 hnx8(H.Takahashi)
    - ライセンス: [the MIT license](http://opensource.org/licenses/mit-license.php)
