namespace Sharpen
{
    public class FloatBuffer
    {
        private ByteBuffer byteBuffer;

        public FloatBuffer(ByteBuffer byteBuffer)
        {
            this.byteBuffer = byteBuffer;
        }

        public void Put(int i, float f)
        {
            byteBuffer.Position(i);
            byteBuffer.PutFloat(f);
        }

        public float Get(int index)
        {
            byteBuffer.Position(index);
            return byteBuffer.GetFloat();
        }
    }
}