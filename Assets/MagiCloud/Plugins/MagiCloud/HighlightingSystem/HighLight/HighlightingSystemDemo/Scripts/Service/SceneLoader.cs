using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.Callbacks;
using System.IO;
#endif

[DisallowMultipleComponent]
public class SceneLoader : MonoBehaviour
{
	static private readonly string descriptionDefault = "Use dropdown above to load different demonstration scenes";
	static private readonly string descriptionConstant = @"
Please refer to the documentation if you see z-fighting artifacts in this scene.

Controls: 
W, S, A, D, Q, E, C, Space, RMB drag - camera movement
LMB Click - toggle flashing
RMB click - toggle see-through mode
'1' - fade in/out constant highlighting
'2' - turn on/off constant highlighting immediately
'3' - turn off all types of highlighting immediately
";
	static private readonly Dictionary<string, string> descriptions = new Dictionary<string, string>()
	{
		{ "01 Welcome",		"" },
		{ "02 Colors",			"Any color on any background can be used for the highlighting." },
		{ "03 Transparency",	"Transparent materials highlighting is supported." },
		{ "04 Occlusion",		"Highlighting occlusion and see-through mode demo." },
		{ "05 OccluderModes",	"Two highlighting occluder modes available." },
		{ "06 Scrpting",		"Using Highlighting System from C#, JavaScript and Boo scripts." },
		{ "07 Compound",		"Any changes in highlightable objects is properly handled." },
		{ "08 Mobile",			"Simple scene for mobile devices." },
	};

	public List<string> sceneNames = new List<string>();

	public Text title;
	public Text description;
	public Dropdown dropdown;
	public Button previous;
	public Button next;

	#if UNITY_EDITOR
	private class SceneInfo
	{
		public string path;
		public string name;
	}

	[InitializeOnLoadMethod]
	static private void OnProjectLoadedInEditor()
	{
		// Get list of all scenes
		List<SceneInfo> allScenes = GetAllScenesAtPath("/HighlightingSystemDemo/Scenes");

		// Create list of missing demo scenes
		List<SceneInfo> missingScenes = GetMissingScenes(allScenes);

		// Ask user to add missing scenes
		EnsureMissingScenes(missingScenes);
	}

	//
	static private List<SceneInfo> GetAllScenesAtPath(string dirPath)
	{
		List<SceneInfo> result = new List<SceneInfo>();

		string dataPath = Application.dataPath;

		int startIndex = dataPath.Length - 6;	// - "Assets".Length

		DirectoryInfo dirInfo = new DirectoryInfo(dataPath + dirPath);
		if (dirInfo.Exists)
		{
			FileInfo[] files = dirInfo.GetFiles("*.unity", SearchOption.TopDirectoryOnly);

			for (int i = 0, l = files.Length; i < l; i++)
			{
				FileInfo file = files[i];

				SceneInfo sceneInfo = new SceneInfo();
				sceneInfo.path = file.FullName.Substring(startIndex).Replace('\\', '/');	// On Windows - Path.DirectorySeparatorChar is '\\', but Unity always uses '/'
				sceneInfo.name = Path.GetFileNameWithoutExtension(file.Name);
				result.Add(sceneInfo);
			}
		}

		return result;
	}

	//
	static private List<SceneInfo> GetMissingScenes(List<SceneInfo> allScenes)
	{
		List<SceneInfo> missingScenes = new List<SceneInfo>();

		EditorBuildSettingsScene[] existingScenes = EditorBuildSettings.scenes;

		for (int i = 0, l1 = allScenes.Count; i < l1; i++)
		{
			SceneInfo sceneInfo = allScenes[i];
			string scenePath = sceneInfo.path;
			bool sceneExist = false;
			for (int j = 0, l2 = existingScenes.Length; j < l2; j++)
			{
				EditorBuildSettingsScene scene = existingScenes[j];
				if (string.Equals(scene.path, scenePath))
				{
					sceneExist = true;
					break;
				}
			}

			if (!sceneExist)
			{
				missingScenes.Add(sceneInfo);
			}
		}

		return missingScenes;
	}

