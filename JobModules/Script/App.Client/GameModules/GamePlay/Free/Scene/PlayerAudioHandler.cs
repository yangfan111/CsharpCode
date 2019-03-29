using Assets.Sources.Free;
using System.Collections.Generic;
using Free.framework;
using Core.Free;
using App.Shared.EntityFactory;
using UnityEngine;
using App.Shared;
using Core.EntityComponent;
using Assets.Sources.Free.UI;
using Core.IFactory;

namespace App.Client.GameModules.GamePlay.Free.Scene
{

    public partial class PlayerAudioHandler : ISimpleMesssageHandler
    {
        private ISoundEntityFactory _soundEntityFactory;

        public PlayerAudioHandler(ISoundEntityFactory soundEntityFactory)
        {
            _soundEntityFactory = soundEntityFactory;
        }

        public bool CanHandle(int key)
        {
            return FreeMessageConstant.PlaySound == key;
        }
        public void Handle(SimpleProto data)
        {
            //SimpleProtoWraper dataWrapper = new SimpleProtoWraper(data);
            //int defaultKey = dataWrapper.In_Val(0);
            //var handler = AudioSimpleProtoProcess.CreateSimpleHandler(dataWrapper);
            //handler.Process();

            //int entityId = 0;

            //if (data.Ins.Count > 1)
            //{

            //    //声源对象实例id
            //    entityId = data.Ins[1];
            //}

            //bool stop = data.Bs[0];
            //bool loop = data.Bs[1];
            //bool hasP = data.Bs[2];

            //string soundKey = data.Ss[0];

            //UnityEngine.Vector3 p = new UnityEngine.Vector3();
            //if (data.Fs.Count > 2)
            //{
            //    p.Set(data.Fs[0], data.Fs[1], data.Fs[2]);
            //}

            //if (stop)
            //{
            //    if (!string.IsNullOrEmpty(soundKey) && cache.ContainsKey(soundKey))
            //    {
            //        if (cache[soundKey] != null && cache[soundKey].isEnabled)
            //        {
            //            cache[soundKey].isFlagDestroy = true;
            //        }
            //        cache.Remove(soundKey);
            //    }
            //}
            //else
            //{
            //    if (hasP)
            //    {
            //        SoundEntity entity = _soundEntityFactory.CreateSelfOnlySound(key, p, loop) as SoundEntity;
            //        if (!string.IsNullOrEmpty(soundKey))
            //        {
            //            if (cache.ContainsKey(soundKey))
            //            {
            //                if (cache[soundKey] != null && cache[soundKey].isEnabled)
            //                {
            //                    cache[soundKey].isFlagDestroy = true;
            //                }
            //                cache.Remove(soundKey);
            //            }
            //            cache.Add(soundKey, entity);
            //        }

            //    }
            //    else
            //    {
            //        if (entityId > 0)
            //        {
            //            FreeMoveEntity move = SingletonManager.Get<FreeUiManager>().Contexts1.freeMove.GetEntityWithEntityKey(new EntityKey(entityId, (int)EEntityType.FreeMove));
            //            if (move != null)
            //            {
            //                SoundEntity entity = _soundEntityFactory.CreateSelfOnlyMoveSound(move.position.Value, new EntityKey(entityId, (int)EEntityType.FreeMove), key, loop) as SoundEntity;
            //                if (cache.ContainsKey(soundKey))
            //                {
            //                    if (cache[soundKey] != null && cache[soundKey].isEnabled)
            //                    {
            //                        cache[soundKey].isFlagDestroy = true;
            //                    }
            //                    cache.Remove(soundKey);
            //                }
            //                cache.Add(soundKey, entity);
            //            }
            //        }
            //        else
            //        {
            //            SoundEntity entity = _soundEntityFactory.CreateSelfOnlySound(key, loop) as SoundEntity;
            //            if (!string.IsNullOrEmpty(soundKey))
            //            {
            //                if (cache.ContainsKey(soundKey))
            //                {
            //                    if (cache[soundKey] != null && cache[soundKey].isEnabled)
            //                    {
            //                        cache[soundKey].isFlagDestroy = true;
            //                    }
            //                    cache.Remove(soundKey);
            //                }
            //                cache.Add(soundKey, entity);
            //            }
            //        }

            //    }
        }


    }
    //public void Handle(SimpleProtoWraper data)
    //{
    //    int key = data.Ins[0];
    //    int entityId = 0;
    //    if (data.Ins.Count > 1)
    //    {
    //        entityId = data.Ins[1];
    //    }

    //    bool stop = data.Bs[0];
    //    bool loop = data.Bs[1];
    //    bool hasP = data.Bs[2];

    //    string soundKey = data.Ss[0];

    //    UnityEngine.Vector3 p = new UnityEngine.Vector3();
    //    if (data.Fs.Count > 2)
    //    {
    //        p.Set(data.Fs[0], data.Fs[1], data.Fs[2]);
    //    }

    //    if (stop)
    //    {
    //        if (!string.IsNullOrEmpty(soundKey) && cache.ContainsKey(soundKey))
    //        {
    //            if (cache[soundKey] != null && cache[soundKey].isEnabled)
    //            {
    //                cache[soundKey].isFlagDestroy = true;
    //            }
    //            cache.Remove(soundKey);
    //        }
    //    }
    //    else
    //    {
    //        if (hasP)
    //        {
    //            SoundEntity entity = _soundEntityFactory.CreateSelfOnlySound(key, p, loop) as SoundEntity;
    //            if (!string.IsNullOrEmpty(soundKey))
    //            {
    //                if (cache.ContainsKey(soundKey))
    //                {
    //                    if (cache[soundKey] != null && cache[soundKey].isEnabled)
    //                    {
    //                        cache[soundKey].isFlagDestroy = true;
    //                    }
    //                    cache.Remove(soundKey);
    //                }
    //                cache.Add(soundKey, entity);
    //            }

    //        }
    //        else
    //        {
    //            if (entityId > 0)
    //            {
    //                FreeMoveEntity move = SingletonManager.Get<FreeUiManager>().Contexts1.freeMove.GetEntityWithEntityKey(new EntityKey(entityId, (int)EEntityType.FreeMove));
    //                if (move != null)
    //                {
    //                    SoundEntity entity = _soundEntityFactory.CreateSelfOnlyMoveSound(move.position.Value, new EntityKey(entityId, (int)EEntityType.FreeMove), key, loop) as SoundEntity;
    //                    if (cache.ContainsKey(soundKey))
    //                    {
    //                        if (cache[soundKey] != null && cache[soundKey].isEnabled)
    //                        {
    //                            cache[soundKey].isFlagDestroy = true;
    //                        }
    //                        cache.Remove(soundKey);
    //                    }
    //                    cache.Add(soundKey, entity);
    //                }
    //            }
    //            else
    //            {
    //                SoundEntity entity = _soundEntityFactory.CreateSelfOnlySound(key, loop) as SoundEntity;
    //                if (!string.IsNullOrEmpty(soundKey))
    //                {
    //                    if (cache.ContainsKey(soundKey))
    //                    {
    //                        if(cache[soundKey] != null && cache[soundKey].isEnabled)
    //                        {
    //                            cache[soundKey].isFlagDestroy = true;
    //                        }
    //                        cache.Remove(soundKey);
    //                    }
    //                    cache.Add(soundKey, entity);
    //                }
    //            }

    //        }
    //    }
    //}

}
