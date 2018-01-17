using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

public class ReplacerStream : Stream
{
    private readonly Stream responseFilter;
    private readonly Encoding contentEncoding;

    public ReplacerStream(Stream responseFilter, Encoding contentEncoding)
    {
        this.responseFilter = responseFilter;
        this.contentEncoding = contentEncoding;
    }

    public override bool CanRead => this.responseFilter.CanRead;

    public override bool CanSeek => this.responseFilter.CanSeek;

    public override bool CanWrite => false;

    public override long Length => this.responseFilter.Length;

    public override long Position
    {
        get => this.responseFilter.Position;
        set => throw new NotSupportedException();
    }

    public override void Flush()
    {
        this.responseFilter.Flush();
    }

    public override long Seek(long offset, SeekOrigin origin)
    {
        return this.responseFilter.Seek(offset, origin);
    }

    public override void SetLength(long value)
    {
        throw new NotSupportedException();
    }

    public override int Read(byte[] buffer, int offset, int count)
    {
        return this.responseFilter.Read(buffer, offset, count);
    }

    public override void Write(byte[] buffer, int offset, int count)
    {
        var content = this.contentEncoding.GetString(buffer);
        var contentBytes = this.contentEncoding.GetBytes(
            Regex.Replace(content, "http://www.xxx.com", "https://www.xxx.com", RegexOptions.IgnoreCase));

        this.responseFilter.Write(contentBytes, 0, contentBytes.Length);
    }
}