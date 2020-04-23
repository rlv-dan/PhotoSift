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
using System.Windows.Forms;
using System.Drawing;
using System.IO;
using System.Reflection;

namespace PhotoSift
{
    /// <summary>
    /// This "partial" collects all input related code for the main form:
    /// Mouse event, keyboard events & clicking menu items
    /// </summary>
    public partial class frmMain : Form
	{

		// -- Handle Menu Items -------------------------------------------------------------------------------------

		private void mnuAbout_Click( object sender, EventArgs e )
		{
			frmAbout f = new frmAbout( settings );
			ForceShowFullscreenCursor();
			f.ShowDialog();
			HideFullscreenForcedCursor();
		}

		private void mnuClearImages_Click( object sender, EventArgs e )
		{
			if ( settings.WarnThresholdOnClearQueue > 0 && pics.Count > settings.WarnThresholdOnClearQueue)
			{
				if (MessageBox.Show(string.Format("Confirm to clear the queue with {0} image(s)?", pics.Count),
								"Clear images queue", MessageBoxButtons.YesNo) != DialogResult.Yes)
					return;
			}
			pics.Clear();
			iCurrentPic = 0;
			ShowNextPic( 0 );
		}

		private void mnuCopyToClipboard_Click( object sender, EventArgs e )
		{
			if (settings.CopyActionType == CopytoClipboardOptions.Bitmap) {
				if (picCurrent.Image != null)
					Clipboard.SetImage(picCurrent.Image);
			}
			else if (settings.CopyActionType == CopytoClipboardOptions.File) {
				System.Collections.Specialized.StringCollection FileCollection = new System.Collections.Specialized.StringCollection();
				FileCollection.Add(pics[iCurrentPic]);
				Clipboard.SetFileDropList(FileCollection);
			}
			else if (settings.CopyActionType == CopytoClipboardOptions.FilePath) {
				Clipboard.SetText(pics[iCurrentPic]);
			}
			// ref: https://stackoverflow.com/questions/2077981/cut-files-to-clipboard-in-c-sharp
		}

		private void mnuExit_Click( object sender, EventArgs e )
		{
			this.Close();
		}

		private void mnuHomepage_Click( object sender, EventArgs e )
		{
			System.Diagnostics.Process.Start( "https://www.rlvision.com" );
		}

		private void mnuAddImages_Click( object sender, EventArgs e )
		{
			ForceShowFullscreenCursor();
			OpenFileDialog ofd = new OpenFileDialog();
			ofd.Multiselect = true;
			ofd.Title = "Select images to add...";
			ofd.Filter = "Images|*.jpg;*.jpeg;*.tif;*.tiff;*.png;*.bmp;*.gif;*.ico;*.wmf;*.emf;*.webp|All files (*.*)|*.*";
			ofd.InitialDirectory = settings.LastFolder_AddFiles;
			if( ofd.ShowDialog() == DialogResult.OK )
			{
				if( ofd.FileNames.Length > 0 )
				{
					AddFiles( ofd.FileNames );
					settings.LastFolder_AddFiles = Path.GetDirectoryName( ofd.FileNames[0] );
				}
			}
			HideFullscreenForcedCursor();
		}

		private void mnuAddFolder_Click(object sender, EventArgs e)
		{
			ForceShowFullscreenCursor();

			var dialog = new Ris.Shuriken.FolderSelectDialog
			{
				InitialDirectory = settings.LastFolder_AddFolder,
				Title = "Select a folder with images to add:",
				multiSelect = true
			};
			if (dialog.Show(Handle))
			{
				string path = dialog.FileName;
				string[] paths = dialog.FileNames;

				panelMain.Cursor = Cursors.WaitCursor;
				AddFiles(paths);
				settings.LastFolder_AddFolder = paths.Length > 0 ? System.IO.Directory.GetParent(paths[0]).FullName : path;
				panelMain.Cursor = Cursors.Arrow;
			}
			HideFullscreenForcedCursor();
		}

		private void mnuAddInRandomOrder_Click( object sender, EventArgs e )
		{
			settings.AddInRandomOrder = !settings.AddInRandomOrder;
			mnuAddInRandomOrder.Checked = settings.AddInRandomOrder;
		}

		private void mnuAutoAdvanceEnabled_Click( object sender, EventArgs e )
		{
			frmMain_KeyUp( this, new KeyEventArgs( Keys.Pause ) );
		}

