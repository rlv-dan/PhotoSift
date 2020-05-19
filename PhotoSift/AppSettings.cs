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
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Drawing;
using System.Xml.Serialization;
using System.Collections.Generic;
using System.IO;

namespace PhotoSift
{
    /// <summary>
    /// Contains all PhotoSift settings. Most attributes control appearance in the PropertyGrid in frmSettings.
    /// </summary>
    [Serializable]
	[DefaultPropertyAttribute( "FileMode" )]
	public class AppSettings
	{
		// -- Settings shown in the property grid --

		// File Operations Group
		// (space in front of category name is intended; makes it sort first)
		[Category("\u200B" + "File Operations" ), DisplayName( "File mode" ), DescriptionAttribute( "When pressing an action key, should the file be copied or moved?" )]
		[TypeConverter( typeof( EnumTypeConverter ) )]
		public FileOperations FileMode { get; set; }

		[Category("\u200B" + "File Operations" ), DisplayName( "Existing files" ), DescriptionAttribute( "When pressing an action key, and the target folder already contains a file with the same name, what action do you want to take? Append number means a (1) will be added to the end of the filename." )]
		[TypeConverter( typeof( EnumTypeConverter ) )]
		public ExistingFileOptions ExistingFiles { get; set; }

		[Category("\u200B" + "File Operations" ), DisplayName( "Delete mode" ), DescriptionAttribute( "Determines the action to take when pressing the delete key. You can force different modes with Shift+Del (Delete), Alt+Del (Recycle) and Ctrl+Del (Remove from List)" )]
		[TypeConverter( typeof( EnumTypeConverter ) )]
		public DeleteOptions DeleteMode { get; set; }
		[Category("\u200B" + "File Operations" ), DisplayName("Target base folder"), DescriptionAttribute("Target base folder. %PhotoSift% or relative path (not \\ starting) will be replaced with the location of the software.")]
		[EditorAttribute(typeof(FolderNameEditor), typeof(UITypeEditor))]
		public string TargetFolderPath { get; set; } // TargetFolder_Serializable
		[System.Xml.Serialization.XmlIgnore]
		[Browsable(false)]
		public string TargetFolder
		{
			get
			{
				string nPath = this.TargetFolderPath.Replace("%PhotoSift%", System.Windows.Forms.Application.StartupPath);
				if (!Path.IsPathRooted(nPath)) nPath = Path.Combine(System.Windows.Forms.Application.StartupPath, nPath); // For filled in the properties panel
				return nPath;
			}
			set
			{
				if (SaveRelativePaths)
					this.TargetFolderPath = value.Replace(System.Windows.Forms.Application.StartupPath, "%PhotoSift%");
				else
					this.TargetFolderPath = value;
			}
		}

		// Appearance Group
#if RLVISION
		[Category( "Appearance" ), DisplayName( "Background color" ), DescriptionAttribute( "Sets the window background color." )]
		[TypeConverter( typeof( EnumTypeConverter ) )]
		public GrayColors ColorBackground { get; set; }
#else
		[XmlIgnore]
		[Category( "Appearance" ), DisplayName( "Background color" ), DescriptionAttribute( "Sets the window background color." )]
		public Color ColorBackground
		{
			get { return ColorBackground_Serializable.ToColor(); }
			set { ColorBackground_Serializable = new SerializableColor( value ); }
		}
		[Browsable( false )]
		public SerializableColor ColorBackground_Serializable { get; set; }
#endif

		[Category("Appearance"), DisplayName("Gradient background mode"), DescriptionAttribute("Set the mode of gradient background colors.")]
		public LineGradientMode ColorGradientBackgroundMode { get; set; }
	
		[XmlIgnore]
		[Category("Appearance"), DisplayName("Gradient background color 1"), DescriptionAttribute("Set the gradient background colors of the window. The main background color is ignored.")]
		public Color ColorGradientBackgroundOne
		{
			get { return ColorGradientBackgroundOne_Serializable.ToColor(); }
			set { ColorGradientBackgroundOne_Serializable = new SerializableColor(value); }
		}
		[Browsable(false)]
		public SerializableColor ColorGradientBackgroundOne_Serializable { get; set; }
		[XmlIgnore]
		[Category("Appearance"), DisplayName("Gradient background color 2"), DescriptionAttribute("Set the gradient background colors of the window. The main background color is ignored.")]
		public Color ColorGradientBackgroundTwo
		{
			get { return ColorGradientBackgroundTwo_Serializable.ToColor(); }
			set { ColorGradientBackgroundTwo_Serializable = new SerializableColor(value); }
		}
		[Browsable(false)]
		public SerializableColor ColorGradientBackgroundTwo_Serializable { get; set; }
		[Category("Appearance"), DisplayName("Gradient background gamma correction"), DescriptionAttribute("Gamma correction is disabled by default. Enabling it may get better or worse result.")]
		public bool ColorGradientBackgroundGammaCorrection { get; set; }


		[XmlIgnore]
		[Category( "Appearance" ), DisplayName( "Text label color" ), DescriptionAttribute( "Sets the font color of text labels." )]
		public Color ColorLabelFront
		{
			get { return ColorLabelFront_Serializable.ToColor(); }
			set { ColorLabelFront_Serializable = new SerializableColor( value ); }
		}
		[Browsable( false )]
		public SerializableColor ColorLabelFront_Serializable { get; set; }

