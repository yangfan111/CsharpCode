using System.Collections.Generic;
using Core.Utils;
using UnityEngine;
using Utils.CharacterState;
using XmlConfig;
using System.Collections;
using Utils.Compare;
using Shared.Scripts;

namespace Utils.Configuration
{
    public class FirstPersonRotationConfig
    {
        public RotationParam HorizontalParam;
        public RotationParam VerticalParam;

        public override string ToString()
        {
            return string.Format("HorizontalParam: {0}, VerticalParam: {1}", HorizontalParam, VerticalParam);
        }
    }

    public struct RotationParam
    {
        public float UpperLimit;
        public float RestoreVel;
        public float VelCoefficient;

        public override string ToString()
        {
            return string.Format("UpperLimit: {0}, RestoreVel: {1}, VelCoefficient: {2}", UpperLimit, RestoreVel,
                VelCoefficient);
        }
    }

    public class FirstPersonOffsetConfigManager : AbstractConfigManager<FirstPersonOffsetConfigManager>
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(FirstPersonOffsetConfigManager));

        private Dictionary<int, FirstPersonOffsetItem> _firstPersonOffsetConfigs =
            new Dictionary<int, FirstPersonOffsetItem>();

        private Dictionary<int, Core.Utils.Tuple<FirstPersonRotationConfig, FirstPersonRotationConfig>>
            _firstPersonRotationConfigs =
                new Dictionary<int, Tuple<FirstPersonRotationConfig, FirstPersonRotationConfig>>();

        private static Core.Utils.Tuple<FirstPersonRotationConfig, FirstPersonRotationConfig> _defaultRotationConfig;


        static FirstPersonOffsetConfigManager()
        {
            var config = new FirstPersonRotationConfig();
            config.HorizontalParam.UpperLimit = 4;
            config.HorizontalParam.RestoreVel = 20;
            config.HorizontalParam.VelCoefficient = 1;
            config.VerticalParam.UpperLimit = 2;
            config.VerticalParam.RestoreVel = 10;
            config.VerticalParam.VelCoefficient = 0.95f;

            _defaultRotationConfig = new Tuple<FirstPersonRotationConfig, FirstPersonRotationConfig>(config, config);
        }

        public FirstPersonOffsetConfigManager()
        {
        }

        public override void ParseConfig(string xml)
        {
            if (string.IsNullOrEmpty(xml))
            {
                Logger.Error("FirstPersonOffset config xml is empty !");
                return;
            }

            _firstPersonOffsetConfigs.Clear();
            _firstPersonRotationConfigs.Clear();
            var cfg = XmlConfigParser<FirstPersonOffsetConfig>.Load(xml);
            if (null == cfg)
            {
                Logger.ErrorFormat("FirstPersonOffset config is illegal content : {0}", xml);
                return;
            }

            foreach (var item in cfg.Items)
            {
                _firstPersonOffsetConfigs.Add(item.Id, item);
                _firstPersonRotationConfigs.Add(item.Id, CreateRotationTuple(item));
            }
        }

        private Core.Utils.Tuple<FirstPersonRotationConfig, FirstPersonRotationConfig> CreateRotationTuple(
            FirstPersonOffsetItem data)
        {
            var item1 = new FirstPersonRotationConfig();
            item1.HorizontalParam.UpperLimit = data.HorizontalUpperLimit;
            item1.HorizontalParam.RestoreVel = data.HorizontalRestoreVel;
            item1.HorizontalParam.VelCoefficient = data.HorizontalVelCoefficient;
            item1.VerticalParam.UpperLimit = data.VerticalUpperLimit;
            item1.VerticalParam.RestoreVel = data.VerticalRestoreVel;
            item1.VerticalParam.VelCoefficient = data.VerticalVelCoefficient;

            var item2 = new FirstPersonRotationConfig();
            item2.HorizontalParam.UpperLimit = data.SightHorizontalUpperLimit;
            item2.HorizontalParam.RestoreVel = data.SightHorizontalRestoreVel;
            item2.HorizontalParam.VelCoefficient = data.SightHorizontalVelCoefficient;
            item2.VerticalParam.UpperLimit = data.SightVerticalUpperLimit;
            item2.VerticalParam.RestoreVel = data.SightVerticalRestoreVel;
            item2.VerticalParam.VelCoefficient = data.SightVerticalVelCoefficient;

            return new Tuple<FirstPersonRotationConfig, FirstPersonRotationConfig>(item1, item2);
        }

        public Vector3 GetFirstPersonOffsetByScreenRatio(int weaponId, float ratio)
        {
            if (FirstPersonOffsetScript.UpdateOffset)
            {
                return FirstPersonOffsetScript.StaticFirstPersonOffset;
            }

            CreateFirstPersonPositionOffsetData(GetFirstPersonItemById(weaponId));
            return CheckRatio(ratio);
        }

        public Vector3 GetFirstPersonRotationOffsetByScreenRatio(int weaponId, float ratio)
        {
            if (FirstPersonOffsetScript.UpdateOffset)
            {
                return FirstPersonOffsetScript.StaticFirstPersonRotationOffset;
            }

            CreateFirstPersonRotationOffsetData(GetFirstPersonItemById(weaponId));
            return CheckRatio(ratio);
        }

        public Vector3 GetSightOffsetByScreenRatio(int weaponId, float ratio)
        {
            if (FirstPersonOffsetScript.UpdateOffset)
            {
                return FirstPersonOffsetScript.StaticOffset;
            }

            CreateSightOffsetData(GetFirstPersonItemById(weaponId));
            return CheckRatio(ratio);
        }

        private static Vector3 CheckRatio(float ratio)
        {
            var offset = new Vector3();
            try
            {
                foreach (var item in _ratioDatas)
                {
                    foreach (var ratioIndex in item.Value)
                    {
                        if (ratioIndex > 0 &&
                            CompareUtility.IsApproximatelyEqual(FirstPersonOffsetConfig.ScreenRatios[ratioIndex],
                                ratio))
                        {
                            var offsetData = _offsetDatas[item.Key];
                            offset.Set(offsetData[0], offsetData[1], offsetData[2]);
                            return offset;
                        }
                    }
                }

                var index = RatioNearEqual(ratio);
                if (index >= 0 && index < _offsetDatas.Count)
                {
                    var data = _offsetDatas[index];
                    offset.Set(data[0], data[1], data[2]);
                }
            }
            catch
            {
                Logger.Error("FirstPersonOffset config xml offset Error !");
            }

            return offset;
        }

        private static int RatioNearEqual(float ratio)
        {
            var len = FirstPersonOffsetConfig.ScreenRatios.Length;
            var ratioIndex = 1;
            if (ratio <= FirstPersonOffsetConfig.ScreenRatios[1]) ratioIndex = 1;
            if (ratio >= FirstPersonOffsetConfig.ScreenRatios[len - 1]) ratioIndex = len - 1;

            for (var i = 1; i < len - 1; ++i)
            {
                if (ratio > FirstPersonOffsetConfig.ScreenRatios[i] &&
                    ratio <= FirstPersonOffsetConfig.ScreenRatios[i + 1])
                {
                    ratioIndex = i + 1;
                    break;
                }
            }

            foreach (var item in _ratioDatas)
            {
                foreach (var index in item.Value)
                {
                    if (index == ratioIndex) return item.Key;
                }
            }

            return -1;
        }

        private static Dictionary<int, List<float>> _offsetDatas = new Dictionary<int, List<float>>();
        private static Dictionary<int, List<int>> _ratioDatas = new Dictionary<int, List<int>>();

        private static void CreateFirstPersonPositionOffsetData(FirstPersonOffsetItem offsetItem)
        {
            if (null == offsetItem)
            {
                Logger.Error("firstPerson position offset item is null");
                return;
            }

            _offsetDatas[0] = offsetItem.Offset1;
            _offsetDatas[1] = offsetItem.Offset2;
            _offsetDatas[2] = offsetItem.Offset3;

            _ratioDatas[0] = offsetItem.OffsetRatios1;
            _ratioDatas[1] = offsetItem.OffsetRatios2;
            _ratioDatas[2] = offsetItem.OffsetRatios3;
        }

        private static void CreateFirstPersonRotationOffsetData(FirstPersonOffsetItem offsetItem)
        {
            if (null == offsetItem)
            {
                Logger.Error("firstPerson rotation offset item is null");
                return;
            }
            
            _offsetDatas[0] = offsetItem.FirstPersonRotationOffset1;
            _offsetDatas[1] = offsetItem.FirstPersonRotationOffset2;
            _offsetDatas[2] = offsetItem.FirstPersonRotationOffset3;

            _ratioDatas[0] = offsetItem.FirstPersonRotationOffsetRatios1;
            _ratioDatas[1] = offsetItem.FirstPersonRotationOffsetRatios2;
            _ratioDatas[2] = offsetItem.FirstPersonRotationOffsetRatios3;
        }

        private static void CreateSightOffsetData(FirstPersonOffsetItem offsetItem)
        {
            if (null == offsetItem)
            {
                Logger.Error("offset item is null");
                return;
            }

            _offsetDatas[0] = offsetItem.SightOffset1;
            _offsetDatas[1] = offsetItem.SightOffset2;
            _offsetDatas[2] = offsetItem.SightOffset3;

            _ratioDatas[0] = offsetItem.SightOffsetRatio1;
            _ratioDatas[1] = offsetItem.SightOffsetRatio2;
            _ratioDatas[2] = offsetItem.SightOffsetRatio3;
        }

        private FirstPersonOffsetItem GetFirstPersonItemById(int id)
        {
            FirstPersonOffsetItem ret;
            if (!_firstPersonOffsetConfigs.TryGetValue(id, out ret))
            {
                Logger.WarnFormat("Not exist firstPerson item id: {0}", id);
            }

            return ret;
        }

        /// <summary>
        /// 第一人称腰射
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FirstPersonRotationConfig GetFirstPersonRotationConfig(int id)
        {
            if (FirstPersonOffsetScript.UpdateOffset)
            {
                var value = new FirstPersonRotationConfig();
                CopyTo(value, FirstPersonOffsetScript.StaticFirstPersonRotation);
                return value;
            }

            return GetFristPersonRotationConfigById(id).Item1;
        }

        /// <summary>
        /// 第一人称肩射
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public FirstPersonRotationConfig GetFirstPersonSightRotationConofig(int id)
        {
            if (FirstPersonOffsetScript.UpdateOffset)
            {
                var value = new FirstPersonRotationConfig();
                CopyTo(value, FirstPersonOffsetScript.StaticFirstPersonRotation);
                return value;
            }

            return GetFristPersonRotationConfigById(id).Item2;
        }

        private Core.Utils.Tuple<FirstPersonRotationConfig, FirstPersonRotationConfig>
            GetFristPersonRotationConfigById(int id)
        {
            Core.Utils.Tuple<FirstPersonRotationConfig, FirstPersonRotationConfig> ret;
            if (!_firstPersonRotationConfigs.TryGetValue(id, out ret))
            {
                Logger.WarnFormat("Not exist first person rotation item id: {0}", id);
                ret = _defaultRotationConfig;
            }

            return ret;
        }

        private void CopyTo(FirstPersonRotationConfig dest, Shared.Scripts.FirstPersonRotationConfig source)
        {
            dest.HorizontalParam.UpperLimit = source.HorizontalParam.UpperLimit;
            dest.HorizontalParam.VelCoefficient = source.HorizontalParam.VelCoefficient;
            dest.HorizontalParam.RestoreVel = source.HorizontalParam.RestoreVel;
            dest.VerticalParam.UpperLimit = source.VerticalParam.UpperLimit;
            dest.VerticalParam.VelCoefficient = source.VerticalParam.VelCoefficient;
            dest.VerticalParam.RestoreVel = source.VerticalParam.RestoreVel;
        }
    }
}