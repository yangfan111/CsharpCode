namespace Sharpen
{
    public class IntBuffer
    {
        private ByteBuffer byteBuffer;

        public IntBuffer(ByteBuffer byteBuffer)
        {
            this.byteBuffer = byteBuffer;
        }

        public void Put(int i, int f)
        {
            byteBuffer.Position(i);
            byteBuffer.PutInt(f);
        }

        public int Get(int index)
        {
            byteBuffer.Position(index);
            return byteBuffer.GetInt();
        }
    }
}