# Automatically-moving-illastrations-by-Anima2D


Anima2D(Unity Asset)のボーン配置・メッシュ割り・重み割り当てを（ほぼ）自動的に行う

![example](https://github.com/bashiiko/Automatically-moving-illastrations-by-Anima2D/blob/media/demo.gif)

## Requirement
- Unity 2019.1.8f1
- Anima2D
- Matlab（画像をwebカメラを使ってキャプチャする場合）

## Example
1.  /Assets/ImageImportWatcher.csに，/Assets/ol_bone_gen.pyへの絶対パスとpythonのPATHを指定する
```csharp
var pythonScriptPath = @"C:\Users\USERNAME\Desktop\Automatically-moving-illastrations-by-Anima2D\Assets\ol_bone_gen.py";
var process = new Process()
{
    StartInfo = new ProcessStartInfo(@"C:\Users\USERNAME\AppData\Local\Programs\Python\Python37\python")
    {
        UseShellExecute = false,
        RedirectStandardOutput = true,
        Arguments = pythonScriptPath + " " + FileName,
    },
};
```
2. ImageCapture.mを実行して画像をキャプチャ
3. ol_bone_gen.pyが自動的に実行→透過画像ファイルと，算出された頂点座標等が格納されたpoints.csvが作成される
4. ./Assets/Sprites/target#.pngを選択し，CTRL+G→SpriteMeshが自動生成され，キャラクターがUnityのゲーム画面上に登場

## Script
- **DirectedExplanation.cs**\
  ol_bone_gen.pyでの処理過程を表示するアニメーションの，説明文を管理
- **DirectedImages.cs**\
  ol_bone_gen.pyでの処理過程を表示するアニメーションの，画像を管理
- **DirectedMoving.cs**\
  Unityゲーム画面上でのキャラクターの移動を管理
- **DirectedRotating.cs**\
  キャラクターのboneの回転を管理
- **FileImportWacher.cs**\
  SpriteMeshの変更・追加を監視
- **flag_animetion.cs**\
  Unity画面上での旗のアニメーションを管理
- **ImageCapture.m**\
  Webカメラを使って画像をキャプチャするアプリケーション
- **ImageImportWatcher.cs**\
  外部から追加されたpng画像の変更・追加を監視
- **ol_bone_gen.py**\
  画像の関節・骨格（もどき）を推定
- **SetAnima2D_ver2.cs**\
  ボーン配置・メッシュ割り・重み割り当て
