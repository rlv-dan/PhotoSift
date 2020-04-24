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
using System.Collections.Generic;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using System.IO;
using System.Linq;

namespace PhotoSift
{
    public partial class frmMain : Form
	{
		// Local variables
		private ImageCache imageCache = new ImageCache();
		private List<string> pics = new List<string>();
		private WinApi winApi = new WinApi();
		private AppSettings settings;
		private FileManagement fileManagement;
		private Image TransformedImage = null;	// a copy of the original image when rotating/flipping

		private int iCurrentPic = 0;
		private bool bAutoAdvanceEnabled = false;
		
		private bool bFullScreen = false;
		private FormWindowState NormalWindowState;
		private Rectangle NormalWindowStateFormRect;

		private bool bMenuInUse = false;
		private bool bEscIsPassed = false;

		private bool bCursorVisible = true;
		private int FullScreenCursorLastMouseX = -1;
		private bool bPreventAutoHideCursor = false;

		// Variables managing the scale modes
		private enum ScaleMode
		{
			NormalFitWindow,
			ActualSize,
			ZoomWithMouse,
			ManualZoomPercentage,
			ManualZoomWidth,
			ManualZoomHeight
		}
		private ScaleMode CurrentScaleMode = ScaleMode.NormalFitWindow;
		private Point ScaleModeOriginalPoint = new Point( -1, -1 );
		private float ScaleModeOriginalScale;
		private Rectangle NormalFitWindowPictureSize;
		private PointF ScaleModeOriginalRelativePoint;
		private float ResizeCurrentPercentage;
		private int ResizeCurrentHeight;
		private int ResizeCurrentWidth;
		private float CurrentAspectRatio = 1;
		private bool bMouseHold = false;
		private Point LastMouseHoldPoint = new Point( -1, -1 );
		//


		// Constructor
		public frmMain()
		{
			InitializeComponent();
		}


		private void frmMain_Load( object sender, EventArgs e )
		{
			fileManagement = new FileManagement( this );

			winApi.HookMouse();

			this.MouseWheel += new MouseEventHandler( MouseWheelEvent );
			foreach( Control control in Controls ) control.MouseMove += RedirectMouseMove;
			
			// Load settings
			settings = SettingsHandler.LoadAppSettings();
			if( !settings.FormRect_Main.IsEmpty )
			{
				this.Left = settings.FormRect_Main.X;
				this.Top = settings.FormRect_Main.Y;
				this.Width = settings.FormRect_Main.Width;
				this.Height = settings.FormRect_Main.Height;
			}
			this.WindowState = settings.WindowState;

			settings.Stats_StartupCount++;
			
			ApplySettings();
			if( settings.PreventSleep ) winApi.PreventSleep();

			// Setup GUI
			Util.CenterControl( lblHeader, picLogo.Image.Height / 2 + 20 );
			lblStatus.Text = lblInfoLabel.Text = "";
			ShowHideLabels();
			UpdateMenuEnabledDisabled();

			// Attempt to load files or folders passed via command line
			string[] args = Util.ParseArguments( Environment.CommandLine );
			if( args.Length > 0 ) AddFiles( args ); else ShowNextPic( 0 );

#if DEBUG && RLVISION
			// Development settings
			settings.FileMode = FileOperations.Copy;
			settings.DeleteMode = DeleteOptions.RemoveFromList;
			settings.TargetFolder = @"t:\PhotoSift Output";
			// Load test suite
			AddFiles( new string[] { @"D:\grid.png", @"D:\grid2.png" } );
			AddFiles( new string[] { @"D:\temp\icons temp\PicSort" } );
			AddFiles( new string[] { @"D:\Temp\Comics" } );
#endif
		}

		/// <summary>
		/// Updates the GUI to match the current settings
		/// </summary>
		private void ApplySettings()
		{
			timerAutoAdvance.Interval = (int)( settings.AutoAdvanceInterval * 1000 );
			timerMouseHider.Interval = settings.FullscreenCursorAutoHideTime;

			mnuAddInRandomOrder.Checked = settings.AddInRandomOrder;
			mnuResetViewMode.Checked = settings.ResetViewModeOnPictureChange;

			ShowHideLabels();

			// Apply colors
#if RLVISION
			panelMain.BackColor = Color.FromArgb( 255, (int)settings.ColorBackground, (int)settings.ColorBackground, (int)settings.ColorBackground );
#else
			panelMain.BackColor = settings.ColorBackground;
#endif
			picCurrent.BackColor = Color.Transparent;
			lblInfoLabel.ForeColor = settings.ColorLabelFront;
			lblStatus.ForeColor = settings.ColorLabelFront;
			lblHeader.ForeColor = settings.ColorLabelFront;
			if( settings.ColorTransparentLabels )
			{
				lblInfoLabel.BackColor = Color.Transparent;
				lblStatus.BackColor = Color.Transparent;
				lblHeader.BackColor = Color.Transparent;
			}
			else
			{
				lblInfoLabel.BackColor = settings.ColorLabelBack;
				lblStatus.BackColor = settings.ColorLabelBack;
				lblHeader.BackColor = settings.ColorLabelBack;
			}

			// fonts
			lblInfoLabel.Font = settings.LabelFont;
			lblStatus.Font = settings.LabelFont;
			lblHeader.Font = settings.LabelFont;

			lblStatus.Top = panelMain.ClientSize.Height - lblStatus.Height - 2;

			if( settings.CustomMenuColors )
			{
				menuStripMain.Renderer = new CustomMenuRenderer( settings );
			}
			/* todo: reset menu renderer
			else
			{
				menuStripMain.Renderer = new ToolStripProfessionalRenderer();
			}*/
		}

		private void frmMain_FormClosing( object sender, FormClosingEventArgs e )
		{
			// Save settings
			settings.WindowState = !bFullScreen ? this.WindowState : NormalWindowState;

			if( bFullScreen || this.WindowState == FormWindowState.Maximized)
				settings.FormRect_Main = new Rectangle( NormalWindowStateFormRect.Left, NormalWindowStateFormRect.Top, NormalWindowStateFormRect.Width, NormalWindowStateFormRect.Height );
			else
				settings.FormRect_Main = new Rectangle( this.Left, this.Top, this.Width, this.Height );
			SettingsHandler.SaveAppSettings( settings );
		}

		private void frmMain_Resize( object sender, EventArgs e )
		{
			SetScaleMode( CurrentScaleMode, false );
			Util.CenterControl( lblHeader, picLogo.Image.Height / 2 + 20 );

#if !NOWMP
			if (wmpCurrent.URL != null)
			{
				wmpCurrent.Width = picCurrent.Width;
				wmpCurrent.Height = picCurrent.Height;
				wmpCurrent.Dock = DockStyle.Fill;
			}
#endif
		}

