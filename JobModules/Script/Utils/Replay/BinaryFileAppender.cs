using System;
using System.IO;
using Unity.IO.Compression;

namespace Utils.Replay
{
    public class BinaryFileAppender:IDisposable
    {
        private readonly string _fileName;
        private readonly BinaryWriter _writer;
        private readonly FileStream _fileStream;
        private readonly DeflateStream _deflateStream;


        public BinaryFileAppender(string filename)
        {
            _fileName = filename;
            _fileStream = new FileStream(_fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read, 4096);
            _deflateStream= new DeflateStream(_fileStream, CompressionMode.Compress);
            _writer = new BinaryWriter(_deflateStream);
        }

        public long Offset
        {
            get { return _writer.BaseStream.Position; }
        }

        public void Write(MemoryStream stream)
        {
            _writer.Write(stream.GetBuffer(), 0, (int) stream.Position);
        }

        public void Write(int n)
        {
            _writer.Write(n);
        }

        public void Dispose()
        {
            _fileStream.Dispose();
            _deflateStream.Dispose();
            _writer.Close();
        }
    }

    public class BinaryFileReader:IDisposable
    {
        private readonly string _fileName;
        private readonly BinaryReader _reader;
        private readonly DeflateStream _deflateStream;
        private readonly FileStream _fileStream;
        public BinaryFileReader(string fileName)
        {
            _fileName = fileName;
            _fileStream = new FileStream(_fileName, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.Read, 4096);
            _deflateStream= new DeflateStream(_fileStream, CompressionMode.Decompress);
            _reader =new BinaryReader (_deflateStream);
        }
        public BinaryReader Reader
        {
            get { return _reader; }
        }

        public object Offset
        {
            get { return _reader.BaseStream.Position; }
        }

        public void Dispose()
        {
            _fileStream.Dispose();
            _deflateStream.Dispose();
            _reader.Close();
        }

        public bool HasRemain()
        {
            return true;
        }
    }
}