/*
== IPPC - Inter-Process Procedure Calling ==
A neat tool used to call procedures exported by an external module.
The tool is also able to get the result from the call, this can be cast into a managed struct and used from there.

By Alden Viljoen
https://github.com/ald0s


== CFinder Summary ==
Exposes a rewrite of GetProcAddress that works remotely.
Reads from the target process' PE header to get access to the export table within, using this information, the address of the target address can be found.
*/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;

namespace IPPC {
    public unsafe partial class CIPPC {
        public IntPtr GetRemoteProcAddress(IntPtr ptrHandle, IntPtr hBaseAddress, string sProcName) {
            IMAGE_DOS_HEADER32 dosHeader = new IMAGE_DOS_HEADER32();

            uint uiSignature = 0;
            IMAGE_FILE_HEADER fileHeader = new IMAGE_FILE_HEADER();
            IMAGE_OPTIONAL_HEADER32 optHeader32 = new IMAGE_OPTIONAL_HEADER32();
            IMAGE_DATA_DIRECTORY exportDirectory = new IMAGE_DATA_DIRECTORY();
            IMAGE_EXPORT_DIRECTORY exportTable = new IMAGE_EXPORT_DIRECTORY();

            IntPtr ptrFunctionTable = IntPtr.Zero;
            IntPtr ptrNameTable = IntPtr.Zero;
            IntPtr ptrOrdinalTable = IntPtr.Zero;

            if (ptrHandle == IntPtr.Zero || hBaseAddress == IntPtr.Zero) {
                WriteError("Invalid call.");
                return IntPtr.Zero;
            }

            IntPtr ptrNumBytesRead = IntPtr.Zero;
            if (!ReadProcessMemory(ptrHandle, hBaseAddress,
                &dosHeader, Marshal.SizeOf(dosHeader), out ptrNumBytesRead)) {
                WriteError("Failed. Error code: " + Marshal.GetLastWin32Error().ToString());
                return IntPtr.Zero;
            }

            if (dosHeader.e_magic != 0x5A4D) {
                WriteError("Image is not a valid DLL. " + dosHeader.e_magic.ToString());
                return IntPtr.Zero;
            }

            if (!ReadProcessMemory(ptrHandle, hBaseAddress + (dosHeader.e_lfanew),
                &uiSignature, Marshal.SizeOf(uiSignature), out ptrNumBytesRead)) {
                WriteError("Failed. Error code: " + Marshal.GetLastWin32Error().ToString());
                return IntPtr.Zero;
            }

            if (uiSignature != 0x4550) {
                WriteError("Invalid NT signature...");
                return IntPtr.Zero;
            }

            if (!ReadProcessMemory(ptrHandle, hBaseAddress + (dosHeader.e_lfanew + Marshal.SizeOf(uiSignature)),
                &fileHeader, Marshal.SizeOf(fileHeader), out ptrNumBytesRead)) {
                WriteError("Failed. Error code: " + Marshal.GetLastWin32Error().ToString());
                return IntPtr.Zero;
            }

            if (!ReadProcessMemory(ptrHandle, hBaseAddress + (dosHeader.e_lfanew + Marshal.SizeOf(uiSignature) + Marshal.SizeOf(fileHeader)),
                &optHeader32, Marshal.SizeOf(optHeader32), out ptrNumBytesRead)) {
                WriteError("Failed. Error code: " + Marshal.GetLastWin32Error().ToString());
                return IntPtr.Zero;
            }

            if (optHeader32.NumberOfRvaAndSizes >= 1) {
                exportDirectory.VirtualAddress =
                    (optHeader32.ExportTable.VirtualAddress);
                exportDirectory.Size = (optHeader32.ExportTable.Size);
            } else {
                WriteError("No export table found.");
                return IntPtr.Zero;
            }

            if (!ReadProcessMemory(ptrHandle, hBaseAddress + (int)(exportDirectory.VirtualAddress),
                &exportTable, Marshal.SizeOf(exportTable), out ptrNumBytesRead)) {
                WriteError("Failed. Error code: " + Marshal.GetLastWin32Error().ToString());
                return IntPtr.Zero;
            }

            uint[] uiExportFuncTable = ReadUIntArray(ptrHandle, hBaseAddress + (int)(exportTable.AddressOfFunctions), (int)exportTable.NumberOfFunctions);
            uint[] uiExportNameTable = ReadUIntArray(ptrHandle, hBaseAddress + (int)(exportTable.AddressOfNames), (int)exportTable.NumberOfNames);
            short[] usExportOrdinalTable = ReadShortArray(ptrHandle, hBaseAddress + (int)(exportTable.AddressOfNameOrdinals), (int)exportTable.NumberOfNames);

            for (int i = 0; i < exportTable.NumberOfNames; i++) {
                if (ReadString(ptrHandle, hBaseAddress + (int)(uiExportNameTable[i])) == sProcName)
                    return new IntPtr((int)hBaseAddress + uiExportFuncTable[usExportOrdinalTable[i]]);
            }

            WriteError("Unable to find the provided procedure name!");
            return IntPtr.Zero;
        }
    }
}
