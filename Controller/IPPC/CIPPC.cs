/*
== IPPC - Inter-Process Procedure Calling ==
A neat tool used to call procedures exported by an external module.
The tool is also able to get the result from the call, this can be cast into a managed struct and used from there.

By Alden Viljoen
https://github.com/ald0s

== CIPPC Summary ==
Exposes the low level functionality used in interacting with the WinAPI directly, also provides a lot of functionality for reading/writing managed and unmanaged memory.
*/

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace IPPC {
    public unsafe partial class CIPPC : CDebugWrapper {
        /// <summary>
        /// Run a procedure in a foreign process, given a handle to it.
        /// </summary>
        /// <param name="ptrHandle">The handle to the foreign process.</param>
        /// <param name="ptrFunction">The address of the foreign function.</param>
        /// <param name="ptrArgument">The argument for the function.</param>
        /// <returns>If the return value is not IntPtr.Zero, the procedure most likely succeeded.</returns>
        public IntPtr Run(IntPtr ptrHandle, IntPtr ptrFunction, IntPtr ptrArgument) {
            try {
                IntPtr ptrThreadHandle,
                    ptrThreadID;
                if ((ptrThreadHandle = CreateRemoteThread(ptrHandle, IntPtr.Zero, 0, ptrFunction, ptrArgument, 0, out ptrThreadID)) == IntPtr.Zero) {
                    WriteError("Failed to Run(), create remote thread returned 0");
                    return IntPtr.Zero;
                }

                return ptrThreadHandle;
            } catch (Exception e) {
                ReportException(e);
                return IntPtr.Zero;
            }
        }

        /// <summary>
        /// Writes a string to a foreign process and returns the location.
        /// </summary>
        /// <param name="ptrHandle">A handle to the foreign process.</param>
        /// <param name="sInput">The string to write.</param>
        /// <returns>A pointer to the string.</returns>
        public IntPtr WriteString(IntPtr ptrHandle, string sInput) {
            try {
                byte[] btInput = Encoding.ASCII.GetBytes(sInput);
                return WriteByteArray(ptrHandle, btInput);
            } catch (Exception e) {
                ReportException(e);
                return IntPtr.Zero;
            }
        }

        /// <summary>
        /// Writes an array of bytes (8 bit) to the process specified by ptrHandle.
        /// </summary>
        /// <param name="ptrHandle">A handle to the foreign process.</param>
        /// <param name="btArray">The byte array to write.</param>
        /// <returns>A pointer (in the external process) to where the data was written.</returns>
        public IntPtr WriteByteArray(IntPtr ptrHandle, byte[] btArray) {
            try {
                IntPtr ptrForeignMemory = VirtualAllocEx(ptrHandle, IntPtr.Zero, (uint)btArray.Length, 0x1000, 0x4);
                if (ptrForeignMemory == IntPtr.Zero) {
                    WriteError("Failed to WriteString(), couldn't allocate memory.");
                    return IntPtr.Zero;
                }

                IntPtr ptrNumBytesWritten = IntPtr.Zero;
                if (!WriteProcessMemory(ptrHandle, ptrForeignMemory, btArray, btArray.Length, out ptrNumBytesWritten)) {
                    WriteError("Failed to WriteString(), WriteProcessMemory failed!");
                    return IntPtr.Zero;
                }

                if (ptrNumBytesWritten.ToInt32() != btArray.Length) {
                    WriteInfo("WriteString() writing string didn't match bytes written with length!");
                }

                return ptrForeignMemory;
            } catch (Exception e) {
                ReportException(e);
                return IntPtr.Zero;
            }
        }

        /// <summary>
        /// Writes a struct to a specified foreign process and returns its location in memory.
        /// </summary>
        /// <param name="ptrHandle">A handle to the target process.</param>
        /// <param name="oObject">The managed structure to write.</param>
        /// <returns>The location in foreign memory of the structure.</returns>
        public IntPtr WriteStruct(IntPtr ptrHandle, object oObject) {
            try {
                int iSize = Marshal.SizeOf(oObject);
                IntPtr ptrLocalMemory = Marshal.AllocHGlobal(iSize);
                Marshal.StructureToPtr(oObject, ptrLocalMemory, false);

                IntPtr ptrForeignMemory = VirtualAllocEx(ptrHandle, IntPtr.Zero, (uint)iSize, 0x1000, 0x4);
                if (ptrForeignMemory == IntPtr.Zero) {
                    WriteError("Failed to allocate remote memory.");
                    Marshal.FreeHGlobal(ptrLocalMemory);

                    return IntPtr.Zero;
                }

                IntPtr ptrBytesWritten = IntPtr.Zero;
                if (!WriteProcessMemory(ptrHandle, ptrForeignMemory, ptrLocalMemory, iSize, out ptrBytesWritten)) {
                    WriteError("Faild to write structure to foreign process.");
                    Marshal.FreeHGlobal(ptrLocalMemory);

                    return IntPtr.Zero;
                }

                WriteSuccess("Wrote structure to foreign process. (" + ptrBytesWritten.ToInt32().ToString() + " bytes)");
                Marshal.FreeHGlobal(ptrLocalMemory);
                return ptrForeignMemory;
            } catch (Exception e) {
                ReportException(e);
                return IntPtr.Zero;
            }
        }

        /// <summary>
        /// Reads a structure from a specified memory location in a target process.
        /// </summary>
        /// <param name="ptrHandle">A handle to the target process.</param>
        /// <param name="ptrMemory">The memory block to read from.</param>
        /// <param name="oObject">The structure (can be blank) to read.</param>
        /// <returns>A NEW structure representing the foreign memory block.</returns>
        public object ReadStruct(IntPtr ptrHandle, IntPtr ptrMemory, object oObject) {
            try {
                int iSize = Marshal.SizeOf(oObject);

                IntPtr ptrLocalMemory = Marshal.AllocHGlobal(iSize);

                IntPtr ptrNumBytesRead = IntPtr.Zero;
                if (!ReadProcessMemory(ptrHandle, ptrMemory, ptrLocalMemory, iSize, out ptrNumBytesRead)) {
                    WriteError("Failed to read process memory.");
                    Marshal.FreeHGlobal(ptrLocalMemory);

                    return null;
                }

                object oResult = Marshal.PtrToStructure(ptrLocalMemory, oObject.GetType());
                Marshal.FreeHGlobal(ptrLocalMemory);

                return oResult;
            } catch (Exception e) {
                ReportException(e);
                return null;
            }
        }

        /// <summary>
        /// Reads an array of an unknown type.
        /// The programmer may specify the size of the target data.
        /// </summary>
        /// <param name="ptrHandle">A handle to the target process.</param>
        /// <param name="ptrBase">A pointer to the base of the memory location.</param>
        /// <param name="count">The number of elements to read.</param>
        /// <param name="sz">The size of each element.</param>
        /// <returns>An array of arrays containing the target data.</returns>
        public byte[][] ReadArrayMemory(IntPtr ptrHandle, IntPtr ptrBase, int count, int sz) {
            try {
                byte[][] result = new byte[count][];
                for(int i = 0; i < count; i++) {
                    IntPtr ptrTarget = (ptrBase + (i * sz));
                    result[i] = new byte[sz];

                    IntPtr ptrNumBytesRead = IntPtr.Zero;
                    if(!ReadProcessMemory(ptrHandle, ptrTarget, result[i], sz, out ptrNumBytesRead)) {
                        WriteError("Failed to read process memory.");
                        return null;
                    }
                }

                return result;
            }catch(Exception e) {
                ReportException(e);
                return null;
            }
        }

        /// <summary>
        /// Reads an array of unsigned integers from memory.
        /// </summary>
        /// <param name="ptrHandle">A handle to the target process.</param>
        /// <param name="ptrBase">A pointer to the base of the memory location.</param>
        /// <param name="iNum">The number of integers to read.</param>
        /// <returns>An array of unsigned integers read from the target memory.</returns>
        public uint[] ReadUIntArray(IntPtr ptrHandle, IntPtr ptrBase, int iNum) {
            uint[] result = new uint[iNum];
            byte[][] uints = ReadArrayMemory(ptrHandle, ptrBase, iNum, sizeof(uint));

            for(int i = 0; i < iNum; i++) {
                result[i] = BitConverter.ToUInt32(uints[i], 0);
            }

            return result;
        }

        /// <summary>
        /// Reads an array of 16 bit integers from memory.
        /// </summary>
        /// <param name="ptrHandle">A handle to the target process.</param>
        /// <param name="ptrBase">A pointer to the base of the memory location.</param>
        /// <param name="iNum">The number of integers to read.</param>
        /// <returns>An array of integers read from the target memory.</returns>
        public short[] ReadShortArray(IntPtr ptrHandle, IntPtr ptrBase, int iNum) {
            short[] result = new short[iNum];
            byte[][] shorts = ReadArrayMemory(ptrHandle, ptrBase, iNum, sizeof(short));

            for (int i = 0; i < iNum; i++) {
                result[i] = BitConverter.ToInt16(shorts[i], 0);
            }

            return result;
        }

        /// <summary>
        /// Gets the value returned by a thread shutting down.
        /// For example, returns the handle to a module mounted by LoadLibraryA()
        /// </summary>
        /// <param name="ptrThreadHandle">The return value of CreateRemoteThread()</param>
        /// <returns>The return value of the function encapsulated by the thread.</returns>
        public uint GetThreadReturnValue(IntPtr ptrThreadHandle) {
            try {
                uint uiReturn = WaitForSingleObject(ptrThreadHandle, 60000);
                if (uiReturn != 0x00000000L) {
                    WriteError("Waiting for object failed.");
                    return 0;
                }

                uint uiExitCode = 0;
                bool bSuccess = GetExitCodeThread(ptrThreadHandle, out uiExitCode);

                if (!bSuccess) {
                    WriteError("Failed to get exit code.");
                    return 0;
                }

                WriteSuccess("Discovered return for " + ptrThreadHandle.ToString() + " is " + uiExitCode.ToString());
                return uiExitCode;
            } catch (Exception e) {
                ReportException(e);
                return 0;
            }
        }

        /// <summary>
        /// Reads a NULL terminated string from a foreign process.
        /// </summary>
        /// <param name="ptrHandle">A handle to the target process.</param>
        /// <param name="ptrBase">Location in memory of the first character.</param>
        /// <returns>The string.</returns>
        public string ReadString(IntPtr ptrHandle, IntPtr ptrBase) {
            try {
                string result = "";
                int counter = 0;

                while (true) {
                    byte btChar = 0;
                    IntPtr ptrNumBytesRead = IntPtr.Zero;

                    if (!ReadProcessMemory(ptrHandle, ptrBase + counter, &btChar, sizeof(byte), out ptrNumBytesRead) ||
                        ptrNumBytesRead.ToInt32() != sizeof(byte)) {

                        WriteError("Error occurred while reading string.");
                        return null;
                    }

                    if (btChar == 0)
                        break;

                    counter++;
                    result += Encoding.ASCII.GetString(new byte[] { btChar });
                }

                return result;
            } catch (Exception e) {
                ReportException(e);
                return null;
            }
        }

        /// <summary>
        /// Reads a string from the target, if the string length is known.
        /// </summary>
        /// <param name="ptrHandle">A handle to the target process.</param>
        /// <param name="ptrBase">Location in memory of the first character.</param>
        /// <param name="count">The amount of bytes to read.</param>
        /// <returns>The result string.</returns>
        public string ReadString(IntPtr ptrHandle, IntPtr ptrBase, int count) {
            IntPtr ptrNumBytesRead = IntPtr.Zero;

            byte[] array = new byte[count];
            if(!ReadProcessMemory(ptrHandle, ptrBase, array, sizeof(char) * count, out ptrNumBytesRead)) {
                WriteError("Error occurred while reading string.");
                return null;
            }

            return Encoding.ASCII.GetString(array);
        }

        /// <summary>
        /// Frees memory at location ptrMemory.
        /// </summary>
        /// <param name="ptrHandle">A handle to the foreign process.</param>
        /// <param name="ptrMemory">The memory to free, previous allocated with VirtualAlloc.</param>
        public void FreeMemory(IntPtr ptrHandle, IntPtr ptrMemory) {
            try {
                VirtualFreeEx(ptrHandle, ptrMemory, 0, 0x8000);
            } catch (Exception e) {
                ReportException(e);
            }
        }

        /// <summary>
        /// Loads a library into a foreign process by LoadLibraryA()
        /// NOTE: When writing the path, possibly have to explicity write terminating NULL character.
        /// </summary>
        /// <param name="ptrHandle">A handle to the target process.</param>
        /// <param name="sAbsolutePath">The absolute path to the module on the filesystem.</param>
        /// <returns>A handle to the newly created thread.</returns>
        public IntPtr LoadLibrary(IntPtr ptrHandle, string sAbsolutePath) {
            try {
                if (!File.Exists(sAbsolutePath)) {
                    WriteError("Failed to write module. (File does not exist.)");
                    return IntPtr.Zero;
                }

                byte[] btPath = Encoding.Unicode.GetBytes(sAbsolutePath);

                IntPtr ptrLoadLibrary = GetProcAddress(GetModuleHandle("kernel32.dll"), "LoadLibraryW");
                if (ptrLoadLibrary == IntPtr.Zero) {
                    WriteError("Failed to find load library.");
                    return IntPtr.Zero;
                }

                IntPtr ptrForeignPath = VirtualAllocEx(ptrHandle, IntPtr.Zero, (uint)btPath.Length, 0x1000, 0x4);
                if (ptrForeignPath == IntPtr.Zero) {
                    WriteError("Failed to allocate foreign path.");
                    return IntPtr.Zero;
                }

                IntPtr ptrBytesWritten = IntPtr.Zero;
                if (!WriteProcessMemory(ptrHandle, ptrForeignPath, btPath, btPath.Length, out ptrBytesWritten)) {
                    WriteError("Failed to write path.");

                    return IntPtr.Zero;
                }
                WriteSuccess("Wrote module path to foreign process (" + ptrBytesWritten.ToInt32() + " bytes)");

                IntPtr ptrThreadHandle = Run(ptrHandle, ptrLoadLibrary, ptrForeignPath);
                if (ptrThreadHandle == IntPtr.Zero) {
                    return IntPtr.Zero;
                }

                WriteSuccess("Successfully created a thread.");

                return ptrThreadHandle;
            } catch (Exception e) {
                ReportException(e);
                return IntPtr.Zero;
            }
        }

        /// <summary>
        /// Attempts to call FreeLibrary() on the targetted process.
        /// </summary>
        /// <param name="ptrHandle">A handle to the foreign process.</param>
        /// <param name="ptrBaseAddress">The base address of a loaded module. (returned by LoadLibrary)</param>
        /// <returns></returns>
        public bool FreeLibrary(IntPtr ptrHandle, IntPtr ptrBaseAddress) {
            try {
                IntPtr ptrFreeLibrary = GetProcAddress(GetModuleHandle("kernel32.dll"), "FreeLibrary");
                if (ptrFreeLibrary == IntPtr.Zero) {
                    WriteError("Failed to find free library.");
                    return false;
                }

                IntPtr ret = Run(ptrHandle, ptrFreeLibrary, ptrBaseAddress);
                uint ui = GetThreadReturnValue(ret);
                Console.WriteLine(ui.ToString());

                return ret != IntPtr.Zero;
            } catch (Exception e) {
                ReportException(e);
                return false;
            }
        }
    }
}
