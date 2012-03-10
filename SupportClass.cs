//
// In order to convert some functionality to Visual C#, the Java Language Conversion Assistant
// creates "support classes" that duplicate the original functionality.  
//
// Support classes replicate the functionality of the original code, but in some cases they are 
// substantially different architecturally. Although every effort is made to preserve the 
// original architecture of the application in the converted project, the user should be aware that 
// the primary goal of these support classes is to replicate functionality, and that at times 
// the architecture of the resulting solution may differ somewhat.
//

using System;
using System.Collections;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace circuit_emulator
{
    /// <summary>
    /// This interface should be implemented by any class whose instances are intended 
    /// to be executed by a thread.
    /// </summary>
    public interface IThreadRunnable
    {
        /// <summary>
        /// This method has to be implemented in order that starting of the thread causes the object's 
        /// run method to be called in that separately executing thread.
        /// </summary>
        void Run();
    }

    /// <summary>
    /// Contains conversion support elements such as classes, interfaces and static methods.
    /// </summary>
    public class SupportClass
    {
        /*******************************/

        /// <summary>
        /// Calculates the ascent of the font, using the GetCellAscent and GetEmHeight methods
        /// </summary>
        /// <param name="font">The Font instance used to obtain the Ascent</param>
        /// <returns>The ascent of the font</returns>
        public static int GetAscent(Font font)
        {
            FontFamily fontFamily = font.FontFamily;
            int ascent = fontFamily.GetCellAscent(font.Style);
            int ascentPixel = (int) font.Size*ascent/fontFamily.GetEmHeight(font.Style);
            return ascentPixel;
        }

        /*******************************/

        /// <summary>
        /// Takes two arrays representing X-axis and Y-axis, and merges them into drawing points
        /// </summary>
        /// <param name="xPoints">Array of X-axis values</param>
        /// <param name="yPoints">Array of Y-axis values</param>
        /// <param name="nPoints">Number of points to create</param>
        /// <returns>Array of System.Drawing.Point</returns>
        public static Point[] GetPoints(int[] xPoints, int[] yPoints, int nPoints)
        {
            var points = new Point[nPoints];
            for (int index = 0; index < nPoints; index++)
            {
                points[index] = new Point(xPoints[index], yPoints[index]);
            }
            return points;
        }

        /*******************************/

        /*******************************/

        /// <summary>
        /// Adds the X and Y coordinates to the current graphics path.
        /// </summary>
        /// <param name="graphPath"> The current Graphics path</param>
        /// <param name="xCoordinate">The x coordinate to be added</param>
        /// <param name="yCoordinate">The y coordinate to be added</param>
        public static void AddPointToGraphicsPath(GraphicsPath graphPath, int x, int y)
        {
            var tempPointArray = new PointF[graphPath.PointCount + 1];
            var tempPointTypeArray = new byte[graphPath.PointCount + 1];

            if (graphPath.PointCount == 0)
            {
                tempPointArray[0] = new PointF(x, y);
                var tempGraphicsPath = new GraphicsPath(tempPointArray, new[] {(byte) PathPointType.Start});
                graphPath.AddPath(tempGraphicsPath, false);
            }
            else
            {
                graphPath.PathPoints.CopyTo(tempPointArray, 0);
                tempPointArray[graphPath.PointCount] = new Point(x, y);

                graphPath.PathTypes.CopyTo(tempPointTypeArray, 0);
                tempPointTypeArray[graphPath.PointCount] = (byte) PathPointType.Line;

                var tempGraphics = new GraphicsPath(tempPointArray, tempPointTypeArray);
                graphPath.Reset();
                graphPath.AddPath(tempGraphics, false);
                graphPath.CloseFigure();
            }
        }

        /*******************************/

        /// <summary>
        /// Calculates the descent of the font, using the GetCellDescent and GetEmHeight
        /// </summary>
        /// <param name="font">The Font instance used to obtain the Descent</param>
        /// <returns>The Descent of the font </returns>
        public static int GetDescent(Font font)
        {
            FontFamily fontFamily = font.FontFamily;
            int descent = fontFamily.GetCellDescent(font.Style);
            int descentPixel = (int) font.Size*descent/fontFamily.GetEmHeight(font.Style);
            return descentPixel;
        }

        /*******************************/

        /// <summary>
        /// Gets all X-axis points from the received graphics path
        /// </summary>
        /// <param name="path">Source graphics path</param>
        /// <returns>The array of X-axis values</returns>
        public static int[] GetXPoints(GraphicsPath path)
        {
            var tempIntArray = new int[path.PointCount];
            for (int index = 0; index < path.PointCount; index++)
            {
                tempIntArray[index] = (int) path.PathPoints[index].X;
            }
            return tempIntArray;
        }

        /*******************************/

        /// <summary>
        /// Gets all Y-axis points from the received graphics path
        /// </summary>
        /// <param name="path">Source graphics path</param>
        /// <returns>The array of Y-axis values</returns>
        public static int[] GetYPoints(GraphicsPath path)
        {
            var tempIntArray = new int[path.PointCount];
            for (int index = 0; index < path.PointCount; index++)
            {
                tempIntArray[index] = (int) path.PathPoints[index].Y;
            }
            return tempIntArray;
        }

        /*******************************/

        /// <summary>
        /// This method returns an Array of System.Int32 containing the size of the non client area of a control.
        /// The non client area includes elements such as scroll bars, borders, title bars, and menus.
        /// </summary>
        /// <param name="control">The control from which to retrieve the values.</param>
        /// <returns>An Array of System.Int32 containing the width of each non client area border in the following order
        /// top, left, right and bottom.</returns>
        public static Int32[] GetInsets(Control control)
        {
            var returnValue = new Int32[4];

            returnValue[0] = (control.RectangleToScreen(control.ClientRectangle).Top - control.Bounds.Top);
            returnValue[1] = (control.RectangleToScreen(control.ClientRectangle).Left - control.Bounds.Left);
            returnValue[2] = (control.Bounds.Right - control.RectangleToScreen(control.ClientRectangle).Right);
            returnValue[3] = (control.Bounds.Bottom - control.RectangleToScreen(control.ClientRectangle).Bottom);
            return returnValue;
        }


        /*******************************/

        /// <summary>
        /// Writes the exception stack trace to the received stream
        /// </summary>
        /// <param name="throwable">Exception to obtain information from</param>
        /// <param name="stream">Output sream used to write to</param>
        public static void WriteStackTrace(Exception throwable, TextWriter stream)
        {
            stream.Write(throwable.StackTrace);
            stream.Flush();
        }

        /*******************************/

        /*******************************/

        /// <summary>Reads a number of characters from the current source Stream and writes the data to the target array at the specified index.</summary>
        /// <param name="sourceStream">The source Stream to read from.</param>
        /// <param name="target">Contains the array of characteres read from the source Stream.</param>
        /// <param name="start">The starting index of the target array.</param>
        /// <param name="count">The maximum number of characters to read from the source Stream.</param>
        /// <returns>The number of characters read. The number will be less than or equal to count depending on the data available in the source Stream. Returns -1 if the end of the stream is reached.</returns>
        public static Int32 ReadInput(Stream sourceStream, sbyte[] target, int start, int count)
        {
            // Returns 0 bytes if not enough space in target
            if (target.Length == 0)
                return 0;

            var receiver = new byte[target.Length];
            int bytesRead = sourceStream.Read(receiver, start, count);

            // Returns -1 if EOF
            if (bytesRead == 0)
                return -1;

            for (int i = start; i < start + bytesRead; i++)
                target[i] = (sbyte) receiver[i];

            return bytesRead;
        }

        /// <summary>Reads a number of characters from the current source TextReader and writes the data to the target array at the specified index.</summary>
        /// <param name="sourceTextReader">The source TextReader to read from</param>
        /// <param name="target">Contains the array of characteres read from the source TextReader.</param>
        /// <param name="start">The starting index of the target array.</param>
        /// <param name="count">The maximum number of characters to read from the source TextReader.</param>
        /// <returns>The number of characters read. The number will be less than or equal to count depending on the data available in the source TextReader. Returns -1 if the end of the stream is reached.</returns>
        public static Int32 ReadInput(TextReader sourceTextReader, sbyte[] target, int start, int count)
        {
            // Returns 0 bytes if not enough space in target
            if (target.Length == 0) return 0;

            var charArray = new char[target.Length];
            int bytesRead = sourceTextReader.Read(charArray, start, count);

            // Returns -1 if EOF
            if (bytesRead == 0) return -1;

            for (int index = start; index < start + bytesRead; index++)
                target[index] = (sbyte) charArray[index];

            return bytesRead;
        }

        /*******************************/

        /// <summary>
        /// Converts an array of sbytes to an array of bytes
        /// </summary>
        /// <param name="sbyteArray">The array of sbytes to be converted</param>
        /// <returns>The new array of bytes</returns>
        public static byte[] ToByteArray(sbyte[] sbyteArray)
        {
            byte[] byteArray = null;

            if (sbyteArray != null)
            {
                byteArray = new byte[sbyteArray.Length];
                for (int index = 0; index < sbyteArray.Length; index++)
                    byteArray[index] = (byte) sbyteArray[index];
            }
            return byteArray;
        }

        /// <summary>
        /// Converts a string to an array of bytes
        /// </summary>
        /// <param name="sourceString">The string to be converted</param>
        /// <returns>The new array of bytes</returns>
        public static byte[] ToByteArray(String sourceString)
        {
            return Encoding.UTF8.GetBytes(sourceString);
        }

        /// <summary>
        /// Converts a array of object-type instances to a byte-type array.
        /// </summary>
        /// <param name="tempObjectArray">Array to convert.</param>
        /// <returns>An array of byte type elements.</returns>
        public static byte[] ToByteArray(Object[] tempObjectArray)
        {
            byte[] byteArray = null;
            if (tempObjectArray != null)
            {
                byteArray = new byte[tempObjectArray.Length];
                for (int index = 0; index < tempObjectArray.Length; index++)
                    byteArray[index] = (byte) tempObjectArray[index];
            }
            return byteArray;
        }

        /*******************************/

        /// <summary>
        /// Receives a byte array and returns it transformed in an sbyte array
        /// </summary>
        /// <param name="byteArray">Byte array to process</param>
        /// <returns>The transformed array</returns>
        public static sbyte[] ToSByteArray(byte[] byteArray)
        {
            sbyte[] sbyteArray = null;
            if (byteArray != null)
            {
                sbyteArray = new sbyte[byteArray.Length];
                for (int index = 0; index < byteArray.Length; index++)
                    sbyteArray[index] = (sbyte) byteArray[index];
            }
            return sbyteArray;
        }

        /*******************************/


        /*******************************/

        /// <summary>
        /// This method returns the literal value received
        /// </summary>
        /// <param name="literal">The literal to return</param>
        /// <returns>The received value</returns>
        public static long Identity(long literal)
        {
            return literal;
        }

        /// <summary>
        /// This method returns the literal value received
        /// </summary>
        /// <param name="literal">The literal to return</param>
        /// <returns>The received value</returns>
        public static ulong Identity(ulong literal)
        {
            return literal;
        }

        /// <summary>
        /// This method returns the literal value received
        /// </summary>
        /// <param name="literal">The literal to return</param>
        /// <returns>The received value</returns>
        public static float Identity(float literal)
        {
            return literal;
        }

        /// <summary>
        /// This method returns the literal value received
        /// </summary>
        /// <param name="literal">The literal to return</param>
        /// <returns>The received value</returns>
        public static double Identity(double literal)
        {
            return literal;
        }

        #region Nested type: CheckBoxSupport

        /// <summary>
        /// This class has static methods for manage CheckBox and RadioButton controls.
        /// </summary>
        public class CheckBoxSupport
        {
            /// <summary>
            /// Receives a CheckBox instance and sets the specific text and style.
            /// </summary>
            /// <param name="checkBoxInstance">CheckBox instance to be set.</param>
            /// <param name="text">The text to set Text property.</param>
            /// <param name="style">The style to be used to set the threeState property.</param>
            public static void SetCheckBox(CheckBox checkBoxInstance, String text, int style)
            {
                checkBoxInstance.Text = text;
                if (style == 2097152)
                    checkBoxInstance.ThreeState = true;
            }

            /// <summary>
            /// Returns a new CheckBox instance with the specified text
            /// </summary>
            /// <param name="text">The text to create the CheckBox with</param>
            /// <returns>A new CheckBox instance</returns>
            public static CheckBox CreateCheckBox(String text)
            {
                var tempCheck = new CheckBox();
                tempCheck.Text = text;
                return tempCheck;
            }

            /// <summary>
            /// Creates a CheckBox with a specified image.
            /// </summary>
            /// <param name="icon">The image to be used with the CheckBox.</param>
            /// <returns>A new CheckBox instance with an image.</returns>
            public static CheckBox CreateCheckBox(Image icon)
            {
                var tempCheckBox = new CheckBox();
                tempCheckBox.Image = icon;
                return tempCheckBox;
            }

            /// <summary>
            /// Creates a CheckBox with a specified label and image.
            /// </summary>
            /// <param name="text">The text to be used as label.</param>
            /// <param name="icon">The image to be used with the CheckBox.</param>
            /// <returns>A new CheckBox instance with a label and an image.</returns>
            public static CheckBox CreateCheckBox(String text, Image icon)
            {
                var tempCheckBox = new CheckBox();
                tempCheckBox.Text = text;
                tempCheckBox.Image = icon;
                return tempCheckBox;
            }

            /// <summary>
            /// Returns a new CheckBox instance with the specified text and state
            /// </summary>
            /// <param name="text">The text to create the CheckBox with</param>
            /// <param name="checkedStatus">Indicates if the Checkbox is checked or not</param>
            /// <returns>A new CheckBox instance</returns>
            public static CheckBox CreateCheckBox(String text, bool checkedStatus)
            {
                var tempCheckBox = new CheckBox();
                tempCheckBox.Text = text;
                tempCheckBox.Checked = checkedStatus;
                return tempCheckBox;
            }

            /// <summary>
            /// Creates a CheckBox with a specified image and checked if checkedStatus is true.
            /// </summary>
            /// <param name="icon">The image to be used with the CheckBox.</param>
            /// <param name="checkedStatus">Value to be set to Checked property.</param>
            /// <returns>A new CheckBox instance.</returns>
            public static CheckBox CreateCheckBox(Image icon, bool checkedStatus)
            {
                var tempCheckBox = new CheckBox();
                tempCheckBox.Image = icon;
                tempCheckBox.Checked = checkedStatus;
                return tempCheckBox;
            }

            /// <summary>
            /// Creates a CheckBox with label, image and checked if checkedStatus is true.
            /// </summary>
            /// <param name="text">The text to be used as label.</param>
            /// <param name="icon">The image to be used with the CheckBox.</param>
            /// <param name="checkedStatus">Value to be set to Checked property.</param>
            /// <returns>A new CheckBox instance.</returns>
            public static CheckBox CreateCheckBox(String text, Image icon, bool checkedStatus)
            {
                var tempCheckBox = new CheckBox();
                tempCheckBox.Text = text;
                tempCheckBox.Image = icon;
                tempCheckBox.Checked = checkedStatus;
                return tempCheckBox;
            }

            /// <summary>
            /// Creates a CheckBox with a specific control.
            /// </summary>
            /// <param name="control">The control to be added to the CheckBox.</param>
            /// <returns>The new CheckBox with the specific control.</returns>
            public static CheckBox CreateCheckBox(Control control)
            {
                var tempCheckBox = new CheckBox();
                tempCheckBox.Controls.Add(control);
                control.Location = new Point(0, 18);
                return tempCheckBox;
            }

            /// <summary>
            /// Creates a CheckBox with the specific control and style.
            /// </summary>
            /// <param name="control">The control to be added to the CheckBox.</param>
            /// <param name="style">The style to be used to set the threeState property.</param>
            /// <returns>The new CheckBox with the specific control and style.</returns>
            public static CheckBox CreateCheckBox(Control control, int style)
            {
                var tempCheckBox = new CheckBox();
                tempCheckBox.Controls.Add(control);
                control.Location = new Point(0, 18);
                if (style == 2097152)
                    tempCheckBox.ThreeState = true;
                return tempCheckBox;
            }

            /// <summary>
            /// Returns a new RadioButton instance with the specified text in the specified panel.
            /// </summary>
            /// <param name="text">The text to create the RadioButton with.</param>
            /// <param name="checkedStatus">Indicates if the RadioButton is checked or not.</param>
            /// <param name="panel">The panel where the RadioButton will be placed.</param>
            /// <returns>A new RadioButton instance.</returns>
            /// <remarks>If the panel is null the third param is ignored</remarks>
            public static RadioButton CreateRadioButton(String text, bool checkedStatus, Panel panel)
            {
                var tempCheckBox = new RadioButton();
                tempCheckBox.Text = text;
                tempCheckBox.Checked = checkedStatus;
                if (panel != null)
                    panel.Controls.Add(tempCheckBox);
                return tempCheckBox;
            }

            /// <summary>
            /// Receives a CheckBox instance and sets the Text and Image properties.
            /// </summary>
            /// <param name="checkBoxInstance">CheckBox instance to be set.</param>
            /// <param name="text">Value to be set to Text property.</param>
            /// <param name="icon">Value to be set to Image property.</param>
            public static void SetCheckBox(CheckBox checkBoxInstance, String text, Image icon)
            {
                checkBoxInstance.Text = text;
                checkBoxInstance.Image = icon;
            }

            /// <summary>
            /// Receives a CheckBox instance and sets the Text and Checked properties.
            /// </summary>
            /// <param name="checkBoxInstance">CheckBox instance to be set</param>
            /// <param name="text">Value to be set to Text</param>
            /// <param name="checkedStatus">Value to be set to Checked property.</param>
            public static void SetCheckBox(CheckBox checkBoxInstance, String text, bool checkedStatus)
            {
                checkBoxInstance.Text = text;
                checkBoxInstance.Checked = checkedStatus;
            }

            /// <summary>
            /// Receives a CheckBox instance and sets the Image and Checked properties.
            /// </summary>
            /// <param name="checkBoxInstance">CheckBox instance to be set.</param>
            /// <param name="icon">Value to be set to Image property.</param>
            /// <param name="checkedStatus">Value to be set to Checked property.</param>
            public static void SetCheckBox(CheckBox checkBoxInstance, Image icon, bool checkedStatus)
            {
                checkBoxInstance.Image = icon;
                checkBoxInstance.Checked = checkedStatus;
            }

            /// <summary>
            /// Receives a CheckBox instance and sets the Text, Image and Checked properties.
            /// </summary>
            /// <param name="checkBoxInstance">CheckBox instance to be set.</param>
            /// <param name="text">Value to be set to Text property.</param>
            /// <param name="icon">Value to be set to Image property.</param>
            /// <param name="checkedStatus">Value to be set to Checked property.</param>
            public static void SetCheckBox(CheckBox checkBoxInstance, String text, Image icon, bool checkedStatus)
            {
                checkBoxInstance.Text = text;
                checkBoxInstance.Image = icon;
                checkBoxInstance.Checked = checkedStatus;
            }

            /// <summary>
            /// Receives a CheckBox instance and sets the control specified.
            /// </summary>
            /// <param name="checkBoxInstance">CheckBox instance to be set.</param>
            /// <param name="control">The control assiciated with the CheckBox</param>
            public static void SetCheckBox(CheckBox checkBoxInstance, Control control)
            {
                checkBoxInstance.Controls.Add(control);
                control.Location = new Point(0, 18);
            }

            /// <summary>
            /// Receives a CheckBox instance and sets the specific control and style.
            /// </summary>
            /// <param name="checkBoxInstance">CheckBox instance to be set.</param>
            /// <param name="control">The control assiciated with the CheckBox.</param>
            /// <param name="style">The style to be used to set the threeState property.</param>
            public static void SetCheckBox(CheckBox checkBoxInstance, Control control, int style)
            {
                checkBoxInstance.Controls.Add(control);
                control.Location = new Point(0, 18);
                if (style == 2097152)
                    checkBoxInstance.ThreeState = true;
            }

            /// <summary>
            /// Receives an instance of a RadioButton and sets its Text and Checked properties.
            /// </summary>
            /// <param name="RadioButtonInstance">The instance of RadioButton to be set.</param>
            /// <param name="text">The text to set Text property.</param>
            /// <param name="checkedStatus">Indicates if the RadioButton is checked or not.</param>
            public static void SetCheckbox(RadioButton radioButtonInstance, String text, bool checkedStatus)
            {
                radioButtonInstance.Text = text;
                radioButtonInstance.Checked = checkedStatus;
            }

            /// <summary>
            /// Receives an instance of a RadioButton and sets its Text and Checked properties and its containing panel
            /// </summary>
            /// <param name="CheckBoxInstance">The instance of RadioButton to be set</param>
            /// <param name="text">The text to set Text property</param>
            /// <param name="checkedStatus">Indicates if the RadioButton is checked or not</param>
            /// <param name="panel">The panel where the RadioButton will be placed</param>
            /// <remarks>If the panel is null the third param is ignored</remarks>
            public static void SetRadioButton(RadioButton radioButtonInstance, String text, bool checkedStatus,
                                              Panel panel)
            {
                radioButtonInstance.Text = text;
                radioButtonInstance.Checked = checkedStatus;
                if (panel != null)
                    panel.Controls.Add(radioButtonInstance);
            }

            /// <summary>
            /// Creates a CheckBox with a specified text label and style.
            /// </summary>
            /// <param name="text">The text to be used as label.</param>
            /// <param name="style">The style to be used to set the threeState property.</param>
            /// <returns>A new CheckBox instance.</returns>
            public static CheckBox CreateCheckBox(String text, int style)
            {
                var checkBox = new CheckBox();
                checkBox.Text = text;
                if (style == 2097152)
                    checkBox.ThreeState = true;
                return checkBox;
            }

            /// <summary>
            /// Receives a CheckBox instance and sets the Text and ThreeState properties.
            /// </summary>
            /// <param name="checkBox">CheckBox instance to be set.</param>
            /// <param name="text">Value to be set to Text property.</param>
            /// <param name="style">The style to be used to set the threeState property.</param>
            public static void setCheckBox(CheckBox checkBox, String text, int style)
            {
                checkBox.Text = text;
                if (style == 2097152)
                    checkBox.ThreeState = true;
            }

            /// <summary>
            /// Creates a CheckBox with a specified text label, image and style.
            /// </summary>
            /// <param name="text">The text to be used as label.</param>
            /// <param name="icon">The image to be used with the CheckBox.</param>
            /// <param name="style">The style to be used to set the threeState property.</param>
            /// <returns>A new CheckBox instance.</returns>
            public static CheckBox CreateCheckBox(String text, Image icon, int style)
            {
                var checkBox = new CheckBox();
                checkBox.Text = text;
                checkBox.Image = icon;
                if (style == 2097152)
                    checkBox.ThreeState = true;
                return checkBox;
            }

            /// <summary>
            /// Receives a CheckBox instance and sets the Text, Image and ThreeState properties.
            /// </summary>
            /// <param name="checkBox">CheckBox instance to be set.</param>
            /// <param name="text">Value to be set to Text property.</param>
            /// <param name="icon">Value to be set to Image property.</param>
            /// <param name="style">The style to be used to set the threeState property.</param>
            public static void setCheckBox(CheckBox checkBox, String text, Image icon, int style)
            {
                checkBox.Text = text;
                checkBox.Image = icon;
                if (style == 2097152)
                    checkBox.ThreeState = true;
            }

            /// <summary>
            /// The SetIndeterminate method is used to sets or clear the CheckState of the CheckBox component.
            /// </summary>
            /// <param name="indeterminate">The value to the Indeterminate state. If true, the state is set; otherwise, it is cleared.</param>
            /// <param name="checkBox">The CheckBox component to be modified.</param>
            /// <returns></returns>
            public static CheckBox SetIndeterminate(bool indeterminate, CheckBox checkBox)
            {
                if (indeterminate)
                    checkBox.CheckState = CheckState.Indeterminate;
                else if (checkBox.Checked)
                    checkBox.CheckState = CheckState.Checked;
                else
                    checkBox.CheckState = CheckState.Unchecked;
                return checkBox;
            }
        }

        #endregion

        #region Nested type: CommandManager

        /// <summary>
        /// Class used to store and retrieve an object command specified as a String.
        /// </summary>
        public class CommandManager
        {
            /// <summary>
            /// Private Hashtable used to store objects and their commands.
            /// </summary>
            private static readonly Hashtable Commands = new Hashtable();

            /// <summary>
            /// Sets a command to the specified object.
            /// </summary>
            /// <param name="obj">The object that has the command.</param>
            /// <param name="cmd">The command for the object.</param>
            public static void SetCommand(Object obj, String cmd)
            {
                if (obj != null)
                {
                    if (Commands.Contains(obj))
                        Commands[obj] = cmd;
                    else
                        Commands.Add(obj, cmd);
                }
            }

            /// <summary>
            /// Gets a command associated with an object.
            /// </summary>
            /// <param name="obj">The object whose command is going to be retrieved.</param>
            /// <returns>The command of the specified object.</returns>
            public static String GetCommand(Object obj)
            {
                String result = "";
                if (obj != null)
                    result = Convert.ToString(Commands[obj]);
                return result;
            }


            /// <summary>
            /// Checks if the Control contains a command, if it does not it sets the default
            /// </summary>
            /// <param name="button">The control whose command will be checked</param>
            public static void CheckCommand(ButtonBase button)
            {
                if (button != null)
                {
                    if (GetCommand(button).Equals(""))
                        SetCommand(button, button.Text);
                }
            }

            /// <summary>
            /// Checks if the Control contains a command, if it does not it sets the default
            /// </summary>
            /// <param name="button">The control whose command will be checked</param>
            public static void CheckCommand(MenuItem menuItem)
            {
                if (menuItem != null)
                {
                    if (GetCommand(menuItem).Equals(""))
                        SetCommand(menuItem, menuItem.Text);
                }
            }

            /// <summary>
            /// Checks if the Control contains a command, if it does not it sets the default
            /// </summary>
            /// <param name="button">The control whose command will be checked</param>
            public static void CheckCommand(ComboBox comboBox)
            {
                if (comboBox != null)
                {
                    if (GetCommand(comboBox).Equals(""))
                        SetCommand(comboBox, "comboBoxChanged");
                }
            }
        }

        #endregion

        #region Nested type: DialogSupport

        /// <summary>
        /// Support class for creation of Forms behaving like Dialog windows.
        /// </summary>
        public class DialogSupport
        {
            /// <summary>
            /// Creates a dialog Form.
            /// </summary>
            /// <returns>The new dialog Form instance.</returns>
            public static Form CreateDialog()
            {
                var tempForm = new Form();
                tempForm.FormBorderStyle = FormBorderStyle.SizableToolWindow;
                tempForm.ShowInTaskbar = false;
                return tempForm;
            }

            /// <summary>
            /// Sets dialog like properties to a Form.
            /// </summary>
            /// <param name="formInstance">Form instance to be modified.</param>
            public static void SetDialog(Form formInstance)
            {
                formInstance.FormBorderStyle = FormBorderStyle.SizableToolWindow;
                formInstance.ShowInTaskbar = false;
            }

            /// <summary>
            /// Creates a dialog Form with an owner.
            /// </summary>
            /// <param name="owner">The form to be set as owner of the newly created one.</param>
            /// <returns>A new dialog Form.</returns>
            public static Form CreateDialog(Form owner)
            {
                var tempForm = new Form();
                tempForm.FormBorderStyle = FormBorderStyle.SizableToolWindow;
                tempForm.ShowInTaskbar = false;
                tempForm.Owner = owner;
                return tempForm;
            }

            /// <summary>
            /// Sets dialog like properties and an owner to a Form.
            /// </summary>
            /// <param name="formInstance">Form instance to be modified.</param>
            /// <param name="owner">The form to be set as owner of the newly created one.</param>
            public static void SetDialog(Form formInstance, Form owner)
            {
                formInstance.FormBorderStyle = FormBorderStyle.SizableToolWindow;
                formInstance.ShowInTaskbar = false;
                formInstance.Owner = owner;
            }


            /// <summary>
            /// Creates a dialog Form with an owner and a text property.
            /// </summary>
            /// <param name="owner">The form to be set as owner of the newly created one.</param>
            /// <param name="text">The title text for the form.</param>
            /// <returns>The new dialog Form.</returns>
            public static Form CreateDialog(Form owner, String text)
            {
                var tempForm = new Form();
                tempForm.FormBorderStyle = FormBorderStyle.SizableToolWindow;
                tempForm.ShowInTaskbar = false;
                tempForm.Owner = owner;
                tempForm.Text = text;
                return tempForm;
            }

            /// <summary>
            /// Sets dialog like properties, an owner and a text property to a Form.
            /// </summary>
            /// <param name="formInstance">Form instance to be modified.</param>
            /// <param name="owner">The form to be set as owner of the newly created one.</param>
            /// <param name="text">The title text for the form.</param>
            public static void SetDialog(Form formInstance, Form owner, String text)
            {
                formInstance.FormBorderStyle = FormBorderStyle.SizableToolWindow;
                formInstance.ShowInTaskbar = false;
                formInstance.Owner = owner;
                formInstance.Text = text;
            }


            /// <summary>
            /// This method sets or unsets a resizable border style to a Form.
            /// </summary>
            /// <param name="formInstance">The form to be modified.</param>
            /// <param name="sizable">Boolean value to be set.</param>
            public static void SetSizable(Form formInstance, bool sizable)
            {
                if (sizable)
                {
                    formInstance.FormBorderStyle = FormBorderStyle.SizableToolWindow;
                }
                else
                {
                    formInstance.FormBorderStyle = FormBorderStyle.FixedDialog;
                }
            }
        }

        #endregion

        #region Nested type: GraphicsManager

        /// <summary>
        /// Give functions to obtain information of graphic elements
        /// </summary>
        public class GraphicsManager
        {
            //Instance of GDI+ drawing surfaces graphics hashtable
            public static GraphicsHashTable manager = new GraphicsHashTable();

            /// <summary>
            /// Creates a new Graphics object from the device context handle associated with the Graphics
            /// parameter
            /// </summary>
            /// <param name="oldGraphics">Graphics instance to obtain the parameter from</param>
            /// <returns>A new GDI+ drawing surface</returns>
            public static Graphics CreateGraphics(Graphics oldGraphics)
            {
                Graphics createdGraphics;
                IntPtr hdc = oldGraphics.GetHdc();
                createdGraphics = Graphics.FromHdc(hdc);
                oldGraphics.ReleaseHdc(hdc);
                return createdGraphics;
            }

            /// <summary>
            /// This method draws a Bezier curve.
            /// </summary>
            /// <param name="graphics">It receives the Graphics instance</param>
            /// <param name="array">An array of (x,y) pairs of coordinates used to draw the curve.</param>
            public static void Bezier(Graphics graphics, int[] array)
            {
                Pen pen;
                pen = manager.GetPen(graphics);
                try
                {
                    graphics.DrawBezier(pen, array[0], array[1], array[2], array[3], array[4], array[5], array[6],
                                        array[7]);
                }
                catch (IndexOutOfRangeException e)
                {
                    throw new IndexOutOfRangeException(e.ToString());
                }
            }

            /// <summary>
            /// Gets the text size width and height from a given GDI+ drawing surface and a given font
            /// </summary>
            /// <param name="graphics">Drawing surface to use</param>
            /// <param name="graphicsFont">Font type to measure</param>
            /// <param name="text">String of text to measure</param>
            /// <returns>A point structure with both size dimentions; x for width and y for height</returns>
            public static Point GetTextSize(Graphics graphics, Font graphicsFont, String text)
            {
                Point textSize;
                SizeF tempSizeF;
                tempSizeF = graphics.MeasureString(text, graphicsFont);
                textSize = new Point();
                textSize.X = (int) tempSizeF.Width;
                textSize.Y = (int) tempSizeF.Height;
                return textSize;
            }

            /// <summary>
            /// Gets the text size width and height from a given GDI+ drawing surface and a given font
            /// </summary>
            /// <param name="graphics">Drawing surface to use</param>
            /// <param name="graphicsFont">Font type to measure</param>
            /// <param name="text">String of text to measure</param>
            /// <param name="width">Maximum width of the string</param>
            /// <param name="format">StringFormat object that represents formatting information, such as line spacing, for the string</param>
            /// <returns>A point structure with both size dimentions; x for width and y for height</returns>
            public static Point GetTextSize(Graphics graphics, Font graphicsFont, String text, Int32 width,
                                            StringFormat format)
            {
                Point textSize;
                SizeF tempSizeF;
                tempSizeF = graphics.MeasureString(text, graphicsFont, width, format);
                textSize = new Point();
                textSize.X = (int) tempSizeF.Width;
                textSize.Y = (int) tempSizeF.Height;
                return textSize;
            }

            #region Nested type: GraphicsHashTable

            /// <summary>
            /// Gives functionality over a hashtable of GDI+ drawing surfaces
            /// </summary>
            public class GraphicsHashTable : Hashtable
            {
                /// <summary>
                /// Gets the graphics object from the given control
                /// </summary>
                /// <param name="control">Control to obtain the graphics from</param>
                /// <returns>A graphics object with the control's characteristics</returns>
                public Graphics GetGraphics(Control control)
                {
                    Graphics graphic;
                    if (control.Visible)
                    {
                        graphic = control.CreateGraphics();
                        SetColor(graphic, control.ForeColor);
                        SetFont(graphic, control.Font);
                    }
                    else
                    {
                        graphic = null;
                    }
                    return graphic;
                }

                /// <summary>
                /// Sets the background color property to the given graphics object in the hashtable. If the element doesn't exist, then it adds the graphic element to the hashtable with the given background color.
                /// </summary>
                /// <param name="graphic">Graphic element to search or add</param>
                /// <param name="color">Background color to set</param>
                public void SetBackColor(Graphics graphic, Color color)
                {
                    if (this[graphic] != null)
                        ((GraphicsProperties) this[graphic]).BackColor = color;
                    else
                    {
                        var tempProps = new GraphicsProperties();
                        tempProps.BackColor = color;
                        Add(graphic, tempProps);
                    }
                }

                /// <summary>
                /// Gets the background color property to the given graphics object in the hashtable. If the element doesn't exist, then it returns White.
                /// </summary>
                /// <param name="graphic">Graphic element to search</param>
                /// <returns>The background color of the graphic</returns>
                public Color GetBackColor(Graphics graphic)
                {
                    if (this[graphic] == null)
                        return Color.White;
                    else
                        return ((GraphicsProperties) this[graphic]).BackColor;
                }

                /// <summary>
                /// Sets the text color property to the given graphics object in the hashtable. If the element doesn't exist, then it adds the graphic element to the hashtable with the given text color.
                /// </summary>
                /// <param name="graphic">Graphic element to search or add</param>
                /// <param name="color">Text color to set</param>
                public void SetTextColor(Graphics graphic, Color color)
                {
                    if (this[graphic] != null)
                        ((GraphicsProperties) this[graphic]).TextColor = color;
                    else
                    {
                        var tempProps = new GraphicsProperties();
                        tempProps.TextColor = color;
                        Add(graphic, tempProps);
                    }
                }

                /// <summary>
                /// Gets the text color property to the given graphics object in the hashtable. If the element doesn't exist, then it returns White.
                /// </summary>
                /// <param name="graphic">Graphic element to search</param>
                /// <returns>The text color of the graphic</returns>
                public Color GetTextColor(Graphics graphic)
                {
                    if (this[graphic] == null)
                        return Color.White;
                    else
                        return ((GraphicsProperties) this[graphic]).TextColor;
                }

                /// <summary>
                /// Sets the GraphicBrush property to the given graphics object in the hashtable. If the element doesn't exist, then it adds the graphic element to the hashtable with the given GraphicBrush.
                /// </summary>
                /// <param name="graphic">Graphic element to search or add</param>
                /// <param name="brush">GraphicBrush to set</param>
                public void SetBrush(Graphics graphic, SolidBrush brush)
                {
                    if (this[graphic] != null)
                        ((GraphicsProperties) this[graphic]).GraphicBrush = brush;
                    else
                    {
                        var tempProps = new GraphicsProperties();
                        tempProps.GraphicBrush = brush;
                        Add(graphic, tempProps);
                    }
                }

                /// <summary>
                /// Sets the GraphicBrush property to the given graphics object in the hashtable. If the element doesn't exist, then it adds the graphic element to the hashtable with the given GraphicBrush.
                /// </summary>
                /// <param name="graphic">Graphic element to search or add</param>
                /// <param name="brush">GraphicBrush to set</param>
                public void SetPaint(Graphics graphic, Brush brush)
                {
                    if (this[graphic] != null)
                        ((GraphicsProperties) this[graphic]).PaintBrush = brush;
                    else
                    {
                        var tempProps = new GraphicsProperties();
                        tempProps.PaintBrush = brush;
                        Add(graphic, tempProps);
                    }
                }

                /// <summary>
                /// Sets the GraphicBrush property to the given graphics object in the hashtable. If the element doesn't exist, then it adds the graphic element to the hashtable with the given GraphicBrush.
                /// </summary>
                /// <param name="graphic">Graphic element to search or add</param>
                /// <param name="color">Color to set</param>
                public void SetPaint(Graphics graphic, Color color)
                {
                    Brush brush = new SolidBrush(color);
                    if (this[graphic] != null)
                        ((GraphicsProperties) this[graphic]).PaintBrush = brush;
                    else
                    {
                        var tempProps = new GraphicsProperties();
                        tempProps.PaintBrush = brush;
                        Add(graphic, tempProps);
                    }
                }


                /// <summary>
                /// Gets the HatchBrush property to the given graphics object in the hashtable. If the element doesn't exist, then it returns Blank.
                /// </summary>
                /// <param name="graphic">Graphic element to search</param>
                /// <returns>The HatchBrush setting of the graphic</returns>
                public HatchBrush GetBrush(Graphics graphic)
                {
                    if (this[graphic] == null)
                        return new HatchBrush(HatchStyle.Plaid, Color.Black, Color.Black);
                    else
                        return new HatchBrush(HatchStyle.Plaid, ((GraphicsProperties) this[graphic]).GraphicBrush.Color,
                                              ((GraphicsProperties) this[graphic]).GraphicBrush.Color);
                }

                /// <summary>
                /// Gets the HatchBrush property to the given graphics object in the hashtable. If the element doesn't exist, then it returns Blank.
                /// </summary>
                /// <param name="graphic">Graphic element to search</param>
                /// <returns>The Brush setting of the graphic</returns>
                public Brush GetPaint(Graphics graphic)
                {
                    if (this[graphic] == null)
                        return new HatchBrush(HatchStyle.Plaid, Color.Black, Color.Black);
                    else
                        return ((GraphicsProperties) this[graphic]).PaintBrush;
                }

                /// <summary>
                /// Sets the GraphicPen property to the given graphics object in the hashtable. If the element doesn't exist, then it adds the graphic element to the hashtable with the given Pen.
                /// </summary>
                /// <param name="graphic">Graphic element to search or add</param>
                /// <param name="pen">Pen to set</param>
                public void SetPen(Graphics graphic, Pen pen)
                {
                    if (this[graphic] != null)
                        ((GraphicsProperties) this[graphic]).GraphicPen = pen;
                    else
                    {
                        var tempProps = new GraphicsProperties();
                        tempProps.GraphicPen = pen;
                        Add(graphic, tempProps);
                    }
                }

                /// <summary>
                /// Gets the GraphicPen property to the given graphics object in the hashtable. If the element doesn't exist, then it returns Black.
                /// </summary>
                /// <param name="graphic">Graphic element to search</param>
                /// <returns>The GraphicPen setting of the graphic</returns>
                public Pen GetPen(Graphics graphic)
                {
                    if (this[graphic] == null)
                        return Pens.Black;
                    else
                        return ((GraphicsProperties) this[graphic]).GraphicPen;
                }

                /// <summary>
                /// Sets the GraphicFont property to the given graphics object in the hashtable. If the element doesn't exist, then it adds the graphic element to the hashtable with the given Font.
                /// </summary>
                /// <param name="graphic">Graphic element to search or add</param>
                /// <param name="Font">Font to set</param>
                public void SetFont(Graphics graphic, Font font)
                {
                    if (this[graphic] != null)
                        ((GraphicsProperties) this[graphic]).GraphicFont = font;
                    else
                    {
                        var tempProps = new GraphicsProperties();
                        tempProps.GraphicFont = font;
                        Add(graphic, tempProps);
                    }
                }

                /// <summary>
                /// Gets the GraphicFont property to the given graphics object in the hashtable. If the element doesn't exist, then it returns Microsoft Sans Serif with size 8.25.
                /// </summary>
                /// <param name="graphic">Graphic element to search</param>
                /// <returns>The GraphicFont setting of the graphic</returns>
                public Font GetFont(Graphics graphic)
                {
                    if (this[graphic] == null)
                        return new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular, GraphicsUnit.Point, ((0)));
                    else
                        return ((GraphicsProperties) this[graphic]).GraphicFont;
                }

                /// <summary>
                /// Sets the color properties for a given Graphics object. If the element doesn't exist, then it adds the graphic element to the hashtable with the color properties set with the given value.
                /// </summary>
                /// <param name="graphic">Graphic element to search or add</param>
                /// <param name="color">Color value to set</param>
                public void SetColor(Graphics graphic, Color color)
                {
                    if (this[graphic] != null)
                    {
                        ((GraphicsProperties) this[graphic]).GraphicPen.Color = color;
                        ((GraphicsProperties) this[graphic]).GraphicBrush.Color = color;
                        ((GraphicsProperties) this[graphic]).color = color;
                        ((GraphicsProperties) this[graphic]).PaintBrush = new SolidBrush(color);
                    }
                    else
                    {
                        var tempProps = new GraphicsProperties();
                        tempProps.GraphicPen.Color = color;
                        tempProps.GraphicBrush.Color = color;
                        tempProps.color = color;
                        Add(graphic, tempProps);
                    }
                }

                /// <summary>
                /// Gets the color property to the given graphics object in the hashtable. If the element doesn't exist, then it returns Black.
                /// </summary>
                /// <param name="graphic">Graphic element to search</param>
                /// <returns>The color setting of the graphic</returns>
                public Color GetColor(Graphics graphic)
                {
                    if (this[graphic] == null)
                        return Color.Black;
                    else
                        return ((GraphicsProperties) this[graphic]).color;
                }

                /// <summary>
                /// This method gets the TextBackgroundColor of a Graphics instance
                /// </summary>
                /// <param name="graphic">The graphics instance</param>
                /// <returns>The color value in ARGB encoding</returns>
                public Color GetTextBackgroundColor(Graphics graphic)
                {
                    if (this[graphic] == null)
                        return Color.Black;
                    else
                    {
                        return ((GraphicsProperties) this[graphic]).TextBackgroundColor;
                    }
                }

                /// <summary>
                /// This method set the TextBackgroundColor of a Graphics instace
                /// </summary>
                /// <param name="graphic">The graphics instace</param>
                /// <param name="color">The System.Color to set the TextBackgroundColor</param>
                public void SetTextBackgroundColor(Graphics graphic, Color color)
                {
                    if (this[graphic] != null)
                    {
                        ((GraphicsProperties) this[graphic]).TextBackgroundColor = color;
                    }
                    else
                    {
                        var tempProps = new GraphicsProperties();
                        tempProps.TextBackgroundColor = color;
                        Add(graphic, tempProps);
                    }
                }

                #region Nested type: GraphicsProperties

                /// <summary>
                /// Structure to store properties from System.Drawing.Graphics objects
                /// </summary>
                private class GraphicsProperties
                {
                    public Color BackColor = Color.White;
                    public SolidBrush GraphicBrush = new SolidBrush(Color.Black);

                    public Font GraphicFont = new Font("Microsoft Sans Serif", 8.25F, FontStyle.Regular,
                                                       GraphicsUnit.Point, ((0)));

                    public Pen GraphicPen = new Pen(Color.Black);
                    public Brush PaintBrush = new SolidBrush(Color.Black);
                    public Color TextBackgroundColor = Color.Black;
                    public Color TextColor = Color.Black;
                    public Color color = Color.Black;
                }

                #endregion
            }

            #endregion
        }

        #endregion

        #region Nested type: RectangleSupport

        /// <summary>
        /// Support functions for the System.Drawing.Rectangle structure
        /// </summary>
        public class RectangleSupport
        {
            /// <summary>
            /// Changes the edges for the rectangle
            /// </summary>
            /// <param name="rectangle">Rectangle to change</param>
            /// <param name="x">New x-coordinate of the upper-left corner</param>
            /// <param name="y">New y-coordinate of the upper-left corner</param>
            /// <param name="width">New width of the Rectangle structure</param>
            /// <param name="height">New height of the Rectangle structure</param>
            public static void ReshapeRectangle(ref Rectangle rectangle, int x, int y, int width, int height)
            {
                rectangle.X = x;
                rectangle.Y = y;
                rectangle.Width = width;
                rectangle.Height = height;
            }

            /// <summary>
            /// Adds a point to the Rectangle
            /// </summary>
            /// <param name="rectangle">Rectangle to change</param>
            /// <param name="newX">X-axis of the point to add</param>
            /// <param name="newY">Y-axis of the point to add</param>
            public static void AddXYToRectangle(ref Rectangle rectangle, int newX, int newY)
            {
                int x = Math.Min(rectangle.X, newX);
                int y = Math.Min(rectangle.Y, newY);
                rectangle.Width = Math.Max(rectangle.X + rectangle.Width, newX) - x;
                rectangle.Height = Math.Max(rectangle.Y + rectangle.Height, newY) - y;
                rectangle.X = x;
                rectangle.Y = y;
            }

            /// <summary>
            /// Adds a the second rectangle to the first rectangle
            /// </summary>
            /// <param name="rectangle">Target rectangle</param>
            /// <param name="newRectangle">Rectangle to add</param>
            public static void AddRectangleToRectangle(ref Rectangle rectangle, Rectangle newRectangle)
            {
                int x = Math.Min(rectangle.X, newRectangle.X);
                int y = Math.Min(rectangle.Y, newRectangle.Y);
                rectangle.Width = Math.Max(rectangle.X + rectangle.Width, newRectangle.X + newRectangle.Width) - x;
                rectangle.Height = Math.Max(rectangle.Y + rectangle.Height, newRectangle.Y + newRectangle.Height) - y;
                rectangle.X = x;
                rectangle.Y = y;
            }

            /// <summary>
            /// Changes the edges for the first rectangle with the values of the second rectangle
            /// </summary>
            /// <param name="rectangle">Rectangle to change</param>
            /// <param name="newRectangle">Rectangle from which to copy shape</param>
            public static void SetBoundsRectangle(ref Rectangle rectangle, Rectangle newRectangle)
            {
                ReshapeRectangle(ref rectangle, newRectangle.X, newRectangle.Y, newRectangle.Width, newRectangle.Height);
            }

            /// <summary>
            /// Adds a point to the area covered by the rectangle
            /// </summary>
            /// <param name="rectangle">Rectangle to resize</param>
            /// <param name="newPoint">Represents the ordered pair of integer x- and y-coordinates to add to the rectangle</param>
            public static void AddPointToRectangle(ref Rectangle rectangle, Point newPoint)
            {
                AddXYToRectangle(ref rectangle, newPoint.X, newPoint.Y);
            }
        }

        #endregion

        #region Nested type: TextNumberFormat

        /// <summary>
        /// Implements number format functions
        /// </summary>
        [Serializable]
        public class TextNumberFormat
        {
            //Current localization number format infomation
            private readonly NumberFormatInfo numberFormat;
            //Enumeration of format types that can be used

            //Current format type used in the instance
            private readonly int numberFormatType;
            //Indicates if grouping is being used
            //Current separator used
            private readonly String separator;
            private bool groupingActivated;
            //Number of maximun digits in the integer portion of the number to represent the number
            //Number of maximun digits in the fraction portion of the number to represent the number
            private int maxFractionDigits;
            private int maxIntDigits;
            //Number of minimum digits in the integer portion of the number to represent the number
            private int minFractionDigits;
            private int minIntDigits;

            /// <summary>
            /// Initializes a new instance of the object class with the default values
            /// </summary>
            public TextNumberFormat()
            {
                numberFormat = new NumberFormatInfo();
                numberFormatType = (int) formatTypes.General;
                groupingActivated = true;
                separator = GetSeparator((int) formatTypes.General);
                maxIntDigits = 127;
                minIntDigits = 1;
                maxFractionDigits = 3;
                minFractionDigits = 0;
            }

            /// <summary>
            /// Initializes a new instance of the class with the specified number format
            /// and the amount of fractional digits to use
            /// </summary>
            /// <param name="theType">Number format</param>
            /// <param name="digits">Number of fractional digits to use</param>
            private TextNumberFormat(formatTypes theType, int digits)
            {
                numberFormat = NumberFormatInfo.CurrentInfo;
                numberFormatType = (int) theType;
                groupingActivated = true;
                separator = GetSeparator((int) theType);
                maxIntDigits = 127;
                minIntDigits = 1;
                maxFractionDigits = 3;
                minFractionDigits = 0;
            }

            /// <summary>
            /// Initializes a new instance of the class with the specified number format,
            /// uses the system's culture information,
            /// and assigns the amount of fractional digits to use
            /// </summary>
            /// <param name="theType">Number format</param>
            /// <param name="cultureNumberFormat">Represents information about a specific culture including the number formatting</param>
            /// <param name="digits">Number of fractional digits to use</param>
            private TextNumberFormat(formatTypes theType, CultureInfo cultureNumberFormat, int digits)
            {
                numberFormat = cultureNumberFormat.NumberFormat;
                numberFormatType = (int) theType;
                groupingActivated = true;
                separator = GetSeparator((int) theType);
                maxIntDigits = 127;
                minIntDigits = 1;
                maxFractionDigits = 3;
                minFractionDigits = 0;
            }

            /// <summary>
            /// Boolean value stating if grouping is used or not
            /// </summary>
            public bool GroupingUsed
            {
                get { return (groupingActivated); }
                set { groupingActivated = value; }
            }

            /// <summary>
            /// Minimum number of integer digits to use in the number format
            /// </summary>
            public int MinIntDigits
            {
                get { return minIntDigits; }
                set { minIntDigits = value; }
            }

            /// <summary>
            /// Maximum number of integer digits to use in the number format
            /// </summary>
            public int MaxIntDigits
            {
                get { return maxIntDigits; }
                set { maxIntDigits = value; }
            }

            /// <summary>
            /// Minimum number of fraction digits to use in the number format
            /// </summary>
            public int MinFractionDigits
            {
                get { return minFractionDigits; }
                set { minFractionDigits = value; }
            }

            /// <summary>
            /// Maximum number of fraction digits to use in the number format
            /// </summary>
            public int MaxFractionDigits
            {
                get { return maxFractionDigits; }
                set { maxFractionDigits = value; }
            }

            /// <summary>
            /// Sets the Maximum integer digits value. 
            /// </summary>
            /// <param name="newValue">the new value for the maxIntDigits field</param>
            public void setMaximumIntegerDigits(int newValue)
            {
                maxIntDigits = newValue;
                if (newValue <= 0)
                {
                    maxIntDigits = 0;
                    minIntDigits = 0;
                }
                else if (maxIntDigits < minIntDigits)
                {
                    minIntDigits = maxIntDigits;
                }
            }

            /// <summary>
            /// Sets the minimum integer digits value. 
            /// </summary>
            /// <param name="newValue">the new value for the minIntDigits field</param>
            public void setMinimumIntegerDigits(int newValue)
            {
                minIntDigits = newValue;
                if (newValue <= 0)
                {
                    minIntDigits = 0;
                }
                else if (maxIntDigits < minIntDigits)
                {
                    maxIntDigits = minIntDigits;
                }
            }

            /// <summary>
            /// Sets the maximum fraction digits value. 
            /// </summary>
            /// <param name="newValue">the new value for the maxFractionDigits field</param>
            public void setMaximumFractionDigits(int newValue)
            {
                maxFractionDigits = newValue;
                if (newValue <= 0)
                {
                    maxFractionDigits = 0;
                    minFractionDigits = 0;
                }
                else if (maxFractionDigits < minFractionDigits)
                {
                    minFractionDigits = maxFractionDigits;
                }
            }

            /// <summary>
            /// Sets the minimum fraction digits value. 
            /// </summary>
            /// <param name="newValue">the new value for the minFractionDigits field</param>
            public void setMinimumFractionDigits(int newValue)
            {
                minFractionDigits = newValue;
                if (newValue <= 0)
                {
                    minFractionDigits = 0;
                }
                else if (maxFractionDigits < minFractionDigits)
                {
                    maxFractionDigits = minFractionDigits;
                }
            }

            /// <summary>
            /// Returns an initialized instance of the TextNumberFormat object
            /// using number representation.
            /// </summary>
            /// <returns>The object instance</returns>
            public static TextNumberFormat getTextNumberInstance()
            {
                var instance = new TextNumberFormat(formatTypes.Number, 3);
                return instance;
            }

            /// <summary>
            /// Returns an initialized instance of the TextNumberFormat object
            /// using currency representation.
            /// </summary>
            /// <returns>The object instance</returns>
            public static TextNumberFormat getTextNumberCurrencyInstance()
            {
                var instance = new TextNumberFormat(formatTypes.Currency, 3);
                return instance.setToCurrencyNumberFormatDefaults(instance);
            }

            /// <summary>
            /// Returns an initialized instance of the TextNumberFormat object
            /// using percent representation.
            /// </summary>
            /// <returns>The object instance</returns>
            public static TextNumberFormat getTextNumberPercentInstance()
            {
                var instance = new TextNumberFormat(formatTypes.Percent, 3);
                return instance.setToPercentNumberFormatDefaults(instance);
            }

            /// <summary>
            /// Returns an initialized instance of the TextNumberFormat object
            /// using number representation, it uses the culture format information provided.
            /// </summary>
            /// <param name="culture">Represents information about a specific culture</param>
            /// <returns>The object instance</returns>
            public static TextNumberFormat getTextNumberInstance(CultureInfo culture)
            {
                var instance = new TextNumberFormat(formatTypes.Number, culture, 3);
                return instance;
            }

            /// <summary>
            /// Returns an initialized instance of the TextNumberFormat object
            /// using currency representation, it uses the culture format information provided.
            /// </summary>
            /// <param name="culture">Represents information about a specific culture</param>
            /// <returns>The object instance</returns>
            public static TextNumberFormat getTextNumberCurrencyInstance(CultureInfo culture)
            {
                var instance = new TextNumberFormat(formatTypes.Currency, culture, 3);
                return instance.setToCurrencyNumberFormatDefaults(instance);
            }

            /// <summary>
            /// Returns an initialized instance of the TextNumberFormat object
            /// using percent representation, it uses the culture format information provided.
            /// </summary>
            /// <param name="culture">Represents information about a specific culture</param>
            /// <returns>The object instance</returns>
            public static TextNumberFormat getTextNumberPercentInstance(CultureInfo culture)
            {
                var instance = new TextNumberFormat(formatTypes.Percent, culture, 3);
                return instance.setToPercentNumberFormatDefaults(instance);
            }

            /// <summary>
            /// Clones the object instance
            /// </summary>
            /// <returns>The cloned object instance</returns>
            public Object Clone()
            {
                return this;
            }

            /// <summary>
            /// Determines if the received object is equal to the
            /// current object instance
            /// </summary>
            /// <param name="textNumberObject">TextNumber instance to compare</param>
            /// <returns>True or false depending if the two instances are equal</returns>
            public override bool Equals(Object obj)
            {
                // Check for null values and compare run-time types.
                if (obj == null || GetType() != obj.GetType())
                    return false;
                var param = (TextNumberFormat) obj;
                return (numberFormat == param.numberFormat) && (numberFormatType == param.numberFormatType)
                       && (groupingActivated == param.groupingActivated) && (separator == param.separator)
                       && (maxIntDigits == param.maxIntDigits) && (minIntDigits == param.minIntDigits)
                       && (maxFractionDigits == param.maxFractionDigits) &&
                       (minFractionDigits == param.minFractionDigits);
            }


            /// <summary>
            /// Serves as a hash function for a particular type, suitable for use in hashing algorithms and data structures like a hash table.
            /// </summary>
            /// <returns>A hash code for the current Object</returns>
            public override int GetHashCode()
            {
                return numberFormat.GetHashCode() ^ numberFormatType ^ groupingActivated.GetHashCode()
                       ^ separator.GetHashCode() ^ maxIntDigits ^ minIntDigits ^ maxFractionDigits ^ minFractionDigits;
            }

            /// <summary>
            /// Formats a number with the current formatting parameters
            /// </summary>
            /// <param name="number">Source number to format</param>
            /// <returns>The formatted number string</returns>
            public String FormatDouble(double number)
            {
                if (groupingActivated)
                {
                    return
                        SetIntDigits(number.ToString(GetCurrentFormatString() + GetNumberOfDigits(number), numberFormat));
                }
                else
                {
                    return
                        SetIntDigits(
                            (number.ToString(GetCurrentFormatString() + GetNumberOfDigits(number), numberFormat)).
                                Replace(separator, ""));
                }
            }

            /// <summary>
            /// Formats a number with the current formatting parameters
            /// </summary>
            /// <param name="number">Source number to format</param>
            /// <returns>The formatted number string</returns>
            public String FormatLong(long number)
            {
                if (groupingActivated)
                {
                    return SetIntDigits(number.ToString(GetCurrentFormatString() + minFractionDigits, numberFormat));
                }
                else
                {
                    return
                        SetIntDigits(
                            (number.ToString(GetCurrentFormatString() + minFractionDigits, numberFormat)).Replace(
                                separator, ""));
                }
            }


            /// <summary>
            /// Formats the number according to the specified number of integer digits 
            /// </summary>
            /// <param name="number">The number to format</param>
            /// <returns></returns>
            private String SetIntDigits(String number)
            {
                String decimals = "";
                String fraction = "";
                int i = number.IndexOf(numberFormat.NumberDecimalSeparator);
                if (i > 0)
                {
                    fraction = number.Substring(i);
                    decimals = number.Substring(0, i).Replace(numberFormat.NumberGroupSeparator, "");
                }
                else decimals = number.Replace(numberFormat.NumberGroupSeparator, "");
                decimals = decimals.PadLeft(MinIntDigits, '0');
                if ((i = decimals.Length - MaxIntDigits) > 0) decimals = decimals.Remove(0, i);
                if (groupingActivated)
                {
                    for (i = decimals.Length; i > 3; i -= 3)
                    {
                        decimals = decimals.Insert(i - 3, numberFormat.NumberGroupSeparator);
                    }
                }
                decimals = decimals + fraction;
                if (decimals.Length == 0) return "0";
                else return decimals;
            }

            /// <summary>
            /// Gets the list of all supported cultures
            /// </summary>
            /// <returns>An array of type CultureInfo that represents the supported cultures</returns>
            public static CultureInfo[] GetAvailableCultures()
            {
                return CultureInfo.GetCultures(CultureTypes.AllCultures);
            }

            /// <summary>
            /// Obtains the current format representation used
            /// </summary>
            /// <returns>A character representing the string format used</returns>
            private String GetCurrentFormatString()
            {
                String currentFormatString = "n"; //Default value
                switch (numberFormatType)
                {
                    case (int) formatTypes.Currency:
                        currentFormatString = "c";
                        break;

                    case (int) formatTypes.General:
                        currentFormatString = "n";
                        break;

                    case (int) formatTypes.Number:
                        currentFormatString = "n";
                        break;

                    case (int) formatTypes.Percent:
                        currentFormatString = "p";
                        break;
                }
                return currentFormatString;
            }

            /// <summary>
            /// Retrieves the separator used, depending on the format type specified
            /// </summary>
            /// <param name="numberFormatType">formatType enumarator value to inquire</param>
            /// <returns>The values of character separator used </returns>
            private String GetSeparator(int numberFormatType)
            {
                String separatorItem = " "; //Default Separator

                switch (numberFormatType)
                {
                    case (int) formatTypes.Currency:
                        separatorItem = numberFormat.CurrencyGroupSeparator;
                        break;

                    case (int) formatTypes.General:
                        separatorItem = numberFormat.NumberGroupSeparator;
                        break;

                    case (int) formatTypes.Number:
                        separatorItem = numberFormat.NumberGroupSeparator;
                        break;

                    case (int) formatTypes.Percent:
                        separatorItem = numberFormat.PercentGroupSeparator;
                        break;
                }
                return separatorItem;
            }

            /// <summary>
            /// Sets the values of minFractionDigits and maxFractionDigits to the currency standard
            /// </summary>
            /// <param name="format">The TextNumberFormat instance to set</param>
            /// <returns>The TextNumberFormat with corresponding the default values</returns>
            private TextNumberFormat setToCurrencyNumberFormatDefaults(TextNumberFormat format)
            {
                format.maxFractionDigits = 2;
                format.minFractionDigits = 2;
                return format;
            }

            /// <summary>
            /// Sets the values of minFractionDigits and maxFractionDigits to the percent standard
            /// </summary>
            /// <param name="format">The TextNumberFormat instance to set</param>
            /// <returns>The TextNumberFormat with corresponding the default values</returns>
            private TextNumberFormat setToPercentNumberFormatDefaults(TextNumberFormat format)
            {
                format.maxFractionDigits = 0;
                format.minFractionDigits = 0;
                return format;
            }

            /// <summary>
            /// Gets the number of fraction digits thats must be used by the format methods
            /// </summary>
            /// <param name="number">The double number</param>
            /// <returns>The number of fraction digits to use</returns>
            private int GetNumberOfDigits(Double number)
            {
                int counter = 0;
                double temp = Math.Abs(number);
                while ((temp%1) > 0)
                {
                    temp *= 10;
                    counter++;
                }
                return (counter < minFractionDigits)
                           ? minFractionDigits
                           : ((counter < maxFractionDigits) ? counter : maxFractionDigits);
            }

            #region Nested type: formatTypes

            private enum formatTypes
            {
                General,
                Number,
                Currency,
                Percent
            };

            #endregion
        }

        #endregion

        #region Nested type: ThreadClass

        /// <summary>
        /// Support class used to handle threads
        /// </summary>
        public class ThreadClass : IThreadRunnable
        {
            /// <summary>
            /// The instance of System.Threading.Thread
            /// </summary>
            private Thread threadField;

            /// <summary>
            /// Initializes a new instance of the ThreadClass class
            /// </summary>
            public ThreadClass()
            {
                threadField = new Thread(Run);
            }

            /// <summary>
            /// Initializes a new instance of the Thread class.
            /// </summary>
            /// <param name="Name">The name of the thread</param>
            public ThreadClass(String Name)
            {
                threadField = new Thread(Run);
                this.Name = Name;
            }

            /// <summary>
            /// Initializes a new instance of the Thread class.
            /// </summary>
            /// <param name="Start">A ThreadStart delegate that references the methods to be invoked when this thread begins executing</param>
            public ThreadClass(ThreadStart Start)
            {
                threadField = new Thread(Start);
            }

            /// <summary>
            /// Initializes a new instance of the Thread class.
            /// </summary>
            /// <param name="Start">A ThreadStart delegate that references the methods to be invoked when this thread begins executing</param>
            /// <param name="Name">The name of the thread</param>
            public ThreadClass(ThreadStart Start, String Name)
            {
                threadField = new Thread(Start);
                this.Name = Name;
            }

            /// <summary>
            /// Gets the current thread instance
            /// </summary>
            public Thread Instance
            {
                get { return threadField; }
                set { threadField = value; }
            }

            /// <summary>
            /// Gets or sets the name of the thread
            /// </summary>
            public String Name
            {
                get { return threadField.Name; }
                set
                {
                    if (threadField.Name == null)
                        threadField.Name = value;
                }
            }

            /// <summary>
            /// Gets or sets a value indicating the scheduling priority of a thread
            /// </summary>
            public ThreadPriority Priority
            {
                get { return threadField.Priority; }
                set { threadField.Priority = value; }
            }

            /// <summary>
            /// Gets a value indicating the execution status of the current thread
            /// </summary>
            public bool IsAlive
            {
                get { return threadField.IsAlive; }
            }

            /// <summary>
            /// Gets or sets a value indicating whether or not a thread is a background thread.
            /// </summary>
            public bool IsBackground
            {
                get { return threadField.IsBackground; }
                set { threadField.IsBackground = value; }
            }

            #region IThreadRunnable Members

            /// <summary>
            /// This method has no functionality unless the method is overridden
            /// </summary>
            public virtual void Run()
            {
            }

            #endregion

            /// <summary>
            /// Causes the operating system to change the state of the current thread instance to ThreadState.Running
            /// </summary>
            public virtual void Start()
            {
                threadField.Start();
            }

            /// <summary>
            /// Interrupts a thread that is in the WaitSleepJoin thread state
            /// </summary>
            public virtual void Interrupt()
            {
                threadField.Interrupt();
            }

            /// <summary>
            /// Blocks the calling thread until a thread terminates
            /// </summary>
            public void Join()
            {
                threadField.Join();
            }

            /// <summary>
            /// Blocks the calling thread until a thread terminates or the specified time elapses
            /// </summary>
            /// <param name="MiliSeconds">Time of wait in milliseconds</param>
            public void Join(long MiliSeconds)
            {
                lock (this)
                {
                    threadField.Join(new TimeSpan(MiliSeconds*10000));
                }
            }

            /// <summary>
            /// Blocks the calling thread until a thread terminates or the specified time elapses
            /// </summary>
            /// <param name="MiliSeconds">Time of wait in milliseconds</param>
            /// <param name="NanoSeconds">Time of wait in nanoseconds</param>
            public void Join(long MiliSeconds, int NanoSeconds)
            {
                lock (this)
                {
                    threadField.Join(new TimeSpan(MiliSeconds*10000 + NanoSeconds*100));
                }
            }

            /// <summary>
            /// Resumes a thread that has been suspended
            /// </summary>
            public void Resume()
            {
                threadField.Resume();
            }

            /// <summary>
            /// Raises a ThreadAbortException in the thread on which it is invoked, 
            /// to begin the process of terminating the thread. Calling this method 
            /// usually terminates the thread
            /// </summary>
            public void Abort()
            {
                threadField.Abort();
            }

            /// <summary>
            /// Raises a ThreadAbortException in the thread on which it is invoked, 
            /// to begin the process of terminating the thread while also providing
            /// exception information about the thread termination. 
            /// Calling this method usually terminates the thread.
            /// </summary>
            /// <param name="stateInfo">An object that contains application-specific information, such as state, which can be used by the thread being aborted</param>
            public void Abort(Object stateInfo)
            {
                lock (this)
                {
                    threadField.Abort(stateInfo);
                }
            }

            /// <summary>
            /// Suspends the thread, if the thread is already suspended it has no effect
            /// </summary>
            public void Suspend()
            {
                threadField.Suspend();
            }

            /// <summary>
            /// Obtain a String that represents the current Object
            /// </summary>
            /// <returns>A String that represents the current Object</returns>
            public override String ToString()
            {
                return "Thread[" + Name + "," + Priority.ToString() + "," + "" + "]";
            }

            /// <summary>
            /// Gets the currently running thread
            /// </summary>
            /// <returns>The currently running thread</returns>
            public static ThreadClass Current()
            {
                var CurrentThread = new ThreadClass();
                CurrentThread.Instance = Thread.CurrentThread;
                return CurrentThread;
            }
        }

        #endregion

        #region Nested type: Tokenizer

        /// <summary>
        /// The class performs token processing in strings
        /// </summary>
        public class Tokenizer : IEnumerator
        {
            /// Char representation of the String to tokenize.
            private readonly char[] chars;

            /// Include demiliters in the results.
            private readonly bool includeDelims;

            /// Position over the string
            private long currentPos;

            //The tokenizer uses the default delimiter set: the space character, the tab character, the newline character, and the carriage-return character and the form-feed character
            private string delimiters = " \t\n\r\f";

            /// <summary>
            /// Initializes a new class instance with a specified string to process
            /// </summary>
            /// <param name="source">String to tokenize</param>
            public Tokenizer(String source)
            {
                chars = source.ToCharArray();
            }

            /// <summary>
            /// Initializes a new class instance with a specified string to process
            /// and the specified token delimiters to use
            /// </summary>
            /// <param name="source">String to tokenize</param>
            /// <param name="delimiters">String containing the delimiters</param>
            public Tokenizer(String source, String delimiters) : this(source)
            {
                this.delimiters = delimiters;
            }


            /// <summary>
            /// Initializes a new class instance with a specified string to process, the specified token 
            /// delimiters to use, and whether the delimiters must be included in the results.
            /// </summary>
            /// <param name="source">String to tokenize</param>
            /// <param name="delimiters">String containing the delimiters</param>
            /// <param name="includeDelims">Determines if delimiters are included in the results.</param>
            public Tokenizer(String source, String delimiters, bool includeDelims) : this(source, delimiters)
            {
                this.includeDelims = includeDelims;
            }


            /// <summary>
            /// Remaining tokens count
            /// </summary>
            public int Count
            {
                get
                {
                    //keeping the current pos
                    long pos = currentPos;
                    int i = 0;

                    try
                    {
                        while (true)
                        {
                            NextToken();
                            i++;
                        }
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        currentPos = pos;
                        return i;
                    }
                }
            }

            #region IEnumerator Members

            /// <summary>
            ///  Performs the same action as NextToken.
            /// </summary>
            public Object Current
            {
                get { return NextToken(); }
            }

            /// <summary>
            //  Performs the same action as HasMoreTokens.
            /// </summary>
            /// <returns>True or false, depending if there are more tokens</returns>
            public bool MoveNext()
            {
                return HasMoreTokens();
            }

            /// <summary>
            /// Does nothing.
            /// </summary>
            public void Reset()
            {
                ;
            }

            #endregion

            /// <summary>
            /// Returns the next token from the token list
            /// </summary>
            /// <returns>The string value of the token</returns>
            public String NextToken()
            {
                return NextToken(delimiters);
            }

            /// <summary>
            /// Returns the next token from the source string, using the provided
            /// token delimiters
            /// </summary>
            /// <param name="delimiters">String containing the delimiters to use</param>
            /// <returns>The string value of the token</returns>
            public String NextToken(String delimiters)
            {
                //According to documentation, the usage of the received delimiters should be temporary (only for this call).
                //However, it seems it is not true, so the following line is necessary.
                this.delimiters = delimiters;

                //at the end 
                if (currentPos == chars.Length)
                    throw new ArgumentOutOfRangeException();
                    //if over a delimiter and delimiters must be returned
                else if ((Array.IndexOf(delimiters.ToCharArray(), chars[currentPos]) != -1)
                         && includeDelims)
                    return "" + chars[currentPos++];
                    //need to get the token wo delimiters.
                else
                    return nextToken(delimiters.ToCharArray());
            }

            //Returns the nextToken wo delimiters
            private String nextToken(char[] delimiters)
            {
                string token = "";
                long pos = currentPos;

                //skip possible delimiters
                while (Array.IndexOf(delimiters, chars[currentPos]) != -1)
                    //The last one is a delimiter (i.e there is no more tokens)
                    if (++currentPos == chars.Length)
                    {
                        currentPos = pos;
                        throw new ArgumentOutOfRangeException();
                    }

                //getting the token
                while (Array.IndexOf(delimiters, chars[currentPos]) == -1)
                {
                    token += chars[currentPos];
                    //the last one is not a delimiter
                    if (++currentPos == chars.Length)
                        break;
                }
                return token;
            }


            /// <summary>
            /// Determines if there are more tokens to return from the source string
            /// </summary>
            /// <returns>True or false, depending if there are more tokens</returns>
            public bool HasMoreTokens()
            {
                //keeping the current pos
                long pos = currentPos;

                try
                {
                    NextToken();
                }
                catch (ArgumentOutOfRangeException)
                {
                    return false;
                }
                finally
                {
                    currentPos = pos;
                }
                return true;
            }
        }

        #endregion
    }
}