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
using System.Threading.Tasks;
using System.IO;
using WebPWrapper;

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
			// Check and fill the cache
			CacheImage( sFilename );

			if( cache.ContainsKey( sFilename ) )
			{
				CachedImage ci = cache[sFilename];
				ci.LoadingTask?.Wait();
				return ci.img;
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
				ci.LoadingTask = Task.Run(() => LoadImage(ci));
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
				cache[sFilename].img.Dispose();
				cache.Remove( sFilename );
			}
		}

		/// <summary>
		/// Loads an image from disk into the supplied CachedImage object
		/// </summary>
		/// <param name="data">CachedImage object containg the filename to load</param>
		private static void LoadImage( object data )
		{
            bool done = false;
            int retry = 0;

            do
            {
                try
                {
					CachedImage ci = (CachedImage)data;

					if (Path.GetExtension(ci.sFilename) == ".webp")
					{
						using (WebP webp = new WebP())
							ci.img = webp.Load(ci.sFilename);
					}
					else
						ci.img = Image.FromFile(ci.sFilename);

                    done = true;
                }
                catch (Exception ex)
                {
                    System.Console.WriteLine("ImageCache ERROR: " + ex.ToString());
                    System.Console.WriteLine("ImageCache exception HResult: " + ex.HResult);

                    // only attempt retries if the reason for the failure was a sharing violation
                    // or a file not found error; otherwise assume the error is permanent
                    if (ex.HResult == unchecked((int)0x80070020) ||
                        ex.HResult == unchecked((int)0x80070002)) // ERROR_SHARING_VIOLATION or ERROR_FILE_NOT_FOUND
                    {
                        System.Console.WriteLine(" ==> ERROR_SHARING_VIOLATION.");
                    }
                    else
                    {
                        done = true;
                    }
                }
                if (!done)
                {
                    retry++;
                    System.Console.WriteLine(" failed to load image, attempting retry #" + retry);
                    // minor delay as workaround for moved files being occasionally still locked by the AV
                    System.Threading.Thread.Sleep(100);
                    
                }
            } while ((retry < 10) && (done == false));
        }
	}

	/// <summary>
	/// Class for holding images, with a reference to the thread that is loading it
	/// </summary>
	class CachedImage
	{
		public string sFilename { get; set; }
		public Image img { get; set; }
		public Task LoadingTask { get; set; }

		public CachedImage( string filename )
		{
			sFilename = filename;
			img = null;
			LoadingTask = null;
		}
	}
}
