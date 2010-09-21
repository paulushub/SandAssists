using System;
using System.IO;
using System.Windows.Forms;
using System.Collections.Generic;

namespace ICSharpCode.CodeEditor.Searching
{
    /// <summary>
    /// Maintains a list of currently added file extensions
    /// </summary>
    public sealed class IconListManager
    {
        private bool _includeBothSizes; //flag, used to determine whether to create two ImageLists.
        private ImageList _smallImages; //will hold ImageList objects
        private ImageList _largeImages; //will hold ImageList objects
        private IconReader.IconSize _iconSize;
        private Dictionary<string, int> _extensionList;

        private IconListManager()
        {
            _extensionList = new Dictionary<string, int>(
                StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Creates an instance of <c>IconListManager</c> that will add icons to a single <c>ImageList</c> using the
        /// specified <c>IconSize</c>.
        /// </summary>
        /// <param name="imageList"><c>ImageList</c> to add icons to.</param>
        /// <param name="iconSize">Size to use (either 32 or 16 pixels).</param>
        public IconListManager(ImageList imageList, IconReader.IconSize iconSize)
            : this()
        {
            // Initialize the members of the class that will hold the image list we're
            // targeting, as well as the icon size (32 or 16)
            _smallImages = imageList;
            if (_smallImages == null)
            {
                _smallImages = new ImageList();
            }

            _iconSize = iconSize;
        }

        /// <summary>
        /// Creates an instance of IconListManager that will add icons to two <c>ImageList</c> types. The two
        /// image lists are intended to be one for large icons, and the other for small icons.
        /// </summary>
        /// <param name="smallImageList">The <c>ImageList</c> that will hold small icons.</param>
        /// <param name="largeImageList">The <c>ImageList</c> that will hold large icons.</param>
        public IconListManager(ImageList smallImageList, ImageList largeImageList)
            : this()
        {
            //add both our image lists
            _smallImages = smallImageList;
            _largeImages = largeImageList;

            if (_smallImages == null)
            {
                _smallImages = new ImageList();
            }

            //set flag
            _includeBothSizes = (_largeImages != null);
        }

        /// <summary>
        /// Used internally, adds the extension to the hashtable, so that its value can then be returned.
        /// </summary>
        /// <param name="Extension"><c>String</c> of the file's extension.</param>
        /// <param name="ImageListPosition">Position of the extension in the <c>ImageList</c>.</param>
        private void AddExtension(string extension, int imageListPosition)
        {
            _extensionList.Add(extension, imageListPosition);
        }

        /// <summary>
        /// Called publicly to add a file's icon to the ImageList.
        /// </summary>
        /// <param name="filePath">Full path to the file.</param>
        /// <returns>Integer of the icon's position in the ImageList</returns>
        public int AddFileIcon(string filePath)
        {
            // Check if the file exists, otherwise, throw exception.
            if (!File.Exists(filePath))
                throw new FileNotFoundException("File does not exist");

            // Split it down so we can get the extension
            string[] splitPath = filePath.Split(new Char[] { '.' });
            string extension = (string)splitPath.GetValue(splitPath.GetUpperBound(0));

            //Check that we haven't already got the extension, if we have, then
            //return back its index
            if (_extensionList.ContainsKey(extension))
            {
                return _extensionList[extension];		//return existing index
            }
            else
            {
                // It's not already been added, so add it and record its position.

                int pos = _smallImages.Images.Count;		//store current count -- new item's index

                if (_includeBothSizes == true)
                {
                    //managing two lists, so add it to small first, then large
                    _smallImages.Images.Add(IconReader.GetFileIcon(
                        filePath, IconReader.IconSize.Small, false));
                    _largeImages.Images.Add(IconReader.GetFileIcon(
                        filePath, IconReader.IconSize.Large, false));
                }
                else
                {
                    //only doing one size, so use IconSize as specified in _iconSize.
                    _smallImages.Images.Add(IconReader.GetFileIcon(
                        filePath, _iconSize, false));	//add to image list
                }

                AddExtension(extension, pos);	// add to hash table
                return pos;
            }
        }

        /// <summary>
        /// Clears any <c>ImageLists</c> that <c>IconListManager</c> is managing.
        /// </summary>
        public void ClearLists()
        {
            if (_smallImages != null)
            {
                ImageList.ImageCollection listImages = _smallImages.Images;

                for (int i = 0; i < _smallImages.Images.Count; i++)
                {
                    listImages[i].Dispose();
                }
                _smallImages.Images.Clear();
            }
            if (_largeImages != null)
            {
                ImageList.ImageCollection listImages = _largeImages.Images;

                for (int i = 0; i < _smallImages.Images.Count; i++)
                {
                    listImages[i].Dispose();
                }
                _largeImages.Images.Clear();
            }

            _extensionList.Clear();			//empty hashtable of entries too.
        }
    }
}