		[XmlIgnore]
		[Category( "Appearance" ), DisplayName( "Text label background color" ), DescriptionAttribute( "Sets the background color of text labels. Not visible if the background is transparent." )]
		public Color ColorLabelBack
		{
			get { return ColorLabelBack_Serializable.ToColor(); }
			set { ColorLabelBack_Serializable = new SerializableColor( value ); }
		}
		[Browsable( false )]
		public SerializableColor ColorLabelBack_Serializable { get; set; }

		[Browsable( false )]
		public SerializableFont LabelFont_Serializable { get; set; }
		[XmlIgnore]
		[Category( "Appearance" ), DisplayName( "Text label font" ), DescriptionAttribute( "Sets the font used in text labels." )]
		public Font LabelFont
		{
			get { return LabelFont_Serializable.ToFont(); }
			set { LabelFont_Serializable = SerializableFont.FromFont( value ); }
		}

		[Category( "Appearance" ), DisplayName( "Transparent text labels" ), DescriptionAttribute( "Decides if text labels should have transparent or solid color background." )]
		public bool ColorTransparentLabels { get; set; }

		[Category( "Appearance" ), DisplayName( "Custom menu theme" ), DescriptionAttribute( "Enables a custom menu theme, based on the four colors specified below" )]
		public bool CustomMenuColors { get; set; }
		[XmlIgnore]
		[Category( "Appearance" ), DisplayName( "Custom menu theme: Background" ), DescriptionAttribute( "Background color" )]
		public Color CustomMenuColorBackground
		{
			get { return CustomMenuColorBackground_Serializable.ToColor(); }
			set { CustomMenuColorBackground_Serializable = new SerializableColor( value ); }
		}
		[Browsable( false )]
		public SerializableColor CustomMenuColorBackground_Serializable { get; set; }
		[XmlIgnore]
		[Category( "Appearance" ), DisplayName( "Custom menu theme: Text" ), DescriptionAttribute( "Text color" )]
		public Color CustomMenuColorText
		{
			get { return CustomMenuColorText_Serializable.ToColor(); }
			set { CustomMenuColorText_Serializable = new SerializableColor( value ); }
		}
		[Browsable( false )]
		public SerializableColor CustomMenuColorText_Serializable { get; set; }
		[XmlIgnore]
		[Category( "Appearance" ), DisplayName( "Custom menu theme: Border" ), DescriptionAttribute( "Border color" )]
		public Color CustomMenuColorBorder
		{
			get { return CustomMenuColorHighlight_Serializable.ToColor(); }
			set { CustomMenuColorHighlight_Serializable = new SerializableColor( value ); }
		}
		[Browsable( false )]
		public SerializableColor CustomMenuColorHighlight_Serializable { get; set; }
		[XmlIgnore]
		[Category( "Appearance" ), DisplayName( "Custom menu theme: Highlight" ), DescriptionAttribute( "Background color for items under mouse" )]
		public Color CustomMenuColorHightlight
		{
			get { return CustomMenuColorSelected_Serializable.ToColor(); }
			set { CustomMenuColorSelected_Serializable = new SerializableColor( value ); }
		}
		[Browsable( false )]
		public SerializableColor CustomMenuColorSelected_Serializable { get; set; }


		// Controls Group
		[Category("Controls"), DisplayName("Hold key repeat interval"), DescriptionAttribute("Interval (ms) for press and hold a key to repeat actions. The value < 100 ms will disalbe this feature.")]
		public int HoldKeyInterval { get; set; }
		[Category("Controls"), DisplayName("Rewind on pool end"), DescriptionAttribute("Try to rewind when the pool end is reached while handle.")]
		public bool RewindOnEnd { get; set; }
		[Category("Controls"), DisplayName("Loop in pool"), DescriptionAttribute("Cycle your pool when you switch images, there is no pool end screen.")]
		public bool LoopInPool { get; set; }
		[Category( "Controls" ), DisplayName( "Medium jump" ), DescriptionAttribute( "Number of images to skip when doing a medium jump. Medium jump is invoked by holding Ctrl and pressing Left/Right." )]
		public int MediumJump { get; set; }

		[Category( "Controls" ), DisplayName( "Long jump" ), DescriptionAttribute( "Number of images to skip when doing a long jump. Long jump is invoked by holding Shift and pressing Left/Right." )]
		public int LargeJump { get; set; }

		[Category("Controls"), DisplayName("Confirm to clear Threshold"), DescriptionAttribute("Confirm to clear the images pool if the number of images in the pool exceeds this value. The warn is disalbed if set as 0. ")]
		public long WarnThresholdOnClearQueue { get; set; }
		[Category( "Controls" ), DisplayName( "Escape to exit" ), DescriptionAttribute( "Allows you to exit the program by pressing the escape key. In fullscreen, escape always reverts back to window mode." )]
		public bool CloseOnEscape { get; set; }

		[Category( "Controls" ), DisplayName( "On delete show next image" ), DescriptionAttribute( "When an image is deleted, this determined if the program is to show the image positioned before or after the deleted image." )]
		public bool OnDeleteStepForward { get; set; }

