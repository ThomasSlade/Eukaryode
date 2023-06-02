using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.IO;

namespace Amino
{
	/// <summary>
	/// An extended <see cref="ContentManager"/> in which all content is stored by name, rather than path.
	/// </summary>
	public class ContentService
	{
		private ContentManager _contentManager;

		/// <summary> All asset paths in the content directory keyed by their filename (with the extension removed). </summary>
		private Dictionary<string, string> _assetPaths = new Dictionary<string, string>();

		public ContentService(ContentManager contentManager, string rootDirectory)
		{
			_contentManager = contentManager;
			_contentManager.RootDirectory = rootDirectory;
			Init();
		}

		protected virtual void Init()
		{
			foreach(string path in Directory.GetFiles(_contentManager.RootDirectory, "*.*", SearchOption.AllDirectories))
			{
				// Directory.GetFiles includes the root path in its result, so these strings are 'Content/Folder/SomeAsset.png'.
				// We just want 'Folder/SomeAsset.png'
				string relativePath = Path.GetRelativePath(_contentManager.RootDirectory, path);
				string pathWithoutExtension = Path.ChangeExtension(relativePath, null);
				string key = Path.GetFileName(pathWithoutExtension).ToLower();
				if(_assetPaths.ContainsKey(key))
				{
					throw new InvalidOperationException($"Cannot register content '{pathWithoutExtension}' because content '{_assetPaths[key]}' is already using its filename. All filemanes in the content directory must be unique.");
				}
				_assetPaths.Add(key, pathWithoutExtension);
			}
		}

		/// <summary> Load an asset of the given key, equal to the asset's filename (without the file extension). </summary>
		public T Load<T>(string assetName)
		{
			assetName = assetName.ToLower();
			if (!_assetPaths.TryGetValue(assetName, out string path))
			{
				throw new ArgumentException($"Asset of name '{assetName}' was not present in this content manager.", nameof(assetName));
			}
			return _contentManager.Load<T>(_assetPaths[assetName]);
		}

		/// <summary> Load all assets under the given directory. Assets must be loadable as type <see cref="T"/>. </summary>
		/// <param name="assetDirectory"> The content directory of the file. </param>
		public List<T> LoadAll<T>(string assetDirectory)
		{
			string[] directories = Directory.GetFiles(Path.Combine(_contentManager.RootDirectory, assetDirectory), "*.*");
			List<T> loaded = new List<T>(directories.Length);
			foreach(string path in directories)
			{
				string relativePath = Path.GetRelativePath(_contentManager.RootDirectory, path);
				loaded.Add(_contentManager.Load<T>(Path.ChangeExtension(relativePath, null)));
			}
			return loaded;
		}
	}
}
