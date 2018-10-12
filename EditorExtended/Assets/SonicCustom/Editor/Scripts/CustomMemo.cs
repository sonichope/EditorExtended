using UnityEngine;
using UnityEditor;
using System;
using System.IO;
using System.Collections.Generic;

public enum Occupation
{
    Planner,
    Programmer,
    Designer
}

public class CustomMemo : EditorWindow
{
    private static CustomMemo instance;
    private static bool isNetwork;
    private static Occupation occupation;
    private static List<TextAsset> selectMemoList = new List<TextAsset>();
    private string memoString;
    private string editString;
    private TextAsset editText;
    private int fontSize;

    [MenuItem("SonicCustom/Memo")]
    public static void ShowWindow()
    {
        if (instance != null)
        {
            isNetwork = Connect();
            GetFiles(occupation);
            instance.Show();
            return;
        }
        occupation = Occupation.Planner;
        GetFiles(occupation);
        isNetwork = Connect();
        instance = GetWindow<CustomMemo>();
        instance.minSize = new Vector2(600, 300);

        //gitの初期化
        GitCommand.Init();
    }

    public static bool Connect()
    {
        //インターネットに接続されているか確認する
        string host = "http://www.yahoo.com";

        System.Net.HttpWebRequest webreq = null;
        System.Net.HttpWebResponse webres = null;
        try
        {
            //HttpWebRequestの作成
            webreq = (System.Net.HttpWebRequest)System.Net.WebRequest.Create(host);
            //メソッドをHEADにする
            webreq.Method = "HEAD";
            //受信する
            webres = (System.Net.HttpWebResponse)webreq.GetResponse();
            return true;
        }
        catch
        {
            return false;
        }
        finally
        {
            if (webres != null)
                webres.Close();
        }
    }


    private void OnGUI()
    {
        GUIStyle guiStyle = new GUIStyle();
        guiStyle.normal.textColor = Color.white;

        //背景描画
        Vector2 size = GetWindow<CustomMemo>().position.size;
        Rect rect = new Rect(new Vector2(0, 0), size);
        EditorGUI.DrawRect(rect, new Color(0.3f,0.3f,0.3f));

        //フォルダー表示
        rect.position = new Vector2(10, 10);
        rect.size = new Vector2(100, 20);
        EditorGUI.LabelField(rect, "ファイル一覧", guiStyle);
        rect.position = new Vector2(100, 10);
        rect.size = new Vector2(20, 20);
        EditorGUI.LabelField(rect, "役職:", guiStyle);
        rect.position = new Vector2(130, 10);
        rect.size = new Vector2(80, 20);
        EditorGUI.BeginChangeCheck();
        occupation = (Occupation)EditorGUI.EnumPopup(rect, occupation);
        if (EditorGUI.EndChangeCheck())
        {
            GetFiles(occupation);
        }
        rect.position = new Vector2(10, 30);
        rect.size = new Vector2(200, size.y - (20 + rect.position.y));
        EditorGUI.DrawRect(rect, Color.black);
        rect.position = rect.position + new Vector2(1, 1);
        rect.size = new Vector2(rect.size.x - 2, rect.size.y - 2);
        EditorGUI.DrawRect(rect, new Color(0.3f, 0.3f, 0.3f));

        for(int i = 0; i < selectMemoList.Count; i++)
        {
            rect = ChangeRect(10, 40 + (20 * i), rect.size.x, 20);
            if (GUI.Button(rect, "・" + selectMemoList[i].name))
            {
                editText = selectMemoList[i];
                memoString = editText.text;
                editString = memoString;
            }
        }

        //エディットファイル表示
        if(editText != null)
        {
            rect = ChangeRect(220, 10, 100, 20);
            EditorGUI.LabelField(rect, editText.name, guiStyle);
        }

        //フォントサイズ変更スライダー
        rect.position = new Vector2(340, 10);
        rect.size = new Vector2(200, 20);
        fontSize = EditorGUI.IntSlider(rect,fontSize,12,62);
        guiStyle.fontSize = fontSize;
        
        //テキストエリア表示
        rect = ChangeRect(210, 30, size.x - (10 + 210), size.y - (20 + 30));
        EditorGUI.DrawRect(rect, Color.black);
        rect = ChangeRect(rect.position + new Vector2(1, 1), new Vector2(rect.size.x - 2, rect.size.y - 2));
        EditorGUI.DrawRect(rect, new Color(0.3f, 0.3f, 0.3f));
        editString = EditorGUI.TextArea(rect, editString,guiStyle);

        //プッシュボタンまたは接続ボタン
        Vector2 position = new Vector2(size.x - 90,5);
        rect = ChangeRect(position, new Vector2(80,20));
        if (isNetwork == true && editText != null)
        {
            if (GUI.Button(rect, "プッシュ"))
            {
                isNetwork = Connect();
                if (isNetwork == false) { return; }
                Push();
            }
        }
        else if(editText != null)
        {
            if (GUI.Button(rect, "接続"))
            {
                isNetwork = Connect();
            }
        }

        //セーブボタン
        position = rect.position;
        position.x -= rect.size.x;
        rect = ChangeRect(position, rect.size);
        if (GUI.Button(rect, "セーブ"))
        {
            TextSave();
        }

        //削除
        position = rect.position;
        position.x -= rect.size.x;
        rect = ChangeRect(position, rect.size);
        if (GUI.Button(rect, "削除"))
        {
            TextDelete();
        }

        //作成ボタン
        position = rect.position;
        position.x -= rect.size.x;
        rect = ChangeRect(position, rect.size);
        if (GUI.Button(rect, "作成"))
        {
            TextCreate();
        }

    }

