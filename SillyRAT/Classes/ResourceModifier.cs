using System;
using System.Runtime.InteropServices;
using System.IO;
using System.Security;
// fixed by s-illy

namespace Server.Classes
{
    public static class ResourceModifier
    {
        [SuppressUnmanagedCodeSecurity]
        private class NativeMethods
        {
            [DllImport("kernel32.dll", SetLastError = true)]
            public static extern IntPtr BeginUpdateResource(string pFileName, [MarshalAs(UnmanagedType.Bool)] bool bDeleteExistingResources);

            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool UpdateResource(IntPtr hUpdate, IntPtr type, IntPtr name, short language, byte[] data, int dataSize);

            [DllImport("kernel32.dll", SetLastError = true)]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool EndUpdateResource(IntPtr hUpdate, [MarshalAs(UnmanagedType.Bool)] bool discard);
        }

        public static void ChangeIcon(string executablePath, string iconPath)
        {
            if (!File.Exists(executablePath) || !File.Exists(iconPath))
                throw new FileNotFoundException("Archivo no encontrado.");

            const uint RT_ICON = 3u;
            const uint RT_GROUP_ICON = 14u;

            var iconFile = IconFileHandler.LoadFromFile(iconPath);
            var hUpdate = NativeMethods.BeginUpdateResource(executablePath, false);

            if (hUpdate == IntPtr.Zero)
                throw new Exception("Error al abrir el recurso. Código: " + Marshal.GetLastWin32Error());

            var groupIconData = iconFile.CreateGroupIconData(1);
            if (!NativeMethods.UpdateResource(hUpdate, new IntPtr(RT_GROUP_ICON), new IntPtr(1), 0, groupIconData, groupIconData.Length))
                throw new Exception("Error al actualizar el grupo de íconos.");

            for (int i = 0; i < iconFile.ImageCount; i++)
            {
                var imageData = iconFile.GetImageData(i);
                if (!NativeMethods.UpdateResource(hUpdate, new IntPtr(RT_ICON), new IntPtr(1 + i), 0, imageData, imageData.Length))
                    throw new Exception("Error al actualizar icono #" + i);
            }

            if (!NativeMethods.EndUpdateResource(hUpdate, false))
                throw new Exception("Error al finalizar la modificación de recursos.");
        }

        private class IconFileHandler
        {
            private IconEntryData[] entryData;
            private byte[][] imageData;

            public int ImageCount => entryData.Length;

            public byte[] GetImageData(int index) => imageData[index];

            public static IconFileHandler LoadFromFile(string filePath)
            {
                byte[] fileBytes = File.ReadAllBytes(filePath);
                if (fileBytes.Length < 6)
                    throw new Exception("El archivo no es un ícono válido.");

                IconFileHandler instance = new IconFileHandler();
                instance.entryData = new IconEntryData[BitConverter.ToUInt16(fileBytes, 4)];
                instance.imageData = new byte[instance.entryData.Length][];

                int offset = 6;
                for (int i = 0; i < instance.entryData.Length; i++)
                {
                    instance.entryData[i] = new IconEntryData(fileBytes, offset);
                    instance.imageData[i] = new byte[instance.entryData[i].BytesInResource];
                    Buffer.BlockCopy(fileBytes, instance.entryData[i].ImageOffset, instance.imageData[i], 0, instance.entryData[i].BytesInResource);
                    offset += 16;
                }

                return instance;
            }

            public byte[] CreateGroupIconData(uint baseID)
            {
                byte[] data = new byte[6 + (entryData.Length * 14)];
                Buffer.BlockCopy(BitConverter.GetBytes((ushort)0), 0, data, 0, 2);
                Buffer.BlockCopy(BitConverter.GetBytes((ushort)1), 0, data, 2, 2);
                Buffer.BlockCopy(BitConverter.GetBytes((ushort)entryData.Length), 0, data, 4, 2);

                int offset = 6;
                for (int i = 0; i < entryData.Length; i++)
                {
                    entryData[i].WriteToGroupIcon(data, offset, (ushort)(baseID + i));
                    offset += 14;
                }

                return data;
            }
        }

        private struct IconEntryData
        {
            public byte Width, Height, ColorCount, Reserved;
            public ushort Planes, BitCount;
            public int BytesInResource, ImageOffset;

            public IconEntryData(byte[] buffer, int offset)
            {
                Width = buffer[offset];
                Height = buffer[offset + 1];
                ColorCount = buffer[offset + 2];
                Reserved = buffer[offset + 3];
                Planes = BitConverter.ToUInt16(buffer, offset + 4);
                BitCount = BitConverter.ToUInt16(buffer, offset + 6);
                BytesInResource = BitConverter.ToInt32(buffer, offset + 8);
                ImageOffset = BitConverter.ToInt32(buffer, offset + 12);
            }

            public void WriteToGroupIcon(byte[] buffer, int offset, ushort id)
            {
                buffer[offset] = Width;
                buffer[offset + 1] = Height;
                buffer[offset + 2] = ColorCount;
                buffer[offset + 3] = Reserved;
                Buffer.BlockCopy(BitConverter.GetBytes(Planes), 0, buffer, offset + 4, 2);
                Buffer.BlockCopy(BitConverter.GetBytes(BitCount), 0, buffer, offset + 6, 2);
                Buffer.BlockCopy(BitConverter.GetBytes(BytesInResource), 0, buffer, offset + 8, 4);
                Buffer.BlockCopy(BitConverter.GetBytes(id), 0, buffer, offset + 12, 2);
            }
        }
    }
}
