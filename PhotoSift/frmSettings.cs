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
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using System.Reflection;
using System.Globalization;

namespace PhotoSift
{
    public partial class frmSettings : Form
	{
		AppSettings settings;

		public frmSettings( AppSettings settings )
		{
			this.settings = settings;

			InitializeComponent();
			propertyGrid.SelectedObject = settings;
			propertyGrid.Focus();
		}

		private void frmSettings_Load( object sender, EventArgs e )
		{
			if( !settings.FormRect_Settings.IsEmpty )
			{
				this.Left = settings.FormRect_Settings.X;
				this.Top = settings.FormRect_Settings.Y;
				this.Width = settings.FormRect_Settings.Width;
				this.Height = settings.FormRect_Settings.Height;
			}			
		}

		private void frmSettings_FormClosing( object sender, FormClosingEventArgs e )
		{
			if( this.WindowState != FormWindowState.Maximized )
			{
				settings.FormRect_Settings = new Rectangle( this.Left, this.Top, this.Width, this.Height );
			}

		}

		private void frmSettings_KeyDown( object sender, KeyEventArgs e )
		{
			if( e.KeyCode == Keys.F12 || e.KeyCode == Keys.Escape ) this.Close();
		}
	}

	public class EnumTypeConverter : EnumConverter
	{
		private Type m_EnumType;
		public EnumTypeConverter( Type type )
			: base( type )
		{
			m_EnumType = type;
		}

		public override bool CanConvertTo( ITypeDescriptorContext context, Type destType )
		{
			return destType == typeof( string );
		}

		public override object ConvertTo( ITypeDescriptorContext context, CultureInfo culture, object value, Type destType )
		{
			FieldInfo fi = m_EnumType.GetField( Enum.GetName( m_EnumType, value ) );
			DescriptionAttribute dna =
				(DescriptionAttribute)Attribute.GetCustomAttribute(
				fi, typeof( DescriptionAttribute ) );

			if( dna != null )
				return dna.Description;
			else
				return value.ToString();
		}

		public override bool CanConvertFrom( ITypeDescriptorContext context, Type srcType )
		{
			return srcType == typeof( string );
		}

		public override object ConvertFrom( ITypeDescriptorContext context, CultureInfo culture, object value )
		{
			foreach( FieldInfo fi in m_EnumType.GetFields() )
			{
				DescriptionAttribute dna =
				(DescriptionAttribute)Attribute.GetCustomAttribute(
				fi, typeof( DescriptionAttribute ) );

				if( ( dna != null ) && ( (string)value == dna.Description ) )
					return Enum.Parse( m_EnumType, fi.Name );
			}
			return Enum.Parse( m_EnumType, (string)value );
		}
	}
}