		private void frmMain_Shown( object sender, EventArgs e )
		{
			if( settings.FirstTimeUsing )
			{
				settings.FirstTimeUsing = false;
				if( MessageBox.Show( "New users should read 'Getting Started' in the help file. Open it now?", "Welcome to " + Util.GetAppName(), MessageBoxButtons.YesNo ) == System.Windows.Forms.DialogResult.Yes )
				{
					mnuShowHelp_Click( this, new EventArgs() );
				}
			}
		}

		// Drag Drop
		private void frmMain_DragEnter( object sender, DragEventArgs e )
		{
			if( e.Data.GetDataPresent( DataFormats.FileDrop ) )
			{
				e.Effect = DragDropEffects.Copy;
			}
			else
			{
				e.Effect = DragDropEffects.None;
			}
		}
		private void frmMain_DragDrop( object sender, DragEventArgs e )
		{
			if( e.Data.GetDataPresent( DataFormats.FileDrop ) )
			{
				string[] files = (string[])e.Data.GetData( DataFormats.FileDrop );
				if( files.Length > 0 )
				{
					if( AddFiles( files ) )
					{
						this.Activate();
					}
				}
			}
		}

		private List<string> allowsPicExts = new List<string>()
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
		private List<string> allowsVideoExts = new List<string>()
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
		// Add file to the image pool
		private bool AddFiles( string[] items )
		{
			if( items.Length == 0 ) return false;

			HaltAutoAdvance();
			this.Text = lblHeader.Text = "Loading...";
			Util.CenterControl( lblHeader, picLogo.Image.Height / 2 + 20 );
			this.Refresh();

			// validate files to add
			List<string> newPics = new List<string>();
			List<string> allowsExts = allowsPicExts.Union(allowsVideoExts).ToList();
			foreach ( string item in items )
			{
				if( System.IO.Directory.Exists( item ) ) // is Directory
				{
					foreach( string file in System.IO.Directory.GetFiles( item, "*.*", System.IO.SearchOption.AllDirectories ) )
					{
						if(allowsExts.Contains(Path.GetExtension(file), StringComparer.OrdinalIgnoreCase)
							&& !pics.Exists(i => i == item))
							newPics.Add( file );
					}
				}
				else if( System.IO.File.Exists( item ) ) // is File
				{
					if (allowsExts.Contains(Path.GetExtension(item), StringComparer.OrdinalIgnoreCase)
						&& !pics.Exists((i => i == item)))
						newPics.Add( item );
				}

			}
			if( newPics.Count == 0 )
			{
				ShowNextPic( 0 );
				return false;
			}

			// add images
			if( settings.AddInRandomOrder ) Util.Shuffle( newPics );
			pics.AddRange( newPics );
			settings.Stats_LoadedPics += newPics.Count;

			// update gui
			ShowStatusMessage( "Added " + newPics.Count + " images..." );
			ResumeAutoAdvance();
			ShowNextPic( 0 );
			
			return true;
		}

		private string getMetaInfo(bool isVideo, bool mediaLoaded)
		{
			StringBuilder sb = new StringBuilder(isVideo ? settings.InfoLabelFormatVideo : settings.InfoLabelFormat);
			if (isVideo && !mediaLoaded) // these info is unavailable
			{
				sb.Replace("%w", "");
				sb.Replace("%h", "");
				sb.Replace("%time", "");
			}

			if (isVideo) // note: including audio
			{
				sb.Replace("%f", System.IO.Path.GetFileName(pics[iCurrentPic]));
				sb.Replace("%p", System.IO.Path.GetDirectoryName(pics[iCurrentPic]));
				sb.Replace("%d", System.IO.Directory.GetParent(pics[iCurrentPic]).Name);
				sb.Replace("%w", wmpCurrent.currentMedia.imageSourceWidth.ToString());
				sb.Replace("%h", wmpCurrent.currentMedia.imageSourceHeight.ToString());
				sb.Replace("%n", "\n");
				sb.Replace("%c", (iCurrentPic + 1).ToString());
				sb.Replace("%time", wmpCurrent.currentMedia.durationString);
				sb.Replace("%t", pics.Count.ToString());
				sb.Replace("%s", Util.TidyFileSize(new FileInfo(pics[iCurrentPic]).Length));
				return sb.ToString();
			}
			else
			{
				sb.Replace("%f", System.IO.Path.GetFileName(pics[iCurrentPic]));
				sb.Replace("%p", System.IO.Path.GetDirectoryName(pics[iCurrentPic]));
				sb.Replace("%d", System.IO.Directory.GetParent(pics[iCurrentPic]).Name);
				sb.Replace("%w", picCurrent.Image.Width.ToString());
				sb.Replace("%h", picCurrent.Image.Height.ToString());
				sb.Replace("%n", "\n");
				sb.Replace("%c", (iCurrentPic + 1).ToString());
				sb.Replace("%t", pics.Count.ToString());
				sb.Replace("%s", Util.TidyFileSize(new FileInfo(pics[iCurrentPic]).Length));
				return sb.ToString();
			}
		}

