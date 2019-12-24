using UnityEngine;
using System.IO;

[ExecuteInEditMode]
public class FileImportWatcher : MonoBehaviour
{
    /// <summary>
    /// ファイル検出時実行処理
    /// </summary>
    [SerializeField, Tooltip("ファイル検出時実行処理")]
    
    /// ファイル変更イベント検出フラグ
    private bool p_ChangedFlg;

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
            // characterプレハブをGameObject型で取得
            GameObject obj = (GameObject)Resources.Load("character");
            GameObject[] objs = GameObject.FindGameObjectsWithTag("Character");
            int count;
            if (objs.Length == 0)
            {
                count = 1;
            } else
            {
                count = objs.Length + 1;
            }
            // characterプレハブを元に、インスタンスを生成
            var ins_obj = Instantiate(obj, new Vector3(0.0f, 0.0f, 0.0f), Quaternion.identity);
            ins_obj.name = "character" + count.ToString();
            ins_obj.SetActive(true);
            ins_obj.tag = "Character";

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
        fileSystemWatcher.Filter = "*.asset";

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
    }

    void OnChanged(System.Object source, System.IO.FileSystemEventArgs e)
    {
        // Specify what is done when a file is changed, created, or deleted.
        WatcherChangeTypes wct = e.ChangeType;
        Debug.Log( e.FullPath+wct.ToString());
    }
}