    /// <summary>
    /// 削除
    /// </summary>
    /// <returns></returns>
    private bool TextDelete()
    {
        string saveDirectory = "Assets/SonicCustom/Editor/Resources/TextMemo/" + occupation + "/";
        string fileName = editText.name;
        string deletePath = saveDirectory + fileName + ".txt";
        selectMemoList.Remove(editText);
        AssetDatabase.DeleteAsset(deletePath);
        return true;
    }

    /// <summary>
    /// セーブ
    /// </summary>
    /// <returns></returns>
    private bool TextSave()
    {
        string saveDirectory = Application.dataPath + "/SonicCustom/Editor/Resources/TextMemo/" + occupation + "/";
        string fileName = editText.name;
        string createPath = saveDirectory + fileName + ".txt";

        Debug.Log(createPath);

        using(StreamWriter streamWriter = new StreamWriter(createPath, false, System.Text.Encoding.Unicode))
        {
            streamWriter.Write(editString);
        }
        AssetDatabase.Refresh();
        Debug.Log("セーブ");

        return true;
    }

    private bool TextCreate()
    {
        string saveDirectory = Application.dataPath + "/SonicCustom/Editor/Resources/TextMemo/" + occupation + "/";
        string fileName = occupation.ToString() + selectMemoList.Count;
        string createPath = saveDirectory + fileName + ".txt";
        Debug.Log(createPath);
        using (StreamWriter fileStream = File.CreateText(createPath))
        {
            if(fileStream != null)
            {
                fileStream.WriteLine(fileName);
            }
        }
        AssetDatabase.Refresh();
        TextAsset textAsset = Resources.Load<TextAsset>("TextMemo/" + occupation.ToString() + "/" + fileName);
        if(textAsset == null)
        {
            return false;
        }
        selectMemoList.Add(textAsset);
        return true;
    }

    private bool Push()
    {
        //git初期化確認
        if(GitCommand.IsInit() == false)
        {
            GitCommand.Init();
        }

        //選択されているメモファイルとメタファイルをインデックスに追加
        string memoFileDirectoryPath = Environment.CurrentDirectory + "\\Assets\\SonicCustom\\Editor\\Resources\\TextMemo\\" + occupation + "\\";
        string memofileName = editText.name;
        string extension = ".txt";
        string selectMemoFilePath = memoFileDirectoryPath + memofileName + extension;
        GitCommand.Add(selectMemoFilePath);
        extension += ".meta";
        selectMemoFilePath = memoFileDirectoryPath + memofileName + extension;
        GitCommand.Add(selectMemoFilePath);

        //インデックスに追加されたファイルをコミット
        string commitMessage = "[Update] " + memofileName + ".txtの更新";
        GitCommand.Commit(commitMessage);

        //現在のブランチにプッシュ
        GitCommand.Push();

        return false;
    }

    private Rect ChangeRect(Vector2 position, Vector2 size)
    {
        return new Rect(position, size);
    }

    private Rect ChangeRect(float x, float y, float width, float height)
    {
        return new Rect(x,y,width,height);
    }

    private static void GetFiles(Occupation occupation)
    {
        switch (occupation)
        {
            case Occupation.Planner:
                TextAsset[] datasPL = Resources.LoadAll<TextAsset>("TextMemo/Planner");
                selectMemoList.Clear();
                selectMemoList.AddRange(datasPL);
                return;
            case Occupation.Programmer:
                TextAsset[] datasPG = Resources.LoadAll<TextAsset>("TextMemo/Programmer");
                selectMemoList.Clear();
                selectMemoList.AddRange(datasPG);
                return;
            case Occupation.Designer:
                TextAsset[] datasDE = Resources.LoadAll<TextAsset>("TextMemo/Designer");
                selectMemoList.Clear();
                selectMemoList.AddRange(datasDE);
                return;
        }
    }
}
