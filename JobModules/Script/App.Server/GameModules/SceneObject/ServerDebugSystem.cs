using System.Runtime.InteropServices;
using App.Protobuf;
using Assets.XmlConfig;
using Core.GameModule.Interface;
using Core.Utils;
using UnityEngine;

namespace App.Server.GameModules.SceneObject
{
    public class ServerDebugSystem:IGamePlaySystem
    {
        private Contexts _contexts;
        private SceneObjectEntity entity;

        public ServerDebugSystem(Contexts contexts)
        {
            _contexts = contexts;
            entity = null;
        }


        public void OnGamePlay()
        {
            
            if (entity == null || entity.isFlagDestroy)
            {
                entity = (SceneObjectEntity) _contexts.session.entityFactoryObject.SceneObjectEntityFactory
                    .CreateSimpleEquipmentEntity(
                        ECategory.GameItem,
                        104,
                        1,
                        RaycastUtility.GetLegalPosition(new UnityEngine.Vector3(0, 100, 0)));
            }
            else
            {
                
                   var time=  _contexts.session.currentTimeObject.CurrentTime;

                var p = time % 6000 / 6000f;

                entity.position.Value = new UnityEngine.Vector3(Mathf.Sin(360*Mathf.Deg2Rad * p)*5, 2, Mathf.Cos(360*Mathf.Deg2Rad * p)*5);
                entity.ReplaceFlagImmutability(_contexts.session.currentTimeObject.CurrentTime);
            }
            
        }
    }
}