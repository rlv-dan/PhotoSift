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
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.IO;
using System.Linq;
using Shell32; //Reference Microsoft Shell Controls And Automation on the COM tab.
using System.Drawing.Design;
using System.ComponentModel;

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

                var thread = (System.Diagnostics.Process.GetCurrentProcess().Threads).OfType<System.Diagnostics.ProcessThread>().SingleOrDefault(x => x.ThreadState == System.Diagnostics.ThreadState.Running);

                    if (thread != null)
                    {
                        hHook = SetWindowsHookEx(WH_MOUSE, MouseHookProcedure, (IntPtr)0, thread.Id);
                }
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

		public string FolderBrowserDialogAPI(string title = "Select a folder:")
		{
			const int BIF_NEWDIALOGSTYLE = 0x40;
			const int BIF_VALIDATE = 0x20;
			const int BIF_EDITBOX = 0x10; // Does not exist in FolderBrowserDialog()
			const int OPTIONS = BIF_NEWDIALOGSTYLE + BIF_VALIDATE + BIF_EDITBOX;
			var shell = new Shell();
			var folder = (Folder2)shell.BrowseForFolder(System.Diagnostics.Process.GetCurrentProcess().MainWindowHandle.ToInt32(), title, OPTIONS);
			if (folder == null) return "";
			return folder.Self.Path;
		}

		// copyed from https://stackoverflow.com/questions/15368771/show-detailed-folder-browser-from-a-propertygrid/15386992#15386992
		public class FolderNameEditor2 : UITypeEditor
		{
			public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
			{
				return UITypeEditorEditStyle.Modal;
			}

			public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
			{
				FolderBrowser2 browser = new FolderBrowser2();
				if (value != null)
				{
					browser.DirectoryPath = string.Format("{0}", value);
				}

				if (browser.ShowDialog(null) == DialogResult.OK)
					return browser.DirectoryPath;

				return value;
			}
		}

		public class FolderBrowser2
		{
			public string DirectoryPath { get; set; }

			public DialogResult ShowDialog(IWin32Window owner)
			{
				IntPtr hwndOwner = owner != null ? owner.Handle : GetActiveWindow();

				IFileOpenDialog dialog = (IFileOpenDialog)new FileOpenDialog();
				try
				{
					IShellItem item;
					if (!string.IsNullOrEmpty(DirectoryPath))
					{
						IntPtr idl;
						uint atts = 0;
						if (SHILCreateFromPath(DirectoryPath, out idl, ref atts) == 0)
						{
							if (SHCreateShellItem(IntPtr.Zero, IntPtr.Zero, idl, out item) == 0)
							{
								dialog.SetFolder(item);
							}
							Marshal.FreeCoTaskMem(idl);
						}
					}
					dialog.SetOptions(FOS.FOS_PICKFOLDERS | FOS.FOS_FORCEFILESYSTEM);
					uint hr = dialog.Show(hwndOwner);
					if (hr == ERROR_CANCELLED)
						return DialogResult.Cancel;

					if (hr != 0)
						return DialogResult.Abort;

					dialog.GetResult(out item);
					string path;
					item.GetDisplayName(SIGDN.SIGDN_FILESYSPATH, out path);
					DirectoryPath = path;
					return DialogResult.OK;
				}
				finally
				{
					Marshal.ReleaseComObject(dialog);
				}
			}

			[DllImport("shell32.dll")]
			private static extern int SHILCreateFromPath([MarshalAs(UnmanagedType.LPWStr)] string pszPath, out IntPtr ppIdl, ref uint rgflnOut);

			[DllImport("shell32.dll")]
			private static extern int SHCreateShellItem(IntPtr pidlParent, IntPtr psfParent, IntPtr pidl, out IShellItem ppsi);

			[DllImport("user32.dll")]
			private static extern IntPtr GetActiveWindow();

			private const uint ERROR_CANCELLED = 0x800704C7;

			[ComImport]
			[Guid("DC1C5A9C-E88A-4dde-A5A1-60F82A20AEF7")]
			private class FileOpenDialog
			{
			}

			[ComImport]
			[Guid("42f85136-db7e-439c-85f1-e4075d135fc8")]
			[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
			private interface IFileOpenDialog
			{
				[PreserveSig]
				uint Show([In] IntPtr parent); // IModalWindow
				void SetFileTypes();  // not fully defined
				void SetFileTypeIndex([In] uint iFileType);
				void GetFileTypeIndex(out uint piFileType);
				void Advise(); // not fully defined
				void Unadvise();
				void SetOptions([In] FOS fos);
				void GetOptions(out FOS pfos);
				void SetDefaultFolder(IShellItem psi);
				void SetFolder(IShellItem psi);
				void GetFolder(out IShellItem ppsi);
				void GetCurrentSelection(out IShellItem ppsi);
				void SetFileName([In, MarshalAs(UnmanagedType.LPWStr)] string pszName);
				void GetFileName([MarshalAs(UnmanagedType.LPWStr)] out string pszName);
				void SetTitle([In, MarshalAs(UnmanagedType.LPWStr)] string pszTitle);
				void SetOkButtonLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszText);
				void SetFileNameLabel([In, MarshalAs(UnmanagedType.LPWStr)] string pszLabel);
				void GetResult(out IShellItem ppsi);
				void AddPlace(IShellItem psi, int alignment);
				void SetDefaultExtension([In, MarshalAs(UnmanagedType.LPWStr)] string pszDefaultExtension);
				void Close(int hr);
				void SetClientGuid();  // not fully defined
				void ClearClientData();
				void SetFilter([MarshalAs(UnmanagedType.Interface)] IntPtr pFilter);
				void GetResults([MarshalAs(UnmanagedType.Interface)] out IntPtr ppenum); // not fully defined
				void GetSelectedItems([MarshalAs(UnmanagedType.Interface)] out IntPtr ppsai); // not fully defined
			}

			[ComImport]
			[Guid("43826D1E-E718-42EE-BC55-A1E261C37BFE")]
			[InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
			private interface IShellItem
			{
				void BindToHandler(); // not fully defined
				void GetParent(); // not fully defined
				void GetDisplayName([In] SIGDN sigdnName, [MarshalAs(UnmanagedType.LPWStr)] out string ppszName);
				void GetAttributes();  // not fully defined
				void Compare();  // not fully defined
			}

			private enum SIGDN : uint
			{
				SIGDN_DESKTOPABSOLUTEEDITING = 0x8004c000,
				SIGDN_DESKTOPABSOLUTEPARSING = 0x80028000,
				SIGDN_FILESYSPATH = 0x80058000,
				SIGDN_NORMALDISPLAY = 0,
				SIGDN_PARENTRELATIVE = 0x80080001,
				SIGDN_PARENTRELATIVEEDITING = 0x80031001,
				SIGDN_PARENTRELATIVEFORADDRESSBAR = 0x8007c001,
				SIGDN_PARENTRELATIVEPARSING = 0x80018001,
				SIGDN_URL = 0x80068000
			}

			[Flags]
			private enum FOS
			{
				FOS_ALLNONSTORAGEITEMS = 0x80,
				FOS_ALLOWMULTISELECT = 0x200,
				FOS_CREATEPROMPT = 0x2000,
				FOS_DEFAULTNOMINIMODE = 0x20000000,
				FOS_DONTADDTORECENT = 0x2000000,
				FOS_FILEMUSTEXIST = 0x1000,
				FOS_FORCEFILESYSTEM = 0x40,
				FOS_FORCESHOWHIDDEN = 0x10000000,
				FOS_HIDEMRUPLACES = 0x20000,
				FOS_HIDEPINNEDPLACES = 0x40000,
				FOS_NOCHANGEDIR = 8,
				FOS_NODEREFERENCELINKS = 0x100000,
				FOS_NOREADONLYRETURN = 0x8000,
				FOS_NOTESTFILECREATE = 0x10000,
				FOS_NOVALIDATE = 0x100,
				FOS_OVERWRITEPROMPT = 2,
				FOS_PATHMUSTEXIST = 0x800,
				FOS_PICKFOLDERS = 0x20,
				FOS_SHAREAWARE = 0x4000,
				FOS_STRICTFILETYPES = 4
			}
		}
		// ----------------------------------------------------------------------------------------------------------
	}

}
