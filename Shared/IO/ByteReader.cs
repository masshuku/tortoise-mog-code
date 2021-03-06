﻿/*
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
using System.ComponentModel;
using System.IO;
using System.Text;

using StormLib;

namespace Tortoise.Shared.IO
{
    /// <summary>
    /// Reads from a byte array.
    /// </summary>
    public class ByteReader
    {
        private byte[] _array;
        private int _pos;
        private ByteConverter _bc;
        private Encoding _encoder;

        public int Avaliable
        {
            get { return _array.Length - _pos; }
        }


        public ByteReader(byte[] array, int offset, int length)
        {
            if (offset == 0 && array.Length == length)
            {
                _array = new byte[length];
                Array.Copy(array, offset, _array, 0, length);
            }
            else
            {
                _array = array;
            }
            Init();
        }

        public ByteReader(byte[] array)
            :this(array, 0, array.Length)
        {
            
        }

        public ByteReader(Stream stream, int offset, int length)
        {
            _array = new byte[length];
            stream.Read(_array, offset, length);
            Init();
        }

        public ByteReader(BinaryReader stream, int length)
        {
            _array = new byte[length];
            _array = stream.ReadBytes(length);
            Init();
        }

        public void LoadNewArray(byte[] array)
        {
            _pos = 0;
            _array = array;
        }

        public byte[] DumpDebugInfo()
        {
            List<byte> debug = new List<byte>();
            debug.AddRange(BitConverter.GetBytes(_pos));
            debug.AddRange(_array);
            return debug.ToArray();            
        }

        private void Init()
        {
            _encoder = Encoding.Unicode;
            _pos = 0;
            _bc = new ByteConverter();
        }


        private ExecutionState EnforceLength(int length)
        {
            if (_pos + length > _array.Length) return ExecutionState.Failed();
            return ExecutionState.Succeeded();
        }

        public ExecutionState<byte> ReadByte()
        {
            if (!EnforceLength(sizeof(byte))) return ExecutionState<byte>.Failed("Failed to enforce length of byte");
            return ExecutionState<byte>.Succeeded(_array[_pos++]);
        }

        public ExecutionState<byte[]> ReadBytes(int length)
        {
            if (!EnforceLength(sizeof(byte) * length)) return ExecutionState<byte[]>.Failed("Failed to enforce length of byte[]");
            byte[] returnValue = new byte[length];
            Array.Copy(_array, _pos, returnValue, 0, length);
            _pos++;
            return ExecutionState<byte[]>.Succeeded(returnValue);
        }

        public ExecutionState<sbyte> ReadSByte()
        {
            if (!EnforceLength(sizeof(sbyte))) return ExecutionState<sbyte>.Failed("Failed to enforce length of sbyte");
            return ExecutionState<sbyte>.Succeeded(Convert.ToSByte(_array[_pos++]));
        }

        public ExecutionState<short> ReadShort()
        {
            if (!EnforceLength(sizeof(ushort))) return ExecutionState<short>.Failed("Failed to enforce length of short");
            var result = ExecutionState<short>.Succeeded(BitConverter.ToInt16(_array, _pos));
            _pos += sizeof(ushort);
            return result;
        }

        public ExecutionState<ushort> ReadUShort()
        {
            if (!EnforceLength(sizeof(ushort))) return ExecutionState<ushort>.Failed("Failed to enforce length of ushort");
            var result = ExecutionState<ushort>.Succeeded(BitConverter.ToUInt16(_array, _pos));
            _pos += sizeof(ushort);
            return result;
        }

        public ExecutionState<int> ReadInt()
        {
            if (!EnforceLength(sizeof(int))) return ExecutionState<int>.Failed("Failed to enforce length of int");
            var result = ExecutionState<int>.Succeeded(BitConverter.ToInt32(_array, _pos));
            _pos += sizeof(int);
            return result;
        }

        public ExecutionState<uint> ReadUInt()
        {
            if (!EnforceLength(sizeof(uint))) return ExecutionState<uint>.Failed("Failed to enforce length of uint");
            var result = ExecutionState<uint>.Succeeded(BitConverter.ToUInt32(_array, _pos));
            _pos += sizeof(uint);
            return result;
        }

        public ExecutionState<long> ReadLong()
        {
            if (!EnforceLength(sizeof(long))) return ExecutionState<long>.Failed("Failed to enforce length of long");
            var result = ExecutionState<long>.Succeeded(BitConverter.ToInt64(_array, _pos));
            _pos += sizeof(long);
            return result;
        }

        public ExecutionState<ulong> ReadULong()
        {
            if (!EnforceLength(sizeof(ulong))) return ExecutionState<ulong>.Failed("Failed to enforce length of ulong");
            var result = ExecutionState<ulong>.Succeeded(BitConverter.ToUInt64(_array, _pos));
            _pos += sizeof(ulong);
            return result;
        }



        public ExecutionState<float> ReadSingle()
        {
            if (!EnforceLength(sizeof(float))) return ExecutionState<float>.Failed("Failed to enforce length of float");
            var result = ExecutionState<float>.Succeeded(BitConverter.ToSingle(_array, _pos));
            _pos += sizeof(float);
            return result;
        }

        public ExecutionState<double> ReadDouble()
        {
            if (!EnforceLength(sizeof(double))) return ExecutionState<double>.Failed("Failed to enforce length of double");
            var result = ExecutionState<double>.Succeeded(BitConverter.ToDouble(_array, _pos));
            _pos += sizeof(double);
            return result;
        }

        public ExecutionState<bool> ReadBoolean()
        {
            if (!EnforceLength(sizeof(bool))) return ExecutionState<bool>.Failed("Failed to enforce length of bool");
            var result = ExecutionState<bool>.Succeeded(BitConverter.ToBoolean(_array, _pos));
            _pos += sizeof(bool);
            return result;
        }

        public ExecutionState<string> ReadString()
        {
            var stateLen = ReadUShort();
            if (!stateLen) return ExecutionState<string>.Failed("Failed to read Length");
            ushort length = stateLen.Result;
            var stateData = ReadBytes(length);
            if (!stateData) return ExecutionState<string>.Failed("Failed to read Data off of length");
            return ExecutionState<string>.Succeeded(_encoder.GetString(stateData.Result));
        }
    }
}
