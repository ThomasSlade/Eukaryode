using Microsoft.Xna.Framework.Content;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Amino
{
	/// <summary>
	/// An extended <see cref="ContentManager"/> which allows files under a specific folder to be considered resources,
	/// in which case they can be loaded just by name rather than by specifying the entire directory.
	/// </summary>
	public class ContentService
	{
		/// <summary> The folder under which resources are defined in content. </summary>
		private const string ResourcesFolder = "Resources";
		private static string ResourcesDirectory;
		private ContentManager _contentManager;

		/// <summary> All asset paths in the resource directory keyed by their filename (with the extension removed). </summary>
		private Dictionary<string, string> _resourcePaths = new Dictionary<string, string>();

		public ContentService(ContentManager contentManager, string rootDirectory)
		{
			_contentManager = contentManager;
			_contentManager.RootDirectory = rootDirectory;
			ResourcesDirectory = Path.Combine(rootDirectory, ResourcesFolder);
			Init();
		}

		/// <summary> Finds all resources under the Resource folder and keys them by their filename. </summary>
		protected virtual void Init()
		{
			if (!Directory.Exists(ResourcesDirectory))
			{
				return;
			}

			// Get all the files in the root directory, and determine what their key would be.
			// If these conflict with anything under the Resources folder, throw an error.
			string[] filesInRootContent = Directory.GetFiles(_contentManager.RootDirectory, "*.*");
			IEnumerable<string> filenames = filesInRootContent.ToList().Select(f => Path.GetFileName(Path.ChangeExtension(f, null)).ToLower());
			HashSet<string> keysInRootContent = new HashSet<string>(filenames);

			foreach (string path in Directory.GetFiles(ResourcesDirectory, "*.*", SearchOption.AllDirectories))
			{
				// Directory.GetFiles includes the root path in its result, so these strings are 'Content/Folder/SomeAsset.png'.
				// We just want 'Folder/SomeAsset.png'
				string relativePath = Path.GetRelativePath(_contentManager.RootDirectory, path);
				string pathWithoutExtension = Path.ChangeExtension(relativePath, null);
				string key = Path.GetFileName(pathWithoutExtension).ToLower();

				if (keysInRootContent.Contains(key))
				{
					throw new InvalidOperationException($"Cannot register resource '{pathWithoutExtension}' because a file with this name is already present in the content directory.");
				}

				if (_resourcePaths.ContainsKey(key))
				{
					throw new InvalidOperationException($"Cannot register resource '{pathWithoutExtension}' because resource '{_resourcePaths[key]}' is already using its filename. All filemanes in the folder '{ResourcesFolder}' must be unique.");
				}
				_resourcePaths.Add(key, pathWithoutExtension);
			}
		}

		/// <summary>
		/// Load an asset by either its resource key or by its path within the Content directory.
		/// </summary>
		public T Load<T>(string keyOrPath)
		{
			keyOrPath = keyOrPath.ToLower();
			if (!_resourcePaths.TryGetValue(keyOrPath, out string path))
			{
				return _contentManager.Load<T>(keyOrPath);
			}
			return _contentManager.Load<T>(path);
		}

		/// <summary>
		/// Load an asset by either its resource key or by its path within the Content directory.
		/// </summary>
		public bool TryLoad<T>(string keyOrPath, out T value)
		{
			value = default;
			try
			{
				value = Load<T>(keyOrPath);
				return true;
			}
			catch (ContentLoadException e)
			{
				return false;
			}
		}

		/// <summary> Load all assets under the given directory. Assets must be loadable as type <see cref="T"/>. </summary>
		/// <param name="assetDirectory"> The content directory of the file. </param>
		public List<T> LoadAllResources<T>(string assetDirectory)
		{
			string[] directories = Directory.GetFiles(Path.Combine(ResourcesDirectory, assetDirectory), "*.*");
			List<T> loaded = new List<T>(directories.Length);
			foreach (string path in directories)
			{
				string relativePath = Path.GetRelativePath(_contentManager.RootDirectory, path);
				loaded.Add(_contentManager.Load<T>(Path.ChangeExtension(relativePath, null)));
			}
			return loaded;
		}
	}
}
