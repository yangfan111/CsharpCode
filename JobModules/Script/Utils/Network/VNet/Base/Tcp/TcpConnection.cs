//#define TCP_DEBUG
using System;
using System.IO;
using System.Net.Sockets;
using Core.ObjectPool;
using Core.Utils;
using VNet.Base.Interface;

namespace VNet.Base.Tcp
{
    public class SendSocketAsyncEventArgsExt : SocketAsyncEventArgs, IReusableObject
    {
        public MemoryStream SendBuffer = new MemoryStream();
        public void ReInit()
        {
            SendBuffer.Seek(0, SeekOrigin.Begin);
            SendBuffer.SetLength(0);
        }
    }

    public class ReceiveSocketAsyncEventArgsExt : SocketAsyncEventArgs, IReusableObject
    {
        private const int BufferSize = 2000;
        public byte[] ReceiveBuffer = new byte[BufferSize];

        public ReceiveSocketAsyncEventArgsExt()
        {
            SetBuffer(ReceiveBuffer, 0, BufferSize);
        }

        public void ReInit()
        {
        }
    }

    public class TcpConnection : IVNetPeer
    {
        private static readonly LoggerAdapter Logger = new LoggerAdapter(typeof(TcpConnection)); 
        public event Action<IVNetPeer, MemoryStream> OnReceiveListener;
        public event Action<IVNetPeer> OnDisconnectListener;

        private readonly MemoryStream _receiveMemoryStream = new MemoryStream();
        private readonly MemoryStream _remainMemoryStream = new MemoryStream();
        private Socket ConnSocket { get; set; }
        private readonly MemoryStream _receiveData = new MemoryStream();
        private bool _littleEndian;

        public MemoryStream ReceiveData
        {
            get { return _receiveData; }
        }

        private static int _connectid;
        public int ConnectId { get; private set; }

        public TcpConnection(bool littleEndian)
        {
            _connectid += 1;
            ConnectId = _connectid;
            _littleEndian = littleEndian;
        }


        //s.Poll returns true if
        //connection is closed, reset, terminated or pending(meaning no active connection)
        //connection is active and there is data available for reading
        //s.Available returns number of bytes available for reading
        //if both are true:
        //there is no data available to read so connection is not active

        public bool IsConnected
        {
            get
            {
                try
                {
                    return _isConnected;
                }
                catch (Exception e)
                {
                    Logger.Error(e);
                    return false;
                }
            }
        }

        private bool _isConnected;
        private string _ip;

        public void OnConnect(Socket acceptSocket)
        {
            try
            {
                _ip = acceptSocket.RemoteEndPoint.ToString();
                ConnSocket = acceptSocket;
                _isConnected = true;
            }
            catch (System.Exception e)
            {
                Logger.Error(e);
            }
          
        }

        public SocketError ErrorCode { get; set; }

        public void StartReceive(SocketAsyncEventArgs receiveArg)
        {
            if (null == ConnSocket)
            {
                return;
            }

            if (null == receiveArg)
            {
                receiveArg = ObjectAllocatorHolder<ReceiveSocketAsyncEventArgsExt>.Allocate();
                receiveArg.Completed += OnReceiveComplete;
            }

            var async = ConnSocket.ReceiveAsync(receiveArg);
            if (!async)
            {
                OnReceive(receiveArg);
            }
        }

        private void OnReceiveComplete(object sender, SocketAsyncEventArgs e)
        {
            OnReceive(e);
        }

        private void OnReceive(SocketAsyncEventArgs receiveArg)
        {
            if (receiveArg.BytesTransferred > 0 && receiveArg.SocketError == SocketError.Success)
            {
                _receiveMemoryStream.Write(receiveArg.Buffer, receiveArg.Offset, receiveArg.BytesTransferred);
                int remainOffset = 0;
                while (true)
                {
                    if (!ProcessRecieve(ref remainOffset))
                    {
                        break;
                    }
                }
                ClearReceiveStream(_receiveMemoryStream, _remainMemoryStream, remainOffset);
                StartReceive(receiveArg);
            }
            else
            {
                OnDisconnect();
                Logger.InfoFormat("Tcp disconnect with error {0} {1}", receiveArg.SocketError,_connectid);
            }
        }