		private void ShowNextPic( int picsToSkip )
		{
			if( pics.Count == 0 )
			{
				this.Text = Util.GetAppName() + " by RL Vision";
				lblHeader.Text = "Add or Drop Images to Start";
				lblHeader.Visible = true;
				picCurrent.Image = null;
				picCurrent.Visible = false;
				wmpCurrent.URL = null;
				wmpCurrent.Visible = false;
				picLogo.Visible = true;
				lblInfoLabel.Text = this.Text;
				Util.CenterControl( lblHeader, picLogo.Image.Height / 2 + 20 );
				UpdateMenuEnabledDisabled();
				return;
			}
			else if( picLogo.Visible )
			{
				picLogo.Visible = false;
				picCurrent.Visible = true;
				picCurrent.Dock = DockStyle.Fill;
			}


			int newPosition = iCurrentPic + picsToSkip;
			iCurrentPic = Math.Max( Math.Min( newPosition, pics.Count ), 0 );

			if( newPosition >= pics.Count )	// past last item
			{
				this.Text = "End of Image Pool";
				lblHeader.Visible = true;
				lblHeader.Text = "End of Image Pool\nGo back or add more images";
				picCurrent.Image = null;
				Util.CenterControl( lblHeader );
				return;
			}


			// load image
			this.Text = "Loading...";
			lblInfoLabel.Text = this.Text;
			lblHeader.Visible = false;
			Application.DoEvents();

			HaltAutoAdvance();

			string URI = pics[iCurrentPic];
			bool tryVideo = !allowsPicExts.Contains(Path.GetExtension(URI), StringComparer.OrdinalIgnoreCase)
				&& allowsVideoExts.Contains(Path.GetExtension(URI), StringComparer.OrdinalIgnoreCase);

			if (tryVideo)
			{
				picCurrent.Hide();
				wmpCurrent.Show();
				if (wmpCurrent.URL == URI)
				{
					var pos = wmpCurrent.Ctlcontrols.currentPosition;
					wmpCurrent.URL = URI; // flash UI effect.
					wmpCurrent.Ctlcontrols.currentPosition = pos;
				}
				else
				{
					wmpCurrent.URL = URI;
				}

				this.Text = "Video playback";
				this.Text = getMetaInfo(true, false);
				lblInfoLabel.Text = this.Text;
				lblInfoLabel.BringToFront();

				UpdateMenuEnabledDisabled();
				ResumeAutoAdvance();
				return;
			}
			else
			{
				picCurrent.Show();
				wmpCurrent.Hide();
				wmpCurrent.URL = null;
			}

			try
			{
				picCurrent.Image = imageCache.GetImage( pics[iCurrentPic] );
				if (picCurrent.Image == null) throw new Exception("Loading image fail: " + pics[iCurrentPic]);

				CurrentAspectRatio = (float)picCurrent.Image.Size.Height / picCurrent.Image.Size.Width;

				if( settings.ResetViewModeOnPictureChange )
					SetScaleMode( ScaleMode.NormalFitWindow, false );	// reset to normal mode
				else
					SetScaleMode( CurrentScaleMode, false );			// keep current view mode

				TransformedImage?.Dispose();
				TransformedImage = null;

			}
			catch( Exception ex )
			{
				// show error message
				lblInfoLabel.Text = "(" + ( iCurrentPic + 1 ) + "/" + pics.Count + ") " + pics[iCurrentPic];
				lblHeader.Visible = true;
				lblHeader.Text = "Error loading image:\n" + ex.Message;
				picCurrent.Image = null;
				Util.CenterControl( lblHeader );
				return;
			}

			// update cache
			List<bool> CacheKeep = new List<bool>( new bool[pics.Count] );	// new list filled with "false" (bool defaults to false)
			for( int n= (iCurrentPic-settings.CacheBehind); n <= (iCurrentPic+settings.CacheAhead); n++ )	// cache around current pic
			{
				if( n >= 0 && n < pics.Count ) CacheKeep[n] = true;
			}
			if( iCurrentPic + picsToSkip >= 0 && iCurrentPic + picsToSkip < pics.Count ) CacheKeep[iCurrentPic + picsToSkip] = true;	// next pic, independent of skip amount
			if( iCurrentPic - picsToSkip >= 0 && iCurrentPic - picsToSkip < pics.Count ) CacheKeep[iCurrentPic - picsToSkip] = true;	// keep previous pic in cache
			for( int n=0; n<pics.Count; n++ )
			{
				if( CacheKeep[n] )
					imageCache.CacheImage( pics[n] );
				else
					imageCache.DropImage( pics[n] );
			}
			//

			this.Text = getMetaInfo(false, true);
			lblInfoLabel.Text = this.Text;

#if RLVISION
			if( settings.AutoMoveToScreen && bFullScreen )
			{
				try
				{
					// move to screen best suited to current image's aspect ratio
					// (the screen with the smallest difference between aspect ratios is the best fit)
					SortedDictionary<float, int> diffs = new SortedDictionary<float, int>();
					Screen currentScreen = Screen.FromControl( this );
					for( int n = 0; n < Screen.AllScreens.Length; n++ )
					{
						float ar = (float)Screen.AllScreens[n].Bounds.Size.Height / Screen.AllScreens[n].Bounds.Size.Width;
						if( !diffs.ContainsKey( Math.Abs( ar - CurrentAspectRatio ) ) )
						{
							diffs.Add( Math.Abs( ar - CurrentAspectRatio ), n );
						}
					}
					if( diffs.Count > 1 )
					{
						foreach( float key in diffs.Keys )
						{
							if( Screen.AllScreens[diffs[key]].DeviceName != currentScreen.DeviceName )
							{
								MoveFullscreenToScreen( diffs[key] );
							}
							break;
						}
					}
				}
				catch( Exception ex )
				{
					Console.WriteLine( ex );
				}
			}
#endif

			UpdateMenuEnabledDisabled();
			ResumeAutoAdvance();
		}


		// Update when a picture changes path, for example after a rename
		private void UpdatePic( string currentPicturePath, string newPicturePath, int pictureIndex = -1 )
		{
			if( pictureIndex == -1 )
			{
				pictureIndex = GetPicIndex( currentPicturePath );
				if( pictureIndex == -1 ) return;	// not in image pool
			}

			pics[pictureIndex] = newPicturePath;
			iCurrentPic = pictureIndex;	// jump to image
			ShowNextPic( 0 );			// reload image
			imageCache.DropImage( currentPicturePath );
		}

		private int GetPicIndex( string filename )
		{
			return pics.FindIndex( s => s == filename );
		}

		// -- Auto Advance control --
		private void StopAutoAdvance()
		{
			bAutoAdvanceEnabled = false;
			mnuAutoAdvanceEnabled.Checked = false;
			timerAutoAdvance.Stop();
			ShowStatusMessage( "Auto Advance: Off" );
		}
		private void StartAutoAdvance()
		{
			bAutoAdvanceEnabled = true;
			mnuAutoAdvanceEnabled.Checked = true;
			timerAutoAdvance.Start();
			ShowStatusMessage( "Auto Advance: On" );
		}
		private void HaltAutoAdvance()
		{
			timerAutoAdvance.Stop();
		}
		private void ResumeAutoAdvance()
		{
			if( bAutoAdvanceEnabled ) timerAutoAdvance.Start();
		}
		private void timerAutoAdvance_Tick( object sender, EventArgs e )
		{
			ShowNextPic( 1 );
		}
		// --

		private void ShowHideLabels()
		{
			if( bFullScreen )
				lblInfoLabel.Visible = ( settings.ShowInfoLabel == ShowModes.AlwaysShow || settings.ShowInfoLabel == ShowModes.FullscreenOnly );
			else
				lblInfoLabel.Visible = ( settings.ShowInfoLabel == ShowModes.AlwaysShow || settings.ShowInfoLabel == ShowModes.WindowedOnly );
		}
		private void ToggleMenuVisibility()
		{
			if( menuStripMain.Visible == true )
				HideMenu();
			else
				ShowMenu();
		}
		private void ShowMenu()
		{
			if( menuStripMain.Visible == true ) return;
			menuStripMain.Visible = true;
		}
		private void HideMenu()
		{
			if( menuStripMain.Visible == false ) return;
			menuStripMain.Visible = false;
		}
	
