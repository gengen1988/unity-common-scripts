using System;
using System.IO;
using System.Linq;
using UnityEditor;

public static class PathHelper
{
    /// <summary>
    /// 创建目录。自动建立子目录
    /// </summary>
    /// <param name="path">文件夹路径</param>
    /// <param name="allowAssets">是否去掉路径开头的 assets</param>
    public static void Mkdirp(string path, bool allowAssets = false)
    {
        char[] separator = {Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar};
        var segments = path.Split(separator, StringSplitOptions.RemoveEmptyEntries);
        var parent = "Assets";

        if (!allowAssets)
        {
            if (segments.Length > 0 && segments[0] == "Assets")
            {
                segments = segments.Skip(1).ToArray();
            }
        }

        while (segments.Length > 0)
        {
            var current = segments.First();
            if (!AssetDatabase.IsValidFolder(Path.Combine(parent, current)))
            {
                AssetDatabase.CreateFolder(parent, current);
            }

            parent = Path.Combine(parent, current);
            segments = segments.Skip(1).ToArray();
        }
    }
}