		[Category( "Controls" ), DisplayName( "Auto advance interval" ), DescriptionAttribute( "When the auto advance slideshow is enabled, this number determines how long to wait in seconds before showing the next images." )]
		public double AutoAdvanceInterval { get; set; }

		[Category( "Controls" ), DisplayName( "Auto scroll in actual size" ), DescriptionAttribute( "In actual size mode (1:1) you can scroll the image. Auto scroll means you only have to move the mouse to scroll. If turned off, you have to click and hold the mouse button to scroll." )]
		public bool ActualSizeAutoScroll { get; set; }

		[Category( "Controls" ), DisplayName( "Auto scroll: Sensitivity" ), DescriptionAttribute( "Determines how far you have to move the mouse (in pixels) to fully scroll the image from one side to the other." )]
		public int ActualSizeAutoScrollDistance { get; set; }

		[Category( "Controls" ), DisplayName( "Auto scroll: Float image" ), DescriptionAttribute( "Allows the image to freely 'float' around the cursor. Otherwise it will stick to the window borders, limiting the scrolling." )]
		public bool ActualSizeAutoScrollNoLimitInsideForm { get; set; }

		[Category( "Controls" ), DisplayName( "Scale mode: Linear mode" ), DescriptionAttribute( "In scale mode (RMB), the mouse position determines the scale factor. If linear, the picture will snap to the new scaled size. If not linear, the image will keep its original scale, but when you move the mouse the scale factor will differ left and right of the original position." )]
		public bool LinearScale { get; set; }

		[Category( "Controls" ), DisplayName( "Scale mode: Snapping" ), DescriptionAttribute( "In scale mode (RMB), the zoom level can be set to snap to an interval, for example each 10 percent. Default is 0, meaning freely zoom without snapping." )]
		public int FreeZoomSnap { get; set; }

		[Category( "Controls" ), DisplayName( "Keyboard zoom levels" ), DescriptionAttribute( "Enter a comma delimited list of zoom levels to use when using keyboard zoom (+/- keys). Default is: '25,50,75,100,125,150,200'. Enter a single value to use this value as incremental steps instead." )]
		public string ZoomSteps { get; set; }

		[Category( "Controls" ), DisplayName( "Limit zoom to windows size" ), DescriptionAttribute( "If enabled, zooming maxes out when the image size is equal to the window size. In other words, you can not zoom images larger that the current window size." )]
		public bool ZoomLimitMaxToWindowSize { get; set; }

		// Display Group
		[Category( "Display" ), DisplayName( "Info label" ), DescriptionAttribute( "Info label is the one in the top left corner. It shows information about the currently loaded image. In windowed mode, this information is also shown in the window title." )]
		[TypeConverter( typeof( EnumTypeConverter ) )]
		public ShowModes ShowInfoLabel { get; set; }

		[Category( "Display" ), DisplayName( "Mode label" ), DescriptionAttribute( "Info label is the one in the bottom left corner. It shows notifications about modes changes and similar. " )]
		[TypeConverter( typeof( EnumTypeConverter ) )]
		public ShowModes ShowModeLabel { get; set; }

		[Category( "Display" ), DisplayName( "Info label format" ), DescriptionAttribute( "Here you can decide what to show in the info label. Available format tags: Filename: %f, Parent folder: %d, Full path: %p, Image width: %w, Image height: %h, Filesize: %s, New line: %n, Image pool total count: %t, Image pool current number: %c" )]
		public string InfoLabelFormat { get; set; }
		[Category("Display"), DisplayName("Info label format for videos"), DescriptionAttribute("Here you can decide what to show in the info label. Available format tags: Filename: %f, Parent folder: %d, Full path: %p, width: %w, height: %h, Filesize: %s, New line: %n, pool total count: %t, pool current number: %c, %time: media duration")]
		public string InfoLabelFormatVideo { get; set; }

		[Category( "Display" ), DisplayName( "Hide cursor in fullscreen" ), DescriptionAttribute( "If enabled, the cursor will automatically be hidden on inactivity." )]
		public bool FullscreenHideCursor { get; set; }

		[Category( "Display" ), DisplayName( "Enlarge small images" ), DescriptionAttribute( "Images smaller that the current view can be enlarged to fill the whole view. Hotkey: F4" )]
		public bool EnlargeSmallImages { get; set; }

#if RLVISION
		[Category( "Display" ), DisplayName( "Auto move to best suited screen" ), DescriptionAttribute( "In a multi-monitor environment, if the application is in fullscreen mode the windows will automatically moves to the best suited monitor (based on aspect ratio) when changing picture." )]
		public bool AutoMoveToScreen { get; set; }
#endif

		// System Group
		[Category( "System" ), DisplayName( "Disable screensaver" ), DescriptionAttribute( "Disable any screensaver, sleep mode etc while the programis running. Restart is required if changed." )]
		public bool PreventSleep { get; set; }


