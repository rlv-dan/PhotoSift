
--- PhotoSift -----------------------------------------------------------------
 
  Developed by RL Vision © 2013-2014 (http://www.rlvision.com)
  Special thanks to Michael for all support, suggestions, testing and the logo.
  
  Free, open source. Licensed under GPL
  Source code is available from the homepage.

  Portable:
    • Just unzip and run
    • Settings are saved in the application folder as 'Settings.xml'.
	  This file can be kept when upgrading. Removing it restores the
	  default settings.


--- About ---------------------------------------------------------------------

  PhotoSift is a utility helping you to quickly organize unsorted image 
  libraries. The basic idea is to load the program with images, then show 
  and inspect each image and press a key on the keyboard. The image will 
  then be moved or copied to a folder of your choice corresponding to 
  that key, and the next images is displayed. 

  Features:
	• Quickly organize your unsorted images
    • Fast keyboard-based workflow
    • Multithreaded cache-ahead loading
    • Various inspection tools (zoom, flip, rotate)
    • Rename or delete files when needed
	• Unlimited undo
    • Fullscreen mode for less distractions
    • Highly configurable
    • Portable
    • Free, open source 


--- Getting Started -----------------------------------------------------------

  This sections outlines how to use PhotoSift for the first time:

  1. Set the base target folder
     
	 The first thing you should do is to decide where images should end
	 up when you press a key. On the "File" menu, select "Set Target Base
	 Folder". (If you don't change this folder, it will default to the 
	 same folder as PhotoSift.exe is located in)

  2. Set file mode

     PhotoSift can either move or copy your images. Default is move. You
	 may want to change this. On the "Edit" menu select "Settings". Locate
	 "File Operations" to find the file mode. Also check the other file
	 operation settings to make sure they suit your needs.

  3. Add images

     Open the "Image Pool" menu and select either "Add Images" or "Add 
	 from Folder". The latter will let you choose a folder, and then locate
	 all images in that folder and its subfolders. You can also drag and 
	 drop images or folders onto the program to add them.

  4. Browse the image pool

     Now that you have loaded images you can start browsing them. Press left
	 and right to step forward and backwards in the image pool. See the hotkey
	 section below to see what more you can do.

  5. Press a Key

     When an image is displayed, press a letter or number key on the keyboard. 
	 Let's say that you press the "A" key. Now PhotoSift will move (or copy) 
	 the image into a subfolder named "A" that is located in the folder that 
	 you decided on in step 1. 

     For example:	 
	   • Currently viewing: "c:\temp\pic.jpg"
	   • Base target folder is: "d:\sorted\"
	   • You Press "A"
	   • Image is moved to: "d:\sorted\A\pic.jpg"
	 
	 The next section describes how to change the destination folder
	 associated with each key.


--- Destination Folders -------------------------------------------------------

    The settings allows you to specify the destination folder associated with 
	each key. There are three ways to specify this:

	1: Leave the field empty.
	   This will use the key as destination folder. So if you pressed the "A" 
	   key, the image will end up in a subfolder name "A" located in the base 
	   target folder.

	2: Enter a custom name.
	   This wil be used as the destination folder, located as a subfolder in 
	   the base target folder. Example: "Landscape Photos"

	3: Enter a complete path.
       If you enter a complete path this will be used as the destination 
	   folder. Example: "c:\photos\portraits\"
    
	You can change the base target folder from the "File" menu in the main 
	window. If you don't change this folder, it will default to the same folder 
	as PhotoSift.exe is located in.

	If the destination folder does not exist, it will be created automatically.


--- Hotkeys -------------------------------------------------------------------

  PhotoSift is built around using shortcut keys. This is a complete list of all
  available keys:

  Navigate:
   • Right/Down/PgDown/Space: Show next image
   • Left/Up/PgUp: Show previous image
   • Ctrl+Arrows: Skip 10 images forward/backwards
   • Shift+Arrows: Skip 25 images forward/backwards
   • Home/End: Go to first or last image

  Mouse:
   • Left Mouse Button: Toggle scale vs actual size (1:1) mode
   • Right Mouse Button: Toggle zoom mode
   • Middle Mouse Button: Flip image vertically (*)
   • MouseWheel: Next/previous image

  File operations & image pool:
   • A-Z and 0-9: Move or copy current image
   • F9: Select base target folder
   • Shift+Del: Deletes the current image
   • Alt+Del: Deletes the current image to recycle bin
   • Ctrl+Del: Remove the current image from the image pool
   • Del: Delete/Recycle/Remove depending on settings
   • Insert: Opens a file dialog to add images to the image pool
   • Ctrl+Insert: Add all images contained in a  folder and its subfolders
   • F2: Lets you rename the currently displayed image file
   • Ctrl+Z: Undo last file operation (**)
   • Ctrl+R: Randomize the order of the images' in the pool

  Display
   • Ctrl+W: Opens input box to scale to specific width (in pixels)
   • Ctrl+H: Opens input box to scale to specific width (in pixels)
   • +: Zoom in
   • -: Zoom out
   • Ctrl+Shift+Up: Flip image vertically (*)
   • Ctrl+Shift+Down: Flip image horizontally (*)
   • Ctrl+Shift+Left: Rotate image CCW (*)
   • Ctrl+Shift+Right: Rotate image CW (*)
   • F4: Toggle enlarge small images
   • F8: Toggles if the scale mode should be kept when changing image

  System:
   • F1: Open this help file
   • F11 or Alt+Enter: Toggle fullscreen
   • F12: Open settings
   • Ctrl+C: Copy the current image to clipboard
   • Pause/Break: Toggle auto advance on or off
   • Tab: Toggle showing the menubar
   • Ctrl+1-9: In a multi monitor setup, if in PhotoSift is in fullscreen 
               mode, Ctrl+# moves the application to screen number #.

   (*)  Flipping and rotating is only visual and does not affect the 
        actual image file.
   
   (**) Undo works on move, copy, rename and delete to recycle bin.


--- Command Line --------------------------------------------------------------

  When starting PhotoSift you can supply a list of files or folders. PhotoSift
  will attempt to load these files and the content of folders (including
  subfolders).

  Example:

    PhotoSift.exe "c:\temp\someimage.jpg" "c:\allmyphotos"


--- Requirements --------------------------------------------------------------

  To run PhotoSift you need the ".NET Framework 3.0" or later. If you get the error 
  message "The application failed to initialize properly (0xc0000135)" 
  this means you don't have .NET 2.0 installed.

  .Net 2.0 is preinstalled on Windows XP (SP3), Vista & 7. Windows 8+ users 
  might need to download or enable it first. Usually Windows will detect and 
  offer to download and install the required framework files for you when 
  you try to start the program for the first time. Let it finish and then 
  try to start the program again. If this does not happen, try this:

    1. Go to Control Panel –> Programs –> Get Programs 
    2. Click Turn Windows features on or off 
    3. Check '.NET Framework 3.5 (includes .NET 2.0 and 3.0)'
    4. Click OK. 

--- Version History -----------------------------------------------------------

  1.11 (2014-06-15)
       Fixed a visual problem with the black scheme on Windows 8

  1.1  (2014-03-29)
       Added undo for all file operations
       Added customizable menu colors (defaults to black scheme)
       Added zoom in/out and to specific width/height
       Added command line support
       Added option to keep scale mode when changing picture
       Added rudimentary stats to about dialog
       Added menu item to shuffle loaded images
       Lots of minor additionas, tweaks & fixes

  1.0  (2014-01-22)
       First public release