        private bool ProcessRecieve(ref int remainOffset)
        {
            int headLength = 4;
            int lenIndex = 0;
            if (_receiveMemoryStream.Length < headLength + remainOffset)
            {
                return false;
            }

            var realLenIndex = lenIndex + remainOffset;
            var buffer = _receiveMemoryStream.GetBuffer();
            var messageLength = _littleEndian ? BitConverter.ToInt32(buffer, realLenIndex) : 
                ( buffer[realLenIndex] << 24) | (buffer[realLenIndex + 1] << 16 ) | (buffer[realLenIndex + 2] << 8) | buffer[realLenIndex + 3];
            if (_receiveMemoryStream.Length < headLength + messageLength + remainOffset)
            {
                return false;
            }

            ReceiveData.Position = 0;
            ReceiveData.SetLength(0);
            ReceiveData.Write(_receiveMemoryStream.GetBuffer(), headLength + remainOffset, messageLength);

            if (null != OnReceiveListener)
            {
                OnReceiveListener(this, ReceiveData);
            }

             remainOffset = headLength + messageLength + remainOffset;
            if (_receiveMemoryStream.Length > remainOffset)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ClearReceiveStream(MemoryStream stream, MemoryStream tempCache, int remainOffset)
        {
            if (remainOffset == stream.Length)
            {
                stream.Position = 0;
                stream.SetLength(0);
                return;
            }
            tempCache.Position = 0;
            tempCache.SetLength(0);
            tempCache.Write(stream.GetBuffer(), remainOffset,
                (int)stream.Length - remainOffset);
            stream.Position = 0;
            stream.SetLength(0);
            stream.Write(tempCache.GetBuffer(), 0, (int)tempCache.Length); 
        }


        public void Send(byte[] bytes, int length, int offset = 0)
        {
            if (!IsConnected)
            {
                return;
            }

            SendSocketAsyncEventArgsExt sendArg = ObjectAllocatorHolder<SendSocketAsyncEventArgsExt>.Allocate();
            sendArg.Completed += OnSendComplete;
            var sendMs = sendArg.SendBuffer;
            
            sendMs.Position = 0;
            sendMs.SetLength(0);

            //var lenArray = BitConverter.GetBytes(length);
            if (!_littleEndian)
            {
                sendMs.WriteByte((byte)(length>>24));
                sendMs.WriteByte((byte)(length>>16));
                sendMs.WriteByte((byte)(length>>8));
                sendMs.WriteByte((byte)length);
                
               
            }
            else
            {
                sendMs.WriteByte((byte)length);
                sendMs.WriteByte((byte)(length>>8));
                sendMs.WriteByte((byte)(length>>16));
                sendMs.WriteByte((byte)(length>>24));
            }
           
            sendMs.Write(bytes, offset, length);
            sendArg.SetBuffer(sendMs.GetBuffer(), 0, (int) sendMs.Length);

            //Logger.Debug("RealSendMsg");
            var async = ConnSocket.SendAsync(sendArg);
            if (!async)
            {
                OnSend(sendArg);
            }
        }

        private void OnSendComplete(object sender, SocketAsyncEventArgs args)
        {
            OnSend(args);
        }

        private void OnSend(SocketAsyncEventArgs args)
        {
            args.Completed -= OnSendComplete;
            ObjectAllocatorHolder<SendSocketAsyncEventArgsExt>.Free(args as SendSocketAsyncEventArgsExt);
            if (args.SocketError != SocketError.Success)
            {
                Logger.ErrorFormat("tcp send failed error {0} {1}", args.SocketError, _connectid);
                OnDisconnect();   
            }
        }

        public void OnDisconnect()
        {
            if (!_isConnected)
            {
                return;
            }
            _isConnected = false;
            try
            {
                if (this.ConnSocket != null)
                {
                    this.ConnSocket.Shutdown(SocketShutdown.Both);
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("Shutdown failed with error {0}", ex.Message);
            }
            try
            {
                if (this.ConnSocket != null)
                {
                    this.ConnSocket.Close();
                }
            }
            catch (Exception ex)
            {
                Logger.ErrorFormat("Close failed with error {0}", ex.Message);
            }
            this.ConnSocket = null;
            if (null != OnDisconnectListener)
            {
                OnDisconnectListener(this);
            }
        }

        public string DebugInfo
        {
            get
            {
                return string.Empty;
            }
        }

        public string Ip
        {
            get { return _ip; }
        }
    }
}