		// Key Folder group
		[Category( "Key Folders" ), DisplayName( "A" ), DescriptionAttribute( "Sets the destination folder where images go when this key is pressing. This can be a subfolder relative to the current base target folder, or a complete path. Clear to use default. See readme file for a more detailed explanation of the various options." )]
		[EditorAttribute( typeof( FolderNameEditor ), typeof( UITypeEditor ) )]
		public string KeyFolder_A { get; set; }
		[Category( "Key Folders" ), DisplayName( "B" ), DescriptionAttribute( "Sets the destination folder where images go when this key is pressing. This can be a subfolder relative to the current base target folder, or a complete path. Clear to use default. See readme file for a more detailed explanation of the various options." )]
		[EditorAttribute( typeof( FolderNameEditor ), typeof( UITypeEditor ) )]
		public string KeyFolder_B { get; set; }
		[Category( "Key Folders" ), DisplayName( "C" ), DescriptionAttribute( "Sets the destination folder where images go when this key is pressing. This can be a subfolder relative to the current base target folder, or a complete path. Clear to use default. See readme file for a more detailed explanation of the various options." )]
		[EditorAttribute( typeof( FolderNameEditor ), typeof( UITypeEditor ) )]
		public string KeyFolder_C { get; set; }
		[Category( "Key Folders" ), DisplayName( "D" ), DescriptionAttribute( "Sets the destination folder where images go when this key is pressing. This can be a subfolder relative to the current base target folder, or a complete path. Clear to use default. See readme file for a more detailed explanation of the various options." )]
		[EditorAttribute( typeof( FolderNameEditor ), typeof( UITypeEditor ) )]
		public string KeyFolder_D { get; set; }
		[Category( "Key Folders" ), DisplayName( "E" ), DescriptionAttribute( "Sets the destination folder where images go when this key is pressing. This can be a subfolder relative to the current base target folder, or a complete path. Clear to use default. See readme file for a more detailed explanation of the various options." )]
		[EditorAttribute( typeof( FolderNameEditor ), typeof( UITypeEditor ) )]
		public string KeyFolder_E { get; set; }
		[Category( "Key Folders" ), DisplayName( "F" ), DescriptionAttribute( "Sets the destination folder where images go when this key is pressing. This can be a subfolder relative to the current base target folder, or a complete path. Clear to use default. See readme file for a more detailed explanation of the various options." )]
		[EditorAttribute( typeof( FolderNameEditor ), typeof( UITypeEditor ) )]
		public string KeyFolder_F { get; set; }
		[Category( "Key Folders" ), DisplayName( "G" ), DescriptionAttribute( "Sets the destination folder where images go when this key is pressing. This can be a subfolder relative to the current base target folder, or a complete path. Clear to use default. See readme file for a more detailed explanation of the various options." )]
		[EditorAttribute( typeof( FolderNameEditor ), typeof( UITypeEditor ) )]
		public string KeyFolder_G { get; set; }
		[Category( "Key Folders" ), DisplayName( "H" ), DescriptionAttribute( "Sets the destination folder where images go when this key is pressing. This can be a subfolder relative to the current base target folder, or a complete path. Clear to use default. See readme file for a more detailed explanation of the various options." )]
		[EditorAttribute( typeof( FolderNameEditor ), typeof( UITypeEditor ) )]
		public string KeyFolder_H { get; set; }
		[Category( "Key Folders" ), DisplayName( "I" ), DescriptionAttribute( "Sets the destination folder where images go when this key is pressing. This can be a subfolder relative to the current base target folder, or a complete path. Clear to use default. See readme file for a more detailed explanation of the various options." )]
		[EditorAttribute( typeof( FolderNameEditor ), typeof( UITypeEditor ) )]
		public string KeyFolder_I { get; set; }
		[Category( "Key Folders" ), DisplayName( "J" ), DescriptionAttribute( "Sets the destination folder where images go when this key is pressing. This can be a subfolder relative to the current base target folder, or a complete path. Clear to use default. See readme file for a more detailed explanation of the various options." )]
		[EditorAttribute( typeof( FolderNameEditor ), typeof( UITypeEditor ) )]
		public string KeyFolder_J { get; set; }
		[Category( "Key Folders" ), DisplayName( "K" ), DescriptionAttribute( "Sets the destination folder where images go when this key is pressing. This can be a subfolder relative to the current base target folder, or a complete path. Clear to use default. See readme file for a more detailed explanation of the various options." )]
		[EditorAttribute( typeof( FolderNameEditor ), typeof( UITypeEditor ) )]
		public string KeyFolder_K { get; set; }
		[Category( "Key Folders" ), DisplayName( "L" ), DescriptionAttribute( "Sets the destination folder where images go when this key is pressing. This can be a subfolder relative to the current base target folder, or a complete path. Clear to use default. See readme file for a more detailed explanation of the various options." )]
		[EditorAttribute( typeof( FolderNameEditor ), typeof( UITypeEditor ) )]
		public string KeyFolder_L { get; set; }
		[Category( "Key Folders" ), DisplayName( "M" ), DescriptionAttribute( "Sets the destination folder where images go when this key is pressing. This can be a subfolder relative to the current base target folder, or a complete path. Clear to use default. See readme file for a more detailed explanation of the various options." )]
		[EditorAttribute( typeof( FolderNameEditor ), typeof( UITypeEditor ) )]
		public string KeyFolder_M { get; set; }
		[Category( "Key Folders" ), DisplayName( "N" ), DescriptionAttribute( "Sets the destination folder where images go when this key is pressing. This can be a subfolder relative to the current base target folder, or a complete path. Clear to use default. See readme file for a more detailed explanation of the various options." )]
		[EditorAttribute( typeof( FolderNameEditor ), typeof( UITypeEditor ) )]
		public string KeyFolder_N { get; set; }
		[Category( "Key Folders" ), DisplayName( "O" ), DescriptionAttribute( "Sets the destination folder where images go when this key is pressing. This can be a subfolder relative to the current base target folder, or a complete path. Clear to use default. See readme file for a more detailed explanation of the various options." )]
		[EditorAttribute( typeof( FolderNameEditor ), typeof( UITypeEditor ) )]
		public string KeyFolder_O { get; set; }
		[Category( "Key Folders" ), DisplayName( "P" ), DescriptionAttribute( "Sets the destination folder where images go when this key is pressing. This can be a subfolder relative to the current base target folder, or a complete path. Clear to use default. See readme file for a more detailed explanation of the various options." )]
		[EditorAttribute( typeof( FolderNameEditor ), typeof( UITypeEditor ) )]
		public string KeyFolder_P { get; set; }
		[Category( "Key Folders" ), DisplayName( "Q" ), DescriptionAttribute( "Sets the destination folder where images go when this key is pressing. This can be a subfolder relative to the current base target folder, or a complete path. Clear to use default. See readme file for a more detailed explanation of the various options." )]
		[EditorAttribute( typeof( FolderNameEditor ), typeof( UITypeEditor ) )]
		public string KeyFolder_Q { get; set; }
		[Category( "Key Folders" ), DisplayName( "R" ), DescriptionAttribute( "Sets the destination folder where images go when this key is pressing. This can be a subfolder relative to the current base target folder, or a complete path. Clear to use default. See readme file for a more detailed explanation of the various options." )]
		[EditorAttribute( typeof( FolderNameEditor ), typeof( UITypeEditor ) )]
		public string KeyFolder_R { get; set; }
		[Category( "Key Folders" ), DisplayName( "S" ), DescriptionAttribute( "Sets the destination folder where images go when this key is pressing. This can be a subfolder relative to the current base target folder, or a complete path. Clear to use default. See readme file for a more detailed explanation of the various options." )]
		[EditorAttribute( typeof( FolderNameEditor ), typeof( UITypeEditor ) )]
		public string KeyFolder_S { get; set; }
		[Category( "Key Folders" ), DisplayName( "T" ), DescriptionAttribute( "Sets the destination folder where images go when this key is pressing. This can be a subfolder relative to the current base target folder, or a complete path. Clear to use default. See readme file for a more detailed explanation of the various options." )]
		[EditorAttribute( typeof( FolderNameEditor ), typeof( UITypeEditor ) )]
		public string KeyFolder_T { get; set; }
		[Category( "Key Folders" ), DisplayName( "U" ), DescriptionAttribute( "Sets the destination folder where images go when this key is pressing. This can be a subfolder relative to the current base target folder, or a complete path. Clear to use default. See readme file for a more detailed explanation of the various options." )]
		[EditorAttribute( typeof( FolderNameEditor ), typeof( UITypeEditor ) )]
		public string KeyFolder_U { get; set; }
		[Category( "Key Folders" ), DisplayName( "V" ), DescriptionAttribute( "Sets the destination folder where images go when this key is pressing. This can be a subfolder relative to the current base target folder, or a complete path. Clear to use default. See readme file for a more detailed explanation of the various options." )]
		[EditorAttribute( typeof( FolderNameEditor ), typeof( UITypeEditor ) )]
		public string KeyFolder_V { get; set; }
		[Category( "Key Folders" ), DisplayName( "W" ), DescriptionAttribute( "Sets the destination folder where images go when this key is pressing. This can be a subfolder relative to the current base target folder, or a complete path. Clear to use default. See readme file for a more detailed explanation of the various options." )]
		[EditorAttribute( typeof( FolderNameEditor ), typeof( UITypeEditor ) )]
		public string KeyFolder_W { get; set; }
		[Category( "Key Folders" ), DisplayName( "X" ), DescriptionAttribute( "Sets the destination folder where images go when this key is pressing. This can be a subfolder relative to the current base target folder, or a complete path. Clear to use default. See readme file for a more detailed explanation of the various options." )]
		[EditorAttribute( typeof( FolderNameEditor ), typeof( UITypeEditor ) )]
		public string KeyFolder_X { get; set; }
		[Category( "Key Folders" ), DisplayName( "Y" ), DescriptionAttribute( "Sets the destination folder where images go when this key is pressing. This can be a subfolder relative to the current base target folder, or a complete path. Clear to use default. See readme file for a more detailed explanation of the various options." )]
		[EditorAttribute( typeof( FolderNameEditor ), typeof( UITypeEditor ) )]
		public string KeyFolder_Y { get; set; }
		[Category( "Key Folders" ), DisplayName( "Z" ), DescriptionAttribute( "Sets the destination folder where images go when this key is pressing. This can be a subfolder relative to the current base target folder, or a complete path. Clear to use default. See readme file for a more detailed explanation of the various options." )]
		[EditorAttribute( typeof( FolderNameEditor ), typeof( UITypeEditor ) )]
		public string KeyFolder_Z { get; set; }
		[Category( "Key Folders" ), DisplayName( "0" ), DescriptionAttribute( "Sets the destination folder where images go when this key is pressing. This can be a subfolder relative to the current base target folder, or a complete path. Clear to use default. See readme file for a more detailed explanation of the various options." )]
		[EditorAttribute( typeof( FolderNameEditor ), typeof( UITypeEditor ) )]
		public string KeyFolder_0 { get; set; }
		[Category( "Key Folders" ), DisplayName( "1" ), DescriptionAttribute( "Sets the destination folder where images go when this key is pressing. This can be a subfolder relative to the current base target folder, or a complete path. Clear to use default. See readme file for a more detailed explanation of the various options." )]
		[EditorAttribute( typeof( FolderNameEditor ), typeof( UITypeEditor ) )]
		public string KeyFolder_1 { get; set; }
		[Category( "Key Folders" ), DisplayName( "2" ), DescriptionAttribute( "Sets the destination folder where images go when this key is pressing. This can be a subfolder relative to the current base target folder, or a complete path. Clear to use default. See readme file for a more detailed explanation of the various options." )]
		[EditorAttribute( typeof( FolderNameEditor ), typeof( UITypeEditor ) )]
		public string KeyFolder_2 { get; set; }
		[Category( "Key Folders" ), DisplayName( "3" ), DescriptionAttribute( "Sets the destination folder where images go when this key is pressing. This can be a subfolder relative to the current base target folder, or a complete path. Clear to use default. See readme file for a more detailed explanation of the various options." )]
		[EditorAttribute( typeof( FolderNameEditor ), typeof( UITypeEditor ) )]
		public string KeyFolder_3 { get; set; }
		[Category( "Key Folders" ), DisplayName( "4" ), DescriptionAttribute( "Sets the destination folder where images go when this key is pressing. This can be a subfolder relative to the current base target folder, or a complete path. Clear to use default. See readme file for a more detailed explanation of the various options." )]
		[EditorAttribute( typeof( FolderNameEditor ), typeof( UITypeEditor ) )]
		public string KeyFolder_4 { get; set; }
		[Category( "Key Folders" ), DisplayName( "5" ), DescriptionAttribute( "Sets the destination folder where images go when this key is pressing. This can be a subfolder relative to the current base target folder, or a complete path. Clear to use default. See readme file for a more detailed explanation of the various options." )]
		[EditorAttribute( typeof( FolderNameEditor ), typeof( UITypeEditor ) )]
		public string KeyFolder_5 { get; set; }
		[Category( "Key Folders" ), DisplayName( "6" ), DescriptionAttribute( "Sets the destination folder where images go when this key is pressing. This can be a subfolder relative to the current base target folder, or a complete path. Clear to use default. See readme file for a more detailed explanation of the various options." )]
		[EditorAttribute( typeof( FolderNameEditor ), typeof( UITypeEditor ) )]
		public string KeyFolder_6 { get; set; }
		[Category( "Key Folders" ), DisplayName( "7" ), DescriptionAttribute( "Sets the destination folder where images go when this key is pressing. This can be a subfolder relative to the current base target folder, or a complete path. Clear to use default. See readme file for a more detailed explanation of the various options." )]
		[EditorAttribute( typeof( FolderNameEditor ), typeof( UITypeEditor ) )]
		public string KeyFolder_7 { get; set; }
		[Category( "Key Folders" ), DisplayName( "8" ), DescriptionAttribute( "Sets the destination folder where images go when this key is pressing. This can be a subfolder relative to the current base target folder, or a complete path. Clear to use default. See readme file for a more detailed explanation of the various options." )]
		[EditorAttribute( typeof( FolderNameEditor ), typeof( UITypeEditor ) )]
		public string KeyFolder_8 { get; set; }
		[Category( "Key Folders" ), DisplayName( "9" ), DescriptionAttribute( "Sets the destination folder where images go when this key is pressing. This can be a subfolder relative to the current base target folder, or a complete path. Clear to use default. See readme file for a more detailed explanation of the various options." )]
		[EditorAttribute( typeof( FolderNameEditor ), typeof( UITypeEditor ) )]
		public string KeyFolder_9 { get; set; }