		private void ToggleFullscreen()
		{
			bFullScreen = !bFullScreen;

			if( !bFullScreen )
			{
				// Entering normal mode
				this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.Sizable;
				ShowMenu();
				mnuFullscreen.Checked = false;
				ShowCursor();
				this.Location = new Point( NormalWindowStateFormRect.Left, NormalWindowStateFormRect.Top );
				this.ClientSize = new Size( NormalWindowStateFormRect.Width, NormalWindowStateFormRect.Height );
				this.WindowState = NormalWindowState;
			}
			else
			{
				// Entering fullscreen mode
				lblInfoLabel.Visible = true;
				NormalWindowState = this.WindowState;
				this.WindowState = FormWindowState.Normal;
				this.FormBorderStyle = FormBorderStyle.None;

				NormalWindowStateFormRect = new Rectangle( this.Left, this.Top, this.Width, this.Height );

				Screen currentScreen = Screen.FromControl( this );
				this.Location = new Point( Screen.GetWorkingArea( this ).Left, Screen.GetWorkingArea( this ).Top );
				this.ClientSize = new Size( currentScreen.Bounds.Width, currentScreen.Bounds.Height );

				HideMenu();
				mnuFullscreen.Checked = true;
				if( settings.FullscreenHideCursor ) HideCursor();
			}

			// reset scale mode
			SetScaleMode( ScaleMode.NormalFitWindow , false );
			ShowHideLabels();
		}

		private void MoveFullscreenToScreen( int scr )
		{
			if( bFullScreen )
			{
				Point p = new Point();
				p.X = Screen.AllScreens[scr].WorkingArea.Left;
				p.Y = Screen.AllScreens[scr].WorkingArea.Top;
				this.Location = p;

				Screen currentScreen = Screen.FromControl( this );
				this.ClientSize = new Size( currentScreen.Bounds.Width, currentScreen.Bounds.Height );
			}
		}

		private void SetScaleMode( ScaleMode newMode , bool updateStaqtusLabel = true )
		{
			if( picCurrent.Image == null ) return;

			if( CurrentScaleMode == ScaleMode.NormalFitWindow ) NormalFitWindowPictureSize = GetPictureDisplaySize();


			if( newMode == ScaleMode.NormalFitWindow )
			{
				picCurrent.Dock = DockStyle.Fill;

				if( !settings.EnlargeSmallImages && picCurrent.Image.Height < panelMain.Height && picCurrent.Image.Width < panelMain.Width )
					picCurrent.SizeMode = PictureBoxSizeMode.CenterImage;
				else
					picCurrent.SizeMode = PictureBoxSizeMode.Zoom;

				ScaleModeOriginalPoint = new Point( -1, -1 );
			}
			else if( newMode == ScaleMode.ActualSize )
			{
				picCurrent.Dock = DockStyle.None;

				picCurrent.Width = picCurrent.Image.Size.Width;

				if( picCurrent.Width < panelMain.Width )
					picCurrent.Left = ( panelMain.Width / 2 ) - ( picCurrent.Width / 2 );
				else
					picCurrent.Left = 0;

				picCurrent.Height = picCurrent.Image.Size.Height;

				if( picCurrent.Height < panelMain.Height )
					picCurrent.Top = ( panelMain.Height / 2 ) - ( picCurrent.Height / 2 );
				else
					picCurrent.Top = 0;

				picCurrent.SizeMode = PictureBoxSizeMode.AutoSize;
			}
			else if( newMode == ScaleMode.ZoomWithMouse )
			{
				picCurrent.SizeMode = PictureBoxSizeMode.StretchImage;
				picCurrent.Dock = DockStyle.None;
			}
			else if( newMode == ScaleMode.ManualZoomPercentage )
			{
				if( CurrentScaleMode == ScaleMode.ManualZoomPercentage )
				{
					ZoomToPercentage( ResizeCurrentPercentage );
				}
				else
				{
					ResizeCurrentPercentage = ( (float)GetPictureDisplaySize().Width / picCurrent.Image.Width ) * 100;	// set scale to current

					picCurrent.SizeMode = PictureBoxSizeMode.StretchImage;
					picCurrent.Dock = DockStyle.None;
				}
			}
			else if( newMode == ScaleMode.ManualZoomWidth )
			{
				if( CurrentScaleMode == ScaleMode.ManualZoomWidth )
				{
					ZoomToWidth( ResizeCurrentWidth );
				}
				else
				{
					picCurrent.SizeMode = PictureBoxSizeMode.StretchImage;
					picCurrent.Dock = DockStyle.None;
				}
			}
			else if( newMode == ScaleMode.ManualZoomHeight )
			{
				if( CurrentScaleMode == ScaleMode.ManualZoomHeight )
				{
					ZoomToHeight( ResizeCurrentHeight );
				}
				else
				{
					picCurrent.SizeMode = PictureBoxSizeMode.StretchImage;
					picCurrent.Dock = DockStyle.None;
				}
			}
			
			CurrentScaleMode = newMode;

			if( updateStaqtusLabel ) UpdateStatusLabel();

			// set cursor
			if( CurrentScaleMode == ScaleMode.ZoomWithMouse )
				panelMain.Cursor = Cursors.SizeWE;
			else if( CurrentScaleMode == ScaleMode.ManualZoomPercentage )
				panelMain.Cursor = Cursors.Default;
			else if( CurrentScaleMode == ScaleMode.ActualSize )
				panelMain.Cursor = Cursors.SizeAll;
			else
				panelMain.Cursor = Cursors.Default;

		}

