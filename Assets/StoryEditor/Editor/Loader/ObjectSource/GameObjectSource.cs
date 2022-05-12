// **********************************************************************
// Author Name :zsm
// Create Time :2022-05-10
// Description :
// **********************************************************************
using UnityEngine;

namespace StoryEditor
{
    public class GameObjectSource : AssetSource<GameObject>
    {
        public GameObjectSource(string path) : base(path) { }
        public override void StartLoad()
        {
            StartLoadGameOjbecct();
        }
    }
}