		// Cache settings
		[Category("Cache"), DisplayName("Cache Ahead (earlier)"), DescriptionAttribute("Pre-reading x pictures earlier than the current image in image pool.")]
		public int CacheAhead { get; set; }
		[Category("Cache"), DisplayName("Cache Behind (later)"), DescriptionAttribute("Pre-reading x pictures later than the current image in image pool.")]
		public int CacheBehind { get; set; }

		// FileType settings
		[Category("File Type"), DisplayName("Image exts"), DescriptionAttribute("File extensions allowed to be added to the pool. Will be ignored if the Check MIME is on.")]
		public string[] allowsPicExts { get; set; }
		[Category("File Type"), DisplayName("Video exts"), DescriptionAttribute("File extensions allowed to be added to the pool. Will be ignored if the Check MIME is on.")]
		public string[] allowsVidExts { get; set; }
		[Category("File Type"), DisplayName("Check MIME"), DescriptionAttribute("Check the actual MIME type of the file instead of checking the extensions. May be slower. Unavailable means that the related function failed to load.")]
		public FeatureSwitch FileMIMEChecker { get; set; }
		[Category("File Type"), DisplayName("Allowed MIME"), DescriptionAttribute("File MIME types allowed to be added to the pool. Work only while the Check MIME option be turn on. Semicolon separated. Spaces on edge are ignored. Default: 'image/;video/;audio/'.")]
		public string allowsMIME { get; set; }

