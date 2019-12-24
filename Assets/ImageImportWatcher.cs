using UnityEngine;
using System.IO;
using System.Diagnostics;
using System;
using System.Collections.Generic;

[ExecuteInEditMode]
public class ImageImportWatcher : MonoBehaviour
{
    /// <summary>
    /// ファイル検出時実行処理
    /// </summary>
    [SerializeField, Tooltip("ファイル検出時実行処理")]

    /// ファイル変更イベント検出フラグ
    private bool p_ChangedFlg;
    // 画像ファイル名
    private string FileName;

    private void Start()
    {
        // 検出フラグOFF
        p_ChangedFlg = false;

        // ファイル監視初期化
        FileSystemWatcher_Init();
    }

    /// <summary>
    /// 定期実行
    /// </summary>
    private void Update()
    {
        if (p_ChangedFlg)
        {
            GameObject[] objs = GameObject.FindGameObjectsWithTag("Character");
            int count;
            if (objs.Length == 0)
            {
                count = 1;
            }
            else
            {
                count = objs.Length + 1;
            }

            // python実行      
            var pythonScriptPath = @"C:\Users\nk\Desktop\OpenLab\Assets\ol_bone_gen.py";
            var process = new Process()
            {
                StartInfo = new ProcessStartInfo(@"C:\Users\nk\AppData\Local\Programs\Python\Python37\python")
                {
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    Arguments = pythonScriptPath + " " + FileName,
                },
            };

            process.Start();
            StreamReader myStreamReader = process.StandardOutput;
            
            string points = myStreamReader.ReadToEnd(); 
            process.WaitForExit();
            process.Close();

            UnityEngine.Debug.Log(points);

            // 検出フラグをOFF
            p_ChangedFlg = false;
        }
        //FileSystemWatcher_Init();
    }

    /// <summary>
    /// ファイル監視インスタンス
    /// </summary>
    private FileSystemWatcher fileSystemWatcher = null;

    /// <summary>
    /// ファイル監視初期化
    /// </summary>
    private void FileSystemWatcher_Init()
    {
        // 監視用インスタンスの作成
        fileSystemWatcher = new FileSystemWatcher();

        // 監視ディレクトリの指定(本例はAssets/SteamingAssetsフォルダ)
        fileSystemWatcher.Path = "Assets/Sprites";

        // 最終アクセス日時、最終更新日時、ファイル名を監視
        fileSystemWatcher.NotifyFilter =
            (System.IO.NotifyFilters.LastAccess
            | System.IO.NotifyFilters.LastWrite
            | System.IO.NotifyFilters.FileName);

        // ".txt"の拡張子を持つファイルを監視
        // 全てのファイルを監視する場合は""を指定する
        // 複数の拡張子を監視する場合は"*.txt|*.jpg"のように指定する
        fileSystemWatcher.Filter = "*.png";

        // イベントハンドラの追加
        // Changed: ファイル変更、Created: ファイル作成、Deleted: ファイル削除、Renamed: ファイル名変更
        //fileSystemWatcher.Changed += new System.IO.FileSystemEventHandler(FileSystemWatcher_Changed);
        fileSystemWatcher.Created += new System.IO.FileSystemEventHandler(FileSystemWatcher_Changed);
        //fileSystemWatcher.Deleted += new System.IO.FileSystemEventHandler(FileSystemWatcher_Changed);


        // 監視を開始する
        fileSystemWatcher.EnableRaisingEvents = true;
    }

    /// <summary>
    /// ファイル変更イベントコールバック関数
    /// </summary>
    private void FileSystemWatcher_Changed(System.Object source, System.IO.FileSystemEventArgs e)
    {
        // ファイル変更イベント発生時の処理
        // 検出フラグONに変更する
        // UnityEventの実行はMainThreadで行う
        p_ChangedFlg = true;
        FileName = e.FullPath;
    }

}