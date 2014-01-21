namespace java.io
{
    public class RandomInputStream : InputStream
    {
        private System.IO.MemoryStream stream;

        public RandomInputStream(System.IO.MemoryStream stream)
        {
            this.stream = stream;
        }

        public System.IO.MemoryStream InnerStream { get { return stream; } }

        public override int read()
        {
            return stream.ReadByte();
        }

        public override void close()
        {
            stream.Close();
        }
    }
}