		// Misc settings
		[Category("Misc"), DisplayName("Copy action"), DescriptionAttribute("Sets the action type when you press Ctrl+C or click the \"Copy to clipboard\" menu in this software.")]
		public CopytoClipboardOptions CopyActionType { get; set; }
		[Category("Misc"), DisplayName("Save relative paths"), DescriptionAttribute("Save the path relative to the location of the program, for paths such as the target folder.")]
		public bool SaveRelativePaths { get; set; }

		// Settings located on the GUI menus (not visible in the property grid)
		[Browsable( false )]
		public bool AddInRandomOrder { get; set; }
		[Browsable(false)]
		public bool ResetViewModeOnPictureChange { get; set; }
		[Browsable(false)]
		public bool MoveIncludingCurrent { get; set; }


		// Hidden / Non-user Settings
		[Browsable( false )]
		public Rectangle FormRect_Main { get; set; }
		[Browsable( false )]
		public Rectangle FormRect_Settings { get; set; }
		[Browsable(false)]
		public System.Windows.Forms.FormWindowState WindowState { get; set; }

		[Browsable( false )]
		public string LastFolder_AddFolder { get; set; }
		[Browsable( false )]
		public string LastFolder_AddFiles { get; set; }

		[Browsable( false )]
		public bool FirstTimeUsing { get; set; }

