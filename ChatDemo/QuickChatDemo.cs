
using System.IO.Ports;
using System.Text;

public sealed class SerialComm : IDisposable
{
    public const byte STX = 0x02;
    public const byte ETX = 0x03;
    public const byte DLE = 0x10;

    private readonly SerialPort _port;
    private readonly object _ioLock = new();

    public bool AutoOpenIfClosed { get; set; } = true;

    public SerialComm(string portName, int baud = 115200, Parity parity = Parity.None, int dataBits = 8, StopBits stopBits = StopBits.One)
    {
        _port = new SerialPort(portName, baud, parity, dataBits, stopBits)
        {
            ReadTimeout = 50,
            WriteTimeout = 1000
        };
    }

    public void Open()
    {
        if (!_port.IsOpen) _port.Open();
    }

    public void Close()
    {
        if (_port.IsOpen) _port.Close();
    }

    public void Dispose() => Close();

    public async Task<byte[]> SendAndReceiveAsync(byte cmd, ReadOnlyMemory<byte> payload, int timeoutMs = 1000, CancellationToken ct = default)
    {
        if (AutoOpenIfClosed && !_port.IsOpen) Open();
        if (!_port.IsOpen) throw new InvalidOperationException("Serial port is not open.");

        var frame = BuildFrame(cmd, payload.Span);

        lock (_ioLock)
        {
            _port.DiscardInBuffer();
            _port.Write(frame, 0, frame.Length);
        }

        using var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        cts.CancelAfter(timeoutMs);

        var buffer = new List<byte>(256);
        var temp = new byte[256];

        while (!cts.IsCancellationRequested)
        {
            int n = 0;
            lock (_ioLock)
            {
                try { n = _port.Read(temp, 0, temp.Length); }
                catch (TimeoutException) { }
            }
            if (n > 0)
            {
                buffer.AddRange(temp.AsSpan(0, n).ToArray());
                if (TryExtractFrame(buffer, out var unescaped))
                {
                    if (unescaped.Length < 1 + 2 + 1 + 2 + 1)
                        throw new FormatException("Frame too short.");

                    if (unescaped[0] != STX || unescaped[^1] != ETX)
                        throw new FormatException("Frame markers invalid.");

                    var core = unescaped.AsSpan(1, unescaped.Length - 2);
                    ushort len = core[0];
                    if (len + 1 > core.Length)
                        throw new FormatException("Length mismatched.");

                    var cmdResp = core[1];
                    var body = core.Slice(2, len - 1 - 2);
                    var crcSpan = core.Slice(2 + body.Length, 2);
                    ushort crcRx = (ushort)(crcSpan[0] | (crcSpan[1] << 8));

                    ushort crcCalc = Crc16Modbus(core.Slice(0, len).ToArray());
                    if (crcRx != crcCalc)
                        throw new FormatException("CRC mismatch.");

                    if (cmdResp != (byte)(cmd | 0x80) && cmdResp != cmd)
                    {
                    }

                    return body.ToArray();
                }
            }

            await Task.Delay(5, cts.Token);
        }

        throw new TimeoutException("Receive timeout.");
    }

    public static byte[] BuildFrame(byte cmd, ReadOnlySpan<byte> payload)
    {
        int len = 1 + payload.Length + 2;
        var core = new byte[1 + len];
        core[0] = (byte)len;
        core[1] = cmd;
        payload.CopyTo(core.AsSpan(2));

        ushort crc = Crc16Modbus(core.AsSpan(0, len).ToArray());
        core[^2] = (byte)(crc & 0xFF);
        core[^1] = (byte)(crc >> 8);

        var framed = new List<byte>(core.Length + 4) { STX };
        EscapeAndAppend(core, framed);
        framed.Add(ETX);
        return framed.ToArray();
    }

    private static bool TryExtractFrame(List<byte> buf, out byte[] unescapedFrame)
    {
        unescapedFrame = Array.Empty<byte>();
        int start = buf.IndexOf(STX);
        if (start < 0) { buf.Clear(); return false; }
        int end = buf.IndexOf(ETX, start + 1);
        if (end < 0) return false;

        var slice = buf.GetRange(start, end - start + 1).ToArray();
        var output = new List<byte>(slice.Length);
        bool inCore = false;
        for (int i = 0; i < slice.Length; i++)
        {
            byte b = slice[i];
            if (!inCore)
            {
                output.Add(b);
                if (b == STX) inCore = true;
                continue;
            }

            if (i == slice.Length - 1)
            {
                output.Add(b);
                break;
            }

            if (b == DLE)
            {
                if (i + 1 >= slice.Length - 1) return false;
                output.Add(slice[i + 1]);
                i++;
            }
            else
            {
                output.Add(b);
            }
        }

        buf.RemoveRange(0, end + 1);
        unescapedFrame = output.ToArray();
        return true;
    }

    private static void EscapeAndAppend(ReadOnlySpan<byte> src, List<byte> dest)
    {
        foreach (var b in src)
        {
            if (b == STX || b == ETX || b == DLE)
            {
                dest.Add(DLE);
                dest.Add(b);
            }
            else
            {
                dest.Add(b);
            }
        }
    }

