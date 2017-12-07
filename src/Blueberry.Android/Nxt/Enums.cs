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

    /// <summary>
    /// Specifes the output port.
    /// </summary>
    internal enum NxtOutputPorts : byte
    {
        /// <summary>
        /// Output port A.
        /// </summary>
        PortA,

        /// <summary>
        /// Output port B.
        /// </summary>
        PortB,

        /// <summary>
        /// Output port C.
        /// </summary>
        PortC,

        /// <summary>
        /// All output ports.
        /// </summary>
        All = 0xFF
    }

    /// <summary>
    /// Specifies the motor mode.
    /// </summary>
    public enum NxtMotorModes : byte
    {
        /// <summary>
        /// Nothing.
        /// </summary>
        None = 0x00,

        /// <summary>
        /// Turn on the motor.
        /// </summary>
        On = 0x01,

        /// <summary>
        /// Use run/brake mode instead of run/float in PWM.
        /// </summary>
        Brake = 0x02,

        /// <summary>
        /// Turns on regulation.
        /// </summary>
        Regulated = 0x04
    }

    /// <summary>
    /// Specifies the motor regulation mode.
    /// </summary>
    public enum NxtMotorRegulationModes : byte
    {
        /// <summary>
        /// No regulation will be enabled.
        /// </summary>
        Idle = 0x00,

        /// <summary>
        /// Power control will be enabled.
        /// </summary>
        MotorSpeed = 0x01,

        /// <summary>
        /// Synchronization will be enabled. Two motors need this flag for it to take effect.
        /// </summary>
        MotorSync = 0x02
    }

    /// <summary>
    /// Specifies the motor run state.
    /// </summary>
    public enum NxtMotorRunStates : byte
    {
        /// <summary>
        /// Output will be idle.
        /// </summary>
        Idle = 0x00,

        /// <summary>
        /// Output will ramp-up.
        /// </summary>
        RampUp = 0x10,

        /// <summary>
        /// Output will be running.
        /// </summary>
        Running = 0x20,

        /// <summary>
        /// Output will ramp-down.
        /// </summary>
        RampDown = 0x40
    }

    /// <summary>
    /// Specifies the input port.
    /// </summary>
    internal enum NxtInputPorts : byte
    {
        /// <summary>
        /// Input port 1.
        /// </summary>
        Port1,

        /// <summary>
        /// Input port 2.
        /// </summary>
        Port2,

        /// <summary>
        /// Input port 3.
        /// </summary>
        Port3,

        /// <summary>
        /// Input port 4.
        /// </summary>
        Port4,
    }

    /// <summary>
    /// Specifies the type of sensor.
    /// </summary>
    public enum NxtSensorTypes : byte
    {
        /// <summary>
        /// No sensor.
        /// </summary>
        None = 0x00,

        /// <summary>
        /// Touch sensor.
        /// </summary>
        Switch = 0x01,

        /// <summary>
        /// Temperature sensor.
        /// </summary>
        Temperature = 0x02,

        /// <summary>
        /// RCX light sensor.
        /// </summary>
        Reflection = 0x03,

        /// <summary>
        /// RCX rotation sensor.
        /// </summary>
        Angle = 0x04,

        /// <summary>
        /// NXT light sensor with LED illuminated.
        /// </summary>
        LightActive = 0x05,

        /// <summary>
        /// NXT light sensor with LED off.
        /// </summary>
        LightInactive = 0x06,

        /// <summary>
        /// NXT sound sensor in dB mode.
        /// </summary>
        SoundDB = 0x07,

        /// <summary>
        /// NXT sound sensor in dBA mode.
        /// </summary>
        SoundDBA = 0x08,

        /// <summary>
        /// Custom sensor.
        /// </summary>
        Custom = 0x09,

        /// <summary>
        /// NXT low speed passive I2C sensor.
        /// </summary>
        LowSpeed = 0x0A,

        /// <summary>
        /// NXT low speed active I2C sensor.
        /// </summary>
        LowSpeed9V = 0x0B,

        /// <summary>
        /// NXT high speed I2C sensor.
        /// </summary>
        HighSpeed = 0x0C,

        /// <summary>
        /// NXT color sensor in normal mode.
        /// </summary>
        ColorFull = 0x0D,

        /// <summary>
        /// NXT color sensor in red projector mode.
        /// </summary>
        ColorRed = 0x0E,

        /// <summary>
        /// NXT color sensor in green projector mode.
        /// </summary>
        ColorGreen = 0x0F,

        /// <summary>
        /// NXT color sensor in blue projector mode.
        /// </summary>
        ColorBlue = 0x10,

        /// <summary>
        /// NXT color sensor in the off state.
        /// </summary>
        ColorNone
    }

    /// <summary>
    /// Specifies the sensor mode.
    /// </summary>
    public enum NxtSensorModes : byte
    {
        /// <summary>
        /// Raw mode.
        /// </summary>
        Raw = 0x00,

        /// <summary>
        /// Boolean mode.
        /// </summary>
        Boolean = 0x20,

        /// <summary>
        /// Count transitions of boolean mode.
        /// </summary>
        TransitionCounter = 0x40,

        /// <summary>
        /// Count periods of up and down of boolean mode.
        /// </summary>
        PeriodCounter = 0x60,

        /// <summary>
        /// Percent mode.
        /// </summary>
        PercentFullScale = 0x80,

        /// <summary>
        /// Celsius mode for temperature sensors.
        /// </summary>
        Celsius = 0xA0,

        /// <summary>
        /// Fahrenheit mode for temperature sensors.
        /// </summary>
        Fahrenheit = 0xC0,

        /// <summary>
        /// Steps mode used for RCX rotation sensor only.
        /// </summary>
        AngleSteps = 0xE0,

        SlopeMask = 0x1F,
        ModeMask = 0xE0
    }
}
