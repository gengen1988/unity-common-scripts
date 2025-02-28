using System.Linq;
using System.Text;
using UnityEngine;
using Weaver;

public class AssetListIndexBuilder<T> where T : Object
{
    public string IndexTypeName;
    public string AcquirePattern;

    private readonly IAssetList<T> AssetList;

    public AssetListIndexBuilder(IAssetList<T> assetList)
    {
        AssetList = assetList;

        var assetListTypeName = assetList.GetType().Name;
        AcquirePattern = $"{assetListTypeName}.Instance[{{0}}]";
        IndexTypeName = $"{assetListTypeName}Index";
    }

    public void Build(StringBuilder text)
    {
        // collect names
        var assetTypeName = typeof(T).Name;

        // generate parts
        var shortHandBuilder = new StringBuilder();
        var lutBuilder = new StringBuilder();
        foreach (var (prefab, index) in AssetList.Select((prefab, index) => (prefab, index)))
        {
            var key = prefab.name.ToUpperInvariant();
            var acquireByConstant = string.Format(AcquirePattern, index);

            lutBuilder.AppendLine($"            \"{key}\" => {index},");
            shortHandBuilder.AppendLine(
                $"    public static {assetTypeName} {key} => {acquireByConstant};"
            );
        }

        // generate whole script
        text.AppendLine($@"
using UnityEngine;

public class {IndexTypeName}
{{"
        );
        text.Append(shortHandBuilder);

        var acquireByVariable = string.Format(AcquirePattern, "index");
        text.AppendLine($@"
    public static {assetTypeName} Find(string str)
    {{
        var matchingKey = str.ToUpperInvariant();
        var index = matchingKey switch
        {{"
        );
        text.Append(lutBuilder);
        text.Append($@"
            _ => -1,
        }};
        return {acquireByVariable};
    }}
}}"
        );
    }
}