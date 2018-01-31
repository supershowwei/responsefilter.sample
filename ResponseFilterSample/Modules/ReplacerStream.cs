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
        var replacedBytes = this.contentEncoding.GetBytes("http://www.xxx.com");
        var replacementBytes = this.contentEncoding.GetBytes("https://www.xxx.com");

        var flag = 0;
        while (flag < buffer.Length)
        {
            if (Match(buffer, flag, replacedBytes))
            {
                this.responseFilter.Write(replacementBytes, 0, replacementBytes.Length);
                flag += replacedBytes.Length;
            }
            else
            {
                this.responseFilter.Write(buffer, flag, 1);
                flag++;
            }
        }
    }

    private static bool Match(byte[] compared, int flag, byte[] comparison)
    {
        var i = 0;
        for (; i < comparison.Length && (flag + i) < compared.Length; i++)
        {
            if (compared[flag + i] != comparison[i]) break;
        }

        return i.Equals(comparison.Length);
    }
}