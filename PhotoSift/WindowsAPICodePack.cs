/*
 *  PhotoSift
 *  Copyright (C) 2013-2020  RL Vision, YFdyh000
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

namespace PhotoSift
{
    /// <summary>
    /// This class contains various static methods of generic kind
    /// </summary>
    public static class WindowsAPICodePack
	{
		public static string GetFileMIME(string filePathWithExtension)
		{
			try
			{
				var shellFile = Microsoft.WindowsAPICodePack.Shell.ShellFile.FromFilePath(filePathWithExtension);
				var prop = shellFile.Properties.GetProperty(Microsoft.WindowsAPICodePack.Shell.PropertySystem.SystemProperties.System.MIMEType);
				return prop.FormatForDisplay(Microsoft.WindowsAPICodePack.Shell.PropertySystem.PropertyDescriptionFormatOptions.None);
			}
			catch { return ""; }
		}
	}

}
