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
using System.Threading.Tasks;
using System.IO;
using Microsoft.VisualBasic.FileIO;

namespace PhotoSift
{
    /// <summary>
    /// Contains methods to copy/move/delete files as well as undo previous operations
    /// </summary>
    public class FileManagement
	{
		private WinApi winApi;
		private frmMain mainForm;
		public FileManagement( frmMain f )
		{
			mainForm = f;
			winApi = new WinApi();
		}

		// -- Undo --

		Stack<UndoData> undo = new Stack<UndoData>();

		public enum UndoCallbackEvent
		{
			UndoUpdated,
			UndoPerformed
		}
		public class UndoCallbackData
		{
			public UndoCallbackEvent undoEvent { get; set; }
			public UndoData undoData { get; set; }

			public UndoCallbackData( UndoCallbackEvent e , UndoData d )
			{
				undoEvent = e;
				undoData = d;
			}
		}
		public enum UndoMode
		{
			None,
			Copy,
			Move,
			Delete,
			Rename
		}
		public class UndoData
		{
			public string source { get; set; }
			public string dest { get; set; }
			public UndoMode mode { get; set; }
			public int picIndex { get; set; }
		}

		private void AddToUndo( string SourceFilename, string DestFilename, UndoMode mode , int picIndex )
		{
			UndoData u = new UndoData();
			u.source = SourceFilename;
			u.dest = DestFilename;
			u.mode = mode;
			u.picIndex = picIndex;
			undo.Push( u );
			mainForm.FileManagementCallback( new UndoCallbackData( UndoCallbackEvent.UndoUpdated, u ) );
		}

		public void UndoLastFileOperation()
		{
			if( undo.Count == 0 )
			{
				MessageBox.Show( "Nothing to undo...", "Oops", MessageBoxButtons.OK, MessageBoxIcon.Error );
				return;
			}

			UndoData u = undo.Pop();
			if( u.mode == UndoMode.Copy )
			{
				DeleteFile( u.dest, false );	// delete copy of original file
			}
			else if( u.mode == UndoMode.Move )
			{
				AppSettings s = new AppSettings();
				s.FileMode = FileOperations.Move;
				s.ExistingFiles = ExistingFileOptions.Overwrite;
				CopyMoveFile( u.dest, u.source, s, -1, false ); // move back file
            }
			else if( u.mode == UndoMode.Rename )
			{
				RenameFile( u.dest, u.source, -1, false );	// rename back to original filename
			}
			else if( u.mode == UndoMode.Delete )
			{
				Undelete( u.source );
			}

			mainForm.FileManagementCallback( new UndoCallbackData( UndoCallbackEvent.UndoPerformed, u ) );
		}

		public UndoMode GetNextUndoType()
		{
			if( undo.Count == 0 ) return UndoMode.None;
			return undo.Peek().mode;
		}

		/// <summary>
		/// Copy or move a file from one directory to another. Runs in a new thread.
		/// </summary>
		/// <param name="fileName">The filename (without path)</param>
		/// <param name="sourceDir">Directory where the file is now</param>
		/// <param name="destDir">Destination direction path</param>
		/// <param name="settings">Pass a settings object with FileOperations and ExistingFileOptions set</param>
		public void CopyMoveFile( string source, string dest, AppSettings settings, int picIndex = -1 , bool bSaveUndo = true )
		{
			// launch operation in separate thread (don't want to stop GUI thread)
			Task.Run(() => 
			{
				try
				{
					string destDir = Path.GetDirectoryName( dest );
					string fileName = Path.GetFileName( dest );

					// create dir if not already present
					if( destDir != "" && !Directory.Exists( destDir ) )
					{
						Directory.CreateDirectory( Path.GetDirectoryName( dest ) );
					}

					// option: append number to existing files
					if( File.Exists( dest ) && settings.ExistingFiles == ExistingFileOptions.AppendNumber )
					{
						int i = 1;
						while( File.Exists( dest ) )
						{
							string filename = Path.GetFileNameWithoutExtension(fileName) + " (" + (i++) + ")" + Path.GetExtension(fileName);
							dest = Path.Combine(destDir, filename);
						}
					}

					// what to do with existing files?
					bool bOverwrite = false;
					bool bSkip = false;
					if( File.Exists( dest ) && settings.ExistingFiles == ExistingFileOptions.Overwrite ) bOverwrite = true;
					if( File.Exists( dest ) && settings.ExistingFiles == ExistingFileOptions.Skip ) bSkip = true;

					// perform copy/move operation
					if( !bSkip )
					{
						if( settings.FileMode == FileOperations.Copy )
						{
							File.Copy( source, dest, bOverwrite );
							settings.Stats_CopiedPics++;
							Console.WriteLine( "[COPY] " + source + " --> " + dest );
							if( bSaveUndo ) AddToUndo( source, dest, UndoMode.Copy, picIndex );
						}
						else if( settings.FileMode == FileOperations.Move )
						{
							if( bOverwrite ) File.Delete( dest );
							File.Move( source, dest );
							settings.Stats_MovedPics++;
							Console.WriteLine( "[MOVE] " + source + " --> " + dest );
							if( bSaveUndo ) AddToUndo( source, dest, UndoMode.Move , picIndex);
						}
					}
				}
				catch( Exception ex )
				{
					MessageBox.Show( "Error copying/moving file: \n\n" + ex.Message, "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
			});
		}

		/// <summary>
		/// Rename file (with undo)
		/// </summary>
		/// <param name="oldName">Current filename (including path)</param>
		/// <param name="newName">New filename (including path)</param>
		/// <param name="bSaveUndo">Enable undo on this operation</param>
		public bool RenameFile( string oldName, string newName, int picIndex = -1, bool bSaveUndo = true )
		{
			try
			{
				File.Move( oldName, newName );
				if( bSaveUndo ) AddToUndo( oldName, newName, UndoMode.Rename, picIndex );
				return true;
			}
			catch( Exception ex )
			{
				MessageBox.Show( "Could not rename file: \n\n" + ex.Message, "File Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				return false;
			}
		}

		/// <summary>
		/// Deletes a file from disk, either a "real" delete or just sending it to the Windows recycle bin. The operation runs in a separate thread.
		/// </summary>
		/// <param name="filePath">The path of the file to be deleted</param>
		/// <param name="sendToRecycleBin">Decides if the file is to be permanentely deleted of just send to the Windows recycle bin</param>
		public void DeleteFile( string filePath, bool sendToRecycleBin, int picIndex = -1 )
		{
			Task.Run(() =>
			{
				try
				{
					if( sendToRecycleBin )
					{
						Console.WriteLine( "[RECYCLE] " + filePath );
						MoveToRecycle( filePath );
						AddToUndo( filePath, "", UndoMode.Delete, picIndex );
					}
					else
					{
						File.Delete( filePath );
						Console.WriteLine( "[DELETE] " + filePath );
					}
				}
				catch( Exception ex )
				{
					MessageBox.Show( "Error deleting file: \n\n" + ex.Message, "Delete Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
				}
			});
		}
		private void Undelete(string filePath)
		{
			if( !winApi.Restore(filePath) )
			{
				MessageBox.Show( "Could not restore " + filePath + "...", "Undelete Error", MessageBoxButtons.OK, MessageBoxIcon.Error );
			}
			
		}

		private void MoveToRecycle(string filePath)
		{
			FileSystem.DeleteFile(filePath, UIOption.OnlyErrorDialogs, RecycleOption.SendToRecycleBin);
		}
	}
}
