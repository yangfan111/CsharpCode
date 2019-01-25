namespace App.Shared.Components.Ui
{
    public class SplitPropInfo
    {
        public SplitPropInfo(long categoryId, long propId, int num, string propName, string key)
        {
            this.categoryId = categoryId;
            this.propId = propId;
            this.Num = num;
            this.propName = propName;
            this.key = key;
        }

        public long categoryId;                      //种类ID
        public long propId;                       //种类内的唯一ID
        public int Num;                           //道具个数
        public string propName;                   //道具名     
        public string key;                        //道具在物品栏中的位置
    }
}