	//
	static private void EnsureMissingScenes(List<SceneInfo> missingScenes)
	{
		if (missingScenes.Count == 0) { return; }

		// Ask user to add missing scenes to the editor build settings
		string message = "Add these demo scenes to the editor build settings?\n";
		int l = missingScenes.Count;
		for (int i = 0; i < l; i++)
		{
			message += string.Format(i != l-1 ? "'{0}', " : "'{0}'.", missingScenes[i].name);
		}
		bool answer = EditorUtility.DisplayDialog("Highlighting System : SceneLoader", message, "Yes", "No");
		if (answer)
		{
			AddMissingScenes(missingScenes);

			if (EditorApplication.isPlayingOrWillChangePlaymode)
			{
				Debug.LogWarning("HighlightingSystem : List of scenes in build has changed. Please restart Play mode for these changes to take effect.");
			}
		}
	}

	//
	static private void AddMissingScenes(List<SceneInfo> missingScenes)
	{
		EditorBuildSettingsScene[] existingScenes = EditorBuildSettings.scenes;
		int l = existingScenes.Length;

		// Create new extended list of scenes and copy existing ones into it
		EditorBuildSettingsScene[] newScenes = new EditorBuildSettingsScene[l + missingScenes.Count];
		existingScenes.CopyTo(newScenes, 0);

		// Add missing scenes
		for (int i = 0; i < missingScenes.Count; i++)
		{
			newScenes[l + i] = new EditorBuildSettingsScene(missingScenes[i].path, true);
		}

		// Assign new scene list
		EditorBuildSettings.scenes = newScenes;
	}

	//
	[PostProcessSceneAttribute(2)]
	static public void OnPostProcessScene()
	{
		EditorBuildSettingsScene[] existingScenes = EditorBuildSettings.scenes;

		List<string> sceneNames = new List<string>();
		for (int i = 0, l = existingScenes.Length; i < l; i++)
		{
			EditorBuildSettingsScene scene = existingScenes[i];
			if (scene.enabled)
			{
				sceneNames.Add(Path.GetFileNameWithoutExtension(scene.path));
			}
		}

		SceneLoader[] sceneLoaders = FindObjectsOfType<SceneLoader>();
		for (int i = 0, l = sceneLoaders.Length; i < l; i++)
		{
			sceneLoaders[i].sceneNames = sceneNames;
		}
	}
	#endif

	//
	void Start()
	{
		int index = -1;

		// Fill scenes dropdown list
		List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
		string activeSceneName = SceneManager.GetActiveScene().name;
		sceneNames.Sort();
		for (int i = 0, l = sceneNames.Count; i < l; i++)
		{
			string sceneName = sceneNames[i];

			Dropdown.OptionData option = new Dropdown.OptionData(sceneName);
			options.Add(option);
			
			if (index == -1 && activeSceneName == sceneName)
			{
				index = i;
			}
		}
		dropdown.options = options;
		if (index != -1)
		{
			dropdown.value = index;
		}

		// 
		UpdateButtons();

		// Set scene title and description
		title.text = activeSceneName;
		string d;
		if (!descriptions.TryGetValue(activeSceneName, out d))
		{
			d = descriptionDefault;
		}
		d += descriptionConstant;
		description.text = d;
	}

	//
	void OnEnable()
	{
		dropdown.onValueChanged.AddListener(OnValueChanged);
		previous.onClick.AddListener(OnPrevious);
		next.onClick.AddListener(OnNext);
	}

	//
	void OnDisable()
	{
		dropdown.onValueChanged.RemoveListener(OnValueChanged);
		previous.onClick.RemoveListener(OnPrevious);
		next.onClick.RemoveListener(OnNext);
	}

	//
	void OnValueChanged(int index)
	{
		List<Dropdown.OptionData> options = dropdown.options;
		index = Mathf.Clamp(index, 0, options.Count - 1);

		string sceneName = options[index].text;
		string activeSceneName = SceneManager.GetActiveScene().name;
		if (!string.Equals(activeSceneName, sceneName))
		{
			SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
			UpdateButtons();
		}
	}

	//
	void OnPrevious()
	{
		OnValueChanged(dropdown.value - 1);
	}

	//
	void OnNext()
	{
		OnValueChanged(dropdown.value + 1);
	}

	//
	void UpdateButtons()
	{
		previous.interactable = dropdown.value > 0;
		next.interactable = dropdown.value < dropdown.options.Count - 1;
	}
}