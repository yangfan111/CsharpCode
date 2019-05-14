using System;
using System.Collections.Generic;
using Core.Utils;
using Entitas;
using UnityEngine;
using Utils.Singleton;

namespace App.Shared.SceneTriggerObject
{
    public class RawMapObjectIdRecorder
    {
        private Dictionary<GameObject, int>[] _mapObjIdRecorder = new Dictionary<GameObject, int>[(int)ETriggerObjectType.MaxCount];
        private Dictionary<GameObject,int> _typeRecord;

        public RawMapObjectIdRecorder()
        {
            for(int i=0;i<(int)ETriggerObjectType.MaxCount;i++)
            {
                _mapObjIdRecorder[i] = new Dictionary<GameObject, int>();
            }
            _typeRecord = new Dictionary<GameObject, int>();
        }
        
        public void RecordId(GameObject obj, int type, int id)
        {
            if(!_mapObjIdRecorder[type].ContainsKey(obj))
                _mapObjIdRecorder[type].Add(obj, id);
            
            if ( !_typeRecord.ContainsKey(obj))
                _typeRecord.Add(obj, type);
        }

        public int GetType(GameObject obj)
        {
            if ( _typeRecord.ContainsKey(obj))
                return _typeRecord[obj];
            return -1;
        }

        public GameObject GetObj(int id)
        {
            foreach (var recorder in _mapObjIdRecorder)
            {
                if (recorder.ContainsValue(id))
                {
                    foreach (var obj in recorder.Keys)
                    {
                        if (recorder[obj] == id) return obj;
                    }
                }
            }
            return null;
        }
        
        public int GetId(GameObject obj)
        {
            int result = 0;
            for (int i = 0; i < (int) ETriggerObjectType.MaxCount; i++)
            {
                if (_mapObjIdRecorder[i].TryGetValue(obj, out result))
                    return result;
            }
            return Int32.MinValue;
        }
    }
   
    
    public class MapObjectRecorder
    {
        private MapObjectInternalRecorder[] _recorders = new MapObjectInternalRecorder[(int)ETriggerObjectType.MaxCount];

        public MapObjectRecorder()
        {
            _recorders[(int)ETriggerObjectType.Door] = new MapObjectInternalRecorder();
            _recorders[(int)ETriggerObjectType.GlassyObject] = new MapObjectInternalRecorder();
            _recorders[(int)ETriggerObjectType.DestructibleObject] = new MapObjectInternalRecorder();
        }

        public MapObjectEntity Get(int gameObjId, int type)
        {
            return _recorders[type].Get(gameObjId);
        }
        
        public void Add(int gameObjId, int type, MapObjectEntity mapObj)
        {
            _recorders[type].Add(gameObjId, mapObj);
        }

        public void Delete(int gameObjId, int type)
        {
            _recorders[type].Delete(gameObjId);
        }
    }

    internal class MapObjectInternalRecorder:IDisposable
    {
        private Dictionary<int, MapObjectEntity> _mapObjDict;

        public MapObjectInternalRecorder()
        {
            _mapObjDict = new Dictionary<int, MapObjectEntity>();
        }

        public MapObjectEntity Get(int id)
        {
            if (_mapObjDict.ContainsKey(id))
                return _mapObjDict[id];
            return null;
        }
        
        public void Add(int id, MapObjectEntity obj)
        {
            if(_mapObjDict.ContainsKey(id)) return;
            _mapObjDict.Add(id, obj);
        }

        public void Delete(int id)
        {
            if (_mapObjDict.ContainsKey(id))
                _mapObjDict.Remove(id);
        }

        public void Dispose()
        {
            _mapObjDict = new Dictionary<int, MapObjectEntity>();
        }
    }
}