    public static ushort Crc16Modbus(ReadOnlySpan<byte> data)
    {
        ushort crc = 0xFFFF;
        foreach (var b in data)
        {
            crc ^= b;
            for (int i = 0; i < 8; i++)
            {
                bool lsb = (crc & 0x0001) != 0;
                crc >>= 1;
                if (lsb) crc ^= 0xA001;
            }
        }
        return crc;
    }

    public static byte XorChecksum(ReadOnlySpan<byte> data)
    {
        byte s = 0;
        foreach (var b in data) s ^= b;
        return s;
    }

    public static byte[] HexToBytes(string hex)
    {
        hex = hex.Replace(" ", "").Replace("-", "");
        if (hex.Length % 2 != 0) throw new ArgumentException("Hex length must be even.");
        var bytes = new byte[hex.Length / 2];
        for (int i = 0; i < bytes.Length; i++)
            bytes[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
        return bytes;
    }

    public static string BytesToHex(ReadOnlySpan<byte> data, string sep = " ")
    {
        var sb = new StringBuilder(data.Length * 3);
        for (int i = 0; i < data.Length; i++)
        {
            if (i > 0 && sep is not null) sb.Append(sep);
            sb.Append(data[i].ToString("X2"));
        }
        return sb.ToString();
    }

    public static int BcdToInt(ReadOnlySpan<byte> bcd)
    {
        int val = 0;
        foreach (var b in bcd)
        {
            int hi = (b >> 4) & 0x0F;
            int lo = b & 0x0F;
            if (hi > 9 || lo > 9) throw new FormatException("Invalid BCD digit.");
            val = val * 100 + hi * 10 + lo;
        }
        return val;
    }

    public static byte[] IntToBcd(int value, int bytes)
    {
        if (value < 0) throw new ArgumentOutOfRangeException(nameof(value));
        var result = new byte[bytes];
        for (int i = bytes - 1; i >= 0; i--)
        {
            int lo = value % 10; value /= 10;
            int hi = value % 10; value /= 10;
            result[i] = (byte)((hi << 4) | lo);
        }
        if (value != 0) throw new ArgumentOutOfRangeException(nameof(value), "Value too large for BCD size.");
        return result;
    }

    public static short ToInt16BE(ReadOnlySpan<byte> d, int o = 0) => (short)((d[o] << 8) | d[o + 1]);
    public static short ToInt16LE(ReadOnlySpan<byte> d, int o = 0) => (short)(d[o] | (d[o + 1] << 8));
    public static ushort ToUInt16BE(ReadOnlySpan<byte> d, int o = 0) => (ushort)((d[o] << 8) | d[o + 1]);
    public static ushort ToUInt16LE(ReadOnlySpan<byte> d, int o = 0) => (ushort)(d[o] | (d[o + 1] << 8));
    public static int ToInt32BE(ReadOnlySpan<byte> d, int o = 0) => (d[o] << 24) | (d[o + 1] << 16) | (d[o + 2] << 8) | d[o + 3];
    public static int ToInt32LE(ReadOnlySpan<byte> d, int o = 0) => d[o] | (d[o + 1] << 8) | (d[o + 2] << 16) | (d[o + 3] << 24);
    public static float ToFloatBE(ReadOnlySpan<byte> d, int o = 0) => BitConverter.ToSingle(new byte[] { d[o + 3], d[o + 2], d[o + 1], d[o] }, 0);
    public static float ToFloatLE(ReadOnlySpan<byte> d, int o = 0) => BitConverter.ToSingle(new byte[] { d[o], d[o + 1], d[o + 2], d[o + 3] }, 0);

    public static ushort Swap16(ushort x) => (ushort)((x << 8) | (x >> 8));
    public static uint Swap32(uint x) => (x << 24) | ((x & 0xFF00u) << 8) | ((x & 0xFF0000u) >> 8) | (x >> 24);

    public static int TwosComplementToInt(uint raw, int bitWidth)
    {
        if (bitWidth <= 0 || bitWidth > 31) throw new ArgumentOutOfRangeException(nameof(bitWidth));
        uint mask = (1u << bitWidth) - 1u;
        raw &= mask;
        bool neg = (raw & (1u << (bitWidth - 1))) != 0;
        return neg ? -((int)((~raw + 1) & mask)) : (int)raw;
    }

    public static (double ch1, double ch2) ParseAdc24ToEngineering(ReadOnlySpan<byte> payload, double scale1, double offset1, double scale2, double offset2)
    {
        if (payload.Length < 6) throw new ArgumentException("Payload too short.");
        int raw1 = (payload[0] << 16) | (payload[1] << 8) | payload[2];
        int raw2 = (payload[3] << 16) | (payload[4] << 8) | payload[5];

        raw1 = TwosComplementToInt((uint)raw1, 24);
        raw2 = TwosComplementToInt((uint)raw2, 24);

        double eng1 = raw1 * scale1 + offset1;
        double eng2 = raw2 * scale2 + offset2;
        return (eng1, eng2);
    }
}