		[Browsable( false )]
		public int FullscreenCursorAutoHideTime { get; set; }

		[Browsable( false )]
		public DateTime Stats_FirstLaunchDate { get; set; }
		[Browsable( false )]
		public int Stats_StartupCount { get; set; }
		[Browsable( false )]
		public int Stats_LoadedPics { get; set; }
		[Browsable( false )]
		public int Stats_RenamedPics { get; set; }
		[Browsable( false )]
		public int Stats_MovedPics { get; set; }
		[Browsable( false )]
		public int Stats_CopiedPics { get; set; }
		[Browsable( false )]
		public int Stats_DeletedPics { get; set; }

		[XmlIgnore]
		public Dictionary<string, object> defaultSettings = new Dictionary<string, object>();
		public AppSettings()
		{
			// File Operations Group
			defaultSettings.Add("FileMode", FileOperations.Move);
			defaultSettings.Add("ExistingFiles", ExistingFileOptions.AppendNumber);
			defaultSettings.Add("DeleteMode", DeleteOptions.RecycleBin);
			defaultSettings.Add("TargetFolderPath", System.Windows.Forms.Application.StartupPath);

			// Appearance Group
#if RLVISION
			defaultSettings.Add("ColorBackground", GrayColors.Col7);
#else
			defaultSettings.Add("ColorBackground", Color.Black);
#endif
			defaultSettings.Add("ColorGradientBackgroundMode", LineGradientMode.Off);
			defaultSettings.Add("ColorGradientBackgroundOne", Color.Gray);
			defaultSettings.Add("ColorGradientBackgroundTwo", Color.Black);
			defaultSettings.Add("ColorGradientBackgroundGammaCorrection", false);
			defaultSettings.Add("ColorLabelFront", Color.Gray);
			defaultSettings.Add("ColorLabelBack", Color.Black);
			defaultSettings.Add("ColorTransparentLabels", true);
			defaultSettings.Add("LabelFont", new Font("Arial", 10, FontStyle.Regular));
			defaultSettings.Add("CustomMenuColors", true);
			defaultSettings.Add("CustomMenuColorBackground", Color.FromArgb(255, 45, 45, 45));
			defaultSettings.Add("CustomMenuColorText", Color.FromArgb(255, 255, 255, 255));
			defaultSettings.Add("CustomMenuColorBorder", Color.FromArgb(255, 128, 128, 128));
			defaultSettings.Add("CustomMenuColorHightlight", Color.FromArgb(255, 65, 65, 65));

			// Controls Group
			defaultSettings.Add("HoldKeyInterval", 1000);
			defaultSettings.Add("RewindOnEnd", false);
			defaultSettings.Add("LoopInPool", false);
			defaultSettings.Add("MediumJump", 10);
			defaultSettings.Add("LargeJump", 25);
			defaultSettings.Add("WarnThresholdOnClearQueue", 0);
			defaultSettings.Add("CloseOnEscape", false);
			defaultSettings.Add("OnDeleteStepForward", true);
			defaultSettings.Add("AutoAdvanceInterval", 4.5);
			defaultSettings.Add("ActualSizeAutoScroll", true);
			defaultSettings.Add("ActualSizeAutoScrollDistance", 100);
			defaultSettings.Add("ActualSizeAutoScrollNoLimitInsideForm", false);
			defaultSettings.Add("LinearScale", false);
			defaultSettings.Add("FreeZoomSnap", 0);
			defaultSettings.Add("ZoomSteps", "5,10,25,50,75,100,125,150,175,200");
			defaultSettings.Add("ZoomLimitMaxToWindowSize", false);

			// Display Group
			defaultSettings.Add("ShowInfoLabel", ShowModes.FullscreenOnly);
			defaultSettings.Add("ShowModeLabel", ShowModes.AlwaysShow);
			defaultSettings.Add("InfoLabelFormat", "(%c / %t) %f");
			defaultSettings.Add("InfoLabelFormatVideo", "(%c / %t) %f  %time  %w x %h");
			defaultSettings.Add("FullscreenHideCursor", true);
			defaultSettings.Add("EnlargeSmallImages", false);
#if RLVISION
			defaultSettings.Add("AutoMoveToScreen", false);
#endif
			// System Group
			defaultSettings.Add("PreventSleep", false);

			// Cache settings
			defaultSettings.Add("CacheAhead", 2);
			defaultSettings.Add("CacheBehind", 1);

			// File Type
			defaultSettings.Add("allowsPicExts", Util.Def_allowsPicExts);
			defaultSettings.Add("allowsVidExts", Util.Def_allowsVideoExts);
			defaultSettings.Add("FileMIMEChecker", FeatureSwitch.Disabled);
			defaultSettings.Add("allowsMIME", "image/);video/);audio/");

			// Misc
			defaultSettings.Add("SaveRelativePaths", true);
			defaultSettings.Add("CopyActionType", CopytoClipboardOptions.Bitmap);

			// GUI settings
			defaultSettings.Add("AddInRandomOrder", false);
			defaultSettings.Add("ResetViewModeOnPictureChange", true);
			defaultSettings.Add("MoveIncludingCurrent", false);

			// Hidden settings
			defaultSettings.Add("FormRect_Main", new Rectangle());
			defaultSettings.Add("FormRect_Settings", new Rectangle());
			defaultSettings.Add("FirstTimeUsing", true);
			defaultSettings.Add("FullscreenCursorAutoHideTime", 3000);
			defaultSettings.Add("Stats_FirstLaunchDate", DateTime.Now);
			defaultSettings.Add("Stats_StartupCount", 0);
			defaultSettings.Add("Stats_LoadedPics", 0);
			defaultSettings.Add("Stats_RenamedPics", 0);
			defaultSettings.Add("Stats_MovedPics", 0);
			defaultSettings.Add("Stats_CopiedPics", 0);
			defaultSettings.Add("Stats_DeletedPics", 0);

			foreach( System.Reflection.PropertyInfo Prop in typeof( AppSettings ).GetProperties() )
			{
				if (Prop.Name.StartsWith("KeyFolder_"))
					defaultSettings.Add(Prop.Name, "");

				if (defaultSettings.TryGetValue(Prop.Name, out object value))
					Prop.SetValue(this, value); // apply default settings
			}
		}

	}

