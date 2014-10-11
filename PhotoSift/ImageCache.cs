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
using System.Drawing;
using System.Threading;
using System.IO;

namespace PhotoSift
{
	/// <summary>
	/// Simple class for keeping an image cache
	/// Images are loaded in a separate thread
	/// Note that caching must be manually handled by the user by loading and dropping images
	/// </summary>
	class ImageCache
	{
		private Dictionary<string, CachedImage> cache;
		
		// Constructor
		public ImageCache()
		{
			cache = new Dictionary<string, CachedImage>();
		}

		/// <summary>
		/// Returns an image, either by loading it from disk or from memory if already cached
		/// </summary>
		/// <param name="sFilename">Filename of image to load</param>
		/// <returns>Requested image</returns>
		public Image GetImage( string sFilename )
		{
			if( !cache.ContainsKey( sFilename ) )
			{
				// Need to load from disk
				CacheImage( sFilename );
			}

			if( cache.ContainsKey( sFilename ) )
			{
				CachedImage ci = cache[sFilename];
				if( ci.LoadingThread != null )
				{
					ci.LoadingThread.Join();	// need to wait for loading thread to finish
					return ci.img;
				}
			}

			return null;
		}

		/// <summary>
		/// Instruct system to cache an image
		/// </summary>
		/// <param name="sFilename">Filename of image to cache</param>
		public void CacheImage( string sFilename )
		{
			if( !cache.ContainsKey( sFilename ) )
			{
				CachedImage ci = new CachedImage( sFilename );
				cache.Add( sFilename, ci );

				// Start loading in a separate thread
				ci.LoadingThread = new Thread( new ParameterizedThreadStart( LoadImage ) );
				ci.LoadingThread.Start( ci );
			}
		}

		/// <summary>
		/// Removes an image from cache and free memory
		/// </summary>
		/// <param name="sFilename">Filename of an image currently cache</param>
		public void DropImage( string sFilename )
		{
			if( cache.ContainsKey( sFilename ) )
			{
				if( cache[sFilename].img != null ) cache[sFilename].img.Dispose();
				cache.Remove( sFilename );
			}
		}

		/// <summary>
		/// Loads an image from disk into the supplied CachedImage object
		/// </summary>
		/// <param name="data">CachedImage object containg the filename to load</param>
		static void LoadImage( object data )
		{
			try
			{
				CachedImage ci = (CachedImage)data;

				var ImageData = File.ReadAllBytes( ci.sFilename );
				ci.img = Bitmap.FromStream( new MemoryStream( ImageData ) );
			}
			catch( Exception ex )
			{
				System.Console.WriteLine( "ImageCache ERROR: " + ex.ToString() );
			}
		}

	}

	/// <summary>
	/// Class for holding images, with a reference to the thread that is loading it
	/// </summary>
	class CachedImage
	{
		public string sFilename { get; set; }
		public Image img { get; set; }
		public Thread LoadingThread { get; set; }

		public CachedImage( string filename )
		{
			sFilename = filename;
			img = null;
			LoadingThread = null;
		}
	}
}