		// Callback from mouse hook
		public void UpdateMouse( int mouseX, int mouseY )
		{
			// update fullscreen mouse auto hide
			if( bFullScreen  &&  mouseX != FullScreenCursorLastMouseX )
			{
				ShowCursor();
			}
			FullScreenCursorLastMouseX = mouseX;

			// show menu in fullscreen
			if( bFullScreen )
			{
				if( mouseY < menuStripMain.Height )
					ShowMenu();
				else if( !bMenuInUse )
					HideMenu();
			}

			// update the various scale modes
			if( picCurrent.Image == null ) return;

			if( CurrentScaleMode == ScaleMode.NormalFitWindow )
			{
				// precalculate ScaleModeOriginalScale for other zoom modes
				Rectangle rectNow = GetPictureDisplaySize();
				if( rectNow.Width > rectNow.Height )
					ScaleModeOriginalScale = (float)rectNow.Width / picCurrent.Image.Width;
				else
					ScaleModeOriginalScale = (float)rectNow.Height / picCurrent.Image.Height;
			}
			else if( CurrentScaleMode == ScaleMode.ActualSize )	// show in 1:1
			{
				// prepare positions
				if( ScaleModeOriginalPoint.X == -1 )
				{
					ScaleModeOriginalPoint = new Point( mouseX, mouseY );

					// calculate where on the image the user clicked (in percent)
					Point xy = panelMain.PointToClient( new Point( mouseX, mouseY ) );

					float aspectX = (float)( xy.X - NormalFitWindowPictureSize.Left ) / NormalFitWindowPictureSize.Width;
					float aspectY = (float)( xy.Y - NormalFitWindowPictureSize.Top ) / NormalFitWindowPictureSize.Height;

					aspectX = Math.Min( Math.Max( aspectX, 0 ), 1 );
					aspectY = Math.Min( Math.Max( aspectY, 0 ), 1 );
					ScaleModeOriginalRelativePoint = new PointF( aspectX * picCurrent.Width, aspectY * picCurrent.Height );

					// init center around mouse in hold mode
					if( !settings.ActualSizeAutoScroll )
					{
						Point relativeMouseXY = panelMain.PointToClient( new Point( mouseX, mouseY ) );
						float left = relativeMouseXY.X - ScaleModeOriginalRelativePoint.X;
						float top = relativeMouseXY.Y - ScaleModeOriginalRelativePoint.Y;
						left = Math.Min( Math.Max( left, -( picCurrent.Width - panelMain.Width ) ), 0 );
						top = Math.Min( Math.Max( top, -( picCurrent.Height - panelMain.Height ) ), 0 );
						if( picCurrent.Image.Width < panelMain.Width ) left = ( panelMain.Width / 2 ) - ( picCurrent.Width / 2 );
						if( picCurrent.Image.Height < panelMain.Height ) top = ( panelMain.Height / 2 ) - ( picCurrent.Height / 2 );
						picCurrent.Left = (int)left;
						picCurrent.Top = (int)top;
					}
				}

				if( settings.ActualSizeAutoScroll )
				{
					// automatically move picture around mouse

					float allowedDist = settings.ActualSizeAutoScrollDistance;
					float actualDistX = ScaleModeOriginalPoint.X - mouseX;
					float actualDistY = ScaleModeOriginalPoint.Y - mouseY;
					float percentMovedX = actualDistX / allowedDist;
					float percentMovedY = actualDistY / allowedDist;

					Point relativeMouseXY = panelMain.PointToClient( new Point( mouseX, mouseY ) );
					int left = relativeMouseXY.X - (int)ScaleModeOriginalRelativePoint.X;
					int top = relativeMouseXY.Y - (int)ScaleModeOriginalRelativePoint.Y;

					// move around mouse
					left = left - (int)( percentMovedX * picCurrent.Width );
					top = top - (int)( percentMovedY * picCurrent.Height );

					if( settings.ActualSizeAutoScrollNoLimitInsideForm )
					{
						// limit around original point
						Point origClick = panelMain.PointToClient( new Point( ScaleModeOriginalPoint.X, ScaleModeOriginalPoint.Y ) );
						if( left > origClick.X ) left = origClick.X;
						if( top > origClick.Y ) top = origClick.Y;
						if( left + picCurrent.Width < origClick.X ) left = origClick.X - picCurrent.Width;
						if( top + picCurrent.Height < origClick.Y ) top = origClick.Y - picCurrent.Height;
					}
					else
					{
						// limit inside form
						left = (int)Math.Min( Math.Max( left, -( picCurrent.Width - panelMain.Width ) ), 0 );
						top = (int)Math.Min( Math.Max( top, -( picCurrent.Height - panelMain.Height ) ), 0 );
					}

					// don't move images smaller than the screen (ie already 1:1)
					if( settings.ActualSizeAutoScrollNoLimitInsideForm )
					{
						if( picCurrent.Width < panelMain.Width && picCurrent.Height < panelMain.Height )
						{
							left = ( panelMain.Width / 2 ) - ( picCurrent.Width / 2 );
							top = ( panelMain.Height / 2 ) - ( picCurrent.Height / 2 );
						}
					}
					else
					{
						if( picCurrent.Width < panelMain.Width ) left = ( panelMain.Width / 2 ) - ( picCurrent.Width / 2 );
						if( picCurrent.Height < panelMain.Height ) top = ( panelMain.Height / 2 ) - ( picCurrent.Height / 2 );
					}

					// update image position
					picCurrent.Left = left;
					picCurrent.Top = top;

				}
				else
				{
					// hold mode
					if( bMouseHold )
					{
						if( LastMouseHoldPoint.X != -1 )
						{
							int totWidth = panelMain.Width;
							int picWidth = picCurrent.Width;
							int totHeight = panelMain.Height;
							int picHeight = picCurrent.Height;

							if( picWidth > totWidth )
							{
								int newX = picCurrent.Left + mouseX - LastMouseHoldPoint.X;
								if( newX < totWidth - picWidth ) newX = totWidth - picWidth;
								if( newX > 0 ) newX = 0;
								picCurrent.Left = newX;
							}

							if( picHeight > totHeight )
							{
								int newY = picCurrent.Top + mouseY - LastMouseHoldPoint.Y;
								if( newY < totHeight - picHeight ) newY = totHeight - picHeight;
								if( newY > 0 ) newY = 0;
								picCurrent.Top = newY;

							}
						}
						LastMouseHoldPoint = new Point( mouseX, mouseY );
					}
				}
			}
			else if( CurrentScaleMode == ScaleMode.ZoomWithMouse )
			{
				float PercentX = 0;

				if( settings.LinearScale )
				{
					if( ScaleModeOriginalPoint.X == -1 )
					{
						ScaleModeOriginalPoint = new Point( mouseX - Location.X, mouseY );
						picCurrent.Left = 0;
						picCurrent.Top = 0;
						ScaleModeOriginalScale = (float)ScaleModeOriginalPoint.X / panelMain.Width;
						if( ScaleModeOriginalScale > 1 ) ScaleModeOriginalScale = 1;
					}

					int mouseXfix = mouseX - Location.X;
					PercentX = ScaleModeOriginalScale * 100;
					if( mouseXfix < ScaleModeOriginalPoint.X )
					{
						float mouseXrel = Math.Abs( mouseXfix - ScaleModeOriginalPoint.X );
						PercentX = 100 - ( mouseXrel / ScaleModeOriginalPoint.X ) * 100;
						PercentX = PercentX * ScaleModeOriginalScale;
					}
					else if( mouseXfix > ScaleModeOriginalPoint.X )
					{
						float mouseXrel = Math.Abs( mouseXfix - ScaleModeOriginalPoint.X );
						PercentX = ( mouseXrel / ( panelMain.Width - ScaleModeOriginalPoint.X ) ) * 100;
						PercentX = ( ScaleModeOriginalScale * 100 ) + ( PercentX * ( 1 - ScaleModeOriginalScale ) );
					}
				}
				else
				{
					// non-linear scale -> distance left & right of pointer do not represent equal zoom

					if( ScaleModeOriginalPoint.X == -1 )
					{
						ScaleModeOriginalPoint = new Point( mouseX - Location.X, mouseY );
						picCurrent.Left = 0;
						picCurrent.Top = 0;
					}

					if( ( mouseX - Location.X + 2 ) >= panelMain.Width )
					{
						// positioning mouse at 100% makes the scale evenly distributed instead of based on original mouse x
						// todo: this should be possible to enhanced to be gradually shifted instead
						ScaleModeOriginalPoint.X = (int)(panelMain.Width * ScaleModeOriginalScale);
					}

					int mouseXfix = mouseX - Location.X;
					PercentX = ScaleModeOriginalScale * 100;
					if( mouseXfix < ScaleModeOriginalPoint.X )
					{
						float mouseXrel = Math.Abs( mouseXfix - ScaleModeOriginalPoint.X );
						PercentX = 100 - ( mouseXrel / ScaleModeOriginalPoint.X ) * 100;
						PercentX = PercentX * ScaleModeOriginalScale;
					}
					else if( mouseXfix > ScaleModeOriginalPoint.X )
					{
						float mouseXrel = Math.Abs( mouseXfix - ScaleModeOriginalPoint.X );
						PercentX = ( mouseXrel / ( panelMain.Width - ScaleModeOriginalPoint.X ) ) * 100;
						PercentX = ( ScaleModeOriginalScale * 100 ) + ( PercentX * ( 1 - ScaleModeOriginalScale ) );
					}
				}

				// optionally snap to interval
				if( settings.FreeZoomSnap > 0 && settings.FreeZoomSnap < 100 )
				{
					PercentX = (int)( PercentX / settings.FreeZoomSnap ) * settings.FreeZoomSnap;
				}

				if( PercentX < 1 ) PercentX = 1;
				if( PercentX >= 99 ) PercentX = 100;

				// set new picture size
				int newWidth = (int)( ( PercentX / 100 ) * picCurrent.Image.Width );
				if( newWidth < 16 ) newWidth = 16;
				picCurrent.Width = newWidth;
				picCurrent.Height = (int)( picCurrent.Width * CurrentAspectRatio );

				picCurrent.Left = ( panelMain.Width / 2 ) - ( picCurrent.Width / 2 );
				picCurrent.Top = ( panelMain.Height / 2 ) - ( picCurrent.Height / 2 );

				// update label
				if( ResizeCurrentPercentage != Math.Round( PercentX, 1 ) )
				{
					ResizeCurrentPercentage = (float)Math.Round( PercentX, 1 );
					UpdateStatusLabel();
				}

			}

		}

