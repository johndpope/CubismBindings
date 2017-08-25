/*
 * Copyright(c) Live2D Inc. All rights reserved.
 * 
 * Use of this source code is governed by the Live2D Open Software license
 * that can be found at http://live2d.com/eula/live2d-open-software-license-agreement_en.html.
 */

/* {{{autogeneratedwarning}}} */


using System;


namespace Live2D.Cubism.Core.Unmanaged
{
    {{#arrayviews}}
    /// <summary>
    /// {{{Name}}} array view.
    /// </summary>
    public sealed class CubismUnmanaged{{{Name}}}ArrayView
    {
        /// <summary>
        /// Array length of unmanaged buffer.
        /// </summary>
        public int Length { get; private set; }

        /// <summary>
        /// Return true if instance is valid.
        /// </summary>
        public bool IsValid { get { return (Address != IntPtr.Zero) && (Length > 0); } }

        /// <summary>
        /// Gets element at index.
        /// </summary>
        /// <param name="index">Index of array.</param>
        /// <returns>Element of array.</returns>
        public unsafe {{{type}}} this[int index]
        {
            get
            {
                var pointer = ({{{type}}}*)Address.ToPointer();


                // Assert instance is valid.
                if (!IsValid)
                {
                    throw new InvalidOperationException("Array is empty, or not valid.");
                }

                if ((index >= Length) || (index < 0))
                {
                    throw new IndexOutOfRangeException("Array index is out of range.");
                }


                return pointer[index];
            }

            set
            {
                var pointer = ({{{type}}}*)Address.ToPointer();


                // Assert instance is valid.
                if (!IsValid)
                {
                    throw new InvalidOperationException("Array is empty, or not valid.");
                }

                if ((index >= Length) || (index < 0))
                {
                    throw new IndexOutOfRangeException("Array index is out of range.");
                }


                pointer[index] = value;
            }
        }


         /// <summary>
        /// Unmanaged buffer address.
        /// </summary>
        private IntPtr Address { get; set; }

        #region Ctors

        /// <summary>
        /// Initializes instance.
        /// </summary>
        /// <param name="address">Unmanaged buffer address.</param>
        /// <param name="length">Length of unmanaged buffer (in types).</param>
        internal unsafe CubismUnmanaged{{{Name}}}ArrayView({{{type}}}* address, int length)
        {
            Address = new IntPtr(address);
            Length = length;
        }

        /// <summary>
        /// Initializes instance.
        /// </summary>
        /// <param name="address">Unmanaged buffer address.</param>
        /// <param name="length">Length of unmanaged buffer (in types).</param>
        internal CubismUnmanaged{{{Name}}}ArrayView(IntPtr address, int length)
        {
            Address = address;
            Length = length;
        }

        #endregion

        /// <summary>
        /// Reads data.
        /// </summary>
        /// <param name="buffer">Destination managed array.</param>
        public unsafe void Read({{{type}}}[] buffer)
        {
            var sourceAddress = ({{{type}}}*)Address.ToPointer();
            var destinationLength = buffer.Length;
            

            // Assert buffer.Length >= Length
            if (destinationLength < Length)
            {
                throw new InvalidOperationException("Destination buffer length must be larger than source buffer length.");
            }

            // Assert instance is valid.
            if (!IsValid)
            {
                throw new InvalidOperationException("Array is empty, or not valid.");
            }


            // Read data into managed.           
            fixed ({{{type}}}* destinationAddress = buffer)
            {
                for (var i = 0; i < Length; ++i)
                {
                    destinationAddress[i] = sourceAddress[i];
                }
            }
        }

        /// <summary>
        /// Writes data.
        /// </summary>
        /// <param name="buffer">Source managed array.</param>
        public unsafe void Write({{{type}}}[] buffer)
        {
            var sourceLength = buffer.Length;
            var destinationAddress = ({{{type}}}*)Address.ToPointer();


            // Assert both length.
            if (sourceLength > Length)
            {
                throw new InvalidOperationException("Destination buffer length must be larger than source buffer length.");
            }

            // Assert instance is valid.
            if (!IsValid)
            {
                throw new InvalidOperationException("Array is empty, or not valid.");
            }


            // Write data into unmanaged.
            fixed ({{{type}}}* sourceAddress = buffer)
            {
                for (var i = 0; i < sourceLength; ++i)
                {
                    destinationAddress[i] = sourceAddress[i];
                }
            }
        }
    }

    {{/arrayviews}}
}