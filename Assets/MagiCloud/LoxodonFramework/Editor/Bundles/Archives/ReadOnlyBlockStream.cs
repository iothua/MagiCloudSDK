#if UNITY_EDITOR
using System;
using System.IO;

namespace Loxodon.Framework.Bundles.Archives
{
    public class ReadOnlyBlockStream : Stream
    {
        private Stream innerStream;
        private bool leaveInnerStreamOpen = false;
        private long startPosition;
        private long length;
        private long position;

        public ReadOnlyBlockStream(Stream innerStream, long offset, long length, bool leaveInnerStreamOpen = false)
        {

            if (innerStream == null || !innerStream.CanRead || offset < 0 || length < 0 || (offset + length) > innerStream.Length)
                throw new ArgumentException();
            lock (innerStream)
            {
                this.innerStream = innerStream;
                this.startPosition = offset;
                this.length = length;
                this.Position = 0;
                this.leaveInnerStreamOpen = leaveInnerStreamOpen;
            }
        }

        public override bool CanTimeout
        {
            get { return false; }
        }

        public override bool CanRead
        {
            get { return this.position < this.length && innerStream.CanRead; }
        }

        public override bool CanSeek
        {
            get { return this.innerStream.CanSeek; }
        }

        public override bool CanWrite
        {
            get { return false; }
        }

        public override int ReadTimeout
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public override int WriteTimeout
        {
            get { throw new NotSupportedException(); }
            set { throw new NotSupportedException(); }
        }

        public override long Length
        {
            get { return this.length; }
        }

        public override long Position
        {
            get { return this.position; }
            set
            {
                lock (innerStream)
                {
                    this.position = value;
                    this.innerStream.Position = startPosition + this.position;
                }
            }
        }

        public override void Flush()
        {
            throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            lock (innerStream)
            {
                if (!this.CanRead)
                    throw new IOException();

                if (this.innerStream.Position != this.startPosition + this.position)
                    this.innerStream.Position = this.startPosition + this.position;

                int len = Math.Min((int)(this.length - this.position), count);

                if (len <= 0)
                    throw new EndOfStreamException();

                int ret = this.innerStream.Read(buffer, offset, len);
                this.position = innerStream.Position - this.startPosition;
                return ret;
            }
        }

        public override long Seek(long offset, SeekOrigin origin)
        {
            lock (this.innerStream)
            {
                switch (origin)
                {
                    case SeekOrigin.Begin:
                        if (offset < 0)
                            throw new IOException();

                        this.position = this.innerStream.Seek(this.startPosition + offset, origin) - this.startPosition;
                        break;
                    case SeekOrigin.End:
                        if (offset > 0)
                            throw new IOException();

                        this.position = this.innerStream.Seek(this.startPosition + this.length + offset, origin) - this.startPosition;
                        break;
                    default:
                        if (offset < -this.position || offset > this.length - this.position)
                            throw new IOException();

                        this.position = this.innerStream.Seek(offset, origin) - this.startPosition;
                        break;
                }
                return this.position;
            }
        }

        #region IDisposable Support
        private bool disposed = false;
        protected override void Dispose(bool disposing)
        {
            lock (innerStream)
            {
                if (!disposed)
                {
                    if (!this.leaveInnerStreamOpen)
                    {
                        this.innerStream.Close();
                    }
                    base.Dispose(disposing);
                    disposed = true;
                }
            }
        }
        #endregion

        public override IAsyncResult BeginRead(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            throw new NotSupportedException();
        }

        public override IAsyncResult BeginWrite(byte[] buffer, int offset, int count, AsyncCallback callback, object state)
        {
            throw new NotSupportedException();
        }

        public override int EndRead(IAsyncResult asyncResult)
        {
            throw new NotSupportedException();
        }
        public override void EndWrite(IAsyncResult asyncResult)
        {
            throw new NotSupportedException();
        }

        public override void SetLength(long value)
        {
            throw new NotSupportedException();
        }

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new NotSupportedException();
        }
    }
}
#endif