		// returns the visible size of the current picture
		private Rectangle GetPictureDisplaySize()
		{
            GraphicsUnit units = GraphicsUnit.Point;
            RectangleF imgRectangleF = picCurrent.Image.GetBounds(ref units);
            Rectangle imgRectangle = Rectangle.Round(imgRectangleF);
            return imgRectangle;
        }

		private void RotateFlipImage( RotateFlipType mode )
		{
			if( picCurrent.Image == null ) return;

			if( TransformedImage == null )
			{
				TransformedImage = picCurrent.Image.Clone() as Image;
				if( TransformedImage == null ) return;
				picCurrent.Image = TransformedImage;
			}
			picCurrent.Image.RotateFlip( mode );
			picCurrent.Image = picCurrent.Image;

			CurrentAspectRatio = (float)picCurrent.Image.Size.Height / picCurrent.Image.Size.Width;
		}


		private void ZoomToWidth( int newWidth )
		{
			if( CurrentScaleMode != ScaleMode.ManualZoomWidth ) SetScaleMode( ScaleMode.ManualZoomWidth );

			picCurrent.Width = newWidth;
			picCurrent.Height = (int)( picCurrent.Width * CurrentAspectRatio );
			picCurrent.Left = ( panelMain.Width / 2 ) - ( picCurrent.Width / 2 );
			picCurrent.Top = ( panelMain.Height / 2 ) - ( picCurrent.Height / 2 );

			ResizeCurrentPercentage = ( (float)GetPictureDisplaySize().Width / picCurrent.Image.Width ) * 100;
			ResizeCurrentWidth = newWidth;
			UpdateStatusLabel();
		}
		private void ZoomToHeight( int newHeight )
		{
			if( CurrentScaleMode != ScaleMode.ManualZoomHeight ) SetScaleMode( ScaleMode.ManualZoomHeight );

			picCurrent.Height = newHeight;
			picCurrent.Width = (int)( picCurrent.Height * (1.0d/CurrentAspectRatio) );
			picCurrent.Left = ( panelMain.Width / 2 ) - ( picCurrent.Width / 2 );
			picCurrent.Top = ( panelMain.Height / 2 ) - ( picCurrent.Height / 2 );

			ResizeCurrentPercentage = ( (float)GetPictureDisplaySize().Width / picCurrent.Image.Width ) * 100;
			ResizeCurrentHeight = newHeight;
			UpdateStatusLabel();
		}
		private void ZoomToPercentage( float newPercentage )
		{
			if( CurrentScaleMode != ScaleMode.ManualZoomPercentage ) SetScaleMode( ScaleMode.ManualZoomPercentage );

			ResizeCurrentPercentage = newPercentage;

			int newWidth = (int)( ( newPercentage / 100 ) * picCurrent.Image.Width );
			if( newWidth < 16 ) newWidth = 16;
			int newHeight = (int)( newWidth * CurrentAspectRatio );
			if( settings.ZoomLimitMaxToWindowSize )
			{
				if( newWidth > panelMain.Width )
				{
					newWidth = panelMain.Width;
					newHeight = (int)( newWidth * CurrentAspectRatio );
					ResizeCurrentPercentage = (float)Math.Round(( (float)newWidth / picCurrent.Image.Width ) * 100 , 2);
				}
				if( newHeight > panelMain.Height )
				{
					newHeight = panelMain.Height;
					newWidth = (int)( newHeight * ( 1 / CurrentAspectRatio ) );
					ResizeCurrentPercentage = (float)Math.Round(( (float)newWidth / picCurrent.Image.Width ) * 100 , 2);
				}
			}
			picCurrent.Width = newWidth;
			picCurrent.Height = newHeight;
			picCurrent.Left = ( panelMain.Width / 2 ) - ( picCurrent.Width / 2 );
			picCurrent.Top = ( panelMain.Height / 2 ) - ( picCurrent.Height / 2 );

			UpdateStatusLabel();
		}

