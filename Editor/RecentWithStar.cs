using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Collections.Generic;
using System.IO;
using System;
using Object = UnityEngine.Object;

public class RecentWithStar : EditorWindow
{
    private const uint HISTORY_LIMIT = 10;
    private const string PERSISTENT_PATH = ".RecentWithStar/bookmarks.json";

    private List<Object> _selectionHistory = new List<Object>();
    private List<Object> _starredItems = new List<Object>();
    private Vector2 _scrollPos;
    private ReorderableList _reorderableStarredList;

    [MenuItem("Window / Recent With Star")]
    public static void ShowWindow()
    {
        GetWindow<RecentWithStar>("Recent With Star");
    }

    private void OnEnable()
    {
        Selection.selectionChanged += OnSelectionChanged;
        LoadBookmarks();
        InitializeBookmarkList();
    }

    private void OnDisable()
    {
        Selection.selectionChanged -= OnSelectionChanged;
        SaveBookmarks();
    }

    private void OnSelectionChanged()
    {
        if (Selection.activeObject == null)
        {
            return;
        }

        if (!AssetDatabase.Contains(Selection.activeObject))
        {
            return;
        }

        if (_selectionHistory.Contains(Selection.activeObject))
        {
            // repaint to update indicator
            Repaint();
            return;
        }

        _selectionHistory.Insert(0, Selection.activeObject);
        while (_selectionHistory.Count > HISTORY_LIMIT)
        {
            _selectionHistory.RemoveAt(_selectionHistory.Count - 1);
        }

        Repaint();
    }

    private void OnGUI()
    {
        // Clean up missing assets
        _selectionHistory.RemoveAll(item => item == null);
        _starredItems.RemoveAll(item => item == null);

        // viewport begin
        _scrollPos = EditorGUILayout.BeginScrollView(_scrollPos);

        // drop zone
        Rect dropArea = GUILayoutUtility.GetRect(0.0f, 50.0f, GUILayout.ExpandWidth(true));
        GUIStyle centeredStyle = new GUIStyle(EditorStyles.helpBox) { alignment = TextAnchor.MiddleCenter };
        GUI.Box(dropArea, "Drop new items here", centeredStyle);
        HandleDragAndDrop(dropArea);
        EditorGUILayout.Space();

        // bookmark list
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Starred Items", EditorStyles.boldLabel);
        if (GUILayout.Button("Clear", GUILayout.Width(50)))
        {
            _starredItems.Clear();
        }

        EditorGUILayout.EndHorizontal();
        _reorderableStarredList.DoLayoutList();
        EditorGUILayout.Space();

        // history list
        EditorGUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Selection History", EditorStyles.boldLabel);
        if (GUILayout.Button("Clear", GUILayout.Width(50)))
        {
            _selectionHistory.Clear();
        }

        EditorGUILayout.EndHorizontal();

        foreach (Object item in _selectionHistory)
        {
            EditorGUILayout.BeginHorizontal();

            // Add arrow indicator for current selection
            if (item == Selection.activeObject)
            {
                GUILayout.Label("→", GUILayout.Width(20));
            }
            else
            {
                GUILayout.Label("", GUILayout.Width(20));
            }

            // object entry
            EditorGUILayout.ObjectField(item, typeof(Object), false, GUILayout.ExpandWidth(true));

            // bookmark button
            if (GUILayout.Button("★", GUILayout.Width(20)))
            {
                if (!_starredItems.Contains(item))
                {
                    _starredItems.Add(item);
                }
            }

            EditorGUILayout.EndHorizontal();
        }

        EditorGUILayout.Space();

        // viewport end
        EditorGUILayout.EndScrollView();
    }

    private void InitializeBookmarkList()
    {
        _reorderableStarredList = new ReorderableList(
            _starredItems,
            typeof(Object),
            true,
            false,
            false,
            true
        )
        {
            multiSelect = true,
            drawElementCallback = HandleDrawBookmarkElement
        };
    }

    private void HandleDrawBookmarkElement(Rect rect, int index, bool isActive, bool isFocused)
    {
        Object item = _starredItems[index];
        rect.y += 2;
        rect.height = EditorGUIUtility.singleLineHeight;
        Rect fieldRect = new Rect(rect.x, rect.y, rect.width, rect.height);
        EditorGUI.ObjectField(fieldRect, item, typeof(Object), false);
    }

    private void HandleDragAndDrop(Rect dropArea)
    {
        Event evt = Event.current;
        switch (evt.type)
        {
            case EventType.DragUpdated:
            case EventType.DragPerform:
                if (!dropArea.Contains(evt.mousePosition))
                {
                    return;
                }

                DragAndDrop.visualMode = DragAndDropVisualMode.Copy;

                if (evt.type == EventType.DragPerform)
                {
                    DragAndDrop.AcceptDrag();

                    foreach (Object draggedObject in DragAndDrop.objectReferences)
                    {
                        if (!AssetDatabase.Contains(draggedObject))
                        {
                            continue;
                        }

                        if (_selectionHistory.Contains(draggedObject))
                        {
                            _selectionHistory.Remove(draggedObject);
                        }

                        _selectionHistory.Insert(0, draggedObject);
                        if (!_starredItems.Contains(draggedObject))
                        {
                            _starredItems.Add(draggedObject);
                        }
                    }
                }

                Event.current.Use();
                break;
        }
    }

    private void SaveBookmarks()
    {
        List<string> paths = new List<string>();
        foreach (Object item in _starredItems)
        {
            if (item != null)
            {
                string path = AssetDatabase.GetAssetPath(item);
                if (!string.IsNullOrEmpty(path))
                {
                    paths.Add(path);
                }
            }
        }

        string json = JsonUtility.ToJson(new BookmarkData { paths = paths });
        string dirName = Path.GetDirectoryName(PERSISTENT_PATH);
        if (!string.IsNullOrEmpty(dirName) && !Directory.Exists(dirName))
        {
            Directory.CreateDirectory(dirName);
        }

        File.WriteAllText(PERSISTENT_PATH, json);
    }

    private void LoadBookmarks()
    {
        if (File.Exists(PERSISTENT_PATH))
        {
            string json = File.ReadAllText(PERSISTENT_PATH);
            BookmarkData data = JsonUtility.FromJson<BookmarkData>(json);
            _starredItems.Clear();
            foreach (string path in data.paths)
            {
                Object item = AssetDatabase.LoadAssetAtPath<Object>(path);
                if (item != null)
                {
                    _starredItems.Add(item);
                }
            }
        }
    }

    [Serializable]
    private class BookmarkData
    {
        public List<string> paths;
    }
}