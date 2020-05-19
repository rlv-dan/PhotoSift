/*
 *  PhotoSift
 *  Copyright (C) 2013-2014  RL Vision
 *  Copyright (C) 2020  YFdyh000
 *  
 *  This program is free software: you can redistribute it and/or modify
 *  it under the terms of the GNU General Public License as published by
 *  the Free Software Foundation, either version 3 of the License, or
 *  (at your option) any later version.
 *  
 *  This program is distributed in the hope that it will be useful,
 *  but WITHOUT ANY WARRANTY; without even the implied warranty of
 *  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 *  GNU General Public License for more details.
 *  
 *  You should have received a copy of the GNU General Public License
 *  along with this program.  If not, see <http://www.gnu.org/licenses/>.
 * 
 * */

using System;
using System.Collections.Generic;
using System.Windows.Forms;
using System.Reflection;

namespace PhotoSift
{
    /// <summary>
    /// This class contains various static methods of generic kind
    /// </summary>
    public static class Util
	{
		public static void CenterControl( Control ctl, int yOffset = 0 )
		{
			ctl.Left = (int)( (double)( ctl.Parent.Width / 2 ) - (double)( ctl.Width / 2 ) );
			ctl.Top = (int)( (double)( ctl.Parent.Height / 2 ) - (double)( ctl.Height / 2 ) ) + yOffset;
		}

		/// <summary>
		/// Randomly re-orders the items in a List
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="list">The list to shuffle</param>
		/// <returns>The list with its items randomly re-ordered</returns>
		public static List<T> Shuffle<T>( IList<T> list )
		{
			Random rng = new Random();
			int n = list.Count;
			while( n > 1 )
			{
				n--;
				int k = rng.Next( n + 1 );
				T value = list[k];
				list[k] = list[n];
				list[n] = value;
			}
			return list as List<T>;
		}

		/// <summary>
		/// Very simple InputBox.
		/// Source: http://www.csharp-examples.net/inputbox/
		/// </summary>
		/// <param name="title">Dialog box title</param>
		/// <param name="promptText">Dialog box text</param>
		/// <param name="value">String containing default text. Input text is returned in this variable.</param>
		/// <returns>DialogResult.OK or DialogResult.Cancel</returns>
		public static DialogResult InputBox( string title, string promptText, ref string value )
		{
			Form form = new Form();
			Label label = new Label();
			TextBox textBox = new TextBox();
			Button buttonOk = new Button();
			Button buttonCancel = new Button();

			form.Text = title;
			label.Text = promptText;
			textBox.Text = value;

			buttonOk.Text = "OK";
			buttonCancel.Text = "Cancel";
			buttonOk.DialogResult = DialogResult.OK;
			buttonCancel.DialogResult = DialogResult.Cancel;

			label.SetBounds( 9, 20, 372, 13 );
			textBox.SetBounds( 12, 36, 372, 20 );
			buttonOk.SetBounds( 228, 72, 75, 23 );
			buttonCancel.SetBounds( 309, 72, 75, 23 );

			label.AutoSize = true;
			textBox.Anchor = textBox.Anchor | AnchorStyles.Right;
			buttonOk.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;
			buttonCancel.Anchor = AnchorStyles.Bottom | AnchorStyles.Right;

			form.ClientSize = new System.Drawing.Size( 396, 107 );
			form.Controls.AddRange( new Control[] { label, textBox, buttonOk, buttonCancel } );
			form.ClientSize = new System.Drawing.Size( Math.Max( 300, label.Right + 10 ), form.ClientSize.Height );
			form.FormBorderStyle = FormBorderStyle.FixedDialog;
			form.StartPosition = FormStartPosition.CenterParent;
			form.MinimizeBox = false;
			form.MaximizeBox = false;
			form.AcceptButton = buttonOk;
			form.CancelButton = buttonCancel;

			DialogResult dialogResult = form.ShowDialog();
			if( dialogResult == DialogResult.OK ) value = textBox.Text;
			return dialogResult;
		}

		/// <summary>
		/// Convert Bytes into KB/MB/GB
		/// </summary>
		/// <param name="byteCount">Size in bytes</param>
		public static string TidyFileSize( double byteCount )
		{
			string size = "0 Bytes";
			if( byteCount >= 1073741824.0 )
				size = String.Format( "{0:##.##}", byteCount / 1073741824.0 ) + " GB";
			else if( byteCount >= 1048576.0 )
				size = String.Format( "{0:##.##}", byteCount / 1048576.0 ) + " MB";
			else if( byteCount >= 1024.0 )
				size = String.Format( "{0:##}", byteCount / 1024.0 ) + " KB";
			else if( byteCount > 0 && byteCount < 1024.0 )
				size = byteCount.ToString() + " Bytes";
			return size;
		}

		public static string GetAppName()
		{
			return Assembly.GetExecutingAssembly().GetName().Name + " Ex"; // Append to avoid space in the generated file's name
		}

		/// <summary>
		/// Get new position relative to start position for an index (array), with optional looparound
		/// </summary>
		/// <param name="indexSize">Size of index</param>
		/// <param name="startPosition">Original index position</param>
		/// <param name="relativeJump">Number of positions to jump ahead or back</param>
		/// <param name="loopAround">If reaching either end of the index, do you want to loop and continue at the other end?</param>
		/// <returns>New position in index</returns>
		public static int IndexJump( int indexSize, int startPosition, int relativeJump, bool loopAround = true )
		{
			int newPosition = startPosition + relativeJump;
			if( loopAround )
			{
				if( newPosition >= indexSize ) newPosition = 0 + ( newPosition - indexSize );
				if( newPosition < 0 ) newPosition = indexSize - Math.Abs( newPosition );
			}
			newPosition = Math.Max( Math.Min( newPosition, indexSize - 1 ), 0 );
			return newPosition;
		}

		public static readonly string[] Def_allowsPicExts = new string[]
		{
			".jpg",
			".jpeg",
			".tif",
			".tiff",
			".png",
			".gif",
			".bmp",
			".ico",
			".wmf",
			".emf",
			".webp"
		};
		public static readonly string[] Def_allowsVideoExts = new string[]
		{ // https://support.microsoft.com/en-us/help/316992/file-types-supported-by-windows-media-player
			".asf",
			".wma",
			".wmv",
			".wm",
			".asx",
			".wax",
			".wvx",
			".wmx",
			".wpl",
			".dvr-ms",
			".wmd",
			".avi",
			".mpg",
			".mpeg",
			".m1v",
			".mp2",
			".mp3",
			".mpa",
			".mpe",
			".m3u",
			".mid",
			".midi",
			".rmi",
			".aif",
			".aifc",
			".aiff",
			".au",
			".snd",
			".wav",
			".cda",
			".ivf",
			".wmz",
			".wms",
			".mov",
			".m4a",
			".mp4",
			".m4v",
			".mp4v",
			".3g2",
			".3gp2",
			".3gp",
			".3gpp",
			".aac",
			".adt",
			".adts",
			".m2ts",
			".flac"
		};
	}

}
