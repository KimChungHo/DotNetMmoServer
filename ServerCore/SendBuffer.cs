﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace ServerCore
{
	public class SendBuffer
	{
		private byte[] _buffer;
		private int _usedSize = 0;

		public int FreeSize { get { return _buffer.Length - _usedSize; } }

		public SendBuffer(int chunkSize)
		{
			_buffer = new byte[chunkSize];
		}

		public ArraySegment<byte> Open(int reserveSize)
		{
			if(reserveSize > FreeSize)
			{
				return null;
			}

			return new ArraySegment<byte>(_buffer, _usedSize, reserveSize);
		}

		public ArraySegment<byte> Close(int usedSize)
		{
			ArraySegment<byte> segment = new ArraySegment<byte>(_buffer, _usedSize, usedSize);
			_usedSize += usedSize;

			return segment;
		}
	}
}
