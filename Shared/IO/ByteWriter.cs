﻿/*
 * Created by SharpDevelop.
 * User: Matthew
 * Date: 8/8/2010
 * Time: 2:16 AM
 * 
 * Copyright 2012 Matthew Cash. All rights reserved.
 * 
 * Redistribution and use in source and binary forms, with or without modification, are
 * permitted provided that the following conditions are met:
 * 
 *    1. Redistributions of source code must retain the above copyright notice, this list of
 *       conditions and the following disclaimer.
 * 
 *    2. Redistributions in binary form must reproduce the above copyright notice, this list
 *       of conditions and the following disclaimer in the documentation and/or other materials
 *       provided with the distribution.
 * 
 * THIS SOFTWARE IS PROVIDED BY Matthew Cash ``AS IS'' AND ANY EXPRESS OR IMPLIED
 * WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
 * FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL Matthew Cash OR
 * CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR
 * CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR
 * SERVICES; LOSS OF USE, DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON
 * ANY THEORY OF LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
 * NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF
 * ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 * 
 * The views and conclusions contained in the software and documentation are those of the
 * authors and should not be interpreted as representing official policies, either expressed
 * or implied, of Matthew Cash.
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Tortoise.Shared.IO
{
	/// <summary>
	/// Writes to a byte array.
	/// </summary>
	public class ByteWriter
	{
		List<byte> _data;
		Encoding _encoder ;

        public int Length
        {
            get { return _data.Count; }
        }

		public ByteWriter()
		{
			_data = new List<byte>();
			_encoder = Encoding.Unicode;
		}
		
		public void Write(byte data)
		{
			_data.Add(data);
		}
		
		public void Write(byte[] data)
		{
			_data.AddRange(data);
		}

        public void Write(sbyte data)
        {
            _data.Add(Convert.ToByte(data));
        }
	
        public void Write(short data)
		{
			_data.AddRange(BitConverter.GetBytes(data));
		}
		
		public void Write(ushort data)
		{
			_data.AddRange(BitConverter.GetBytes(data));
		}
		public void Write(int data)
		{
			_data.AddRange(BitConverter.GetBytes(data));
		}
		public void Write(uint data)
		{
			_data.AddRange(BitConverter.GetBytes(data));
		}
		public void Write(long data)
		{
			_data.AddRange(BitConverter.GetBytes(data));
		}
		public void Write(ulong data)
		{
			_data.AddRange(BitConverter.GetBytes(data));
		}
		public void Write(float data)
		{
			_data.AddRange(BitConverter.GetBytes(data));
		}
		public void Write(double data)
		{
			_data.AddRange(BitConverter.GetBytes(data));
		}
		public void Write(string data)
		{
			Write(Convert.ToUInt16(data.Length));
			Write(_encoder.GetBytes(data));
		}
        public void Write(bool data)
        {
            _data.Add(Convert.ToByte(data));
        }

		public byte[] GetArray()
		{
			return _data.ToArray();
		}
	}
}
