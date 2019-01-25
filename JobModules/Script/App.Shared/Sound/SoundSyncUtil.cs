using App.Shared.Components.Player;
using Core.Components;
using Core.Utils;
using System;
using System.Collections.Generic;
using XmlConfig;

namespace App.Shared.Sound
{
    public static class SoundSyncUtil
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(SoundSyncUtil));
        private const int ByteCountInLong = 8;
        private const int ShortCountInLong = 4;
        private const int ShortCountInInt = 4;
        private const int ByteCountInInt = 2;
        private const int LongLength = 64;
        private const int IntLength = 32;
        private const int ShortLength = 16;
        private const int ByteLength = 8;
        private static List<short> _lastPlayList = new List<short>(8);
        private static List<short> _curPlayList = new List<short>(8);
        public static void NonLoopSoundToComponent(short soundId, SoundComponent comp)
        {
            SetAvaliableShort(soundId, ref comp.PlayOnce);
        }

        public static void LoopSoundToComponent(short soundId, SoundComponent comp)
        {
            SetAvaliableShort(soundId, ref comp.Playing);
        }

        public static void NewNonLoopSoundFromComponent(SoundComponent comp, List<short> list)
        {
            if(null == list)
            {
                Logger.Error("GetNewSoundFromComponent failed , list is null");
                return;
            }
            if(null == comp)
            {
                Logger.Error("GetNewSoundFromComponent failed , comp is null");
                return;
            }
            list.Clear();
            Diff(comp.LastPlayOnce, comp.PlayOnce,  list);
            comp.LastPlayOnce = comp.PlayOnce;
        }

        public static void NewLoopSoundFromComponent(SoundComponent comp, List<short> list)
        {
            if (null == list)
            {
                Logger.Error("GetNewSoundFromComponent failed , list is null");
                return;
            }
            if(null == comp)
            {
                Logger.Error("GetNewSoundFromComponent failed , comp is null");
                return;
            }
            list.Clear();
            Diff(comp.LastPlayOnce, comp.PlayOnce,  list );
            comp.LastPlayOnce = comp.PlayOnce;
        }

        public static void Diff(long last, long current, List<short> list)
        {
            _lastPlayList.Clear();
            GetShorts(last, _lastPlayList);
            _curPlayList.Clear();
            GetShorts(current, _curPlayList);
            for(var i = 0; i < ShortCountInLong; i++)
            {
                if(_curPlayList[i] != _lastPlayList[i])
                {
                    if(_curPlayList[i] > 0)
                    {
                        list.Add(_curPlayList[i]);
                    }
                }
            }
        }

        public static void Diff(int last, int current,  List<short> list)
        {
            _lastPlayList.Clear();
            GetShorts(last, _lastPlayList);
            _curPlayList.Clear();
            GetShorts(current, _curPlayList);
            for(var i = 0; i < ByteCountInInt; i++)
            {
                if(_curPlayList[i] != _lastPlayList[i])
                {
                    if(_curPlayList[i] > 0)
                    {
                        list.Add(_curPlayList[i]);
                    }
                }
            }
        }

        ////循环设置声音，并将循环的下一个（最老的一个）清空
        public static void SetAvaliableByte(byte val, ref long target)
        {
            SetAvaliableVal(val, 0xFF, ByteLength, ref target);
            //if (val <= 0)
            //{
            //    return;
            //}
            //long lval = val;
            //for (int i = 0; i < ByteCountInLong; i++)
            //{
            //    var index = i * ByteLength;
            //    if (((target >> index) & 0xFF) < 1)
            //    {
            //        target += lval << index;
            //        var next = (index + ByteLength) % LongLength;
            //        long mask = (long)0xFF << (next);
            //        target &= ~mask;
            //        break;
            //    }
            //}
        }

        public static void SetAvaliableByte(byte val, ref int target)
        {
            SetAvaliableVal(val, 0xFF, ByteLength, ref target);
            //if (val <= 0)
            //{
            //    return;
            //}
            //int ival = val;
            //for (int i = 0; i < ByteCountInLong; i++)
            //{
            //    var index = i * ByteLength;
            //    if (((target >> index) & 0xFF) < 1)
            //    {
            //        target += ival << index;
            //        var next = (index + ByteLength) % IntLength;
            //        int mask = 0xFF << (next);
            //        target &= ~mask;
            //        break;
            //    }
            //}
        }

        public static void SetAvaliableShort(short val, ref long target)
        {
            SetAvaliableVal(val, 0xFFFF, ShortLength, ref target);
        }

        public static void SetAvaliableShort(short val, ref int target)
        {
            SetAvaliableVal(val, 0xFFFF, ShortLength, ref target);
        }

        public static void SetAvaliableVal(int val, int bitMask, int valLength, ref int target)
        {
            for (int i = 0; i < ByteCountInInt; i++)
            {
                var index = i * valLength;
                if(((target >> index) & bitMask) < 1)
                {
                    target += val << index;
                    var next = (index + valLength) % IntLength;
                    int mask = bitMask << (next);
                    target &= ~mask;
                    break;
                }
            }
        }

        public static void SetAvaliableVal(long val, long bitMask, int valLength, ref long target)
        {
            for (int i = 0; i < ByteCountInLong; i++)
            {
                var index = i * valLength;
                if(((target >> index) & bitMask) < 1)
                {
                    target += val << index;
                    var next = (index + valLength) % LongLength;
                    long mask = bitMask << (next);
                    target &= ~mask;
                    break;
                }
            }
        }

        public static void GetBytes(int source, List<byte> target)
        {
            if(null == target)
            {
                Logger.Error("GetBytes failed, result list is null");
                return;
            }
            target.Clear();
            for(int i = 0; i < ByteCountInInt; i++)
            {
                var index = i * ByteLength;
                var r = (byte)((source >> index) & 0xFF);
                target.Add(r);
            }
        }

        public static void GetBytes(long source, List<byte> target)
        {
            if(null == target)
            {
                Logger.Error("GetBytes failed, result list is null");
                return;
            }
            target.Clear();
            for(int i = 0; i < ByteCountInLong; i++)
            {
                var index = i * ByteLength;
                var r = (byte)((source >> index) & 0xFF);
                target.Add(r);
            }
        }

        public static void GetShorts(long source, List<short> target)
        {
            if(null == target)
            {
                Logger.Error("GetBytes failed, result list is null");
                return;
            }
            target.Clear();
            for(int i = 0; i < ShortCountInLong; i++)
            {
                var index = i * ShortLength;
                var r = (short)((source >> index) & 0xFFFF);
                target.Add(r);
            }
        }

        public static void GetShorts(int source, List<short> target)
        {
            if(null == target)
            {
                Logger.Error("GetBytes failed, result list is null");
                return;
            }
            target.Clear();
            for(int i = 0; i < ShortCountInInt; i++)
            {
                var index = i * ShortLength;
                var r = (short)((source >> index) & 0xFFFF);
                target.Add(r);
            }
        }
    }
}