	// Setting values

	public enum ShowModes
	{
		[Description( "Always Show" )]
		AlwaysShow,
		[Description( "Always Hide" )]
		AlwaysHide,
		[Description( "Fullscreen Only" )]
		FullscreenOnly,
		[Description( "Windowed Only" )]
		WindowedOnly,
	}

	public enum YesNo
	{
		[Description( "Yes" )]
		Yes,
		[Description( "No" )]
		No,
	}

	public enum DeleteOptions
	{
		[Description( "Delete File" )]
		Delete,
		[Description( "Delete to Recycle Bin" )]
		RecycleBin,
		[Description( "Only Remove from Image Pool" )]
		RemoveFromList,
	}

	public enum FileOperations
	{
		[Description( "Copy" )]
		Copy,
		[Description( "Move" )]
		Move,
	}

	public enum ExistingFileOptions
	{
		[Description( "Overwrite" )]
		Overwrite,
		[Description( "Append Number" )]
		AppendNumber,
		[Description( "Skip" )]
		Skip,
	}

	public enum CopytoClipboardOptions
	{
		[Description("Bitmap")]
		Bitmap,
		[Description("File")]
		File,
		[Description("File Path")]
		FilePath,
	}
	public enum FeatureSwitch
	{
		[Description("Disabled")]
		Disabled = 0,
		[Description("Enabled")]
		Enabled = 1,
		[Description("Unavailable")]
		Unavailable = -1,
	}
	public enum LineGradientMode
	{
		Off = -1,
		Horizontal = 0,
		Vertical = 1,
		ForwardDiagonal = 2,
		BackwardDiagonal = 3
	}

#if RLVISION
	public enum GrayColors
	{
		[Description( "White" )]
		Col1 = 255,
		[Description( "Lighter Gray (30)" )]
		Col2 = 191,
		[Description( "Light Gray (40)" )]
		Col3 = 98,
		[Description( "Medium Gray (51)" )]
		Col4 = 51,
		[Description( "Dark Gray (98)" )]
		Col5 = 40,
		[Description( "Darker Gray (191)" )]
		Col6 = 30,
		[Description( "Black" )]
		Col7 = 0,
	}
#endif

}


