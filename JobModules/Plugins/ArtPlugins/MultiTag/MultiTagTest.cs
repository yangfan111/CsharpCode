using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
namespace ArtPlugins
{
    [DisallowMultipleComponent]
    public class MultiTagTest : MonoBehaviour
    {
        private void OnGUI()
        {
            if (GUILayout.Button("test tag")) {
                Stopwatch sd = new Stopwatch();
                sd.Start();
                var items = MultiTag.getItems((int)MultiTag.TagEnum.OutProps);

                sd.Stop();
                print("init time:" + sd.ElapsedMilliseconds);
                foreach (var item in items)
                {
                    item.gameObject.SetActive(false);
                    //  print(item.name);
                }

            }

            if (GUILayout.Button("test multiTag")) {
                Stopwatch sd = new Stopwatch();
                sd.Start();
                var items=MultiTag.getItems((int)MultiTag.TagEnum.Wall);
          
                sd.Stop();
                print("init time:" + sd.ElapsedMilliseconds);
                foreach (var item in items)
                {
                    item.gameObject.SetActive(false);
                  //  print(item.name);
                }
            }
        }
    }
}