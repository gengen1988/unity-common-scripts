using System.Text;
using Weaver;

// public class BuffPrefabList : AssetList<Buff>
// {
//     [AssetReference] public static readonly BuffPrefabList Instance;
//
// #if UNITY_EDITOR
//     [AssetReference, ProceduralAsset] public static readonly UnityEditor.MonoScript Index;
// #endif
//
//     private static void GenerateIndex(StringBuilder text)
//     {
//         var builder = new AssetListIndexBuilder<Buff>(Instance);
//         builder.IndexTypeName = "BuffPrefabIndex";
//         builder.Build(text);
//     }
// }