namespace Core
{
    public struct BulletStatisticsInfo
    {
        public string originStr;
        public string bulletBaseStr;
        public string bulletRunStartStr;
        
        public string bulletRunEndStr;
        public string aniStr;
        public string colliderStr;
        public bool isNewIns;
        public int cmdSeq;
        public bool isBlockIns;
        //todo:
        public override string ToString()
        {
            return string.Format(
                "[cmdReq]{0}\n[origin]\n{1}\n[bulletBase]\n{2}\n[bulletStart]\n{3}\n[bulletEnd]\n{4}\n[aniStr]\n{5}\n[colliderStr]\n{6}",
                cmdSeq, originStr, bulletBaseStr, bulletRunStartStr, bulletRunEndStr, aniStr, colliderStr);
        }
    }
}