		private void UpdateStatusLabel()
		{
			// show suitable message depending on scale mode

			if( CurrentScaleMode == ScaleMode.ZoomWithMouse )
			{
				ShowStatusMessage( "Scale: " + ResizeCurrentPercentage + "% (" + picCurrent.Width + "x" + picCurrent.Height + ")" );
			}
			else if( CurrentScaleMode == ScaleMode.ActualSize )
			{
				ShowStatusMessage( lblStatus.Text = "Scale: Actual Size (" + picCurrent.Width + "x" + picCurrent.Height + ")" );
			}
			else if( CurrentScaleMode == ScaleMode.ManualZoomWidth )
			{
				ShowStatusMessage( "Scale: Locked to Width (" + picCurrent.Width + "x" + picCurrent.Height + ")" );
			}
			else if( CurrentScaleMode == ScaleMode.ManualZoomHeight )
			{
				ShowStatusMessage( "Scale: Locked to Height (" + picCurrent.Width + "x" + picCurrent.Height + ")" );
			}
			else if( CurrentScaleMode == ScaleMode.ManualZoomPercentage )
			{
				ShowStatusMessage( "Scale: " + ResizeCurrentPercentage + "% (" + picCurrent.Width + "x" + picCurrent.Height + ")" );
			}
			else
			{
				ShowStatusMessage( lblStatus.Text = "Scale: Fit Window (" + picCurrent.Width + "x" + picCurrent.Height + ")" );
			}
		}

		private void ShowStatusMessage( string msg )
		{
			if( bFullScreen )
				lblStatus.Visible = ( settings.ShowModeLabel == ShowModes.AlwaysShow || settings.ShowModeLabel == ShowModes.FullscreenOnly );
			else
				lblStatus.Visible = ( settings.ShowModeLabel == ShowModes.AlwaysShow || settings.ShowModeLabel == ShowModes.WindowedOnly );

			if( !lblStatus.Visible ) return;

			lblStatus.Text = msg;
			timerHideStatusLabel.Stop();
			lblStatus.Top = panelMain.ClientSize.Height - lblStatus.Height - 2;
			timerHideStatusLabel.Interval = 10;	// 1250
			timerHideStatusLabel.Tag = 0;
			timerHideStatusLabel.Start();
		}

		private void timerHideStatusLabel_Tick( object sender, EventArgs e )
		{
			// simple animation to hide the label
			timerHideStatusLabel.Tag = (int)timerHideStatusLabel.Tag + 1;
			if( (int)timerHideStatusLabel.Tag > 100 )
			{
				lblStatus.Top += 2;
				if( lblStatus.Top > panelMain.ClientSize.Height )
				{
					lblStatus.Visible = false;
					lblStatus.Top = panelMain.ClientSize.Height - lblStatus.Height - 2;
					timerHideStatusLabel.Stop();
				}
			}
		}

		private void UpdateMenuEnabledDisabled()
		{
			// Enables/Disables menu items

			if( pics.Count == 0 )
			{
				mnuRenameFile.Enabled = false;
				mnuCopyToClipboard.Enabled = false;
				mnuFlipX.Enabled = false;
				mnuFlipY.Enabled = false;
				mnuRotateLeft.Enabled = false;
				mnuRotateRight.Enabled = false;
				mnuNavigateBackLarge.Enabled = false;
				mnuNavigateBackMedium.Enabled = false;
				mnuNavigateFirst.Enabled = false;
				mnuNavigateForwardLarge.Enabled = false;
				mnuNavigateForwardMedium.Enabled = false;
				mnuNavigateLast.Enabled = false;
				mnuNavigateNext.Enabled = false;
				mnuNavigatePrev.Enabled = false;
				mnuClearImages.Enabled = false;
				mnuZoomIn.Enabled = false;
				mnuZoomOut.Enabled = false;
				mnuZoomToHeight.Enabled = false;
				mnuZoomToWidth.Enabled = false;
				mnuResetZoom.Enabled = false;
			}
			else
			{
				mnuRenameFile.Enabled = true;
				mnuCopyToClipboard.Enabled = true;
				mnuFlipX.Enabled = true;
				mnuFlipY.Enabled = true;
				mnuRotateLeft.Enabled = true;
				mnuRotateRight.Enabled = true;
				mnuNavigateBackLarge.Enabled = true;
				mnuNavigateBackMedium.Enabled = true;
				mnuNavigateFirst.Enabled = true;
				mnuNavigateForwardLarge.Enabled = true;
				mnuNavigateForwardMedium.Enabled = true;
				mnuNavigateLast.Enabled = true;
				mnuNavigateNext.Enabled = true;
				mnuNavigatePrev.Enabled = true;
				mnuClearImages.Enabled = true;
				mnuZoomIn.Enabled = true;
				mnuZoomOut.Enabled = true;
				mnuZoomToHeight.Enabled = true;
				mnuZoomToWidth.Enabled = true;
				mnuResetZoom.Enabled = true;
			}

			// undo menu should reflect "next" undo type
			bool bUndo = true;
			switch( fileManagement.GetNextUndoType() )
			{
				case FileManagement.UndoMode.Rename:
					mnuUndo.Text = "Undo Rename";
					break;
				case FileManagement.UndoMode.Copy:
					mnuUndo.Text = "Undo Copy";
					break;
				case FileManagement.UndoMode.Move:
					mnuUndo.Text = "Undo Move";
					break;
				case FileManagement.UndoMode.Delete:
					mnuUndo.Text = "Undo Delete";
					break;
				default:
					mnuUndo.Text = "Undo";
					bUndo = false;
					break;
			}
			mnuUndo.Enabled = bUndo;
		}


		// -- Callbacks from FileManagement class when using undo --

		public delegate void FileManagementCallback_Delegate( FileManagement.UndoCallbackData d );

		public void FileManagementCallback( FileManagement.UndoCallbackData d )
		{
			if( this.InvokeRequired )
			{
				// if required for thread safety, call self using invoke instead
				this.Invoke( new MethodInvoker( delegate() { FileManagementCallback( d ); } ) );
			}
			else
			{
				if( d.undoEvent == FileManagement.UndoCallbackEvent.UndoPerformed )
				{
					switch( d.undoData.mode )
					{
						case FileManagement.UndoMode.Rename:
							UpdatePic( d.undoData.dest, d.undoData.source );
							settings.Stats_RenamedPics--;
							break;
						case FileManagement.UndoMode.Copy:
						case FileManagement.UndoMode.Move:
						case FileManagement.UndoMode.Delete:
							if( d.undoData.picIndex < 0 ) return;
							if( d.undoData.picIndex > pics.Count ) d.undoData.picIndex = pics.Count;
							pics.Insert( d.undoData.picIndex, d.undoData.source );
							iCurrentPic = d.undoData.picIndex;	// jump to image
							ShowNextPic( 0 );					// reload image
							if( d.undoData.mode == FileManagement.UndoMode.Copy ) settings.Stats_CopiedPics--;
							if( d.undoData.mode == FileManagement.UndoMode.Move ) settings.Stats_MovedPics--;
							if( d.undoData.mode == FileManagement.UndoMode.Delete ) settings.Stats_DeletedPics--;
							break;
					}
					ShowStatusMessage( "Undo..." );

				}

				UpdateMenuEnabledDisabled();
			}
		}
		// --


