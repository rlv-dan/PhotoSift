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
using System.Text;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Threading;
using System.IO;
using Shell32; //Reference Microsoft Shell Controls And Automation on the COM tab.

namespace PhotoSift
{
	/// <summary>
	/// Collects all Windows specific API calls
	/// </summary>
	public class WinApi
	{
		// -- Mouse Hook --------------------------------------------------------------------------------------------

		/// <summary>
		/// Get mouse coordinates relative to window instead of per control
		/// </summary>
		public void HookMouse()
		{
			if( hHook == 0 )
			{
				// Create an instance of HookProc.
				MouseHookProcedure = new HookProc( WinApi.MouseHookProc );

				hHook = SetWindowsHookEx( WH_MOUSE, MouseHookProcedure, (IntPtr)0, AppDomain.GetCurrentThreadId() );
				if( hHook == 0 )
				{
					MessageBox.Show( "SetWindowsHookEx Failed" );
					return;
				}
			}
			else
			{
				bool ret = UnhookWindowsHookEx( hHook );
				if( ret == false )
				{
					MessageBox.Show( "UnhookWindowsHookEx Failed" );
					return;
				}
				hHook = 0;
			}
		}

		public delegate int HookProc( int nCode, IntPtr wParam, IntPtr lParam );

		//Declare the hook handle as an int.
		static int hHook = 0;

		//Declare the mouse hook constant.
		//For other hook types, you can obtain these values from Winuser.h in the Microsoft SDK.
		public const int WH_MOUSE = 7;
		//private System.Windows.Forms.Button button1;

		//Declare MouseHookProcedure as a HookProc type.
		HookProc MouseHookProcedure;

		//Declare the wrapper managed POINT class.
		[StructLayout( LayoutKind.Sequential )]
		public class POINT
		{
			public int x;
			public int y;
		}

		//Declare the wrapper managed MouseHookStruct class.
		[StructLayout( LayoutKind.Sequential )]
		public class MouseHookStruct
		{
			public POINT pt;
			public int hwnd;
			public int wHitTestCode;
			public int dwExtraInfo;
		}

		//This is the Import for the SetWindowsHookEx function.
		//Use this function to install a thread-specific hook.
		[DllImport( "user32.dll", CharSet = CharSet.Auto,
		 CallingConvention = CallingConvention.StdCall )]
		public static extern int SetWindowsHookEx( int idHook, HookProc lpfn,
		IntPtr hInstance, int threadId );

		//This is the Import for the UnhookWindowsHookEx function.
		//Call this function to uninstall the hook.
		[DllImport( "user32.dll", CharSet = CharSet.Auto,
		 CallingConvention = CallingConvention.StdCall )]
		public static extern bool UnhookWindowsHookEx( int idHook );

		//This is the Import for the CallNextHookEx function.
		//Use this function to pass the hook information to the next hook procedure in chain.
		[DllImport( "user32.dll", CharSet = CharSet.Auto,
		 CallingConvention = CallingConvention.StdCall )]
		public static extern int CallNextHookEx( int idHook, int nCode,
		IntPtr wParam, IntPtr lParam );
		// --
		public static int MouseHookProc( int nCode, IntPtr wParam, IntPtr lParam )
		{
			//Marshall the data from the callback.
			MouseHookStruct MyMouseHookStruct = (MouseHookStruct)Marshal.PtrToStructure( lParam, typeof( MouseHookStruct ) );

			if( nCode < 0 )
			{
				return CallNextHookEx( hHook, nCode, wParam, lParam );
			}
			else
			{
				// -- PhotoSift custom code --
				frmMain tempForm = Form.ActiveForm as frmMain;	//You must get the active form because it is a static function.
				if( tempForm != null ) tempForm.UpdateMouse( MyMouseHookStruct.pt.x, MyMouseHookStruct.pt.y );
				// ---------------------------

				return CallNextHookEx( hHook, nCode, wParam, lParam );
			}
		}

		// ----------------------------------------------------------------------------------------------------------


		// --- Prevent screensaver etc. -----------------------------------------------------------------------------
		[FlagsAttribute]
		private enum EXECUTION_STATE : uint
		{
			ES_SYSTEM_REQUIRED = 0x00000001,
			ES_DISPLAY_REQUIRED = 0x00000002,
			// Legacy flag, should not be used.
			// ES_USER_PRESENT   = 0x00000004,
			ES_AWAYMODE_REQUIRED = 0x00000040,
			ES_CONTINUOUS = 0x80000000,
		}

		[DllImport( "kernel32.dll", CharSet = CharSet.Auto, SetLastError = true )]
		private static extern EXECUTION_STATE SetThreadExecutionState( EXECUTION_STATE esFlags );

