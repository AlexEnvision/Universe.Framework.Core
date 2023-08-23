//  ╔═════════════════════════════════════════════════════════════════════════════════╗
//  ║                                                                                 ║
//  ║   Copyright 2021 Universe.Framework.Core                                        ║
//  ║                                                                                 ║
//  ║   Licensed under the Apache License, Version 2.0 (the "License");               ║
//  ║   you may not use this file except in compliance with the License.              ║
//  ║   You may obtain a copy of the License at                                       ║
//  ║                                                                                 ║
//  ║       http://www.apache.org/licenses/LICENSE-2.0                                ║
//  ║                                                                                 ║
//  ║   Unless required by applicable law or agreed to in writing, software           ║
//  ║   distributed under the License is distributed on an "AS IS" BASIS,             ║
//  ║   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.      ║
//  ║   See the License for the specific language governing permissions and           ║
//  ║   limitations under the License.                                                ║
//  ║                                                                                 ║
//  ║                                                                                 ║
//  ║   Copyright 2021 Universe.Framework.Core                                        ║
//  ║                                                                                 ║
//  ║   Лицензировано согласно Лицензии Apache, Версия 2.0 ("Лицензия");              ║
//  ║   вы можете использовать этот файл только в соответствии с Лицензией.           ║
//  ║   Вы можете найти копию Лицензии по адресу                                      ║
//  ║                                                                                 ║
//  ║       http://www.apache.org/licenses/LICENSE-2.0.                               ║
//  ║                                                                                 ║
//  ║   За исключением случаев, когда это регламентировано существующим               ║
//  ║   законодательством или если это не оговорено в письменном соглашении,          ║
//  ║   программное обеспечение распространяемое на условиях данной Лицензии,         ║
//  ║   предоставляется "КАК ЕСТЬ" и любые явные или неявные ГАРАНТИИ ОТВЕРГАЮТСЯ.    ║
//  ║   Информацию об основных правах и ограничениях,                                 ║
//  ║   применяемых к определенному языку согласно Лицензии,                          ║
//  ║   вы можете найти в данной Лицензии.                                            ║
//  ║                                                                                 ║
//  ╚═════════════════════════════════════════════════════════════════════════════════╝

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Universe.Types.Collection
{
    /// <summary>
    /// <author>Alex Envision</author>
    /// </summary>
    public class BigArray<T> : IEnumerable<T>, IEnumerable
    {
        private T[][] _data;
        private long _blockSize;

        public BigArray(long length)
        {
            this.Allocate(length);
        }

        public long Length { get; private set; }

        public long BlockSize
        {
            get
            {
                return this._blockSize;
            }
        }

        public long IndexOf(T item)
        {
            return this.IndexOf(item, 0L, this.Length);
        }

        public long IndexOf(T item, long startIndex)
        {
            return this.IndexOf(item, startIndex, this.Length - startIndex);
        }

        public long IndexOf(T item, long startIndex, long count)
        {
            long num = -1;
            if (startIndex < 0L || startIndex > this.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            if (count < 0L || count > this.Length - startIndex)
                throw new ArgumentOutOfRangeException(nameof(count));
            long index1 = startIndex / this._blockSize;
            int startIndex1 = (int)(startIndex % this._blockSize);
            count += startIndex;
            for (long index2 = startIndex; index2 < count && index1 < (long)this._data.Length; ++index1)
            {
                int count1 = this._data[index1].Length;
                if (index2 + (long)count1 > count)
                    count1 = (int)(count - index2);
                num = (long)Array.IndexOf<T>(this._data[index1], item, startIndex1, count1);
                startIndex1 = 0;
                if (num != -1L)
                {
                    num += index1 * this._blockSize;
                    break;
                }
                index2 += (long)count1;
            }
            return num;
        }

        public void Clear()
        {
            this.Clear(0L, this.Length);
        }

        public void Clear(long startIndex, long count)
        {
            if (startIndex < 0L || startIndex > this.Length)
                throw new ArgumentOutOfRangeException(nameof(startIndex));
            if (count < 0L || count > this.Length - startIndex)
                throw new ArgumentOutOfRangeException(nameof(count));
            long index1 = startIndex / this._blockSize;
            int index2 = (int)(startIndex % this._blockSize);
            count += startIndex;
            for (long index3 = startIndex; index3 < count && index1 < (long)this._data.Length; ++index1)
            {
                int length = this._data[index1].Length;
                if (index3 + (long)length > count)
                    length = (int)(count - index3);
                Array.Clear((Array)this._data[index1], index2, length);
                index2 = 0;
                index3 += (long)length;
            }
        }

        public void Resize(long newSize)
        {
            if (newSize == this.Length)
                return;
            int newSize1 = (int)(newSize / this._blockSize);
            if (newSize > (long)newSize1 * this._blockSize)
                ++newSize1;
            int length = this._data.Length;
            int newSize2 = (int)(newSize - (long)(newSize1 - 1) * this._blockSize);
            int num = (int)(this.Length - (long)(newSize1 - 1) * this._blockSize);
            if (length != newSize1)
            {
                if (length < newSize1)
                {
                    if ((long)num != this._blockSize)
                        Array.Resize<T>(ref this._data[length - 1], (int)this._blockSize);
                    Array.Resize<T[]>(ref this._data, newSize1);
                    for (int index = length; index < newSize1 - 1; ++index)
                        this._data[length] = new T[this._blockSize];
                    this._data[newSize1 - 1] = new T[newSize2];
                }
                else
                {
                    Array.Resize<T[]>(ref this._data, newSize1);
                    Array.Resize<T>(ref this._data[newSize1 - 1], newSize2);
                }
            }
            else
                Array.Resize<T>(ref this._data[newSize1 - 1], newSize2);
            this.Length = newSize;
        }

        public void CopyTo(long index, T[] destinationArray, long count)
        {
            this.CopyTo(index, destinationArray, 0, count);
        }

        public void CopyTo(long index, T[] destinationArray, int destinationIndex, long count)
        {
            if (destinationArray == null)
                throw new ArgumentNullException(nameof(destinationArray));
            if (index < 0L || index > this.Length)
                throw new ArgumentOutOfRangeException(nameof(index));
            if (destinationIndex < 0 || destinationIndex > destinationArray.Length)
                throw new ArgumentOutOfRangeException(nameof(destinationIndex));
            if (count < 0L || count > this.Length - index || count > (long)(destinationArray.Length - destinationIndex))
                throw new ArgumentOutOfRangeException(nameof(count));
            int num = destinationIndex;
            for (long index1 = index; index1 < index + count; ++index1)
                destinationArray[num++] = this[index1];
        }

        public T this[long index]
        {
            get
            {
                return this._data[(int)(index / this._blockSize)][index % this._blockSize];
            }
            set
            {
                this._data[(int)(index / this._blockSize)][index % this._blockSize] = value;
            }
        }

        public IEnumerator<T> GetEnumerator()
        {
            return ((IEnumerable<T[]>)this._data).SelectMany<T[], T>((Func<T[], IEnumerable<T>>)(t => (IEnumerable<T>)t)).GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return (IEnumerator)this.GetEnumerator();
        }

        private void Allocate(long length)
        {
            if (length < 0L)
                throw new ArgumentException("Must specify a length >= 0");
            this.Length = length;
            if (typeof(T).IsValueType)
            {
                this._blockSize = (long)(2147483591 / Marshal.SizeOf(typeof(T)));
            }
            else
            {
                int num = 8;
                if (!Environment.Is64BitProcess)
                    num = 4;
                this._blockSize = (long)(2147483591 / num - 1);
            }
            int length1 = (int)(length / this._blockSize);
            if (length > (long)length1 * this._blockSize)
                ++length1;
            this._data = new T[length1][];
            for (int index = 0; index < length1 - 1; ++index)
                this._data[index] = new T[this._blockSize];
            if (length1 <= 0)
                return;
            this._data[length1 - 1] = new T[length - (long)(length1 - 1) * this._blockSize];
        }
    }
}