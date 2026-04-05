# Animation Window Enhancer

Unity 標準の Animation Window の DopeSheet 上に、カーブや色のプレビューを表示するパッケージです。

[![license](https://img.shields.io/badge/LICENSE-MIT-green.svg)](LICENSE.md)

> [English](README.md)

## インストール

1. **Window > Package Manager** からパッケージマネージャを開く
2. 左上の「+」ボタンをクリックし、**Add package from git URL** を選択
3. 以下の URL を入力: `https://github.com/yuyu0127/AnimationWindowEnhancer.git`

または、`Packages/manifest.json` の `dependencies` に以下を追加することでもインストールが可能です。

```json
{
    "dependencies": {
        "com.yuyu.animationwindowenhancer": "https://github.com/yuyu0127/AnimationWindowEnhancer.git"
    }
}
```

## 機能

### カーブ・色プレビュー

DopeSheet 上に値変化のカーブと色変化のプレビューを表示します。

<img width="784" alt="curve and color preview" src="https://github.com/user-attachments/assets/6f932182-42e7-4f53-b2e1-79368a8d75a3" />

### ラベル表示

DopeSheet およびカーブエディタ上にラベルを表示します。

<img width="784" alt="label" src="https://github.com/user-attachments/assets/0afa4529-4585-4075-8a84-106ef91bd798" />

## 設定

<img width="598" alt="preferences" src="https://github.com/user-attachments/assets/d498b292-bd8a-465a-b15d-8a469bdd4044" />

### Curve
- Default Heatmap: オーバーライドが設定されていないカーブに使用されるヒートマップ
- Heatmap Overrides: プロパティ名に基づいて特定のカーブのヒートマップを上書きする
- Resolution: カーブ描画時の1フレームあたりのサンプル数

### Label
- Color: 各 DopeeLine のラベルの色
- Font Size: 各 DopeLine のラベルのフォントサイズ

### Others
- Color Band Height: 色プロパティの DopeLine に表示される帯の高さ
- Parent Dope Line Color: 子階層を持つ DopeLine の色
