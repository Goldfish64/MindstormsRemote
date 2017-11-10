/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
* File: Enums.cs
* 
* Copyright (c) 2016-2017 John Davis
*
* Permission is hereby granted, free of charge, to any person obtaining a
* copy of this software and associated documentation files (the "Software"),
* to deal in the Software without restriction, including without limitation
* the rights to use, copy, modify, merge, publish, distribute, sublicense,
* and/or sell copies of the Software, and to permit persons to whom the
* Software is furnished to do so, subject to the following conditions:
*
* The above copyright notice and this permission notice shall be included
* in all copies or substantial portions of the Software.
* 
* THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS
* OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
* FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL
* THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
* LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
* FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS
* IN THE SOFTWARE.
* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

namespace Blueberry.Nxt
{
    /// <summary>
    /// Command type opcodes.
    /// </summary>
    internal enum CommandTypes : byte
    {
        DirectCommand = 0x00,
        SystemCommand = 0x01,
        ReplyCommand = 0x02,

        DirectCommandNoReply = 0x80,
        SystemCommandNoReply = 0x81
    }

    /// <summary>
    /// System command opcodes.
    /// </summary>
    internal enum SystemCommands : byte
    {
        OpenRead = 0x80,
        OpenWrite = 0x81,
        Read = 0x82,
        Write = 0x83,
        Close = 0x84,
        Delete = 0x85,
        FindFirst = 0x86,
        FindNext = 0x87,
        GetFirmwareVersion = 0x88,
        OpenWriteLinear = 0x89,
        OpenReadLinear = 0x8A,
        OpenWriteData = 0x8B,
        OpenAppendData = 0x8C,
        CropDataFile = 0x0D,
        FindFirstModule = 0x90,
        FindNextModule = 0x91,
        CloseModuleHandle = 0x92,
        IOMapRead = 0x94,
        IOMapWrite = 0x95,
        BootSamba = 0x97,
        SetBrickName = 0x98,
        GetBluetoothAddress = 0x9A,
        GetDeviceInfo = 0x9B,
        DeleteUserFlash = 0xA0,
        PollCommandLength = 0xA1,
        PollCommand = 0xA2,
        RenameFile = 0xA3,
        BluetoothFactoryReset = 0xA4 // Here for reference only, cannot actually be sent via Bluetooth.
    }


    /// <summary>
    /// Direct command protocol opcodes.
    /// </summary>
    internal enum RemoteCommands : byte
    {
        StartProgram,
        StopProgram,
        PlaySoundFile,
        PlayTone,
        SetOutputState,
        SetInputMode, // 0x05
        GetOutputState,
        GetInputValues,
        ResetInputScaledValue,
        MessageWrite,
        ResetMotorPosition, // 0x0A
        GetBatteryLevel,
        StopSoundPlayback,
        KeepAlive,
        LsGetStatus,
        LsWrite,
        LsRead,
        GetCurrentProgramName,
        GetButtonState,
        MessageRead,
        Reserved1,
        Reserved2,
        Reserved3,
        Reserved4,
        Reserved5,
        DatalogRead,
        DatalogSetTimes,
        BtGetContactCount,
        BtGetContactName,
        BtGetConnectionCount,
        BtGetConnectionName,
        SetProperty,
        GetProperty,
        UpdateResetCount
    }

    /// <summary>
    /// Selectors for RC Get and Set properties.
    /// </summary>
    internal enum Properties : byte
    {
        BtOnOff,
        SoundLevel,
        SleepTimeout
    }

    /// <summary>
    /// Remote control ("direct commands") errors.
    /// </summary>
    internal enum CommandErrors : byte
    {
        /// <summary>
        /// Data contains out-of-range values.
        /// </summary>
        IllegalValue = 0xC0,

        /// <summary>
        /// Clearly insane packet.
        /// </summary>
        BadPacket = 0xBF,

        /// <summary>
        /// Unknown command opcode.
        /// </summary>
        UnknownCommand = 0xBE,

        /// <summary>
        /// Request failed (i.e. specified file not found).
        /// </summary>
        Failed = 0xBD
    }
}
