using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Xml.Serialization;

namespace XmlConfig.BootConfig
{
    [XmlType("config")]
    public class TerrainSampleConfig
    {
        public string Url;
        public string Key;
        public string Quality;

        /// <summary>
        /// 是否使用游戏大厅的游戏品级设置
        /// </summary>
        public bool UseHallQuality;

        /// <summary>
        /// 大厅游戏品质等级
        /// </summary>
        public int HallQuality;

        /// <summary>
        /// 每次采样的帧数，从中去除最大最小值后求平均值
        /// </summary>
        public int SampleCount;

        /// <summary>
        /// 采样点资源准备完毕后等待的帧数，用于测试渡过最初的卡顿期
        /// </summary>
        public int WaitForSample;

        /// <summary>
        /// 是否启用帧率限制，启用时当在采样处低于FrameLimit时暂停
        /// </summary>
        public bool EnableFrameLimit;

        /// <summary>
        /// 采样帧率限制
        /// </summary>
        public int FrameLimit;

        /// <summary>
        /// 是否启用位置限制，启用时当采样完该点时暂停
        /// </summary>
        public bool EnablePosLimit;

        /// <summary>
        /// 采样限制位置的X坐标
        /// </summary>
        public int PosLimitX;

        /// <summary>
        /// 采样限制位置的Z坐标
        /// </summary>
        public int PosLimitZ;

        /// <summary>
        /// 采样限制位置的方向，-1表示取最坏，0~3对应四个方向
        /// </summary>
        public int PosLimitDir;
    }
}