		public void PreventSleep()
		{
			if( SetThreadExecutionState( EXECUTION_STATE.ES_CONTINUOUS
				| EXECUTION_STATE.ES_DISPLAY_REQUIRED
				| EXECUTION_STATE.ES_SYSTEM_REQUIRED
				| EXECUTION_STATE.ES_AWAYMODE_REQUIRED ) == 0 ) //Away mode for Windows >= Vista
				SetThreadExecutionState( EXECUTION_STATE.ES_CONTINUOUS
					| EXECUTION_STATE.ES_DISPLAY_REQUIRED
					| EXECUTION_STATE.ES_SYSTEM_REQUIRED ); //Windows < Vista, forget away mode
		}

		// ----------------------------------------------------------------------------------------------------------

#if RLVISION
		// -- Toggle NumLock Programmatically -----------------------------------------------------------------------
		[DllImport( "user32", EntryPoint = "keybd_event", ExactSpelling = true, CharSet = CharSet.Ansi, SetLastError = true )]
		private static extern void keybd_event( byte bVk, byte bScan, int dwFlags, int dwExtraInfo );
		private const int VK_NUMLOCK = 0X90;
		private const int KEYEVENTF_KEYDOWN = 0X0;
		private const int KEYEVENTF_EXTENDEDKEY = 0X1;
		private const int KEYEVENTF_KEYUP = 0X2;
		public void ToggleNumlock()
		{
			keybd_event( VK_NUMLOCK, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYDOWN, 0 );
			keybd_event( VK_NUMLOCK, 0x45, KEYEVENTF_EXTENDEDKEY | KEYEVENTF_KEYUP, 0 );
		}
		// ----------------------------------------------------------------------------------------------------------
#endif

		// -- Delete to recycle bin ---------------------------------------------------------------------------------

		private const int FO_DELETE = 3;
		private const int FOF_ALLOWUNDO = 0x40;
		private const int FOF_NOCONFIRMATION = 0x0010;

		[StructLayout( LayoutKind.Sequential, CharSet = CharSet.Auto, Pack = 1 )]
		public struct SHFILEOPSTRUCT
		{
			public IntPtr hwnd;
			[MarshalAs( UnmanagedType.U4 )]
			public int wFunc;
			public string pFrom;
			public string pTo;
			public short fFlags;
			[MarshalAs( UnmanagedType.Bool )]
			public bool fAnyOperationsAborted;
			public IntPtr hNameMappings;
			public string lpszProgressTitle;
		}

		[DllImport( "shell32.dll", CharSet = CharSet.Auto )]
		static extern int SHFileOperation( ref SHFILEOPSTRUCT FileOp );

		private Shell Shl;
		private const long ssfBITBUCKET = 10;
		private const int recycleNAME = 0;
		private const int recyclePATH = 1;

		public void Recycle( string filePath )
		{
			SHFILEOPSTRUCT fileop = new SHFILEOPSTRUCT();
			fileop.wFunc = FO_DELETE;
			fileop.pFrom = filePath + '\0' + '\0';
			fileop.fFlags = FOF_ALLOWUNDO | FOF_NOCONFIRMATION;

			SHFileOperation( ref fileop );
		}


		// -- Undelete from Recycle bin -----------------------------------------------------------------------------
		// Based on: http://stackoverflow.com/questions/6025311/how-to-restore-files-from-recycle-bin?lq=1

		public bool Restore( string Item )
		{
			bool success = false;
			try
			{
				Item = Item.Replace( @"\\", @"\" );	// restore is sensitive to double backslashes
				Shl = new Shell();
				Folder Recycler = Shl.NameSpace( 10 );
				foreach( FolderItem FI in Recycler.Items() )
				{
					string FileName = Recycler.GetDetailsOf( FI, 0 );
					if( Path.GetExtension( FileName ) == "" ) FileName += Path.GetExtension( FI.Path );
					//Necessary for systems with hidden file extensions.
					string FilePath = Recycler.GetDetailsOf( FI, 1 );
					if( Item == Path.Combine( FilePath, FileName ) )
					{
						DoVerb( FI, "&E" ); // "R&estore", "还原(&E)", etc.
						success = true;
						break;
					}
				}

			}
			catch( Exception e )
			{
				Console.WriteLine( "Restore Error: " + e );
			}
			finally
			{
				Marshal.FinalReleaseComObject( Shl );
			}
			return success;
		}
		private bool DoVerb( FolderItem Item, string Verb )
		{
			foreach( FolderItemVerb FIVerb in Item.Verbs() )
			{
				if( FIVerb.Name.ToUpper().Contains( Verb.ToUpper() ) )
				{
					FIVerb.DoIt();
					return true;
				}
			}
			return false;
		}

		// ----------------------------------------------------------------------------------------------------------

	}

}
