// **********************************************************************
// Author Name :zsm
// Create Time :2022-05-10
// Description :
// **********************************************************************
using UnityEngine;

namespace StoryEditor
{
    public class TextAssetSource : AssetSource<UnityEngine.TextAsset>
    {
        public TextAssetSource(string path) : base(path) { }
    }
}

