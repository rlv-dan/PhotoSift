/*
 *  PhotoSift
 *  Copyright (C) 2013-2014  RL Vision
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
using System.Xml.Serialization;
using System.IO;
using System.Windows.Forms;
using System.Drawing;

namespace PhotoSift
{
    /// <summary>
    /// Helper class for managing settings.
    /// </summary>
    public static class SettingsHandler
	{
		public static string SettingsFile = Path.GetDirectoryName(Application.ExecutablePath) + System.IO.Path.DirectorySeparatorChar + "Settings.xml";

		/// <summary>
		/// Load settings from 'SettingsFile' path. Returns an AppSettings object.
		/// </summary>
		public static AppSettings LoadAppSettings()
		{
			AppSettings settings = new AppSettings();

			if( !System.IO.File.Exists( SettingsHandler.SettingsFile ) ) return settings;

			try
			{
				XmlSerializer xml = new XmlSerializer( typeof( AppSettings ) );
				using( StreamReader reader = new StreamReader( SettingsHandler.SettingsFile ) )
				{
					settings = (AppSettings)xml.Deserialize( reader );
					return settings;
				}
			}
			catch( Exception ex )
			{
				System.Console.WriteLine( "--- Error loading settings ---\n" + ex );
			}
			return settings;
		}

		/// <summary>
		/// Saves the supplied AppSettings object to disk. Save path is taken from SettingsFile
		/// </summary>
		public static void SaveAppSettings( AppSettings settings )
		{
			try
			{
				XmlSerializer xml = new XmlSerializer( settings.GetType() );
				using( StreamWriter writer = new StreamWriter( SettingsHandler.SettingsFile ) )
				{
					xml.Serialize( writer, settings );
				}
			}
			catch( Exception ex )
			{
				System.Console.WriteLine( "--- Error saving settings ---\n" + ex );
			}
		}
	}



	/// <summary>
	/// Font descriptor that can be xml-serialized
	/// </summary>
	[Serializable]
	public class SerializableFont
	{
		public string FontFamily { get; set; }
		public GraphicsUnit GraphicsUnit { get; set; }
		public float Size { get; set; }
		public FontStyle Style { get; set; }

		/// <summary>
		/// Intended for xml serialization purposes only
		/// </summary>
		private SerializableFont() { }

		public SerializableFont( Font f )
		{
			FontFamily = f.FontFamily.Name;
			GraphicsUnit = f.Unit;
			Size = f.Size;
			Style = f.Style;
		}

		public static SerializableFont FromFont( Font f )
		{
			return new SerializableFont( f );
		}

		public Font ToFont()
		{
			return new Font( FontFamily, Size, Style,
				GraphicsUnit );
		}
	}


	/// <summary>
	/// Color descriptor that can be xml-serialized
	/// </summary>
	[Serializable]
	public struct SerializableColor
	{
		public int A;
		public int R;
		public int G;
		public int B;

		public SerializableColor( Color color )
		{
			this.A = color.A;
			this.R = color.R;
			this.G = color.G;
			this.B = color.B;
		}

		public static SerializableColor FromColor( Color color )
		{
			return new SerializableColor( color );
		}

		public Color ToColor()
		{
			return Color.FromArgb( A, R, G, B );
		}
	}

}