		private void mnuOpenSettings_Click( object sender, EventArgs e )
		{
			HaltAutoAdvance();
			ForceShowFullscreenCursor();

			Form frm = new frmSettings( settings );
			frm.ShowDialog();

			ApplySettings();
			HideFullscreenForcedCursor();
			ShowNextPic( 0 );	// reload picture
		}

		private void mnuFullscreen_Click( object sender, EventArgs e )
		{
			ToggleFullscreen();
		}

		private void mnuShowHelp_Click( object sender, EventArgs e )
		{
			string ReadmeFile = Path.Combine(System.Windows.Forms.Application.StartupPath, "ReadMe.txt");
			if( System.IO.File.Exists( ReadmeFile ) )
			{
				System.Diagnostics.ProcessStartInfo psi = new System.Diagnostics.ProcessStartInfo( ReadmeFile );
				psi.UseShellExecute = true;
				System.Diagnostics.Process.Start( psi );
			}
			else
			{
				MessageBox.Show( "ReadMe file not found", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
		}

		private void mnuSetTargetFolder_Click(object sender, EventArgs e)
		{
			var dialog = new Ris.Shuriken.FolderSelectDialog
			{
				InitialDirectory = settings.TargetFolder,
				Title = "Select a folder:"
			};
			if (dialog.Show(Handle))
			{
				string path = dialog.FileName;
				settings.TargetFolder = path;
			}
		}

		private void mnuHideMenu_Click( object sender, EventArgs e )
		{
			frmMain_KeyUp( this, new KeyEventArgs( Keys.Tab ) );
		}

		private void mnuRenameFile_Click( object sender, EventArgs e )
		{
			ForceShowFullscreenCursor();
			string old_name = Path.GetFileNameWithoutExtension( pics[iCurrentPic] );
			string new_name = old_name;
			string ext = Path.GetExtension( pics[iCurrentPic] );
			string path = Path.GetDirectoryName( pics[iCurrentPic] );
			if( Util.InputBox( "Rename File", "Enter a new name:", ref new_name ) == System.Windows.Forms.DialogResult.OK )
			{
				string o = Path.Combine(path, old_name + ext);
				string n = Path.Combine(path, new_name + ext);
				if ( fileManagement.RenameFile(o, n, iCurrentPic ) )
				{
					UpdatePic(o, n, iCurrentPic);
					settings.Stats_RenamedPics++;
				}
			}
			HideFullscreenForcedCursor();
		}

		private void mnuFlipImage_Click( object sender, EventArgs e )
		{
			RotateFlipImage( RotateFlipType.RotateNoneFlipX );
		}

		private void mnuFlipY_Click( object sender, EventArgs e )
		{
			RotateFlipImage( RotateFlipType.RotateNoneFlipY );
		}

		private void mnuRotateLeft_Click( object sender, EventArgs e )
		{
			RotateFlipImage( RotateFlipType.Rotate270FlipNone );
		}

		private void mnuRotateRight_Click( object sender, EventArgs e )
		{
			RotateFlipImage( RotateFlipType.Rotate90FlipNone );
		}

		private void mnuNavigateNext_Click( object sender, EventArgs e )
		{
			ShowNextPic( 1 );
		}
		private void mnuNavigatePrev_Click( object sender, EventArgs e )
		{
			ShowNextPic( -1 );
		}
		private void mnuNavigateFirst_Click( object sender, EventArgs e )
		{
			iCurrentPic = 0;
			ShowNextPic( 0 );
		}
		private void mnuNavigateLast_Click( object sender, EventArgs e )
		{
			iCurrentPic = pics.Count - 1;
			ShowNextPic( 0 );
		}
		private void mnuNavigateForwardMedium_Click( object sender, EventArgs e )
		{
			ShowNextPic( settings.MediumJump );
		}
		private void mnuNavigateBackMedium_Click( object sender, EventArgs e )
		{
			ShowNextPic( -settings.MediumJump );
		}
		private void mnuNavigateForwardLarge_Click( object sender, EventArgs e )
		{
			ShowNextPic( settings.LargeJump );
		}
		private void mnuNavigateBackLarge_Click( object sender, EventArgs e )
		{
			ShowNextPic( -settings.LargeJump );
		}

		private void mnuUndo_Click( object sender, EventArgs e )
		{
			if( fileManagement.GetNextUndoType() == FileManagement.UndoMode.None ) return;
			fileManagement.UndoLastFileOperation();
		}

		private void mnuZoomToWidth_Click( object sender, EventArgs e )
		{
			string sNewWidth = GetPictureDisplaySize().Width.ToString();
			if( Util.InputBox( "New width in pixels:", "Zoom to Width", ref sNewWidth ) == System.Windows.Forms.DialogResult.OK )
			{
				try
				{
					int iNewWidth = Convert.ToInt32( sNewWidth );
					if( iNewWidth < 1 || iNewWidth > picCurrent.Image.Width * 2 ) throw new Exception();
					ZoomToWidth( iNewWidth );
				}
				catch
				{
					MessageBox.Show( "Not a valid size...", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
					return;
				}
			}
		}

		private void mnuZoomToHeight_Click( object sender, EventArgs e )
		{
			string sNewHeight = GetPictureDisplaySize().Height.ToString();
			if( Util.InputBox( "New height in pixels:", "Zoom to Height", ref sNewHeight ) == System.Windows.Forms.DialogResult.OK )
			{
				try
				{
					int iNewHeight = Convert.ToInt32( sNewHeight );
					if( iNewHeight < 1 || iNewHeight > picCurrent.Image.Width * 2 ) throw new Exception();
					ZoomToHeight( iNewHeight );
				}
				catch
				{
					MessageBox.Show( "Not a valid size...", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
					return;
				}
			}
		}

		private void mnuZoomIn_Click( object sender, EventArgs e )
		{
			frmMain_KeyDown( sender, new KeyEventArgs( Keys.Oemplus ) );
		}

		private void mnuZoomOut_Click( object sender, EventArgs e )
		{
			frmMain_KeyDown( sender, new KeyEventArgs( Keys.OemMinus ) );
		}

		private void mnuResetZoom_Click( object sender, EventArgs e )
		{
			frmMain_KeyDown( sender, new KeyEventArgs( Keys.Multiply ) );
		}

		private void mnuResetViewMode_Click( object sender, EventArgs e )
		{
			settings.ResetViewModeOnPictureChange = !settings.ResetViewModeOnPictureChange;
			mnuResetViewMode.Checked = settings.ResetViewModeOnPictureChange;
			if( settings.ResetViewModeOnPictureChange )
				ShowStatusMessage( "Scale mode: RESET when changing picture" );
			else
				ShowStatusMessage( "Scale mode: KEEP when changing picture" );
		}

		private void menuStripMain_MenuActivate( object sender, EventArgs e )
		{
			bMenuInUse = true;
			ForceShowFullscreenCursor();
		}

		private void menuStripMain_MenuDeactivate( object sender, EventArgs e )
		{
			bMenuInUse = false;
			HideFullscreenForcedCursor();
		}

		private void mnuRandimizeOrder_Click( object sender, EventArgs e )
		{
			panelMain.Cursor = Cursors.WaitCursor;
			Util.Shuffle( pics );
			ShowNextPic( 0 );
			ShowStatusMessage( "Randomized image order..." );
			panelMain.Cursor = Cursors.Arrow;
		}


		// -- Handle mouse events -----------------------------------------------------------------------------------

		private void RedirectMouseMove( object sender, MouseEventArgs e )
		{
			Control control = (Control)sender;
			Point screenPoint = control.PointToScreen( new Point( e.X, e.Y ) );
			Point formPoint = PointToClient( screenPoint );
			MouseEventArgs args = new MouseEventArgs( e.Button, e.Clicks, formPoint.X, formPoint.Y, e.Delta );
			picCurrent_MouseMove( sender, args );
		}

		private void picCurrent_MouseMove( object sender, MouseEventArgs e )
		{
			if( picCurrent.Image == null ) return;

			// hold mouse button
			if( e.Button == System.Windows.Forms.MouseButtons.Left )
			{
				bMouseHold = true;
			}
		}

		private void panelMain_MouseUp( object sender, MouseEventArgs e )
		{
			picCurrent_MouseUp( sender, e );	// forward event
		}

		private void frmMain_MouseUp( object sender, MouseEventArgs e )
		{
			picCurrent_MouseUp( sender, e );	// forward event
		}

		private void picCurrent_MouseUp( object sender, MouseEventArgs e )
		{
			if( picCurrent.Image == null ) return;

			if( !bMouseHold )
			{
				if( e.Button == System.Windows.Forms.MouseButtons.Left )
				{
					if( CurrentScaleMode == ScaleMode.ActualSize )
						SetScaleMode( ScaleMode.NormalFitWindow );
					else
						SetScaleMode( ScaleMode.ActualSize );
				}
				if( e.Button == System.Windows.Forms.MouseButtons.Right )
				{
					if( CurrentScaleMode == ScaleMode.ZoomWithMouse || CurrentScaleMode == ScaleMode.ManualZoomPercentage )
						SetScaleMode( ScaleMode.NormalFitWindow );
					else
						SetScaleMode( ScaleMode.ZoomWithMouse );
				}
				if( e.Button == System.Windows.Forms.MouseButtons.Middle )
				{
					RotateFlipImage( RotateFlipType.RotateNoneFlipX );
				}
			}
			bMouseHold = false;
			LastMouseHoldPoint = new Point( -1, -1 );
			ShowCursor();
		}

		private void MouseWheelEvent( object sender, MouseEventArgs e )
		{
			if( ModifierKeys == Keys.Control )
			{
				// zoom
				if( e.Delta > 0 )
					frmMain_KeyDown( sender, new KeyEventArgs( Keys.Oemplus ) );
				else
					frmMain_KeyDown( sender, new KeyEventArgs( Keys.OemMinus ) );

			}
			else
			{
				// show next/previous picture
				if( e.Delta > 0 )
					ShowNextPic( -1 );
				else
					ShowNextPic( 1 );
			}
		}


		// -- Handle keys (except for shortcut keys handled by menu items) ------------------------------------------
		private bool IsKeyDown(System.Windows.Input.Key key)
		{
			return System.Windows.Input.Keyboard.IsKeyDown(key);
		}
		private bool IsWindowsKeyPressed()
		{
			return IsKeyDown(System.Windows.Input.Key.LWin) || IsKeyDown(System.Windows.Input.Key.RWin);
		}

		private void frmMain_KeyUp( object sender, KeyEventArgs e )
		{
			if( bMenuInUse ) return;

			// First process arrow keys (next/prev image) & delete since they can use Control/Shift/Alt
			if( pics.Count > 0 )
			{
				if ( !( e.Control && e.Shift ) && !IsWindowsKeyPressed())
				{
					if( e.KeyCode == Keys.Right || e.KeyCode == Keys.Down || e.KeyCode == Keys.PageDown )	// forward
					{
						int n = 1;
						if( e.Control ) n = settings.MediumJump;
						else if( e.Shift ) n = settings.LargeJump;
						ShowNextPic( n );
						e.Handled = true;
					}
					else if( e.KeyCode == Keys.Left || e.KeyCode == Keys.Up || e.KeyCode == Keys.PageUp )	// backwards
					{
						int n = -1;
						if( e.Control ) n = -settings.MediumJump;
						else if( e.Shift ) n = -settings.LargeJump;
						ShowNextPic( n );
						e.Handled = true;
					}
					else if( e.KeyCode == Keys.Delete )		// Delete
					{
						string DropPicPath = pics[iCurrentPic];
						int DropPicIndex = iCurrentPic;
						ShowNextPic(settings.OnDeleteStepForward ? 1 : -1);

						if ( !e.Control )	// Ctrl+Del --> only remove from list
						{
							if( e.Shift ) fileManagement.DeleteFile(DropPicPath, false, DropPicIndex);	// Shift+Del --> force delete
							else if( e.Alt ) fileManagement.DeleteFile( DropPicPath, true, DropPicIndex );	// Alt+Del --> force recycle
							else if( settings.DeleteMode == DeleteOptions.RecycleBin ) fileManagement.DeleteFile( DropPicPath, true, DropPicIndex );
							else if( settings.DeleteMode == DeleteOptions.Delete ) fileManagement.DeleteFile( DropPicPath, false, DropPicIndex );
							else if( settings.DeleteMode == DeleteOptions.RemoveFromList ) { } // do nothing
						}

						Console.WriteLine( ">--> REMOVE FROM LIST" );
						imageCache.DropImage( DropPicPath );
						pics.RemoveAt( DropPicIndex );
						iCurrentPic--;
						if ( settings.DeleteMode != DeleteOptions.RemoveFromList ) settings.Stats_DeletedPics++;

						e.Handled = true;
					}
				}
			}

			if( e.Alt && e.KeyCode == Keys.Enter )
			{
				ToggleFullscreen();
				e.Handled = true;
			}

			// Move across monitors
			if( e.Control )
			{
				if( e.KeyCode >= Keys.D1 && e.KeyCode <= Keys.D9 )
				{
					int scr = e.KeyCode - Keys.D1;
					if( scr < Screen.AllScreens.Length )
					{
						MoveFullscreenToScreen( scr );
					}
					e.Handled = true;
				}
			}

			if (e.Control && e.KeyCode == Keys.NumPad0) // allow both
				mnuClearImages_Click(this, e);

#if RLVISION
			if( e.Control && e.KeyCode == Keys.N )
			{
				winApi.ToggleNumlock();
				if( Console.NumberLock )
					ShowStatusMessage( "NumLock OFF" );
				else
					ShowStatusMessage( "NumLock ON" );
				e.Handled = true;
			}
#endif


			// Control/Shift/Alt are not used below so skip the rest
			if( e.Alt || e.Control || e.Shift ) return;


			// Process all other keys
			if( e.KeyCode == Keys.Escape && bEscIsPassed)		// exit
			{
				bEscIsPassed = false;
				if( bFullScreen )
					ToggleFullscreen();
				else if( settings.CloseOnEscape )
					this.Close();
				e.Handled = true;
			}
			else if( e.KeyCode == Keys.Pause )		// toggle Auto Advance
			{
				if( bAutoAdvanceEnabled ) StopAutoAdvance(); else StartAutoAdvance();
				e.Handled = true;
			}

			else if( e.KeyCode == Keys.Space )	// Show next
			{
				ShowNextPic( 1 );
				e.Handled = true;
			}
			else if( e.KeyCode == Keys.Home )	// Goto first image
			{
				iCurrentPic = 0;
				ShowNextPic( 0 );
				e.Handled = true;
			}
			else if( e.KeyCode == Keys.End )	// Goto last image
			{
				iCurrentPic = pics.Count - 1;
				ShowNextPic( 0 );
				e.Handled = true;
			}


			// If no images stop processing keys here
			else if( iCurrentPic == -1 || iCurrentPic > pics.Count || picCurrent.Image == null
				&& string.IsNullOrEmpty(wmpCurrent.URL)
				)
			{
				return;
			}


			else if( e.KeyCode == Keys.F4 )	// Toggle enlarge small images
			{
				settings.EnlargeSmallImages = !settings.EnlargeSmallImages;
				SetScaleMode( ScaleMode.NormalFitWindow );

				if( settings.EnlargeSmallImages )
					ShowStatusMessage( "Small images: Enlarge to window" );
				else
					ShowStatusMessage( "Small images: Actual size" );
				e.Handled = true;
			}
			else	// Copy/Move image
			{
				bool bValidKey = false;
				if( e.KeyCode >= Keys.D0 && e.KeyCode <= Keys.D9 ) bValidKey = true;
				if( e.KeyCode >= Keys.NumPad0 && e.KeyCode <= Keys.NumPad9 ) bValidKey = true;
				if( e.KeyCode >= Keys.A && e.KeyCode <= Keys.Z ) bValidKey = true;

				if( bValidKey )
				{
					picCurrent.Image = null;
					imageCache.DropImage( pics[iCurrentPic] );
					wmpCurrent.URL = null;

					string Filename = System.IO.Path.GetFileName( pics[iCurrentPic] );

					// get pressed key
					string KeyName = Convert.ToString( (char)e.KeyValue );
					if( e.KeyCode == Keys.NumPad0 ) KeyName = "0";
					if( e.KeyCode == Keys.NumPad1 ) KeyName = "1";
					if( e.KeyCode == Keys.NumPad2 ) KeyName = "2";
					if( e.KeyCode == Keys.NumPad3 ) KeyName = "3";
					if( e.KeyCode == Keys.NumPad4 ) KeyName = "4";
					if( e.KeyCode == Keys.NumPad5 ) KeyName = "5";
					if( e.KeyCode == Keys.NumPad6 ) KeyName = "6";
					if( e.KeyCode == Keys.NumPad7 ) KeyName = "7";
					if( e.KeyCode == Keys.NumPad8 ) KeyName = "8";
					if( e.KeyCode == Keys.NumPad9 ) KeyName = "9";

					// Get custom target folder from settings
					PropertyInfo Prop = ( typeof( AppSettings ) ).GetProperty( "KeyFolder_" + KeyName );
					String tmp = (string)Prop.GetValue( settings, null );
					string CustomKeyFolder = "";
					if( tmp.Trim() != "" ) CustomKeyFolder = tmp.Trim();

					// figure out how to use the custome folder (complete path or relative to base folder)
					string targetDir = Path.Combine(settings.TargetFolder, KeyName);		// use keyname as default subfolder name
					if( CustomKeyFolder != "" )
					{
						try
						{
							if( Path.IsPathRooted( CustomKeyFolder ) )
							{
								targetDir = CustomKeyFolder;	// fully qualified path
							}
							else
							{
								targetDir = Path.Combine(settings.TargetFolder, CustomKeyFolder);	// folder is relative to base folder
							}
						}
						catch
						{
							MessageBox.Show( "The target folder for this key is not valid:\n\n" + CustomKeyFolder, "Path Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
						}
					}

					string sourceDir = System.IO.Path.GetDirectoryName( pics[iCurrentPic] );
					string sourceFile = Path.Combine(sourceDir, Filename);
					string targetFil = Path.Combine(targetDir, Filename);
					try
					{
						sourceFile = Path.GetFullPath( sourceFile );	// strip double slashes etc
						targetFil = Path.GetFullPath( targetFil );
					}
					catch( Exception ex )
					{
						Console.WriteLine( "GetFullPath Error" + ex );
					}
					Console.WriteLine( sourceFile );
					fileManagement.CopyMoveFile( sourceFile, targetFil, settings, iCurrentPic );

					pics.RemoveAt( iCurrentPic );
					ShowNextPic( 0 );
					e.Handled = true;
				}
			}

		}

		private void frmMain_KeyDown( object sender, KeyEventArgs e )
		{
			if( bMenuInUse ) return;

			// tab key (handled in keydown event in order to prevent triggering it when alt-tabbing back to applications)
			if( e.KeyCode == Keys.Tab )
			{
				ToggleMenuVisibility();
				e.Handled = true;
			}

			// keyboard zoom (handled in keydown event to allow for key repeats)
			else if( e.KeyCode == Keys.Add || e.KeyCode == Keys.Subtract || e.KeyCode == Keys.Oemplus || e.KeyCode == Keys.OemMinus )
			{
				if( CurrentScaleMode != ScaleMode.ManualZoomPercentage ) SetScaleMode( ScaleMode.ManualZoomPercentage );

				float newZoom = ResizeCurrentPercentage;

				// figure out next zoom percentage
				string[] SplitZoomFactors = settings.ZoomSteps.Split( ',' );
				if( SplitZoomFactors.Length >= 2 )
				{
					List<int> ZoomFactors = new List<int>();
					for( int i = 0; i < SplitZoomFactors.Length; i++ )
					{
						SplitZoomFactors[i] = SplitZoomFactors[i].Trim();
						try
						{
							int tmp = Convert.ToInt32( SplitZoomFactors[i] );
							if( tmp > 0 ) ZoomFactors.Add( tmp );
						}
						catch
						{
						}
					}

					ZoomFactors.Sort();
					int biggest = ZoomFactors[ZoomFactors.Count - 1];
					int smallest = ZoomFactors[0];
					if( e.KeyCode == Keys.Subtract || e.KeyCode == Keys.OemMinus ) ZoomFactors.Reverse();

					for( int i = 0; i < ZoomFactors.Count; i++ )
					{
						if( ( ( e.KeyCode == Keys.Add || e.KeyCode == Keys.Oemplus ) && ZoomFactors[i] > (int)newZoom )
							|| ( ( e.KeyCode == Keys.Subtract || e.KeyCode == Keys.OemMinus ) && ZoomFactors[i] < (int)newZoom ) )
						{
							newZoom = ZoomFactors[i];
							break;
						}
					}
					newZoom = (int)Math.Max( Math.Min( newZoom, biggest ), smallest );
				}
				else
				{
					int step = 1;
					try { step = Convert.ToInt32( settings.ZoomSteps ); }
					catch { }
					if( step == 0 ) step = 1;
					if( e.KeyCode == Keys.Add || e.KeyCode == Keys.Oemplus )
					{
						newZoom += step;
					}
					else
					{
						newZoom -= step;
						if( newZoom < step ) newZoom = step;
					}
				}

				ZoomToPercentage( newZoom );
				e.Handled = true;
			}
			else if( e.KeyCode == Keys.Back || e.KeyCode == Keys.Multiply )	// reset zoom
			{
				SetScaleMode( ScaleMode.NormalFitWindow );
				e.Handled = true;
			}
			else if (e.KeyCode == Keys.Escape)
			{
				bEscIsPassed = true;
			}
		}


	}
}