		// -- Handle mouse autohide in fullscreen --
		private void ShowCursor()
		{
			if( !bCursorVisible )
			{
				bCursorVisible = true;
				Cursor.Show();
				if( settings.FullscreenHideCursor )
				{
					timerMouseHider.Stop();
					timerMouseHider.Start();
				}
			}
		}
		private void HideCursor()
		{
			if( bCursorVisible )
			{
				bCursorVisible = false;
				Cursor.Hide();
			}
			timerMouseHider.Stop();
		}
		private void ForceShowFullscreenCursor()
		{
			if( bFullScreen && settings.FullscreenHideCursor )
			{
				bPreventAutoHideCursor = true;
				ShowCursor();
			}
		}
		private void HideFullscreenForcedCursor()
		{
			if( bFullScreen && settings.FullscreenHideCursor )
			{
				bPreventAutoHideCursor = false;
				HideCursor();
			}
		}
		private void timerMouseHider_Tick( object sender, EventArgs e )
		{
			if( !this.Focused ) return;
			if( bFullScreen && !bPreventAutoHideCursor ) HideCursor();
			timerMouseHider.Stop();
		}

		private void frmMain_Deactivate( object sender, EventArgs e )
		{
			ShowCursor();
		}		
		
		// --


		/// <summary>
		/// Setting WS_EX_COMPOSITED style prevents flickering when changing between different scale modes
		/// </summary>
		protected override CreateParams CreateParams
		{
			get
			{
				CreateParams cp = base.CreateParams;
				cp.ExStyle |= 0x02000000;
				return cp;
			}
		}

		// --------------------------------------------------------------------

		// Custom (Black) Menustrip Colors
		//		Based on the following sources:
		//		http://stackoverflow.com/questions/9260303/how-to-change-menu-hover-color-winforms
		//		http://www.codeproject.com/Articles/70204/Custom-VisualStudio-2008-style-MenuStrip-and-ToolS
		//		.NET ToolStrip Customizer: http://toolstripcustomizer.codeplex.com/

		private class CustomMenuRenderer : ToolStripProfessionalRenderer
		{
			private AppSettings settings;
			public CustomMenuRenderer( AppSettings s )
				: base( new CustomMenuColorTable( s ) )
			{
				settings = s;
			}

			protected override void OnRenderItemText( ToolStripItemTextRenderEventArgs e )
			{
				e.Item.ForeColor = settings.CustomMenuColorText;
				base.OnRenderItemText( e );
			}
			protected override void OnRenderItemCheck( ToolStripItemImageRenderEventArgs e )
			{
				e.Graphics.DrawString( "ü", new Font( "Wingdings", e.ToolStrip.Font.Size + 4 ), new SolidBrush( settings.CustomMenuColorText ), e.ImageRectangle );	// draw a checkmark
			}
			protected override void OnRenderArrow( ToolStripArrowRenderEventArgs e )
			{
				e.ArrowColor = settings.CustomMenuColorText;
				base.OnRenderArrow( e );
			}
			protected override void OnRenderMenuItemBackground( ToolStripItemRenderEventArgs e )
			{
				base.OnRenderMenuItemBackground( e );
				if( e.Item.Selected )
				{
					var rect = new Rectangle( 0, 0, e.Item.Width - 1, e.Item.Height - 1 );
					if( e.Item.IsOnDropDown ) rect = new Rectangle( 2, 0, e.Item.Width - 4, e.Item.Height - 1 );
					if( e.Item.Enabled )
						e.Graphics.DrawRectangle( new Pen( settings.CustomMenuColorBorder ), rect );
					else
						e.Graphics.DrawRectangle( new Pen( settings.CustomMenuColorHightlight ), rect );
				}
			}
		}
		private class CustomMenuColorTable : ProfessionalColorTable
		{
			private AppSettings settings;
			public CustomMenuColorTable( AppSettings s )
			{
				settings = s;
			}

			public override Color MenuStripGradientBegin { get { return settings.CustomMenuColorBackground; } }
			public override Color MenuStripGradientEnd { get { return settings.CustomMenuColorBackground; } }
			public override Color MenuItemPressedGradientBegin { get { return settings.CustomMenuColorBackground; } }
			public override Color MenuItemPressedGradientMiddle { get { return settings.CustomMenuColorBackground; } }
			public override Color MenuItemPressedGradientEnd { get { return settings.CustomMenuColorBackground; } }
			public override Color ToolStripDropDownBackground { get { return settings.CustomMenuColorBackground; } }
			public override Color ImageMarginGradientBegin { get { return settings.CustomMenuColorBackground; } }
			public override Color ImageMarginGradientMiddle { get { return settings.CustomMenuColorBackground; } }
			public override Color ImageMarginGradientEnd { get { return settings.CustomMenuColorBackground; } }
			public override Color SeparatorDark { get { return settings.CustomMenuColorBorder; } }
			public override Color MenuBorder { get { return settings.CustomMenuColorBorder; } }
			public override Color ButtonSelectedHighlight { get { return settings.CustomMenuColorHightlight; } }
			public override Color ButtonSelectedGradientMiddle { get { return settings.CustomMenuColorHightlight; } }
			public override Color MenuItemSelected { get { return settings.CustomMenuColorHightlight; } }
			public override Color MenuItemSelectedGradientBegin { get { return settings.CustomMenuColorHightlight; } }
			public override Color MenuItemSelectedGradientEnd { get { return settings.CustomMenuColorHightlight; } }
		}

		private void wmpCurrent_KeyUpEvent(object sender, AxWMPLib._WMPOCXEvents_KeyUpEvent e)
		{
			frmMain_KeyUp(sender, new KeyEventArgs((Keys)e.nKeyCode));
		}

		private void wmpCurrent_KeyDownEvent(object sender, AxWMPLib._WMPOCXEvents_KeyDownEvent e)
		{
			frmMain_KeyDown(sender, new KeyEventArgs((Keys)e.nKeyCode));
		}

		private void wmpCurrent_PlayStateChange(object sender, AxWMPLib._WMPOCXEvents_PlayStateChangeEvent e)
		{
			if (e.newState == (int)WMPLib.WMPPlayState.wmppsPlaying)
			{
				this.Text = getMetaInfo(true, true);
				lblInfoLabel.Text = this.Text;
			}
		}
		// --------------------------------------------------------------------


	}

}
