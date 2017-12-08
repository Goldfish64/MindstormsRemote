/* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *
* File: NxtMotor.cs
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
    /// Represents an NXT motor.
    /// </summary>
    public class NxtMotor : NxtDevice
    {
        #region Private variables

        private NxtOutputPorts port;
        private NxtGetOutputStateResponse? pollingData;

        #endregion

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="NxtMotor"/> class.
        /// </summary>
        public NxtMotor() { }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the number of degrees the motor has turned since the last reset and poll. A null value indicates the motor has not yet been polled.
        /// </summary>
        public int? TachoCount
        {
            get { return pollingData?.TachoCount; }
        }

        /// <summary>
        /// Gets the number of complete rotations the motor has turned since the last reset and poll. A null value indicates the motor has not yet been polled.
        /// </summary>
        public int? RotationCount
        {
            get { return pollingData?.RotationCount; }
        }

        /// <summary>
        /// Gets the motor's value as a string.
        /// </summary>
        public override string Value => "";

        /// <summary>
        /// Gets the motor's friendly name.
        /// </summary>
        public override string FriendlyName => "Motor";

        #endregion

        #region Methods

        /// <summary>
        /// Attaches the motor to an <see cref="NxtBrick"/>.
        /// </summary>
        /// <param name="brick">The <see cref="NxtBrick"/> to use.</param>
        /// <param name="port">The port the motor is on.</param>
        internal void Attach(NxtBrick brick, NxtOutputPorts port)
        {
            // Save brick and port.
            this.brick = brick;
            this.port = port;
        }

        /// <summary>
        /// Detaches the motor.
        /// </summary>
        internal virtual void Detach()
        {
            PollingInterval = 0;
            brick = null;
            port = 0;
        }

        /// <summary>
        /// Polls the motor.
        /// </summary>
        public override void Poll()
        {
            // Get motor information.
            pollingData = brick.GetOutputState(port);
            base.Poll();
        }

        private void RunMotor(sbyte powerLevel, NxtMotorRegulationModes regMode, sbyte turnRatio, uint tachoLimit, bool brake)
        {
            // Start motor.
            if (brake)
                brick.SetOutputState(port, powerLevel, NxtMotorModes.On | NxtMotorModes.Brake | NxtMotorModes.Regulated, regMode, turnRatio, NxtMotorRunStates.Running, tachoLimit);
            else
                brick.SetOutputState(port, powerLevel, NxtMotorModes.On | NxtMotorModes.Regulated, regMode, turnRatio, NxtMotorRunStates.Running, tachoLimit);
        }

        /// <summary>
        /// Starts the motor forwards, with an optional tacho limit.
        /// </summary>
        /// <param name="powerLevel">The power level to set the motor at. Valid range is 0 to 100.</param>
        /// <param name="tachoLimit">The maximum number of degrees the motor should run forward.</param>
        /// <param name="brake">Specify true to brake the motor when the tacho limit is reached.</param>
        public void OnForward(byte powerLevel, uint tachoLimit = 0, bool brake = false)
        {
            OnForward(powerLevel, NxtMotorRegulationModes.MotorSpeed, 0, tachoLimit, brake);
        }

        /// <summary>
        /// Starts the motor forwards with the specified regulation mode, with an optional tacho limit.
        /// </summary>
        /// <param name="powerLevel">The power level to set the motor at. Valid range is 0 to 100.</param>
        /// <param name="regMode">The regulation mode to use.</param>
        /// <param name="turnRatio">The percent amount to turn.</param>
        /// <param name="tachoLimit">The maximum number of degrees the motor should run forward.</param>
        /// <param name="brake">Specify true to brake the motor when the tacho limit is reached.</param>
        public void OnForward(byte powerLevel, NxtMotorRegulationModes regMode, sbyte turnRatio = 0, uint tachoLimit = 0, bool brake = false)
        {
            // If power is set to 0, just turn off the motor.
            if (powerLevel == 0)
            {
                if (brake)
                    Off();
                else
                    Coast();
                return;
            }

            // Correct power level.
            if (powerLevel > 100)
                powerLevel = 100;

            // Start motor forwards.
            RunMotor((sbyte)powerLevel, regMode, turnRatio, tachoLimit, brake);
        }

        /// <summary>
        /// Starts the motor backwards, with an optional tacho limit.
        /// </summary>
        /// <param name="powerLevel">The power level to set the motor at. Valid range is 0 to 100.</param>
        /// <param name="tachoLimit">The maximum number of degrees the motor should run backward.</param>
        /// <param name="brake">Specify true to brake the motor when the tacho limit is reached.</param>
        public void OnBackward(byte powerLevel, uint tachoLimit = 0, bool brake = false)
        {
            OnBackward(powerLevel, NxtMotorRegulationModes.MotorSpeed, 0, tachoLimit, brake);
        }

        /// <summary>
        /// Starts the motor backwards with the specified regulation mode, with an optional tacho limit.
        /// </summary>
        /// <param name="powerLevel">The power level to set the motor at. Valid range is 0 to 100.</param>
        /// <param name="regMode">The regulation mode to use.</param>
        /// <param name="turnRatio">The percent amount to turn.</param>
        /// <param name="tachoLimit">The maximum number of degrees the motor should run backward.</param>
        /// <param name="brake">Specify true to brake the motor when the tacho limit is reached.</param>
        public void OnBackward(byte powerLevel, NxtMotorRegulationModes regMode, sbyte turnRatio = 0, uint tachoLimit = 0, bool brake = false)
        {
            // If power is set to 0, just turn off the motor.
            if (powerLevel == 0)
            {
                if (brake)
                    Off();
                else
                    Coast();
                return;
            }

            // Correct power level.
            if (powerLevel > 100)
                powerLevel = 100;

            // Start motor backwards.
            RunMotor((sbyte)-powerLevel, regMode, turnRatio, tachoLimit, brake);
        }

        /// <summary>
        /// Stop and brake the motor.
        /// </summary>
        public void Off()
        {
            // Stop and hold motor in position.
            brick.SetOutputState(port, 0, NxtMotorModes.On | NxtMotorModes.Brake | NxtMotorModes.Regulated, NxtMotorRegulationModes.Idle, 0, NxtMotorRunStates.Running);
        }

        /// <summary>
        /// Turns off the motor, letting it coast.
        /// </summary>
        public void Coast()
        {
            // Stop and coast motor.
            brick.SetOutputState(port, 0, NxtMotorModes.None, NxtMotorRegulationModes.Idle, 0, NxtMotorRunStates.Idle);
        }

        public void Reset()
        {
            // Reset motor stats.
            brick.ResetMotorPosition(port);
        }

        #endregion
    }
}
