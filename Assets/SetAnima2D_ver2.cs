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

[ExecuteInEditMode]
public class SetAnima2D_ver2 : MonoBehaviour
{
    private  TextAsset csvFile; // CSVファイル
    private  List<string[]> csvDatas = new List<string[]>(); // CSVの中身を入れるリスト
    private  int height = 0; // CSVの行数
    
    // Start is called before the first frame update
    private void Awake()
    {
        // 座標の読み込み
        csvFile = Resources.Load("points") as TextAsset;
        StringReader reader = new StringReader(csvFile.text);

        // 最初の一行は距離のため読み捨てる
        reader.ReadLine();
        while (reader.Peek() > -1)
        {
            string line = reader.ReadLine();
            string[] list = line.Split(',');
            List<string> list_L = new List<string>(list);
            list_L.RemoveAt(0);
            list = list_L.ToArray();
            csvDatas.Add(list); // リストに入れる
            height++; // 行数加算
        }

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

        Sprite spriteImg = AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/target"+count.ToString() +".png");
        Anima2D.SpriteMesh spriteMesh = AssetDatabase.LoadAssetAtPath<Anima2D.SpriteMesh>("Assets/Sprites/target" + count.ToString() + ".asset");

        Texture2D textureImg = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/Sprites/target" + count.ToString() + ".png");

        int wid = textureImg.width;
        int hei = textureImg.height;

        // Mesh
        GameObject tmpMesh = GameObject.Find(transform.name);
        //tmpMesh.tag = "Character";
        var smeshInstance = tmpMesh.AddComponent<Anima2D.SpriteMeshInstance>();
        smeshInstance.sharedMaterial = AssetDatabase.GetBuiltinExtraResource<Material>("Sprites-Default.mat");


        tmpMesh.AddComponent<SkinnedMeshRenderer>();
        tmpMesh.AddComponent<DirectedMoving>();
        
        SkinnedMeshRenderer rend = tmpMesh.GetComponent<SkinnedMeshRenderer>();
        rend.sharedMesh = SpriteToMesh(spriteImg);

        tmpMesh.GetComponent<Transform>().position = new Vector3(UnityEngine.Random.Range(-9.0f, 9.0f), UnityEngine.Random.Range(-5.0f, 5.0f), 2.0f);

        // Bone
        GameObject[] tmpBone = new GameObject[csvDatas[1].Length];
        List<Anima2D.Bone2D> cmpBone = new List<Anima2D.Bone2D>();

        // boneの先端
        Vector3[] bone_vertex = new Vector3[csvDatas[1].Length];
        for (int i = 0; i < csvDatas[1].Length; i++)
        {
            tmpBone[i] = new GameObject("bone_" + i);
            cmpBone.Add(tmpBone[i].AddComponent<Anima2D.Bone2D>());
            tmpBone[i].AddComponent<DirectedRotating>();

            // csvのx軸とy軸の値が逆
            // openCVの座標がどっかで間違ってるっぽい
            tmpBone[i].transform.parent = tmpMesh.transform;
            //tmpBone[i].GetComponent<Transform>().localPosition = new Vector3(0, 0, 0);
            tmpBone[i].GetComponent<Transform>().localPosition = new Vector3((float.Parse(csvDatas[2][i])-wid/2)  / 100, ( - float.Parse(csvDatas[3][i])+hei/2)/ 100, 0);

            float len = Mathf.Sqrt(Mathf.Pow(float.Parse(csvDatas[0][i])- float.Parse(csvDatas[2][i]),2)
                + Mathf.Pow( float.Parse(csvDatas[1][i]) - float.Parse(csvDatas[3][i]),2))/100;
            tmpBone[i].GetComponent<Transform>().localScale= new Vector3(len, 1, 1);

            double arc = Math.Atan2(-1*double.Parse(csvDatas[1][i]) +  float.Parse(csvDatas[3][i]), double.Parse(csvDatas[0][i]) - double.Parse(csvDatas[2][i]));
            tmpBone[i].GetComponent<Transform>().eulerAngles = new Vector3(0, 0, (float)(arc * 180.0 / Math.PI));
            bone_vertex[i] = new Vector3((float)Math.Cos(arc) / 2, (float)Math.Sin(arc) * (float)Math.Sqrt(3) / 2, 0);
        }
        
        // Sprite Mesh Instance の設定
        smeshInstance.spriteMesh = spriteMesh as Anima2D.SpriteMesh; //  Meshの割り当て
        smeshInstance.bones = cmpBone;

    // https://answers.unity.com/questions/1190600/how-to-code-a-skinnedmesh.html
   //  https://qiita.com/romaroma/items/f175973f5c3ea1634729
        Mesh mesh = smeshInstance.spriteMesh.sharedMesh;

        // メッシュの頂点
        Vector3[] vtx = mesh.vertices;

        // 全てのボーン
        Transform[] boneArray = rend.bones; 
        // 頂点数と同じだけ存在する。各頂点が、どのボーンとどの程度の重みでつながっているかの情報を持つ。
         BoneWeight[] boneWeights = new BoneWeight[mesh.vertexCount];
        // ボーン数と同じ数だけ存在する。ボーンの初期姿勢
        Matrix4x4[] bindArray = new Matrix4x4[boneArray.Length];
        for (int i = 0; i < vtx.Length; i++)
        {
           var distance = new List<float>();
            
            for (int j = 0; j < boneArray.Length; j++)
            {
                distance.Add(Vector3.Distance(bone_vertex[j], vtx[i]));
            }

            BoneWeight b = new BoneWeight();
            int min_index = distance.IndexOf(distance.Min());
            b.boneIndex0 = min_index;
            b.weight0 = 1 / distance.Min();
            distance.RemoveAt(min_index);

            min_index = distance.IndexOf(distance.Min());
            b.boneIndex1 = min_index;
            b.weight1 = 1 / distance.Min();
            distance.RemoveAt(min_index);

            min_index = distance.IndexOf(distance.Min());
            b.boneIndex2 = min_index;
            b.weight2 = 1 / distance.Min();
            distance.RemoveAt(min_index);

            min_index = distance.IndexOf(distance.Min());
            b.boneIndex3 = min_index;
            b.weight3 = 1 / distance.Min();
            distance.RemoveAt(min_index);

            boneWeights[i] = b;
        }
        for (int cnt = 0; cnt < boneArray.Length; cnt++)
        {
            bindArray[cnt] = boneArray[cnt].worldToLocalMatrix * transform.localToWorldMatrix;
        }

        mesh.bindposes = bindArray;
        mesh.boneWeights = boneWeights;
        rend.bones = boneArray;
        rend.sharedMesh = mesh;
        // Debug.Log(rend.bones[mesh.boneWeights[33].boneIndex2].name); //bone_3
    }

    private Mesh SpriteToMesh(Sprite sprite)
    {
        var mesh = new Mesh();
        mesh.SetVertices(Array.ConvertAll(sprite.vertices, c => (Vector3)c).ToList());
        mesh.SetUVs(0, sprite.uv.ToList());
        mesh.SetTriangles(Array.ConvertAll(sprite.triangles, c => (int)c), 0);
        
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        return mesh;
    }
}