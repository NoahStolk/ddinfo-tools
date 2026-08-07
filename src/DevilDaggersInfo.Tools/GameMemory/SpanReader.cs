using System.Text;

namespace DevilDaggersInfo.Tools.GameMemory;

public ref struct SpanReader
{
	private readonly Span<byte> _buffer;
	private int _position;

	public SpanReader(Span<byte> buffer)
	{
		_buffer = buffer;
		_position = 0;
	}

	public void Seek(int length)
	{
		_position += length;
	}

	public bool ReadBoolean()
	{
		bool value = _buffer[_position] == 1;
		_position++;
		return value;
	}

	public byte ReadByte()
	{
		byte value = _buffer[_position];
		_position++;
		return value;
	}

	public short ReadInt16()
	{
		short value = BitConverter.ToInt16(_buffer.Slice(_position, sizeof(short)));
		_position += sizeof(short);
		return value;
	}

	public int ReadInt32()
	{
		int value = BitConverter.ToInt32(_buffer.Slice(_position, sizeof(int)));
		_position += sizeof(int);
		return value;
	}

	public long ReadInt64()
	{
		long value = BitConverter.ToInt64(_buffer.Slice(_position, sizeof(long)));
		_position += sizeof(long);
		return value;
	}

	public float ReadSingle()
	{
		float value = BitConverter.ToSingle(_buffer.Slice(_position, sizeof(float)));
		_position += sizeof(float);
		return value;
	}

	public string ReadNullTerminatedString(int length)
	{
		int nullTerminatorIndex = _buffer.Slice(_position, length).IndexOf((byte)0);
		if (nullTerminatorIndex == -1)
			nullTerminatorIndex = length;

		string value = Encoding.ASCII.GetString(_buffer.Slice(_position, nullTerminatorIndex));
		_position += length;
		return value;
	}

	public byte[] ReadBytes(int length)
	{
		byte[] value = _buffer.Slice(_position, length).ToArray();
		_position += length;
		return value;
	}
}
