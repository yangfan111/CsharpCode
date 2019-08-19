using App.Shared.GameModules.GamePlay.SimpleTest;
using App.Shared.Player;
using Assets.App.Client.GameModules;
using Assets.Sources.Free;
using Assets.Sources.Free.UI;
using Core.Free;
using Free.framework;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Utils.Singleton;

namespace App.Client.GameModules.GamePlay.Free.Player
{
    public enum EPositionHandler {
        EPosition_Init = 0,
        EPosition_Add = 1,
        EPosition_Remove = 2,
    }

    public class TestPositionHandler : ISimpleMesssageHandler
    {
        public bool CanHandle(int key)
        {
            return key == FreeMessageConstant.TestPosition;
        }

        private static void CreateGameObject(Vector3 position, float rotation, string timeStamp)
        {
            GameObject testObj = GameObject.CreatePrimitive(PrimitiveType.Cube);
            BoxCollider boxCollider = testObj.GetComponent<BoxCollider>();
            if (null != boxCollider)
            {
                boxCollider.enabled = false;
            }
            testObj.transform.position = new Vector3(position.x, position.y, position.z);
            testObj.transform.eulerAngles = new Vector3(0, rotation, 0);
            testObj.transform.localScale = new Vector3(0.5f, 2, 0.5f);

            ObjectTag objectTag = testObj.AddComponent<ObjectTag>();
            objectTag.tag = (int)EObjectTag.TestPosition;

            testObj.name = timeStamp;
        }

        private static string TimeStamp()
        {
            return ((long)((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000)).ToString();
        }

        private static void Clear()
        {
            ObjectTag[] objectTags = GameObject.FindObjectsOfType<ObjectTag>();
            for (int i = 0, maxi = (null == objectTags ? 0 : objectTags.Length); i < maxi; i++)
            {
                ObjectTag objectTag = objectTags[i];
                if (null == objectTag)
                    continue;

                GameObject.DestroyImmediate(objectTag.gameObject);
                objectTags[i] = null;
            }
        }

        public void Handle(SimpleProto data)
        {
            if (null != data)
            {
                int positionHandler = data.Ins[0];

                Contexts contexts = SingletonManager.Get<FreeUiManager>().Contexts1;

                if (positionHandler == (int)EPositionHandler.EPosition_Init)
                {
                    string type = data.Ss[0];
                    int mapId = contexts.session.commonSession.RoomInfo.MapId;

                    Clear();

                    TestPositionManager.IniTable(mapId, type);
                    List<TestPosition> testPositions = TestPositionManager.GetTestPositions();
                    for (int i = 0, maxi = (null == testPositions ? 0 : testPositions.Count); i < maxi; i++) {
                        TestPosition testPosition = testPositions[i];
                        Vector3 position = new Vector3(testPosition.x, testPosition.y, testPosition.z);
                        float rotation = testPosition.rotation;
                        string timeStamp = testPosition.time;
                        CreateGameObject(position, rotation, timeStamp);
                    }
                }
                else if (positionHandler == (int)EPositionHandler.EPosition_Add)
                {
                    Vector3 position = contexts.player.flagSelfEntity.position.Value;
                    GameObject rootGo = contexts.player.flagSelfEntity.RootGo();
                    string timeStamp = TimeStamp();
                    CreateGameObject(position, rootGo.transform.eulerAngles.y, timeStamp);
                    TestPositionManager.Add(position, rootGo.transform.eulerAngles.y, timeStamp);
                }
                else if (positionHandler == (int)EPositionHandler.EPosition_Remove)
                {
                    Vector3 position = contexts.player.flagSelfEntity.position.Value;
                    GameObject rootGo = contexts.player.flagSelfEntity.RootGo();

                    Bounds bounds = new Bounds(position, Vector3.one);

                    ObjectTag[] objectTags = GameObject.FindObjectsOfType<ObjectTag>();
                    for (int i = 0, maxi = (null == objectTags ? 0 : objectTags.Length); i < maxi; i++)
                    {
                        ObjectTag objectTag = objectTags[i];
                        if (null == objectTag)
                            continue;

                        if (bounds.Contains(objectTag.transform.position))
                        {
                            TestPositionManager.Delete(objectTag.gameObject.name);
                            GameObject.DestroyImmediate(objectTag.gameObject);
                            objectTags[i] = null;
                        }
                    }
                }
            }
        }
    }
}
