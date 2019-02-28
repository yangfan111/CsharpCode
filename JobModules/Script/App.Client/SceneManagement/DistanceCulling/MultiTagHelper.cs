using ArtPlugins;
using UnityEngine;

namespace App.Client.SceneManagement.DistanceCulling
{
    static class MultiTagHelper
    {
        public static bool InDoor(int tagValue)
        {
            return (tagValue & (1 << (int)MultiTagBase.TagEnum.InOnlyProps)) != 0;
        }

        public static bool OutDoor(int tagValue)
        {
            return (tagValue & (1 << (int)MultiTagBase.TagEnum.OutProps)) != 0
                || (tagValue & (1 << (int)MultiTagBase.TagEnum.InBothProps)) != 0;
        }

        public static bool RoadShortDecal(GameObject go)
        {
            bool ret = false;

            if (go != null)
            {
                var tag = go.GetComponent<MultiTagBase>();
                if (tag != null)
                {
                    ret = (tag.tagMask & (1 << (int)MultiTagBase.TagEnum.RoadShortDecal)) != 0;
                }
            }

            return ret;
        }

        public static bool RoadLongDecal(GameObject go)
        {
            bool ret = false;

            if (go != null)
            {
                var tag = go.GetComponent<MultiTagBase>();
                if (tag != null)
                {
                    ret = (tag.tagMask & (1 << (int)MultiTagBase.TagEnum.RoadLongDecal)) != 0;
                }
            }

            return ret;
        }

        public static bool RoadCross(GameObject go)
        {
            bool ret = false;

            if (go != null)
            {
                var tag = go.GetComponent<MultiTagBase>();
                if (tag != null)
                {
                    ret = (tag.tagMask & (1 << (int)MultiTagBase.TagEnum.RoadCross)) != 0;
                }
            }

            return ret;
        }

        public static bool Road(GameObject go)
        {
            bool ret = false;

            if (go != null)
            {
                var tag = go.GetComponent<MultiTagBase>();
                if (tag != null)
                {
                    ret = (tag.tagMask & (1 << (int)MultiTagBase.TagEnum.Road)) != 0;
                }
            }

            return ret;
        }
    }
}