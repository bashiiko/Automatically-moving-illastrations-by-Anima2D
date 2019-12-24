/*
 * 画像の読み込み＆ボーン配置＆weight割り
 * 参考：http://techblog.sega.jp/entry/2018/05/25/100000
 */


using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;

public class ImportAnima2D : EditorWindow
{
    private static TextAsset csvFile; // CSVファイル
    private static List<string[]> csvDatas = new List<string[]>(); // CSVの中身を入れるリスト
    private static int height = 0; // CSVの行数
 
    // Start is called before the first frame update

    [MenuItem("Window/Anima2D/Import Anima2D")]
    public static void Open()
    {
        // 座標の読み込み
        TextAsset csvFile = Resources.Load("CSV/points") as TextAsset;
        StringReader reader = new StringReader(csvFile.text);

        while (reader.Peek() > -1)
        {
            string line = reader.ReadLine();
            csvDatas.Add(line.Split(',')); // リストに入れる
            height++; // 行数加算
        }

        Sprite spriteImg = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/target.png");
        Anima2D.SpriteMesh spriteMesh = AssetDatabase.LoadAssetAtPath<Anima2D.SpriteMesh>("Assets/Sprites/target.asset");

        Texture2D textureImg = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Sprites/target.png");

        GameObject meshRoot = GameObject.Find("mesh");
        GameObject boneRoot = GameObject.Find("bone");
        
        int w = textureImg.width;
        int h = textureImg.height;
        
        // Mesh
        GameObject tmpMesh = new GameObject("target");
        var smeshInstance = tmpMesh.AddComponent<Anima2D.SpriteMeshInstance>();
        smeshInstance.sharedMaterial = UnityEditor.AssetDatabase.GetBuiltinExtraResource<Material>("Sprites-Default.mat");
        tmpMesh.AddComponent<MeshFilter>();
        tmpMesh.AddComponent<MeshRenderer>();
        
        tmpMesh.AddComponent<SkinnedMeshRenderer>();
        SkinnedMeshRenderer rend = tmpMesh.GetComponent<SkinnedMeshRenderer>();

        // Bone
        GameObject[] tmpBone = new GameObject[csvDatas[1].Length];
        List<Anima2D.Bone2D> cmpBone = new List<Anima2D.Bone2D>();

        boneRoot.GetComponent<Transform>().position = new Vector3(float.Parse(csvDatas[2][0]), float.Parse(csvDatas[2][1]), 0);
        for (int i = 0; i < csvDatas[1].Length; i++)
        {
            tmpBone[i] = new GameObject("bone_" + i);
            cmpBone.Add(tmpBone[i].AddComponent<Anima2D.Bone2D>());
            tmpBone[i].AddComponent<DirectedMoving>();

            // csvのx軸とy軸の値が逆
            // openCVの座標がどっかで間違ってるっぽい
            tmpBone[i].transform.parent = boneRoot.transform;
            tmpBone[i].GetComponent<Transform>().localPosition = new Vector3(0, 0, 0);
            //float len = Mathf.Sqrt(Mathf.Pow(float.Parse(csvDatas[1][i])/w- float.Parse(csvDatas[2][1])/w,2)
            //    + Mathf.Pow((-1 * float.Parse(csvDatas[0][i]) + h - float.Parse(csvDatas[2][0]))/w,2));

            //tmpBone[i].GetComponent<Transform>().localScale= new Vector3(len, 1, 1);

            double arc = Math.Atan2(-1*double.Parse(csvDatas[0][i]) + h - double.Parse(csvDatas[2][0]) , (double.Parse(csvDatas[1][i]) - double.Parse(csvDatas[2][1])));
            tmpBone[i].GetComponent<Transform>().eulerAngles = new Vector3(0, 0, (float)(arc * 180.0 / Math.PI));
        }
        
        // Sprite Mesh Instance の設定
        smeshInstance.spriteMesh = spriteMesh as Anima2D.SpriteMesh; //  Meshの割り当て
        smeshInstance.bones = cmpBone;

        // Position の設定
        meshRoot.GetComponent<Transform>().position = new Vector3(float.Parse(csvDatas[2][0]), float.Parse(csvDatas[2][1]), 0);

        tmpMesh.transform.parent = meshRoot.transform;
        tmpMesh.GetComponent<Transform>().localPosition = new Vector3(0, 0, 0);
         
        rend.sharedMesh = SpriteToMesh(spriteImg);

        // 以下いらないかも？weight自動割り当てされてる？
        Mesh mesh = rend.sharedMesh;

        Vector3[] vtx = mesh.vertices; // メッシュの頂点
        Transform[] boneArray = rend.bones; // 全てのボーン
        //BoneWeight[] bwArray = mesh.boneWeights; // 頂点数と同じだけ存在する。各頂点が、どのボーンとどの程度の重みでつながっているかの情報を持つ。
        //Matrix4x4[] bindArray = mesh.bindposes; // ボーン数と同じ数だけ存在する。ボーンの初期姿勢

        // boneIndex:0~3 (1頂点につき4つのみ）
        // weight:0~3（各boneIndexに対するweight．合計が１になるように割り振る）
        BoneWeight[] weights = new BoneWeight[vtx.Length];
        weights[0].boneIndex0 = 0;
        weights[0].weight0 = 1;
        weights[1].boneIndex0 = 0;
        weights[1].weight0 = 1;
        weights[2].boneIndex0 = 1;
        weights[2].weight0 = 1;
        weights[5].boneIndex0 = 1;
        weights[5].weight0 = 1;
        //mesh.boneWeights = weights;
        
    }

    private static Mesh SpriteToMesh(Sprite sprite)
    {
        var mesh = new Mesh();
        mesh.SetVertices(Array.ConvertAll(sprite.vertices, c => (Vector3)c).ToList());
        mesh.SetUVs(0, sprite.uv.ToList());
        mesh.SetTriangles(Array.ConvertAll(sprite.triangles, c => (int)c), 0);

        return mesh;
    }
}