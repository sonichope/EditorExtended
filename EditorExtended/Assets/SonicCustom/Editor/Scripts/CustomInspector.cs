using UnityEngine;
using UnityEditor;

public class CustomInspector : EditorWindow
{

    public GameObject SelectObject { get; private set; }

    [MenuItem("SonicCustom/Inspector")]
    public static void ShowWindow()
    {
        GetWindow(typeof(CustomInspector));
    }

    private void OnGUI()
    {
        if(SelectObject == null) { return; }

        SelectObject.name = EditorGUILayout.TextField("名前", SelectObject.name);
        SelectObject.tag = EditorGUILayout.TagField("タグ", SelectObject.tag);
    }

    private void OnHierarchyChange()
    {
        SelectObject = GameObjectSlection();
    }

    /// <summary>
    /// Hierarchyに選択されているオブジェクトを取得する
    /// </summary>
    private GameObject GameObjectSlection()
    {
        if (Selection.gameObjects.Length == 0) { return null; }
        return Selection.gameObjects[0];
    }
}
