
namespace Assets.Sources.Free.Effect
{

    public class RaderImage
    {
        public string img;
        public float scaleX;
        public float scaleY;
        public float alpha;
        public string smallMapFullImg;

        // 1=isMask, 2=useMask,3=full,4=useMask And full
        public int mask;

        public RaderImage()
        {
            scaleX = 100;
            scaleY = 100;
            alpha = 100;
            img = "";
            smallMapFullImg = "";
        }

        public bool IsMask()
        {
            return mask == 1;
        }

        public bool UseMask()
        {
            return mask == 2 || mask == 4;
        }

        public bool IsFull()
        {
            return mask == 3 || mask == 4;
        }
    }
}
